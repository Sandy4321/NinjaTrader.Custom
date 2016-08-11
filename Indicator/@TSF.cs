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
	/// The TSF (Time Series Forecast) calculates probable future values for the price by fitting a linear regression line over a given number of price bars and following that line forward into the future. A linear regression line is a straight line which is as close to all of the given price points as possible. Also see the Linear Regression indicator.
	/// </summary>
	[Description("The TSF (Time Series Forecast) calculates probable future values for the price by fitting a linear regression line over a given number of price bars and following that line forward into the future. A linear regression line is a straight line which is as close to all of the given price points as possible. Also see the Linear Regression indicator.")]
	public class TSF : Indicator
	{
		#region Variables
		private int					period		= 14;
		private int					forecast	= 3;
		private	DataSeries		y;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Orange, "TSF"));
	
			y					= new DataSeries(this);
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
			
			y.Set(Input[0]);
			double	slope		= ((double) Period * sumXY - sumX * SUM(y, Period)[0]) / divisor;
			double	intercept	= (SUM(y, Period)[0] - slope * sumX) / Period;
			
			Value.Set(intercept + slope * ((Period - 1) + forecast));
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("The forecast period. Positive values follow the linear regression line forward from the current point, negative values goes backwards. A value of zero will make this indicator plot excactly like the Linear Regression indicator.")]
		[GridCategory("Parameters")]
		public int Forecast
		{
			get { return forecast; }
			set { forecast = Math.Max(-10, value); }
		}

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
        private TSF[] cacheTSF = null;

        private static TSF checkTSF = new TSF();

        /// <summary>
        /// The TSF (Time Series Forecast) calculates probable future values for the price by fitting a linear regression line over a given number of price bars and following that line forward into the future. A linear regression line is a straight line which is as close to all of the given price points as possible. Also see the Linear Regression indicator.
        /// </summary>
        /// <returns></returns>
        public TSF TSF(int forecast, int period)
        {
            return TSF(Input, forecast, period);
        }

        /// <summary>
        /// The TSF (Time Series Forecast) calculates probable future values for the price by fitting a linear regression line over a given number of price bars and following that line forward into the future. A linear regression line is a straight line which is as close to all of the given price points as possible. Also see the Linear Regression indicator.
        /// </summary>
        /// <returns></returns>
        public TSF TSF(Data.IDataSeries input, int forecast, int period)
        {
            if (cacheTSF != null)
                for (int idx = 0; idx < cacheTSF.Length; idx++)
                    if (cacheTSF[idx].Forecast == forecast && cacheTSF[idx].Period == period && cacheTSF[idx].EqualsInput(input))
                        return cacheTSF[idx];

            lock (checkTSF)
            {
                checkTSF.Forecast = forecast;
                forecast = checkTSF.Forecast;
                checkTSF.Period = period;
                period = checkTSF.Period;

                if (cacheTSF != null)
                    for (int idx = 0; idx < cacheTSF.Length; idx++)
                        if (cacheTSF[idx].Forecast == forecast && cacheTSF[idx].Period == period && cacheTSF[idx].EqualsInput(input))
                            return cacheTSF[idx];

                TSF indicator = new TSF();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Forecast = forecast;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                TSF[] tmp = new TSF[cacheTSF == null ? 1 : cacheTSF.Length + 1];
                if (cacheTSF != null)
                    cacheTSF.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTSF = tmp;
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
        /// The TSF (Time Series Forecast) calculates probable future values for the price by fitting a linear regression line over a given number of price bars and following that line forward into the future. A linear regression line is a straight line which is as close to all of the given price points as possible. Also see the Linear Regression indicator.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TSF TSF(int forecast, int period)
        {
            return _indicator.TSF(Input, forecast, period);
        }

        /// <summary>
        /// The TSF (Time Series Forecast) calculates probable future values for the price by fitting a linear regression line over a given number of price bars and following that line forward into the future. A linear regression line is a straight line which is as close to all of the given price points as possible. Also see the Linear Regression indicator.
        /// </summary>
        /// <returns></returns>
        public Indicator.TSF TSF(Data.IDataSeries input, int forecast, int period)
        {
            return _indicator.TSF(input, forecast, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The TSF (Time Series Forecast) calculates probable future values for the price by fitting a linear regression line over a given number of price bars and following that line forward into the future. A linear regression line is a straight line which is as close to all of the given price points as possible. Also see the Linear Regression indicator.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TSF TSF(int forecast, int period)
        {
            return _indicator.TSF(Input, forecast, period);
        }

        /// <summary>
        /// The TSF (Time Series Forecast) calculates probable future values for the price by fitting a linear regression line over a given number of price bars and following that line forward into the future. A linear regression line is a straight line which is as close to all of the given price points as possible. Also see the Linear Regression indicator.
        /// </summary>
        /// <returns></returns>
        public Indicator.TSF TSF(Data.IDataSeries input, int forecast, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TSF(input, forecast, period);
        }
    }
}
#endregion
