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
    /// Moving average of volume
    /// </summary>
    [Description("Moving average of volume")]
    public class VolSMA : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period = 10; // Default setting for Period
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
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            
            // Calculate the volume average
            double _average = SMA(VOL(), period)[0];
            //double _volume = VOL()[0]; 
            
            Plot0.Set(_average);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot0
        {
            get { return Values[0]; }
        }

        [Description("Number of periods")]
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
        private VolSMA[] cacheVolSMA = null;

        private static VolSMA checkVolSMA = new VolSMA();

        /// <summary>
        /// Moving average of volume
        /// </summary>
        /// <returns></returns>
        public VolSMA VolSMA(int period)
        {
            return VolSMA(Input, period);
        }

        /// <summary>
        /// Moving average of volume
        /// </summary>
        /// <returns></returns>
        public VolSMA VolSMA(Data.IDataSeries input, int period)
        {
            if (cacheVolSMA != null)
                for (int idx = 0; idx < cacheVolSMA.Length; idx++)
                    if (cacheVolSMA[idx].Period == period && cacheVolSMA[idx].EqualsInput(input))
                        return cacheVolSMA[idx];

            lock (checkVolSMA)
            {
                checkVolSMA.Period = period;
                period = checkVolSMA.Period;

                if (cacheVolSMA != null)
                    for (int idx = 0; idx < cacheVolSMA.Length; idx++)
                        if (cacheVolSMA[idx].Period == period && cacheVolSMA[idx].EqualsInput(input))
                            return cacheVolSMA[idx];

                VolSMA indicator = new VolSMA();
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

                VolSMA[] tmp = new VolSMA[cacheVolSMA == null ? 1 : cacheVolSMA.Length + 1];
                if (cacheVolSMA != null)
                    cacheVolSMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheVolSMA = tmp;
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
        /// Moving average of volume
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolSMA VolSMA(int period)
        {
            return _indicator.VolSMA(Input, period);
        }

        /// <summary>
        /// Moving average of volume
        /// </summary>
        /// <returns></returns>
        public Indicator.VolSMA VolSMA(Data.IDataSeries input, int period)
        {
            return _indicator.VolSMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Moving average of volume
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolSMA VolSMA(int period)
        {
            return _indicator.VolSMA(Input, period);
        }

        /// <summary>
        /// Moving average of volume
        /// </summary>
        /// <returns></returns>
        public Indicator.VolSMA VolSMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.VolSMA(input, period);
        }
    }
}
#endregion
