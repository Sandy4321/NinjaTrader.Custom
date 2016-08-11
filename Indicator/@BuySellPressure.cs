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
    /// Indicates the current buying or selling pressure as a perecentage. This is a tick by tick indicator. If 'Calculate on bar close' is true, the indicator values will always be 100.
    /// </summary>
    [Description("Indicates the current buying or selling pressure as a perecentage. This is a tick by tick indicator. If 'Calculate on bar close' is true, the indicator values will always be 100.")]
    public class BuySellPressure : Indicator
    {
        #region Variables

        // Wizard generated variables
        // User defined variables (add any user defined variables below)
		private int						activeBar		= int.MaxValue;
		private DataSeries			buys;
		private double					previousVol		= 0;
		private DataSeries			sells;
		private DateTime				startTime;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.ForestGreen, PlotStyle.Line, "Buy Pressure"));
			Add(new Plot(Color.Red, PlotStyle.Line, "Sell Pressure"));
			Add(new Line(Color.Black, 75, "Upper"));
			Add(new Line(Color.Black, 25, "Lower"));
            CalculateOnBarClose	= false;
            Overlay				= false;
			startTime			= DateTime.Now;

			buys	= new DataSeries(this);
			sells	= new DataSeries(this);
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar != activeBar)
			{
				previousVol = 0;
				activeBar = CurrentBar;
			}
			
			// Calculate buy/sell trades if indicator running in real-time
			if (!CalculateOnBarClose && startTime.Ticks <= Time[0].Ticks)
			{
				double tradeVol = previousVol == 0 ? Volume[0] : Volume[0] - previousVol;
				if (Close[0] >= GetCurrentAsk())
					buys.Set(buys[0] + tradeVol);
				else if (Close[0] <= GetCurrentBid())
					sells.Set(sells[0] + tradeVol);
				else if (buys[0] == 0 && sells[0] == 0)
				{
					buys.Set(1);
					sells.Set(1);
				}
				previousVol = Volume[0];
			}
			else
			{
				buys.Set(1);
				sells.Set(1);
			}

			BuyPressure.Set((buys[0] / (buys[0] + sells[0])) * 100);
			SellPressure.Set((sells[0] / (buys[0] + sells[0])) * 100);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries BuyPressure
        {
            get { return Values[0]; }
        }

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
		[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
		public DataSeries SellPressure
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
        private BuySellPressure[] cacheBuySellPressure = null;

        private static BuySellPressure checkBuySellPressure = new BuySellPressure();

        /// <summary>
        /// Indicates the current buying or selling pressure as a perecentage. This is a tick by tick indicator. If 'Calculate on bar close' is true, the indicator values will always be 100.
        /// </summary>
        /// <returns></returns>
        public BuySellPressure BuySellPressure()
        {
            return BuySellPressure(Input);
        }

        /// <summary>
        /// Indicates the current buying or selling pressure as a perecentage. This is a tick by tick indicator. If 'Calculate on bar close' is true, the indicator values will always be 100.
        /// </summary>
        /// <returns></returns>
        public BuySellPressure BuySellPressure(Data.IDataSeries input)
        {
            if (cacheBuySellPressure != null)
                for (int idx = 0; idx < cacheBuySellPressure.Length; idx++)
                    if (cacheBuySellPressure[idx].EqualsInput(input))
                        return cacheBuySellPressure[idx];

            lock (checkBuySellPressure)
            {
                if (cacheBuySellPressure != null)
                    for (int idx = 0; idx < cacheBuySellPressure.Length; idx++)
                        if (cacheBuySellPressure[idx].EqualsInput(input))
                            return cacheBuySellPressure[idx];

                BuySellPressure indicator = new BuySellPressure();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                BuySellPressure[] tmp = new BuySellPressure[cacheBuySellPressure == null ? 1 : cacheBuySellPressure.Length + 1];
                if (cacheBuySellPressure != null)
                    cacheBuySellPressure.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheBuySellPressure = tmp;
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
        /// Indicates the current buying or selling pressure as a perecentage. This is a tick by tick indicator. If 'Calculate on bar close' is true, the indicator values will always be 100.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BuySellPressure BuySellPressure()
        {
            return _indicator.BuySellPressure(Input);
        }

        /// <summary>
        /// Indicates the current buying or selling pressure as a perecentage. This is a tick by tick indicator. If 'Calculate on bar close' is true, the indicator values will always be 100.
        /// </summary>
        /// <returns></returns>
        public Indicator.BuySellPressure BuySellPressure(Data.IDataSeries input)
        {
            return _indicator.BuySellPressure(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Indicates the current buying or selling pressure as a perecentage. This is a tick by tick indicator. If 'Calculate on bar close' is true, the indicator values will always be 100.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BuySellPressure BuySellPressure()
        {
            return _indicator.BuySellPressure(Input);
        }

        /// <summary>
        /// Indicates the current buying or selling pressure as a perecentage. This is a tick by tick indicator. If 'Calculate on bar close' is true, the indicator values will always be 100.
        /// </summary>
        /// <returns></returns>
        public Indicator.BuySellPressure BuySellPressure(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.BuySellPressure(input);
        }
    }
}
#endregion
