// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
// Reference to "Trading with Adaptive Price Zone" article in TASC, October 2006, p. 16 by Ian Copsey.
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
    /// Relative Spread Strength of the spread between two moving averages. TASC, October 2006, p. 16.
    /// </summary>
    [Description("Relative Spread Strength of the spread between two moving averages. TASC, October 2006, p. 16.")]
	public class RSS : Indicator
    {
        #region Variables
            private int ema1 = 10; 
            private int ema2 = 40;
            private int length = 5;
			private DataSeries spread;
			private DataSeries rs;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Green, PlotStyle.Line, "RSS"));
            Add(new Line(Color.LightSlateGray, 20, "Lower"));
            Add(new Line(Color.LightSlateGray, 80, "Upper"));
			spread 	= new DataSeries(this);
			rs		= new DataSeries(this);
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			spread.Set(EMA(EMA1)[0] - EMA(EMA2)[0]);
			rs.Set(RSI(spread, Length, 0)[0]);	
			Value.Set(SMA(rs, 5)[0]);
        }

        #region Properties

        [Description("")]
        [GridCategory("Parameters")]
        public int EMA1
        {
            get { return ema1; }
            set { ema1 = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int EMA2
        {
            get { return ema2; }
            set { ema2 = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Length
        {
            get { return length; }
            set { length = Math.Max(1, value); }
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
        private RSS[] cacheRSS = null;

        private static RSS checkRSS = new RSS();

        /// <summary>
        /// Relative Spread Strength of the spread between two moving averages. TASC, October 2006, p. 16.
        /// </summary>
        /// <returns></returns>
        public RSS RSS(int eMA1, int eMA2, int length)
        {
            return RSS(Input, eMA1, eMA2, length);
        }

        /// <summary>
        /// Relative Spread Strength of the spread between two moving averages. TASC, October 2006, p. 16.
        /// </summary>
        /// <returns></returns>
        public RSS RSS(Data.IDataSeries input, int eMA1, int eMA2, int length)
        {
            if (cacheRSS != null)
                for (int idx = 0; idx < cacheRSS.Length; idx++)
                    if (cacheRSS[idx].EMA1 == eMA1 && cacheRSS[idx].EMA2 == eMA2 && cacheRSS[idx].Length == length && cacheRSS[idx].EqualsInput(input))
                        return cacheRSS[idx];

            lock (checkRSS)
            {
                checkRSS.EMA1 = eMA1;
                eMA1 = checkRSS.EMA1;
                checkRSS.EMA2 = eMA2;
                eMA2 = checkRSS.EMA2;
                checkRSS.Length = length;
                length = checkRSS.Length;

                if (cacheRSS != null)
                    for (int idx = 0; idx < cacheRSS.Length; idx++)
                        if (cacheRSS[idx].EMA1 == eMA1 && cacheRSS[idx].EMA2 == eMA2 && cacheRSS[idx].Length == length && cacheRSS[idx].EqualsInput(input))
                            return cacheRSS[idx];

                RSS indicator = new RSS();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.EMA1 = eMA1;
                indicator.EMA2 = eMA2;
                indicator.Length = length;
                Indicators.Add(indicator);
                indicator.SetUp();

                RSS[] tmp = new RSS[cacheRSS == null ? 1 : cacheRSS.Length + 1];
                if (cacheRSS != null)
                    cacheRSS.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheRSS = tmp;
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
        /// Relative Spread Strength of the spread between two moving averages. TASC, October 2006, p. 16.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.RSS RSS(int eMA1, int eMA2, int length)
        {
            return _indicator.RSS(Input, eMA1, eMA2, length);
        }

        /// <summary>
        /// Relative Spread Strength of the spread between two moving averages. TASC, October 2006, p. 16.
        /// </summary>
        /// <returns></returns>
        public Indicator.RSS RSS(Data.IDataSeries input, int eMA1, int eMA2, int length)
        {
            return _indicator.RSS(input, eMA1, eMA2, length);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Relative Spread Strength of the spread between two moving averages. TASC, October 2006, p. 16.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.RSS RSS(int eMA1, int eMA2, int length)
        {
            return _indicator.RSS(Input, eMA1, eMA2, length);
        }

        /// <summary>
        /// Relative Spread Strength of the spread between two moving averages. TASC, October 2006, p. 16.
        /// </summary>
        /// <returns></returns>
        public Indicator.RSS RSS(Data.IDataSeries input, int eMA1, int eMA2, int length)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.RSS(input, eMA1, eMA2, length);
        }
    }
}
#endregion
