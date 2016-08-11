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
	/// The StochRSI is an oscillator similar in computation to the stochastic measure, except instead of price values as input, the StochRSI uses RSI values. The StochRSI computes the current position of the RSI relative to the high and low RSI values over a specified number of days. The intent of this measure, designed by Tushard Chande and Stanley Kroll, is to provide further information about the overbought/oversold nature of the RSI. The StochRSI ranges between 0.0 and 1.0. Values above 0.8 are generally seen to identify overbought levels and values below 0.2 are considered to indicate oversold conditions.
	/// </summary>
	[Description("The StochRSI is an oscillator similar in computation to the stochastic measure, except instead of price values as input, the StochRSI uses RSI values. The StochRSI computes the current position of the RSI relative to the high and low RSI values over a specified number of days. The intent of this measure, designed by Tushard Chande and Stanley Kroll, is to provide further information about the overbought/oversold nature of the RSI. The StochRSI ranges between 0.0 and 1.0. Values above 0.8 are generally seen to identify overbought levels and values below 0.2 are considered to indicate oversold conditions.")]
	public class StochRSI : Indicator
	{
		#region Variables
		private int			period		= 14;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Green, "StochRSI"));

			Add(new Line(Color.Red,  0.8, "Overbought"));
			Add(new Line(Color.Blue, 0.5, "Neutral"));
			Add(new Line(Color.Red,  0.2, "Oversold"));

			Overlay				= false;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			double rsi  = RSI(Inputs[0], period, 1)[0];
			double rsiL = MIN(RSI(Inputs[0], period, 1), period)[0];
			double rsiH = MAX(RSI(Inputs[0], period, 1), period)[0];

			if (rsi != rsiL && rsiH != rsiL)
				Value.Set((rsi - rsiL) / (rsiH - rsiL));
			else
				Value.Set(0);
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
        private StochRSI[] cacheStochRSI = null;

        private static StochRSI checkStochRSI = new StochRSI();

        /// <summary>
        /// The StochRSI is an oscillator similar in computation to the stochastic measure, except instead of price values as input, the StochRSI uses RSI values. The StochRSI computes the current position of the RSI relative to the high and low RSI values over a specified number of days. The intent of this measure, designed by Tushard Chande and Stanley Kroll, is to provide further information about the overbought/oversold nature of the RSI. The StochRSI ranges between 0.0 and 1.0. Values above 0.8 are generally seen to identify overbought levels and values below 0.2 are considered to indicate oversold conditions.
        /// </summary>
        /// <returns></returns>
        public StochRSI StochRSI(int period)
        {
            return StochRSI(Input, period);
        }

        /// <summary>
        /// The StochRSI is an oscillator similar in computation to the stochastic measure, except instead of price values as input, the StochRSI uses RSI values. The StochRSI computes the current position of the RSI relative to the high and low RSI values over a specified number of days. The intent of this measure, designed by Tushard Chande and Stanley Kroll, is to provide further information about the overbought/oversold nature of the RSI. The StochRSI ranges between 0.0 and 1.0. Values above 0.8 are generally seen to identify overbought levels and values below 0.2 are considered to indicate oversold conditions.
        /// </summary>
        /// <returns></returns>
        public StochRSI StochRSI(Data.IDataSeries input, int period)
        {
            if (cacheStochRSI != null)
                for (int idx = 0; idx < cacheStochRSI.Length; idx++)
                    if (cacheStochRSI[idx].Period == period && cacheStochRSI[idx].EqualsInput(input))
                        return cacheStochRSI[idx];

            lock (checkStochRSI)
            {
                checkStochRSI.Period = period;
                period = checkStochRSI.Period;

                if (cacheStochRSI != null)
                    for (int idx = 0; idx < cacheStochRSI.Length; idx++)
                        if (cacheStochRSI[idx].Period == period && cacheStochRSI[idx].EqualsInput(input))
                            return cacheStochRSI[idx];

                StochRSI indicator = new StochRSI();
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

                StochRSI[] tmp = new StochRSI[cacheStochRSI == null ? 1 : cacheStochRSI.Length + 1];
                if (cacheStochRSI != null)
                    cacheStochRSI.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheStochRSI = tmp;
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
        /// The StochRSI is an oscillator similar in computation to the stochastic measure, except instead of price values as input, the StochRSI uses RSI values. The StochRSI computes the current position of the RSI relative to the high and low RSI values over a specified number of days. The intent of this measure, designed by Tushard Chande and Stanley Kroll, is to provide further information about the overbought/oversold nature of the RSI. The StochRSI ranges between 0.0 and 1.0. Values above 0.8 are generally seen to identify overbought levels and values below 0.2 are considered to indicate oversold conditions.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.StochRSI StochRSI(int period)
        {
            return _indicator.StochRSI(Input, period);
        }

        /// <summary>
        /// The StochRSI is an oscillator similar in computation to the stochastic measure, except instead of price values as input, the StochRSI uses RSI values. The StochRSI computes the current position of the RSI relative to the high and low RSI values over a specified number of days. The intent of this measure, designed by Tushard Chande and Stanley Kroll, is to provide further information about the overbought/oversold nature of the RSI. The StochRSI ranges between 0.0 and 1.0. Values above 0.8 are generally seen to identify overbought levels and values below 0.2 are considered to indicate oversold conditions.
        /// </summary>
        /// <returns></returns>
        public Indicator.StochRSI StochRSI(Data.IDataSeries input, int period)
        {
            return _indicator.StochRSI(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The StochRSI is an oscillator similar in computation to the stochastic measure, except instead of price values as input, the StochRSI uses RSI values. The StochRSI computes the current position of the RSI relative to the high and low RSI values over a specified number of days. The intent of this measure, designed by Tushard Chande and Stanley Kroll, is to provide further information about the overbought/oversold nature of the RSI. The StochRSI ranges between 0.0 and 1.0. Values above 0.8 are generally seen to identify overbought levels and values below 0.2 are considered to indicate oversold conditions.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.StochRSI StochRSI(int period)
        {
            return _indicator.StochRSI(Input, period);
        }

        /// <summary>
        /// The StochRSI is an oscillator similar in computation to the stochastic measure, except instead of price values as input, the StochRSI uses RSI values. The StochRSI computes the current position of the RSI relative to the high and low RSI values over a specified number of days. The intent of this measure, designed by Tushard Chande and Stanley Kroll, is to provide further information about the overbought/oversold nature of the RSI. The StochRSI ranges between 0.0 and 1.0. Values above 0.8 are generally seen to identify overbought levels and values below 0.2 are considered to indicate oversold conditions.
        /// </summary>
        /// <returns></returns>
        public Indicator.StochRSI StochRSI(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.StochRSI(input, period);
        }
    }
}
#endregion
