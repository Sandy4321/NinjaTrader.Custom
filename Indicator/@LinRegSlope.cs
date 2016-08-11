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
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// Linear Regression Slope
	/// </summary>
	[Description("Linear Regression Slope")]
	public class LinRegSlope : Indicator
	{
		#region Variables
		private int					period	= 14;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Orange, "LinRegSlope"));
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
				
			Value.Set(((double) Period * sumXY - sumX * SUM(Inputs[0], Period)[0]) / divisor);
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
        private LinRegSlope[] cacheLinRegSlope = null;

        private static LinRegSlope checkLinRegSlope = new LinRegSlope();

        /// <summary>
        /// Linear Regression Slope
        /// </summary>
        /// <returns></returns>
        public LinRegSlope LinRegSlope(int period)
        {
            return LinRegSlope(Input, period);
        }

        /// <summary>
        /// Linear Regression Slope
        /// </summary>
        /// <returns></returns>
        public LinRegSlope LinRegSlope(Data.IDataSeries input, int period)
        {
            if (cacheLinRegSlope != null)
                for (int idx = 0; idx < cacheLinRegSlope.Length; idx++)
                    if (cacheLinRegSlope[idx].Period == period && cacheLinRegSlope[idx].EqualsInput(input))
                        return cacheLinRegSlope[idx];

            lock (checkLinRegSlope)
            {
                checkLinRegSlope.Period = period;
                period = checkLinRegSlope.Period;

                if (cacheLinRegSlope != null)
                    for (int idx = 0; idx < cacheLinRegSlope.Length; idx++)
                        if (cacheLinRegSlope[idx].Period == period && cacheLinRegSlope[idx].EqualsInput(input))
                            return cacheLinRegSlope[idx];

                LinRegSlope indicator = new LinRegSlope();
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

                LinRegSlope[] tmp = new LinRegSlope[cacheLinRegSlope == null ? 1 : cacheLinRegSlope.Length + 1];
                if (cacheLinRegSlope != null)
                    cacheLinRegSlope.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheLinRegSlope = tmp;
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
        /// Linear Regression Slope
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.LinRegSlope LinRegSlope(int period)
        {
            return _indicator.LinRegSlope(Input, period);
        }

        /// <summary>
        /// Linear Regression Slope
        /// </summary>
        /// <returns></returns>
        public Indicator.LinRegSlope LinRegSlope(Data.IDataSeries input, int period)
        {
            return _indicator.LinRegSlope(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Linear Regression Slope
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.LinRegSlope LinRegSlope(int period)
        {
            return _indicator.LinRegSlope(Input, period);
        }

        /// <summary>
        /// Linear Regression Slope
        /// </summary>
        /// <returns></returns>
        public Indicator.LinRegSlope LinRegSlope(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.LinRegSlope(input, period);
        }
    }
}
#endregion
