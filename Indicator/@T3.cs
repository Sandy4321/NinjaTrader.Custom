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
    /// T3 Moving Average
    /// </summary>
    [Description("T3 Moving Average")]
    public class T3 : Indicator
    {
        #region Variables

            private double vFactor = 0.7; // Default setting for VFactor
			private int tCount = 3;
			private int period = 14;
			private System.Collections.ArrayList seriesCollection;

        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "T3"));
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (TCount == 1)
			{
				CalculateGD(Inputs[0], Values[0]);
				return;
			}
				
			if (seriesCollection == null)
			{
				seriesCollection = new System.Collections.ArrayList();
				for (int i = 0; i < TCount - 1; i++) 
					seriesCollection.Add(new DataSeries(this));
			}
			
			CalculateGD(Inputs[0], (DataSeries) seriesCollection[0]);
			
			for (int i = 0; i <= seriesCollection.Count - 2; i++) 
				CalculateGD((DataSeries) seriesCollection[i], (DataSeries) seriesCollection[i + 1]);
			
			CalculateGD((DataSeries) seriesCollection[seriesCollection.Count - 1], Values[0]);
         }
		
		private void CalculateGD(IDataSeries input, DataSeries output)
		{
			output.Set((EMA(input, Period)[0] * (1 + VFactor)) - (EMA(EMA(input, Period), Period)[0] * VFactor));
		}
		
        #region Properties
		[Description("Numbers of bars used for calculations")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }
		
		[Description("The smooth count")]
        [GridCategory("Parameters")]
        public int TCount
        {
            get { return tCount; }
            set { tCount = Math.Max(1, value); }
        }

        [Description("VFactor")]
        [GridCategory("Parameters")]
        public double VFactor
        {
            get { return vFactor; }
            set { vFactor = Math.Max(0, value); }
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
        private T3[] cacheT3 = null;

        private static T3 checkT3 = new T3();

        /// <summary>
        /// T3 Moving Average
        /// </summary>
        /// <returns></returns>
        public T3 T3(int period, int tCount, double vFactor)
        {
            return T3(Input, period, tCount, vFactor);
        }

        /// <summary>
        /// T3 Moving Average
        /// </summary>
        /// <returns></returns>
        public T3 T3(Data.IDataSeries input, int period, int tCount, double vFactor)
        {
            if (cacheT3 != null)
                for (int idx = 0; idx < cacheT3.Length; idx++)
                    if (cacheT3[idx].Period == period && cacheT3[idx].TCount == tCount && Math.Abs(cacheT3[idx].VFactor - vFactor) <= double.Epsilon && cacheT3[idx].EqualsInput(input))
                        return cacheT3[idx];

            lock (checkT3)
            {
                checkT3.Period = period;
                period = checkT3.Period;
                checkT3.TCount = tCount;
                tCount = checkT3.TCount;
                checkT3.VFactor = vFactor;
                vFactor = checkT3.VFactor;

                if (cacheT3 != null)
                    for (int idx = 0; idx < cacheT3.Length; idx++)
                        if (cacheT3[idx].Period == period && cacheT3[idx].TCount == tCount && Math.Abs(cacheT3[idx].VFactor - vFactor) <= double.Epsilon && cacheT3[idx].EqualsInput(input))
                            return cacheT3[idx];

                T3 indicator = new T3();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                indicator.TCount = tCount;
                indicator.VFactor = vFactor;
                Indicators.Add(indicator);
                indicator.SetUp();

                T3[] tmp = new T3[cacheT3 == null ? 1 : cacheT3.Length + 1];
                if (cacheT3 != null)
                    cacheT3.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheT3 = tmp;
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
        /// T3 Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.T3 T3(int period, int tCount, double vFactor)
        {
            return _indicator.T3(Input, period, tCount, vFactor);
        }

        /// <summary>
        /// T3 Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.T3 T3(Data.IDataSeries input, int period, int tCount, double vFactor)
        {
            return _indicator.T3(input, period, tCount, vFactor);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// T3 Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.T3 T3(int period, int tCount, double vFactor)
        {
            return _indicator.T3(Input, period, tCount, vFactor);
        }

        /// <summary>
        /// T3 Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.T3 T3(Data.IDataSeries input, int period, int tCount, double vFactor)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.T3(input, period, tCount, vFactor);
        }
    }
}
#endregion
