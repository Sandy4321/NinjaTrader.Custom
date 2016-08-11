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
	public class StochasticsFast : Indicator
	{
		#region Variables
		private int					periodD	= 3;
		private int					periodK	= 14;
		private DataSeries			den;
		private DataSeries			nom;
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

			den		= new DataSeries(this);
			nom		= new DataSeries(this);
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
            nom.Set(Close[0] - MIN(Low, PeriodK)[0]);
            den.Set(MAX(High, PeriodK)[0] - MIN(Low, PeriodK)[0]);

            if (den[0].Compare(0, 0.000000000001) == 0)
                K.Set(CurrentBar == 0 ? 50 : K[1]);
            else
                K.Set(Math.Min(100, Math.Max(0, 100 * nom[0] / den[0])));

            D.Set(SMA(K, PeriodD)[0]);
        }

		#region Properties
		/// <summary>
		/// Gets the fast D value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries D
		{
			get { return Values[0]; }
		}

		/// <summary>
		/// Gets the fast K value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries K
		{
			get { return Values[1]; }
		}
		
		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for the moving average over K values")]
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
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private StochasticsFast[] cacheStochasticsFast = null;

        private static StochasticsFast checkStochasticsFast = new StochasticsFast();

        /// <summary>
        /// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
        /// </summary>
        /// <returns></returns>
        public StochasticsFast StochasticsFast(int periodD, int periodK)
        {
            return StochasticsFast(Input, periodD, periodK);
        }

        /// <summary>
        /// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
        /// </summary>
        /// <returns></returns>
        public StochasticsFast StochasticsFast(Data.IDataSeries input, int periodD, int periodK)
        {
            if (cacheStochasticsFast != null)
                for (int idx = 0; idx < cacheStochasticsFast.Length; idx++)
                    if (cacheStochasticsFast[idx].PeriodD == periodD && cacheStochasticsFast[idx].PeriodK == periodK && cacheStochasticsFast[idx].EqualsInput(input))
                        return cacheStochasticsFast[idx];

            lock (checkStochasticsFast)
            {
                checkStochasticsFast.PeriodD = periodD;
                periodD = checkStochasticsFast.PeriodD;
                checkStochasticsFast.PeriodK = periodK;
                periodK = checkStochasticsFast.PeriodK;

                if (cacheStochasticsFast != null)
                    for (int idx = 0; idx < cacheStochasticsFast.Length; idx++)
                        if (cacheStochasticsFast[idx].PeriodD == periodD && cacheStochasticsFast[idx].PeriodK == periodK && cacheStochasticsFast[idx].EqualsInput(input))
                            return cacheStochasticsFast[idx];

                StochasticsFast indicator = new StochasticsFast();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.PeriodD = periodD;
                indicator.PeriodK = periodK;
                Indicators.Add(indicator);
                indicator.SetUp();

                StochasticsFast[] tmp = new StochasticsFast[cacheStochasticsFast == null ? 1 : cacheStochasticsFast.Length + 1];
                if (cacheStochasticsFast != null)
                    cacheStochasticsFast.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheStochasticsFast = tmp;
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
        public Indicator.StochasticsFast StochasticsFast(int periodD, int periodK)
        {
            return _indicator.StochasticsFast(Input, periodD, periodK);
        }

        /// <summary>
        /// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
        /// </summary>
        /// <returns></returns>
        public Indicator.StochasticsFast StochasticsFast(Data.IDataSeries input, int periodD, int periodK)
        {
            return _indicator.StochasticsFast(input, periodD, periodK);
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
        public Indicator.StochasticsFast StochasticsFast(int periodD, int periodK)
        {
            return _indicator.StochasticsFast(Input, periodD, periodK);
        }

        /// <summary>
        /// The Stochastic Oscillator is made up of two lines that oscillate between a vertical scale of 0 to 100. The %K is the main line and it is drawn as a solid line. The second is the %D line and is a moving average of %K. The %D line is drawn as a dotted line. Use as a buy/sell signal generator, buying when fast moves above slow and selling when fast moves below slow.
        /// </summary>
        /// <returns></returns>
        public Indicator.StochasticsFast StochasticsFast(Data.IDataSeries input, int periodD, int periodK)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.StochasticsFast(input, periodD, periodK);
        }
    }
}
#endregion
