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
    /// ROC with custom line color options
    /// </summary>
    [Description("ROC with custom line color options")]
    public class CustomROC : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period = 14; // Default setting for Period
            private int smooth = 3; // Default setting for Smooth
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.SeaGreen), PlotStyle.Line, "AboveZero"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Firebrick), PlotStyle.Line, "BelowZero"));
            Add(new Line(Color.FromKnownColor(KnownColor.DarkOliveGreen), 0, "ZeroLine"));
            Overlay				= false;
            CalculateOnBarClose = true;

            Plots[0].Min = 0;
            Plots[1].Max = 0;

        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            if (CurrentBar<period)
            {
                return;
            }


            AboveZero.Set(ROC(period)[0]);
            BelowZero.Set(ROC(period)[0]);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries AboveZero
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries BelowZero
        {
            get { return Values[1]; }
        }

        [Description("Number of periods")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }

        [Description("Smoothing rate")]
        [GridCategory("Parameters")]
        public int Smooth
        {
            get { return smooth; }
            set { smooth = Math.Max(1, value); }
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
        private CustomROC[] cacheCustomROC = null;

        private static CustomROC checkCustomROC = new CustomROC();

        /// <summary>
        /// ROC with custom line color options
        /// </summary>
        /// <returns></returns>
        public CustomROC CustomROC(int period, int smooth)
        {
            return CustomROC(Input, period, smooth);
        }

        /// <summary>
        /// ROC with custom line color options
        /// </summary>
        /// <returns></returns>
        public CustomROC CustomROC(Data.IDataSeries input, int period, int smooth)
        {
            if (cacheCustomROC != null)
                for (int idx = 0; idx < cacheCustomROC.Length; idx++)
                    if (cacheCustomROC[idx].Period == period && cacheCustomROC[idx].Smooth == smooth && cacheCustomROC[idx].EqualsInput(input))
                        return cacheCustomROC[idx];

            lock (checkCustomROC)
            {
                checkCustomROC.Period = period;
                period = checkCustomROC.Period;
                checkCustomROC.Smooth = smooth;
                smooth = checkCustomROC.Smooth;

                if (cacheCustomROC != null)
                    for (int idx = 0; idx < cacheCustomROC.Length; idx++)
                        if (cacheCustomROC[idx].Period == period && cacheCustomROC[idx].Smooth == smooth && cacheCustomROC[idx].EqualsInput(input))
                            return cacheCustomROC[idx];

                CustomROC indicator = new CustomROC();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                indicator.Smooth = smooth;
                Indicators.Add(indicator);
                indicator.SetUp();

                CustomROC[] tmp = new CustomROC[cacheCustomROC == null ? 1 : cacheCustomROC.Length + 1];
                if (cacheCustomROC != null)
                    cacheCustomROC.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheCustomROC = tmp;
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
        /// ROC with custom line color options
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.CustomROC CustomROC(int period, int smooth)
        {
            return _indicator.CustomROC(Input, period, smooth);
        }

        /// <summary>
        /// ROC with custom line color options
        /// </summary>
        /// <returns></returns>
        public Indicator.CustomROC CustomROC(Data.IDataSeries input, int period, int smooth)
        {
            return _indicator.CustomROC(input, period, smooth);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// ROC with custom line color options
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.CustomROC CustomROC(int period, int smooth)
        {
            return _indicator.CustomROC(Input, period, smooth);
        }

        /// <summary>
        /// ROC with custom line color options
        /// </summary>
        /// <returns></returns>
        public Indicator.CustomROC CustomROC(Data.IDataSeries input, int period, int smooth)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.CustomROC(input, period, smooth);
        }
    }
}
#endregion
