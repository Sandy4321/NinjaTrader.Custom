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
    /// This is a sample for an indicator which with custom plotting.
    /// </summary>
    [Description("This is a sample for an indicator using custom plotting.")]
    public class CustomPlotSample : Indicator
    {
        #region Variables
        // Wizard generated variables
        // User defined variables (add any user defined variables below)
		private StringFormat			stringFormat	= new StringFormat();
		private	SolidBrush				textBrush		= new SolidBrush(Color.Black);
		private	System.Drawing.Font		textFont		= new Font("Arial", 10);
		#endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			ChartOnly			= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
        }

        #region Miscellaneous
		/// <summary>
        /// Overload this method to handle the termination of an indicator. Use this method to dispose of any resources vs overloading the Dispose() method.
		/// </summary>
		protected override void OnTermination()
		{
			textBrush.Dispose();
			stringFormat.Dispose();
		}

		/// <summary>
		/// Called when the indicator is plotted.
		/// </summary>
		/// <param name="graphics">Graphics context</param>
		/// <param name="bounds">Paintable chart region</param>
		/// <param name="min">Min indicator value plotted</param>
		/// <param name="max">Max indicator value plotted</param>
		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{
			// Default plotting in base class. Uncomment if indicators holds at least one plot.
			// base.Plot(graphics, bounds, min, max);

			// fill the complete paintable region
			// note: although the rectangle is greater and even would start plotting at the left upper corner of the chart, 
			// it's clipped to the paintable region
			// the qualification 'InHitTest' is used to exclude the drawn rectangle from the mouse selection action
			SolidBrush tmpBrush = new SolidBrush(Color.LightGray);
			if (!InHitTest)
				graphics.FillRectangle(tmpBrush, new Rectangle (0, 0, 2000, 2000));
			tmpBrush.Dispose();

			// plot a green line from the upper left to the lower right corner
			// all painting needs to go by bounds X/Y offset
			Pen tmpPen = new Pen(Color.Green);
			graphics.DrawLine(tmpPen, bounds.X, bounds.Y, bounds.X + bounds.Width, bounds.Y + bounds.Height);

			// plot a green line from the lower left to the upper right corner and apply 
			// apply AnitAlias to make the line look nicer
			// don't forget to reset after plotting -> smoothing has some performance impact
			SmoothingMode oldSmoothingMode = graphics.SmoothingMode;		// save current smoothing mode
			graphics.SmoothingMode = SmoothingMode.AntiAlias;				// apply smoothing mode
			graphics.DrawLine(new Pen(Color.Green), bounds.X, bounds.Y + bounds.Height, bounds.X + bounds.Width, bounds.Y);
			graphics.SmoothingMode = oldSmoothingMode;						// restore smoothing mode
			tmpPen.Dispose();

			// plot text in the upper left corner at position 10/10
			stringFormat.Alignment	= StringAlignment.Near;					// text is docked to the left
			textBrush.Color			= Color.Blue;							// set text color
			graphics.DrawString("Upper left corner", textFont, textBrush, bounds.X + 10, bounds.Y + 10, stringFormat);
			
			// paint text at the lower left corner right to the bottom on background with an outline
			// 1) plot background rectangle
			tmpBrush = new SolidBrush(Color.Red);
			graphics.FillRectangle(tmpBrush, bounds.X + 10, bounds.Y + bounds.Height - 20, 140, 19);
			tmpBrush.Dispose();
			// 2) plot outline
			tmpPen = new Pen(Color.Black);
			graphics.DrawRectangle(tmpPen, bounds.X + 10, bounds.Y + bounds.Height - 20, 140, 19);
			tmpPen.Dispose();
			// 3) plot text
			graphics.DrawString("Lower left corner", textFont, textBrush, bounds.X + 10, bounds.Y + bounds.Height - 20, stringFormat);
		}

		#endregion

        #region Properties
        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private CustomPlotSample[] cacheCustomPlotSample = null;

        private static CustomPlotSample checkCustomPlotSample = new CustomPlotSample();

        /// <summary>
        /// This is a sample for an indicator using custom plotting.
        /// </summary>
        /// <returns></returns>
        public CustomPlotSample CustomPlotSample()
        {
            return CustomPlotSample(Input);
        }

        /// <summary>
        /// This is a sample for an indicator using custom plotting.
        /// </summary>
        /// <returns></returns>
        public CustomPlotSample CustomPlotSample(Data.IDataSeries input)
        {
            if (cacheCustomPlotSample != null)
                for (int idx = 0; idx < cacheCustomPlotSample.Length; idx++)
                    if (cacheCustomPlotSample[idx].EqualsInput(input))
                        return cacheCustomPlotSample[idx];

            lock (checkCustomPlotSample)
            {
                if (cacheCustomPlotSample != null)
                    for (int idx = 0; idx < cacheCustomPlotSample.Length; idx++)
                        if (cacheCustomPlotSample[idx].EqualsInput(input))
                            return cacheCustomPlotSample[idx];

                CustomPlotSample indicator = new CustomPlotSample();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                CustomPlotSample[] tmp = new CustomPlotSample[cacheCustomPlotSample == null ? 1 : cacheCustomPlotSample.Length + 1];
                if (cacheCustomPlotSample != null)
                    cacheCustomPlotSample.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheCustomPlotSample = tmp;
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
        /// This is a sample for an indicator using custom plotting.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.CustomPlotSample CustomPlotSample()
        {
            return _indicator.CustomPlotSample(Input);
        }

        /// <summary>
        /// This is a sample for an indicator using custom plotting.
        /// </summary>
        /// <returns></returns>
        public Indicator.CustomPlotSample CustomPlotSample(Data.IDataSeries input)
        {
            return _indicator.CustomPlotSample(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// This is a sample for an indicator using custom plotting.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.CustomPlotSample CustomPlotSample()
        {
            return _indicator.CustomPlotSample(Input);
        }

        /// <summary>
        /// This is a sample for an indicator using custom plotting.
        /// </summary>
        /// <returns></returns>
        public Indicator.CustomPlotSample CustomPlotSample(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.CustomPlotSample(input);
        }
    }
}
#endregion
