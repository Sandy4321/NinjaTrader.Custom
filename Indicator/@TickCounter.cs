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
	/// The Tick Counter displays the current tick count of a bar.
	/// </summary>
	[Description("Displays the tick count of a bar.")]
	public class TickCounter : Indicator
	{
		#region Variables
		private bool			countDown		= true;
		private string			errorTick		= "Tick Counter only works on bars built with a set number of ticks";
		private bool			showPercent		= false;
		private StringFormat	stringFormat	= new StringFormat();
		private SolidBrush		textBrush		= new SolidBrush(Color.Black);
		private Font			textFont		= new Font("Arial", 30);
		private float			textWidth		= 0;
		private float			textHeight		= 0;
		private float noTickTextWidth = 0;
		private float noTickTextHeight = 0;
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
		protected override void OnBarUpdate(){}

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

				SizeF size = graphics.MeasureString((CountDown ? "Ticks remaining = %" : "Tick count = %") + Bars.Period.Value, textFont);
				textWidth		= size.Width + 5;
				textHeight		= size.Height + 5;

				SizeF noTickSize = graphics.MeasureString(errorTick, textFont);
				noTickTextWidth = noTickSize.Width + 5;
				noTickTextHeight = noTickSize.Height + 5;
			}

			// Plot the tick count message to the lower right hand corner of the chart
			if (Bars.Period.Id == PeriodType.Tick || (Bars.Period.BasePeriodType == PeriodType.Tick 
				&& Bars.Period.Id != PeriodType.PointAndFigure && Bars.Period.Id != PeriodType.Kagi && Bars.Period.Id != PeriodType.LineBreak))
			{
				int		periodValue	= (Bars.Period.Id == PeriodType.Tick) ? Bars.Period.Value : Bars.Period.BasePeriodValue;
				double	tickCount	= ShowPercent ? CountDown ? (1 - Bars.PercentComplete) * 100 : Bars.PercentComplete * 100 : CountDown ? periodValue - Bars.TickCount : Bars.TickCount;
				graphics.DrawString((CountDown ? " Ticks remaining = " + tickCount : "Tick count = " + tickCount) + (ShowPercent ? "%" : ""), ChartControl.Font, textBrush, bounds.X + bounds.Width - textWidth, bounds.Y + bounds.Height - textHeight, stringFormat);
			}
			else
				graphics.DrawString(errorTick, ChartControl.Font, textBrush, bounds.X + bounds.Width - noTickTextWidth, bounds.Y + bounds.Height - noTickTextHeight, stringFormat);
		}
        #endregion

		#region Properties
		/// <summary>
		/// </summary>
		[Gui.Design.VisualizationOnly()]
		[Description("Indicates if the indicator displays remaining ticks or current ticks of a bar.")]
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
        private TickCounter[] cacheTickCounter = null;

        private static TickCounter checkTickCounter = new TickCounter();

        /// <summary>
        /// Displays the tick count of a bar.
        /// </summary>
        /// <returns></returns>
        public TickCounter TickCounter(bool countDown, bool showPercent)
        {
            return TickCounter(Input, countDown, showPercent);
        }

        /// <summary>
        /// Displays the tick count of a bar.
        /// </summary>
        /// <returns></returns>
        public TickCounter TickCounter(Data.IDataSeries input, bool countDown, bool showPercent)
        {
            if (cacheTickCounter != null)
                for (int idx = 0; idx < cacheTickCounter.Length; idx++)
                    if (cacheTickCounter[idx].CountDown == countDown && cacheTickCounter[idx].ShowPercent == showPercent && cacheTickCounter[idx].EqualsInput(input))
                        return cacheTickCounter[idx];

            lock (checkTickCounter)
            {
                checkTickCounter.CountDown = countDown;
                countDown = checkTickCounter.CountDown;
                checkTickCounter.ShowPercent = showPercent;
                showPercent = checkTickCounter.ShowPercent;

                if (cacheTickCounter != null)
                    for (int idx = 0; idx < cacheTickCounter.Length; idx++)
                        if (cacheTickCounter[idx].CountDown == countDown && cacheTickCounter[idx].ShowPercent == showPercent && cacheTickCounter[idx].EqualsInput(input))
                            return cacheTickCounter[idx];

                TickCounter indicator = new TickCounter();
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

                TickCounter[] tmp = new TickCounter[cacheTickCounter == null ? 1 : cacheTickCounter.Length + 1];
                if (cacheTickCounter != null)
                    cacheTickCounter.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTickCounter = tmp;
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
        /// Displays the tick count of a bar.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TickCounter TickCounter(bool countDown, bool showPercent)
        {
            return _indicator.TickCounter(Input, countDown, showPercent);
        }

        /// <summary>
        /// Displays the tick count of a bar.
        /// </summary>
        /// <returns></returns>
        public Indicator.TickCounter TickCounter(Data.IDataSeries input, bool countDown, bool showPercent)
        {
            return _indicator.TickCounter(input, countDown, showPercent);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Displays the tick count of a bar.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TickCounter TickCounter(bool countDown, bool showPercent)
        {
            return _indicator.TickCounter(Input, countDown, showPercent);
        }

        /// <summary>
        /// Displays the tick count of a bar.
        /// </summary>
        /// <returns></returns>
        public Indicator.TickCounter TickCounter(Data.IDataSeries input, bool countDown, bool showPercent)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TickCounter(input, countDown, showPercent);
        }
    }
}
#endregion
