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
	/// The Hull Moving Average (HMA) employs weighted MA calculations to offer superior smoothing, and much less lag, over traditional SMA indicators.
	/// This indicator is based on the reference article found here:
	/// http://www.justdata.com.au/Journals/AlanHull/hull_ma.htm
	/// </summary>
	[Description("The Hull Moving Average (HMA) employs weighted MA calculations to offer superior smoothing, and much less lag, over traditional SMA indicators.")]
	public class HMA : Indicator
	{
		#region Variables
		private int				period		= 21;
		private DataSeries diffSeries;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Orange,  "HMA"));

			Overlay				= true;	// Plots the indicator on top of price
			diffSeries			= new DataSeries(this);
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			double value1 = 2 * WMA(Inputs[0], (int)(Period / 2))[0];
			double value2 = WMA(Inputs[0], Period)[0];
			diffSeries.Set(value1 - value2);

			Value.Set(WMA(diffSeries, (int) Math.Sqrt(Period))[0]);
		}

		#region Properties
		/// <summary>
		/// Period
		/// </summary>
		[Description("Number of bars used for calculation")]
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
        private HMA[] cacheHMA = null;

        private static HMA checkHMA = new HMA();

        /// <summary>
        /// The Hull Moving Average (HMA) employs weighted MA calculations to offer superior smoothing, and much less lag, over traditional SMA indicators.
        /// </summary>
        /// <returns></returns>
        public HMA HMA(int period)
        {
            return HMA(Input, period);
        }

        /// <summary>
        /// The Hull Moving Average (HMA) employs weighted MA calculations to offer superior smoothing, and much less lag, over traditional SMA indicators.
        /// </summary>
        /// <returns></returns>
        public HMA HMA(Data.IDataSeries input, int period)
        {
            if (cacheHMA != null)
                for (int idx = 0; idx < cacheHMA.Length; idx++)
                    if (cacheHMA[idx].Period == period && cacheHMA[idx].EqualsInput(input))
                        return cacheHMA[idx];

            lock (checkHMA)
            {
                checkHMA.Period = period;
                period = checkHMA.Period;

                if (cacheHMA != null)
                    for (int idx = 0; idx < cacheHMA.Length; idx++)
                        if (cacheHMA[idx].Period == period && cacheHMA[idx].EqualsInput(input))
                            return cacheHMA[idx];

                HMA indicator = new HMA();
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

                HMA[] tmp = new HMA[cacheHMA == null ? 1 : cacheHMA.Length + 1];
                if (cacheHMA != null)
                    cacheHMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheHMA = tmp;
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
        /// The Hull Moving Average (HMA) employs weighted MA calculations to offer superior smoothing, and much less lag, over traditional SMA indicators.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.HMA HMA(int period)
        {
            return _indicator.HMA(Input, period);
        }

        /// <summary>
        /// The Hull Moving Average (HMA) employs weighted MA calculations to offer superior smoothing, and much less lag, over traditional SMA indicators.
        /// </summary>
        /// <returns></returns>
        public Indicator.HMA HMA(Data.IDataSeries input, int period)
        {
            return _indicator.HMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Hull Moving Average (HMA) employs weighted MA calculations to offer superior smoothing, and much less lag, over traditional SMA indicators.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.HMA HMA(int period)
        {
            return _indicator.HMA(Input, period);
        }

        /// <summary>
        /// The Hull Moving Average (HMA) employs weighted MA calculations to offer superior smoothing, and much less lag, over traditional SMA indicators.
        /// </summary>
        /// <returns></returns>
        public Indicator.HMA HMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.HMA(input, period);
        }
    }
}
#endregion
