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
	/// The VMA (Variable Moving Average, also known as VIDYA or Variable Index Dynamic Average) is an exponential moving average that automatically adjusts the smoothing weight based on the volatility of the data series. VMA solves a problem with most moving averages. In times of low volatility, such as when the price is trending, the moving average time period should be shorter to be sensitive to the inevitable break in the trend. Whereas, in more volatile non-trending times, the moving average time period should be longer to filter out the choppiness. VIDYA uses the CMO indicator for it's internal volatility calculations. Both the VMA and the CMO period are adjustable.
	/// </summary>
	[Description("The VMA (Variable Moving Average, also known as VIDYA or Variable Index Dynamic Average) is an exponential moving average that automatically adjusts the smoothing weight based on the volatility of the data series. VMA solves a problem with most moving averages. In times of low volatility, such as when the price is trending, the moving average time period should be shorter to be sensitive to the inevitable break in the trend. Whereas, in more volatile non-trending times, the moving average time period should be longer to filter out the choppiness. VIDYA uses the CMO indicator for it's internal volatility calculations. Both the VMA and the CMO period are adjustable.")]
	public class VMA : Indicator
	{
		#region Variables
		private int		period			 = 9;
		private int		volatilityPeriod = 9;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Blue, "VMA"));

			Overlay = true;
		}
		
		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0) 
			{
				Value.Set(Input[0]);
				return;
			}

			// Smoothing constant
			double sc  = 2.0 / (double)(period + 1);

			// Volatility index
			double vi = Math.Abs(CMO(Inputs[0], volatilityPeriod)[0]) / 100;

			double vma = sc * vi * Input[0] + (1 - sc * vi) * Value[1];
			Value.Set(vma);
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("VMA period.")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set	{ period = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Period used to calculate the CMO-based volatility index.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Volatility period")]
		public int VolatilityPeriod
		{
			get { return volatilityPeriod; }
			set	{ volatilityPeriod = Math.Max(1, value); }
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
        private VMA[] cacheVMA = null;

        private static VMA checkVMA = new VMA();

        /// <summary>
        /// The VMA (Variable Moving Average, also known as VIDYA or Variable Index Dynamic Average) is an exponential moving average that automatically adjusts the smoothing weight based on the volatility of the data series. VMA solves a problem with most moving averages. In times of low volatility, such as when the price is trending, the moving average time period should be shorter to be sensitive to the inevitable break in the trend. Whereas, in more volatile non-trending times, the moving average time period should be longer to filter out the choppiness. VIDYA uses the CMO indicator for it's internal volatility calculations. Both the VMA and the CMO period are adjustable.
        /// </summary>
        /// <returns></returns>
        public VMA VMA(int period, int volatilityPeriod)
        {
            return VMA(Input, period, volatilityPeriod);
        }

        /// <summary>
        /// The VMA (Variable Moving Average, also known as VIDYA or Variable Index Dynamic Average) is an exponential moving average that automatically adjusts the smoothing weight based on the volatility of the data series. VMA solves a problem with most moving averages. In times of low volatility, such as when the price is trending, the moving average time period should be shorter to be sensitive to the inevitable break in the trend. Whereas, in more volatile non-trending times, the moving average time period should be longer to filter out the choppiness. VIDYA uses the CMO indicator for it's internal volatility calculations. Both the VMA and the CMO period are adjustable.
        /// </summary>
        /// <returns></returns>
        public VMA VMA(Data.IDataSeries input, int period, int volatilityPeriod)
        {
            if (cacheVMA != null)
                for (int idx = 0; idx < cacheVMA.Length; idx++)
                    if (cacheVMA[idx].Period == period && cacheVMA[idx].VolatilityPeriod == volatilityPeriod && cacheVMA[idx].EqualsInput(input))
                        return cacheVMA[idx];

            lock (checkVMA)
            {
                checkVMA.Period = period;
                period = checkVMA.Period;
                checkVMA.VolatilityPeriod = volatilityPeriod;
                volatilityPeriod = checkVMA.VolatilityPeriod;

                if (cacheVMA != null)
                    for (int idx = 0; idx < cacheVMA.Length; idx++)
                        if (cacheVMA[idx].Period == period && cacheVMA[idx].VolatilityPeriod == volatilityPeriod && cacheVMA[idx].EqualsInput(input))
                            return cacheVMA[idx];

                VMA indicator = new VMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                indicator.VolatilityPeriod = volatilityPeriod;
                Indicators.Add(indicator);
                indicator.SetUp();

                VMA[] tmp = new VMA[cacheVMA == null ? 1 : cacheVMA.Length + 1];
                if (cacheVMA != null)
                    cacheVMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheVMA = tmp;
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
        /// The VMA (Variable Moving Average, also known as VIDYA or Variable Index Dynamic Average) is an exponential moving average that automatically adjusts the smoothing weight based on the volatility of the data series. VMA solves a problem with most moving averages. In times of low volatility, such as when the price is trending, the moving average time period should be shorter to be sensitive to the inevitable break in the trend. Whereas, in more volatile non-trending times, the moving average time period should be longer to filter out the choppiness. VIDYA uses the CMO indicator for it's internal volatility calculations. Both the VMA and the CMO period are adjustable.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VMA VMA(int period, int volatilityPeriod)
        {
            return _indicator.VMA(Input, period, volatilityPeriod);
        }

        /// <summary>
        /// The VMA (Variable Moving Average, also known as VIDYA or Variable Index Dynamic Average) is an exponential moving average that automatically adjusts the smoothing weight based on the volatility of the data series. VMA solves a problem with most moving averages. In times of low volatility, such as when the price is trending, the moving average time period should be shorter to be sensitive to the inevitable break in the trend. Whereas, in more volatile non-trending times, the moving average time period should be longer to filter out the choppiness. VIDYA uses the CMO indicator for it's internal volatility calculations. Both the VMA and the CMO period are adjustable.
        /// </summary>
        /// <returns></returns>
        public Indicator.VMA VMA(Data.IDataSeries input, int period, int volatilityPeriod)
        {
            return _indicator.VMA(input, period, volatilityPeriod);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The VMA (Variable Moving Average, also known as VIDYA or Variable Index Dynamic Average) is an exponential moving average that automatically adjusts the smoothing weight based on the volatility of the data series. VMA solves a problem with most moving averages. In times of low volatility, such as when the price is trending, the moving average time period should be shorter to be sensitive to the inevitable break in the trend. Whereas, in more volatile non-trending times, the moving average time period should be longer to filter out the choppiness. VIDYA uses the CMO indicator for it's internal volatility calculations. Both the VMA and the CMO period are adjustable.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VMA VMA(int period, int volatilityPeriod)
        {
            return _indicator.VMA(Input, period, volatilityPeriod);
        }

        /// <summary>
        /// The VMA (Variable Moving Average, also known as VIDYA or Variable Index Dynamic Average) is an exponential moving average that automatically adjusts the smoothing weight based on the volatility of the data series. VMA solves a problem with most moving averages. In times of low volatility, such as when the price is trending, the moving average time period should be shorter to be sensitive to the inevitable break in the trend. Whereas, in more volatile non-trending times, the moving average time period should be longer to filter out the choppiness. VIDYA uses the CMO indicator for it's internal volatility calculations. Both the VMA and the CMO period are adjustable.
        /// </summary>
        /// <returns></returns>
        public Indicator.VMA VMA(Data.IDataSeries input, int period, int volatilityPeriod)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.VMA(input, period, volatilityPeriod);
        }
    }
}
#endregion
