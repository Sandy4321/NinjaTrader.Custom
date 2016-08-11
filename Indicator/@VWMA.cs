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
	/// The VWMA (Volume-Weighted Moving Average) returns the volume-weighted moving average for the specified price series and period. VWMA is similar to a Simple Moving Average (SMA), but each bar of data is weighted by the bar's Volume. VWMA places more significance on the days with the largest volume and the least for the days with lowest volume for the period specified.
	/// </summary>
	[Description("The VWMA (Volume-Weighted Moving Average) returns the volume-weighted moving average for the specified price series and period. VWMA is similar to a Simple Moving Average (SMA), but each bar of data is weighted by the bar's Volume. VWMA places more significance on the days with the largest volume and the least for the days with lowest volume for the period specified.")]
	public class VWMA : Indicator
	{
		#region Variables
		private int			period		= 14;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Blue, "VWMA"));

			Overlay				= true;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			int numBars = Math.Min(CurrentBar, period);

			double volPriceSum  = 0;
			double volSum		= 0;

			for (int i = 0; i < numBars; i++)
			{
				volPriceSum	+= Input[i] * Volume[i];
				volSum		+= Volume[i];
			}

			// Protect agains div by zero evilness
			if (volSum <= double.Epsilon)
				Value.Set(volPriceSum);
			else
				Value.Set(volPriceSum / volSum);
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculations.")]
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
        private VWMA[] cacheVWMA = null;

        private static VWMA checkVWMA = new VWMA();

        /// <summary>
        /// The VWMA (Volume-Weighted Moving Average) returns the volume-weighted moving average for the specified price series and period. VWMA is similar to a Simple Moving Average (SMA), but each bar of data is weighted by the bar's Volume. VWMA places more significance on the days with the largest volume and the least for the days with lowest volume for the period specified.
        /// </summary>
        /// <returns></returns>
        public VWMA VWMA(int period)
        {
            return VWMA(Input, period);
        }

        /// <summary>
        /// The VWMA (Volume-Weighted Moving Average) returns the volume-weighted moving average for the specified price series and period. VWMA is similar to a Simple Moving Average (SMA), but each bar of data is weighted by the bar's Volume. VWMA places more significance on the days with the largest volume and the least for the days with lowest volume for the period specified.
        /// </summary>
        /// <returns></returns>
        public VWMA VWMA(Data.IDataSeries input, int period)
        {
            if (cacheVWMA != null)
                for (int idx = 0; idx < cacheVWMA.Length; idx++)
                    if (cacheVWMA[idx].Period == period && cacheVWMA[idx].EqualsInput(input))
                        return cacheVWMA[idx];

            lock (checkVWMA)
            {
                checkVWMA.Period = period;
                period = checkVWMA.Period;

                if (cacheVWMA != null)
                    for (int idx = 0; idx < cacheVWMA.Length; idx++)
                        if (cacheVWMA[idx].Period == period && cacheVWMA[idx].EqualsInput(input))
                            return cacheVWMA[idx];

                VWMA indicator = new VWMA();
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

                VWMA[] tmp = new VWMA[cacheVWMA == null ? 1 : cacheVWMA.Length + 1];
                if (cacheVWMA != null)
                    cacheVWMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheVWMA = tmp;
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
        /// The VWMA (Volume-Weighted Moving Average) returns the volume-weighted moving average for the specified price series and period. VWMA is similar to a Simple Moving Average (SMA), but each bar of data is weighted by the bar's Volume. VWMA places more significance on the days with the largest volume and the least for the days with lowest volume for the period specified.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VWMA VWMA(int period)
        {
            return _indicator.VWMA(Input, period);
        }

        /// <summary>
        /// The VWMA (Volume-Weighted Moving Average) returns the volume-weighted moving average for the specified price series and period. VWMA is similar to a Simple Moving Average (SMA), but each bar of data is weighted by the bar's Volume. VWMA places more significance on the days with the largest volume and the least for the days with lowest volume for the period specified.
        /// </summary>
        /// <returns></returns>
        public Indicator.VWMA VWMA(Data.IDataSeries input, int period)
        {
            return _indicator.VWMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The VWMA (Volume-Weighted Moving Average) returns the volume-weighted moving average for the specified price series and period. VWMA is similar to a Simple Moving Average (SMA), but each bar of data is weighted by the bar's Volume. VWMA places more significance on the days with the largest volume and the least for the days with lowest volume for the period specified.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VWMA VWMA(int period)
        {
            return _indicator.VWMA(Input, period);
        }

        /// <summary>
        /// The VWMA (Volume-Weighted Moving Average) returns the volume-weighted moving average for the specified price series and period. VWMA is similar to a Simple Moving Average (SMA), but each bar of data is weighted by the bar's Volume. VWMA places more significance on the days with the largest volume and the least for the days with lowest volume for the period specified.
        /// </summary>
        /// <returns></returns>
        public Indicator.VWMA VWMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.VWMA(input, period);
        }
    }
}
#endregion
