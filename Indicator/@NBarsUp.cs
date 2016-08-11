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
	/// This indicator returns 1 when we have n of consecutive bars up, otherwise returns 0. An up bar is defined as a bar where the close is above the open and the bars makes a higher high and a higher low. You can adjust the specific requirements with the indicator options.
	/// </summary>
	[Description("This indicator returns 1 when we have n of consecutive bars up, otherwise returns 0. An up bar is defined as a bar where the close is above the open and the bars makes a higher high and a higher low. You can adjust the specific requirements with the indicator options.")]
	public class NBarsUp : Indicator
	{
		#region Variables
		private int			barCount	= 3;
		private bool		barUp		= true;
		private bool		higherHigh	= true;
		private bool		higherLow	= true;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.LimeGreen, 2), PlotStyle.Bar, "Trigger"));
		}

		/// <summary>
		/// Calculates the indicator value(s) at the current index.
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar < BarCount)
			{
				Value.Set(0);
			}
			else
			{
				bool gotBars = false;

				for (int i = 0; i < BarCount + 1; i++)
				{
					if (i == BarCount)
					{
						gotBars = true;
						break;
					}

					if (!(Close[i] > Close[i + 1]))
						break;

					if (BarUp && !(Close[i] > Open[i])) 
						break;

					if (HigherHigh && !(High[i] > High[i + 1]))
						break;

					if (HigherLow && !(Low[i] > Low[i + 1]))
						break;
				}

				Value.Set(gotBars == true ? 1 : 0);
			}
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Numbers of bars back to check.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Bar count")]
		public int BarCount
		{
			get { return barCount; }
			set { barCount = Math.Max(2, value); }
		}

		[Description("Require the close to be higher than the open for all bars.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Bar up")]
		public bool BarUp
		{
			get { return barUp; }
			set { barUp = value; }
		}

		[Description("Require the high of consecutive bars to be higher than the high of the previous bar.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Higher high")]
		public bool HigherHigh
		{
			get { return higherHigh; }
			set { higherHigh = value; }
		}

		[Description("Require the low of consecutive bars to be higher than the low of the previous bar.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Higher low")]
		public bool HigherLow
		{
			get { return higherLow; }
			set { higherLow = value; }
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
        private NBarsUp[] cacheNBarsUp = null;

        private static NBarsUp checkNBarsUp = new NBarsUp();

        /// <summary>
        /// This indicator returns 1 when we have n of consecutive bars up, otherwise returns 0. An up bar is defined as a bar where the close is above the open and the bars makes a higher high and a higher low. You can adjust the specific requirements with the indicator options.
        /// </summary>
        /// <returns></returns>
        public NBarsUp NBarsUp(int barCount, bool barUp, bool higherHigh, bool higherLow)
        {
            return NBarsUp(Input, barCount, barUp, higherHigh, higherLow);
        }

        /// <summary>
        /// This indicator returns 1 when we have n of consecutive bars up, otherwise returns 0. An up bar is defined as a bar where the close is above the open and the bars makes a higher high and a higher low. You can adjust the specific requirements with the indicator options.
        /// </summary>
        /// <returns></returns>
        public NBarsUp NBarsUp(Data.IDataSeries input, int barCount, bool barUp, bool higherHigh, bool higherLow)
        {
            if (cacheNBarsUp != null)
                for (int idx = 0; idx < cacheNBarsUp.Length; idx++)
                    if (cacheNBarsUp[idx].BarCount == barCount && cacheNBarsUp[idx].BarUp == barUp && cacheNBarsUp[idx].HigherHigh == higherHigh && cacheNBarsUp[idx].HigherLow == higherLow && cacheNBarsUp[idx].EqualsInput(input))
                        return cacheNBarsUp[idx];

            lock (checkNBarsUp)
            {
                checkNBarsUp.BarCount = barCount;
                barCount = checkNBarsUp.BarCount;
                checkNBarsUp.BarUp = barUp;
                barUp = checkNBarsUp.BarUp;
                checkNBarsUp.HigherHigh = higherHigh;
                higherHigh = checkNBarsUp.HigherHigh;
                checkNBarsUp.HigherLow = higherLow;
                higherLow = checkNBarsUp.HigherLow;

                if (cacheNBarsUp != null)
                    for (int idx = 0; idx < cacheNBarsUp.Length; idx++)
                        if (cacheNBarsUp[idx].BarCount == barCount && cacheNBarsUp[idx].BarUp == barUp && cacheNBarsUp[idx].HigherHigh == higherHigh && cacheNBarsUp[idx].HigherLow == higherLow && cacheNBarsUp[idx].EqualsInput(input))
                            return cacheNBarsUp[idx];

                NBarsUp indicator = new NBarsUp();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BarCount = barCount;
                indicator.BarUp = barUp;
                indicator.HigherHigh = higherHigh;
                indicator.HigherLow = higherLow;
                Indicators.Add(indicator);
                indicator.SetUp();

                NBarsUp[] tmp = new NBarsUp[cacheNBarsUp == null ? 1 : cacheNBarsUp.Length + 1];
                if (cacheNBarsUp != null)
                    cacheNBarsUp.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheNBarsUp = tmp;
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
        /// This indicator returns 1 when we have n of consecutive bars up, otherwise returns 0. An up bar is defined as a bar where the close is above the open and the bars makes a higher high and a higher low. You can adjust the specific requirements with the indicator options.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.NBarsUp NBarsUp(int barCount, bool barUp, bool higherHigh, bool higherLow)
        {
            return _indicator.NBarsUp(Input, barCount, barUp, higherHigh, higherLow);
        }

        /// <summary>
        /// This indicator returns 1 when we have n of consecutive bars up, otherwise returns 0. An up bar is defined as a bar where the close is above the open and the bars makes a higher high and a higher low. You can adjust the specific requirements with the indicator options.
        /// </summary>
        /// <returns></returns>
        public Indicator.NBarsUp NBarsUp(Data.IDataSeries input, int barCount, bool barUp, bool higherHigh, bool higherLow)
        {
            return _indicator.NBarsUp(input, barCount, barUp, higherHigh, higherLow);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// This indicator returns 1 when we have n of consecutive bars up, otherwise returns 0. An up bar is defined as a bar where the close is above the open and the bars makes a higher high and a higher low. You can adjust the specific requirements with the indicator options.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.NBarsUp NBarsUp(int barCount, bool barUp, bool higherHigh, bool higherLow)
        {
            return _indicator.NBarsUp(Input, barCount, barUp, higherHigh, higherLow);
        }

        /// <summary>
        /// This indicator returns 1 when we have n of consecutive bars up, otherwise returns 0. An up bar is defined as a bar where the close is above the open and the bars makes a higher high and a higher low. You can adjust the specific requirements with the indicator options.
        /// </summary>
        /// <returns></returns>
        public Indicator.NBarsUp NBarsUp(Data.IDataSeries input, int barCount, bool barUp, bool higherHigh, bool higherLow)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.NBarsUp(input, barCount, barUp, higherHigh, higherLow);
        }
    }
}
#endregion
