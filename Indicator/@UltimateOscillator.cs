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
	/// The Ultimate Oscillator is the weighted sum of three oscillators of different time periods. The typical time periods are 7, 14 and 28. The values of the Ultimate Oscillator range from zero to 100. Values over 70 indicate overbought conditions, and values under 30 indicate oversold conditions. Also look for agreement/divergence with the price to confirm a trend or signal the end of a trend.
	/// </summary>
	[Description("The Ultimate Oscillator is the weighted sum of three oscillators of different time periods. The typical time periods are 7, 14 and 28. The values of the Ultimate Oscillator range from zero to 100. Values over 70 indicate overbought conditions, and values under 30 indicate oversold conditions. Also look for agreement/divergence with the price to confirm a trend or signal the end of a trend.")]
	public class UltimateOscillator : Indicator
	{
		#region Variables
		private int					fast			= 7;
		private int					intermediate	= 14;
		private int					slow			= 28;

		private DataSeries		buyingPressure;
		private DataSeries		trueRange;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Blue, "UltimateOsc"));

			Add(new Line(Color.LightGray, 30, "OverSold"));
			Add(new Line(Color.LightGray, 50, "Neutral"));
			Add(new Line(Color.LightGray, 70, "OverBought"));

			buyingPressure	= new DataSeries(this);
			trueRange		= new DataSeries(this);
		}

		/// <summary>
		/// Calculates the indicator value(s) at the current index.
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				Value.Set(0);
			}
			else
			{
				double trueLow = Math.Min(Low[0], Close[1]);
				buyingPressure.Set(Close[0] - trueLow);
				trueRange.Set(Math.Max(Math.Max(High[0] - Low[0], High[0] - Close[1]), Close[1] - Low[0]));

				double bpSum1 = SUM(buyingPressure, fast)[0];
				double bpSum2 = SUM(buyingPressure, intermediate)[0];
				double bpSum3 = SUM(buyingPressure, slow)[0];

				double trSum1 = SUM(trueRange, fast)[0];
				double trSum2 = SUM(trueRange, intermediate)[0];
				double trSum3 = SUM(trueRange, slow)[0];

				// Use previous value if we get into trouble
				if (trSum1 == 0 || trSum2 == 0 || trSum3 == 0) 
				{
					Value.Set(Value[1]);
					return;
				}

				double factor1 = (double)slow / (double)fast;
				double factor2 = (double)slow / (double)intermediate;

				Value.Set((((bpSum1 / trSum1) * factor1 + 
							(bpSum2 / trSum2) * factor2 + 
							(bpSum3 / trSum3)) / 
							(factor1 + factor2 + 1)) * 100);
			}
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculation.")]
		[GridCategory("Parameters")]
		public int Fast
		{
			get { return fast; }
			set { fast = Math.Max(1, value); }
		}

		[Description("Numbers of bars used for calculation.")]
		[GridCategory("Parameters")]
		public int Intermediate
		{
			get { return intermediate; }
			set { intermediate = Math.Max(1, value); }
		}

		[Description("Numbers of bars used for calculation.")]
		[GridCategory("Parameters")]
		public int Slow
		{
			get { return slow; }
			set { slow = Math.Max(1, value); }
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
        private UltimateOscillator[] cacheUltimateOscillator = null;

        private static UltimateOscillator checkUltimateOscillator = new UltimateOscillator();

        /// <summary>
        /// The Ultimate Oscillator is the weighted sum of three oscillators of different time periods. The typical time periods are 7, 14 and 28. The values of the Ultimate Oscillator range from zero to 100. Values over 70 indicate overbought conditions, and values under 30 indicate oversold conditions. Also look for agreement/divergence with the price to confirm a trend or signal the end of a trend.
        /// </summary>
        /// <returns></returns>
        public UltimateOscillator UltimateOscillator(int fast, int intermediate, int slow)
        {
            return UltimateOscillator(Input, fast, intermediate, slow);
        }

        /// <summary>
        /// The Ultimate Oscillator is the weighted sum of three oscillators of different time periods. The typical time periods are 7, 14 and 28. The values of the Ultimate Oscillator range from zero to 100. Values over 70 indicate overbought conditions, and values under 30 indicate oversold conditions. Also look for agreement/divergence with the price to confirm a trend or signal the end of a trend.
        /// </summary>
        /// <returns></returns>
        public UltimateOscillator UltimateOscillator(Data.IDataSeries input, int fast, int intermediate, int slow)
        {
            if (cacheUltimateOscillator != null)
                for (int idx = 0; idx < cacheUltimateOscillator.Length; idx++)
                    if (cacheUltimateOscillator[idx].Fast == fast && cacheUltimateOscillator[idx].Intermediate == intermediate && cacheUltimateOscillator[idx].Slow == slow && cacheUltimateOscillator[idx].EqualsInput(input))
                        return cacheUltimateOscillator[idx];

            lock (checkUltimateOscillator)
            {
                checkUltimateOscillator.Fast = fast;
                fast = checkUltimateOscillator.Fast;
                checkUltimateOscillator.Intermediate = intermediate;
                intermediate = checkUltimateOscillator.Intermediate;
                checkUltimateOscillator.Slow = slow;
                slow = checkUltimateOscillator.Slow;

                if (cacheUltimateOscillator != null)
                    for (int idx = 0; idx < cacheUltimateOscillator.Length; idx++)
                        if (cacheUltimateOscillator[idx].Fast == fast && cacheUltimateOscillator[idx].Intermediate == intermediate && cacheUltimateOscillator[idx].Slow == slow && cacheUltimateOscillator[idx].EqualsInput(input))
                            return cacheUltimateOscillator[idx];

                UltimateOscillator indicator = new UltimateOscillator();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Fast = fast;
                indicator.Intermediate = intermediate;
                indicator.Slow = slow;
                Indicators.Add(indicator);
                indicator.SetUp();

                UltimateOscillator[] tmp = new UltimateOscillator[cacheUltimateOscillator == null ? 1 : cacheUltimateOscillator.Length + 1];
                if (cacheUltimateOscillator != null)
                    cacheUltimateOscillator.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheUltimateOscillator = tmp;
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
        /// The Ultimate Oscillator is the weighted sum of three oscillators of different time periods. The typical time periods are 7, 14 and 28. The values of the Ultimate Oscillator range from zero to 100. Values over 70 indicate overbought conditions, and values under 30 indicate oversold conditions. Also look for agreement/divergence with the price to confirm a trend or signal the end of a trend.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.UltimateOscillator UltimateOscillator(int fast, int intermediate, int slow)
        {
            return _indicator.UltimateOscillator(Input, fast, intermediate, slow);
        }

        /// <summary>
        /// The Ultimate Oscillator is the weighted sum of three oscillators of different time periods. The typical time periods are 7, 14 and 28. The values of the Ultimate Oscillator range from zero to 100. Values over 70 indicate overbought conditions, and values under 30 indicate oversold conditions. Also look for agreement/divergence with the price to confirm a trend or signal the end of a trend.
        /// </summary>
        /// <returns></returns>
        public Indicator.UltimateOscillator UltimateOscillator(Data.IDataSeries input, int fast, int intermediate, int slow)
        {
            return _indicator.UltimateOscillator(input, fast, intermediate, slow);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Ultimate Oscillator is the weighted sum of three oscillators of different time periods. The typical time periods are 7, 14 and 28. The values of the Ultimate Oscillator range from zero to 100. Values over 70 indicate overbought conditions, and values under 30 indicate oversold conditions. Also look for agreement/divergence with the price to confirm a trend or signal the end of a trend.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.UltimateOscillator UltimateOscillator(int fast, int intermediate, int slow)
        {
            return _indicator.UltimateOscillator(Input, fast, intermediate, slow);
        }

        /// <summary>
        /// The Ultimate Oscillator is the weighted sum of three oscillators of different time periods. The typical time periods are 7, 14 and 28. The values of the Ultimate Oscillator range from zero to 100. Values over 70 indicate overbought conditions, and values under 30 indicate oversold conditions. Also look for agreement/divergence with the price to confirm a trend or signal the end of a trend.
        /// </summary>
        /// <returns></returns>
        public Indicator.UltimateOscillator UltimateOscillator(Data.IDataSeries input, int fast, int intermediate, int slow)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.UltimateOscillator(input, fast, intermediate, slow);
        }
    }
}
#endregion
