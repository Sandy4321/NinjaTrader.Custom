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
	/// This indicator returns 1 when we have n of consecutive bars down, otherwise returns 0. A down bar is defined as a bar where the close is below the open and the bars makes a lower high and a lower low. You can adjust the specific requirements with the indicator options.
	/// </summary>
	[Description("This indicator returns 1 when we have n of consecutive bars down, otherwise returns 0. A down bar is defined as a bar where the close is below the open and the bars makes a lower high and a lower low. You can adjust the specific requirements with the indicator options.")]
	public class NBarsDown : Indicator
	{
		#region Variables
		private int			barCount	= 3;
		private bool		barDown		= true;
		private bool		lowerHigh	= true;
		private bool		lowerLow	= true;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Crimson, 2), PlotStyle.Bar, "Trigger"));
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

					if (!(Close[i] < Close[i + 1]))
						break;

					if (BarDown && !(Close[i] < Open[i])) 
						break;

					if (LowerHigh && !(High[i] < High[i + 1]))
						break;

					if (LowerLow && !(Low[i] < Low[i + 1]))
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

		[Description("Require the close to be lower than the open for all bars.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Bar down")]
		public bool BarDown
		{
			get { return barDown; }
			set { barDown = value; }
		}

		[Description("Require the high of consecutive bars to be lower than the high of the previous bar.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Lower high")]
		public bool LowerHigh
		{
			get { return lowerHigh; }
			set { lowerHigh = value; }
		}

		[Description("Require the low of consecutive bars to be lower than the low of the previous bar.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Lower low")]
		public bool LowerLow
		{
			get { return lowerLow; }
			set { lowerLow = value; }
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
        private NBarsDown[] cacheNBarsDown = null;

        private static NBarsDown checkNBarsDown = new NBarsDown();

        /// <summary>
        /// This indicator returns 1 when we have n of consecutive bars down, otherwise returns 0. A down bar is defined as a bar where the close is below the open and the bars makes a lower high and a lower low. You can adjust the specific requirements with the indicator options.
        /// </summary>
        /// <returns></returns>
        public NBarsDown NBarsDown(int barCount, bool barDown, bool lowerHigh, bool lowerLow)
        {
            return NBarsDown(Input, barCount, barDown, lowerHigh, lowerLow);
        }

        /// <summary>
        /// This indicator returns 1 when we have n of consecutive bars down, otherwise returns 0. A down bar is defined as a bar where the close is below the open and the bars makes a lower high and a lower low. You can adjust the specific requirements with the indicator options.
        /// </summary>
        /// <returns></returns>
        public NBarsDown NBarsDown(Data.IDataSeries input, int barCount, bool barDown, bool lowerHigh, bool lowerLow)
        {
            if (cacheNBarsDown != null)
                for (int idx = 0; idx < cacheNBarsDown.Length; idx++)
                    if (cacheNBarsDown[idx].BarCount == barCount && cacheNBarsDown[idx].BarDown == barDown && cacheNBarsDown[idx].LowerHigh == lowerHigh && cacheNBarsDown[idx].LowerLow == lowerLow && cacheNBarsDown[idx].EqualsInput(input))
                        return cacheNBarsDown[idx];

            lock (checkNBarsDown)
            {
                checkNBarsDown.BarCount = barCount;
                barCount = checkNBarsDown.BarCount;
                checkNBarsDown.BarDown = barDown;
                barDown = checkNBarsDown.BarDown;
                checkNBarsDown.LowerHigh = lowerHigh;
                lowerHigh = checkNBarsDown.LowerHigh;
                checkNBarsDown.LowerLow = lowerLow;
                lowerLow = checkNBarsDown.LowerLow;

                if (cacheNBarsDown != null)
                    for (int idx = 0; idx < cacheNBarsDown.Length; idx++)
                        if (cacheNBarsDown[idx].BarCount == barCount && cacheNBarsDown[idx].BarDown == barDown && cacheNBarsDown[idx].LowerHigh == lowerHigh && cacheNBarsDown[idx].LowerLow == lowerLow && cacheNBarsDown[idx].EqualsInput(input))
                            return cacheNBarsDown[idx];

                NBarsDown indicator = new NBarsDown();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BarCount = barCount;
                indicator.BarDown = barDown;
                indicator.LowerHigh = lowerHigh;
                indicator.LowerLow = lowerLow;
                Indicators.Add(indicator);
                indicator.SetUp();

                NBarsDown[] tmp = new NBarsDown[cacheNBarsDown == null ? 1 : cacheNBarsDown.Length + 1];
                if (cacheNBarsDown != null)
                    cacheNBarsDown.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheNBarsDown = tmp;
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
        /// This indicator returns 1 when we have n of consecutive bars down, otherwise returns 0. A down bar is defined as a bar where the close is below the open and the bars makes a lower high and a lower low. You can adjust the specific requirements with the indicator options.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.NBarsDown NBarsDown(int barCount, bool barDown, bool lowerHigh, bool lowerLow)
        {
            return _indicator.NBarsDown(Input, barCount, barDown, lowerHigh, lowerLow);
        }

        /// <summary>
        /// This indicator returns 1 when we have n of consecutive bars down, otherwise returns 0. A down bar is defined as a bar where the close is below the open and the bars makes a lower high and a lower low. You can adjust the specific requirements with the indicator options.
        /// </summary>
        /// <returns></returns>
        public Indicator.NBarsDown NBarsDown(Data.IDataSeries input, int barCount, bool barDown, bool lowerHigh, bool lowerLow)
        {
            return _indicator.NBarsDown(input, barCount, barDown, lowerHigh, lowerLow);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// This indicator returns 1 when we have n of consecutive bars down, otherwise returns 0. A down bar is defined as a bar where the close is below the open and the bars makes a lower high and a lower low. You can adjust the specific requirements with the indicator options.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.NBarsDown NBarsDown(int barCount, bool barDown, bool lowerHigh, bool lowerLow)
        {
            return _indicator.NBarsDown(Input, barCount, barDown, lowerHigh, lowerLow);
        }

        /// <summary>
        /// This indicator returns 1 when we have n of consecutive bars down, otherwise returns 0. A down bar is defined as a bar where the close is below the open and the bars makes a lower high and a lower low. You can adjust the specific requirements with the indicator options.
        /// </summary>
        /// <returns></returns>
        public Indicator.NBarsDown NBarsDown(Data.IDataSeries input, int barCount, bool barDown, bool lowerHigh, bool lowerLow)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.NBarsDown(input, barCount, barDown, lowerHigh, lowerLow);
        }
    }
}
#endregion
