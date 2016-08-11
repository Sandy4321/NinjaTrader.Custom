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
	/// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
	/// </summary>
	[Description("The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.")]
	public class Stochastics : Indicator
	{
		#region Variables
		private int				periodD	= 7;	// SlowDperiod
		private int				periodK	= 14;	// Kperiod
		private int				smooth	= 3;	// SlowKperiod
		private DataSeries		den;
		private DataSeries		nom;
        private DataSeries      fastK;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Green, "D"));
			Add(new Plot(Color.Orange, "K"));

			Add(new Line(Color.DarkViolet, 20, "Lower"));
			Add(new Line(Color.YellowGreen, 80, "Upper"));

			den			= new DataSeries(this);
			nom			= new DataSeries(this);
            fastK       = new DataSeries(this);
		}

		/// <summary>
		/// Calculates the indicator value(s) at the current index.
		/// </summary>
		protected override void OnBarUpdate()
		{
            nom.Set(Close[0] - MIN(Low, PeriodK)[0]);
            den.Set(MAX(High, PeriodK)[0] - MIN(Low, PeriodK)[0]);

            if (den[0].Compare(0, 0.000000000001) == 0)
                fastK.Set(CurrentBar == 0 ? 50 : fastK[1]);
            else
                fastK.Set(Math.Min(100, Math.Max(0, 100 * nom[0] / den[0])));

            // Slow %K == Fast %D
            K.Set(SMA(fastK, Smooth)[0]);
            D.Set(SMA(K, PeriodD)[0]);
        }

		#region Properties
		/// <summary>
		/// Gets the slow D value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries D
		{
			get { return Values[0]; }
		}

		/// <summary>
		/// Gets the slow K value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries K
		{
			get { return Values[1]; }
		}
		
		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for moving average over K values")]
		[GridCategory("Parameters")]
		public int PeriodD
		{
			get { return periodD; }
			set { periodD = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculating the K values")]
		[GridCategory("Parameters")]
		public int PeriodK
		{
			get { return periodK; }
			set { periodK = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Number of bars for smoothing the slow K values")]
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
        private Stochastics[] cacheStochastics = null;

        private static Stochastics checkStochastics = new Stochastics();

        /// <summary>
        /// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
        /// </summary>
        /// <returns></returns>
        public Stochastics Stochastics(int periodD, int periodK, int smooth)
        {
            return Stochastics(Input, periodD, periodK, smooth);
        }

        /// <summary>
        /// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
        /// </summary>
        /// <returns></returns>
        public Stochastics Stochastics(Data.IDataSeries input, int periodD, int periodK, int smooth)
        {
            if (cacheStochastics != null)
                for (int idx = 0; idx < cacheStochastics.Length; idx++)
                    if (cacheStochastics[idx].PeriodD == periodD && cacheStochastics[idx].PeriodK == periodK && cacheStochastics[idx].Smooth == smooth && cacheStochastics[idx].EqualsInput(input))
                        return cacheStochastics[idx];

            lock (checkStochastics)
            {
                checkStochastics.PeriodD = periodD;
                periodD = checkStochastics.PeriodD;
                checkStochastics.PeriodK = periodK;
                periodK = checkStochastics.PeriodK;
                checkStochastics.Smooth = smooth;
                smooth = checkStochastics.Smooth;

                if (cacheStochastics != null)
                    for (int idx = 0; idx < cacheStochastics.Length; idx++)
                        if (cacheStochastics[idx].PeriodD == periodD && cacheStochastics[idx].PeriodK == periodK && cacheStochastics[idx].Smooth == smooth && cacheStochastics[idx].EqualsInput(input))
                            return cacheStochastics[idx];

                Stochastics indicator = new Stochastics();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.PeriodD = periodD;
                indicator.PeriodK = periodK;
                indicator.Smooth = smooth;
                Indicators.Add(indicator);
                indicator.SetUp();

                Stochastics[] tmp = new Stochastics[cacheStochastics == null ? 1 : cacheStochastics.Length + 1];
                if (cacheStochastics != null)
                    cacheStochastics.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheStochastics = tmp;
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
        /// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Stochastics Stochastics(int periodD, int periodK, int smooth)
        {
            return _indicator.Stochastics(Input, periodD, periodK, smooth);
        }

        /// <summary>
        /// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
        /// </summary>
        /// <returns></returns>
        public Indicator.Stochastics Stochastics(Data.IDataSeries input, int periodD, int periodK, int smooth)
        {
            return _indicator.Stochastics(input, periodD, periodK, smooth);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Stochastics Stochastics(int periodD, int periodK, int smooth)
        {
            return _indicator.Stochastics(Input, periodD, periodK, smooth);
        }

        /// <summary>
        /// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
        /// </summary>
        /// <returns></returns>
        public Indicator.Stochastics Stochastics(Data.IDataSeries input, int periodD, int periodK, int smooth)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Stochastics(input, periodD, periodK, smooth);
        }
    }
}
#endregion
