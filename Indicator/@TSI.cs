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
	/// The TSI (True Strength Index) is a momentum-based indicator, developed by William Blau. Designed to determine both trend and overbought/oversold conditions, the TSI is applicable to intraday time frames as well as long term trading.
	/// </summary>
	[Description("The TSI (True Strength Index) is a momentum-based indicator, developed by William Blau. Designed to determine both trend and overbought/oversold conditions, the TSI is applicable to intraday time frames as well as long term trading.")]
	public class TSI : Indicator
	{
		#region Variables
		private int					fast	= 3;
		private int					slow	= 14;

		DataSeries				fastEma;
		DataSeries				fastAbsEma;
		DataSeries				slowEma;
		DataSeries				slowAbsEma;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Green, "TSI"));

			fastAbsEma	= new DataSeries(this);
			fastEma		= new DataSeries(this);
			slowAbsEma	= new DataSeries(this);
			slowEma		= new DataSeries(this);
		}
		
		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				fastAbsEma.Set(0);
				fastEma.Set(0);
				slowAbsEma.Set(0);
				slowEma.Set(0);
				Value.Set(0);
			}
			else
			{
				double momentum	= Input[0] - Input[1];
				slowEma.Set(momentum * (2.0 / (1 + Slow)) + (1 - (2.0 / (1 + Slow))) * slowEma[1]);
				fastEma.Set(slowEma[0] * (2.0 / (1 + Fast)) + (1 - (2.0 / (1 + Fast))) * fastEma[1]);
				slowAbsEma.Set(Math.Abs(momentum) * (2.0 / (1 + Slow)) + (1 - (2.0 / (1 + Slow)))* slowAbsEma[1]);
				fastAbsEma.Set(slowAbsEma[0] * (2.0 / (1 + Fast)) + (1 - (2.0 / (1 + Fast))) * fastAbsEma[1]);
				Value.Set(fastAbsEma[0] == 0 ? 0 : 100 * fastEma[0] / fastAbsEma[0]);
			}
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Number of bars for fast EMA")]
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
			set	{ slow = Math.Max(1, value); }
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
        private TSI[] cacheTSI = null;

        private static TSI checkTSI = new TSI();

        /// <summary>
        /// The TSI (True Strength Index) is a momentum-based indicator, developed by William Blau. Designed to determine both trend and overbought/oversold conditions, the TSI is applicable to intraday time frames as well as long term trading.
        /// </summary>
        /// <returns></returns>
        public TSI TSI(int fast, int slow)
        {
            return TSI(Input, fast, slow);
        }

        /// <summary>
        /// The TSI (True Strength Index) is a momentum-based indicator, developed by William Blau. Designed to determine both trend and overbought/oversold conditions, the TSI is applicable to intraday time frames as well as long term trading.
        /// </summary>
        /// <returns></returns>
        public TSI TSI(Data.IDataSeries input, int fast, int slow)
        {
            if (cacheTSI != null)
                for (int idx = 0; idx < cacheTSI.Length; idx++)
                    if (cacheTSI[idx].Fast == fast && cacheTSI[idx].Slow == slow && cacheTSI[idx].EqualsInput(input))
                        return cacheTSI[idx];

            lock (checkTSI)
            {
                checkTSI.Fast = fast;
                fast = checkTSI.Fast;
                checkTSI.Slow = slow;
                slow = checkTSI.Slow;

                if (cacheTSI != null)
                    for (int idx = 0; idx < cacheTSI.Length; idx++)
                        if (cacheTSI[idx].Fast == fast && cacheTSI[idx].Slow == slow && cacheTSI[idx].EqualsInput(input))
                            return cacheTSI[idx];

                TSI indicator = new TSI();
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

                TSI[] tmp = new TSI[cacheTSI == null ? 1 : cacheTSI.Length + 1];
                if (cacheTSI != null)
                    cacheTSI.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTSI = tmp;
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
        /// The TSI (True Strength Index) is a momentum-based indicator, developed by William Blau. Designed to determine both trend and overbought/oversold conditions, the TSI is applicable to intraday time frames as well as long term trading.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TSI TSI(int fast, int slow)
        {
            return _indicator.TSI(Input, fast, slow);
        }

        /// <summary>
        /// The TSI (True Strength Index) is a momentum-based indicator, developed by William Blau. Designed to determine both trend and overbought/oversold conditions, the TSI is applicable to intraday time frames as well as long term trading.
        /// </summary>
        /// <returns></returns>
        public Indicator.TSI TSI(Data.IDataSeries input, int fast, int slow)
        {
            return _indicator.TSI(input, fast, slow);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The TSI (True Strength Index) is a momentum-based indicator, developed by William Blau. Designed to determine both trend and overbought/oversold conditions, the TSI is applicable to intraday time frames as well as long term trading.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TSI TSI(int fast, int slow)
        {
            return _indicator.TSI(Input, fast, slow);
        }

        /// <summary>
        /// The TSI (True Strength Index) is a momentum-based indicator, developed by William Blau. Designed to determine both trend and overbought/oversold conditions, the TSI is applicable to intraday time frames as well as long term trading.
        /// </summary>
        /// <returns></returns>
        public Indicator.TSI TSI(Data.IDataSeries input, int fast, int slow)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TSI(input, fast, slow);
        }
    }
}
#endregion
