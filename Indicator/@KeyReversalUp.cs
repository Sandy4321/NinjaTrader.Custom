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
	/// Returns a value of 1 when the current close is greater than the prior close after penetrating the lowest low of the last n bars.
    /// </summary>
    [Description("Returns a value of 1 when the current close is greater than the prior close after penetrating the lowest low of the last n bars.")]
    public class KeyReversalUp : Indicator
    {
        #region Variable
            private int period = 1;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Blue, PlotStyle.Bar, "Plot0"));
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            if (CurrentBar < Period + 1)
				return;
			
			Value.Set(Low[0] < MIN(Low, Period)[1] && Close[0] > Close[1] ? 1: 0);
        }

        #region Properties
        [Description("Look back period.")]
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
        private KeyReversalUp[] cacheKeyReversalUp = null;

        private static KeyReversalUp checkKeyReversalUp = new KeyReversalUp();

        /// <summary>
        /// Returns a value of 1 when the current close is greater than the prior close after penetrating the lowest low of the last n bars.
        /// </summary>
        /// <returns></returns>
        public KeyReversalUp KeyReversalUp(int period)
        {
            return KeyReversalUp(Input, period);
        }

        /// <summary>
        /// Returns a value of 1 when the current close is greater than the prior close after penetrating the lowest low of the last n bars.
        /// </summary>
        /// <returns></returns>
        public KeyReversalUp KeyReversalUp(Data.IDataSeries input, int period)
        {
            if (cacheKeyReversalUp != null)
                for (int idx = 0; idx < cacheKeyReversalUp.Length; idx++)
                    if (cacheKeyReversalUp[idx].Period == period && cacheKeyReversalUp[idx].EqualsInput(input))
                        return cacheKeyReversalUp[idx];

            lock (checkKeyReversalUp)
            {
                checkKeyReversalUp.Period = period;
                period = checkKeyReversalUp.Period;

                if (cacheKeyReversalUp != null)
                    for (int idx = 0; idx < cacheKeyReversalUp.Length; idx++)
                        if (cacheKeyReversalUp[idx].Period == period && cacheKeyReversalUp[idx].EqualsInput(input))
                            return cacheKeyReversalUp[idx];

                KeyReversalUp indicator = new KeyReversalUp();
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

                KeyReversalUp[] tmp = new KeyReversalUp[cacheKeyReversalUp == null ? 1 : cacheKeyReversalUp.Length + 1];
                if (cacheKeyReversalUp != null)
                    cacheKeyReversalUp.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheKeyReversalUp = tmp;
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
        /// Returns a value of 1 when the current close is greater than the prior close after penetrating the lowest low of the last n bars.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.KeyReversalUp KeyReversalUp(int period)
        {
            return _indicator.KeyReversalUp(Input, period);
        }

        /// <summary>
        /// Returns a value of 1 when the current close is greater than the prior close after penetrating the lowest low of the last n bars.
        /// </summary>
        /// <returns></returns>
        public Indicator.KeyReversalUp KeyReversalUp(Data.IDataSeries input, int period)
        {
            return _indicator.KeyReversalUp(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Returns a value of 1 when the current close is greater than the prior close after penetrating the lowest low of the last n bars.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.KeyReversalUp KeyReversalUp(int period)
        {
            return _indicator.KeyReversalUp(Input, period);
        }

        /// <summary>
        /// Returns a value of 1 when the current close is greater than the prior close after penetrating the lowest low of the last n bars.
        /// </summary>
        /// <returns></returns>
        public Indicator.KeyReversalUp KeyReversalUp(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.KeyReversalUp(input, period);
        }
    }
}
#endregion
