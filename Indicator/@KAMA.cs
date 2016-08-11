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
	/// Kaufman's Adaptive Moving Average. Developed by Perry Kaufman, this indicator is an EMA using an Efficiency Ratio to modify the smoothing constant, which ranges from a minimum of Fast Length to a maximum of Slow Length. Since this moving average is adaptive it tends to follow prices more closely than other MA's.
	/// </summary>
	[Description("Developed by Perry Kaufman, this indicator is an EMA using an Efficiency Ratio to modify the smoothing constant, which ranges from a minimum of Fast Length to a maximum of Slow Length. Since this moving average is adaptive it tends to follow prices more closely than other MA's.")]
	public class KAMA : Indicator
	{
		#region Variables
		private int				period	= 10;
		private int				fast	= 2;
		private int				slow	= 30;

		DataSeries			diffSeries;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before
		/// any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Blue, "KAMA"));

			diffSeries			= new DataSeries(this);

			Overlay				= true;
		}
		
		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar > 0) 
			{
				diffSeries.Set(Math.Abs(Input[0] - Input[1]));
			}
			
			if (CurrentBar < Period) 
			{
				Value.Set(Input[0]);
				return;
			}

			double fastCF = 2.0 / (double)(fast + 1);
			double slowCF = 2.0 / (double)(slow + 1);

			double signal = Math.Abs(Input[0] - Input[Period]);
			double noise  = SUM(diffSeries, Period)[0];
	
			// Prevent div by zero
			if (noise == 0) 
			{
				Value.Set(Value[1]);
				return;
			}

			double smooth = Math.Pow((signal / noise) * (fastCF - slowCF) + slowCF, 2);

			Value.Set(Value[1] + smooth * (Input[0] - Value[1]));
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Fast Length.")]
		[GridCategory("Parameters")]
		public int Fast
		{
			get { return fast; }
			set { fast = Math.Min(125, Math.Max(1, value)); }
		}

		/// <summary>
		/// </summary>
		[Description("Number of bars used for calculations.")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(5, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Slow Length.")]
		[GridCategory("Parameters")]
		public int Slow
		{
			get { return slow; }
			set { slow = Math.Min(125, Math.Max(1, value)); }
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
        private KAMA[] cacheKAMA = null;

        private static KAMA checkKAMA = new KAMA();

        /// <summary>
        /// Developed by Perry Kaufman, this indicator is an EMA using an Efficiency Ratio to modify the smoothing constant, which ranges from a minimum of Fast Length to a maximum of Slow Length. Since this moving average is adaptive it tends to follow prices more closely than other MA's.
        /// </summary>
        /// <returns></returns>
        public KAMA KAMA(int fast, int period, int slow)
        {
            return KAMA(Input, fast, period, slow);
        }

        /// <summary>
        /// Developed by Perry Kaufman, this indicator is an EMA using an Efficiency Ratio to modify the smoothing constant, which ranges from a minimum of Fast Length to a maximum of Slow Length. Since this moving average is adaptive it tends to follow prices more closely than other MA's.
        /// </summary>
        /// <returns></returns>
        public KAMA KAMA(Data.IDataSeries input, int fast, int period, int slow)
        {
            if (cacheKAMA != null)
                for (int idx = 0; idx < cacheKAMA.Length; idx++)
                    if (cacheKAMA[idx].Fast == fast && cacheKAMA[idx].Period == period && cacheKAMA[idx].Slow == slow && cacheKAMA[idx].EqualsInput(input))
                        return cacheKAMA[idx];

            lock (checkKAMA)
            {
                checkKAMA.Fast = fast;
                fast = checkKAMA.Fast;
                checkKAMA.Period = period;
                period = checkKAMA.Period;
                checkKAMA.Slow = slow;
                slow = checkKAMA.Slow;

                if (cacheKAMA != null)
                    for (int idx = 0; idx < cacheKAMA.Length; idx++)
                        if (cacheKAMA[idx].Fast == fast && cacheKAMA[idx].Period == period && cacheKAMA[idx].Slow == slow && cacheKAMA[idx].EqualsInput(input))
                            return cacheKAMA[idx];

                KAMA indicator = new KAMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Fast = fast;
                indicator.Period = period;
                indicator.Slow = slow;
                Indicators.Add(indicator);
                indicator.SetUp();

                KAMA[] tmp = new KAMA[cacheKAMA == null ? 1 : cacheKAMA.Length + 1];
                if (cacheKAMA != null)
                    cacheKAMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheKAMA = tmp;
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
        /// Developed by Perry Kaufman, this indicator is an EMA using an Efficiency Ratio to modify the smoothing constant, which ranges from a minimum of Fast Length to a maximum of Slow Length. Since this moving average is adaptive it tends to follow prices more closely than other MA's.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.KAMA KAMA(int fast, int period, int slow)
        {
            return _indicator.KAMA(Input, fast, period, slow);
        }

        /// <summary>
        /// Developed by Perry Kaufman, this indicator is an EMA using an Efficiency Ratio to modify the smoothing constant, which ranges from a minimum of Fast Length to a maximum of Slow Length. Since this moving average is adaptive it tends to follow prices more closely than other MA's.
        /// </summary>
        /// <returns></returns>
        public Indicator.KAMA KAMA(Data.IDataSeries input, int fast, int period, int slow)
        {
            return _indicator.KAMA(input, fast, period, slow);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Developed by Perry Kaufman, this indicator is an EMA using an Efficiency Ratio to modify the smoothing constant, which ranges from a minimum of Fast Length to a maximum of Slow Length. Since this moving average is adaptive it tends to follow prices more closely than other MA's.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.KAMA KAMA(int fast, int period, int slow)
        {
            return _indicator.KAMA(Input, fast, period, slow);
        }

        /// <summary>
        /// Developed by Perry Kaufman, this indicator is an EMA using an Efficiency Ratio to modify the smoothing constant, which ranges from a minimum of Fast Length to a maximum of Slow Length. Since this moving average is adaptive it tends to follow prices more closely than other MA's.
        /// </summary>
        /// <returns></returns>
        public Indicator.KAMA KAMA(Data.IDataSeries input, int fast, int period, int slow)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.KAMA(input, fast, period, slow);
        }
    }
}
#endregion
