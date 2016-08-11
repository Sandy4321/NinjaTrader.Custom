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
	[Description("Displays the range countdown of a bar.")]
	public class RangeCounter : Indicator
	{
		#region Variables
		private bool			countDown			= true;
		private bool			isRangeDerivate		= false;
		private bool			rangeDerivateChecked= false;
		private StringFormat	stringFormat		= new StringFormat();
		private SolidBrush		textBrush			= new SolidBrush(Color.Black);
		private Font			textFont			= new Font("Arial", 30);
		private float			textWidth			= 0;
		private float			textHeight			= 0;
		private string			noRangeMessage		= "Range Counter only works on Range bars.";
		private float			noRangeTextWidth	= 0;
		private float			noRangeTextHeight	= 0;
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
		protected override void OnBarUpdate()
		{
			if (rangeDerivateChecked)
				return;
			if (BarsArray == null || BarsArray.Length == 0)
				return;

			if (BarsArray[0].BarsType.BuiltFrom == Data.PeriodType.Tick && BarsArray[0].Period.ToString().IndexOf("Range") >= 0)
				isRangeDerivate = true;
			rangeDerivateChecked = true;
		}

        #region Miscellaneous
		/// <summary>
        /// Overload this method to handle the termination of an indicator. Use this method to dispose of any resources vs overloading the Dispose() method.
		/// </summary>
		protected override void OnTermination()
		{
			textFont.Dispose();
			textBrush.Dispose();
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

				SizeF size = graphics.MeasureString((CountDown ? "Range remaining = %" : "Range count = %") + Bars.Period.Value, textFont);
				textWidth		= size.Width + 5;
				textHeight		= size.Height + 5;

				SizeF noTickSize = graphics.MeasureString(noRangeMessage, textFont);
				noRangeTextWidth = noTickSize.Width + 5;
				noRangeTextHeight = noTickSize.Height + 5;
			}

			// Plot the range count message to the lower right hand corner of the chart
			if (Bars.Period.Id == PeriodType.Range || isRangeDerivate)
			{
				int	actualRange	= (int) Math.Round(Math.Max(Close[0] - Low[0], High[0] - Close[0]) / Bars.Instrument.MasterInstrument.TickSize);
				int	rangeCount	= CountDown ? Bars.Period.Value - actualRange : actualRange;
				graphics.DrawString((CountDown ? "Range remaining = " + rangeCount : "Range count = " + rangeCount), ChartControl.Font, textBrush, bounds.X + bounds.Width - textWidth, bounds.Y + bounds.Height - textHeight, stringFormat);
			}
			else
				graphics.DrawString(noRangeMessage, ChartControl.Font, textBrush, bounds.X + bounds.Width - noRangeTextWidth, bounds.Y + bounds.Height - noRangeTextHeight, stringFormat);
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
        #endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private RangeCounter[] cacheRangeCounter = null;

        private static RangeCounter checkRangeCounter = new RangeCounter();

        /// <summary>
        /// Displays the range countdown of a bar.
        /// </summary>
        /// <returns></returns>
        public RangeCounter RangeCounter(bool countDown)
        {
            return RangeCounter(Input, countDown);
        }

        /// <summary>
        /// Displays the range countdown of a bar.
        /// </summary>
        /// <returns></returns>
        public RangeCounter RangeCounter(Data.IDataSeries input, bool countDown)
        {
            if (cacheRangeCounter != null)
                for (int idx = 0; idx < cacheRangeCounter.Length; idx++)
                    if (cacheRangeCounter[idx].CountDown == countDown && cacheRangeCounter[idx].EqualsInput(input))
                        return cacheRangeCounter[idx];

            lock (checkRangeCounter)
            {
                checkRangeCounter.CountDown = countDown;
                countDown = checkRangeCounter.CountDown;

                if (cacheRangeCounter != null)
                    for (int idx = 0; idx < cacheRangeCounter.Length; idx++)
                        if (cacheRangeCounter[idx].CountDown == countDown && cacheRangeCounter[idx].EqualsInput(input))
                            return cacheRangeCounter[idx];

                RangeCounter indicator = new RangeCounter();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.CountDown = countDown;
                Indicators.Add(indicator);
                indicator.SetUp();

                RangeCounter[] tmp = new RangeCounter[cacheRangeCounter == null ? 1 : cacheRangeCounter.Length + 1];
                if (cacheRangeCounter != null)
                    cacheRangeCounter.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheRangeCounter = tmp;
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
        /// Displays the range countdown of a bar.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.RangeCounter RangeCounter(bool countDown)
        {
            return _indicator.RangeCounter(Input, countDown);
        }

        /// <summary>
        /// Displays the range countdown of a bar.
        /// </summary>
        /// <returns></returns>
        public Indicator.RangeCounter RangeCounter(Data.IDataSeries input, bool countDown)
        {
            return _indicator.RangeCounter(input, countDown);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Displays the range countdown of a bar.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.RangeCounter RangeCounter(bool countDown)
        {
            return _indicator.RangeCounter(Input, countDown);
        }

        /// <summary>
        /// Displays the range countdown of a bar.
        /// </summary>
        /// <returns></returns>
        public Indicator.RangeCounter RangeCounter(Data.IDataSeries input, bool countDown)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.RangeCounter(input, countDown);
        }
    }
}
#endregion
