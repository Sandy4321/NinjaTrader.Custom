// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
// Reference to "Trading with Adaptive Price Zone" article in S&C, September 2006, p. 28 by Lee Leibfarth.
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
    /// The APZ (Adaptive Prize Zone) forms a steady channel based on double smoothed exponential moving averages around the average price. See S/C, September 2006, p.28.
    /// </summary>
    [Description("The APZ (Adaptive Prize Zone) forms a steady channel based on double smoothed exponential moving averages around the average price.")]
    public class APZ : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double bandPct = 2; // Default setting for BandPct
            private int period = 20; // Default setting for Period
			private int newPeriod = 0;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.OrangeRed, PlotStyle.Line, "Lower"));
            Add(new Plot(Color.OrangeRed, PlotStyle.Line, "Upper"));
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            if (CurrentBar < Period)
				return;

			EMA ema = EMA(EMA(newPeriod), newPeriod);
			double rangeOffset = BandPct * EMA(Range(), Period)[0];
			Lower.Set(ema[0] - rangeOffset);
            Upper.Set(ema[0] + rangeOffset);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Lower
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Upper
        {
            get { return Values[1]; }
        }

        [Description("Deviation factor")]
        [GridCategory("Parameters")]
        public double BandPct
        {
            get { return bandPct; }
            set { bandPct = Math.Max(1, value); }
        }

        [Description("Number of bars used  for calculations")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set 
			{ 
				period = Math.Max(1, value);
				newPeriod = Convert.ToInt32(Math.Sqrt(Convert.ToDouble(value)));
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
        private APZ[] cacheAPZ = null;

        private static APZ checkAPZ = new APZ();

        /// <summary>
        /// The APZ (Adaptive Prize Zone) forms a steady channel based on double smoothed exponential moving averages around the average price.
        /// </summary>
        /// <returns></returns>
        public APZ APZ(double bandPct, int period)
        {
            return APZ(Input, bandPct, period);
        }

        /// <summary>
        /// The APZ (Adaptive Prize Zone) forms a steady channel based on double smoothed exponential moving averages around the average price.
        /// </summary>
        /// <returns></returns>
        public APZ APZ(Data.IDataSeries input, double bandPct, int period)
        {
            if (cacheAPZ != null)
                for (int idx = 0; idx < cacheAPZ.Length; idx++)
                    if (Math.Abs(cacheAPZ[idx].BandPct - bandPct) <= double.Epsilon && cacheAPZ[idx].Period == period && cacheAPZ[idx].EqualsInput(input))
                        return cacheAPZ[idx];

            lock (checkAPZ)
            {
                checkAPZ.BandPct = bandPct;
                bandPct = checkAPZ.BandPct;
                checkAPZ.Period = period;
                period = checkAPZ.Period;

                if (cacheAPZ != null)
                    for (int idx = 0; idx < cacheAPZ.Length; idx++)
                        if (Math.Abs(cacheAPZ[idx].BandPct - bandPct) <= double.Epsilon && cacheAPZ[idx].Period == period && cacheAPZ[idx].EqualsInput(input))
                            return cacheAPZ[idx];

                APZ indicator = new APZ();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BandPct = bandPct;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                APZ[] tmp = new APZ[cacheAPZ == null ? 1 : cacheAPZ.Length + 1];
                if (cacheAPZ != null)
                    cacheAPZ.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheAPZ = tmp;
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
        /// The APZ (Adaptive Prize Zone) forms a steady channel based on double smoothed exponential moving averages around the average price.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.APZ APZ(double bandPct, int period)
        {
            return _indicator.APZ(Input, bandPct, period);
        }

        /// <summary>
        /// The APZ (Adaptive Prize Zone) forms a steady channel based on double smoothed exponential moving averages around the average price.
        /// </summary>
        /// <returns></returns>
        public Indicator.APZ APZ(Data.IDataSeries input, double bandPct, int period)
        {
            return _indicator.APZ(input, bandPct, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The APZ (Adaptive Prize Zone) forms a steady channel based on double smoothed exponential moving averages around the average price.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.APZ APZ(double bandPct, int period)
        {
            return _indicator.APZ(Input, bandPct, period);
        }

        /// <summary>
        /// The APZ (Adaptive Prize Zone) forms a steady channel based on double smoothed exponential moving averages around the average price.
        /// </summary>
        /// <returns></returns>
        public Indicator.APZ APZ(Data.IDataSeries input, double bandPct, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.APZ(input, bandPct, period);
        }
    }
}
#endregion
