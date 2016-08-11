// 
// Copyright (C) 2008, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Double stochastics
    /// </summary>
    [Description("Double stochastics")]
    [Gui.Design.DisplayName("DoubleStochastics")]
    public class DoubleStochastics : Indicator
    {
        #region Variables
        private int period = 10;
		private DataSeries p1;
		private DataSeries p2;
		private DataSeries p3;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Red, PlotStyle.Line, "K"));
            Add(new Line(Color.Blue, 90, "Upper"));
            Add(new Line(Color.Blue, 10, "Lower"));
			Lines[0].Pen.DashStyle = DashStyle.Dash;
			Lines[1].Pen.DashStyle = DashStyle.Dash;

			p1 = new DataSeries(this);
			p2 = new DataSeries(this);
			p3 = new DataSeries(this);
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			double r = MAX(High, Period)[0] - MIN(Low, Period)[0];
            r = r.Compare(0, 0.000000000001) == 0 ? 0 : r;

            if (r == 0)
                p1.Set(CurrentBar == 0 ? 50 : p1[1]);
            else
                p1.Set(Math.Min(100, Math.Max(0, 100 * (Close[0] - MIN(Low, Period)[0]) / r)));

			p2.Set(EMA(p1, 3)[0]);
			
			double s = MAX(p2, Period)[0] - MIN(p2, Period)[0];
            s = s.Compare(0, 0.000000000001) == 0 ? 0 : s;

            if (s == 0)
                p3.Set(CurrentBar == 0 ? 50 : p3[1]);
            else
                p3.Set(Math.Min(100, Math.Max(0, 100 * (p2[0] - MIN(p2, Period)[0]) / s)));

			K.Set(EMA(p3, 3)[0]);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries K
        {
            get { return Values[0]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
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
        private DoubleStochastics[] cacheDoubleStochastics = null;

        private static DoubleStochastics checkDoubleStochastics = new DoubleStochastics();

        /// <summary>
        /// Double stochastics
        /// </summary>
        /// <returns></returns>
        public DoubleStochastics DoubleStochastics(int period)
        {
            return DoubleStochastics(Input, period);
        }

        /// <summary>
        /// Double stochastics
        /// </summary>
        /// <returns></returns>
        public DoubleStochastics DoubleStochastics(Data.IDataSeries input, int period)
        {
            if (cacheDoubleStochastics != null)
                for (int idx = 0; idx < cacheDoubleStochastics.Length; idx++)
                    if (cacheDoubleStochastics[idx].Period == period && cacheDoubleStochastics[idx].EqualsInput(input))
                        return cacheDoubleStochastics[idx];

            lock (checkDoubleStochastics)
            {
                checkDoubleStochastics.Period = period;
                period = checkDoubleStochastics.Period;

                if (cacheDoubleStochastics != null)
                    for (int idx = 0; idx < cacheDoubleStochastics.Length; idx++)
                        if (cacheDoubleStochastics[idx].Period == period && cacheDoubleStochastics[idx].EqualsInput(input))
                            return cacheDoubleStochastics[idx];

                DoubleStochastics indicator = new DoubleStochastics();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                DoubleStochastics[] tmp = new DoubleStochastics[cacheDoubleStochastics == null ? 1 : cacheDoubleStochastics.Length + 1];
                if (cacheDoubleStochastics != null)
                    cacheDoubleStochastics.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheDoubleStochastics = tmp;
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
        /// Double stochastics
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DoubleStochastics DoubleStochastics(int period)
        {
            return _indicator.DoubleStochastics(Input, period);
        }

        /// <summary>
        /// Double stochastics
        /// </summary>
        /// <returns></returns>
        public Indicator.DoubleStochastics DoubleStochastics(Data.IDataSeries input, int period)
        {
            return _indicator.DoubleStochastics(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Double stochastics
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DoubleStochastics DoubleStochastics(int period)
        {
            return _indicator.DoubleStochastics(Input, period);
        }

        /// <summary>
        /// Double stochastics
        /// </summary>
        /// <returns></returns>
        public Indicator.DoubleStochastics DoubleStochastics(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.DoubleStochastics(input, period);
        }
    }
}
#endregion
