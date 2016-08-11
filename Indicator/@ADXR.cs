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
	/// Average Directional Movement Rating quantifies momentum change in the ADX. It is calculated by adding two values of ADX (the current value and a value n periods back), then dividing by two. This additional smoothing makes the ADXR slightly less responsive than ADX. The interpretation is the same as the ADX; the higher the value, the stronger the trend.
	/// </summary>
	[Description("Average Directional Movement Rating quantifies momentum change in the ADX. It is calculated by adding two values of ADX (the current value and a value n periods back), then dividing by two. This additional smoothing makes the ADXR slightly less responsive than ADX. The interpretation is the same as the ADX; the higher the value, the stronger the trend.")]
	public class ADXR : Indicator
	{
		#region Variables
		private int interval = 10;
		private int period	 = 14;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Green, "ADXR"));
			Add(new Line(Color.DarkViolet, 25, "Lower"));
			Add(new Line(Color.YellowGreen, 75, "Upper"));
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar < Interval)
				Value.Set((ADX(period)[0] + ADX(period)[CurrentBar]) / 2);
			else
				Value.Set((ADX(period)[0] + ADX(period)[interval]) / 2);
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Used to average the current ADX value with the ADX n periods ago.")]
		[GridCategory("Parameters")]
		public int Interval
		{
			get { return interval; }
			set { interval = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculating the ADX.")]
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
        private ADXR[] cacheADXR = null;

        private static ADXR checkADXR = new ADXR();

        /// <summary>
        /// Average Directional Movement Rating quantifies momentum change in the ADX. It is calculated by adding two values of ADX (the current value and a value n periods back), then dividing by two. This additional smoothing makes the ADXR slightly less responsive than ADX. The interpretation is the same as the ADX; the higher the value, the stronger the trend.
        /// </summary>
        /// <returns></returns>
        public ADXR ADXR(int interval, int period)
        {
            return ADXR(Input, interval, period);
        }

        /// <summary>
        /// Average Directional Movement Rating quantifies momentum change in the ADX. It is calculated by adding two values of ADX (the current value and a value n periods back), then dividing by two. This additional smoothing makes the ADXR slightly less responsive than ADX. The interpretation is the same as the ADX; the higher the value, the stronger the trend.
        /// </summary>
        /// <returns></returns>
        public ADXR ADXR(Data.IDataSeries input, int interval, int period)
        {
            if (cacheADXR != null)
                for (int idx = 0; idx < cacheADXR.Length; idx++)
                    if (cacheADXR[idx].Interval == interval && cacheADXR[idx].Period == period && cacheADXR[idx].EqualsInput(input))
                        return cacheADXR[idx];

            lock (checkADXR)
            {
                checkADXR.Interval = interval;
                interval = checkADXR.Interval;
                checkADXR.Period = period;
                period = checkADXR.Period;

                if (cacheADXR != null)
                    for (int idx = 0; idx < cacheADXR.Length; idx++)
                        if (cacheADXR[idx].Interval == interval && cacheADXR[idx].Period == period && cacheADXR[idx].EqualsInput(input))
                            return cacheADXR[idx];

                ADXR indicator = new ADXR();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Interval = interval;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                ADXR[] tmp = new ADXR[cacheADXR == null ? 1 : cacheADXR.Length + 1];
                if (cacheADXR != null)
                    cacheADXR.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheADXR = tmp;
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
        /// Average Directional Movement Rating quantifies momentum change in the ADX. It is calculated by adding two values of ADX (the current value and a value n periods back), then dividing by two. This additional smoothing makes the ADXR slightly less responsive than ADX. The interpretation is the same as the ADX; the higher the value, the stronger the trend.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ADXR ADXR(int interval, int period)
        {
            return _indicator.ADXR(Input, interval, period);
        }

        /// <summary>
        /// Average Directional Movement Rating quantifies momentum change in the ADX. It is calculated by adding two values of ADX (the current value and a value n periods back), then dividing by two. This additional smoothing makes the ADXR slightly less responsive than ADX. The interpretation is the same as the ADX; the higher the value, the stronger the trend.
        /// </summary>
        /// <returns></returns>
        public Indicator.ADXR ADXR(Data.IDataSeries input, int interval, int period)
        {
            return _indicator.ADXR(input, interval, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Average Directional Movement Rating quantifies momentum change in the ADX. It is calculated by adding two values of ADX (the current value and a value n periods back), then dividing by two. This additional smoothing makes the ADXR slightly less responsive than ADX. The interpretation is the same as the ADX; the higher the value, the stronger the trend.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ADXR ADXR(int interval, int period)
        {
            return _indicator.ADXR(Input, interval, period);
        }

        /// <summary>
        /// Average Directional Movement Rating quantifies momentum change in the ADX. It is calculated by adding two values of ADX (the current value and a value n periods back), then dividing by two. This additional smoothing makes the ADXR slightly less responsive than ADX. The interpretation is the same as the ADX; the higher the value, the stronger the trend.
        /// </summary>
        /// <returns></returns>
        public Indicator.ADXR ADXR(Data.IDataSeries input, int interval, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ADXR(input, interval, period);
        }
    }
}
#endregion
