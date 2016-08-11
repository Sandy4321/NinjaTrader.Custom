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
	/// Linnear Regression Intercept
	/// </summary>
	[Description("Linnear Regression Intercept")]
	public class LinRegIntercept : Indicator
	{
		#region Variables
		private int					period	= 14;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Orange, "LinRegIntercept"));
			Overlay = true;
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
			
			double	slope		= ((double) Period * sumXY - sumX * SUM(Inputs[0], Period)[0]) / divisor;
			Value.Set((SUM(Inputs[0], Period)[0] - slope * sumX) / Period);
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
		private LinRegIntercept[] cacheLinRegIntercept = null;

		private static LinRegIntercept checkLinRegIntercept = new LinRegIntercept();

		/// <summary>
		/// Linnear Regression Intercept
		/// </summary>
		/// <returns></returns>
		public LinRegIntercept LinRegIntercept(int period)
		{
			return LinRegIntercept(Input, period);
		}

		/// <summary>
		/// Linnear Regression Intercept
		/// </summary>
		/// <returns></returns>
		public LinRegIntercept LinRegIntercept(Data.IDataSeries input, int period)
		{
			if (cacheLinRegIntercept != null)
				for (int idx = 0; idx < cacheLinRegIntercept.Length; idx++)
					if (cacheLinRegIntercept[idx].Period == period && cacheLinRegIntercept[idx].EqualsInput(input))
						return cacheLinRegIntercept[idx];

			lock (checkLinRegIntercept)
			{
				checkLinRegIntercept.Period = period;
				period = checkLinRegIntercept.Period;

				if (cacheLinRegIntercept != null)
					for (int idx = 0; idx < cacheLinRegIntercept.Length; idx++)
						if (cacheLinRegIntercept[idx].Period == period && cacheLinRegIntercept[idx].EqualsInput(input))
							return cacheLinRegIntercept[idx];

				LinRegIntercept indicator = new LinRegIntercept();
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

				LinRegIntercept[] tmp = new LinRegIntercept[cacheLinRegIntercept == null ? 1 : cacheLinRegIntercept.Length + 1];
				if (cacheLinRegIntercept != null)
					cacheLinRegIntercept.CopyTo(tmp, 0);
				tmp[tmp.Length - 1] = indicator;
				cacheLinRegIntercept = tmp;
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
		/// Linnear Regression Intercept
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.LinRegIntercept LinRegIntercept(int period)
		{
			return _indicator.LinRegIntercept(Input, period);
		}

		/// <summary>
		/// Linnear Regression Intercept
		/// </summary>
		/// <returns></returns>
		public Indicator.LinRegIntercept LinRegIntercept(Data.IDataSeries input, int period)
		{
			return _indicator.LinRegIntercept(input, period);
		}
	}
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
	public partial class Strategy : StrategyBase
	{
		/// <summary>
		/// Linnear Regression Intercept
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.LinRegIntercept LinRegIntercept(int period)
		{
			return _indicator.LinRegIntercept(Input, period);
		}

		/// <summary>
		/// Linnear Regression Intercept
		/// </summary>
		/// <returns></returns>
		public Indicator.LinRegIntercept LinRegIntercept(Data.IDataSeries input, int period)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return _indicator.LinRegIntercept(input, period);
		}
	}
}
#endregion
