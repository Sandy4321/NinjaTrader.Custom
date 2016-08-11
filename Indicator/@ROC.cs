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
	/// The ROC (Rate-of-Change) indicator displays the percent change between the current price and the price x-time periods ago.
	/// </summary>
	[Description("The ROC (Rate-of-Change) indicator displays the percent change between the current price and the price x-time periods ago.")]
	public class ROC : Indicator
	{
		#region Variables
		private int					period	= 14;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Line(Color.DarkGray, 0, "Zero line"));
			Add(new Plot(Color.Blue, "ROC"));
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
            int barsAgo = Math.Min(CurrentBar, Period);
			Value.Set(((Input[0] - Input[barsAgo]) / Input[barsAgo]) * 100);
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
        private ROC[] cacheROC = null;

        private static ROC checkROC = new ROC();

        /// <summary>
        /// The ROC (Rate-of-Change) indicator displays the difference between the current price and the price x-time periods ago.
        /// </summary>
        /// <returns></returns>
        public ROC ROC(int period)
        {
            return ROC(Input, period);
        }

        /// <summary>
        /// The ROC (Rate-of-Change) indicator displays the difference between the current price and the price x-time periods ago.
        /// </summary>
        /// <returns></returns>
        public ROC ROC(Data.IDataSeries input, int period)
        {
            if (cacheROC != null)
                for (int idx = 0; idx < cacheROC.Length; idx++)
                    if (cacheROC[idx].Period == period && cacheROC[idx].EqualsInput(input))
                        return cacheROC[idx];

            lock (checkROC)
            {
                checkROC.Period = period;
                period = checkROC.Period;

                if (cacheROC != null)
                    for (int idx = 0; idx < cacheROC.Length; idx++)
                        if (cacheROC[idx].Period == period && cacheROC[idx].EqualsInput(input))
                            return cacheROC[idx];

                ROC indicator = new ROC();
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

                ROC[] tmp = new ROC[cacheROC == null ? 1 : cacheROC.Length + 1];
                if (cacheROC != null)
                    cacheROC.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheROC = tmp;
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
        /// The ROC (Rate-of-Change) indicator displays the difference between the current price and the price x-time periods ago.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ROC ROC(int period)
        {
            return _indicator.ROC(Input, period);
        }

        /// <summary>
        /// The ROC (Rate-of-Change) indicator displays the difference between the current price and the price x-time periods ago.
        /// </summary>
        /// <returns></returns>
        public Indicator.ROC ROC(Data.IDataSeries input, int period)
        {
            return _indicator.ROC(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The ROC (Rate-of-Change) indicator displays the difference between the current price and the price x-time periods ago.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ROC ROC(int period)
        {
            return _indicator.ROC(Input, period);
        }

        /// <summary>
        /// The ROC (Rate-of-Change) indicator displays the difference between the current price and the price x-time periods ago.
        /// </summary>
        /// <returns></returns>
        public Indicator.ROC ROC(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ROC(input, period);
        }
    }
}
#endregion
