// 
// Copyright (C) 2008, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Chaikin Volatility
    /// </summary>
    [Description("Chaikin Volatility")]
    public class ChaikinVolatility : Indicator
    {
        #region Variables
            private int period = 14; // Default setting for Period
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.OrangeRed), PlotStyle.Line, "ChaikinVolatility"));
            CalculateOnBarClose	= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			int myPeriod = Math.Min(CurrentBar, Period);
			Range myRange = Range();
            Value.Set(((EMA(myRange, myPeriod)[0] - EMA(myRange, myPeriod)[myPeriod]) / EMA(myRange, myPeriod)[myPeriod]) * 100);
        }

		#region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
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
        private ChaikinVolatility[] cacheChaikinVolatility = null;

        private static ChaikinVolatility checkChaikinVolatility = new ChaikinVolatility();

        /// <summary>
        /// Chaikin Volatility
        /// </summary>
        /// <returns></returns>
        public ChaikinVolatility ChaikinVolatility(int period)
        {
            return ChaikinVolatility(Input, period);
        }

        /// <summary>
        /// Chaikin Volatility
        /// </summary>
        /// <returns></returns>
        public ChaikinVolatility ChaikinVolatility(Data.IDataSeries input, int period)
        {
            if (cacheChaikinVolatility != null)
                for (int idx = 0; idx < cacheChaikinVolatility.Length; idx++)
                    if (cacheChaikinVolatility[idx].Period == period && cacheChaikinVolatility[idx].EqualsInput(input))
                        return cacheChaikinVolatility[idx];

            lock (checkChaikinVolatility)
            {
                checkChaikinVolatility.Period = period;
                period = checkChaikinVolatility.Period;

                if (cacheChaikinVolatility != null)
                    for (int idx = 0; idx < cacheChaikinVolatility.Length; idx++)
                        if (cacheChaikinVolatility[idx].Period == period && cacheChaikinVolatility[idx].EqualsInput(input))
                            return cacheChaikinVolatility[idx];

                ChaikinVolatility indicator = new ChaikinVolatility();
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

                ChaikinVolatility[] tmp = new ChaikinVolatility[cacheChaikinVolatility == null ? 1 : cacheChaikinVolatility.Length + 1];
                if (cacheChaikinVolatility != null)
                    cacheChaikinVolatility.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheChaikinVolatility = tmp;
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
        /// Chaikin Volatility
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ChaikinVolatility ChaikinVolatility(int period)
        {
            return _indicator.ChaikinVolatility(Input, period);
        }

        /// <summary>
        /// Chaikin Volatility
        /// </summary>
        /// <returns></returns>
        public Indicator.ChaikinVolatility ChaikinVolatility(Data.IDataSeries input, int period)
        {
            return _indicator.ChaikinVolatility(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Chaikin Volatility
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ChaikinVolatility ChaikinVolatility(int period)
        {
            return _indicator.ChaikinVolatility(Input, period);
        }

        /// <summary>
        /// Chaikin Volatility
        /// </summary>
        /// <returns></returns>
        public Indicator.ChaikinVolatility ChaikinVolatility(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ChaikinVolatility(input, period);
        }
    }
}
#endregion
