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
	/// The Commodity Channel Index (CCI) measures the variation of a security's price from its statistical mean. High values show that prices are unusually high compared to average prices whereas low values indicate that prices are unusually low.
	/// </summary>
	[Description("The Commodity Channel Index (CCI) measures the variation of a security's price from its statistical mean. High values show that prices are unusually high compared to average prices whereas low values indicate that prices are unusually low.")]
	public class CCI : Indicator
	{
		#region Variables
		private int			period		= 14;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Orange, "CCI"));
			Add(new Line(Color.DarkGray, 200, "Level 2"));
			Add(new Line(Color.DarkGray, 100, "Level 1"));
			Add(new Line(Color.DarkGray, 0, "Zero line"));
			Add(new Line(Color.DarkGray, -100, "Level -1"));
			Add(new Line(Color.DarkGray, -200, "Level -2"));
		}

		/// <summary>
		/// Calculates the indicator value(s) at the current index.
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
				Value.Set(0);
			else
			{
				double mean = 0;
				for (int idx = Math.Min(CurrentBar, Period - 1); idx >= 0; idx--)
					mean += Math.Abs(Typical[idx] - SMA(Typical, Period)[0]);
				Value.Set((Typical[0] - SMA(Typical, Period)[0]) / (mean == 0 ? 1 : (0.015 * (mean / Math.Min(Period, CurrentBar + 1)))));
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
        private CCI[] cacheCCI = null;

        private static CCI checkCCI = new CCI();

        /// <summary>
        /// The Commodity Channel Index (CCI) measures the variation of a security's price from its statistical mean. High values show that prices are unusually high compared to average prices whereas low values indicate that prices are unusually low.
        /// </summary>
        /// <returns></returns>
        public CCI CCI(int period)
        {
            return CCI(Input, period);
        }

        /// <summary>
        /// The Commodity Channel Index (CCI) measures the variation of a security's price from its statistical mean. High values show that prices are unusually high compared to average prices whereas low values indicate that prices are unusually low.
        /// </summary>
        /// <returns></returns>
        public CCI CCI(Data.IDataSeries input, int period)
        {
            if (cacheCCI != null)
                for (int idx = 0; idx < cacheCCI.Length; idx++)
                    if (cacheCCI[idx].Period == period && cacheCCI[idx].EqualsInput(input))
                        return cacheCCI[idx];

            lock (checkCCI)
            {
                checkCCI.Period = period;
                period = checkCCI.Period;

                if (cacheCCI != null)
                    for (int idx = 0; idx < cacheCCI.Length; idx++)
                        if (cacheCCI[idx].Period == period && cacheCCI[idx].EqualsInput(input))
                            return cacheCCI[idx];

                CCI indicator = new CCI();
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

                CCI[] tmp = new CCI[cacheCCI == null ? 1 : cacheCCI.Length + 1];
                if (cacheCCI != null)
                    cacheCCI.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheCCI = tmp;
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
        /// The Commodity Channel Index (CCI) measures the variation of a security's price from its statistical mean. High values show that prices are unusually high compared to average prices whereas low values indicate that prices are unusually low.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.CCI CCI(int period)
        {
            return _indicator.CCI(Input, period);
        }

        /// <summary>
        /// The Commodity Channel Index (CCI) measures the variation of a security's price from its statistical mean. High values show that prices are unusually high compared to average prices whereas low values indicate that prices are unusually low.
        /// </summary>
        /// <returns></returns>
        public Indicator.CCI CCI(Data.IDataSeries input, int period)
        {
            return _indicator.CCI(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Commodity Channel Index (CCI) measures the variation of a security's price from its statistical mean. High values show that prices are unusually high compared to average prices whereas low values indicate that prices are unusually low.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.CCI CCI(int period)
        {
            return _indicator.CCI(Input, period);
        }

        /// <summary>
        /// The Commodity Channel Index (CCI) measures the variation of a security's price from its statistical mean. High values show that prices are unusually high compared to average prices whereas low values indicate that prices are unusually low.
        /// </summary>
        /// <returns></returns>
        public Indicator.CCI CCI(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.CCI(input, period);
        }
    }
}
#endregion
