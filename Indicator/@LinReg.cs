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
	/// Linear Regression. The Linear Regression is an indicator that 'predicts' the value of a security's price.
	/// </summary>
	[Description("The Linear Regression is an indicator that 'predicts' the value of a security's price.")]
	public class LinReg : Indicator
	{
		#region Variables
		private int					period	= 14;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Orange, "LinReg"));
	
			Overlay				= true;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			double	sumX	= (double) Period * (Period - 1) * 0.5;
			double	divisor = sumX * sumX - (double) Period * Period * (Period - 1) * (2 * Period - 1) / 6;
			double	sumXY	= 0;

			for (int count = 0; count < Period && CurrentBar - count >= 0; count++)
				sumXY += count * Input[count];
			
            double slope        = ((double)Period * sumXY - sumX * SUM(Inputs[0], Period)[0]) / divisor;
            double intercept    = (SUM(Inputs[0], Period)[0] - slope * sumX) / Period;
			
			Value.Set(intercept + slope * (Period - 1));
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(2, value); }
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
        private LinReg[] cacheLinReg = null;

        private static LinReg checkLinReg = new LinReg();

        /// <summary>
        /// The Linear Regression is an indicator that 'predicts' the value of a security's price.
        /// </summary>
        /// <returns></returns>
        public LinReg LinReg(int period)
        {
            return LinReg(Input, period);
        }

        /// <summary>
        /// The Linear Regression is an indicator that 'predicts' the value of a security's price.
        /// </summary>
        /// <returns></returns>
        public LinReg LinReg(Data.IDataSeries input, int period)
        {
            if (cacheLinReg != null)
                for (int idx = 0; idx < cacheLinReg.Length; idx++)
                    if (cacheLinReg[idx].Period == period && cacheLinReg[idx].EqualsInput(input))
                        return cacheLinReg[idx];

            lock (checkLinReg)
            {
                checkLinReg.Period = period;
                period = checkLinReg.Period;

                if (cacheLinReg != null)
                    for (int idx = 0; idx < cacheLinReg.Length; idx++)
                        if (cacheLinReg[idx].Period == period && cacheLinReg[idx].EqualsInput(input))
                            return cacheLinReg[idx];

                LinReg indicator = new LinReg();
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

                LinReg[] tmp = new LinReg[cacheLinReg == null ? 1 : cacheLinReg.Length + 1];
                if (cacheLinReg != null)
                    cacheLinReg.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheLinReg = tmp;
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
        /// The Linear Regression is an indicator that 'predicts' the value of a security's price.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.LinReg LinReg(int period)
        {
            return _indicator.LinReg(Input, period);
        }

        /// <summary>
        /// The Linear Regression is an indicator that 'predicts' the value of a security's price.
        /// </summary>
        /// <returns></returns>
        public Indicator.LinReg LinReg(Data.IDataSeries input, int period)
        {
            return _indicator.LinReg(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Linear Regression is an indicator that 'predicts' the value of a security's price.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.LinReg LinReg(int period)
        {
            return _indicator.LinReg(Input, period);
        }

        /// <summary>
        /// The Linear Regression is an indicator that 'predicts' the value of a security's price.
        /// </summary>
        /// <returns></returns>
        public Indicator.LinReg LinReg(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.LinReg(input, period);
        }
    }
}
#endregion
