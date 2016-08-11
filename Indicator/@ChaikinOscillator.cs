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
	/// Chaikin Oscillator.
	/// </summary>
	[Description("Chaikin Oscillator.")]
	public class ChaikinOscillator : Indicator
	{
		#region Variables
		private DataSeries			cummulative;
		private int						fast	= 3;
		private DataSeries			moneyFlow;
		private int						slow	= 10;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Orange, Name));

			cummulative		= new DataSeries(this);
			moneyFlow		= new DataSeries(this);
		}

		/// <summary>
		/// Calculates the indicator value(s) at the current index.
		/// </summary>
		protected override void OnBarUpdate()
		{
			moneyFlow.Set(Volume[0] * ((Close[0]  - Low[0]) - (High[0] - Close[0])) / ((High[0] - Low[0]) == 0 ? 1 : (High[0] - Low[0])));
			cummulative.Set(moneyFlow[0] + (CurrentBar == 0 ? 0 : cummulative[1]));
			Value.Set(EMA(cummulative, Fast)[0] - EMA(cummulative, Slow)[0]);
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Number of bars for fast SMA")]
		[GridCategory("Parameters")]
		public int Fast
		{
			get { return fast; }
			set { fast = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Number of bars for slow SMA")]
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
        private ChaikinOscillator[] cacheChaikinOscillator = null;

        private static ChaikinOscillator checkChaikinOscillator = new ChaikinOscillator();

        /// <summary>
        /// Chaikin Oscillator.
        /// </summary>
        /// <returns></returns>
        public ChaikinOscillator ChaikinOscillator(int fast, int slow)
        {
            return ChaikinOscillator(Input, fast, slow);
        }

        /// <summary>
        /// Chaikin Oscillator.
        /// </summary>
        /// <returns></returns>
        public ChaikinOscillator ChaikinOscillator(Data.IDataSeries input, int fast, int slow)
        {
            if (cacheChaikinOscillator != null)
                for (int idx = 0; idx < cacheChaikinOscillator.Length; idx++)
                    if (cacheChaikinOscillator[idx].Fast == fast && cacheChaikinOscillator[idx].Slow == slow && cacheChaikinOscillator[idx].EqualsInput(input))
                        return cacheChaikinOscillator[idx];

            lock (checkChaikinOscillator)
            {
                checkChaikinOscillator.Fast = fast;
                fast = checkChaikinOscillator.Fast;
                checkChaikinOscillator.Slow = slow;
                slow = checkChaikinOscillator.Slow;

                if (cacheChaikinOscillator != null)
                    for (int idx = 0; idx < cacheChaikinOscillator.Length; idx++)
                        if (cacheChaikinOscillator[idx].Fast == fast && cacheChaikinOscillator[idx].Slow == slow && cacheChaikinOscillator[idx].EqualsInput(input))
                            return cacheChaikinOscillator[idx];

                ChaikinOscillator indicator = new ChaikinOscillator();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Fast = fast;
                indicator.Slow = slow;
                Indicators.Add(indicator);
                indicator.SetUp();

                ChaikinOscillator[] tmp = new ChaikinOscillator[cacheChaikinOscillator == null ? 1 : cacheChaikinOscillator.Length + 1];
                if (cacheChaikinOscillator != null)
                    cacheChaikinOscillator.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheChaikinOscillator = tmp;
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
        /// Chaikin Oscillator.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ChaikinOscillator ChaikinOscillator(int fast, int slow)
        {
            return _indicator.ChaikinOscillator(Input, fast, slow);
        }

        /// <summary>
        /// Chaikin Oscillator.
        /// </summary>
        /// <returns></returns>
        public Indicator.ChaikinOscillator ChaikinOscillator(Data.IDataSeries input, int fast, int slow)
        {
            return _indicator.ChaikinOscillator(input, fast, slow);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Chaikin Oscillator.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ChaikinOscillator ChaikinOscillator(int fast, int slow)
        {
            return _indicator.ChaikinOscillator(Input, fast, slow);
        }

        /// <summary>
        /// Chaikin Oscillator.
        /// </summary>
        /// <returns></returns>
        public Indicator.ChaikinOscillator ChaikinOscillator(Data.IDataSeries input, int fast, int slow)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ChaikinOscillator(input, fast, slow);
        }
    }
}
#endregion
