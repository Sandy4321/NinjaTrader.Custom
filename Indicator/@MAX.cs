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
    /// The Maximum shows the maximum of the last n bars.
    /// </summary>
    [Description("The Maximum shows the maximum of the last n bars.")]
    public class MAX : Indicator
    {
        #region Variables
        private int    lastBar;
        private double lastMax;
        private int    period     = 14;
        private double runningMax;
        private int    runningBar;
        private int    thisBar;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Green, "MAX"));
            Overlay = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            if (CurrentBar == 0)
            {
                runningMax = Input[0];
                lastMax    = Input[0];
                runningBar = 0;
                lastBar    = 0;
                thisBar    = 0;
                return;
            }

            if (CurrentBar - runningBar >= Period)
            {
                runningMax = double.MinValue;
                for (int barsBack = Math.Min(CurrentBar, Period - 1); barsBack > 0; barsBack--)
                    if (Input[barsBack] >= runningMax)
                    {
                        runningMax  = Input[barsBack];
                        runningBar  = CurrentBar - barsBack;
                    }
            }

            if (thisBar != CurrentBar)
            {
                lastMax = runningMax;
                lastBar = runningBar;
                thisBar = CurrentBar;
            }

            if (Input[0] >= lastMax)
            {
                runningMax = Input[0];
                runningBar = CurrentBar;
            }
            else
            {
                runningMax = lastMax;
                runningBar = lastBar;
            }

            Value.Set(runningMax);
        }

        #region Properties
        /// <summary>
        /// </summary>
        [Description("Numbers of bars used for calculations")]
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
        private MAX[] cacheMAX = null;

        private static MAX checkMAX = new MAX();

        /// <summary>
        /// The Maximum shows the maximum of the last n bars.
        /// </summary>
        /// <returns></returns>
        public MAX MAX(int period)
        {
            return MAX(Input, period);
        }

        /// <summary>
        /// The Maximum shows the maximum of the last n bars.
        /// </summary>
        /// <returns></returns>
        public MAX MAX(Data.IDataSeries input, int period)
        {
            if (cacheMAX != null)
                for (int idx = 0; idx < cacheMAX.Length; idx++)
                    if (cacheMAX[idx].Period == period && cacheMAX[idx].EqualsInput(input))
                        return cacheMAX[idx];

            lock (checkMAX)
            {
                checkMAX.Period = period;
                period = checkMAX.Period;

                if (cacheMAX != null)
                    for (int idx = 0; idx < cacheMAX.Length; idx++)
                        if (cacheMAX[idx].Period == period && cacheMAX[idx].EqualsInput(input))
                            return cacheMAX[idx];

                MAX indicator = new MAX();
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

                MAX[] tmp = new MAX[cacheMAX == null ? 1 : cacheMAX.Length + 1];
                if (cacheMAX != null)
                    cacheMAX.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheMAX = tmp;
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
        /// The Maximum shows the maximum of the last n bars.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MAX MAX(int period)
        {
            return _indicator.MAX(Input, period);
        }

        /// <summary>
        /// The Maximum shows the maximum of the last n bars.
        /// </summary>
        /// <returns></returns>
        public Indicator.MAX MAX(Data.IDataSeries input, int period)
        {
            return _indicator.MAX(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Maximum shows the maximum of the last n bars.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MAX MAX(int period)
        {
            return _indicator.MAX(Input, period);
        }

        /// <summary>
        /// The Maximum shows the maximum of the last n bars.
        /// </summary>
        /// <returns></returns>
        public Indicator.MAX MAX(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.MAX(input, period);
        }
    }
}
#endregion
