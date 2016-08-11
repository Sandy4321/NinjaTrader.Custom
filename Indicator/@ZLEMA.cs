// 
// Copyright (C) 2010, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

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
    /// The ZLEMA (Zero-Lag Exponential Moving Average) is an EMA variant that attempts to adjust for lag.
    /// </summary>
    [Description("The ZLEMA (Zero-Lag Exponential Moving Average) is an EMA variant that attempts to adjust for lag.")]
    public class ZLEMA : Indicator
    {
        #region Variables
		private int		period		= 14; // Default setting for Period
		private double	k			= 0;
		private double	oneMinusK	= 0;
		private int		lag			= 0;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Orange, "ZLEMA"));
            Overlay	= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar == 0)
			{
				k			= 2.0 / (Period + 1);
				oneMinusK	= 1 - k;
				lag			= (int) Math.Ceiling((Period - 1) / 2.0);
			}
			else if (CurrentBar >= lag)
            	Value.Set(k * (2 * Input[0] - Input[lag]) + oneMinusK * Value[1]);
        }

        #region Properties
        [Description("Numbers of bars used for calculations")]
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
        private ZLEMA[] cacheZLEMA = null;

        private static ZLEMA checkZLEMA = new ZLEMA();

        /// <summary>
        /// The ZLEMA (Zero-Lag Exponential Moving Average) is an EMA variant that attempts to adjust for lag.
        /// </summary>
        /// <returns></returns>
        public ZLEMA ZLEMA(int period)
        {
            return ZLEMA(Input, period);
        }

        /// <summary>
        /// The ZLEMA (Zero-Lag Exponential Moving Average) is an EMA variant that attempts to adjust for lag.
        /// </summary>
        /// <returns></returns>
        public ZLEMA ZLEMA(Data.IDataSeries input, int period)
        {
            if (cacheZLEMA != null)
                for (int idx = 0; idx < cacheZLEMA.Length; idx++)
                    if (cacheZLEMA[idx].Period == period && cacheZLEMA[idx].EqualsInput(input))
                        return cacheZLEMA[idx];

            lock (checkZLEMA)
            {
                checkZLEMA.Period = period;
                period = checkZLEMA.Period;

                if (cacheZLEMA != null)
                    for (int idx = 0; idx < cacheZLEMA.Length; idx++)
                        if (cacheZLEMA[idx].Period == period && cacheZLEMA[idx].EqualsInput(input))
                            return cacheZLEMA[idx];

                ZLEMA indicator = new ZLEMA();
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

                ZLEMA[] tmp = new ZLEMA[cacheZLEMA == null ? 1 : cacheZLEMA.Length + 1];
                if (cacheZLEMA != null)
                    cacheZLEMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZLEMA = tmp;
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
        /// The ZLEMA (Zero-Lag Exponential Moving Average) is an EMA variant that attempts to adjust for lag.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZLEMA ZLEMA(int period)
        {
            return _indicator.ZLEMA(Input, period);
        }

        /// <summary>
        /// The ZLEMA (Zero-Lag Exponential Moving Average) is an EMA variant that attempts to adjust for lag.
        /// </summary>
        /// <returns></returns>
        public Indicator.ZLEMA ZLEMA(Data.IDataSeries input, int period)
        {
            return _indicator.ZLEMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The ZLEMA (Zero-Lag Exponential Moving Average) is an EMA variant that attempts to adjust for lag.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZLEMA ZLEMA(int period)
        {
            return _indicator.ZLEMA(Input, period);
        }

        /// <summary>
        /// The ZLEMA (Zero-Lag Exponential Moving Average) is an EMA variant that attempts to adjust for lag.
        /// </summary>
        /// <returns></returns>
        public Indicator.ZLEMA ZLEMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZLEMA(input, period);
        }
    }
}
#endregion
