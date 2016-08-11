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
	/// The Price Oscillator indicator shows the variation among two moving averages for the price of a security.
	/// </summary>
	[Description("The Price Oscillator indicator shows the variation among two moving averages for the price of a security.")]
	public class PriceOscillator : Indicator
	{
		#region Variables
		private int					fast	= 12;
		private int					slow	= 26;
		private int					smooth	= 9;
		private	DataSeries			smoothEma;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Line(Color.DarkGray, 0, "Zero line"));
			Add(new Plot(Color.Orange, Name));
			
			smoothEma = new DataSeries(this);
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			smoothEma.Set(EMA(Fast)[0] - EMA(Slow)[0]);
			Value.Set(EMA(smoothEma, Smooth)[0]);
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Number of bars for slow EMA")]
		[GridCategory("Parameters")]
		public int Fast
		{
			get { return fast; }
			set { fast = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Number of bars for slow EMA")]
		[GridCategory("Parameters")]
		public int Slow
		{
			get { return slow; }
			set { slow = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Number of bars for smoothing")]
		[GridCategory("Parameters")]
		public int Smooth
		{
			get { return smooth; }
			set { smooth = Math.Max(1, value); }
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
        private PriceOscillator[] cachePriceOscillator = null;

        private static PriceOscillator checkPriceOscillator = new PriceOscillator();

        /// <summary>
        /// The Price Oscillator indicator shows the variation among two moving averages for the price of a security.
        /// </summary>
        /// <returns></returns>
        public PriceOscillator PriceOscillator(int fast, int slow, int smooth)
        {
            return PriceOscillator(Input, fast, slow, smooth);
        }

        /// <summary>
        /// The Price Oscillator indicator shows the variation among two moving averages for the price of a security.
        /// </summary>
        /// <returns></returns>
        public PriceOscillator PriceOscillator(Data.IDataSeries input, int fast, int slow, int smooth)
        {
            if (cachePriceOscillator != null)
                for (int idx = 0; idx < cachePriceOscillator.Length; idx++)
                    if (cachePriceOscillator[idx].Fast == fast && cachePriceOscillator[idx].Slow == slow && cachePriceOscillator[idx].Smooth == smooth && cachePriceOscillator[idx].EqualsInput(input))
                        return cachePriceOscillator[idx];

            lock (checkPriceOscillator)
            {
                checkPriceOscillator.Fast = fast;
                fast = checkPriceOscillator.Fast;
                checkPriceOscillator.Slow = slow;
                slow = checkPriceOscillator.Slow;
                checkPriceOscillator.Smooth = smooth;
                smooth = checkPriceOscillator.Smooth;

                if (cachePriceOscillator != null)
                    for (int idx = 0; idx < cachePriceOscillator.Length; idx++)
                        if (cachePriceOscillator[idx].Fast == fast && cachePriceOscillator[idx].Slow == slow && cachePriceOscillator[idx].Smooth == smooth && cachePriceOscillator[idx].EqualsInput(input))
                            return cachePriceOscillator[idx];

                PriceOscillator indicator = new PriceOscillator();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Fast = fast;
                indicator.Slow = slow;
                indicator.Smooth = smooth;
                Indicators.Add(indicator);
                indicator.SetUp();

                PriceOscillator[] tmp = new PriceOscillator[cachePriceOscillator == null ? 1 : cachePriceOscillator.Length + 1];
                if (cachePriceOscillator != null)
                    cachePriceOscillator.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachePriceOscillator = tmp;
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
        /// The Price Oscillator indicator shows the variation among two moving averages for the price of a security.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PriceOscillator PriceOscillator(int fast, int slow, int smooth)
        {
            return _indicator.PriceOscillator(Input, fast, slow, smooth);
        }

        /// <summary>
        /// The Price Oscillator indicator shows the variation among two moving averages for the price of a security.
        /// </summary>
        /// <returns></returns>
        public Indicator.PriceOscillator PriceOscillator(Data.IDataSeries input, int fast, int slow, int smooth)
        {
            return _indicator.PriceOscillator(input, fast, slow, smooth);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Price Oscillator indicator shows the variation among two moving averages for the price of a security.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PriceOscillator PriceOscillator(int fast, int slow, int smooth)
        {
            return _indicator.PriceOscillator(Input, fast, slow, smooth);
        }

        /// <summary>
        /// The Price Oscillator indicator shows the variation among two moving averages for the price of a security.
        /// </summary>
        /// <returns></returns>
        public Indicator.PriceOscillator PriceOscillator(Data.IDataSeries input, int fast, int slow, int smooth)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.PriceOscillator(input, fast, slow, smooth);
        }
    }
}
#endregion
