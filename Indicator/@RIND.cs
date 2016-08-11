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
	/// RIND (Range Indicator) compares the intraday range (high - low) to the inter-day (close - previous close) range. When the intraday range is greater than the inter-day range, the Range Indicator will be a high value. This signals an end to the current trend. When the Range Indicator is at a low level, a new trend is about to start.
	/// </summary>
	[Description("RIND (Range Indicator) compares the intraday range (high - low) to the inter-day (close - previous close) range. When the intraday range is greater than the inter-day range, the Range Indicator will be a high value. This signals an end to the current trend. When the Range Indicator is at a low level, a new trend is about to start.")]
	public class RIND : Indicator
	{
		#region Variables
		private int				periodQ		= 3;
		private int				smooth		= 10;

		private DataSeries stochRange;
		private DataSeries val1;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Green, "RIND"));

			stochRange			= new DataSeries(this);
			val1				= new DataSeries(this);

			Overlay				= false;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{

			if (CurrentBar == 0)
			{
				stochRange.Set(50);
				return;
			}

			double trueRange = Math.Max(High[0], Close[1]) - Math.Min(Low[0], Close[1]);

			if (Close[0] > Close[1])
				val1.Set(trueRange / (Close[0] - Close[1]));
			else
				val1.Set(trueRange);

			double val2 = MIN(val1, periodQ)[0];
			double val3 = MAX(val1, periodQ)[0];

			if ((val3 - val2) > 0)
				stochRange.Set(100 * ((val1[0] - val2) / (val3 - val2)));
			else
				stochRange.Set(100 * (val1[0] - val2));

			Value.Set(EMA(stochRange, smooth)[0]);
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for the short-term 'q' parameter stochastic range lookback.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Period q")]
		public int PeriodQ
		{
			get { return periodQ; }
			set { periodQ = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for EMA smoothing of the indicator.")]
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
        private RIND[] cacheRIND = null;

        private static RIND checkRIND = new RIND();

        /// <summary>
        /// RIND (Range Indicator) compares the intraday range (high - low) to the inter-day (close - previous close) range. When the intraday range is greater than the inter-day range, the Range Indicator will be a high value. This signals an end to the current trend. When the Range Indicator is at a low level, a new trend is about to start.
        /// </summary>
        /// <returns></returns>
        public RIND RIND(int periodQ, int smooth)
        {
            return RIND(Input, periodQ, smooth);
        }

        /// <summary>
        /// RIND (Range Indicator) compares the intraday range (high - low) to the inter-day (close - previous close) range. When the intraday range is greater than the inter-day range, the Range Indicator will be a high value. This signals an end to the current trend. When the Range Indicator is at a low level, a new trend is about to start.
        /// </summary>
        /// <returns></returns>
        public RIND RIND(Data.IDataSeries input, int periodQ, int smooth)
        {
            if (cacheRIND != null)
                for (int idx = 0; idx < cacheRIND.Length; idx++)
                    if (cacheRIND[idx].PeriodQ == periodQ && cacheRIND[idx].Smooth == smooth && cacheRIND[idx].EqualsInput(input))
                        return cacheRIND[idx];

            lock (checkRIND)
            {
                checkRIND.PeriodQ = periodQ;
                periodQ = checkRIND.PeriodQ;
                checkRIND.Smooth = smooth;
                smooth = checkRIND.Smooth;

                if (cacheRIND != null)
                    for (int idx = 0; idx < cacheRIND.Length; idx++)
                        if (cacheRIND[idx].PeriodQ == periodQ && cacheRIND[idx].Smooth == smooth && cacheRIND[idx].EqualsInput(input))
                            return cacheRIND[idx];

                RIND indicator = new RIND();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.PeriodQ = periodQ;
                indicator.Smooth = smooth;
                Indicators.Add(indicator);
                indicator.SetUp();

                RIND[] tmp = new RIND[cacheRIND == null ? 1 : cacheRIND.Length + 1];
                if (cacheRIND != null)
                    cacheRIND.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheRIND = tmp;
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
        /// RIND (Range Indicator) compares the intraday range (high - low) to the inter-day (close - previous close) range. When the intraday range is greater than the inter-day range, the Range Indicator will be a high value. This signals an end to the current trend. When the Range Indicator is at a low level, a new trend is about to start.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.RIND RIND(int periodQ, int smooth)
        {
            return _indicator.RIND(Input, periodQ, smooth);
        }

        /// <summary>
        /// RIND (Range Indicator) compares the intraday range (high - low) to the inter-day (close - previous close) range. When the intraday range is greater than the inter-day range, the Range Indicator will be a high value. This signals an end to the current trend. When the Range Indicator is at a low level, a new trend is about to start.
        /// </summary>
        /// <returns></returns>
        public Indicator.RIND RIND(Data.IDataSeries input, int periodQ, int smooth)
        {
            return _indicator.RIND(input, periodQ, smooth);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// RIND (Range Indicator) compares the intraday range (high - low) to the inter-day (close - previous close) range. When the intraday range is greater than the inter-day range, the Range Indicator will be a high value. This signals an end to the current trend. When the Range Indicator is at a low level, a new trend is about to start.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.RIND RIND(int periodQ, int smooth)
        {
            return _indicator.RIND(Input, periodQ, smooth);
        }

        /// <summary>
        /// RIND (Range Indicator) compares the intraday range (high - low) to the inter-day (close - previous close) range. When the intraday range is greater than the inter-day range, the Range Indicator will be a high value. This signals an end to the current trend. When the Range Indicator is at a low level, a new trend is about to start.
        /// </summary>
        /// <returns></returns>
        public Indicator.RIND RIND(Data.IDataSeries input, int periodQ, int smooth)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.RIND(input, periodQ, smooth);
        }
    }
}
#endregion
