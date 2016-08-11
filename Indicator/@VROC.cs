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
	/// The VROC (Volume Rate-of-Change) shows whether or not a volume trend is developing in either an up or down direction. It is similar to the ROC indicator, but is applied to volume instead.
	/// </summary>
	[Description("The VROC (Volume Rate-of-Change) shows whether or not a volume trend is developing in either an up or down direction. It is similar to the ROC indicator, but is applied to volume instead.")]
	public class VROC : Indicator
	{
		#region Variables
		private int					period	= 14;
		private	int					smooth	= 3;
		private	DataSeries		smaVolume;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Line(Color.DarkGray, 0, "Zero line"));
			Add(new Plot(Color.Orange, "VROC"));

			smaVolume			= new DataSeries(this);
			
			Overlay				= false;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			double back = Volume[Math.Min(CurrentBar, Period - 1)];
			smaVolume.Set((100 * Volume[0] / (back == 0 ? 1 : back)) - 100);
			Value.Set(SMA(smaVolume, Smooth)[0]);
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

		/// <summary>
		/// </summary>
		[Description("Number of bars for smoothing")]
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
        private VROC[] cacheVROC = null;

        private static VROC checkVROC = new VROC();

        /// <summary>
        /// The VROC (Volume Rate-of-Change) shows whether or not a volume trend is developing in either an up or down direction. It is similar to the ROC indicator, but is applied to volume instead.
        /// </summary>
        /// <returns></returns>
        public VROC VROC(int period, int smooth)
        {
            return VROC(Input, period, smooth);
        }

        /// <summary>
        /// The VROC (Volume Rate-of-Change) shows whether or not a volume trend is developing in either an up or down direction. It is similar to the ROC indicator, but is applied to volume instead.
        /// </summary>
        /// <returns></returns>
        public VROC VROC(Data.IDataSeries input, int period, int smooth)
        {
            if (cacheVROC != null)
                for (int idx = 0; idx < cacheVROC.Length; idx++)
                    if (cacheVROC[idx].Period == period && cacheVROC[idx].Smooth == smooth && cacheVROC[idx].EqualsInput(input))
                        return cacheVROC[idx];

            lock (checkVROC)
            {
                checkVROC.Period = period;
                period = checkVROC.Period;
                checkVROC.Smooth = smooth;
                smooth = checkVROC.Smooth;

                if (cacheVROC != null)
                    for (int idx = 0; idx < cacheVROC.Length; idx++)
                        if (cacheVROC[idx].Period == period && cacheVROC[idx].Smooth == smooth && cacheVROC[idx].EqualsInput(input))
                            return cacheVROC[idx];

                VROC indicator = new VROC();
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

                VROC[] tmp = new VROC[cacheVROC == null ? 1 : cacheVROC.Length + 1];
                if (cacheVROC != null)
                    cacheVROC.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheVROC = tmp;
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
        /// The VROC (Volume Rate-of-Change) shows whether or not a volume trend is developing in either an up or down direction. It is similar to the ROC indicator, but is applied to volume instead.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VROC VROC(int period, int smooth)
        {
            return _indicator.VROC(Input, period, smooth);
        }

        /// <summary>
        /// The VROC (Volume Rate-of-Change) shows whether or not a volume trend is developing in either an up or down direction. It is similar to the ROC indicator, but is applied to volume instead.
        /// </summary>
        /// <returns></returns>
        public Indicator.VROC VROC(Data.IDataSeries input, int period, int smooth)
        {
            return _indicator.VROC(input, period, smooth);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The VROC (Volume Rate-of-Change) shows whether or not a volume trend is developing in either an up or down direction. It is similar to the ROC indicator, but is applied to volume instead.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VROC VROC(int period, int smooth)
        {
            return _indicator.VROC(Input, period, smooth);
        }

        /// <summary>
        /// The VROC (Volume Rate-of-Change) shows whether or not a volume trend is developing in either an up or down direction. It is similar to the ROC indicator, but is applied to volume instead.
        /// </summary>
        /// <returns></returns>
        public Indicator.VROC VROC(Data.IDataSeries input, int period, int smooth)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.VROC(input, period, smooth);
        }
    }
}
#endregion
