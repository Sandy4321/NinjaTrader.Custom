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
	/// The RVI (Relative Volatility Index) was developed by Donald Dorsey as a compliment to and a confirmation of momentum based indicators. When used to confirm other signals, only buy when the RVI is over 50 and only sell when the RVI is under 50.
	/// </summary>
	[Description("The RVI (Relative Volatility Index) was developed by Donald Dorsey as a compliment to and a confirmation of momentum based indicators. When used to confirm other signals, only buy when the RVI is over 50 and only sell when the RVI is under 50.")]
	public class RVI : Indicator
	{
		#region Variables
		private int				period			= 14;
		private int     		savedCurrentBar	= -1;

		private double			dnAvgH		= 0;
		private double			dnAvgL		= 0;
		private double			upAvgH		= 0;
		private double			upAvgL		= 0;

		private double			lastDnAvgH	= 0;
		private double			lastDnAvgL	= 0;
		private double			lastUpAvgH	= 0;
		private double			lastUpAvgL	= 0;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.DarkOrange, "RVI"));
			Add(new Line(Color.LightGray, 50, "SignalLine"));

			Overlay = false;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				Value.Set(50);
				return;
			}

			if (CurrentBar != savedCurrentBar)
			{
				dnAvgH = lastDnAvgH;
				dnAvgL = lastDnAvgL;
				upAvgH = lastUpAvgH;
				upAvgL = lastUpAvgL;
				savedCurrentBar	= CurrentBar;
			}

			double up;
			double dn;
			
			// RVI(High)
			up = 0;
			dn = 0;

			if (High[0] > High[1])
				up = StdDev(High, 10)[0];

			if (High[0] < High[1])
				dn = StdDev(High, 10)[0];

			double actUpAvgH = lastUpAvgH = (upAvgH * (Period - 1) + up) / Period;
			double actDnAvgH = lastDnAvgH = (dnAvgH * (Period - 1) + dn) / Period;

			double rviH = 100 * (actUpAvgH / (actUpAvgH + actDnAvgH));

			// RVI(Low)
			up = 0;
			dn = 0;

			if (Low[0] > Low[1])
				up = StdDev(Low, 10)[0];

			if (Low[0] < Low[1])
				dn = StdDev(Low, 10)[0];

			double actUpAvgL = lastUpAvgL = (upAvgL * (Period - 1) + up) / Period;
			double actDnAvgL = lastDnAvgL = (dnAvgL * (Period - 1) + dn) / Period;

			double rviL = 100 * (actUpAvgL / (actUpAvgL + actDnAvgL));

			double rvi = (rviH + rviL) / 2;

			Value.Set(rvi);
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculations.")]
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
        private RVI[] cacheRVI = null;

        private static RVI checkRVI = new RVI();

        /// <summary>
        /// The RVI (Relative Volatility Index) was developed by Donald Dorsey as a compliment to and a confirmation of momentum based indicators. When used to confirm other signals, only buy when the RVI is over 50 and only sell when the RVI is under 50.
        /// </summary>
        /// <returns></returns>
        public RVI RVI(int period)
        {
            return RVI(Input, period);
        }

        /// <summary>
        /// The RVI (Relative Volatility Index) was developed by Donald Dorsey as a compliment to and a confirmation of momentum based indicators. When used to confirm other signals, only buy when the RVI is over 50 and only sell when the RVI is under 50.
        /// </summary>
        /// <returns></returns>
        public RVI RVI(Data.IDataSeries input, int period)
        {
            if (cacheRVI != null)
                for (int idx = 0; idx < cacheRVI.Length; idx++)
                    if (cacheRVI[idx].Period == period && cacheRVI[idx].EqualsInput(input))
                        return cacheRVI[idx];

            lock (checkRVI)
            {
                checkRVI.Period = period;
                period = checkRVI.Period;

                if (cacheRVI != null)
                    for (int idx = 0; idx < cacheRVI.Length; idx++)
                        if (cacheRVI[idx].Period == period && cacheRVI[idx].EqualsInput(input))
                            return cacheRVI[idx];

                RVI indicator = new RVI();
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

                RVI[] tmp = new RVI[cacheRVI == null ? 1 : cacheRVI.Length + 1];
                if (cacheRVI != null)
                    cacheRVI.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheRVI = tmp;
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
        /// The RVI (Relative Volatility Index) was developed by Donald Dorsey as a compliment to and a confirmation of momentum based indicators. When used to confirm other signals, only buy when the RVI is over 50 and only sell when the RVI is under 50.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.RVI RVI(int period)
        {
            return _indicator.RVI(Input, period);
        }

        /// <summary>
        /// The RVI (Relative Volatility Index) was developed by Donald Dorsey as a compliment to and a confirmation of momentum based indicators. When used to confirm other signals, only buy when the RVI is over 50 and only sell when the RVI is under 50.
        /// </summary>
        /// <returns></returns>
        public Indicator.RVI RVI(Data.IDataSeries input, int period)
        {
            return _indicator.RVI(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The RVI (Relative Volatility Index) was developed by Donald Dorsey as a compliment to and a confirmation of momentum based indicators. When used to confirm other signals, only buy when the RVI is over 50 and only sell when the RVI is under 50.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.RVI RVI(int period)
        {
            return _indicator.RVI(Input, period);
        }

        /// <summary>
        /// The RVI (Relative Volatility Index) was developed by Donald Dorsey as a compliment to and a confirmation of momentum based indicators. When used to confirm other signals, only buy when the RVI is over 50 and only sell when the RVI is under 50.
        /// </summary>
        /// <returns></returns>
        public Indicator.RVI RVI(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.RVI(input, period);
        }
    }
}
#endregion
