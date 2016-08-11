#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using _dValueEnums;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    // <summary> (origonal -> CalculateValueArea)
    // The Value Area is the price range where 70% of yesterdays volume traded
	// Written by Ben L. at sbgtrading@yahoo.com
	//   Theory taken from several sources, but summarized at: http://www.secretsoftraders.com/ValueAreaHelpGuide.htm
	// May God bless you and your trading!
	//
	
	//	Description of the "ProfileType" parameter:

	//		I've given the option of creating the profile in 3 different ways:
	//			1)  VOC - This method loads all the volume of a bar onto the closing price of that bar.
	//						e.g.  A 5-minute bar has a volume of 280 and a range of 1.5 points with a close at 1534.25, then
	//						all 280 hits of volume are loaded onto the 1534.25 closing price.
	//			2)  TPO - This method disregards volume altogether, and gives a single hit to each price in the range of the bar.
	//						e.g.  A 5-minute bar has a range of 1.5 points with a High at 1534 and a Low at 1532.5, then
	//						1 contract (or "hit") is loaded to the seven prices in that range: 1532.50, 1532.75, 1533.0, 1533.25, 1533.50, 1533.75, and 1534
	//			3)  VWTPO - This method distribues the volume of a bar over the price range of the bar.
	//						e.g.  A 5-minute bar has a volume of 280 and a range of 1.5 points with a High at 1534 and a Low at 1532.5, then
	//						40 contracts (=280/7) are loaded to each of the seven prices in that range: 1532.50, 1532.75, 1533.0, 1533.25, 1533.50, 1533.75, and 1534
	//			4)  VTPO - This method distribues the volume of a bar Evenly over the price range of the bar.
	//						e.g.  A 5-minute bar has a volume of 280 and a range of 1.5 points with a High at 1534 and a Low at 1532.5, then
	//						280 contracts are loaded to each of the seven prices in that range: 1532.50, 1532.75, 1533.0, 1533.25, 1533.50, 1533.75, and 1534
	//						Since the calcs. are Relative to other bars / price points, more volume bars / price points will show that.
	//
	// Mods DeanV - 11/2008
	//	3/20/2010 - NT7 conversion (version 7.0)
	//		Adjusted call params for v7 requirements and non-equidistant charts, tweeked session detection, and re-positioned a few lables.
	//		Included a global enum namespace/file with distribution (_dValueEnums) so different versions don't have to deal with it).
	//	v7.0.1 - 3/23 - Added Session Template time override. Overwrites start / end settings with template in use settings.
	//	v7.0.2 - 3/28 - Added ZOrder flag... attempt to show behind other stuff.
	//	v7.0.3 - 4/07/10 - merged MDay's features... Map visable screen or combo's of days. Added / Changed these inputs
	//		ScreenMapType - 0=daily(daily maps), 1=screen(whatever is on screen), 2=combine days (uses PreviousSessions to determin # of days to combine)
	//		PreviousSessions - # of days to add to today's activity when ScreenMapType = 2. 0 = today only, 1 = today and yesterday.
	//		ShowDailyPlots - if true will show daily plot lines (regardless of SCreenMapType).
	//		* ShowEvolvingPOC's - added type 3 = Extended lines (full screen display of evolving lines in combo modes)
	//		* increase TotalSlots Maximum to 1000 (combo's probably need more slots for larger ranging)
	//		* combo's will include pre-sessions if part of chart display.
	//	v7.0.4 - 4/19-20 - Added Zero (Auto) setting on "Slot Min. Height".. 0 = fill in gaps between slots, etc., to display as contiguous verticals (no gaps or overprints)
	//		(when slots are combining ticks (TotalSlot<ticks in range & slot min. = 0), will increase POC & VA's buy about 1/2 tick... probably a little more accurate)
	//	v7.0.5 - 4/21 - Added -1 input (Auto slots seperated by 1 pix.) to "Slot Min. Height". -1 = separates slots by 1 pix.
	//		When "Slot Min. Height" <=0 - maps will adjust to center when slots are combining. Reworked POC, VA calc's to reflect centered slots.
	//
	//
	/// </summary>
    [Description("Plot Daily Profile Maps, POC and Value Areas.")]
    public class dValueArea : Indicator
    {
			
		#region Variables
        // Wizard generated variables
			private bool inclWeekendVol= false;
			private int openHour=8,openMinute=30;
			private double pctOfVolumeInVA = 0.70;
			private _dValueEnums.dValueAreaTypes profileType=_dValueEnums.dValueAreaTypes.VWTPO;
			private double sessionLengthInHours=6.75;
			private int totalSlots= 300;
			private int presentMethod= 2;	//0=no mapping (best for real time), 1,2 = too busy for real time.
        	private int showEvolving = 2;
			private Color ePOCColor = Color.Red;
			private Color eVAtColor = Color.Green;
			private Color eVAbColor = Color.Green;
			private int eTransparent = 40;
			private int eHeight = 2;
			private int sTransparent = 70;
			private Color sPColor = Color.Red;
			private Color sColor = Color.Lime;
		
		// User defined variables (add any user defined variables below)
			private double[,] PriceHitsArray;// = new double[2,500];
			private double TheSessionHigh,TheSessionLow;
			private double VAtop=0.0,VAbot=0.0,PriceOfPOC=0.0;
			private DateTime CSEndTime,CSStartTime;
		
        	private int minSlotHeight = 0;

		
		private IntSeries SesNum;
		private int sEndBnum = 0;
		private int sStartBnum = 0;	//first date bar# we care about
 		private SolidBrush		SessBrush, PreSessBrush, ePOCBrush, eVAtBrush, eVAbBrush;
		private double sessVAtop=0.0,sessVAbot=0.0,sessPriceOfPOC=0.0,sessMaxHits=0.0;
		private double eVAtop=0.0,eVAbot=0.0,ePriceOfPOC=0.0;	//evolving
		
		//make these global and set on plot call, so we don't have to pass around.
		private Rectangle PlotBounds;
		private double PlotMin, PlotMax;
		private double PlotMMDiff;

		private int TicksPerSlot;
		private int LastSlotUsed;
		private double SlotHalfRange;
		
		private int vACalcType = 2;
		private bool showRtPOC = false;
		
		private int rightScrnBnum;	//right most bar# on screen.
		private int leftScrnBnum;	//left most bar# in display
		private int screenPct = 100;
		private int screenPosition = 1;
		
		private int etDecimals = 2;
		private int etPosition = 0;
		private String textForm = "0.";
		
		private bool useSessTemplate = false;
		private bool zOrderBehind = false;
		
		private bool showDaily = true;
		private int screenMapType = 0;
		private int previousSessions = 2;

		#endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.LightGreen, PlotStyle.Dot, "VAt"));
            Add(new Plot(Color.Pink, PlotStyle.Dot, "VAb"));
            Add(new Plot(Color.Green, PlotStyle.Line, "POC"));
            Add(new Plot(Color.Chocolate, PlotStyle.Line, "RtPOC"));
			AutoScale			= false;
            CalculateOnBarClose	= true;
            Overlay				= true;
            PriceTypeSupported	= false;
			
			PriceHitsArray = new double[2,totalSlots];
			SesNum = new IntSeries(this, MaximumBarsLookBack.Infinite);
			CalcColors();
			
			for(int i = etDecimals; i>0 ;i--) textForm += "0";
			
			if(zOrderBehind) ZOrder	= -1; else ZOrder = 0;
			
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
 			if(CurrentBar<3) {
				if(useSessTemplate) {
					DateTime sessionBegin;
					DateTime sessionEnd;

					Bars.Session.GetNextBeginEnd(Time[0], out sessionBegin, out sessionEnd);
					openHour = sessionBegin.Hour;
					openMinute = sessionBegin.Minute;
					TimeSpan difftime = sessionEnd - sessionBegin;
					sessionLengthInHours = difftime.TotalHours;
					//Print("Next Session Start: " + sessionBegin + " Next Session End: " + sessionEnd + " Length in Hrs.: " + sessionLengthInHours);
				}
				CSStartTime = new DateTime(Time[0].Year,Time[0].Month,Time[0].Day,openHour,openMinute,0,0,DateTimeKind.Utc);
				CSEndTime = CSStartTime.AddHours(sessionLengthInHours);
				if(CurrentBar>1) SetCurSessEndTime();	//get it started (skip bad 1st bar data issue).
				SesNum.Set(0);
				return;
			}

			SesNum.Set(SesNum[1]);	//copy previous # for default.
			
			// (do on first (complete) bar, outside of session range. Show previous session.)
			if( Time[0].CompareTo(CSEndTime) > 0 ) {
				NewSessStart();
			}
			
			if(VAtop>0.0 && showDaily)
			{	VAt.Set(VAtop);
				VAb.Set(VAbot);
				POC.Set(PriceOfPOC);				
			}
			if(showRtPOC && showDaily) {	//recalc for real time.
				SetSessionBars(0);	//current session
				if(Time[0].CompareTo(CSStartTime) <= 0) SetPreSessBars(0);	// could be pre-session
				if(sEndBnum - sStartBnum == 0 ) return;	//must have at least 2 bars
				DetermineHL_BarRange();
				if(!StuffHits_BarRange()) return;
				RtPOC.Set(sessPriceOfPOC);				
			}

        }

        #region PlotOverride
		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{ 	//Gets here anytime a screen draw/redraw range is requested.
			// Default plotting in base class. This indicator has some, so call it.
			base.Plot(graphics, bounds, min, max);
			if(presentMethod < 1) return;
			if(BarsArray[0].Count <2) return;
			
			int x;
			
		//	int bars = Math.Min(ChartControl.LastBarPainted, ChartControl.BarsPainted);	//max bars that "could?" be in display.
			int bars = Math.Min(LastBarIndexPainted, ChartControl.BarsPainted);	//max bars that "could?" be in display.
			if( bars < 2) return;

		//	rightScrnBnum = Math.Min(ChartControl.LastBarPainted, CurrentBar); //limit to actual bars vs. white space on right.
		//	leftScrnBnum = Math.Min(ChartControl.FirstBarPainted, CurrentBar); //not bigger than CurrentBar (ie. only 1 bar on screen).
			rightScrnBnum = Math.Min(LastBarIndexPainted, CurrentBar); //limit to actual bars vs. white space on right.
			leftScrnBnum = Math.Min(FirstBarIndexPainted, CurrentBar); //not bigger than CurrentBar (ie. only 1 bar on screen).
						
			int leftMostSessNum = SesNum[(CurrentBar - leftScrnBnum)];
			int rightMostSessNum = SesNum[(CurrentBar - rightScrnBnum)];

			//set these globals 1x, before any calls that use them
			PlotBounds = bounds;
			PlotMin = min;
			PlotMax = max;
			PlotMMDiff = ChartControl.MaxMinusMin(PlotMax,PlotMin); //don't know why we do it this way?
			
			// Find lowest screen price for ref to calc slot pix's.
			double ChartLowPrice  = double.MaxValue;
			for (int i = leftScrnBnum; i <= rightScrnBnum; i++) {
				ChartLowPrice  = Math.Min(ChartLowPrice , Bars.Get(i).Low);
			}
			int ScreenVLowPos = GetYPos(ChartLowPrice);	//low value screen position

			//draw combo's or daily...
			if(screenPosition > 0 && screenMapType == 1) {
				//use bars on screen. Dispaly on right side.
				sEndBnum = (rightScrnBnum );
				sStartBnum = (leftScrnBnum);
								
				if(sEndBnum > sStartBnum) {	//Have some bars, should be OK.
					DetermineHL_BarRange();
				
					if(!StuffHits_BarRange()) return;
					
					//calc slot height for this BarRange (needed SlotHalfRange).
					int SlotVHeight = ScreenVLowPos - GetYPos(ChartLowPrice + (SlotHalfRange*2.0));//-1;	//at least 1
					if(SlotVHeight < 1) SlotVHeight = 1;
					
					DrawSlotsUV(graphics, SlotVHeight, 0, SessBrush);
				}
			}
			else if(screenPosition > 0 && screenMapType == 2) {
				//combine day's
				int oldestSnum = SesNum[0]-previousSessions;
				if(oldestSnum < 0) oldestSnum = 0;
				SetSessBarDays(previousSessions);
				//SetSessionBars(SesNum[0] - x);
				if(sEndBnum > sStartBnum) {	//regular session should be OK.
					DetermineHL_BarRange();
				
					if(!StuffHits_BarRange()) return;
					
					//calc slot height for this BarRange.
					int SlotVHeight = ScreenVLowPos - GetYPos(ChartLowPrice + (SlotHalfRange*2.0));//-1;	//at least 1
					if(SlotVHeight < 1) SlotVHeight = 1;
					
					DrawSlotsUV(graphics, SlotVHeight, oldestSnum, SessBrush);

				}	
			}
			//daily stuff...
			else if (screenMapType == 0 && leftMostSessNum <= SesNum[0]) {
				//current or previous session is in the display.
				//loop through screen dipslay day history				
				for(x = leftMostSessNum; x <= rightMostSessNum;x++) {
					//do pre-session first
					if(presentMethod == 2) {
						SetSessionBars(SesNum[0] - x);	//have to do this first.
						SetPreSessBars(SesNum[0] - x);

						if(sEndBnum > sStartBnum) {	//1st bar of presession, or presess hasn't started yet.

							DetermineHL_BarRange();	//using previous call.
							if(!StuffHits_BarRange()) continue;
							
							int SlotVHeight = ScreenVLowPos - GetYPos(ChartLowPrice + (SlotHalfRange*2.0));
							if(SlotVHeight < 1) SlotVHeight = 1;	//at least 1
							
							
							DrawSlotsUV(graphics, SlotVHeight, x, PreSessBrush);
							if(showEvolving > 1 && x == SesNum[0]) {
								if(eVAtColor != Color.Transparent) DrawEvolving(graphics,  SesNum[0], eVAtBrush ,sessVAtop);
								if(ePOCColor != Color.Transparent) DrawEvolving(graphics,  SesNum[0], ePOCBrush ,sessPriceOfPOC);
								if(eVAbColor != Color.Transparent) DrawEvolving(graphics,  SesNum[0], eVAbBrush ,sessVAbot);
								}
						}
						
					}
					//now do defined session
					SetSessionBars(SesNum[0] - x);
					if(sEndBnum > sStartBnum) {	//regular session should be OK.
						DetermineHL_BarRange();	//using previous call.
					
						if(!StuffHits_BarRange()) return;
						
						//calc slot height for this BarRange.
						int SlotVHeight = ScreenVLowPos - GetYPos(ChartLowPrice + (SlotHalfRange*2.0));
						if(SlotVHeight < 1) SlotVHeight = 1;
						
						DrawSlotsUV(graphics, SlotVHeight, x, SessBrush);

						if(showEvolving > 1 && x == SesNum[0]) {
							if(eVAtColor != Color.Transparent) DrawEvolving(graphics,  SesNum[0], eVAtBrush ,sessVAtop);
							if(ePOCColor != Color.Transparent) DrawEvolving(graphics,  SesNum[0], ePOCBrush ,sessPriceOfPOC);
							if(eVAbColor != Color.Transparent) DrawEvolving(graphics,  SesNum[0], eVAbBrush ,sessVAbot);
							}
						}
					
					//do text for either pre or reg session...
					if(showEvolving == 1 && x == SesNum[0]) {
						DrawTextValue(graphics, eVAtBrush, sessVAtop);
						DrawTextValue(graphics, ePOCBrush, sessPriceOfPOC);
						DrawTextValue(graphics, eVAbBrush, sessVAbot);
					}
				
				}
				
			}
		
		}
        #endregion

		private void NewSessStart() {
			//it's a new session, increment and calc POC from yesterday.
			if(!inclWeekendVol && (Time[1].DayOfWeek==DayOfWeek.Saturday || Time[1].DayOfWeek==DayOfWeek.Sunday)) return;
			
			SetSessionBars(0);	//calc for just completed (1 session ago) session
			//Do this in any case to correct start,end times.
			SetCurSessEndTime();	//get current session End date/time, so we can compare on next bar, regardless.
			//cancel session increment with no live hrs. Sess bars
			if(sEndBnum - sStartBnum == 0 ) {	//nothing in Previous session
				//Print(Time[0] +" " +CurrentBar+" " +SesNum[0] +" No Live Session"  +" " +sStartBnum +" "+sEndBnum);
				return;
			}
			
			//increment session number.
			//SesNum's switch on first bar after last bar of defined session.
			//  They include any pre / post bars displayed, as well as open hours.
			SesNum.Set(SesNum[0] + 1);
	
			SetSessionBars(1);	//calc for Real (current bar excluded), just completed (1 session ago) session
		
			DetermineHL_BarRange();	//using previous call.
			if(!StuffHits_BarRange()) return;

			VAtop = sessVAtop;
			VAbot = sessVAbot;
			PriceOfPOC = sessPriceOfPOC;
		
		}
		
        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries VAt
        {
            get { return Values[0]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries VAb
        {
            get { return Values[1]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries POC
        {
            get { return Values[2]; }
        }

		[Browsable(false)]
        [XmlIgnore()]
        public DataSeries RtPOC		//Real Time POC
        {
            get { return Values[3]; }
        }

		[Description("Include Weekend Volume")]
        [Category("Parameters")]
		[Gui.Design.DisplayName ("\t\t\t\t\tInclWeekendVol")]
        public bool InclWeekendVol
        {
            get { return inclWeekendVol; }
			set { inclWeekendVol = value;}
        }
		
		[Description("Use Session Template for Open Hours and Length (Changes Input settings).")]
		[Gui.Design.DisplayName ("\t\t\t\tUseSessTemplate")]
        [Category("Parameters")]
        public bool UseSessTemplate
        {
            get { return useSessTemplate; }
			set { useSessTemplate = value;}
        }
        [Description("Market open hour (as Integer) IN 24HR FORMAT")]
		[Gui.Design.DisplayName ("\t\t\tOpenHour")]
        [Category("Parameters")]
        public int OpenHour
        {
            get { return openHour; }
			set { openHour = Math.Max(0,value);}
        }
        [Description("Market open minute (as Integer)")]
		[Gui.Design.DisplayName ("\t\tOpenMinute")]
        [Category("Parameters")]
        public int OpenMinute
        {
            get { return openMinute; }
			set { openMinute = Math.Max(0,value);}
        }
        [Description("Session length (as Decimal, in hours) (Max = <24)")]
		[Gui.Design.DisplayName ("\tSessionLengthInHours")]
        [Category("Parameters")]
        public double SessionLengthInHours
        {
            get { return sessionLengthInHours; }
			set { sessionLengthInHours = value;}
        }

		[Description("Type of profile (VOC = Volume at Close, TPO = Price, VWTPO = Volume Weighted Price, VTPO = Volume)")]
        [Category("Parameters")]
        public _dValueEnums.dValueAreaTypes ProfileType
        {
            get { return profileType; }
			set { profileType = value; }
        }
		
        [Description("Percent of volume within Value Area (0.01 - 1.0)")]
        [Category("Parameters")]
        public double PctOfVolumeInVA
        {
            get { return pctOfVolumeInVA; }
			set { pctOfVolumeInVA = Math.Max(0.01,Math.Min(1.0,value));}
        }
        [Description("Total Number Slots to Calc. / display for each day (3 - 1000) (value is aprox. middle of each slot)")]
        [Category("Parameters")]
        public int TotalSlots
        {
            get { return totalSlots; }
			set { totalSlots = Math.Max(3,Math.Min(1000,value));}
        }
        [Description("How to (show) Value Maps. 0 = No Mapping), 1 = Sess. Only, 2 = Both Pre-Sess. and Sess.")]
        [Category("Parameters")]
        public int PresentMethod
        {
            get { return presentMethod; }
			set { presentMethod = Math.Max(0,Math.Min(5,value));}
        }
        [Description("Map daily, Screen summary, or Combine Day's back (0=daily, 1=screen, 2=day's back (using PreviousSessions setting)")]
        [Category("Parameters")]
        public int ScreenMapType
        {
            get { return screenMapType; }
			set { screenMapType = Math.Max(0,Math.Min(2,value));}
        }
		
		
		
 		
		[Description("Evolving Display Method (0 = No, 1 = Text, 2 = Lines, 3 = Extended lines (combo's only))")]
        [Category("Display Settings")]
		[Gui.Design.DisplayName ("\t\t\t\t\t\t\t\t\t\t\tShow Evolving POCs")]
        public int ShowEvolving
        {
            get { return showEvolving; }
			set { showEvolving = Math.Max(0,Math.Min(3,value));}
        }

		[Description("Evolving Text Decimals (0 - 6)")]
        [Category("Display Settings")]
		[Gui.Design.DisplayName ("\t\t\t\t\t\t\t\t\t\t\tE-Text Decimals")]
        public int EtDecimals
        {
            get { return etDecimals; }
			set { etDecimals = Math.Max(0,Math.Min(6,value));}
        }

		[Description("Evolving Text Position. Shift left edge x-Bars from last bar (+-100, Neg=toLeft, Pos=toRight)")]
        [Category("Display Settings")]
		[Gui.Design.DisplayName ("\t\t\t\t\t\t\t\t\t\t\tE-Text Position")]
        public int EtPosition
        {
            get { return etPosition; }
			set { etPosition = Math.Max(-100,Math.Min(100,value));}
        }

		[Description("Evolving Line Thickness (1 - 50) (extend downward, Value is at top of band.)")]
        [Category("Display Settings")]
		[Gui.Design.DisplayName ("\t\t\t\t\t\t\t\t\t\tEvolving Line Height")]
        public int EHeight
        {
            get { return eHeight; }
			set { eHeight = Math.Max(1,Math.Min(50,value));}
        }

		[Description("Evolving Line / Text Transparancy (0 - 100)")]
        [Category("Display Settings")]
		[Gui.Design.DisplayName ("\t\t\t\t\t\t\t\t\tE Line Transparancy")]
        public int ETransparent
        {
            get { return eTransparent; }
			set { eTransparent = Math.Max(0,Math.Min(100,value));}
        }

		[XmlIgnore()]
		[Description("Color for Evolving VA Top")]
		[Category("Display Settings")]
		[Gui.Design.DisplayName ("\t\t\t\t\t\t\t\tEvolving VAt Color")]
		public Color EVAtColor
		{
			get { return eVAtColor; }
			set { eVAtColor = value; }
		}
		// Serialize our Color object
		[Browsable(false)]
		public string EVAtColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(eVAtColor); }
			set { eVAtColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		[XmlIgnore()]
		[Description("Color for Evolving POC")]
		[Category("Display Settings")]
		[Gui.Design.DisplayName ("\t\t\t\t\t\t\tEvolving POC Color")]
		public Color EPOCColor
		{
			get { return ePOCColor; }
			set { ePOCColor = value; }
		}
		// Serialize our Color object
		[Browsable(false)]
		public string EPOCColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(ePOCColor); }
			set { ePOCColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[XmlIgnore()]
		[Description("Color for Evolving VA Bottom")]
		[Category("Display Settings")]
		[Gui.Design.DisplayName ("\t\t\t\t\t\tEvolving VAb Color")]
		public Color EVAbColor
		{
			get { return eVAbColor; }
			set { eVAbColor = value; }
		}
		// Serialize our Color object
		[Browsable(false)]
		public string EVAbColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(eVAbColor); }
			set { eVAbColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}


		
		[Description("Minimum Slot Thickness  (-1 - 50) (0 = Auto) (-1 = Separated Auto)")]
        [Category("Display Settings")]
		[Gui.Design.DisplayName ("\t\t\t\t\tSlot Min. Height")]
        public int SlotMinHeight
        {
            get { return minSlotHeight; }
			set { minSlotHeight = Math.Max(-1,Math.Min(50,value));}
        }
		
		[XmlIgnore()]
		[Description("Slot Open Hrs. Color")]
        [Category("Display Settings")]
		[Gui.Design.DisplayName ("\t\t\t\tSlot Session Color")]
		public Color SColor
		{
			get { return sColor; }
			set { sColor = value; }
		}
		// Serialize our Color object
		[Browsable(false)]
		public string SColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(sColor); }
			set { sColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[Description("Slot Pre-Session Color")]
        [Category("Display Settings")]
		[Gui.Design.DisplayName ("\t\t\tSlot Pre-Sess Color")]
		public Color SPColor
		{
			get { return sPColor; }
			set { sPColor = value; }
		}
		// Serialize our Color object
		[Browsable(false)]
		public string sPColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(sPColor); }
			set { sPColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		[Description("Slot Transparancy (0 - 100)")]
        [Category("Display Settings")]
		[Gui.Design.DisplayName ("\t\tSlot Transparancy")]
        public int STransparent
        {
            get { return sTransparent; }
			set { sTransparent = Math.Max(0,Math.Min(100,value));}
        }

		[Description("Show Real Time POC")]
        [Category("Display Settings")]
		[Gui.Design.DisplayName ("\tReal Time POC")]
        public bool ShowRtPOC
        {
            get { return showRtPOC; }
			set { showRtPOC = value;}
        }
		
		[Description("Show Behind other stuff. (Disables ZOrder, may have to manually re-set ZOrder when turned to false)")]
        [Category("Display Settings")]
		[Gui.Design.DisplayName ("\tZOrder Put Behind")]
        public bool ZOrderBehind
        {
            get { return zOrderBehind; }
			set { zOrderBehind = value;}
        }
		
        [Description("Old (midrange based) or New (POC / Slot based) VA calc type (1=old, 2=new).")]
        [Category("Parameters")]
        public int VACalcType
        {
            get { return vACalcType; }
            set { vACalcType = Math.Max(1,Math.Min(2,value));}
        }

        [Description("Slot Display Max-Width Percent ( (10-100) percent of screen area).")]
        [Category("Parameters")]
        public int ScreenPercent
        {
            get { return screenPct; }
            set { screenPct = Math.Max(10,Math.Min(100,value));}
        }
		
        [Description("Slot Base Position (1=Left, 2=Right).")]
        [Category("Parameters")]
        public int ScreenPosition
        {
            get { return screenPosition; }
            set { screenPosition = Math.Max(1,Math.Min(2,value));}
        }
		
        [Description("Show Daily POC and VA Line Plots")]
        [Category("Parameters")]
        public bool ShowDailyPlots
        {
            get { return showDaily; }
            set { showDaily = value;}
        }
		
        [Description("Previous Sessions (Days) to combine when ScreenMapType=1. (0=today only, 1 = today and yesterday)")]
        [Category("Parameters")]
        public int PreviousSessions
        {
            get { return previousSessions; }
			set { previousSessions = Math.Max(0,value);}
        }

		#endregion
//===================================================================================
//================ DeanV mod/add stuff below.
		
		private void SetSessBarDays(int DaysAgo) {
			//find start/end bars going back DaysAgo days (end is always CurrentBar)
			// days are really SesNum's.
			//have to have a default. Usefull if sessionago == 0.
			int sNum = SesNum[0] - DaysAgo;	//last sess num we car about
			if(sNum < 0) sNum = 0;			//not less than zero.
		
			sEndBnum = sStartBnum = CurrentBar;
			//now find first sNum bar, looking forwards
			for(int x = CurrentBar-1 ; x >0 ; x--) {	//start at oldest barago and go forwards
				if(SesNum[x] == sNum) {	// first bar of session range (could be pre-session)
						sStartBnum = CurrentBar - x;	//combo's include pre-session, if part of chart.
						break;	//end
				}
			}
		}
		
		private void SetPreSessBars(int SessionAgo) {	//reg session has to be set before this call.
			int sNum = SesNum[0] - SessionAgo;
			int x = 1;
			
			//we (MUST) have session bars, so just continue backwards...
			
			sEndBnum = sStartBnum -1;
			//start before sess start barago and go backwards
			for(x = CurrentBar - sEndBnum ; x < CurrentBar; x++) {
				if(SesNum[x] != sNum) break;
			}
			sStartBnum = CurrentBar - x +1;
			//EBnum will be -1 from SBnum, if no pre-ses bars found.

		}
	
		private void SetSessionBars(int SessionAgo) {
			//have to have a default. Usefull if sessionago == 0.
			DateTime StartTime = CSStartTime;
			DateTime EndTime;
			int sNum = SesNum[0] - SessionAgo;
		
			if(sNum == SesNum[0] || sNum < 0) {	//not a previous session
				sEndBnum = sStartBnum = CurrentBar;
				for(int x = 0 ; x < CurrentBar; x++) {	//start at newest barago and go backwards
					if(Time[x].CompareTo(StartTime) <= 0) {	// 1 bar too far.
						sStartBnum = CurrentBar - x +1;	//don't need pre-session bars.
						break;	//end
					}
				if(SesNum[x] != SesNum[0]) break;	//ses hasn't started yet.
				}

			}
			else { //find and set pervious session date/times and barnumber range we care about.
				sStartBnum = 1;
				sEndBnum = 0;
				int y = 0;
				for(int x = 1 ; x < CurrentBar; x++) {	//start at newest barago and go backwards to catch right date
					if(sEndBnum == 0 && SesNum[x] == sNum) {	// last bar of session range
						//skip weekends? (Weekends are included with Mondays sess# when skipped)
						sEndBnum = CurrentBar - x;// +1;
						//use last bar of session count to set correct date/times of session.
						StartTime = new DateTime(Time[x].Year,Time[x].Month,Time[x].Day,openHour,openMinute,0,0,DateTimeKind.Utc);
						if(Time[x].CompareTo(StartTime) < 0) //it's a previous day, so subtract 24 hrs
							StartTime = StartTime.AddHours(-24);
						//keep going till we find the ses start.
						EndTime = StartTime.AddHours(sessionLengthInHours);
						if(Time[x+1].CompareTo(EndTime) > 0)  {	//no sess bars showing
							sStartBnum = sEndBnum;
							break;
						}
						for(y = x ; y < CurrentBar; y++) {
							if(Time[y].CompareTo(StartTime) <= 0) {
								if(SesNum[y] == sNum)
									sStartBnum = CurrentBar - y +1;
								else sStartBnum = sEndBnum;
								break;
							}
						}
					}
					if(sEndBnum > 0 && Time[x].CompareTo(StartTime) <= 0) {	// 1 bar too far.
						sStartBnum = CurrentBar - x +1;	//don't need pre-session bars.
						break;	//end
					}
				}
				if(sEndBnum == 0) sEndBnum = CurrentBar;	//safty if havn't got there yet.
			}
		}

		private void DetermineHL_BarRange() {
			// use bar number range from SetSessionTimes(>0).
			int sBarsAgo= CurrentBar - sStartBnum; //first bar to check in barsago.
			int eBarsAgo= CurrentBar - sEndBnum; //last bar to check in barsago.
			
			TheSessionHigh = High[eBarsAgo];
			TheSessionLow = Low[eBarsAgo];
						
			for(int x = eBarsAgo ; x <= sBarsAgo; x++) {
				if(High[x] > TheSessionHigh) TheSessionHigh =High[x];
				if(Low[x] < TheSessionLow)   TheSessionLow  =Low[x];
			}
		}

		private bool StuffHits_BarRange() {
			int x;
			
			int TicksInRange = (int) Math.Round((TheSessionHigh-TheSessionLow)/TickSize,0);
			
			//fit ticks into array by so many TicksPerSlot
			TicksPerSlot = (TicksInRange / totalSlots) +1;	//should just drop the fract part.
			LastSlotUsed = TicksInRange / TicksPerSlot;	//Zero based, drop fract part.
			
			double comboSlotOffset;
			if(minSlotHeight > 0 ) {
				SlotHalfRange = ((TickSize * TicksPerSlot) - TickSize) /2;
				comboSlotOffset = (TicksPerSlot > 1 ?  SlotHalfRange : 0);
			}
			else {
				SlotHalfRange = ((TickSize * TicksPerSlot) ) /2;
				comboSlotOffset = (TicksPerSlot > 1 ?  SlotHalfRange - (((TheSessionLow+((LastSlotUsed+1) * TickSize * TicksPerSlot)) -TheSessionHigh)/2) : 0);	//move down to center it.
			}

			//clear counts in any case.
			for(x=0;x<=LastSlotUsed;x++) {	// 0 -> 999, reset from bottom up.
				 	//PriceHitsArray[0,x]=(x*TickSize*TicksPerSlot) + SlotHalfRange; //Lowest Tick Value/Slot upped to mid value point
				 	//PriceHitsArray[0,x]=(x*TickSize*TicksPerSlot) + (TicksPerSlot > 1 ? SlotHalfRange : 0); //Lowest Tick Value/Slot upped to mid value point
				 	PriceHitsArray[0,x]=(x*TickSize*TicksPerSlot) +  comboSlotOffset; //Lowest Tick Value/Slot upped to mid value point
				 	PriceHitsArray[0,x] += TheSessionLow; //add it to the bottom
					PriceHitsArray[1,x]=0.0;	//clear counts per value.
			}
			
			if( TicksInRange > 0) {	//do it
				
				int sBarsAgo= CurrentBar - sStartBnum; //first bar to check in barsago.
				int eBarsAgo= CurrentBar - sEndBnum; //last bar to check in barsago.

				int index=0;
				int i=eBarsAgo;
				double BarH;
				double BarL;
				while (i <= sBarsAgo) { //Accumulate the volume/hits for each previous days bar into PriceVolume array
					BarH=High[i]; BarL=Low[i];

					if(profileType == _dValueEnums.dValueAreaTypes.VOC) {//Volume On Close - puts all the volume for that bar on the close price
						index = (int) Math.Round((Close[i]-TheSessionLow)/TickSize,0);
						index /= TicksPerSlot;	//drop fract part.
						PriceHitsArray[1,index] += Volume[i];
					}
					if (profileType == _dValueEnums.dValueAreaTypes.TPO) {//Time Price Opportunity - disregards volume, only counts number of times prices are touched
						//BarH=High[i]; BarL=Low[i];
						while(BarL <= BarH) {
							index = (int) Math.Round((BarL-TheSessionLow)/TickSize,0);	//ticks from bottom
							index /= TicksPerSlot;	//drop fract part.
							PriceHitsArray[1,index] += 1;	//increment this value count.
							BarL = BarL + TickSize;	//up 1 tick
						}
					}
					if (profileType == _dValueEnums.dValueAreaTypes.VWTPO) {//Volume Weighted Time Price Opportunity - Disperses the Volume of the bar over the range of the bar so each price touched is weighted with volume
						//BarH=High[i]; BarL=Low[i];
						int TicksInBar = (int) Math.Round((BarH-Low[i])/TickSize+1,0);
						while(BarL<= BarH) {
							index = (int) Math.Round((BarL-TheSessionLow)/TickSize,0);
							index /= TicksPerSlot;	//drop fract part.
							PriceHitsArray[1,index] += Volume[i]/TicksInBar;
							BarL = BarL + TickSize;
						}
					}
					if (profileType == _dValueEnums.dValueAreaTypes.VTPO) {//Volume  Time Price Opportunity - Counts raw Volume
						//BarH=High[i]; BarL=Low[i];
						while(BarL<= BarH) {
							index = (int) Math.Round((BarL-TheSessionLow)/TickSize,0);
							index /= TicksPerSlot;	//drop fract part.
							PriceHitsArray[1,index] += (Volume[i]);
							BarL = BarL + TickSize;
						}
					}
					i++;
				}
				//arrays are stuffed.
				
				//Calculate the Average price as weighted by the hit counts AND find the price with the highest hits (POC price)
				i=0;
				double THxP=0.0; //Total of Hits multiplied by Price at that volume
				double HitsTotal=0.0;
				sessPriceOfPOC=0.0;
				sessMaxHits = 0.0;
				int POCIndex = 0;	//track POC slot#
				
				double midPoint = TheSessionLow + ((TheSessionHigh-TheSessionLow) *.5);
				double midUpCnt=0,midDnCnt=0;	//counts above/below midpoint.
				
				while(i<=LastSlotUsed) {	//Sum up Volume*Price in THxP...and sum up Volume in VolumeTotal
					if(PriceHitsArray[1,i]>0.0) {
						THxP += (PriceHitsArray[1,i] * PriceHitsArray[0,i]);
						HitsTotal += PriceHitsArray[1,i];
						if(PriceHitsArray[1,i] > sessMaxHits) { //used to determine POC level
							sessMaxHits = PriceHitsArray[1,i]; 
							sessPriceOfPOC = PriceHitsArray[0,i];
							POCIndex = i;	//OK if only 1
						}
						//sum up hits for possable later use
						if(PriceHitsArray[0,i] > midPoint) midUpCnt += PriceHitsArray[1,i];	//don't count equals
						if(PriceHitsArray[0,i] < midPoint) midDnCnt += PriceHitsArray[1,i];
					}
					i++;
				}
				if(HitsTotal == 0) return false;	//nothing to do.
				
				//now lowest (or only) sessMaxHits/POC is known.
				//determine in others match, and pick best choice.
				//
				//Rules to use are:
				// 1. If there is more than 1 price with the same 'most' TPO's then the price closest to the mid-point of the range (high - low) is used. 
				// 2. If the 2 'most' TPO prices are equi-distance from the mid-point then the price on the side of the mid-point with the most TPO's is used. 
				// 3. If there are equal number of TPO's on each side then the lower price is used. 
				//
				int POCcount = 0;
				double mid1=midPoint,mid2=midPoint;	//Distance from midPoint, set larger than possable
				int mid1Dx = 0, mid2Dx = 0;	//array index count of finds.
				
				i=0;
				while(i<=LastSlotUsed) {	//scan known array from bottom to top
					if(PriceHitsArray[1,i] == sessMaxHits) {	//should be at least 1.
						POCcount++;
						//find 2 closest to midpoint
						if(Math.Abs(midPoint - PriceHitsArray[0,i]) <= mid1) {
							mid2 = mid1;//rotate next closest
							mid2Dx = mid1Dx;
							mid1 = Math.Abs(midPoint - PriceHitsArray[0,i]);	//how far away from midpoint
							mid1Dx = i;
						}
					}
					i++;
				}
				if(POCcount > 1) {
					if(mid1 != mid2) {	//found it, rule #1
						sessPriceOfPOC = PriceHitsArray[0,mid1Dx];
						POCIndex = mid1Dx;
					}
					else {	//they are equal, Rule #2 may apply
						if(midUpCnt == midDnCnt) {	//must use Rule #3
							sessPriceOfPOC = PriceHitsArray[0,mid2Dx];	//use the lower.
							POCIndex = mid2Dx;
						}
						else {	//Rule #2
							if(midUpCnt > midDnCnt) {
								sessPriceOfPOC = PriceHitsArray[0,mid1Dx];	//mid1 = upper of 2
								POCIndex = mid1Dx;
							}
							else {
								sessPriceOfPOC = PriceHitsArray[0,mid2Dx];	//must be the lower.
								POCIndex = mid2Dx;
							}
						}//end Rule #2
					}
				}
				//end of finding best fit for POC
				

				if(vACalcType == 1) {	//start mid-range and expand
					//AvgPrice = THxP/HitsTotal;
					sessVAtop=THxP/HitsTotal;
					sessVAbot=sessVAtop;
	
					//This loop calculates the percentage of hits contained within the Value Area
					double ViA=0.0;
					double TV=0.00001;
					double Adj=0.0;
					while(ViA/TV < pctOfVolumeInVA) {
						sessVAbot = sessVAbot - Adj;
						sessVAtop = sessVAtop + Adj;
						ViA=0.0;
						TV=0.00001;
						for(i=0;i<=LastSlotUsed;i++) {
							if(PriceHitsArray[0,i] > sessVAbot-Adj && PriceHitsArray[0,i] < sessVAtop+Adj) ViA += PriceHitsArray[1,i];
							TV += PriceHitsArray[1,i];
						}
						Adj=TickSize;
					}
				}
				else {	//start at POC Slot and expand by slots.
					//start at POC and add largest of each side till done.
					double ViA=PriceHitsArray[1,POCIndex];
					int upSlot = POCIndex+1, dnSlot = POCIndex-1;
					double upCnt,dnCnt;
					
					while(ViA/HitsTotal < pctOfVolumeInVA) {
						if(upSlot <= LastSlotUsed) upCnt = PriceHitsArray[1,upSlot]; else upCnt =0;
						if(dnSlot >= 0) dnCnt = PriceHitsArray[1,dnSlot]; else dnCnt =0;
						if(upCnt == dnCnt) {	//if both equal, add this one.
							if(POCIndex - dnSlot < upSlot - POCIndex) upCnt = 0;	//use closest
							else dnCnt = 0;	//use upper if the same.
						}
						if(upCnt >= dnCnt) {	//if still equal (i.e. zero), do it.
							ViA += upCnt;
							if(upSlot <= LastSlotUsed) upSlot++;
						}
						if(upCnt <= dnCnt) {	//need equals to increment counts.
							ViA += dnCnt;
							if(dnSlot >= 0) dnSlot--;
						}
						if(upSlot > LastSlotUsed && dnSlot < 0) {	//error.
							upSlot = LastSlotUsed;
							dnSlot = 0;
							break;
						}
					}
					//index's have gone one too far...
					sessVAtop=PriceHitsArray[0,--upSlot];
					sessVAbot=PriceHitsArray[0,++dnSlot];
							
				}
				
			}	//ticksinRange
			return true;
		}
		
		private void SetCurSessEndTime() {
			//if(Time[0].CompareTo(CSEndTime)<=0) 
			//	return;
			
			double slenAdd = sessionLengthInHours>=24 ? 24.0-1.0/60.0 : sessionLengthInHours;
			CSEndTime = CSStartTime.AddHours(slenAdd);
			while(CSEndTime.CompareTo(Time[0])< 0) {	//already happened, so add days for catch (weekends)
				CSEndTime = CSEndTime.AddHours(24);
				CSStartTime = CSStartTime.AddHours(24);
			}
		}


//======== graphic stuff ==========
		private void CalcColors() 
		{
			int	alpha	 = (int) (255.0 * ((100.0 - sTransparent) / 100.0));

			SessBrush   = new SolidBrush(Color.FromArgb(alpha, sColor.R, sColor.G, sColor.B));
			PreSessBrush     = new SolidBrush(Color.FromArgb(alpha, sPColor.R, sPColor.G, sPColor.B));
			
			//now set evolving brushes
			alpha	 = (int) (255.0 * ((100.0 - eTransparent) / 100.0));
			
			ePOCBrush   = new SolidBrush(Color.FromArgb(alpha, ePOCColor.R, ePOCColor.G, ePOCColor.B));
			eVAtBrush   = new SolidBrush(Color.FromArgb(alpha, eVAtColor.R, eVAtColor.G, eVAtColor.B));
			eVAbBrush   = new SolidBrush(Color.FromArgb(alpha, eVAbColor.R, eVAbColor.G, eVAbColor.B));
		}

		//this version uses per-set globals from Plot().
		private int GetYPos(double price) {
			int TotalHeight = PlotBounds.Y + PlotBounds.Height;
			
			int ret =  (int) ((TotalHeight) - ((price - PlotMin) / PlotMMDiff) * PlotBounds.Height);
			return ret;
		}
		
		private void DrawEvolving(Graphics graphics, int ThisSesNum, SolidBrush eBrush, double ePrice) {
			int x,yPos,vPos,vHeight;
			double price;
			int HWidth ;
			
			int barPaintWidth = ChartControl.ChartStyle.GetBarPaintWidth(ChartControl.BarWidth);
			int halfBarWidth = (int)(barPaintWidth*0.5);
			
			int sbarNum = leftScrnBnum;	//default is off screen to left
			int barsInRange = 0;
			for(x = leftScrnBnum; x <= rightScrnBnum; x++) {	//scan left to right
				if(SesNum[CurrentBar -x] == ThisSesNum) {
					if(presentMethod > 0 && x < sStartBnum) continue; //wait till we get an open session bar.
					if(presentMethod ==2 && x > sEndBnum ) break; //stop wen we get an open session bar if sEB says it's a presession.
					barsInRange++;
				}
				if(barsInRange ==1 ) sbarNum = x;	//found first valid bar	
				}
			if(--barsInRange < 1) return;	//have at least 1 bar to paint. Do paint last bar.
			
			int ebarNum = sbarNum + barsInRange;
			
			int leftMostPos = ChartControl.GetXByBarIdx(BarsArray[0], sbarNum) - halfBarWidth ;
			int rightMostPos = ChartControl.GetXByBarIdx(BarsArray[0], ebarNum) + halfBarWidth ;

			if( sEndBnum+1 >= CurrentBar ) 
				rightMostPos = PlotBounds.Width;
			
			int TotalHeight = PlotBounds.Y + PlotBounds.Height;

			price = ePrice;
			HWidth   = (int) ((rightMostPos - leftMostPos));
			
			yPos =  (int) ((TotalHeight) - ((price - PlotMin) / PlotMMDiff) * PlotBounds.Height);
			vPos = yPos + eHeight;
			if (yPos >= TotalHeight) return;	//too low or high to display
			if (vPos <=1) vPos = 1;
			
			//take out any negitive portion
			yPos += (yPos < 0 ? yPos*-1 : 0);
			
			if(vPos > TotalHeight) vPos = TotalHeight;
			vHeight = vPos - yPos;

			graphics.FillRectangle(eBrush, new Rectangle(leftMostPos, yPos, HWidth, vHeight));
			

			
		}
		
		//draw slots universal
		// left or right based, and percentage of area
		private void DrawSlotsUV(Graphics graphics, int slotHeight, int ThisSesNum, SolidBrush theBrush) {
			int x,yPos,vPos,vHeight;
			double price;
			int HWidth ;
			int HalfslotHeight = 0;
			
			//if(slotHeight < minSlotHeight) HalfslotHeight = minSlotHeight /2;
			if(minSlotHeight > 0 && slotHeight < minSlotHeight) HalfslotHeight = minSlotHeight /2;
				
			int barPaintWidth = ChartControl.ChartStyle.GetBarPaintWidth(ChartControl.BarWidth);
			int halfBarWidth = (int)(barPaintWidth*0.5);

			int sbarNum = leftScrnBnum;	//default is left most screen bar.
			int barsInRange = 0;
			
			//determin barsInRage based on screenMapType..
			if(screenMapType == 1) {	//on-screen
				barsInRange = rightScrnBnum - leftScrnBnum;
			}
			else if(screenMapType == 2) {	//day's combined
				barsInRange = rightScrnBnum - leftScrnBnum;
				if(SesNum[CurrentBar -leftScrnBnum] < ThisSesNum) { //find first onscreen bar
					for(x = leftScrnBnum; x <= rightScrnBnum; x++) {	//scan left to right
						if(SesNum[CurrentBar -x] == ThisSesNum) {
							sbarNum = x;
							x = rightScrnBnum+1;	//break
						}
						else barsInRange--;
					}
				}		
			}
			else {	//daily only			
				for(x = leftScrnBnum; x <= rightScrnBnum; x++) {	//scan left to right
					if(SesNum[CurrentBar -x] == ThisSesNum) {
						if(presentMethod > 0 && x < sStartBnum) continue; //wait till we get an open session bar.
						if(presentMethod ==2 && x > sEndBnum ) break; //stop wen we get an open session bar if sEB says it's a presession.
						barsInRange++;
					}
					if(barsInRange ==1 ) sbarNum = x;	//found first valid bar	
				}
			}//end screenMapType
			
			
			
			if(--barsInRange < 1) return;	//have at least 1 bar to paint. Don't paint last bar.
			int ebarNum = sbarNum + barsInRange;
			
			int leftMostPos = ChartControl.GetXByBarIdx(BarsArray[0], sbarNum) - (ScreenPosition==1 ? halfBarWidth : 0);
			int rightMostPos = ChartControl.GetXByBarIdx(BarsArray[0], ebarNum) + (ScreenPosition==2 ? halfBarWidth : 0);
			//adjust right screen display area if combo draws.
			if(screenMapType >0) {
				rightMostPos = PlotBounds.Width;
			}
			
			//now for some draw stuff...
			int TotalHeight = PlotBounds.Y + PlotBounds.Height;
			
			//reduce pix to show % amount of it.
			price = ((rightMostPos - leftMostPos) * ((100-screenPct)*0.01));	//temp usage of price
			//reset left or right depending on screenposition..
			if(ScreenPosition==1) rightMostPos -= (int)price;
			else leftMostPos += (int)price;
			int TotalWidth = rightMostPos - leftMostPos;

			int prevYpos = 0;
			for(x=0;x<=LastSlotUsed;x++) {
				price = PriceHitsArray[0,x] + SlotHalfRange;	//take it from mid to top.
				HWidth   = (int) ((TotalWidth) * (PriceHitsArray[1,x] / ( sessMaxHits )));
				
				yPos =  GetYPos(price);
				
				vPos = yPos  + slotHeight + HalfslotHeight;
				if(minSlotHeight == 0 && x != 0 ) vPos = prevYpos;
				if(minSlotHeight == -1 && x != 0 ) vPos = prevYpos-1;
				
				yPos = yPos - HalfslotHeight;
				prevYpos = yPos;	//incase we continue here.
				if (yPos >= TotalHeight || vPos <= 1) continue;	//too low or high to display
				
				//take out any negitive portion
				yPos += (yPos < 0 ? yPos*-1 : 0);
				
				if(vPos > TotalHeight) vPos = TotalHeight;
				vHeight = vPos - yPos;
				prevYpos = yPos;
	
				if(ScreenPosition==1) graphics.FillRectangle(theBrush, new Rectangle(leftMostPos, yPos, HWidth, vHeight));
				else graphics.FillRectangle(theBrush, new Rectangle(leftMostPos +(TotalWidth- HWidth), yPos, HWidth, vHeight));
			}
			
			//draw evolving here if not daily (screenMapType > 0)
			if(screenMapType > 0) {
				if(showEvolving >= 2) {
					int leftPos = leftMostPos;	//defaults are screen %
					int rightWidth = TotalWidth;
					
					if(showEvolving == 3) {	//show evolving lines full screen
						leftPos = PlotBounds.Left;
						rightWidth = PlotBounds.Width;
					}
					if(ePOCColor != Color.Transparent) graphics.FillRectangle(eVAtBrush, new Rectangle(leftPos, GetYPos(sessVAtop), rightWidth, eHeight));
					if(eVAtColor != Color.Transparent) graphics.FillRectangle(ePOCBrush, new Rectangle(leftPos, GetYPos(sessPriceOfPOC), rightWidth, eHeight));
					if(eVAbColor != Color.Transparent) graphics.FillRectangle(eVAbBrush, new Rectangle(leftPos, GetYPos(sessVAbot), rightWidth, eHeight));
					
				}
				else if(showEvolving == 1) {
					DrawTextValue(graphics, eVAtBrush, sessVAtop);
					DrawTextValue(graphics, ePOCBrush, sessPriceOfPOC);
					DrawTextValue(graphics, eVAbBrush, sessVAbot);
				}
				
			}
			
		}
		
		private void DrawTextValue(Graphics graphics, SolidBrush eBrush, double eValue) {
			int yPos = GetYPos(eValue) - (int)(ChartControl.Font.GetHeight() * 0.5);
			int rightMostPos = ChartControl.GetXByBarIdx(BarsArray[0], Bars.Count + etPosition);
			
			graphics.DrawString(eValue.ToString(textForm),ChartControl.Font,eBrush,rightMostPos,yPos);
		}

//===================================================================================
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private dValueArea[] cachedValueArea = null;

        private static dValueArea checkdValueArea = new dValueArea();

        /// <summary>
        /// Plot Daily Profile Maps, POC and Value Areas.
        /// </summary>
        /// <returns></returns>
        public dValueArea dValueArea(bool inclWeekendVol, int openHour, int openMinute, double pctOfVolumeInVA, int presentMethod, int previousSessions, _dValueEnums.dValueAreaTypes profileType, int screenMapType, int screenPercent, int screenPosition, double sessionLengthInHours, bool showDailyPlots, int totalSlots, bool useSessTemplate, int vACalcType)
        {
            return dValueArea(Input, inclWeekendVol, openHour, openMinute, pctOfVolumeInVA, presentMethod, previousSessions, profileType, screenMapType, screenPercent, screenPosition, sessionLengthInHours, showDailyPlots, totalSlots, useSessTemplate, vACalcType);
        }

        /// <summary>
        /// Plot Daily Profile Maps, POC and Value Areas.
        /// </summary>
        /// <returns></returns>
        public dValueArea dValueArea(Data.IDataSeries input, bool inclWeekendVol, int openHour, int openMinute, double pctOfVolumeInVA, int presentMethod, int previousSessions, _dValueEnums.dValueAreaTypes profileType, int screenMapType, int screenPercent, int screenPosition, double sessionLengthInHours, bool showDailyPlots, int totalSlots, bool useSessTemplate, int vACalcType)
        {
            if (cachedValueArea != null)
                for (int idx = 0; idx < cachedValueArea.Length; idx++)
                    if (cachedValueArea[idx].InclWeekendVol == inclWeekendVol && cachedValueArea[idx].OpenHour == openHour && cachedValueArea[idx].OpenMinute == openMinute && Math.Abs(cachedValueArea[idx].PctOfVolumeInVA - pctOfVolumeInVA) <= double.Epsilon && cachedValueArea[idx].PresentMethod == presentMethod && cachedValueArea[idx].PreviousSessions == previousSessions && cachedValueArea[idx].ProfileType == profileType && cachedValueArea[idx].ScreenMapType == screenMapType && cachedValueArea[idx].ScreenPercent == screenPercent && cachedValueArea[idx].ScreenPosition == screenPosition && Math.Abs(cachedValueArea[idx].SessionLengthInHours - sessionLengthInHours) <= double.Epsilon && cachedValueArea[idx].ShowDailyPlots == showDailyPlots && cachedValueArea[idx].TotalSlots == totalSlots && cachedValueArea[idx].UseSessTemplate == useSessTemplate && cachedValueArea[idx].VACalcType == vACalcType && cachedValueArea[idx].EqualsInput(input))
                        return cachedValueArea[idx];

            lock (checkdValueArea)
            {
                checkdValueArea.InclWeekendVol = inclWeekendVol;
                inclWeekendVol = checkdValueArea.InclWeekendVol;
                checkdValueArea.OpenHour = openHour;
                openHour = checkdValueArea.OpenHour;
                checkdValueArea.OpenMinute = openMinute;
                openMinute = checkdValueArea.OpenMinute;
                checkdValueArea.PctOfVolumeInVA = pctOfVolumeInVA;
                pctOfVolumeInVA = checkdValueArea.PctOfVolumeInVA;
                checkdValueArea.PresentMethod = presentMethod;
                presentMethod = checkdValueArea.PresentMethod;
                checkdValueArea.PreviousSessions = previousSessions;
                previousSessions = checkdValueArea.PreviousSessions;
                checkdValueArea.ProfileType = profileType;
                profileType = checkdValueArea.ProfileType;
                checkdValueArea.ScreenMapType = screenMapType;
                screenMapType = checkdValueArea.ScreenMapType;
                checkdValueArea.ScreenPercent = screenPercent;
                screenPercent = checkdValueArea.ScreenPercent;
                checkdValueArea.ScreenPosition = screenPosition;
                screenPosition = checkdValueArea.ScreenPosition;
                checkdValueArea.SessionLengthInHours = sessionLengthInHours;
                sessionLengthInHours = checkdValueArea.SessionLengthInHours;
                checkdValueArea.ShowDailyPlots = showDailyPlots;
                showDailyPlots = checkdValueArea.ShowDailyPlots;
                checkdValueArea.TotalSlots = totalSlots;
                totalSlots = checkdValueArea.TotalSlots;
                checkdValueArea.UseSessTemplate = useSessTemplate;
                useSessTemplate = checkdValueArea.UseSessTemplate;
                checkdValueArea.VACalcType = vACalcType;
                vACalcType = checkdValueArea.VACalcType;

                if (cachedValueArea != null)
                    for (int idx = 0; idx < cachedValueArea.Length; idx++)
                        if (cachedValueArea[idx].InclWeekendVol == inclWeekendVol && cachedValueArea[idx].OpenHour == openHour && cachedValueArea[idx].OpenMinute == openMinute && Math.Abs(cachedValueArea[idx].PctOfVolumeInVA - pctOfVolumeInVA) <= double.Epsilon && cachedValueArea[idx].PresentMethod == presentMethod && cachedValueArea[idx].PreviousSessions == previousSessions && cachedValueArea[idx].ProfileType == profileType && cachedValueArea[idx].ScreenMapType == screenMapType && cachedValueArea[idx].ScreenPercent == screenPercent && cachedValueArea[idx].ScreenPosition == screenPosition && Math.Abs(cachedValueArea[idx].SessionLengthInHours - sessionLengthInHours) <= double.Epsilon && cachedValueArea[idx].ShowDailyPlots == showDailyPlots && cachedValueArea[idx].TotalSlots == totalSlots && cachedValueArea[idx].UseSessTemplate == useSessTemplate && cachedValueArea[idx].VACalcType == vACalcType && cachedValueArea[idx].EqualsInput(input))
                            return cachedValueArea[idx];

                dValueArea indicator = new dValueArea();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.InclWeekendVol = inclWeekendVol;
                indicator.OpenHour = openHour;
                indicator.OpenMinute = openMinute;
                indicator.PctOfVolumeInVA = pctOfVolumeInVA;
                indicator.PresentMethod = presentMethod;
                indicator.PreviousSessions = previousSessions;
                indicator.ProfileType = profileType;
                indicator.ScreenMapType = screenMapType;
                indicator.ScreenPercent = screenPercent;
                indicator.ScreenPosition = screenPosition;
                indicator.SessionLengthInHours = sessionLengthInHours;
                indicator.ShowDailyPlots = showDailyPlots;
                indicator.TotalSlots = totalSlots;
                indicator.UseSessTemplate = useSessTemplate;
                indicator.VACalcType = vACalcType;
                Indicators.Add(indicator);
                indicator.SetUp();

                dValueArea[] tmp = new dValueArea[cachedValueArea == null ? 1 : cachedValueArea.Length + 1];
                if (cachedValueArea != null)
                    cachedValueArea.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachedValueArea = tmp;
                return indicator;
            }
        }
    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// Plot Daily Profile Maps, POC and Value Areas.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.dValueArea dValueArea(bool inclWeekendVol, int openHour, int openMinute, double pctOfVolumeInVA, int presentMethod, int previousSessions, _dValueEnums.dValueAreaTypes profileType, int screenMapType, int screenPercent, int screenPosition, double sessionLengthInHours, bool showDailyPlots, int totalSlots, bool useSessTemplate, int vACalcType)
        {
            return _indicator.dValueArea(Input, inclWeekendVol, openHour, openMinute, pctOfVolumeInVA, presentMethod, previousSessions, profileType, screenMapType, screenPercent, screenPosition, sessionLengthInHours, showDailyPlots, totalSlots, useSessTemplate, vACalcType);
        }

        /// <summary>
        /// Plot Daily Profile Maps, POC and Value Areas.
        /// </summary>
        /// <returns></returns>
        public Indicator.dValueArea dValueArea(Data.IDataSeries input, bool inclWeekendVol, int openHour, int openMinute, double pctOfVolumeInVA, int presentMethod, int previousSessions, _dValueEnums.dValueAreaTypes profileType, int screenMapType, int screenPercent, int screenPosition, double sessionLengthInHours, bool showDailyPlots, int totalSlots, bool useSessTemplate, int vACalcType)
        {
            return _indicator.dValueArea(input, inclWeekendVol, openHour, openMinute, pctOfVolumeInVA, presentMethod, previousSessions, profileType, screenMapType, screenPercent, screenPosition, sessionLengthInHours, showDailyPlots, totalSlots, useSessTemplate, vACalcType);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Plot Daily Profile Maps, POC and Value Areas.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.dValueArea dValueArea(bool inclWeekendVol, int openHour, int openMinute, double pctOfVolumeInVA, int presentMethod, int previousSessions, _dValueEnums.dValueAreaTypes profileType, int screenMapType, int screenPercent, int screenPosition, double sessionLengthInHours, bool showDailyPlots, int totalSlots, bool useSessTemplate, int vACalcType)
        {
            return _indicator.dValueArea(Input, inclWeekendVol, openHour, openMinute, pctOfVolumeInVA, presentMethod, previousSessions, profileType, screenMapType, screenPercent, screenPosition, sessionLengthInHours, showDailyPlots, totalSlots, useSessTemplate, vACalcType);
        }

        /// <summary>
        /// Plot Daily Profile Maps, POC and Value Areas.
        /// </summary>
        /// <returns></returns>
        public Indicator.dValueArea dValueArea(Data.IDataSeries input, bool inclWeekendVol, int openHour, int openMinute, double pctOfVolumeInVA, int presentMethod, int previousSessions, _dValueEnums.dValueAreaTypes profileType, int screenMapType, int screenPercent, int screenPosition, double sessionLengthInHours, bool showDailyPlots, int totalSlots, bool useSessTemplate, int vACalcType)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.dValueArea(input, inclWeekendVol, openHour, openMinute, pctOfVolumeInVA, presentMethod, previousSessions, profileType, screenMapType, screenPercent, screenPosition, sessionLengthInHours, showDailyPlots, totalSlots, useSessTemplate, vACalcType);
        }
    }
}
#endregion
