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
    /// Indicates where the open price is equal to the close pricemind
    /// </summary>
    [Description("Indicates where the open price is equal to the close price")]
    public class OpenIsClose : Indicator
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

  

            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Bar, "Plot0"));
            CalculateOnBarClose = true;
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
            Plot0.Set(Open[0] == Close[0] ? 1: 0);
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
        private OpenIsClose[] cacheOpenIsClose = null;

        private static OpenIsClose checkOpenIsClose = new OpenIsClose();

        /// <summary>
        /// Indicates where the open price is equal to the close price
        /// </summary>
        /// <returns></returns>
        public OpenIsClose OpenIsClose()
        {
            return OpenIsClose(Input);
        }

        /// <summary>
        /// Indicates where the open price is equal to the close price
        /// </summary>
        /// <returns></returns>
        public OpenIsClose OpenIsClose(Data.IDataSeries input)
        {
            if (cacheOpenIsClose != null)
                for (int idx = 0; idx < cacheOpenIsClose.Length; idx++)
                    if (cacheOpenIsClose[idx].EqualsInput(input))
                        return cacheOpenIsClose[idx];

            lock (checkOpenIsClose)
            {
                if (cacheOpenIsClose != null)
                    for (int idx = 0; idx < cacheOpenIsClose.Length; idx++)
                        if (cacheOpenIsClose[idx].EqualsInput(input))
                            return cacheOpenIsClose[idx];

                OpenIsClose indicator = new OpenIsClose();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                OpenIsClose[] tmp = new OpenIsClose[cacheOpenIsClose == null ? 1 : cacheOpenIsClose.Length + 1];
                if (cacheOpenIsClose != null)
                    cacheOpenIsClose.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheOpenIsClose = tmp;
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
        /// Indicates where the open price is equal to the close price
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.OpenIsClose OpenIsClose()
        {
            return _indicator.OpenIsClose(Input);
        }

        /// <summary>
        /// Indicates where the open price is equal to the close price
        /// </summary>
        /// <returns></returns>
        public Indicator.OpenIsClose OpenIsClose(Data.IDataSeries input)
        {
            return _indicator.OpenIsClose(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Indicates where the open price is equal to the close price
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.OpenIsClose OpenIsClose()
        {
            return _indicator.OpenIsClose(Input);
        }

        /// <summary>
        /// Indicates where the open price is equal to the close price
        /// </summary>
        /// <returns></returns>
        public Indicator.OpenIsClose OpenIsClose(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.OpenIsClose(input);
        }
    }
}
#endregion
