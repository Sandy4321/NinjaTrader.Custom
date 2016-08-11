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
    /// Enter the description of your new custom indicator here
    /// </summary>
    [Description("Enter the description of your new custom indicator here")]
    public class kEventOnChart : Indicator
    {
        #region Variables
        // Wizard generated variables
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
        }

        #region Properties

        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private kEventOnChart[] cachekEventOnChart = null;

        private static kEventOnChart checkkEventOnChart = new kEventOnChart();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public kEventOnChart kEventOnChart()
        {
            return kEventOnChart(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public kEventOnChart kEventOnChart(Data.IDataSeries input)
        {
            if (cachekEventOnChart != null)
                for (int idx = 0; idx < cachekEventOnChart.Length; idx++)
                    if (cachekEventOnChart[idx].EqualsInput(input))
                        return cachekEventOnChart[idx];

            lock (checkkEventOnChart)
            {
                if (cachekEventOnChart != null)
                    for (int idx = 0; idx < cachekEventOnChart.Length; idx++)
                        if (cachekEventOnChart[idx].EqualsInput(input))
                            return cachekEventOnChart[idx];

                kEventOnChart indicator = new kEventOnChart();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                kEventOnChart[] tmp = new kEventOnChart[cachekEventOnChart == null ? 1 : cachekEventOnChart.Length + 1];
                if (cachekEventOnChart != null)
                    cachekEventOnChart.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachekEventOnChart = tmp;
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
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.kEventOnChart kEventOnChart()
        {
            return _indicator.kEventOnChart(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.kEventOnChart kEventOnChart(Data.IDataSeries input)
        {
            return _indicator.kEventOnChart(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.kEventOnChart kEventOnChart()
        {
            return _indicator.kEventOnChart(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.kEventOnChart kEventOnChart(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.kEventOnChart(input);
        }
    }
}
#endregion
