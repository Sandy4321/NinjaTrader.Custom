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
    /// Stores intermediary calculations without the use of plots
    /// </summary>
    [Description("Stores intermediary calculations without the use of plots")]
    public class CustomDataSeries : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int sMAPeriod = 5; // Default setting for SMAPeriod
            private DataSeries myDataSeries; // Decleare a DataSeries variable 


        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "Plot0"));
            CalculateOnBarClose = true;
            Overlay				= false;

            myDataSeries = new DataSeries(this); // this referes to the indicator/strategy iteslf 
                                                    // and syncs the DataSeries object to historical data bars


        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            
            // Calculate the range of the current bar and se the value

            myDataSeries.Set(Close[0] - Open[0]);


            
            Plot0.Set(SMA(sMAPeriod)[0] + myDataSeries[0]);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot0
        {
            get { return Values[0]; }
        }

        [Description("Simple Moving Average Period")]
        [GridCategory("Parameters")]
        public int SMAPeriod
        {
            get { return sMAPeriod; }
            set { sMAPeriod = Math.Max(1, value); }
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
        private CustomDataSeries[] cacheCustomDataSeries = null;

        private static CustomDataSeries checkCustomDataSeries = new CustomDataSeries();

        /// <summary>
        /// Stores intermediary calculations without the use of plots
        /// </summary>
        /// <returns></returns>
        public CustomDataSeries CustomDataSeries(int sMAPeriod)
        {
            return CustomDataSeries(Input, sMAPeriod);
        }

        /// <summary>
        /// Stores intermediary calculations without the use of plots
        /// </summary>
        /// <returns></returns>
        public CustomDataSeries CustomDataSeries(Data.IDataSeries input, int sMAPeriod)
        {
            if (cacheCustomDataSeries != null)
                for (int idx = 0; idx < cacheCustomDataSeries.Length; idx++)
                    if (cacheCustomDataSeries[idx].SMAPeriod == sMAPeriod && cacheCustomDataSeries[idx].EqualsInput(input))
                        return cacheCustomDataSeries[idx];

            lock (checkCustomDataSeries)
            {
                checkCustomDataSeries.SMAPeriod = sMAPeriod;
                sMAPeriod = checkCustomDataSeries.SMAPeriod;

                if (cacheCustomDataSeries != null)
                    for (int idx = 0; idx < cacheCustomDataSeries.Length; idx++)
                        if (cacheCustomDataSeries[idx].SMAPeriod == sMAPeriod && cacheCustomDataSeries[idx].EqualsInput(input))
                            return cacheCustomDataSeries[idx];

                CustomDataSeries indicator = new CustomDataSeries();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.SMAPeriod = sMAPeriod;
                Indicators.Add(indicator);
                indicator.SetUp();

                CustomDataSeries[] tmp = new CustomDataSeries[cacheCustomDataSeries == null ? 1 : cacheCustomDataSeries.Length + 1];
                if (cacheCustomDataSeries != null)
                    cacheCustomDataSeries.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheCustomDataSeries = tmp;
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
        /// Stores intermediary calculations without the use of plots
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.CustomDataSeries CustomDataSeries(int sMAPeriod)
        {
            return _indicator.CustomDataSeries(Input, sMAPeriod);
        }

        /// <summary>
        /// Stores intermediary calculations without the use of plots
        /// </summary>
        /// <returns></returns>
        public Indicator.CustomDataSeries CustomDataSeries(Data.IDataSeries input, int sMAPeriod)
        {
            return _indicator.CustomDataSeries(input, sMAPeriod);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Stores intermediary calculations without the use of plots
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.CustomDataSeries CustomDataSeries(int sMAPeriod)
        {
            return _indicator.CustomDataSeries(Input, sMAPeriod);
        }

        /// <summary>
        /// Stores intermediary calculations without the use of plots
        /// </summary>
        /// <returns></returns>
        public Indicator.CustomDataSeries CustomDataSeries(Data.IDataSeries input, int sMAPeriod)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.CustomDataSeries(input, sMAPeriod);
        }
    }
}
#endregion
