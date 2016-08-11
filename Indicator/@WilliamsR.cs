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
	/// The Williams %R is a momentum indicator that is designed to identify overbought and oversold areas in a nontrending market.
	/// </summary>
	[Description("The Williams %R is a momentum indicator that is designed to identify overbought and oversold areas in a nontrending market.")]
	public class WilliamsR : Indicator
	{
		#region Variables
		private int		period	= 14;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Line(Color.DarkGray, -25, "Upper"));
			Add(new Line(Color.DarkGray, -75, "Lower"));
			Add(new Plot(Color.Orange, "Williams %R"));
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			Value.Set(-100 * (MAX(High, Period)[0] - Close[0]) / (MAX(High, Period)[0] - MIN(Low, Period)[0] == 0 ? 1 : MAX(High, Period)[0] - MIN(Low, Period)[0]));
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
        private WilliamsR[] cacheWilliamsR = null;

        private static WilliamsR checkWilliamsR = new WilliamsR();

        /// <summary>
        /// The Williams %R is a momentum indicator that is designed to identify overbought and oversold areas in a nontrending market.
        /// </summary>
        /// <returns></returns>
        public WilliamsR WilliamsR(int period)
        {
            return WilliamsR(Input, period);
        }

        /// <summary>
        /// The Williams %R is a momentum indicator that is designed to identify overbought and oversold areas in a nontrending market.
        /// </summary>
        /// <returns></returns>
        public WilliamsR WilliamsR(Data.IDataSeries input, int period)
        {
            if (cacheWilliamsR != null)
                for (int idx = 0; idx < cacheWilliamsR.Length; idx++)
                    if (cacheWilliamsR[idx].Period == period && cacheWilliamsR[idx].EqualsInput(input))
                        return cacheWilliamsR[idx];

            lock (checkWilliamsR)
            {
                checkWilliamsR.Period = period;
                period = checkWilliamsR.Period;

                if (cacheWilliamsR != null)
                    for (int idx = 0; idx < cacheWilliamsR.Length; idx++)
                        if (cacheWilliamsR[idx].Period == period && cacheWilliamsR[idx].EqualsInput(input))
                            return cacheWilliamsR[idx];

                WilliamsR indicator = new WilliamsR();
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

                WilliamsR[] tmp = new WilliamsR[cacheWilliamsR == null ? 1 : cacheWilliamsR.Length + 1];
                if (cacheWilliamsR != null)
                    cacheWilliamsR.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheWilliamsR = tmp;
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
        /// The Williams %R is a momentum indicator that is designed to identify overbought and oversold areas in a nontrending market.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.WilliamsR WilliamsR(int period)
        {
            return _indicator.WilliamsR(Input, period);
        }

        /// <summary>
        /// The Williams %R is a momentum indicator that is designed to identify overbought and oversold areas in a nontrending market.
        /// </summary>
        /// <returns></returns>
        public Indicator.WilliamsR WilliamsR(Data.IDataSeries input, int period)
        {
            return _indicator.WilliamsR(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Williams %R is a momentum indicator that is designed to identify overbought and oversold areas in a nontrending market.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.WilliamsR WilliamsR(int period)
        {
            return _indicator.WilliamsR(Input, period);
        }

        /// <summary>
        /// The Williams %R is a momentum indicator that is designed to identify overbought and oversold areas in a nontrending market.
        /// </summary>
        /// <returns></returns>
        public Indicator.WilliamsR WilliamsR(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.WilliamsR(input, period);
        }
    }
}
#endregion
