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
	/// The WMA (Weighted Moving Average) is a Moving Average indicator that shows the average value of a security's price over a period of time with special emphasis on the more recent portions of the time period under analysis as opposed to the earlier.
	/// </summary>
	[Description("The WMA (Weighted Moving Average) is a Moving Average indicator that shows the average value of a security's price over a period of time with special emphasis on the more recent portions of the time period under analysis as opposed to the earlier.")]
	public class WMA : NinjaTrader.Indicator.Indicator
	{
		#region Variables
		private int		period	= 14;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Orange, Name));

			Overlay				= true;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
				Value.Set(Input[0]);
			else
			{
				int		back	= Math.Min(Period - 1, CurrentBar);
				double	val		= 0;
				int		weight	= 0;
				for (int idx = back; idx >=0; idx--)
				{
					val		+= (idx + 1) * Input[back - idx];
					weight	+= (idx + 1);
				}
				Value.Set(val / weight);
			}
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
        private WMA[] cacheWMA = null;

        private static WMA checkWMA = new WMA();

        /// <summary>
        /// The WMA (Weighted Moving Average) is a Moving Average indicator that shows the average value of a security's price over a period of time with special emphasis on the more recent portions of the time period under analysis as opposed to the earlier.
        /// </summary>
        /// <returns></returns>
        public WMA WMA(int period)
        {
            return WMA(Input, period);
        }

        /// <summary>
        /// The WMA (Weighted Moving Average) is a Moving Average indicator that shows the average value of a security's price over a period of time with special emphasis on the more recent portions of the time period under analysis as opposed to the earlier.
        /// </summary>
        /// <returns></returns>
        public WMA WMA(Data.IDataSeries input, int period)
        {
            if (cacheWMA != null)
                for (int idx = 0; idx < cacheWMA.Length; idx++)
                    if (cacheWMA[idx].Period == period && cacheWMA[idx].EqualsInput(input))
                        return cacheWMA[idx];

            lock (checkWMA)
            {
                checkWMA.Period = period;
                period = checkWMA.Period;

                if (cacheWMA != null)
                    for (int idx = 0; idx < cacheWMA.Length; idx++)
                        if (cacheWMA[idx].Period == period && cacheWMA[idx].EqualsInput(input))
                            return cacheWMA[idx];

                WMA indicator = new WMA();
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

                WMA[] tmp = new WMA[cacheWMA == null ? 1 : cacheWMA.Length + 1];
                if (cacheWMA != null)
                    cacheWMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheWMA = tmp;
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
        /// The WMA (Weighted Moving Average) is a Moving Average indicator that shows the average value of a security's price over a period of time with special emphasis on the more recent portions of the time period under analysis as opposed to the earlier.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.WMA WMA(int period)
        {
            return _indicator.WMA(Input, period);
        }

        /// <summary>
        /// The WMA (Weighted Moving Average) is a Moving Average indicator that shows the average value of a security's price over a period of time with special emphasis on the more recent portions of the time period under analysis as opposed to the earlier.
        /// </summary>
        /// <returns></returns>
        public Indicator.WMA WMA(Data.IDataSeries input, int period)
        {
            return _indicator.WMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The WMA (Weighted Moving Average) is a Moving Average indicator that shows the average value of a security's price over a period of time with special emphasis on the more recent portions of the time period under analysis as opposed to the earlier.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.WMA WMA(int period)
        {
            return _indicator.WMA(Input, period);
        }

        /// <summary>
        /// The WMA (Weighted Moving Average) is a Moving Average indicator that shows the average value of a security's price over a period of time with special emphasis on the more recent portions of the time period under analysis as opposed to the earlier.
        /// </summary>
        /// <returns></returns>
        public Indicator.WMA WMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.WMA(input, period);
        }
    }
}
#endregion
