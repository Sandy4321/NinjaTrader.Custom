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
	/// Standard Deviation is a statistical measure of volatility. Standard Deviation is typically used as a component of other indicators, rather than as a stand-alone indicator. For example, Bollinger Bands are calculated by adding a security's Standard Deviation to a moving average.
	/// </summary>
	[Description("Standard Deviation is a statistical measure of volatility. Standard Deviation is typically used as a component of other indicators, rather than as a stand-alone indicator. For example, Bollinger Bands are calculated by adding a security's Standard Deviation to a moving average.")]
	public class StdDev : Indicator
	{
		#region Variables
		private int		    period	= 14;
	    private DataSeries  sumSeries;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Green, "StdDev"));
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
            if (CurrentBar < 1)
            {
                Value.Set(0);
                sumSeries.Set(Input[0]);
            }
            else
            {
                sumSeries.Set(Input[0] + sumSeries[1] - (CurrentBar >= Period ? Input[Period] : 0));
                double avg = sumSeries[0] / Math.Min(CurrentBar + 1, Period);
                double sum = 0;
                for (int barsBack = Math.Min(CurrentBar, Period - 1); barsBack >= 0; barsBack--)
                    sum += (Input[barsBack] - avg) * (Input[barsBack] - avg);

                Value.Set(Math.Sqrt(sum / Math.Min(CurrentBar + 1, Period)));
            }
		}

        protected override void OnStartUp()
        {
            sumSeries =  new DataSeries(this, period <= 256 ? MaximumBarsLookBack.TwoHundredFiftySix : MaximumBarsLookBack.Infinite);
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
        private StdDev[] cacheStdDev = null;

        private static StdDev checkStdDev = new StdDev();

        /// <summary>
        /// Standard Deviation is a statistical measure of volatility. Standard Deviation is typically used as a component of other indicators, rather than as a stand-alone indicator. For example, Bollinger Bands are calculated by adding a security's Standard Deviation to a moving average.
        /// </summary>
        /// <returns></returns>
        public StdDev StdDev(int period)
        {
            return StdDev(Input, period);
        }

        /// <summary>
        /// Standard Deviation is a statistical measure of volatility. Standard Deviation is typically used as a component of other indicators, rather than as a stand-alone indicator. For example, Bollinger Bands are calculated by adding a security's Standard Deviation to a moving average.
        /// </summary>
        /// <returns></returns>
        public StdDev StdDev(Data.IDataSeries input, int period)
        {
            if (cacheStdDev != null)
                for (int idx = 0; idx < cacheStdDev.Length; idx++)
                    if (cacheStdDev[idx].Period == period && cacheStdDev[idx].EqualsInput(input))
                        return cacheStdDev[idx];

            lock (checkStdDev)
            {
                checkStdDev.Period = period;
                period = checkStdDev.Period;

                if (cacheStdDev != null)
                    for (int idx = 0; idx < cacheStdDev.Length; idx++)
                        if (cacheStdDev[idx].Period == period && cacheStdDev[idx].EqualsInput(input))
                            return cacheStdDev[idx];

                StdDev indicator = new StdDev();
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

                StdDev[] tmp = new StdDev[cacheStdDev == null ? 1 : cacheStdDev.Length + 1];
                if (cacheStdDev != null)
                    cacheStdDev.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheStdDev = tmp;
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
        /// Standard Deviation is a statistical measure of volatility. Standard Deviation is typically used as a component of other indicators, rather than as a stand-alone indicator. For example, Bollinger Bands are calculated by adding a security's Standard Deviation to a moving average.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.StdDev StdDev(int period)
        {
            return _indicator.StdDev(Input, period);
        }

        /// <summary>
        /// Standard Deviation is a statistical measure of volatility. Standard Deviation is typically used as a component of other indicators, rather than as a stand-alone indicator. For example, Bollinger Bands are calculated by adding a security's Standard Deviation to a moving average.
        /// </summary>
        /// <returns></returns>
        public Indicator.StdDev StdDev(Data.IDataSeries input, int period)
        {
            return _indicator.StdDev(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Standard Deviation is a statistical measure of volatility. Standard Deviation is typically used as a component of other indicators, rather than as a stand-alone indicator. For example, Bollinger Bands are calculated by adding a security's Standard Deviation to a moving average.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.StdDev StdDev(int period)
        {
            return _indicator.StdDev(Input, period);
        }

        /// <summary>
        /// Standard Deviation is a statistical measure of volatility. Standard Deviation is typically used as a component of other indicators, rather than as a stand-alone indicator. For example, Bollinger Bands are calculated by adding a security's Standard Deviation to a moving average.
        /// </summary>
        /// <returns></returns>
        public Indicator.StdDev StdDev(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.StdDev(input, period);
        }
    }
}
#endregion
