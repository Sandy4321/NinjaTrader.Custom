// 
// Copyright (C) 2008, NinjaTrader LLC <www.ninjatrader.com>.
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
    /// Double Exponential Moving Average
    /// </summary>
    [Description("Double Exponential Moving Average")]
	[Gui.Design.DisplayName("DEMA (Double Exponential Moving Average)")]
    public class DEMA : Indicator
    {
        #region Variables
            private int period = 14;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			Add(new Plot(Color.Orange, "DEMA"));
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			Value.Set(2 * EMA(Inputs[0], Period)[0] -  EMA(EMA(Inputs[0], Period), Period)[0]);
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
        private DEMA[] cacheDEMA = null;

        private static DEMA checkDEMA = new DEMA();

        /// <summary>
        /// Double Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        public DEMA DEMA(int period)
        {
            return DEMA(Input, period);
        }

        /// <summary>
        /// Double Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        public DEMA DEMA(Data.IDataSeries input, int period)
        {
            if (cacheDEMA != null)
                for (int idx = 0; idx < cacheDEMA.Length; idx++)
                    if (cacheDEMA[idx].Period == period && cacheDEMA[idx].EqualsInput(input))
                        return cacheDEMA[idx];

            lock (checkDEMA)
            {
                checkDEMA.Period = period;
                period = checkDEMA.Period;

                if (cacheDEMA != null)
                    for (int idx = 0; idx < cacheDEMA.Length; idx++)
                        if (cacheDEMA[idx].Period == period && cacheDEMA[idx].EqualsInput(input))
                            return cacheDEMA[idx];

                DEMA indicator = new DEMA();
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

                DEMA[] tmp = new DEMA[cacheDEMA == null ? 1 : cacheDEMA.Length + 1];
                if (cacheDEMA != null)
                    cacheDEMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheDEMA = tmp;
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
        /// Double Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DEMA DEMA(int period)
        {
            return _indicator.DEMA(Input, period);
        }

        /// <summary>
        /// Double Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.DEMA DEMA(Data.IDataSeries input, int period)
        {
            return _indicator.DEMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Double Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DEMA DEMA(int period)
        {
            return _indicator.DEMA(Input, period);
        }

        /// <summary>
        /// Double Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.DEMA DEMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.DEMA(input, period);
        }
    }
}
#endregion
