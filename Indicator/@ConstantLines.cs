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
    /// Plots lines at user  defined values.
    /// </summary>
    [Description("Plots lines at user defined values. Lines with values of zero will not plot.")]
    public class ConstantLines : Indicator
    {
        #region Variables

        // Wizard generated variables
            private double line1Value = 0; // Default setting for Line1Value
            private double line2Value = 0; // Default setting for Line2Value
            private double line3Value = 0; // Default setting for Line3Value
            private double line4Value = 0; // Default setting for Line4Value
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Navy, PlotStyle.Line, "Line1"));
            Add(new Plot(Color.Green, PlotStyle.Line, "Line2"));
            Add(new Plot(Color.DarkViolet, PlotStyle.Line, "Line3"));
            Add(new Plot(Color.Firebrick, PlotStyle.Line, "Line4"));

			ChartOnly			= true;
            AutoScale			= false;
			CalculateOnBarClose	= true;
			DisplayInDataBox	= false;
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (Line1Value != 0) Line1.Set(Line1Value);
            if (Line2Value != 0) Line2.Set(Line2Value);
            if (Line3Value != 0) Line3.Set(Line3Value);
            if (Line4Value != 0) Line4.Set(Line4Value);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Line1
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Line2
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Line3
        {
            get { return Values[2]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Line4
        {
            get { return Values[3]; }
        }

        [Description("Line 1 value")]
        [GridCategory("Parameters")]
        public double Line1Value
        {
            get { return line1Value; }
            set { line1Value = value; }
        }

        [Description("Line 2 value")]
        [GridCategory("Parameters")]
        public double Line2Value
        {
            get { return line2Value; }
            set { line2Value = value; }
        }

        [Description("Line 3 value")]
        [GridCategory("Parameters")]
        public double Line3Value
        {
            get { return line3Value; }
            set { line3Value = value; }
        }

        [Description("Line 4 Value")]
        [GridCategory("Parameters")]
        public double Line4Value
        {
            get { return line4Value; }
            set { line4Value = value; }
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
        private ConstantLines[] cacheConstantLines = null;

        private static ConstantLines checkConstantLines = new ConstantLines();

        /// <summary>
        /// Plots lines at user defined values. Lines with values of zero will not plot.
        /// </summary>
        /// <returns></returns>
        public ConstantLines ConstantLines(double line1Value, double line2Value, double line3Value, double line4Value)
        {
            return ConstantLines(Input, line1Value, line2Value, line3Value, line4Value);
        }

        /// <summary>
        /// Plots lines at user defined values. Lines with values of zero will not plot.
        /// </summary>
        /// <returns></returns>
        public ConstantLines ConstantLines(Data.IDataSeries input, double line1Value, double line2Value, double line3Value, double line4Value)
        {
            if (cacheConstantLines != null)
                for (int idx = 0; idx < cacheConstantLines.Length; idx++)
                    if (Math.Abs(cacheConstantLines[idx].Line1Value - line1Value) <= double.Epsilon && Math.Abs(cacheConstantLines[idx].Line2Value - line2Value) <= double.Epsilon && Math.Abs(cacheConstantLines[idx].Line3Value - line3Value) <= double.Epsilon && Math.Abs(cacheConstantLines[idx].Line4Value - line4Value) <= double.Epsilon && cacheConstantLines[idx].EqualsInput(input))
                        return cacheConstantLines[idx];

            lock (checkConstantLines)
            {
                checkConstantLines.Line1Value = line1Value;
                line1Value = checkConstantLines.Line1Value;
                checkConstantLines.Line2Value = line2Value;
                line2Value = checkConstantLines.Line2Value;
                checkConstantLines.Line3Value = line3Value;
                line3Value = checkConstantLines.Line3Value;
                checkConstantLines.Line4Value = line4Value;
                line4Value = checkConstantLines.Line4Value;

                if (cacheConstantLines != null)
                    for (int idx = 0; idx < cacheConstantLines.Length; idx++)
                        if (Math.Abs(cacheConstantLines[idx].Line1Value - line1Value) <= double.Epsilon && Math.Abs(cacheConstantLines[idx].Line2Value - line2Value) <= double.Epsilon && Math.Abs(cacheConstantLines[idx].Line3Value - line3Value) <= double.Epsilon && Math.Abs(cacheConstantLines[idx].Line4Value - line4Value) <= double.Epsilon && cacheConstantLines[idx].EqualsInput(input))
                            return cacheConstantLines[idx];

                ConstantLines indicator = new ConstantLines();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Line1Value = line1Value;
                indicator.Line2Value = line2Value;
                indicator.Line3Value = line3Value;
                indicator.Line4Value = line4Value;
                Indicators.Add(indicator);
                indicator.SetUp();

                ConstantLines[] tmp = new ConstantLines[cacheConstantLines == null ? 1 : cacheConstantLines.Length + 1];
                if (cacheConstantLines != null)
                    cacheConstantLines.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheConstantLines = tmp;
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
        /// Plots lines at user defined values. Lines with values of zero will not plot.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ConstantLines ConstantLines(double line1Value, double line2Value, double line3Value, double line4Value)
        {
            return _indicator.ConstantLines(Input, line1Value, line2Value, line3Value, line4Value);
        }

        /// <summary>
        /// Plots lines at user defined values. Lines with values of zero will not plot.
        /// </summary>
        /// <returns></returns>
        public Indicator.ConstantLines ConstantLines(Data.IDataSeries input, double line1Value, double line2Value, double line3Value, double line4Value)
        {
            return _indicator.ConstantLines(input, line1Value, line2Value, line3Value, line4Value);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Plots lines at user defined values. Lines with values of zero will not plot.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ConstantLines ConstantLines(double line1Value, double line2Value, double line3Value, double line4Value)
        {
            return _indicator.ConstantLines(Input, line1Value, line2Value, line3Value, line4Value);
        }

        /// <summary>
        /// Plots lines at user defined values. Lines with values of zero will not plot.
        /// </summary>
        /// <returns></returns>
        public Indicator.ConstantLines ConstantLines(Data.IDataSeries input, double line1Value, double line2Value, double line3Value, double line4Value)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ConstantLines(input, line1Value, line2Value, line3Value, line4Value);
        }
    }
}
#endregion
