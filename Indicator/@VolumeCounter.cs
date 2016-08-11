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
	/// The Volume Counter displays the current volume count of a bar.
	/// </summary>
	[Description("Displays the volume count of a bar.")]
	public class VolumeCounter : Indicator
	{
		#region Variables
		private bool			countDown		= true;
		private string			errorText		= "Volume Counter only works on volume based intervals";
		private bool			showPercent		= false;
		private StringFormat	stringFormat	= new StringFormat();
		private SolidBrush		textBrush		= new SolidBrush(Color.Black);
		private Font			textFont		= new Font("Arial", 30);
		private float			textWidth		= 0;
		private float			textHeight		= 0;
		private float			noTickTextWidth	= 0;
		private float			noTickTextHeight= 0;
		private double			volume			= 0;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			ChartOnly			= true;
			Overlay				= true;
			CalculateOnBarClose = false;
		}

		/// <summary>
		/// </summary>
		protected override void OnBarUpdate() { volume = Volume[0]; }

        #region Miscellaneous
		/// <summary>
        /// Overload this method to handle the termination of an indicator. Use this method to dispose of any resources vs overloading the Dispose() method.
		/// </summary>
		protected override void OnTermination()
		{
			textBrush.Dispose();
            textFont.Dispose();
			stringFormat.Dispose();
		}

		/// <summary>
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="bounds"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{
			if (Bars == null)
				return;

			// Recalculate the proper string size should the chart control object font and axis color change
			if (textBrush.Color != ChartControl.AxisColor || textFont != ChartControl.Font)
			{
				textBrush.Color = ChartControl.AxisColor;
				textFont = (Font) ChartControl.Font.Clone();

				SizeF size = graphics.MeasureString((CountDown ? "Volume remaining = %" : "Volume count = %") + Bars.Period.Value, textFont);
				textWidth		= size.Width + 5;
				textHeight		= size.Height + 5;

				SizeF noTickSize = graphics.MeasureString(errorText, textFont);
				noTickTextWidth = noTickSize.Width + 5;
				noTickTextHeight = noTickSize.Height + 5;
			}

			// Plot the volume count message to the lower right hand corner of the chart
			if (Bars.Period.Id == PeriodType.Volume)
			{
				double volumeCount = ShowPercent ? CountDown ? (1 - Bars.PercentComplete) * 100 : Bars.PercentComplete * 100 : CountDown ? Bars.Period.Value - volume : volume;
				graphics.DrawString((CountDown ? " Volume remaining = " + volumeCount : "Volume = " + volumeCount) + (ShowPercent ? "%" : ""), ChartControl.Font, textBrush, bounds.X + bounds.Width - textWidth, bounds.Y + bounds.Height - textHeight, stringFormat);
			}
			else
				graphics.DrawString(errorText, ChartControl.Font, textBrush, bounds.X + bounds.Width - noTickTextWidth, bounds.Y + bounds.Height - noTickTextHeight, stringFormat);
		}
        #endregion

		#region Properties
		/// <summary>
		/// </summary>
		[Gui.Design.VisualizationOnly()]
		[Description("Indicates if the indicator displays remaining volume or current volume of a bar.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayName("Count down")]
		public bool CountDown
		{
			get { return countDown; }
			set { countDown = value; }
		}

		[Gui.Design.VisualizationOnly()]
		[Description("Shows a percent value instead of absolute value.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayName("Show percent")]
		public bool ShowPercent
		{
			get { return showPercent; }
			set { showPercent = value; }
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
        private VolumeCounter[] cacheVolumeCounter = null;

        private static VolumeCounter checkVolumeCounter = new VolumeCounter();

        /// <summary>
        /// Displays the volume count of a bar.
        /// </summary>
        /// <returns></returns>
        public VolumeCounter VolumeCounter(bool countDown, bool showPercent)
        {
            return VolumeCounter(Input, countDown, showPercent);
        }

        /// <summary>
        /// Displays the volume count of a bar.
        /// </summary>
        /// <returns></returns>
        public VolumeCounter VolumeCounter(Data.IDataSeries input, bool countDown, bool showPercent)
        {
            if (cacheVolumeCounter != null)
                for (int idx = 0; idx < cacheVolumeCounter.Length; idx++)
                    if (cacheVolumeCounter[idx].CountDown == countDown && cacheVolumeCounter[idx].ShowPercent == showPercent && cacheVolumeCounter[idx].EqualsInput(input))
                        return cacheVolumeCounter[idx];

            lock (checkVolumeCounter)
            {
                checkVolumeCounter.CountDown = countDown;
                countDown = checkVolumeCounter.CountDown;
                checkVolumeCounter.ShowPercent = showPercent;
                showPercent = checkVolumeCounter.ShowPercent;

                if (cacheVolumeCounter != null)
                    for (int idx = 0; idx < cacheVolumeCounter.Length; idx++)
                        if (cacheVolumeCounter[idx].CountDown == countDown && cacheVolumeCounter[idx].ShowPercent == showPercent && cacheVolumeCounter[idx].EqualsInput(input))
                            return cacheVolumeCounter[idx];

                VolumeCounter indicator = new VolumeCounter();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.CountDown = countDown;
                indicator.ShowPercent = showPercent;
                Indicators.Add(indicator);
                indicator.SetUp();

                VolumeCounter[] tmp = new VolumeCounter[cacheVolumeCounter == null ? 1 : cacheVolumeCounter.Length + 1];
                if (cacheVolumeCounter != null)
                    cacheVolumeCounter.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheVolumeCounter = tmp;
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
        /// Displays the volume count of a bar.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolumeCounter VolumeCounter(bool countDown, bool showPercent)
        {
            return _indicator.VolumeCounter(Input, countDown, showPercent);
        }

        /// <summary>
        /// Displays the volume count of a bar.
        /// </summary>
        /// <returns></returns>
        public Indicator.VolumeCounter VolumeCounter(Data.IDataSeries input, bool countDown, bool showPercent)
        {
            return _indicator.VolumeCounter(input, countDown, showPercent);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Displays the volume count of a bar.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolumeCounter VolumeCounter(bool countDown, bool showPercent)
        {
            return _indicator.VolumeCounter(Input, countDown, showPercent);
        }

        /// <summary>
        /// Displays the volume count of a bar.
        /// </summary>
        /// <returns></returns>
        public Indicator.VolumeCounter VolumeCounter(Data.IDataSeries input, bool countDown, bool showPercent)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.VolumeCounter(input, countDown, showPercent);
        }
    }
}
#endregion
