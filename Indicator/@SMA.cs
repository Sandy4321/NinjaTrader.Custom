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
	/// The SMA (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
	/// </summary>
	[Description("The SMA (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.")]
	public class SMA : Indicator
	{
		#region Variables
		private int		period	= 14;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Orange, "SMA"));

			Overlay = true;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick).
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
				Value.Set(Input[0]);
			else
			{
				double last = Value[1] * Math.Min(CurrentBar, Period);

				if (CurrentBar >= Period)
					Value.Set((last + Input[0] - Input[Period]) / Math.Min(CurrentBar, Period));
				else
					Value.Set((last + Input[0]) / (Math.Min(CurrentBar, Period) + 1));
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
        private SMA[] cacheSMA = null;

        private static SMA checkSMA = new SMA();

        /// <summary>
        /// The SMA (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public SMA SMA(int period)
        {
            return SMA(Input, period);
        }

        /// <summary>
        /// The SMA (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public SMA SMA(Data.IDataSeries input, int period)
        {
            if (cacheSMA != null)
                for (int idx = 0; idx < cacheSMA.Length; idx++)
                    if (cacheSMA[idx].Period == period && cacheSMA[idx].EqualsInput(input))
                        return cacheSMA[idx];

            lock (checkSMA)
            {
                checkSMA.Period = period;
                period = checkSMA.Period;

                if (cacheSMA != null)
                    for (int idx = 0; idx < cacheSMA.Length; idx++)
                        if (cacheSMA[idx].Period == period && cacheSMA[idx].EqualsInput(input))
                            return cacheSMA[idx];

                SMA indicator = new SMA();
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

                SMA[] tmp = new SMA[cacheSMA == null ? 1 : cacheSMA.Length + 1];
                if (cacheSMA != null)
                    cacheSMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSMA = tmp;
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
        /// The SMA (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SMA SMA(int period)
        {
            return _indicator.SMA(Input, period);
        }

        /// <summary>
        /// The SMA (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public Indicator.SMA SMA(Data.IDataSeries input, int period)
        {
            return _indicator.SMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The SMA (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SMA SMA(int period)
        {
            return _indicator.SMA(Input, period);
        }

        /// <summary>
        /// The SMA (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public Indicator.SMA SMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SMA(input, period);
        }
    }
}
#endregion
