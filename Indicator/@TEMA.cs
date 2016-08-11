// 
// Copyright (C) 2007, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//
#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Triple Exponential Moving Average
    /// </summary>
    [Description("Triple Exponential Moving Average")]
	[Gui.Design.DisplayName("TEMA (Triple Exponential Moving Average)")]
    public class TEMA : Indicator
    {
        #region Variables
        private int period = 14;
		private EMA ema1;
        private EMA ema2;
        private EMA ema3;

        #endregion

        protected override void OnStartUp()
        {
            ema1 = EMA(Inputs[0], Period);
            ema2 = EMA(ema1, Period);
            ema3 = EMA(ema2, Period);
        }

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			Add(new Plot(Color.Orange, "TEMA"));
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            Value.Set(3 * ema1[0] - 3 * ema2[0] + ema3[0]);
        }

        #region Properties

        [Description("Number of bars used for calculations")]
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
        private TEMA[] cacheTEMA = null;

        private static TEMA checkTEMA = new TEMA();

        /// <summary>
        /// Triple Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        public TEMA TEMA(int period)
        {
            return TEMA(Input, period);
        }

        /// <summary>
        /// Triple Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        public TEMA TEMA(Data.IDataSeries input, int period)
        {
            if (cacheTEMA != null)
                for (int idx = 0; idx < cacheTEMA.Length; idx++)
                    if (cacheTEMA[idx].Period == period && cacheTEMA[idx].EqualsInput(input))
                        return cacheTEMA[idx];

            lock (checkTEMA)
            {
                checkTEMA.Period = period;
                period = checkTEMA.Period;

                if (cacheTEMA != null)
                    for (int idx = 0; idx < cacheTEMA.Length; idx++)
                        if (cacheTEMA[idx].Period == period && cacheTEMA[idx].EqualsInput(input))
                            return cacheTEMA[idx];

                TEMA indicator = new TEMA();
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

                TEMA[] tmp = new TEMA[cacheTEMA == null ? 1 : cacheTEMA.Length + 1];
                if (cacheTEMA != null)
                    cacheTEMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTEMA = tmp;
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
        /// Triple Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TEMA TEMA(int period)
        {
            return _indicator.TEMA(Input, period);
        }

        /// <summary>
        /// Triple Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.TEMA TEMA(Data.IDataSeries input, int period)
        {
            return _indicator.TEMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Triple Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TEMA TEMA(int period)
        {
            return _indicator.TEMA(Input, period);
        }

        /// <summary>
        /// Triple Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.TEMA TEMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TEMA(input, period);
        }
    }
}
#endregion
