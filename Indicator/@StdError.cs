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
	/// Standard Error shows how near prices go around a linear regression line.
	/// </summary>
	[Description("Standard Error shows how near prices go around a linear regression line.  The closer the prices are to the linear regression line, the stronger is the trend.")]
	public class StdError : Indicator
	{
		// Documentation of Linear Regression: http://en.wikipedia.org/wiki/Linear_regression
		// Documentation of Standard Error: http://tadoc.org/indicator/STDERR.htm 

		#region Variables
		private int					period	= 14;
		private	DataSeries		y;
		#endregion


		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Orange, "LinReg"));
            Add(new Plot(Color.DarkViolet, "Upper"));
			Add(new Plot(Color.DarkViolet, "Lower"));
	
			y					= new DataSeries(this);
			Overlay				= true;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			// calculate Linear Regression
			double	sumX	= (double) Period * (Period - 1) * 0.5;
			double	divisor = sumX * sumX - (double) Period * Period * (Period - 1) * (2 * Period - 1) / 6;
			double	sumXY	= 0;

			for (int count = 0; count < Period && CurrentBar - count >= 0; count++)
				sumXY += count * Input[count];
			
			y.Set(Input[0]);
			double	slope		= ((double) Period * sumXY - sumX * SUM(y, Period)[0]) / divisor;
			double	intercept	= (SUM(y, Period)[0] - slope * sumX) / Period;
			double	linReg		= intercept + slope * (Period - 1);
			
			// Calculate Standard Error
			double sumSquares = 0;
			for (int count = 0; count < Period && CurrentBar - count >= 0; count++)
			{
				double linRegX = intercept + slope * (Period - 1 - count);
				double valueX = Input[count];
				double diff = Math.Abs(valueX - linRegX);

				sumSquares += diff * diff;
			}
			double stdErr = Math.Sqrt(sumSquares / Period);

			Middle.Set(linReg);
			Upper.Set(linReg + stdErr);
			Lower.Set(linReg - stdErr);
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

		/// <summary>
		/// Lower band of Standard Error
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Data.DataSeries Lower
		{
			get { return Values[2]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Data.DataSeries Middle
		{
			get { return Values[0]; }
		}

		/// <summary>
		/// Upper band of Standard Error
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Data.DataSeries Upper
		{
			get { return Values[1]; }
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
        private StdError[] cacheStdError = null;

        private static StdError checkStdError = new StdError();

        /// <summary>
        /// Standard Error shows how near prices go around a linear regression line.  The closer the prices are to the linear regression line, the stronger is the trend.
        /// </summary>
        /// <returns></returns>
        public StdError StdError(int period)
        {
            return StdError(Input, period);
        }

        /// <summary>
        /// Standard Error shows how near prices go around a linear regression line.  The closer the prices are to the linear regression line, the stronger is the trend.
        /// </summary>
        /// <returns></returns>
        public StdError StdError(Data.IDataSeries input, int period)
        {
            if (cacheStdError != null)
                for (int idx = 0; idx < cacheStdError.Length; idx++)
                    if (cacheStdError[idx].Period == period && cacheStdError[idx].EqualsInput(input))
                        return cacheStdError[idx];

            lock (checkStdError)
            {
                checkStdError.Period = period;
                period = checkStdError.Period;

                if (cacheStdError != null)
                    for (int idx = 0; idx < cacheStdError.Length; idx++)
                        if (cacheStdError[idx].Period == period && cacheStdError[idx].EqualsInput(input))
                            return cacheStdError[idx];

                StdError indicator = new StdError();
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

                StdError[] tmp = new StdError[cacheStdError == null ? 1 : cacheStdError.Length + 1];
                if (cacheStdError != null)
                    cacheStdError.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheStdError = tmp;
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
        /// Standard Error shows how near prices go around a linear regression line.  The closer the prices are to the linear regression line, the stronger is the trend.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.StdError StdError(int period)
        {
            return _indicator.StdError(Input, period);
        }

        /// <summary>
        /// Standard Error shows how near prices go around a linear regression line.  The closer the prices are to the linear regression line, the stronger is the trend.
        /// </summary>
        /// <returns></returns>
        public Indicator.StdError StdError(Data.IDataSeries input, int period)
        {
            return _indicator.StdError(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Standard Error shows how near prices go around a linear regression line.  The closer the prices are to the linear regression line, the stronger is the trend.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.StdError StdError(int period)
        {
            return _indicator.StdError(Input, period);
        }

        /// <summary>
        /// Standard Error shows how near prices go around a linear regression line.  The closer the prices are to the linear regression line, the stronger is the trend.
        /// </summary>
        /// <returns></returns>
        public Indicator.StdError StdError(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.StdError(input, period);
        }
    }
}
#endregion
