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
	/// The TRIX (Triple Exponential Average) displays the percentage Rate of Change (ROC) of a triple EMA. Trix oscillates above and below the zero value. The indicator applies triple smoothing in an attempt to eliminate insignificant price movements within the trend that you're trying to isolate.
	/// </summary>
	[Description("The TRIX (Triple Exponential Average) displays the percentage Rate of Change (ROC) of a triple EMA. Trix oscillates above and below the zero value. The indicator applies triple smoothing in an attempt to eliminate insignificant price movements within the trend that you're trying to isolate.")]
	public class TRIX : Indicator
	{
		#region Variables
		private int	period			= 14;
		private int	signalPeriod	= 3;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Black, "Default"));
			Add(new Plot(Color.Red,   "Signal"));

			Add(new Line(Color.DarkGray, 0, "Zero line"));
		}

		/// <summary>
		/// Calculates the indicator value(s) at the current index.
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0) 
			{
				Value.Set(Input[0]);
				return;
			}

			EMA tripleEma = EMA(EMA(EMA(Inputs[0], period), period), period);
			double trix = 100 * ((tripleEma[0] - tripleEma[1]) / tripleEma[0]);

			Default.Set(trix);
			Signal.Set(EMA(Default, signalPeriod)[0]);
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculations.")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
		}

		[Description("Period for the signal line.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Signal period")]
		public int SignalPeriod
		{
			get { return signalPeriod; }
			set { signalPeriod = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Signal
		{
			get { return Values[1]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Default
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
        private TRIX[] cacheTRIX = null;

        private static TRIX checkTRIX = new TRIX();

        /// <summary>
        /// The TRIX (Triple Exponential Average) displays the percentage Rate of Change (ROC) of a triple EMA. Trix oscillates above and below the zero value. The indicator applies triple smoothing in an attempt to eliminate insignificant price movements within the trend that you're trying to isolate.
        /// </summary>
        /// <returns></returns>
        public TRIX TRIX(int period, int signalPeriod)
        {
            return TRIX(Input, period, signalPeriod);
        }

        /// <summary>
        /// The TRIX (Triple Exponential Average) displays the percentage Rate of Change (ROC) of a triple EMA. Trix oscillates above and below the zero value. The indicator applies triple smoothing in an attempt to eliminate insignificant price movements within the trend that you're trying to isolate.
        /// </summary>
        /// <returns></returns>
        public TRIX TRIX(Data.IDataSeries input, int period, int signalPeriod)
        {
            if (cacheTRIX != null)
                for (int idx = 0; idx < cacheTRIX.Length; idx++)
                    if (cacheTRIX[idx].Period == period && cacheTRIX[idx].SignalPeriod == signalPeriod && cacheTRIX[idx].EqualsInput(input))
                        return cacheTRIX[idx];

            lock (checkTRIX)
            {
                checkTRIX.Period = period;
                period = checkTRIX.Period;
                checkTRIX.SignalPeriod = signalPeriod;
                signalPeriod = checkTRIX.SignalPeriod;

                if (cacheTRIX != null)
                    for (int idx = 0; idx < cacheTRIX.Length; idx++)
                        if (cacheTRIX[idx].Period == period && cacheTRIX[idx].SignalPeriod == signalPeriod && cacheTRIX[idx].EqualsInput(input))
                            return cacheTRIX[idx];

                TRIX indicator = new TRIX();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                indicator.SignalPeriod = signalPeriod;
                Indicators.Add(indicator);
                indicator.SetUp();

                TRIX[] tmp = new TRIX[cacheTRIX == null ? 1 : cacheTRIX.Length + 1];
                if (cacheTRIX != null)
                    cacheTRIX.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTRIX = tmp;
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
        /// The TRIX (Triple Exponential Average) displays the percentage Rate of Change (ROC) of a triple EMA. Trix oscillates above and below the zero value. The indicator applies triple smoothing in an attempt to eliminate insignificant price movements within the trend that you're trying to isolate.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TRIX TRIX(int period, int signalPeriod)
        {
            return _indicator.TRIX(Input, period, signalPeriod);
        }

        /// <summary>
        /// The TRIX (Triple Exponential Average) displays the percentage Rate of Change (ROC) of a triple EMA. Trix oscillates above and below the zero value. The indicator applies triple smoothing in an attempt to eliminate insignificant price movements within the trend that you're trying to isolate.
        /// </summary>
        /// <returns></returns>
        public Indicator.TRIX TRIX(Data.IDataSeries input, int period, int signalPeriod)
        {
            return _indicator.TRIX(input, period, signalPeriod);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The TRIX (Triple Exponential Average) displays the percentage Rate of Change (ROC) of a triple EMA. Trix oscillates above and below the zero value. The indicator applies triple smoothing in an attempt to eliminate insignificant price movements within the trend that you're trying to isolate.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TRIX TRIX(int period, int signalPeriod)
        {
            return _indicator.TRIX(Input, period, signalPeriod);
        }

        /// <summary>
        /// The TRIX (Triple Exponential Average) displays the percentage Rate of Change (ROC) of a triple EMA. Trix oscillates above and below the zero value. The indicator applies triple smoothing in an attempt to eliminate insignificant price movements within the trend that you're trying to isolate.
        /// </summary>
        /// <returns></returns>
        public Indicator.TRIX TRIX(Data.IDataSeries input, int period, int signalPeriod)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TRIX(input, period, signalPeriod);
        }
    }
}
#endregion
