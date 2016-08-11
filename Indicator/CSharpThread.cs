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
    public class CSharpThread : Indicator
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
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            Print(string.Format("realtimeBar.Add(\"{0},{1},{2},{3},{4},{5}\");",Time[0],Open[0],High[0],Low[0],Close[0],Volume[0]));
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
        private CSharpThread[] cacheCSharpThread = null;

        private static CSharpThread checkCSharpThread = new CSharpThread();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public CSharpThread CSharpThread()
        {
            return CSharpThread(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public CSharpThread CSharpThread(Data.IDataSeries input)
        {
            if (cacheCSharpThread != null)
                for (int idx = 0; idx < cacheCSharpThread.Length; idx++)
                    if (cacheCSharpThread[idx].EqualsInput(input))
                        return cacheCSharpThread[idx];

            lock (checkCSharpThread)
            {
                if (cacheCSharpThread != null)
                    for (int idx = 0; idx < cacheCSharpThread.Length; idx++)
                        if (cacheCSharpThread[idx].EqualsInput(input))
                            return cacheCSharpThread[idx];

                CSharpThread indicator = new CSharpThread();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                CSharpThread[] tmp = new CSharpThread[cacheCSharpThread == null ? 1 : cacheCSharpThread.Length + 1];
                if (cacheCSharpThread != null)
                    cacheCSharpThread.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheCSharpThread = tmp;
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
        public Indicator.CSharpThread CSharpThread()
        {
            return _indicator.CSharpThread(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.CSharpThread CSharpThread(Data.IDataSeries input)
        {
            return _indicator.CSharpThread(input);
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
        public Indicator.CSharpThread CSharpThread()
        {
            return _indicator.CSharpThread(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.CSharpThread CSharpThread(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.CSharpThread(input);
        }
    }
}
#endregion
