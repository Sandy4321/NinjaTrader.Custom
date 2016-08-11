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
	/// Keltner Channel. The Keltner Channel is a similar indicator to Bollinger Bands. Here the midline is a standard moving average with the upper and lower bands offset by the SMA of the difference between the high and low of the previous bars. The offset multiplier as well as the SMA period is configurable.
	/// </summary>
	[Description("The Keltner Channel is a similar indicator to Bollinger Bands. Here the midline is a standard moving average with the upper and lower bands offset by the SMA of the difference between the high and low of the previous bars. The offset multiplier as well as the SMA period is configurable.")]
	public class KeltnerChannel : Indicator
	{
		#region Variables
		private	int					period				= 10;
		private double				offsetMultiplier	= 1.5;
		private DataSeries		diff;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.DarkGray, "Midline"));
			Add(new Plot(Color.Blue,     "Upper"));
			Add(new Plot(Color.Blue,     "Lower"));

			diff				= new DataSeries(this);

			Overlay				= true;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick).
		/// </summary>
		protected override void OnBarUpdate()
		{
			diff.Set(High[0] - Low[0]);

			double middle	= SMA(Typical, Period)[0];
			double offset	= SMA(diff, Period)[0] * offsetMultiplier;

			double upper	= middle + offset;
			double lower	= middle - offset;

			Midline.Set(middle);
			Upper.Set(upper);
			Lower.Set(lower);
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
		[Description("How much to expand the upper and lower band from the normal offset")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Offset multiplier")]
		public double OffsetMultiplier
		{
			get { return offsetMultiplier; }
			set { offsetMultiplier = Math.Max(0.01, value); }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Midline
		{
			get { return Values[0]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Upper
		{
			get { return Values[1]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Lower
		{
			get { return Values[2]; }
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
        private KeltnerChannel[] cacheKeltnerChannel = null;

        private static KeltnerChannel checkKeltnerChannel = new KeltnerChannel();

        /// <summary>
        /// The Keltner Channel is a similar indicator to Bollinger Bands. Here the midline is a standard moving average with the upper and lower bands offset by the SMA of the difference between the high and low of the previous bars. The offset multiplier as well as the SMA period is configurable.
        /// </summary>
        /// <returns></returns>
        public KeltnerChannel KeltnerChannel(double offsetMultiplier, int period)
        {
            return KeltnerChannel(Input, offsetMultiplier, period);
        }

        /// <summary>
        /// The Keltner Channel is a similar indicator to Bollinger Bands. Here the midline is a standard moving average with the upper and lower bands offset by the SMA of the difference between the high and low of the previous bars. The offset multiplier as well as the SMA period is configurable.
        /// </summary>
        /// <returns></returns>
        public KeltnerChannel KeltnerChannel(Data.IDataSeries input, double offsetMultiplier, int period)
        {
            if (cacheKeltnerChannel != null)
                for (int idx = 0; idx < cacheKeltnerChannel.Length; idx++)
                    if (Math.Abs(cacheKeltnerChannel[idx].OffsetMultiplier - offsetMultiplier) <= double.Epsilon && cacheKeltnerChannel[idx].Period == period && cacheKeltnerChannel[idx].EqualsInput(input))
                        return cacheKeltnerChannel[idx];

            lock (checkKeltnerChannel)
            {
                checkKeltnerChannel.OffsetMultiplier = offsetMultiplier;
                offsetMultiplier = checkKeltnerChannel.OffsetMultiplier;
                checkKeltnerChannel.Period = period;
                period = checkKeltnerChannel.Period;

                if (cacheKeltnerChannel != null)
                    for (int idx = 0; idx < cacheKeltnerChannel.Length; idx++)
                        if (Math.Abs(cacheKeltnerChannel[idx].OffsetMultiplier - offsetMultiplier) <= double.Epsilon && cacheKeltnerChannel[idx].Period == period && cacheKeltnerChannel[idx].EqualsInput(input))
                            return cacheKeltnerChannel[idx];

                KeltnerChannel indicator = new KeltnerChannel();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.OffsetMultiplier = offsetMultiplier;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                KeltnerChannel[] tmp = new KeltnerChannel[cacheKeltnerChannel == null ? 1 : cacheKeltnerChannel.Length + 1];
                if (cacheKeltnerChannel != null)
                    cacheKeltnerChannel.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheKeltnerChannel = tmp;
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
        /// The Keltner Channel is a similar indicator to Bollinger Bands. Here the midline is a standard moving average with the upper and lower bands offset by the SMA of the difference between the high and low of the previous bars. The offset multiplier as well as the SMA period is configurable.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.KeltnerChannel KeltnerChannel(double offsetMultiplier, int period)
        {
            return _indicator.KeltnerChannel(Input, offsetMultiplier, period);
        }

        /// <summary>
        /// The Keltner Channel is a similar indicator to Bollinger Bands. Here the midline is a standard moving average with the upper and lower bands offset by the SMA of the difference between the high and low of the previous bars. The offset multiplier as well as the SMA period is configurable.
        /// </summary>
        /// <returns></returns>
        public Indicator.KeltnerChannel KeltnerChannel(Data.IDataSeries input, double offsetMultiplier, int period)
        {
            return _indicator.KeltnerChannel(input, offsetMultiplier, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Keltner Channel is a similar indicator to Bollinger Bands. Here the midline is a standard moving average with the upper and lower bands offset by the SMA of the difference between the high and low of the previous bars. The offset multiplier as well as the SMA period is configurable.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.KeltnerChannel KeltnerChannel(double offsetMultiplier, int period)
        {
            return _indicator.KeltnerChannel(Input, offsetMultiplier, period);
        }

        /// <summary>
        /// The Keltner Channel is a similar indicator to Bollinger Bands. Here the midline is a standard moving average with the upper and lower bands offset by the SMA of the difference between the high and low of the previous bars. The offset multiplier as well as the SMA period is configurable.
        /// </summary>
        /// <returns></returns>
        public Indicator.KeltnerChannel KeltnerChannel(Data.IDataSeries input, double offsetMultiplier, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.KeltnerChannel(input, offsetMultiplier, period);
        }
    }
}
#endregion
