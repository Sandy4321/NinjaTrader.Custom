// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
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
    /// Calculates the range of a bar.
    /// </summary>
    [Description("Calculates the range of a bar.")]
	public class Range : Indicator
    {
        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Firebrick, PlotStyle.Bar, "RangeValue"));
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            Value.Set(High[0] - Low[0]);
        }
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private Range[] cacheRange = null;

        private static Range checkRange = new Range();

        /// <summary>
        /// Calculates the range of a bar.
        /// </summary>
        /// <returns></returns>
        public Range Range()
        {
            return Range(Input);
        }

        /// <summary>
        /// Calculates the range of a bar.
        /// </summary>
        /// <returns></returns>
        public Range Range(Data.IDataSeries input)
        {
            if (cacheRange != null)
                for (int idx = 0; idx < cacheRange.Length; idx++)
                    if (cacheRange[idx].EqualsInput(input))
                        return cacheRange[idx];

            lock (checkRange)
            {
                if (cacheRange != null)
                    for (int idx = 0; idx < cacheRange.Length; idx++)
                        if (cacheRange[idx].EqualsInput(input))
                            return cacheRange[idx];

                Range indicator = new Range();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                Range[] tmp = new Range[cacheRange == null ? 1 : cacheRange.Length + 1];
                if (cacheRange != null)
                    cacheRange.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheRange = tmp;
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
        /// Calculates the range of a bar.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Range Range()
        {
            return _indicator.Range(Input);
        }

        /// <summary>
        /// Calculates the range of a bar.
        /// </summary>
        /// <returns></returns>
        public Indicator.Range Range(Data.IDataSeries input)
        {
            return _indicator.Range(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Calculates the range of a bar.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Range Range()
        {
            return _indicator.Range(Input);
        }

        /// <summary>
        /// Calculates the range of a bar.
        /// </summary>
        /// <returns></returns>
        public Indicator.Range Range(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Range(input);
        }
    }
}
#endregion
