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
	/// The Average True Range (ATR) is a measure of volatility. It was introduced by Welles Wilder in his book 'New Concepts in Technical Trading Systems' and has since been used as a component of many indicators and trading systems.
	/// </summary>
	[Description("The Average True Range (ATR) is a measure of volatility. It was introduced by Welles Wilder in his book 'New Concepts in Technical Trading Systems' and has since been used as a component of many indicators and trading systems.")]
	public class ATR : Indicator
	{
		#region Variables
		private int			period		= 14;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Green, "ATR"));
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
				Value.Set(High[0] - Low[0]);
			else
			{
				double trueRange = High[0] - Low[0];
				trueRange = Math.Max(Math.Abs(Low[0] - Close[1]), Math.Max(trueRange, Math.Abs(High[0] - Close[1])));
				Value.Set(((Math.Min(CurrentBar + 1, Period) - 1 ) * Value[1] + trueRange) / Math.Min(CurrentBar + 1, Period));
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
        private ATR[] cacheATR = null;

        private static ATR checkATR = new ATR();

        /// <summary>
        /// The Average True Range (ATR) is a measure of volatility. It was introduced by Welles Wilder in his book 'New Concepts in Technical Trading Systems' and has since been used as a component of many indicators and trading systems.
        /// </summary>
        /// <returns></returns>
        public ATR ATR(int period)
        {
            return ATR(Input, period);
        }

        /// <summary>
        /// The Average True Range (ATR) is a measure of volatility. It was introduced by Welles Wilder in his book 'New Concepts in Technical Trading Systems' and has since been used as a component of many indicators and trading systems.
        /// </summary>
        /// <returns></returns>
        public ATR ATR(Data.IDataSeries input, int period)
        {
            if (cacheATR != null)
                for (int idx = 0; idx < cacheATR.Length; idx++)
                    if (cacheATR[idx].Period == period && cacheATR[idx].EqualsInput(input))
                        return cacheATR[idx];

            lock (checkATR)
            {
                checkATR.Period = period;
                period = checkATR.Period;

                if (cacheATR != null)
                    for (int idx = 0; idx < cacheATR.Length; idx++)
                        if (cacheATR[idx].Period == period && cacheATR[idx].EqualsInput(input))
                            return cacheATR[idx];

                ATR indicator = new ATR();
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

                ATR[] tmp = new ATR[cacheATR == null ? 1 : cacheATR.Length + 1];
                if (cacheATR != null)
                    cacheATR.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheATR = tmp;
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
        /// The Average True Range (ATR) is a measure of volatility. It was introduced by Welles Wilder in his book 'New Concepts in Technical Trading Systems' and has since been used as a component of many indicators and trading systems.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ATR ATR(int period)
        {
            return _indicator.ATR(Input, period);
        }

        /// <summary>
        /// The Average True Range (ATR) is a measure of volatility. It was introduced by Welles Wilder in his book 'New Concepts in Technical Trading Systems' and has since been used as a component of many indicators and trading systems.
        /// </summary>
        /// <returns></returns>
        public Indicator.ATR ATR(Data.IDataSeries input, int period)
        {
            return _indicator.ATR(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Average True Range (ATR) is a measure of volatility. It was introduced by Welles Wilder in his book 'New Concepts in Technical Trading Systems' and has since been used as a component of many indicators and trading systems.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ATR ATR(int period)
        {
            return _indicator.ATR(Input, period);
        }

        /// <summary>
        /// The Average True Range (ATR) is a measure of volatility. It was introduced by Welles Wilder in his book 'New Concepts in Technical Trading Systems' and has since been used as a component of many indicators and trading systems.
        /// </summary>
        /// <returns></returns>
        public Indicator.ATR ATR(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ATR(input, period);
        }
    }
}
#endregion
