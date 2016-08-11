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
	/// Returns a value of 1 when the current close is less than the prior close after penetrating the highest high of the last n bars.
    /// </summary>
    [Description("Returns a value of 1 when the current close is less than the prior close after penetrating the highest high of the last n bars.")]
    public class KeyReversalDown : Indicator
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
			
			Value.Set(High[0] > MAX(High, Period)[1] && Close[0] < Close[1] ? 1: 0);
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
        private KeyReversalDown[] cacheKeyReversalDown = null;

        private static KeyReversalDown checkKeyReversalDown = new KeyReversalDown();

        /// <summary>
        /// Returns a value of 1 when the current close is less than the prior close after penetrating the highest high of the last n bars.
        /// </summary>
        /// <returns></returns>
        public KeyReversalDown KeyReversalDown(int period)
        {
            return KeyReversalDown(Input, period);
        }

        /// <summary>
        /// Returns a value of 1 when the current close is less than the prior close after penetrating the highest high of the last n bars.
        /// </summary>
        /// <returns></returns>
        public KeyReversalDown KeyReversalDown(Data.IDataSeries input, int period)
        {
            if (cacheKeyReversalDown != null)
                for (int idx = 0; idx < cacheKeyReversalDown.Length; idx++)
                    if (cacheKeyReversalDown[idx].Period == period && cacheKeyReversalDown[idx].EqualsInput(input))
                        return cacheKeyReversalDown[idx];

            lock (checkKeyReversalDown)
            {
                checkKeyReversalDown.Period = period;
                period = checkKeyReversalDown.Period;

                if (cacheKeyReversalDown != null)
                    for (int idx = 0; idx < cacheKeyReversalDown.Length; idx++)
                        if (cacheKeyReversalDown[idx].Period == period && cacheKeyReversalDown[idx].EqualsInput(input))
                            return cacheKeyReversalDown[idx];

                KeyReversalDown indicator = new KeyReversalDown();
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

                KeyReversalDown[] tmp = new KeyReversalDown[cacheKeyReversalDown == null ? 1 : cacheKeyReversalDown.Length + 1];
                if (cacheKeyReversalDown != null)
                    cacheKeyReversalDown.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheKeyReversalDown = tmp;
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
        /// Returns a value of 1 when the current close is less than the prior close after penetrating the highest high of the last n bars.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.KeyReversalDown KeyReversalDown(int period)
        {
            return _indicator.KeyReversalDown(Input, period);
        }

        /// <summary>
        /// Returns a value of 1 when the current close is less than the prior close after penetrating the highest high of the last n bars.
        /// </summary>
        /// <returns></returns>
        public Indicator.KeyReversalDown KeyReversalDown(Data.IDataSeries input, int period)
        {
            return _indicator.KeyReversalDown(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Returns a value of 1 when the current close is less than the prior close after penetrating the highest high of the last n bars.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.KeyReversalDown KeyReversalDown(int period)
        {
            return _indicator.KeyReversalDown(Input, period);
        }

        /// <summary>
        /// Returns a value of 1 when the current close is less than the prior close after penetrating the highest high of the last n bars.
        /// </summary>
        /// <returns></returns>
        public Indicator.KeyReversalDown KeyReversalDown(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.KeyReversalDown(input, period);
        }
    }
}
#endregion
