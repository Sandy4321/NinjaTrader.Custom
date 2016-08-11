// 
// Copyright (C) 2007, NinjaTrader LLC <www.ninjatrader.com>.
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
	/// The Bar Timer displays the remaining time of a minute based bar.
	/// </summary>
	[Description("Displays remaining time of minute based bar")]
	public class BarTimer : Indicator
	{
		#region Variables
		private	string						errorDisabled		= "Bar timer disabled since either you are disconnected or current time is outside session time or chart end date";
		private	string						errorIntraday		= "Bar timer only works on intraday time based intervals";
		private	DateTime					lastTimePlot		= Cbi.Globals.MinDate;
        private float						noTickTextWidth		= 0;
        private float						noTickTextHeight	= 0;
		private float						noConTextWidth		= 0;
		private float						noConTextHeight		= 0;
		private StringFormat				stringFormat		= new StringFormat();
		private SolidBrush					textBrush			= new SolidBrush(Color.Black);
		private Font						textFont			= new Font("Arial", 30);
		private float						textWidth			= 0;
		private float						textHeight			= 0;
        private System.Windows.Forms.Timer	timer;
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
            if (timer == null)
            {
				if (DisplayTime())
				{
					timer = new System.Windows.Forms.Timer();
					timer.Interval = 100;
					timer.Tick += new EventHandler(OnTimerTick);
					timer.Enabled = true;
				}
            }
        }

        #region Properties
        #endregion

        #region Miscellaneous
        private bool DisplayTime()
        {
			if (ChartControl != null
					&& Bars != null
					&& Bars.Count > 0
					&& Bars.MarketData != null
					&& Bars.MarketData.Connection.PriceStatus == Cbi.ConnectionStatus.Connected
					&& Bars.Session.InSession(Now, Bars.Period, true, Bars.BarsType))
				return true;

            return false;
        }

		/// <summary>
        /// Overload this method to handle the termination of an indicator. Use this method to dispose of any resources vs overloading the Dispose() method.
		/// </summary>
		protected override void OnTermination()
        {
            textBrush.Dispose();		
            textFont.Dispose();
            stringFormat.Dispose();

            if (timer != null)
            {
                timer.Enabled = false;
                timer = null;
            }
        }

		private DateTime Now
		{
			get 
			{ 
				DateTime now = (Bars.MarketData.Connection.Options.Provider == Cbi.Provider.Replay ? Bars.MarketData.Connection.Now : DateTime.Now); 

				if (now.Millisecond > 0)
					now = Cbi.Globals.MinDate.AddSeconds((long) System.Math.Floor(now.Subtract(Cbi.Globals.MinDate).TotalSeconds));

				return now;
			}
		}

        private void OnTimerTick(object sender, EventArgs e)
        {
			if (DateTime.Now.Subtract(lastTimePlot).Seconds >= 1 && DisplayTime())
			{
			    ChartControl.ChartPanel.Invalidate();
				lastTimePlot = DateTime.Now;
			}
        }

        public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
        {
            if (Bars == null)
                return;

			// Recalculate the proper string size should the chart control object font and axis color change
            if (textBrush.Color != ChartControl.AxisColor || textFont != ChartControl.Font)
            {
                textBrush.Color = ChartControl.AxisColor;
                textFont = (Font) ChartControl.Font.Clone();

                SizeF size = graphics.MeasureString("Time remaining = -00:-00:-00", textFont);
                textWidth = size.Width + 5;
                textHeight = size.Height + 5;

                SizeF noTickSize = graphics.MeasureString(errorIntraday, textFont);
                noTickTextWidth = noTickSize.Width + 5;
                noTickTextHeight = noTickSize.Height + 5;

				SizeF noConSize = graphics.MeasureString(errorDisabled, textFont);
				noConTextWidth	= noConSize.Width + 5;
				noConTextHeight = noConSize.Height + 5;
            }

            // Plot the time remaining message to the lower right hand corner of the chart
			if (Bars.Period.Id == PeriodType.Minute || Bars.Period.Id == PeriodType.Second)
			{
				if (DisplayTime())
				{
					if (timer != null && !timer.Enabled)
						timer.Enabled = true;
					TimeSpan	barTimeLeft = Bars.GetTime(Bars.Count - 1).Subtract(Now);
					string		timeLeft	= (barTimeLeft.Ticks < 0 ? "00:00:00" : barTimeLeft.Hours.ToString("00") + ":" + barTimeLeft.Minutes.ToString("00") + ":" + barTimeLeft.Seconds.ToString("00"));
					graphics.DrawString("Time remaining = " + timeLeft, ChartControl.Font, textBrush, bounds.X + bounds.Width - textWidth, bounds.Y + bounds.Height - textHeight, stringFormat);
				}
				else
					graphics.DrawString(errorDisabled, ChartControl.Font, textBrush, bounds.X + bounds.Width - noConTextWidth, bounds.Y + bounds.Height - noConTextHeight, stringFormat);
			}
			else
			{
				graphics.DrawString(errorIntraday, ChartControl.Font, textBrush, bounds.X + bounds.Width - noTickTextWidth, bounds.Y + bounds.Height - noTickTextHeight, stringFormat);
				if (timer != null)
					timer.Enabled = false;
			}
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
        private BarTimer[] cacheBarTimer = null;

        private static BarTimer checkBarTimer = new BarTimer();

        /// <summary>
        /// Displays remaining time of minute based bar
        /// </summary>
        /// <returns></returns>
        public BarTimer BarTimer()
        {
            return BarTimer(Input);
        }

        /// <summary>
        /// Displays remaining time of minute based bar
        /// </summary>
        /// <returns></returns>
        public BarTimer BarTimer(Data.IDataSeries input)
        {
            if (cacheBarTimer != null)
                for (int idx = 0; idx < cacheBarTimer.Length; idx++)
                    if (cacheBarTimer[idx].EqualsInput(input))
                        return cacheBarTimer[idx];

            lock (checkBarTimer)
            {
                if (cacheBarTimer != null)
                    for (int idx = 0; idx < cacheBarTimer.Length; idx++)
                        if (cacheBarTimer[idx].EqualsInput(input))
                            return cacheBarTimer[idx];

                BarTimer indicator = new BarTimer();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                BarTimer[] tmp = new BarTimer[cacheBarTimer == null ? 1 : cacheBarTimer.Length + 1];
                if (cacheBarTimer != null)
                    cacheBarTimer.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheBarTimer = tmp;
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
        /// Displays remaining time of minute based bar
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BarTimer BarTimer()
        {
            return _indicator.BarTimer(Input);
        }

        /// <summary>
        /// Displays remaining time of minute based bar
        /// </summary>
        /// <returns></returns>
        public Indicator.BarTimer BarTimer(Data.IDataSeries input)
        {
            return _indicator.BarTimer(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Displays remaining time of minute based bar
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BarTimer BarTimer()
        {
            return _indicator.BarTimer(Input);
        }

        /// <summary>
        /// Displays remaining time of minute based bar
        /// </summary>
        /// <returns></returns>
        public Indicator.BarTimer BarTimer(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.BarTimer(input);
        }
    }
}
#endregion
