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
	/// Donchian Channel. The Donchian Channel indicator was created by Richard Donchian. It uses the highest high and the lowest low of a period of time to plot the channel.
	/// </summary>
	[Description("The Donchian Channel indicator was created by Richard Donchian. It uses the highest high and the lowest low of a period of time to plot the channel.")]
	public class DonchianChannel : Indicator
	{
		#region Variables
		private int			period		= 14;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Orange, "Mean"));
			Add(new Plot(Color.Blue, "Upper"));
			Add(new Plot(Color.Blue, "Lower"));

			Overlay			= true;
		}
		
		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			Value.Set((MAX(High, Period)[0] + MIN(Low, Period)[0]) / 2);
			Upper.Set(MAX(High, Period)[0]);
			Lower.Set(MIN(Low, Period)[0]);
		}
		
		#region Properties
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Lower
		{
			get { return Values[2]; }
		}
			
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Mean
		{
			get { return Values[0]; }
		}
		
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
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Upper
		{
			get { return Values[1]; }
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
        private DonchianChannel[] cacheDonchianChannel = null;

        private static DonchianChannel checkDonchianChannel = new DonchianChannel();

        /// <summary>
        /// The Donchian Channel indicator was created by Richard Donchian. It uses the highest high and the lowest low of a period of time to plot the channel.
        /// </summary>
        /// <returns></returns>
        public DonchianChannel DonchianChannel(int period)
        {
            return DonchianChannel(Input, period);
        }

        /// <summary>
        /// The Donchian Channel indicator was created by Richard Donchian. It uses the highest high and the lowest low of a period of time to plot the channel.
        /// </summary>
        /// <returns></returns>
        public DonchianChannel DonchianChannel(Data.IDataSeries input, int period)
        {
            if (cacheDonchianChannel != null)
                for (int idx = 0; idx < cacheDonchianChannel.Length; idx++)
                    if (cacheDonchianChannel[idx].Period == period && cacheDonchianChannel[idx].EqualsInput(input))
                        return cacheDonchianChannel[idx];

            lock (checkDonchianChannel)
            {
                checkDonchianChannel.Period = period;
                period = checkDonchianChannel.Period;

                if (cacheDonchianChannel != null)
                    for (int idx = 0; idx < cacheDonchianChannel.Length; idx++)
                        if (cacheDonchianChannel[idx].Period == period && cacheDonchianChannel[idx].EqualsInput(input))
                            return cacheDonchianChannel[idx];

                DonchianChannel indicator = new DonchianChannel();
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

                DonchianChannel[] tmp = new DonchianChannel[cacheDonchianChannel == null ? 1 : cacheDonchianChannel.Length + 1];
                if (cacheDonchianChannel != null)
                    cacheDonchianChannel.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheDonchianChannel = tmp;
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
        /// The Donchian Channel indicator was created by Richard Donchian. It uses the highest high and the lowest low of a period of time to plot the channel.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DonchianChannel DonchianChannel(int period)
        {
            return _indicator.DonchianChannel(Input, period);
        }

        /// <summary>
        /// The Donchian Channel indicator was created by Richard Donchian. It uses the highest high and the lowest low of a period of time to plot the channel.
        /// </summary>
        /// <returns></returns>
        public Indicator.DonchianChannel DonchianChannel(Data.IDataSeries input, int period)
        {
            return _indicator.DonchianChannel(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Donchian Channel indicator was created by Richard Donchian. It uses the highest high and the lowest low of a period of time to plot the channel.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DonchianChannel DonchianChannel(int period)
        {
            return _indicator.DonchianChannel(Input, period);
        }

        /// <summary>
        /// The Donchian Channel indicator was created by Richard Donchian. It uses the highest high and the lowest low of a period of time to plot the channel.
        /// </summary>
        /// <returns></returns>
        public Indicator.DonchianChannel DonchianChannel(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.DonchianChannel(input, period);
        }
    }
}
#endregion
