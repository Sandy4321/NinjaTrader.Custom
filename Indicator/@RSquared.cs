// 
// Copyright (C) 2008, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//
#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// R-squared indicator
    /// </summary>
    [Description("R-squared indicator")]
    public class RSquared : Indicator
    {
        #region Variables
		private int period = 8; // Default setting for Period
		private double	sumX	= 0;
		private double	divisor = 0;
		private double	sumXY	= 0;
		private double	sumX2	= 0;
		private double	sumY2	= 0;
		private double numerator = 0;
		private double denominator = 0;
		private double r = 0;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Red, PlotStyle.Line, "R-Squared"));
			Add(new Line(Color.DarkViolet, 0.2, "Lower"));
			Add(new Line(Color.YellowGreen, 0.75, "Upper"));
			
            Overlay				= false;
            PriceTypeSupported	= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			sumX	= (double) Period * (Period - 1) * 0.5;
			divisor = sumX * sumX - (double) Period * Period * (Period - 1) * (2 * Period - 1) / 6;
			sumXY	= 0;
			sumX2	= 0;
			sumY2	= 0;

			for (int count = 0; count < Period && CurrentBar - count >= 0; count++)
			{
				sumXY += count * Input[count];
				sumX2 += (count * count);
				sumY2 += (Input[count] * Input[count]);
			}
			
			numerator = (Period * sumXY - sumX * SUM(Inputs[0], Period)[0]);
			denominator = (Period * sumX2- (sumX*sumX)) * (Period*sumY2 - (SUM(Inputs[0], Period)[0]*SUM(Inputs[0], Period)[0]));
			
			if (denominator > 0)
				r = Math.Pow((numerator / Math.Sqrt(denominator)), 2);
			else
				r = 0;
			Value.Set(r);

        }

        #region Properties
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
        private RSquared[] cacheRSquared = null;

        private static RSquared checkRSquared = new RSquared();

        /// <summary>
        /// R-squared indicator
        /// </summary>
        /// <returns></returns>
        public RSquared RSquared(int period)
        {
            return RSquared(Input, period);
        }

        /// <summary>
        /// R-squared indicator
        /// </summary>
        /// <returns></returns>
        public RSquared RSquared(Data.IDataSeries input, int period)
        {
            if (cacheRSquared != null)
                for (int idx = 0; idx < cacheRSquared.Length; idx++)
                    if (cacheRSquared[idx].Period == period && cacheRSquared[idx].EqualsInput(input))
                        return cacheRSquared[idx];

            lock (checkRSquared)
            {
                checkRSquared.Period = period;
                period = checkRSquared.Period;

                if (cacheRSquared != null)
                    for (int idx = 0; idx < cacheRSquared.Length; idx++)
                        if (cacheRSquared[idx].Period == period && cacheRSquared[idx].EqualsInput(input))
                            return cacheRSquared[idx];

                RSquared indicator = new RSquared();
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

                RSquared[] tmp = new RSquared[cacheRSquared == null ? 1 : cacheRSquared.Length + 1];
                if (cacheRSquared != null)
                    cacheRSquared.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheRSquared = tmp;
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
        /// R-squared indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.RSquared RSquared(int period)
        {
            return _indicator.RSquared(Input, period);
        }

        /// <summary>
        /// R-squared indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.RSquared RSquared(Data.IDataSeries input, int period)
        {
            return _indicator.RSquared(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// R-squared indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.RSquared RSquared(int period)
        {
            return _indicator.RSquared(Input, period);
        }

        /// <summary>
        /// R-squared indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.RSquared RSquared(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.RSquared(input, period);
        }
    }
}
#endregion
