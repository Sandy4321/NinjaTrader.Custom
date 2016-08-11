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
	/// Linear regression is used to calculate a best fit line for the price data. In addition an upper and lower band is added by calculating the standard deviation of prices from the regression line.
	/// </summary>
	[Description("Linear regression is used to calculate a best fit line for the price data. In addition an upper and lower band is added by calculating the standard deviation of prices from the regression line.")]
	public class RegressionChannel : Indicator
	{
		#region Variables
		private int				period	= 35;
		private double			width	= 2;

		private DataSeries		interceptSeries;
		private DataSeries		slopeSeries;
		private DataSeries		stdDeviationSeries;
		private	DataSeries		y;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Gray,	"Middle"));
			Add(new Plot(Color.Blue,	"Upper"));
			Add(new Plot(Color.Blue,	"Lower"));

			interceptSeries		= new DataSeries(this);
			slopeSeries			= new DataSeries(this);
			stdDeviationSeries	= new DataSeries(this);
			y					= new DataSeries(this);

			Overlay				= true;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
				return;

			// First we calculate the linear regression parameters

			double sumX	= (double) Period * (Period - 1) * 0.5;
			double divisor = sumX * sumX - 
				(double) Period * Period * (Period - 1) * (2 * Period - 1) / 6;
			double sumXY = 0;
			double sumY  = 0;

			int barCount = Math.Min(Period, CurrentBar);

			for (int count = 0; count < barCount; count++)
			{
				sumXY += count * Input[count];
				sumY  += Input[count];
			}

			if (divisor == 0 || Period == 0)
				return;

			double	slope	  = ((double) Period * sumXY - sumX * sumY) / divisor;
			double	intercept = (sumY - slope * sumX) / Period;

			slopeSeries.Set(slope);
			interceptSeries.Set(intercept);

			// Next we calculate the standard deviation of the 
			// residuals (vertical distances to the regression line).

			double sumResiduals = 0;

			for (int count = 0; count < barCount; count++) 
			{
				double regressionValue = intercept + slope * (Period - 1 - count);
				double residual = Math.Abs(Input[count] - regressionValue);

				sumResiduals += residual;
			}

			double avgResiduals = sumResiduals / Math.Min(CurrentBar - 1, Period);

			sumResiduals = 0;
			for (int count = 0; count < barCount; count++)
			{
				double regressionValue = intercept + slope * (Period - 1 - count);
				double residual        = Math.Abs(Input[count] - regressionValue);

				sumResiduals += (residual - avgResiduals) * (residual - avgResiduals);
			}

			double stdDeviation = Math.Sqrt(sumResiduals / Math.Min(CurrentBar + 1, Period));
			stdDeviationSeries.Set(stdDeviation);

  			double middle = intercept + slope * (Period - 1);
  			Middle.Set(middle);
  			Upper.Set(middle + stdDeviation * Width);
  			Lower.Set(middle - stdDeviation * Width);
		}

		#region Properties

 		/// <summary>
 		/// </summary>
		[Browsable(false)]
 		[XmlIgnore]
 		public DataSeries Lower
 		{
 			get { return Values[2]; }
 		}

 		/// <summary>
 		/// </summary>
 		[Browsable(false)]
 		[XmlIgnore]
 		public DataSeries Middle
 		{
 			get { return Values[0]; }
 		}

		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculations.")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(2, value); }
		}

 		/// <summary>
 		/// </summary>
 		[Browsable(false)]
 		[XmlIgnore]
 		public DataSeries Upper
 		{
 			get { return Values[1]; }
 		}

		/// <summary>
		/// </summary>
		[Description("Number of standard deviations to use for the width of the channel.")]
		[GridCategory("Parameters")]
		public double Width 
		{
			get { return width; }
			set { width = Math.Max(1, value); }
		}
		#endregion

		#region Miscellaneous
		private int GetXPos(int barsBack)
		{
			return ChartControl.GetXByBarIdx(BarsArray[0], Math.Max(0, Bars.Count - 1 - barsBack - (CalculateOnBarClose ? 1 : 0)));
		}

		private int GetYPos(double price, Rectangle bounds, double min, double max)
		{
			return ChartControl.GetYByValue(this, price);
		}

		/// <summary>
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="bounds"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{
			if (Bars == null || ChartControl == null)
				return;

			int    idx 		 = Input.Count - 1 - (CalculateOnBarClose ? 1 : 0);
			double intercept = interceptSeries.Get(idx);
			double slope 	 = slopeSeries.Get(idx);
			double stdDev	 = stdDeviationSeries.Get(idx);

			int stdDevPixels = (int) Math.Round(((stdDev * width) / ChartControl.MaxMinusMin(max, min)) * bounds.Height, 0);

			int xPos = GetXPos(Period - 1);			
			int yPos = GetYPos(intercept, bounds, min, max);

			int xPos2 = GetXPos(0);
			int yPos2 = GetYPos(intercept + slope * (Period - 1), bounds, min, max);

			SmoothingMode oldSmoothingMode = graphics.SmoothingMode;
			graphics.SmoothingMode = SmoothingMode.AntiAlias;

			// Middle
			graphics.DrawLine(Plots[0].Pen,
				xPos,
				yPos,
				xPos2,
				yPos2);

			// Upper
			graphics.DrawLine(Plots[1].Pen,
				xPos,
				yPos  - stdDevPixels,
				xPos2,
				yPos2 - stdDevPixels);

			// Lower
			graphics.DrawLine(Plots[2].Pen,
				xPos,
				yPos  + stdDevPixels,
				xPos2,
				yPos2 + stdDevPixels);

			graphics.SmoothingMode = oldSmoothingMode;
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
        private RegressionChannel[] cacheRegressionChannel = null;

        private static RegressionChannel checkRegressionChannel = new RegressionChannel();

        /// <summary>
        /// Linear regression is used to calculate a best fit line for the price data. In addition an upper and lower band is added by calculating the standard deviation of prices from the regression line.
        /// </summary>
        /// <returns></returns>
        public RegressionChannel RegressionChannel(int period, double width)
        {
            return RegressionChannel(Input, period, width);
        }

        /// <summary>
        /// Linear regression is used to calculate a best fit line for the price data. In addition an upper and lower band is added by calculating the standard deviation of prices from the regression line.
        /// </summary>
        /// <returns></returns>
        public RegressionChannel RegressionChannel(Data.IDataSeries input, int period, double width)
        {
            if (cacheRegressionChannel != null)
                for (int idx = 0; idx < cacheRegressionChannel.Length; idx++)
                    if (cacheRegressionChannel[idx].Period == period && Math.Abs(cacheRegressionChannel[idx].Width - width) <= double.Epsilon && cacheRegressionChannel[idx].EqualsInput(input))
                        return cacheRegressionChannel[idx];

            lock (checkRegressionChannel)
            {
                checkRegressionChannel.Period = period;
                period = checkRegressionChannel.Period;
                checkRegressionChannel.Width = width;
                width = checkRegressionChannel.Width;

                if (cacheRegressionChannel != null)
                    for (int idx = 0; idx < cacheRegressionChannel.Length; idx++)
                        if (cacheRegressionChannel[idx].Period == period && Math.Abs(cacheRegressionChannel[idx].Width - width) <= double.Epsilon && cacheRegressionChannel[idx].EqualsInput(input))
                            return cacheRegressionChannel[idx];

                RegressionChannel indicator = new RegressionChannel();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                indicator.Width = width;
                Indicators.Add(indicator);
                indicator.SetUp();

                RegressionChannel[] tmp = new RegressionChannel[cacheRegressionChannel == null ? 1 : cacheRegressionChannel.Length + 1];
                if (cacheRegressionChannel != null)
                    cacheRegressionChannel.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheRegressionChannel = tmp;
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
        /// Linear regression is used to calculate a best fit line for the price data. In addition an upper and lower band is added by calculating the standard deviation of prices from the regression line.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.RegressionChannel RegressionChannel(int period, double width)
        {
            return _indicator.RegressionChannel(Input, period, width);
        }

        /// <summary>
        /// Linear regression is used to calculate a best fit line for the price data. In addition an upper and lower band is added by calculating the standard deviation of prices from the regression line.
        /// </summary>
        /// <returns></returns>
        public Indicator.RegressionChannel RegressionChannel(Data.IDataSeries input, int period, double width)
        {
            return _indicator.RegressionChannel(input, period, width);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Linear regression is used to calculate a best fit line for the price data. In addition an upper and lower band is added by calculating the standard deviation of prices from the regression line.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.RegressionChannel RegressionChannel(int period, double width)
        {
            return _indicator.RegressionChannel(Input, period, width);
        }

        /// <summary>
        /// Linear regression is used to calculate a best fit line for the price data. In addition an upper and lower band is added by calculating the standard deviation of prices from the regression line.
        /// </summary>
        /// <returns></returns>
        public Indicator.RegressionChannel RegressionChannel(Data.IDataSeries input, int period, double width)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.RegressionChannel(input, period, width);
        }
    }
}
#endregion
