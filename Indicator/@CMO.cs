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
	/// The CMO differs from other momentum oscillators such as Relative Strength Index (RSI) and Stochastics. It uses both up and down days data in the numerator of the calculation to measure momentum directly. Primarily used to look for extreme overbought and oversold conditions, CMO can also be used to look for trends.
	/// </summary>
	[Description("The CMO differs from other momentum oscillators such as Relative Strength Index (RSI) and Stochastics. It uses both up and down days data in the numerator of the calculation to measure momentum directly. Primarily used to look for extreme overbought and oversold conditions, CMO can also be used to look for trends.")]
	public class CMO : Indicator
	{
		#region Variables
		private int				period		= 14;

		private DataSeries	down;
		private DataSeries up;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Blue, "CMO"));

			down				= new DataSeries(this);
			up					= new DataSeries(this);

			Overlay				= false;
		}
		
		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				down.Set(0);
				up.Set(0);
				return;
			}

            down.Set(Math.Max(Input[1] - Input[0], 0));
            up.Set(Math.Max(Input[0] - Input[1], 0));

			double downs = SUM(down, Period)[0];
			double ups   = SUM(up, Period)[0];

			if (Math.Abs(ups + downs) < double.Epsilon)
				Value.Set(0);
			else
				Value.Set(100 * ((ups - downs) / (ups + downs)));
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
        private CMO[] cacheCMO = null;

        private static CMO checkCMO = new CMO();

        /// <summary>
        /// The CMO differs from other momentum oscillators such as Relative Strength Index (RSI) and Stochastics. It uses both up and down days data in the numerator of the calculation to measure momentum directly. Primarily used to look for extreme overbought and oversold conditions, CMO can also be used to look for trends.
        /// </summary>
        /// <returns></returns>
        public CMO CMO(int period)
        {
            return CMO(Input, period);
        }

        /// <summary>
        /// The CMO differs from other momentum oscillators such as Relative Strength Index (RSI) and Stochastics. It uses both up and down days data in the numerator of the calculation to measure momentum directly. Primarily used to look for extreme overbought and oversold conditions, CMO can also be used to look for trends.
        /// </summary>
        /// <returns></returns>
        public CMO CMO(Data.IDataSeries input, int period)
        {
            if (cacheCMO != null)
                for (int idx = 0; idx < cacheCMO.Length; idx++)
                    if (cacheCMO[idx].Period == period && cacheCMO[idx].EqualsInput(input))
                        return cacheCMO[idx];

            lock (checkCMO)
            {
                checkCMO.Period = period;
                period = checkCMO.Period;

                if (cacheCMO != null)
                    for (int idx = 0; idx < cacheCMO.Length; idx++)
                        if (cacheCMO[idx].Period == period && cacheCMO[idx].EqualsInput(input))
                            return cacheCMO[idx];

                CMO indicator = new CMO();
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

                CMO[] tmp = new CMO[cacheCMO == null ? 1 : cacheCMO.Length + 1];
                if (cacheCMO != null)
                    cacheCMO.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheCMO = tmp;
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
        /// The CMO differs from other momentum oscillators such as Relative Strength Index (RSI) and Stochastics. It uses both up and down days data in the numerator of the calculation to measure momentum directly. Primarily used to look for extreme overbought and oversold conditions, CMO can also be used to look for trends.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.CMO CMO(int period)
        {
            return _indicator.CMO(Input, period);
        }

        /// <summary>
        /// The CMO differs from other momentum oscillators such as Relative Strength Index (RSI) and Stochastics. It uses both up and down days data in the numerator of the calculation to measure momentum directly. Primarily used to look for extreme overbought and oversold conditions, CMO can also be used to look for trends.
        /// </summary>
        /// <returns></returns>
        public Indicator.CMO CMO(Data.IDataSeries input, int period)
        {
            return _indicator.CMO(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The CMO differs from other momentum oscillators such as Relative Strength Index (RSI) and Stochastics. It uses both up and down days data in the numerator of the calculation to measure momentum directly. Primarily used to look for extreme overbought and oversold conditions, CMO can also be used to look for trends.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.CMO CMO(int period)
        {
            return _indicator.CMO(Input, period);
        }

        /// <summary>
        /// The CMO differs from other momentum oscillators such as Relative Strength Index (RSI) and Stochastics. It uses both up and down days data in the numerator of the calculation to measure momentum directly. Primarily used to look for extreme overbought and oversold conditions, CMO can also be used to look for trends.
        /// </summary>
        /// <returns></returns>
        public Indicator.CMO CMO(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.CMO(input, period);
        }
    }
}
#endregion
