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
    /// The Dynamic Momentum Index is a variable term RSI. The RSI term varies from 3 to 30. The variable time period makes the RSI more responsive to short-term moves. The more volatile the price is, the shorter the time period is. It is interpreted in the same way as the RSI, but provides signals earlier.
    /// </summary>
    [Description("The Dynamic Momentum Index (DMI) as developed by Tushar Chande and Stanley Kroll and outlined in detail in their book The New Technical Trader.")]
    public class DMIndex : Indicator
    {
        #region Variables
		private int smooth = 3;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Green, PlotStyle.Line, "DMIndex"));
            Overlay				= false;
            PriceTypeSupported	= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            Values[0].Set(RSI((int)(14 / (StdDev(5)[0] / SMA(StdDev(5), 10)[0])), Smooth)[0]);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        [Description("RSI's smooth factor")]
        [GridCategory("Parameters")]
        public int Smooth
        {
            get { return smooth; }
            set { smooth = Math.Max(1, value); }
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
        private DMIndex[] cacheDMIndex = null;

        private static DMIndex checkDMIndex = new DMIndex();

        /// <summary>
        /// The Dynamic Momentum Index (DMI) as developed by Tushar Chande and Stanley Kroll and outlined in detail in their book The New Technical Trader.
        /// </summary>
        /// <returns></returns>
        public DMIndex DMIndex(int smooth)
        {
            return DMIndex(Input, smooth);
        }

        /// <summary>
        /// The Dynamic Momentum Index (DMI) as developed by Tushar Chande and Stanley Kroll and outlined in detail in their book The New Technical Trader.
        /// </summary>
        /// <returns></returns>
        public DMIndex DMIndex(Data.IDataSeries input, int smooth)
        {
            if (cacheDMIndex != null)
                for (int idx = 0; idx < cacheDMIndex.Length; idx++)
                    if (cacheDMIndex[idx].Smooth == smooth && cacheDMIndex[idx].EqualsInput(input))
                        return cacheDMIndex[idx];

            lock (checkDMIndex)
            {
                checkDMIndex.Smooth = smooth;
                smooth = checkDMIndex.Smooth;

                if (cacheDMIndex != null)
                    for (int idx = 0; idx < cacheDMIndex.Length; idx++)
                        if (cacheDMIndex[idx].Smooth == smooth && cacheDMIndex[idx].EqualsInput(input))
                            return cacheDMIndex[idx];

                DMIndex indicator = new DMIndex();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Smooth = smooth;
                Indicators.Add(indicator);
                indicator.SetUp();

                DMIndex[] tmp = new DMIndex[cacheDMIndex == null ? 1 : cacheDMIndex.Length + 1];
                if (cacheDMIndex != null)
                    cacheDMIndex.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheDMIndex = tmp;
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
        /// The Dynamic Momentum Index (DMI) as developed by Tushar Chande and Stanley Kroll and outlined in detail in their book The New Technical Trader.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DMIndex DMIndex(int smooth)
        {
            return _indicator.DMIndex(Input, smooth);
        }

        /// <summary>
        /// The Dynamic Momentum Index (DMI) as developed by Tushar Chande and Stanley Kroll and outlined in detail in their book The New Technical Trader.
        /// </summary>
        /// <returns></returns>
        public Indicator.DMIndex DMIndex(Data.IDataSeries input, int smooth)
        {
            return _indicator.DMIndex(input, smooth);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Dynamic Momentum Index (DMI) as developed by Tushar Chande and Stanley Kroll and outlined in detail in their book The New Technical Trader.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DMIndex DMIndex(int smooth)
        {
            return _indicator.DMIndex(Input, smooth);
        }

        /// <summary>
        /// The Dynamic Momentum Index (DMI) as developed by Tushar Chande and Stanley Kroll and outlined in detail in their book The New Technical Trader.
        /// </summary>
        /// <returns></returns>
        public Indicator.DMIndex DMIndex(Data.IDataSeries input, int smooth)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.DMIndex(input, smooth);
        }
    }
}
#endregion
