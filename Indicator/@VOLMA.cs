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
	/// The VOLMA (Volume Moving Average) plots an exponential moving average (EMA) of volume.
	/// </summary>
	[Description("The VOLMA (Volume Moving Average) plots an exponential moving average (EMA) of volume.")]
	public class VOLMA : Indicator
	{
		#region Variables
		private int			period		= 14;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.DarkOrange, "VOLMA"));

			Overlay				= false;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			Value.Set(EMA(Volume, Period)[0]);
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
        private VOLMA[] cacheVOLMA = null;

        private static VOLMA checkVOLMA = new VOLMA();

        /// <summary>
        /// The VOLMA (Volume Moving Average) plots an exponential moving average (EMA) of volume.
        /// </summary>
        /// <returns></returns>
        public VOLMA VOLMA(int period)
        {
            return VOLMA(Input, period);
        }

        /// <summary>
        /// The VOLMA (Volume Moving Average) plots an exponential moving average (EMA) of volume.
        /// </summary>
        /// <returns></returns>
        public VOLMA VOLMA(Data.IDataSeries input, int period)
        {
            if (cacheVOLMA != null)
                for (int idx = 0; idx < cacheVOLMA.Length; idx++)
                    if (cacheVOLMA[idx].Period == period && cacheVOLMA[idx].EqualsInput(input))
                        return cacheVOLMA[idx];

            lock (checkVOLMA)
            {
                checkVOLMA.Period = period;
                period = checkVOLMA.Period;

                if (cacheVOLMA != null)
                    for (int idx = 0; idx < cacheVOLMA.Length; idx++)
                        if (cacheVOLMA[idx].Period == period && cacheVOLMA[idx].EqualsInput(input))
                            return cacheVOLMA[idx];

                VOLMA indicator = new VOLMA();
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

                VOLMA[] tmp = new VOLMA[cacheVOLMA == null ? 1 : cacheVOLMA.Length + 1];
                if (cacheVOLMA != null)
                    cacheVOLMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheVOLMA = tmp;
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
        /// The VOLMA (Volume Moving Average) plots an exponential moving average (EMA) of volume.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VOLMA VOLMA(int period)
        {
            return _indicator.VOLMA(Input, period);
        }

        /// <summary>
        /// The VOLMA (Volume Moving Average) plots an exponential moving average (EMA) of volume.
        /// </summary>
        /// <returns></returns>
        public Indicator.VOLMA VOLMA(Data.IDataSeries input, int period)
        {
            return _indicator.VOLMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The VOLMA (Volume Moving Average) plots an exponential moving average (EMA) of volume.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VOLMA VOLMA(int period)
        {
            return _indicator.VOLMA(Input, period);
        }

        /// <summary>
        /// The VOLMA (Volume Moving Average) plots an exponential moving average (EMA) of volume.
        /// </summary>
        /// <returns></returns>
        public Indicator.VOLMA VOLMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.VOLMA(input, period);
        }
    }
}
#endregion
