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
    public class matlabTradeReportIndicator : Indicator
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

        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
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
        private matlabTradeReportIndicator[] cachematlabTradeReportIndicator = null;

        private static matlabTradeReportIndicator checkmatlabTradeReportIndicator = new matlabTradeReportIndicator();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public matlabTradeReportIndicator matlabTradeReportIndicator()
        {
            return matlabTradeReportIndicator(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public matlabTradeReportIndicator matlabTradeReportIndicator(Data.IDataSeries input)
        {
            if (cachematlabTradeReportIndicator != null)
                for (int idx = 0; idx < cachematlabTradeReportIndicator.Length; idx++)
                    if (cachematlabTradeReportIndicator[idx].EqualsInput(input))
                        return cachematlabTradeReportIndicator[idx];

            lock (checkmatlabTradeReportIndicator)
            {
                if (cachematlabTradeReportIndicator != null)
                    for (int idx = 0; idx < cachematlabTradeReportIndicator.Length; idx++)
                        if (cachematlabTradeReportIndicator[idx].EqualsInput(input))
                            return cachematlabTradeReportIndicator[idx];

                matlabTradeReportIndicator indicator = new matlabTradeReportIndicator();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                matlabTradeReportIndicator[] tmp = new matlabTradeReportIndicator[cachematlabTradeReportIndicator == null ? 1 : cachematlabTradeReportIndicator.Length + 1];
                if (cachematlabTradeReportIndicator != null)
                    cachematlabTradeReportIndicator.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachematlabTradeReportIndicator = tmp;
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
        public Indicator.matlabTradeReportIndicator matlabTradeReportIndicator()
        {
            return _indicator.matlabTradeReportIndicator(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.matlabTradeReportIndicator matlabTradeReportIndicator(Data.IDataSeries input)
        {
            return _indicator.matlabTradeReportIndicator(input);
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
        public Indicator.matlabTradeReportIndicator matlabTradeReportIndicator()
        {
            return _indicator.matlabTradeReportIndicator(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.matlabTradeReportIndicator matlabTradeReportIndicator(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.matlabTradeReportIndicator(input);
        }
    }
}
#endregion
