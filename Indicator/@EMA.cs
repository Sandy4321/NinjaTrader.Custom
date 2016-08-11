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
	/// Exponential Moving Average. The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA applies more weight to recent prices than the SMA.
	/// </summary>
	[Description("The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA applies more weight to recent prices than the SMA.")]
	public class EMA : Indicator
	{
		#region Variables
		private int			period		= 14;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Orange, "EMA"));

			Overlay				= true;
		}
		
		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
            //MY PIECE OF CODE TO ONLY INCLUDE THE LINES FOR THE 5MIN CHART
            if (!((BarsPeriod.Id == PeriodType.Minute) && (BarsPeriod.Value == 5)))
            {
                return;
            }

			Value.Set(CurrentBar == 0 ? Input[0] : Input[0] * (2.0 / (1 + Period)) + (1 - (2.0 / (1 + Period))) * Value[1]);
		}

		#region Properties
		/// <summary>
		/// </summary>
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
        private EMA[] cacheEMA = null;

        private static EMA checkEMA = new EMA();

        /// <summary>
        /// The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA applies more weight to recent prices than the SMA.
        /// </summary>
        /// <returns></returns>
        public EMA EMA(int period)
        {
            return EMA(Input, period);
        }

        /// <summary>
        /// The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA applies more weight to recent prices than the SMA.
        /// </summary>
        /// <returns></returns>
        public EMA EMA(Data.IDataSeries input, int period)
        {
            if (cacheEMA != null)
                for (int idx = 0; idx < cacheEMA.Length; idx++)
                    if (cacheEMA[idx].Period == period && cacheEMA[idx].EqualsInput(input))
                        return cacheEMA[idx];

            lock (checkEMA)
            {
                checkEMA.Period = period;
                period = checkEMA.Period;

                if (cacheEMA != null)
                    for (int idx = 0; idx < cacheEMA.Length; idx++)
                        if (cacheEMA[idx].Period == period && cacheEMA[idx].EqualsInput(input))
                            return cacheEMA[idx];

                EMA indicator = new EMA();
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

                EMA[] tmp = new EMA[cacheEMA == null ? 1 : cacheEMA.Length + 1];
                if (cacheEMA != null)
                    cacheEMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheEMA = tmp;
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
        /// The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA applies more weight to recent prices than the SMA.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.EMA EMA(int period)
        {
            return _indicator.EMA(Input, period);
        }

        /// <summary>
        /// The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA applies more weight to recent prices than the SMA.
        /// </summary>
        /// <returns></returns>
        public Indicator.EMA EMA(Data.IDataSeries input, int period)
        {
            return _indicator.EMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA applies more weight to recent prices than the SMA.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.EMA EMA(int period)
        {
            return _indicator.EMA(Input, period);
        }

        /// <summary>
        /// The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA applies more weight to recent prices than the SMA.
        /// </summary>
        /// <returns></returns>
        public Indicator.EMA EMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.EMA(input, period);
        }
    }
}
#endregion
