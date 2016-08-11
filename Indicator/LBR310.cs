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

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// LBR:3/10 Oscillator
    /// </summary>
    [Description("LBR:3/10 Oscillator")]
    [Gui.Design.DisplayName("LBR310")]
    public class LBR310 : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int fast = 3; // Default setting for Fast
            private int slow = 10; // Default setting for Slow
            private int longLine = 16; // Default setting for LongLine
			private int MoMoPeriod = 40;  //Look for momentum highs
			private int BarHiLo = 20;  //Look for price highs
			private int BarLookback = 3;  //Period in which the BarHigh needs to have occurred   
			private DataSeries		diff;
			private DataSeries		avg;
		// User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.DarkOrange, PlotStyle.Line, "OscLine"));
            Add(new Plot(Color.CornflowerBlue, PlotStyle.Bar, "OscBarUP"));
			Add(new Plot(Color.Maroon, PlotStyle.Bar, "OscBarDn"));
            Add(new Plot(Color.Lime, PlotStyle.Line, "HPLineUp"));
			Add(new Plot(Color.DarkViolet, PlotStyle.Line, "HPLineDn"));
			Add(new Line(Color.LightGray, 0, "ZeroLine"));
			
			diff				= new DataSeries(this);
			avg					= new DataSeries(this);
			
            CalculateOnBarClose	= false;
			DrawOnPricePanel	= false;
            Overlay				= false;
            PriceTypeSupported	= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
			
			double FastAvg = SMA(Close, fast)[0];
			double SlowAvg = SMA(Close, slow)[0];
			
			double Difference = (FastAvg - SlowAvg) / TickSize;
						
			diff.Set((FastAvg - SlowAvg) / TickSize);
			
			double Average = SMA(diff, longLine)[0];
			
			avg.Set(SMA(diff, longLine)[0]);
			
			OscLinePlot.Set(Difference);
			Print(High + "     " + CurrentBar);
			if (Rising(diff))
            	OscBarUpPlot.Set(Difference);
			else
				OscBarDnPlot.Set(Difference);
			
			if ((HighestBar(diff, MoMoPeriod) == 0 && HighestBar(High, BarHiLo) <= BarLookback) || (LowestBar(diff, MoMoPeriod) == 0 && LowestBar(Low, BarHiLo) <= BarLookback))
				DrawDot("Up Dot" + CurrentBar, true, 0, Difference, Color.Lime);
			
			if (Rising(avg))
			{
				HPLinePlotUp.Set(1, avg[1]);
				HPLinePlotUp.Set(Average);
			}
			else if (Falling(avg))
			{
				HPLinePlotDn.Set(1, avg[1]);
				HPLinePlotDn.Set(Average);
			}
			
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries OscLinePlot
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries OscBarUpPlot
        {
            get { return Values[1]; }
        }

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries OscBarDnPlot
        {
            get { return Values[2]; }
        }
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HPLinePlotUp
        {
            get { return Values[3]; }
        }

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HPLinePlotDn
        {
            get { return Values[4]; }
        }
		
        [Description("Fast Avg")]
        [Category("Parameters")]
        public int Fast
        {
            get { return fast; }
            set { fast = Math.Max(1, value); }
        }

        [Description("Slow Avg")]
        [Category("Parameters")]
        public int Slow
        {
            get { return slow; }
            set { slow = Math.Max(1, value); }
        }

        [Description("HP Avg")]
        [Category("Parameters")]
        public int LongLine
        {
            get { return longLine; }
            set { longLine = Math.Max(1, value); }
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
        private LBR310[] cacheLBR310 = null;

        private static LBR310 checkLBR310 = new LBR310();

        /// <summary>
        /// LBR:3/10 Oscillator
        /// </summary>
        /// <returns></returns>
        public LBR310 LBR310(int fast, int longLine, int slow)
        {
            return LBR310(Input, fast, longLine, slow);
        }

        /// <summary>
        /// LBR:3/10 Oscillator
        /// </summary>
        /// <returns></returns>
        public LBR310 LBR310(Data.IDataSeries input, int fast, int longLine, int slow)
        {
            if (cacheLBR310 != null)
                for (int idx = 0; idx < cacheLBR310.Length; idx++)
                    if (cacheLBR310[idx].Fast == fast && cacheLBR310[idx].LongLine == longLine && cacheLBR310[idx].Slow == slow && cacheLBR310[idx].EqualsInput(input))
                        return cacheLBR310[idx];

            lock (checkLBR310)
            {
                checkLBR310.Fast = fast;
                fast = checkLBR310.Fast;
                checkLBR310.LongLine = longLine;
                longLine = checkLBR310.LongLine;
                checkLBR310.Slow = slow;
                slow = checkLBR310.Slow;

                if (cacheLBR310 != null)
                    for (int idx = 0; idx < cacheLBR310.Length; idx++)
                        if (cacheLBR310[idx].Fast == fast && cacheLBR310[idx].LongLine == longLine && cacheLBR310[idx].Slow == slow && cacheLBR310[idx].EqualsInput(input))
                            return cacheLBR310[idx];

                LBR310 indicator = new LBR310();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Fast = fast;
                indicator.LongLine = longLine;
                indicator.Slow = slow;
                Indicators.Add(indicator);
                indicator.SetUp();

                LBR310[] tmp = new LBR310[cacheLBR310 == null ? 1 : cacheLBR310.Length + 1];
                if (cacheLBR310 != null)
                    cacheLBR310.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheLBR310 = tmp;
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
        /// LBR:3/10 Oscillator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.LBR310 LBR310(int fast, int longLine, int slow)
        {
            return _indicator.LBR310(Input, fast, longLine, slow);
        }

        /// <summary>
        /// LBR:3/10 Oscillator
        /// </summary>
        /// <returns></returns>
        public Indicator.LBR310 LBR310(Data.IDataSeries input, int fast, int longLine, int slow)
        {
            return _indicator.LBR310(input, fast, longLine, slow);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// LBR:3/10 Oscillator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.LBR310 LBR310(int fast, int longLine, int slow)
        {
            return _indicator.LBR310(Input, fast, longLine, slow);
        }

        /// <summary>
        /// LBR:3/10 Oscillator
        /// </summary>
        /// <returns></returns>
        public Indicator.LBR310 LBR310(Data.IDataSeries input, int fast, int longLine, int slow)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.LBR310(input, fast, longLine, slow);
        }
    }
}
#endregion
