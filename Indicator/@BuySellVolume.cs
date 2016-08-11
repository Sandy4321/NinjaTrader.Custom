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
	/// This indicator is a real-time indicator and does not plot against historical data. Plots a histogram splitting volume between trades at the ask or higher and trades at the bid and lower.
	/// </summary>
	[Description("This indicator is a real-time indicator and does not plot against historical data. Plots a histogram splitting volume between trades at the ask or higher and trades at the bid and lower.")]
	[Gui.Design.DisplayName("BuySellVolume")]
	public class BuySellVolume : Indicator
	{
		#region Variables
		private int activeBar = -1;
		private System.Collections.ArrayList alBuys = new System.Collections.ArrayList();
		private System.Collections.ArrayList alSells = new System.Collections.ArrayList();
		private double buys = 0;
		private bool firstPaint = true;
		private double previousVol = 0;
		private double sells = 0;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Red, 6), PlotStyle.Bar, "Sells"));
			Add(new Plot(new Pen(Color.Green, 6), PlotStyle.Bar, "Buys"));
			CalculateOnBarClose = false;
			DisplayInDataBox = false;
			PaintPriceMarkers = false;
			PlotsConfigurable = true;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar < activeBar)
			{
				Values[0].Set((double)alSells[CurrentBar]);
				Values[1].Set((double)alSells[CurrentBar] + (double)alBuys[CurrentBar]);
				return;
			}
			else if (CurrentBar != activeBar)
			{
				previousVol = 0;
				alBuys.Insert(Math.Max(activeBar, 0), Historical ? 0 : buys);
				alSells.Insert(Math.Max(activeBar, 0), Historical ? 0 : sells);
				buys = 0;
				sells = 0;
				activeBar = CurrentBar;
			}

			if (firstPaint)
				firstPaint = false;
			else
			{
				double tradeVol = previousVol == 0 ? Volume[0] : Volume[0] - previousVol;
				if (Close[0] >= GetCurrentAsk())
					buys += tradeVol;
				else if (Close[0] <= GetCurrentBid())
					sells += tradeVol;
			}

			previousVol = Volume[0];
			if (!firstPaint && !Historical)
			{
				Values[0].Set(sells);
				Values[1].Set(buys + sells);
			}
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private BuySellVolume[] cacheBuySellVolume = null;

        private static BuySellVolume checkBuySellVolume = new BuySellVolume();

        /// <summary>
        /// This indicator is a real-time indicator and does not plot against historical data. Plots a histogram splitting volume between trades at the ask or higher and trades at the bid and lower.
        /// </summary>
        /// <returns></returns>
        public BuySellVolume BuySellVolume()
        {
            return BuySellVolume(Input);
        }

        /// <summary>
        /// This indicator is a real-time indicator and does not plot against historical data. Plots a histogram splitting volume between trades at the ask or higher and trades at the bid and lower.
        /// </summary>
        /// <returns></returns>
        public BuySellVolume BuySellVolume(Data.IDataSeries input)
        {
            if (cacheBuySellVolume != null)
                for (int idx = 0; idx < cacheBuySellVolume.Length; idx++)
                    if (cacheBuySellVolume[idx].EqualsInput(input))
                        return cacheBuySellVolume[idx];

            lock (checkBuySellVolume)
            {
                if (cacheBuySellVolume != null)
                    for (int idx = 0; idx < cacheBuySellVolume.Length; idx++)
                        if (cacheBuySellVolume[idx].EqualsInput(input))
                            return cacheBuySellVolume[idx];

                BuySellVolume indicator = new BuySellVolume();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                BuySellVolume[] tmp = new BuySellVolume[cacheBuySellVolume == null ? 1 : cacheBuySellVolume.Length + 1];
                if (cacheBuySellVolume != null)
                    cacheBuySellVolume.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheBuySellVolume = tmp;
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
        /// This indicator is a real-time indicator and does not plot against historical data. Plots a histogram splitting volume between trades at the ask or higher and trades at the bid and lower.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BuySellVolume BuySellVolume()
        {
            return _indicator.BuySellVolume(Input);
        }

        /// <summary>
        /// This indicator is a real-time indicator and does not plot against historical data. Plots a histogram splitting volume between trades at the ask or higher and trades at the bid and lower.
        /// </summary>
        /// <returns></returns>
        public Indicator.BuySellVolume BuySellVolume(Data.IDataSeries input)
        {
            return _indicator.BuySellVolume(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// This indicator is a real-time indicator and does not plot against historical data. Plots a histogram splitting volume between trades at the ask or higher and trades at the bid and lower.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BuySellVolume BuySellVolume()
        {
            return _indicator.BuySellVolume(Input);
        }

        /// <summary>
        /// This indicator is a real-time indicator and does not plot against historical data. Plots a histogram splitting volume between trades at the ask or higher and trades at the bid and lower.
        /// </summary>
        /// <returns></returns>
        public Indicator.BuySellVolume BuySellVolume(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.BuySellVolume(input);
        }
    }
}
#endregion
