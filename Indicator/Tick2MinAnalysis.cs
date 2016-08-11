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
    public class Tick2MinAnalysis : Indicator
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
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "Plot0"));
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
            Plot0.Set(Close[0]);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot0
        {
            get { return Values[0]; }
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
        private Tick2MinAnalysis[] cacheTick2MinAnalysis = null;

        private static Tick2MinAnalysis checkTick2MinAnalysis = new Tick2MinAnalysis();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Tick2MinAnalysis Tick2MinAnalysis()
        {
            return Tick2MinAnalysis(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Tick2MinAnalysis Tick2MinAnalysis(Data.IDataSeries input)
        {
            if (cacheTick2MinAnalysis != null)
                for (int idx = 0; idx < cacheTick2MinAnalysis.Length; idx++)
                    if (cacheTick2MinAnalysis[idx].EqualsInput(input))
                        return cacheTick2MinAnalysis[idx];

            lock (checkTick2MinAnalysis)
            {
                if (cacheTick2MinAnalysis != null)
                    for (int idx = 0; idx < cacheTick2MinAnalysis.Length; idx++)
                        if (cacheTick2MinAnalysis[idx].EqualsInput(input))
                            return cacheTick2MinAnalysis[idx];

                Tick2MinAnalysis indicator = new Tick2MinAnalysis();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                Tick2MinAnalysis[] tmp = new Tick2MinAnalysis[cacheTick2MinAnalysis == null ? 1 : cacheTick2MinAnalysis.Length + 1];
                if (cacheTick2MinAnalysis != null)
                    cacheTick2MinAnalysis.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTick2MinAnalysis = tmp;
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
        public Indicator.Tick2MinAnalysis Tick2MinAnalysis()
        {
            return _indicator.Tick2MinAnalysis(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.Tick2MinAnalysis Tick2MinAnalysis(Data.IDataSeries input)
        {
            return _indicator.Tick2MinAnalysis(input);
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
        public Indicator.Tick2MinAnalysis Tick2MinAnalysis()
        {
            return _indicator.Tick2MinAnalysis(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.Tick2MinAnalysis Tick2MinAnalysis(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Tick2MinAnalysis(input);
        }
    }
}
#endregion
