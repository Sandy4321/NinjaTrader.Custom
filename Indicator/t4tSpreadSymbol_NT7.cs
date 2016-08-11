//
// This indicator is intended for TRUE ELITE users at BMT. 
// The public result plot value can be used as Input for any other indicators... (only limited by the high cpu load)
//
// PLEASE DO NOT STEAL AND SELL TO OTHERS, YOU KNOW WHO YOU ARE (and we know too!). 
//
// Written by TimeTrade (atm-service@online.de)
// Version 1.1 (Apr 10, 2012)
// www.tools4trading.de

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
using t4t;
#endregion

namespace t4t
{
    public enum SpreadSymResult
    {
        SSRclose,
        SSRhigh,
        SSRlow,
        SSRopen,   
		SSRbase,
        SSRprice1,
        SSRprice2,
    }

//only the ProVersion has & use a very fast code for direct work with the NinjaTickFiles without the slowly 1tick dataseries :)
// public enum SpreadSymMode 
// {
//    SSMdataseries,
//    SSMninjafile,
// }
}


// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Tools4Trading: True SpreadSymbolChart, full tickbased and syncronized for any (intraday) BarTypes
    /// </summary>
    [Description("Tools4Trading: True SpreadSymbolChart, full tickbased and syncronized for any (intraday) BarTypes")]
    public class t4tSpreadSym : Indicator
    {
        #region Variables, PreSettings & Cleanup
			private bool bRunInit = true;

			private int iSStrades1 = 0;
            private double dSSprice1 = 0;
            private int iSStrades2 = 0;
            private double dSSprice2 = 0;

            private double dSSbase = 0;
            private double dSSopen = 0;
            private double dSShigh = 0;
            private double dSSlow = 0;
            private double dSSclose = 0;

            private DataSeries dsSSbase = null;
            private DataSeries dsSSopen = null;
            private DataSeries dsSShigh = null;
            private DataSeries dsSSlow = null;
            private DataSeries dsSSclose = null;

			private int startbar = -1;
			private int lastcalcbar = -1;

			private Color           colorBaseLine        = Color.Indigo;
			private Color           colorZeroLine        = Color.Blue;

			private Color barColorDown = Color.Red;
			private Color           barColorUp           = Color.Lime;
			private SolidBrush      brushDown            = null;
			private SolidBrush      brushUp              = null;
			private Color           shadowColor          = Color.Black;
			private Pen             baseLinePen          = null;
			private Pen             zeroLinePen          = null;
			private Pen             shadowPen            = null;
			private int             shadowWidth          = 1;

        /// <summary>
        /// </summary>
        protected override void OnStartUp()
        {
			if(!bRunInit) return;
			bRunInit=false;
			
			iSStrades1 = 0;
            dSSprice1 = 0;

            iSStrades2 = 0;
            dSSprice2 = 0;

            dSSbase = 0;
            dSSopen = 0;
            dSShigh = 0;
            dSSlow = 0;
            dSSclose = 0;

            brushUp = new SolidBrush(barColorUp);
            brushDown = new SolidBrush(barColorDown);
            shadowPen = new Pen(shadowColor, shadowWidth);
            baseLinePen = new Pen(colorBaseLine,1);
            zeroLinePen = new Pen(colorZeroLine,1);

            dsSSbase = new DataSeries(this);
            dsSSopen = new DataSeries(this);
            dsSShigh = new DataSeries(this);
            dsSSlow = new DataSeries(this);
            dsSSclose = new DataSeries(this);
        }

        /// <summary>
        /// </summary>
        protected override void OnTermination()
        {
            if (dsSSbase != null) dsSSbase.Dispose();
            if (dsSSopen != null) dsSSopen.Dispose();
            if (dsSShigh != null) dsSShigh.Dispose();
            if (dsSSlow != null) dsSSlow.Dispose();
            if (dsSSclose != null) dsSSclose.Dispose();

            if (brushUp != null) brushUp.Dispose();
            if (brushDown != null) brushDown.Dispose();
            if (shadowPen != null) shadowPen.Dispose();
            if (baseLinePen != null) baseLinePen.Dispose();
            if (zeroLinePen != null) zeroLinePen.Dispose();
        }
        #endregion

        /// <summary>
        /// </summary>
        protected override void Initialize()
        {
			Name="t4tSpreadSym11";
			
            Add(new Plot(Color.Gray, PlotStyle.Dot, "SSresult"));
			
		    MaximumBarsLookBack = MaximumBarsLookBack.Infinite;
            PaintPriceMarkers   = true;
			CalculateOnBarClose = false;
            PlotsConfigurable   = false;
            Overlay             = false;

            if (sSSfullName1 == "") sSSfullName1 = Instrument.FullName;
            if (sSSfullName2 == "") sSSfullName2 = Instrument.FullName;

		//only the ProVersion has & use a very fast code for direct work with the NinjaTickFiles without the slowly 1tick dataseries :)
		//	if(tMultiSymMode == t4tMultiSymMode.MSMdataseries)
			{
    	        Add(sSSfullName1, PeriodType.Tick, 1);  // this is the additional SymbolDataSeries
    	        Add(sSSfullName2, PeriodType.Tick, 1);  // this is the additional SymbolDataSeries
			}

			bRunInit=true;
        }

        #region Update-Methods
		
        /// <summary>
        /// </summary>
        protected override void OnBarUpdate()
        {
			try {
				if((!Historical) && (!CalculateOnBarClose)) // simple variant for live with TickMode
				{
                    if (BarsInProgress == 2) // SecondarySymbol2?
                    {
                        DoBIP2tickUpdate();
                    }
                    else if (BarsInProgress == 1) // SecondarySymbol1?
                    {
                        DoBIP1tickUpdate();
                    }
                    else // BasePeriod
					{
						if(FirstTickOfBar)
							DoBarUpdate(1); // FirstTick/BarOpen
						else
							DoBarUpdate(0); // DefaultTick
					}
				}
				else // Historical || CalcOnBarClose=true
				{
                    if (BarsInProgress == 2) // SecondarySymbol2?
                    {
                        DoBIP2tickUpdate();
                    } 
                    else if (BarsInProgress == 1) // SecondarySymbol1?
					{
						DoBIP1tickUpdate();
					}
					else // BasePeriod?
					{
						DoBarUpdate(2); // LastTick/BarClose
					}
				}
			} catch (Exception ex) { Print("Exception in t4tSpreadSym on line 162: " + ex.Message); }
        }

        private void DoBIP1tickUpdate()
		{
			try {
				if(Bars.FirstBarOfSession || Bars.IsNewSession(Times[1][0])) {iSStrades1=0;dSSclose=0;}

                dSSprice1 = Closes[1][0];
                iSStrades1++;

                if((dSSprice1!=0) && (dSSprice2!=0))
                {
				    if(dSSclose==0)
				    {
                        dSSclose = (dSSprice1 * dSSfactor1) / (dSSprice2 * dSSfactor2);
					    dSSopen = dSSclose; dSShigh = dSSclose; dSSlow = dSSclose;
				    }
				    else
				    {
                        dSSclose = (dSSprice1 * dSSfactor1) / (dSSprice2 * dSSfactor2);
				    }
    				
	    			if (dSShigh < dSSclose) dSShigh = dSSclose;
		    		if (dSSlow > dSSclose) dSSlow = dSSclose;
                }
            }  catch (Exception ex) { Print("Exception in t4tSpreadSym on line 192: " + ex.Message); }
		}

        private void DoBIP2tickUpdate()
        {
            try
            {
                if (Bars.FirstBarOfSession || Bars.IsNewSession(Times[2][0])) {iSStrades2=0;dSSclose=0;}

                dSSprice2 = Closes[2][0];
                iSStrades2++;

                if ((dSSprice1 != 0) && (dSSprice2 != 0))
                {
                    if (dSSclose == 0)
                    {
                        dSSclose = (dSSprice1 * dSSfactor1) / (dSSprice2 * dSSfactor2);
                        dSSopen = dSSclose; dSShigh = dSSclose; dSSlow = dSSclose;
                    }
                    else
                    {
                        dSSclose = (dSSprice1 * dSSfactor1) / (dSSprice2 * dSSfactor2);
                    }

                    if (dSShigh < dSSclose) dSShigh = dSSclose;
                    if (dSSlow > dSSclose) dSSlow = dSSclose;
                }
            }
            catch (Exception ex) { Print("Exception in t4tSpreadSym on line 192: " + ex.Message); }
        }

        private void DoBarUpdate(int iUpdateMode) //Mode= -1:"OutOfSync", 0:Default, 1:FirstTick/BarOpen, 2:LastTick/BarClose
        {
			try {
				if (startbar == -1)
					startbar = CurrentBar;

				lastcalcbar = CurrentBar;

   			    if(Bars.FirstBarOfSession) dSSbase=dSSopen;

				switch(iUpdateMode)
				{
					case 0:
						dsSSclose[0]=dSSclose;
						dsSSlow[0]=dSSlow;
						dsSShigh[0]=dSShigh;
						dsSSopen[0]=dSSopen;
						dsSSbase[0]=dSSbase;
						break;
						
					case 1:
						dsSSclose[0]=dSSclose;
						dsSSlow[0]=dSSclose;
						dsSShigh[0]=dSSclose;
						dsSSopen[0]=dSSclose;
						dsSSbase[0]=dSSbase;

						iSStrades1=0;
						iSStrades2=0;
						dSSopen=dSSclose;
						dSSlow=dSSclose;
						dSShigh=dSSclose;
						break;
						
					case 2:
						dsSSclose[0]=dSSclose;
						dsSSlow[0]=dSSlow;
						dsSShigh[0]=dSShigh;
						dsSSopen[0]=dSSopen;
						dsSSbase[0]=dSSbase;

						iSStrades1=0;
						iSStrades2=0;
						dSSopen=dSSclose;
						dSSlow=dSSclose;
						dSShigh=dSSclose;
						break;
				}					

				switch (tSpreadSymResult)
				{
                    case t4t.SpreadSymResult.SSRclose: Values[0][0] = dsSSclose[0]; break;
                    case t4t.SpreadSymResult.SSRlow: Values[0][0] = dsSSlow[0]; break;
                    case t4t.SpreadSymResult.SSRhigh: Values[0][0] = dsSShigh[0]; break;
                    case t4t.SpreadSymResult.SSRopen: Values[0][0] = dsSSopen[0]; break;
                    case t4t.SpreadSymResult.SSRprice1: Values[0][0] = dSSprice1; break;
                    case t4t.SpreadSymResult.SSRprice2: Values[0][0] = dSSprice2; break;
                }
            } catch (Exception ex) { Print("Exception in t4tSpreadSym on line 229: " + ex.Message); }
		}
        #endregion
			
        #region Draw-Methods

        public override void GetMinMaxValues(ChartControl chartControl, ref double min, ref double max)
        {
			if((chartControl==null) || (Bars == null)) return;

			int lastBar,firstBar;
			double tmp;

			min = Double.MaxValue;
			max = Double.MinValue;

			firstBar = Math.Max(this.FirstBarIndexPainted,1);
			if(firstBar<startbar) firstBar=startbar;

			lastBar = Math.Min(this.LastBarIndexPainted, Bars.Count - 1);
			if(lastBar>lastcalcbar) lastBar=lastcalcbar;

			for (int idx = firstBar; idx <= lastBar; idx++)
			{
				tmp=dsSShigh.Get(idx); if (max<tmp) max=tmp;
				tmp=dsSSlow.Get(idx); if(min>tmp) min=tmp;
			}

			if ((max - min) < .000001)
			{
				min -= .000001;
				max += .000001;
			}
		}


		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
        {
			double valH=0;
			double valL=0;
            double valC=0;
            double valO=0;
            int x = 0;
            int y1 = 0;
            int y2 = 0;
            int y3 = 0;
            int y4 = 0;
			
			try {
				if (Bars == null || ChartControl == null) return;

				if(colorZeroLine!=Color.Transparent)
				{
					int yz = ChartControl.GetYByValue(this, 0);
					graphics.DrawLine(zeroLinePen, bounds.Left, yz, bounds.Right, yz);
				}

				int barPaintWidth = Math.Max(3, 1 + 2 * ((int)Bars.BarsData.ChartStyle.BarWidth - 1) + 2 * shadowWidth);

				int firstBar = Math.Max(this.FirstBarIndexPainted,1);
				if(firstBar<startbar) firstBar=startbar;
				
				int lastBar = Math.Min(this.LastBarIndexPainted, Bars.Count - 1);
				if(lastBar>lastcalcbar) lastBar=lastcalcbar;
	
				for (int idx = firstBar; idx <= lastBar; idx++)
				{
					valH = dsSShigh.Get(idx);
					valL = dsSSlow.Get(idx);
					valC = dsSSclose.Get(idx);
					valO = dsSSopen.Get(idx);
					
					x = ChartControl.GetXByBarIdx(BarsArray[0], idx);
					y1 = ChartControl.GetYByValue(this, valO);
					y2 = ChartControl.GetYByValue(this, valH);
					y3 = ChartControl.GetYByValue(this, valL);
					y4 = ChartControl.GetYByValue(this, valC);

					if(colorBaseLine!=Color.Transparent)
					{
		                double valB = dsSSbase.Get(idx);
						int yb = ChartControl.GetYByValue(this, valB);
						
						graphics.DrawLine(baseLinePen, x-barPaintWidth, yb, x+barPaintWidth, yb);
					}
			
					graphics.DrawLine(shadowPen, x, y2, x, y3);

					if (y4 == y1)
						graphics.DrawLine(shadowPen, x - barPaintWidth / 2, y1, x + barPaintWidth / 2, y1);
					else
					{
						if (y4 > y1)
							graphics.FillRectangle(brushDown, x - barPaintWidth / 2, y1, barPaintWidth, y4 - y1);
						else
							graphics.FillRectangle(brushUp, x - barPaintWidth / 2, y4, barPaintWidth, y1 - y4);
						graphics.DrawRectangle(shadowPen, (x - barPaintWidth / 2) + (shadowPen.Width / 2), Math.Min(y4, y1), barPaintWidth - shadowPen.Width, Math.Abs(y4 - y1));
					}
				}
			} catch (Exception ex) {Print("Exception in t4tSpreadSym on line 420: " + ex.Message);}
        }
        #endregion

		#region Properties

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries SSresult
        {
            get { return Values[0]; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries SSbase
        {
            get { return dsSSbase; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries SSopen
        {
            get { return dsSSopen; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries SShigh
        {
            get { return dsSShigh; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries SSlow
        {
            get { return dsSSlow; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries SSclose
        {
            get { return dsSSclose; }
        }

        [XmlIgnore()]
        [Description("Color of base line.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Color base line")]
        public Color ColorBaseLine
        {
            get { return colorBaseLine; }
            set { colorBaseLine = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string ColorBaseLineSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(colorBaseLine); }
            set { colorBaseLine = Gui.Design.SerializableColor.FromString(value); }
        }

        [XmlIgnore()]
        [Description("Color of zero line.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Color zero line")]
        public Color ColorZeroLine
        {
            get { return colorZeroLine; }
            set { colorZeroLine = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string ColorZeroLineSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(colorZeroLine); }
            set { colorZeroLine = Gui.Design.SerializableColor.FromString(value); }
        }

        [XmlIgnore()]
        [Description("Color of down bars.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Down color")]
        public Color BarColorDown
        {
            get { return barColorDown; }
            set { barColorDown = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string BarColorDownSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(barColorDown); }
            set { barColorDown = Gui.Design.SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [XmlIgnore()]
        [Description("Color of up bars.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Up color")]
        public Color BarColorUp
        {
            get { return barColorUp; }
            set { barColorUp = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string BarColorUpSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(barColorUp); }
            set { barColorUp = Gui.Design.SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [XmlIgnore()]
        [Description("Color of shadow line.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Shadow color")]
        public Color ShadowColor
        {
            get { return shadowColor; }
            set { shadowColor = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string ShadowColorSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(shadowColor); }
            set { shadowColor = Gui.Design.SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Width of shadow line.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Shadow width")]
        public int ShadowWidth
        {
            get { return shadowWidth; }
            set { shadowWidth = Math.Max(value, 1); }
        }

        private string sSSfullName1="";
        /// <summary>
        /// </summary>
        [Description("FullName of SpreadSymbol1")]
        [Category("Parameters")]
        [Gui.Design.DisplayNameAttribute("SpreadSymbol 1")]
        [RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(t4t.SymbolManagerConverter))]
        public string SSfullName1
        {
            get { return sSSfullName1; }
            set { sSSfullName1 = value; }
        }

        private double dSSfactor1 = 1;
        /// <summary>
        /// </summary>
        [Description("Multiplicator for SpreadSymbol1")]
        [Category("Parameters")]
        [Gui.Design.DisplayNameAttribute("SpreadFactor 1")]
        public double SSfactor1
        {
            get { return dSSfactor1; }
            set { dSSfactor1 = Math.Max(0.0001,value); }
        }

        private string sSSfullName2 = "";
        /// <summary>
        /// </summary>
        [Description("FullName of SpreadSymbol2")]
        [Category("Parameters")]
        [Gui.Design.DisplayNameAttribute("SpreadSymbol 2")]
        [RefreshProperties(RefreshProperties.All)]
        [TypeConverter(typeof(t4t.SymbolManagerConverter))]
        public string SSfullName2
        {
            get { return sSSfullName2; }
            set { sSSfullName2 = value; }
        }

        private double dSSfactor2 = 1;
        /// <summary>
        /// </summary>
        [Description("Multiplicator for SpreadSymbol2")]
        [Category("Parameters")]
        [Gui.Design.DisplayNameAttribute("SpreadFactor 2")]
        public double SSfactor2
        {
            get { return dSSfactor2; }
            set { dSSfactor2 = Math.Max(0.0001, value); }
        }

        private t4t.SpreadSymResult tSpreadSymResult = t4t.SpreadSymResult.SSRclose;
        /// <summary>
        /// </summary>
        [Description("ResultValue form SpreadSymbolCandle indicator")]
        [Category("Parameters")]
        [Gui.Design.DisplayNameAttribute("SpreadSymbol Result")]
        public t4t.SpreadSymResult SpreadSymResult
        {
            get { return tSpreadSymResult; }
            set { tSpreadSymResult = value; }
        }
      
	//only the ProVersion has & use a very fast code for direct work with the NinjaTickFiles without the slowly 1tick dataseries :)
    //    private t4tMultiSymMode tMultiSymMode = t4tMultiSymMode.MSMdataseries;
    //    /// <summary>
    //    /// </summary>
    //    [Description("Define the MultiSymbolCandle indicator data access mode")]
    //    [Category("Parameters")]
    //    [Gui.Design.DisplayNameAttribute("MultiSymbol DataMode")]
    //    public t4tMultiSymMode MultiSymMode
    //    {
    //        get { return tMultiSymMode; }
    //        set { tMultiSymMode = value; }
    //    }

		#endregion
    }
}


#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private t4tSpreadSym[] cachet4tSpreadSym = null;

        private static t4tSpreadSym checkt4tSpreadSym = new t4tSpreadSym();

        /// <summary>
        /// Tools4Trading: True SpreadSymbolChart, full tickbased and syncronized for any (intraday) BarTypes
        /// </summary>
        /// <returns></returns>
        public t4tSpreadSym t4tSpreadSym(t4t.SpreadSymResult spreadSymResult, double sSfactor1, double sSfactor2, string sSfullName1, string sSfullName2)
        {
            return t4tSpreadSym(Input, spreadSymResult, sSfactor1, sSfactor2, sSfullName1, sSfullName2);
        }

        /// <summary>
        /// Tools4Trading: True SpreadSymbolChart, full tickbased and syncronized for any (intraday) BarTypes
        /// </summary>
        /// <returns></returns>
        public t4tSpreadSym t4tSpreadSym(Data.IDataSeries input, t4t.SpreadSymResult spreadSymResult, double sSfactor1, double sSfactor2, string sSfullName1, string sSfullName2)
        {
            if (cachet4tSpreadSym != null)
                for (int idx = 0; idx < cachet4tSpreadSym.Length; idx++)
                    if (cachet4tSpreadSym[idx].SpreadSymResult == spreadSymResult && Math.Abs(cachet4tSpreadSym[idx].SSfactor1 - sSfactor1) <= double.Epsilon && Math.Abs(cachet4tSpreadSym[idx].SSfactor2 - sSfactor2) <= double.Epsilon && cachet4tSpreadSym[idx].SSfullName1 == sSfullName1 && cachet4tSpreadSym[idx].SSfullName2 == sSfullName2 && cachet4tSpreadSym[idx].EqualsInput(input))
                        return cachet4tSpreadSym[idx];

            lock (checkt4tSpreadSym)
            {
                checkt4tSpreadSym.SpreadSymResult = spreadSymResult;
                spreadSymResult = checkt4tSpreadSym.SpreadSymResult;
                checkt4tSpreadSym.SSfactor1 = sSfactor1;
                sSfactor1 = checkt4tSpreadSym.SSfactor1;
                checkt4tSpreadSym.SSfactor2 = sSfactor2;
                sSfactor2 = checkt4tSpreadSym.SSfactor2;
                checkt4tSpreadSym.SSfullName1 = sSfullName1;
                sSfullName1 = checkt4tSpreadSym.SSfullName1;
                checkt4tSpreadSym.SSfullName2 = sSfullName2;
                sSfullName2 = checkt4tSpreadSym.SSfullName2;

                if (cachet4tSpreadSym != null)
                    for (int idx = 0; idx < cachet4tSpreadSym.Length; idx++)
                        if (cachet4tSpreadSym[idx].SpreadSymResult == spreadSymResult && Math.Abs(cachet4tSpreadSym[idx].SSfactor1 - sSfactor1) <= double.Epsilon && Math.Abs(cachet4tSpreadSym[idx].SSfactor2 - sSfactor2) <= double.Epsilon && cachet4tSpreadSym[idx].SSfullName1 == sSfullName1 && cachet4tSpreadSym[idx].SSfullName2 == sSfullName2 && cachet4tSpreadSym[idx].EqualsInput(input))
                            return cachet4tSpreadSym[idx];

                t4tSpreadSym indicator = new t4tSpreadSym();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.SpreadSymResult = spreadSymResult;
                indicator.SSfactor1 = sSfactor1;
                indicator.SSfactor2 = sSfactor2;
                indicator.SSfullName1 = sSfullName1;
                indicator.SSfullName2 = sSfullName2;
                Indicators.Add(indicator);
                indicator.SetUp();

                t4tSpreadSym[] tmp = new t4tSpreadSym[cachet4tSpreadSym == null ? 1 : cachet4tSpreadSym.Length + 1];
                if (cachet4tSpreadSym != null)
                    cachet4tSpreadSym.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachet4tSpreadSym = tmp;
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
        /// Tools4Trading: True SpreadSymbolChart, full tickbased and syncronized for any (intraday) BarTypes
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.t4tSpreadSym t4tSpreadSym(t4t.SpreadSymResult spreadSymResult, double sSfactor1, double sSfactor2, string sSfullName1, string sSfullName2)
        {
            return _indicator.t4tSpreadSym(Input, spreadSymResult, sSfactor1, sSfactor2, sSfullName1, sSfullName2);
        }

        /// <summary>
        /// Tools4Trading: True SpreadSymbolChart, full tickbased and syncronized for any (intraday) BarTypes
        /// </summary>
        /// <returns></returns>
        public Indicator.t4tSpreadSym t4tSpreadSym(Data.IDataSeries input, t4t.SpreadSymResult spreadSymResult, double sSfactor1, double sSfactor2, string sSfullName1, string sSfullName2)
        {
            return _indicator.t4tSpreadSym(input, spreadSymResult, sSfactor1, sSfactor2, sSfullName1, sSfullName2);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Tools4Trading: True SpreadSymbolChart, full tickbased and syncronized for any (intraday) BarTypes
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.t4tSpreadSym t4tSpreadSym(t4t.SpreadSymResult spreadSymResult, double sSfactor1, double sSfactor2, string sSfullName1, string sSfullName2)
        {
            return _indicator.t4tSpreadSym(Input, spreadSymResult, sSfactor1, sSfactor2, sSfullName1, sSfullName2);
        }

        /// <summary>
        /// Tools4Trading: True SpreadSymbolChart, full tickbased and syncronized for any (intraday) BarTypes
        /// </summary>
        /// <returns></returns>
        public Indicator.t4tSpreadSym t4tSpreadSym(Data.IDataSeries input, t4t.SpreadSymResult spreadSymResult, double sSfactor1, double sSfactor2, string sSfullName1, string sSfullName2)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.t4tSpreadSym(input, spreadSymResult, sSfactor1, sSfactor2, sSfullName1, sSfullName2);
        }
    }
}
#endregion
