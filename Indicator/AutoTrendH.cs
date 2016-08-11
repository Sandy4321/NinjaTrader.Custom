// 
// Copyright (C) 2007, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//
#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Version 2.01 12/13/2012 Bugfix: (Dots not removed when showHistory set to false)
	/// This is a modification to AutoTrendLine that has trendline and trend breaks historys enabled along with some signaling.
	/// Version 2.0 11/21/2012 (DerekPhelps22@gmail.com for bug reports) Added option to enable trend line view history
	/// Usage:	AutoTrendH(bool: alertOnBreak, bool: showHistory, int: strength).TrendPrice 	::returns price at trendline at bar position
	/// 		AutoTrendH(bool: alertOnBreak, bool: showHistory, int: strength).Signal 		::0 = no signal, 1 = Buy signal on break of down trend line, -1 = Sell signal on break of up trend line
	/// 		AutoTrendH(bool: alertOnBreak, bool: showHistory, int: strength).Direction 		::0 = Undefined  1=active up trend w/price above, -1=active down trend w/price below
	/// 		
    /// </summary>
    [Description("AutoTrendLine with History option")]
    [Gui.Design.DisplayName("AutoTrendH")]
    public class AutoTrendH : Indicator
    {
        #region Variables
			private bool	alertOnBreak = true;
            private int 	strength 		= 5; 		// Default setting for Strength
			private bool	showHistory		= true;		//Show trend line history
        	private Color 	downTrendColor 	= Color.Red;
			private Color 	upTrendColor 	= Color.Green;
			private Color 	downHistColor 	= Color.Red;
			private Color 	upHistColor	 	= Color.Green;
			
			private int 	triggerBarIndex = 0;
			private int 	signal 			= 0; 				// 0 = no signal, 1 = buy signal on down trend break, -1 = sell signal on up trend break
			private int     direction		= 0;				// 0 = Undefined or broken trend, 1=active up trend, -1=active down trend
			private double	trendPrice		= 0;
			private int     lineWidth 		= 1;
			private int 	lineCount 		= 0;
			private double	startBarPriceOld= 0;
		#endregion

		protected override void Initialize()
        {
			DisplayInDataBox 	= false;
            CalculateOnBarClose	= true;
            Overlay				= true;
            PriceTypeSupported	= false;
        }

		protected override void OnBarUpdate()
        {
	//DETERMINE LOCATION OF LAST UP/DOWN TREND LINES	
			signal = 0;
			int upTrendOccurence 		= 1;	int upTrendStartBarsAgo		= 0;	int upTrendEndBarsAgo 		= 0;
			int downTrendOccurence 		= 1;	int	downTrendStartBarsAgo	= 0;	int	downTrendEndBarsAgo 	= 0;
		// Only calculate new autotrend line if ray hasent been put into manual mode by unlocking current ray
		if ( ((DrawObjects["UpTrendRay"] ==null) || (DrawObjects["UpTrendRay"].Locked)) && ((DrawObjects["DownTrendRay"] ==null) || (DrawObjects["DownTrendRay"].Locked)) ) 
//		if (  (DrawObjects["UpTrendRay"].Locked) || (DrawObjects["DowntrendTrendRay"].Locked) ) 
		{//Only do the following if existing ray is in auto mode	
		// Calculate up trend line
			upTrendOccurence 		= 1;	
			while (Low[upTrendEndBarsAgo] <= Low[upTrendStartBarsAgo])
			{	upTrendStartBarsAgo 	= Swing(strength).SwingLowBar(0, upTrendOccurence + 1, CurrentBar);
				upTrendEndBarsAgo 	= Swing(strength).SwingLowBar(0, upTrendOccurence, CurrentBar);
				if (upTrendStartBarsAgo < 0 || upTrendEndBarsAgo < 0)
					break;
				upTrendOccurence++;
			}
		// Calculate down trend line	
			downTrendOccurence 		= 1;
			while (High[downTrendEndBarsAgo] >= High[downTrendStartBarsAgo])
			{	downTrendStartBarsAgo 		= Swing(strength).SwingHighBar(0, downTrendOccurence + 1, CurrentBar);
				downTrendEndBarsAgo 		= Swing(strength).SwingHighBar(0, downTrendOccurence, CurrentBar);
				if (downTrendStartBarsAgo < 0 || downTrendEndBarsAgo < 0)
					break;
				downTrendOccurence++;
			}
		 }	
		// Clear out arrows that mark trend line breaks unless ShowHistory flag is true
			if (!showHistory) RemoveDrawObject("DownTrendBreak");							
			if (!showHistory) RemoveDrawObject("UpTrendBreak");
			
	//PROCESS UPTREND LINE IF CURRENT
			if (upTrendStartBarsAgo > 0 && upTrendEndBarsAgo > 0 && upTrendStartBarsAgo < downTrendStartBarsAgo)
			{	RemoveDrawObject("DownTrendRay");
				double startBarPrice 	= Low[upTrendStartBarsAgo];
				double endBarPrice 		= Low[upTrendEndBarsAgo];
				double changePerBar 	= (endBarPrice - startBarPrice) / (Math.Abs(upTrendEndBarsAgo - upTrendStartBarsAgo));
			//Test to see if this is a new trendline and increment lineCounter if so.
				if (startBarPrice!=startBarPriceOld)	
				{	direction=1;  //Signal that we have a new uptrend and put dot on trendline where new trend detected
					if (showHistory)	DrawDot(CurrentBar.ToString(), true, 0, startBarPrice+(upTrendStartBarsAgo*changePerBar), UpTrendColor);
					lineCount=lineCount+1;	
					triggerBarIndex = 0;
					ResetAlert("Alert");
				}
				startBarPriceOld=startBarPrice;
				//
			// Draw the up trend line
			// If user has unlocked the ray use manual rays position instead of auto generated positions to track ray position
				if ( (DrawObjects["UpTrendRay"] !=null) && (!DrawObjects["UpTrendRay"].Locked) )
				{	IRay upTrendRay		= (IRay) DrawObjects["UpTrendRay"];	
					startBarPrice 		= upTrendRay.Anchor1Y;
					endBarPrice 		= upTrendRay.Anchor2Y;
					upTrendStartBarsAgo = upTrendRay.Anchor1BarsAgo;
					upTrendEndBarsAgo   = upTrendRay.Anchor2BarsAgo;
					changePerBar 		= (endBarPrice - startBarPrice)/(Math.Abs(upTrendRay.Anchor2BarsAgo-upTrendRay.Anchor1BarsAgo));
					upTrendRay.Pen.DashStyle=DashStyle.Dash;
					upTrendRay.Pen.Color=Color.Blue;
				}
				else
				{	DrawRay("UpTrendRay", false, upTrendStartBarsAgo, startBarPrice, upTrendEndBarsAgo, endBarPrice, UpTrendColor, DashStyle.Solid, lineWidth);
				}
			//Draw the history line that will stay persistent on chart using lineCounter to establish a unique name
				if (showHistory) DrawLine("HistoryLine"+lineCount.ToString(), false, upTrendStartBarsAgo, startBarPrice, 0, startBarPrice+(upTrendStartBarsAgo*changePerBar), UpHistColor, DashStyle.Solid, lineWidth);
		//SET RETURN VALUES FOR INDICATOR
			// Check for an uptrend line break
				trendPrice=(startBarPrice+(upTrendStartBarsAgo*changePerBar));
				for (int barsAgo = upTrendEndBarsAgo - 1; barsAgo >= 0; barsAgo--) 
				{	if (Close[barsAgo] < endBarPrice + (Math.Abs(upTrendEndBarsAgo - barsAgo) * changePerBar))
					{	if (showHistory) 
						{	DrawArrowDown("UpTrendBreak"+lineCount.ToString(), barsAgo, High[barsAgo] + TickSize, downTrendColor); }
						else
						{	DrawArrowDown("UpTrendBreak", barsAgo, High[barsAgo] + TickSize, downTrendColor); }
			// Set the break signal only if the break is on the right most bar
						if (barsAgo == 0)
							signal = -1;
			// Alert will only trigger in real-time
						if (AlertOnBreak && triggerBarIndex == 0)
						{	triggerBarIndex = CurrentBar - upTrendEndBarsAgo;
							Alert("Alert", Priority.High, "Up trend line broken", "Alert2.wav", 100000, Color.Black, Color.Red);
						}
						break;
			}	}	}


			else 
	//DETECT AND PROCESS DOWNTREND LINE	IF CURRENT	
			if (downTrendStartBarsAgo > 0 && downTrendEndBarsAgo > 0  && upTrendStartBarsAgo > downTrendStartBarsAgo)
			{	RemoveDrawObject("UpTrendRay");
				double startBarPrice 	= High[downTrendStartBarsAgo];
				double endBarPrice 		= High[downTrendEndBarsAgo];
				double changePerBar 	= (endBarPrice - startBarPrice) / (Math.Abs(downTrendEndBarsAgo - downTrendStartBarsAgo));
			//Test to see if this is a new trendline and increment lineCount if so.
				if (startBarPrice!=startBarPriceOld)	
				{	direction=-1;		//signl that we have a new downtrend
					if (showHistory)	DrawDot(CurrentBar.ToString(), true, 0, startBarPrice+(downTrendStartBarsAgo*changePerBar), DownTrendColor);
					lineCount=lineCount+1;	
					triggerBarIndex = 0;
					ResetAlert("Alert");
				}
				startBarPriceOld=startBarPrice;
				//
			// Draw the down trend line
				// If user has unlocked the ray use manual rays position instead
				if ( (DrawObjects["DownTrendRay"] !=null) && (!DrawObjects["DownTrendRay"].Locked) )
				{	IRay downTrendRay	= (IRay) DrawObjects["DownTrendRay"];	
					startBarPrice 		= downTrendRay.Anchor1Y;
					endBarPrice 		= downTrendRay.Anchor2Y;;
					downTrendStartBarsAgo = downTrendRay.Anchor1BarsAgo;
					downTrendEndBarsAgo   = downTrendRay.Anchor2BarsAgo;
					changePerBar 		= (endBarPrice - startBarPrice)/(Math.Abs(downTrendRay.Anchor2BarsAgo-downTrendRay.Anchor1BarsAgo));
					downTrendRay.Pen.DashStyle=DashStyle.Dash;
					downTrendRay.Pen.Color=Color.Blue;

				}
				else			
				{	DrawRay("DownTrendRay", false, downTrendStartBarsAgo, startBarPrice, downTrendEndBarsAgo, endBarPrice, DownTrendColor, DashStyle.Solid, lineWidth);
				}
				if (showHistory) DrawLine("HistoryLine"+lineCount.ToString(), false, downTrendStartBarsAgo, startBarPrice, 0, startBarPrice+(downTrendStartBarsAgo*changePerBar), downHistColor, DashStyle.Solid, lineWidth);
		//SET RETURN VALUES FOR INDICATOR
			// Check for a down trend line break
				trendPrice=(startBarPrice+(downTrendStartBarsAgo*changePerBar));
				for (int barsAgo = downTrendEndBarsAgo - 1; barsAgo >= 0; barsAgo--) 
				{//	direction=-1;
					if (Close[barsAgo] > endBarPrice + (Math.Abs(downTrendEndBarsAgo - barsAgo) * changePerBar))
					{	if (showHistory) 
						{	DrawArrowUp("DownTrendBreak"+lineCount.ToString(), barsAgo, Low[barsAgo] - TickSize, upTrendColor); }
						else
						{	DrawArrowUp("DownTrendBreak", barsAgo, Low[barsAgo] - TickSize, upTrendColor); }
					// Set the break signal only if the break is on the right most bar
						if (barsAgo == 0)
							signal = 1;
					// Alert will only trigger in real-time
						if (AlertOnBreak && triggerBarIndex == 0)
						{	triggerBarIndex = CurrentBar - downTrendEndBarsAgo;
							Alert("Alert", Priority.High, "Down trend line broken", "Alert2.wav", 100000, Color.Black, Color.Green);
						}
						break;
					}
				}
			}		
        }

        #region Properties

	[Description("Generates a visual and audible alert on a trend line break")]
        [Category("Parameters")]
        public bool AlertOnBreak
        {
            get { return alertOnBreak; }
            set { alertOnBreak = value; }
        }
		
		[Description("Number of bars required on each side swing pivot points used to connect the trend lines")]
        [Category("Parameters")]
        public int Strength
        {
            get { return strength; }
            set { strength = Math.Max(1, value); }
        }

		[Description("Show Historical Trendlines & Breaks")]
        [Category("Parameters")]
        public bool ShowHistory
        {
            get { return showHistory; }
            set { showHistory = value; }
        }
		
		/// <summary>
		/// Gets the trade signal. 0 = no signal, 1 = Buy signal on break of down trend line, -1 = Sell signal on break of up trend line
		/// </summary>
		/// <returns></returns>
		public int Signal
		{
			get { Update(); return signal; }
		}
		/// <summary>
		/// Gets Trend Direction
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public int Direction
		{
			get { Update(); return direction; }
		}

		/// <summary>
		/// Gets TrendPrice
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public double TrendPrice
		{
			get { Update(); return trendPrice; }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore()]
		[Description("Color of the down trend line.")]
		[Category("Colors")]
		[Gui.Design.DisplayNameAttribute("Down trend")]
		public Color DownTrendColor
		{
			get { return downTrendColor; }
			set { downTrendColor = value; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		public string DownTrendColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(downTrendColor); }
			set { downTrendColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[XmlIgnore()]
		[Description("Color of the up trend line.")]
		[Category("Colors")]
		[Gui.Design.DisplayNameAttribute("Up trend")]
		public Color UpTrendColor
		{
			get { return upTrendColor; }
			set { upTrendColor = value; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		public string UpTrendColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(upTrendColor); }
			set { upTrendColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[XmlIgnore()]
		[Description("Color of History down trend line.")]
		[Category("Colors")]
		[Gui.Design.DisplayNameAttribute("History Down trend")]
		public Color DownHistColor
		{
			get { return downHistColor; }
			set { downHistColor = value; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		public string DownHistColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(downHistColor); }
			set { downHistColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[XmlIgnore()]
		[Description("Color of the History up trend line.")]
		[Category("Colors")]
		[Gui.Design.DisplayNameAttribute("History Up trend")]
		public Color UpHistColor
		{
			get { return upHistColor; }
			set { upHistColor = value; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		public string UpHistColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(upHistColor); }
			set { upHistColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
        #endregion
    }
}
#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private AutoTrendH[] cacheAutoTrendH = null;

        private static AutoTrendH checkAutoTrendH = new AutoTrendH();

        /// <summary>
        /// AutoTrendLine with History option
        /// </summary>
        /// <returns></returns>
        public AutoTrendH AutoTrendH(bool alertOnBreak, bool showHistory, int strength)
        {
            return AutoTrendH(Input, alertOnBreak, showHistory, strength);
        }

        /// <summary>
        /// AutoTrendLine with History option
        /// </summary>
        /// <returns></returns>
        public AutoTrendH AutoTrendH(Data.IDataSeries input, bool alertOnBreak, bool showHistory, int strength)
        {
            if (cacheAutoTrendH != null)
                for (int idx = 0; idx < cacheAutoTrendH.Length; idx++)
                    if (cacheAutoTrendH[idx].AlertOnBreak == alertOnBreak && cacheAutoTrendH[idx].ShowHistory == showHistory && cacheAutoTrendH[idx].Strength == strength && cacheAutoTrendH[idx].EqualsInput(input))
                        return cacheAutoTrendH[idx];

            lock (checkAutoTrendH)
            {
                checkAutoTrendH.AlertOnBreak = alertOnBreak;
                alertOnBreak = checkAutoTrendH.AlertOnBreak;
                checkAutoTrendH.ShowHistory = showHistory;
                showHistory = checkAutoTrendH.ShowHistory;
                checkAutoTrendH.Strength = strength;
                strength = checkAutoTrendH.Strength;

                if (cacheAutoTrendH != null)
                    for (int idx = 0; idx < cacheAutoTrendH.Length; idx++)
                        if (cacheAutoTrendH[idx].AlertOnBreak == alertOnBreak && cacheAutoTrendH[idx].ShowHistory == showHistory && cacheAutoTrendH[idx].Strength == strength && cacheAutoTrendH[idx].EqualsInput(input))
                            return cacheAutoTrendH[idx];

                AutoTrendH indicator = new AutoTrendH();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AlertOnBreak = alertOnBreak;
                indicator.ShowHistory = showHistory;
                indicator.Strength = strength;
                Indicators.Add(indicator);
                indicator.SetUp();

                AutoTrendH[] tmp = new AutoTrendH[cacheAutoTrendH == null ? 1 : cacheAutoTrendH.Length + 1];
                if (cacheAutoTrendH != null)
                    cacheAutoTrendH.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheAutoTrendH = tmp;
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
        /// AutoTrendLine with History option
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AutoTrendH AutoTrendH(bool alertOnBreak, bool showHistory, int strength)
        {
            return _indicator.AutoTrendH(Input, alertOnBreak, showHistory, strength);
        }

        /// <summary>
        /// AutoTrendLine with History option
        /// </summary>
        /// <returns></returns>
        public Indicator.AutoTrendH AutoTrendH(Data.IDataSeries input, bool alertOnBreak, bool showHistory, int strength)
        {
            return _indicator.AutoTrendH(input, alertOnBreak, showHistory, strength);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// AutoTrendLine with History option
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AutoTrendH AutoTrendH(bool alertOnBreak, bool showHistory, int strength)
        {
            return _indicator.AutoTrendH(Input, alertOnBreak, showHistory, strength);
        }

        /// <summary>
        /// AutoTrendLine with History option
        /// </summary>
        /// <returns></returns>
        public Indicator.AutoTrendH AutoTrendH(Data.IDataSeries input, bool alertOnBreak, bool showHistory, int strength)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.AutoTrendH(input, alertOnBreak, showHistory, strength);
        }
    }
}
#endregion
