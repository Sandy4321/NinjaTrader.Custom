//
// Copyright (C) 2006, NinjaTrader LLC <ninjatrader@ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private MTDoubleShotStrategyInfoBar[] cacheMTDoubleShotStrategyInfoBar = null;
        private MTDoubleShotStrategyVisualiser[] cacheMTDoubleShotStrategyVisualiser = null;

        private static MTDoubleShotStrategyInfoBar checkMTDoubleShotStrategyInfoBar = new MTDoubleShotStrategyInfoBar();
        private static MTDoubleShotStrategyVisualiser checkMTDoubleShotStrategyVisualiser = new MTDoubleShotStrategyVisualiser();

        /// <summary>
        /// MicroTrends.Indicator.DoubleShotStrategyInfoBar Strategy Info bar
        /// </summary>
        /// <returns></returns>
        public MTDoubleShotStrategyInfoBar MTDoubleShotStrategyInfoBar()
        {
            return MTDoubleShotStrategyInfoBar(Input);
        }

        /// <summary>
        /// MicroTrends.Indicator.DoubleShotStrategyInfoBar Strategy Info bar
        /// </summary>
        /// <returns></returns>
        public MTDoubleShotStrategyInfoBar MTDoubleShotStrategyInfoBar(Data.IDataSeries input)
        {
            if (cacheMTDoubleShotStrategyInfoBar != null)
                for (int idx = 0; idx < cacheMTDoubleShotStrategyInfoBar.Length; idx++)
                    if (cacheMTDoubleShotStrategyInfoBar[idx].EqualsInput(input))
                        return cacheMTDoubleShotStrategyInfoBar[idx];

            lock (checkMTDoubleShotStrategyInfoBar)
            {
                if (cacheMTDoubleShotStrategyInfoBar != null)
                    for (int idx = 0; idx < cacheMTDoubleShotStrategyInfoBar.Length; idx++)
                        if (cacheMTDoubleShotStrategyInfoBar[idx].EqualsInput(input))
                            return cacheMTDoubleShotStrategyInfoBar[idx];

                MTDoubleShotStrategyInfoBar indicator = new MTDoubleShotStrategyInfoBar();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                MTDoubleShotStrategyInfoBar[] tmp = new MTDoubleShotStrategyInfoBar[cacheMTDoubleShotStrategyInfoBar == null ? 1 : cacheMTDoubleShotStrategyInfoBar.Length + 1];
                if (cacheMTDoubleShotStrategyInfoBar != null)
                    cacheMTDoubleShotStrategyInfoBar.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheMTDoubleShotStrategyInfoBar = tmp;
                return indicator;
            }
        }

        /// <summary>
        /// MTDouble Shot Strategy Visualiser
        /// </summary>
        /// <returns></returns>
        public MTDoubleShotStrategyVisualiser MTDoubleShotStrategyVisualiser()
        {
            return MTDoubleShotStrategyVisualiser(Input);
        }

        /// <summary>
        /// MTDouble Shot Strategy Visualiser
        /// </summary>
        /// <returns></returns>
        public MTDoubleShotStrategyVisualiser MTDoubleShotStrategyVisualiser(Data.IDataSeries input)
        {
            if (cacheMTDoubleShotStrategyVisualiser != null)
                for (int idx = 0; idx < cacheMTDoubleShotStrategyVisualiser.Length; idx++)
                    if (cacheMTDoubleShotStrategyVisualiser[idx].EqualsInput(input))
                        return cacheMTDoubleShotStrategyVisualiser[idx];

            lock (checkMTDoubleShotStrategyVisualiser)
            {
                if (cacheMTDoubleShotStrategyVisualiser != null)
                    for (int idx = 0; idx < cacheMTDoubleShotStrategyVisualiser.Length; idx++)
                        if (cacheMTDoubleShotStrategyVisualiser[idx].EqualsInput(input))
                            return cacheMTDoubleShotStrategyVisualiser[idx];

                MTDoubleShotStrategyVisualiser indicator = new MTDoubleShotStrategyVisualiser();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                MTDoubleShotStrategyVisualiser[] tmp = new MTDoubleShotStrategyVisualiser[cacheMTDoubleShotStrategyVisualiser == null ? 1 : cacheMTDoubleShotStrategyVisualiser.Length + 1];
                if (cacheMTDoubleShotStrategyVisualiser != null)
                    cacheMTDoubleShotStrategyVisualiser.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheMTDoubleShotStrategyVisualiser = tmp;
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
        /// MicroTrends.Indicator.DoubleShotStrategyInfoBar Strategy Info bar
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MTDoubleShotStrategyInfoBar MTDoubleShotStrategyInfoBar()
        {
            return _indicator.MTDoubleShotStrategyInfoBar(Input);
        }

        /// <summary>
        /// MicroTrends.Indicator.DoubleShotStrategyInfoBar Strategy Info bar
        /// </summary>
        /// <returns></returns>
        public Indicator.MTDoubleShotStrategyInfoBar MTDoubleShotStrategyInfoBar(Data.IDataSeries input)
        {
            return _indicator.MTDoubleShotStrategyInfoBar(input);
        }

        /// <summary>
        /// MTDouble Shot Strategy Visualiser
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MTDoubleShotStrategyVisualiser MTDoubleShotStrategyVisualiser()
        {
            return _indicator.MTDoubleShotStrategyVisualiser(Input);
        }

        /// <summary>
        /// MTDouble Shot Strategy Visualiser
        /// </summary>
        /// <returns></returns>
        public Indicator.MTDoubleShotStrategyVisualiser MTDoubleShotStrategyVisualiser(Data.IDataSeries input)
        {
            return _indicator.MTDoubleShotStrategyVisualiser(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// MicroTrends.Indicator.DoubleShotStrategyInfoBar Strategy Info bar
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MTDoubleShotStrategyInfoBar MTDoubleShotStrategyInfoBar()
        {
            return _indicator.MTDoubleShotStrategyInfoBar(Input);
        }

        /// <summary>
        /// MicroTrends.Indicator.DoubleShotStrategyInfoBar Strategy Info bar
        /// </summary>
        /// <returns></returns>
        public Indicator.MTDoubleShotStrategyInfoBar MTDoubleShotStrategyInfoBar(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.MTDoubleShotStrategyInfoBar(input);
        }

        /// <summary>
        /// MTDouble Shot Strategy Visualiser
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MTDoubleShotStrategyVisualiser MTDoubleShotStrategyVisualiser()
        {
            return _indicator.MTDoubleShotStrategyVisualiser(Input);
        }

        /// <summary>
        /// MTDouble Shot Strategy Visualiser
        /// </summary>
        /// <returns></returns>
        public Indicator.MTDoubleShotStrategyVisualiser MTDoubleShotStrategyVisualiser(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.MTDoubleShotStrategyVisualiser(input);
        }
    }
}
#endregion
