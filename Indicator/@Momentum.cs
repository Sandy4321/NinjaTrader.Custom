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
	/// The Momentum indicator measures the amount that a security's price has changed over a given time span.
	/// </summary>
	[Description("The Momentum indicator measures the amount that a security's price has changed over a given time span.")]
	public class Momentum : Indicator
	{
		#region Variables
		private int		period	= 14;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Green, "Momentum"));
			Add(new Line(Color.DarkViolet, 0, "Zero line"));
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			Value.Set(CurrentBar == 0 ? 0 : Input[0] - Input[Math.Min(CurrentBar, Period)]);
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
        private Momentum[] cacheMomentum = null;

        private static Momentum checkMomentum = new Momentum();

        /// <summary>
        /// The Momentum indicator measures the amount that a security's price has changed over a given time span.
        /// </summary>
        /// <returns></returns>
        public Momentum Momentum(int period)
        {
            return Momentum(Input, period);
        }

        /// <summary>
        /// The Momentum indicator measures the amount that a security's price has changed over a given time span.
        /// </summary>
        /// <returns></returns>
        public Momentum Momentum(Data.IDataSeries input, int period)
        {
            if (cacheMomentum != null)
                for (int idx = 0; idx < cacheMomentum.Length; idx++)
                    if (cacheMomentum[idx].Period == period && cacheMomentum[idx].EqualsInput(input))
                        return cacheMomentum[idx];

            lock (checkMomentum)
            {
                checkMomentum.Period = period;
                period = checkMomentum.Period;

                if (cacheMomentum != null)
                    for (int idx = 0; idx < cacheMomentum.Length; idx++)
                        if (cacheMomentum[idx].Period == period && cacheMomentum[idx].EqualsInput(input))
                            return cacheMomentum[idx];

                Momentum indicator = new Momentum();
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

                Momentum[] tmp = new Momentum[cacheMomentum == null ? 1 : cacheMomentum.Length + 1];
                if (cacheMomentum != null)
                    cacheMomentum.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheMomentum = tmp;
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
        /// The Momentum indicator measures the amount that a security's price has changed over a given time span.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Momentum Momentum(int period)
        {
            return _indicator.Momentum(Input, period);
        }

        /// <summary>
        /// The Momentum indicator measures the amount that a security's price has changed over a given time span.
        /// </summary>
        /// <returns></returns>
        public Indicator.Momentum Momentum(Data.IDataSeries input, int period)
        {
            return _indicator.Momentum(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Momentum indicator measures the amount that a security's price has changed over a given time span.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Momentum Momentum(int period)
        {
            return _indicator.Momentum(Input, period);
        }

        /// <summary>
        /// The Momentum indicator measures the amount that a security's price has changed over a given time span.
        /// </summary>
        /// <returns></returns>
        public Indicator.Momentum Momentum(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Momentum(input, period);
        }
    }
}
#endregion
