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
	/// The Aroon Oscillator is based upon his Aroon Indicator. Much like the Aroon Indicator, the Aroon Oscillator measures the strength of a trend.
	/// </summary>
	[Description("The Aroon Oscillator is based upon his Aroon Indicator. Much like the Aroon Indicator, the Aroon Oscillator measures the strength of a trend.")]
	public class AroonOscillator : Indicator
	{
		#region Variables
		private int			period		= 14;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Line(Color.DarkGray, 0, "Zero line"));
			Add(new Plot(Color.Orange, "Up"));
			
			PriceTypeSupported	= true;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
            if (CurrentBar == 0)
                Value.Set(0);
            else
            {
                int back = Math.Min(Period, CurrentBar);
                int idxMax = -1;
                int idxMin = -1;
                double max = double.MinValue;
                double min = double.MaxValue;

                for (int idx = back; idx >= 0; idx--)
                {
                    if (High[back - idx] - double.Epsilon >= max)
                    {
                        max = High[back - idx];
                        idxMax = CurrentBar - back + idx;
                    }

                    if (Low[back - idx] + double.Epsilon <= min)
                    {
                        min = Low[back - idx];
                        idxMin = CurrentBar - back + idx;
                    }
                }

                Value.Set(100 * ((double)(back - (CurrentBar - idxMax)) / back) - 100 * ((double)(back - (CurrentBar - idxMin)) / back));
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
        private AroonOscillator[] cacheAroonOscillator = null;

        private static AroonOscillator checkAroonOscillator = new AroonOscillator();

        /// <summary>
        /// The Aroon Oscillator is based upon his Aroon Indicator. Much like the Aroon Indicator, the Aroon Oscillator measures the strength of a trend.
        /// </summary>
        /// <returns></returns>
        public AroonOscillator AroonOscillator(int period)
        {
            return AroonOscillator(Input, period);
        }

        /// <summary>
        /// The Aroon Oscillator is based upon his Aroon Indicator. Much like the Aroon Indicator, the Aroon Oscillator measures the strength of a trend.
        /// </summary>
        /// <returns></returns>
        public AroonOscillator AroonOscillator(Data.IDataSeries input, int period)
        {
            if (cacheAroonOscillator != null)
                for (int idx = 0; idx < cacheAroonOscillator.Length; idx++)
                    if (cacheAroonOscillator[idx].Period == period && cacheAroonOscillator[idx].EqualsInput(input))
                        return cacheAroonOscillator[idx];

            lock (checkAroonOscillator)
            {
                checkAroonOscillator.Period = period;
                period = checkAroonOscillator.Period;

                if (cacheAroonOscillator != null)
                    for (int idx = 0; idx < cacheAroonOscillator.Length; idx++)
                        if (cacheAroonOscillator[idx].Period == period && cacheAroonOscillator[idx].EqualsInput(input))
                            return cacheAroonOscillator[idx];

                AroonOscillator indicator = new AroonOscillator();
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

                AroonOscillator[] tmp = new AroonOscillator[cacheAroonOscillator == null ? 1 : cacheAroonOscillator.Length + 1];
                if (cacheAroonOscillator != null)
                    cacheAroonOscillator.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheAroonOscillator = tmp;
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
        /// The Aroon Oscillator is based upon his Aroon Indicator. Much like the Aroon Indicator, the Aroon Oscillator measures the strength of a trend.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AroonOscillator AroonOscillator(int period)
        {
            return _indicator.AroonOscillator(Input, period);
        }

        /// <summary>
        /// The Aroon Oscillator is based upon his Aroon Indicator. Much like the Aroon Indicator, the Aroon Oscillator measures the strength of a trend.
        /// </summary>
        /// <returns></returns>
        public Indicator.AroonOscillator AroonOscillator(Data.IDataSeries input, int period)
        {
            return _indicator.AroonOscillator(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Aroon Oscillator is based upon his Aroon Indicator. Much like the Aroon Indicator, the Aroon Oscillator measures the strength of a trend.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AroonOscillator AroonOscillator(int period)
        {
            return _indicator.AroonOscillator(Input, period);
        }

        /// <summary>
        /// The Aroon Oscillator is based upon his Aroon Indicator. Much like the Aroon Indicator, the Aroon Oscillator measures the strength of a trend.
        /// </summary>
        /// <returns></returns>
        public Indicator.AroonOscillator AroonOscillator(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.AroonOscillator(input, period);
        }
    }
}
#endregion
