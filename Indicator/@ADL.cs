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
	/// The Accumulation/Distribution (AD) study attempts to quantify the amount of volume flowing into or out of an instrument by identifying the position of the close of the period in relation to that period�s high/low range.
    /// </summary>
    [Description("The Accumulation/Distribution (AD) study attempts to quantify the amount of volume flowing into or out of an instrument by identifying the position of the close of the period in relation to that period�s high/low range.")]
    public class ADL : Indicator
    {
        #region Variables
        // Wizard generated variables
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Green, PlotStyle.Line, "AD"));
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            AD.Set((CurrentBar == 0 ? 0 : AD[1]) + (High[0] != Low[0] ? (((Close[0] - Low[0]) - (High[0] - Close[0])) / (High[0] - Low[0])) * Volume[0] : 0));
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries AD
        {
            get { return Values[0]; }
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
        private ADL[] cacheADL = null;

        private static ADL checkADL = new ADL();

        /// <summary>
        /// The Accumulation/Distribution (AD) study attempts to quantify the amount of volume flowing into or out of an instrument by identifying the position of the close of the period in relation to that period�s high/low range.
        /// </summary>
        /// <returns></returns>
        public ADL ADL()
        {
            return ADL(Input);
        }

        /// <summary>
        /// The Accumulation/Distribution (AD) study attempts to quantify the amount of volume flowing into or out of an instrument by identifying the position of the close of the period in relation to that period�s high/low range.
        /// </summary>
        /// <returns></returns>
        public ADL ADL(Data.IDataSeries input)
        {
            if (cacheADL != null)
                for (int idx = 0; idx < cacheADL.Length; idx++)
                    if (cacheADL[idx].EqualsInput(input))
                        return cacheADL[idx];

            lock (checkADL)
            {
                if (cacheADL != null)
                    for (int idx = 0; idx < cacheADL.Length; idx++)
                        if (cacheADL[idx].EqualsInput(input))
                            return cacheADL[idx];

                ADL indicator = new ADL();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                ADL[] tmp = new ADL[cacheADL == null ? 1 : cacheADL.Length + 1];
                if (cacheADL != null)
                    cacheADL.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheADL = tmp;
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
        /// The Accumulation/Distribution (AD) study attempts to quantify the amount of volume flowing into or out of an instrument by identifying the position of the close of the period in relation to that period�s high/low range.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ADL ADL()
        {
            return _indicator.ADL(Input);
        }

        /// <summary>
        /// The Accumulation/Distribution (AD) study attempts to quantify the amount of volume flowing into or out of an instrument by identifying the position of the close of the period in relation to that period�s high/low range.
        /// </summary>
        /// <returns></returns>
        public Indicator.ADL ADL(Data.IDataSeries input)
        {
            return _indicator.ADL(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Accumulation/Distribution (AD) study attempts to quantify the amount of volume flowing into or out of an instrument by identifying the position of the close of the period in relation to that period�s high/low range.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ADL ADL()
        {
            return _indicator.ADL(Input);
        }

        /// <summary>
        /// The Accumulation/Distribution (AD) study attempts to quantify the amount of volume flowing into or out of an instrument by identifying the position of the close of the period in relation to that period�s high/low range.
        /// </summary>
        /// <returns></returns>
        public Indicator.ADL ADL(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ADL(input);
        }
    }
}
#endregion
