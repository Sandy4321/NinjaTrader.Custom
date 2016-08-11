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
    /// Custom drawn CCI
    /// </summary>
    [Description("Custom drawn CCI")]
    public class CCICustom : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period = 14; // Default setting for Period
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "Above"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "Neutral"));
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkViolet), PlotStyle.Line, "Below"));
            Add(new Line(Color.FromKnownColor(KnownColor.DarkSlateBlue), 70, "PosSeventy"));
            Add(new Line(Color.FromKnownColor(KnownColor.MediumBlue), -70, "NegSeventy"));
            CalculateOnBarClose = true;
            Overlay				= false;

            Plots[2].Pen.DashStyle = DashStyle.Dash;
            Plots[0].Min = 70;
            Plots[1].Max = 70;
            Plots[1].Min = -70;
            Plots[2].Max = -70;


        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.

            if (CurrentBar < period) return;

            double value = CCI(period)[0];

            if (value > 200)
            {
                BackColor = Color.PaleGreen;
                BarColor = Color.Yellow;
                CandleOutlineColor = Color.Black;

            }
            else if (value > 150)
            {
                BackColor = Color.PaleGreen;

            }
            else if (value < -200)
            {
                BackColor = Color.Pink;
                BarColor = Color.Yellow;
                CandleOutlineColor = Color.Black;

            }
            else if (value <-150)
            {
                BackColor = Color.Pink;

            }


            
            
            
            
            Above.Set(value);
            Neutral.Set(value);
            Below.Set(value);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Above
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Neutral
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Below
        {
            get { return Values[2]; }
        }

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
        private CCICustom[] cacheCCICustom = null;

        private static CCICustom checkCCICustom = new CCICustom();

        /// <summary>
        /// Custom drawn CCI
        /// </summary>
        /// <returns></returns>
        public CCICustom CCICustom(int period)
        {
            return CCICustom(Input, period);
        }

        /// <summary>
        /// Custom drawn CCI
        /// </summary>
        /// <returns></returns>
        public CCICustom CCICustom(Data.IDataSeries input, int period)
        {
            if (cacheCCICustom != null)
                for (int idx = 0; idx < cacheCCICustom.Length; idx++)
                    if (cacheCCICustom[idx].Period == period && cacheCCICustom[idx].EqualsInput(input))
                        return cacheCCICustom[idx];

            lock (checkCCICustom)
            {
                checkCCICustom.Period = period;
                period = checkCCICustom.Period;

                if (cacheCCICustom != null)
                    for (int idx = 0; idx < cacheCCICustom.Length; idx++)
                        if (cacheCCICustom[idx].Period == period && cacheCCICustom[idx].EqualsInput(input))
                            return cacheCCICustom[idx];

                CCICustom indicator = new CCICustom();
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

                CCICustom[] tmp = new CCICustom[cacheCCICustom == null ? 1 : cacheCCICustom.Length + 1];
                if (cacheCCICustom != null)
                    cacheCCICustom.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheCCICustom = tmp;
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
        /// Custom drawn CCI
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.CCICustom CCICustom(int period)
        {
            return _indicator.CCICustom(Input, period);
        }

        /// <summary>
        /// Custom drawn CCI
        /// </summary>
        /// <returns></returns>
        public Indicator.CCICustom CCICustom(Data.IDataSeries input, int period)
        {
            return _indicator.CCICustom(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Custom drawn CCI
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.CCICustom CCICustom(int period)
        {
            return _indicator.CCICustom(Input, period);
        }

        /// <summary>
        /// Custom drawn CCI
        /// </summary>
        /// <returns></returns>
        public Indicator.CCICustom CCICustom(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.CCICustom(input, period);
        }
    }
}
#endregion
