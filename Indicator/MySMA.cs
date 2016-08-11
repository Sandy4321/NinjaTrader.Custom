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
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Simple Moving Average
    /// </summary>
    [Description("Simple Moving Average")]
    public class MySMA : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period = 1; // Default setting for Period
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "Plot0"));
            CalculateOnBarClose = true;
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Do not calculate if we don't have enough bars
            if (CurrentBar < Period) return;

            double sum = 0;

            for (int barsAgo = 0; barsAgo < Period; barsAgo++)
            {
                sum = sum + Input[barsAgo];
            }


            Plot0.Set(sum/Period);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot0
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
        private MySMA[] cacheMySMA = null;

        private static MySMA checkMySMA = new MySMA();

        /// <summary>
        /// Simple Moving Average
        /// </summary>
        /// <returns></returns>
        public MySMA MySMA(int period)
        {
            return MySMA(Input, period);
        }

        /// <summary>
        /// Simple Moving Average
        /// </summary>
        /// <returns></returns>
        public MySMA MySMA(Data.IDataSeries input, int period)
        {
            if (cacheMySMA != null)
                for (int idx = 0; idx < cacheMySMA.Length; idx++)
                    if (cacheMySMA[idx].Period == period && cacheMySMA[idx].EqualsInput(input))
                        return cacheMySMA[idx];

            lock (checkMySMA)
            {
                checkMySMA.Period = period;
                period = checkMySMA.Period;

                if (cacheMySMA != null)
                    for (int idx = 0; idx < cacheMySMA.Length; idx++)
                        if (cacheMySMA[idx].Period == period && cacheMySMA[idx].EqualsInput(input))
                            return cacheMySMA[idx];

                MySMA indicator = new MySMA();
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

                MySMA[] tmp = new MySMA[cacheMySMA == null ? 1 : cacheMySMA.Length + 1];
                if (cacheMySMA != null)
                    cacheMySMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheMySMA = tmp;
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
        /// Simple Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MySMA MySMA(int period)
        {
            return _indicator.MySMA(Input, period);
        }

        /// <summary>
        /// Simple Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.MySMA MySMA(Data.IDataSeries input, int period)
        {
            return _indicator.MySMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Simple Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MySMA MySMA(int period)
        {
            return _indicator.MySMA(Input, period);
        }

        /// <summary>
        /// Simple Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.MySMA MySMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.MySMA(input, period);
        }
    }
}
#endregion
