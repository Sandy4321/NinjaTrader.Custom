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
    /// The TMA (Triangular Moving Average) is a weighted moving average. Compared to the WMA which puts more weight on the latest price bar, the TMA puts more weight on the data in the middle of the specified period.
    /// </summary>
    [Description("The TMA (Triangular Moving Average) is a weighted moving average. Compared to the WMA which puts more weight on the latest price bar, the TMA puts more weight on the data in the middle of the specified period.")]
    public class TMA : Indicator
    {
        #region Variables
		private int period = 14;
		private int p1;
		private int p2;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.SlateBlue, "TMA"));

            Overlay = true;	// Plots the indicator on top of price

			InitializeParameters();
		}

		private void InitializeParameters()
		{
			if ((Period & 1) == 0) 
			{
				// Even period
				p1 = Period / 2;
				p2 = p1 + 1;
			} 
			else 
			{
				// Odd period
				p1 = (Period + 1) / 2;
				p2 = p1;
			}
		}

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			Value.Set(SMA(SMA(Inputs[0], p1), p2)[0]);	
        }

        #region Properties
        /// <summary>
        /// Period
        /// </summary>
        [Description("Number of bars used for calculation")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set 
			{ 
				period = Math.Max(1, value); 
				InitializeParameters();
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
        private TMA[] cacheTMA = null;

        private static TMA checkTMA = new TMA();

        /// <summary>
        /// The TMA (Triangular Moving Average) is a weighted moving average. Compared to the WMA which puts more weight on the latest price bar, the TMA puts more weight on the data in the middle of the specified period.
        /// </summary>
        /// <returns></returns>
        public TMA TMA(int period)
        {
            return TMA(Input, period);
        }

        /// <summary>
        /// The TMA (Triangular Moving Average) is a weighted moving average. Compared to the WMA which puts more weight on the latest price bar, the TMA puts more weight on the data in the middle of the specified period.
        /// </summary>
        /// <returns></returns>
        public TMA TMA(Data.IDataSeries input, int period)
        {
            if (cacheTMA != null)
                for (int idx = 0; idx < cacheTMA.Length; idx++)
                    if (cacheTMA[idx].Period == period && cacheTMA[idx].EqualsInput(input))
                        return cacheTMA[idx];

            lock (checkTMA)
            {
                checkTMA.Period = period;
                period = checkTMA.Period;

                if (cacheTMA != null)
                    for (int idx = 0; idx < cacheTMA.Length; idx++)
                        if (cacheTMA[idx].Period == period && cacheTMA[idx].EqualsInput(input))
                            return cacheTMA[idx];

                TMA indicator = new TMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                TMA[] tmp = new TMA[cacheTMA == null ? 1 : cacheTMA.Length + 1];
                if (cacheTMA != null)
                    cacheTMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTMA = tmp;
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
        /// The TMA (Triangular Moving Average) is a weighted moving average. Compared to the WMA which puts more weight on the latest price bar, the TMA puts more weight on the data in the middle of the specified period.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TMA TMA(int period)
        {
            return _indicator.TMA(Input, period);
        }

        /// <summary>
        /// The TMA (Triangular Moving Average) is a weighted moving average. Compared to the WMA which puts more weight on the latest price bar, the TMA puts more weight on the data in the middle of the specified period.
        /// </summary>
        /// <returns></returns>
        public Indicator.TMA TMA(Data.IDataSeries input, int period)
        {
            return _indicator.TMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The TMA (Triangular Moving Average) is a weighted moving average. Compared to the WMA which puts more weight on the latest price bar, the TMA puts more weight on the data in the middle of the specified period.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TMA TMA(int period)
        {
            return _indicator.TMA(Input, period);
        }

        /// <summary>
        /// The TMA (Triangular Moving Average) is a weighted moving average. Compared to the WMA which puts more weight on the latest price bar, the TMA puts more weight on the data in the middle of the specified period.
        /// </summary>
        /// <returns></returns>
        public Indicator.TMA TMA(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TMA(input, period);
        }
    }
}
#endregion
