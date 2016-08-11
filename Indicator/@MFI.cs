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
	/// The MFI (Money Flow Index) is a momentum indicator that measures the strength of money flowing in and out of a security.
	/// </summary>
	[Description("The MFI (Money Flow Index) is a momentum indicator that measures the strength of money flowing in and out of a security.")]
	public class MFI : Indicator
	{
		#region Variables
		private int					period	= 14;
		private	DataSeries		negative;
		private	DataSeries		positive;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Orange, "MFI"));
			Add(new Line(Color.DarkViolet, 20, "Lower"));
			Add(new Line(Color.YellowGreen, 80, "Upper"));
			
			negative			= new DataSeries(this);
			positive			= new DataSeries(this);
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
				Value.Set(50);
			else
			{
				negative.Set(Typical[0] < Typical[1] ? Typical[0] * Volume[0] : 0);
				positive.Set(Typical[0] > Typical[1] ? Typical[0] * Volume[0] : 0);

				Value.Set(SUM(negative, Period)[0] == 0 ? 50 : 100.0 - (100.0 / (1 + SUM(positive, Period)[0] / SUM(negative, Period)[0])));
			}
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
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private MFI[] cacheMFI = null;

        private static MFI checkMFI = new MFI();

        /// <summary>
        /// The MFI (Money Flow Index) is a momentum indicator that measures the strength of money flowing in and out of a security.
        /// </summary>
        /// <returns></returns>
        public MFI MFI(int period)
        {
            return MFI(Input, period);
        }

        /// <summary>
        /// The MFI (Money Flow Index) is a momentum indicator that measures the strength of money flowing in and out of a security.
        /// </summary>
        /// <returns></returns>
        public MFI MFI(Data.IDataSeries input, int period)
        {
            if (cacheMFI != null)
                for (int idx = 0; idx < cacheMFI.Length; idx++)
                    if (cacheMFI[idx].Period == period && cacheMFI[idx].EqualsInput(input))
                        return cacheMFI[idx];

            lock (checkMFI)
            {
                checkMFI.Period = period;
                period = checkMFI.Period;

                if (cacheMFI != null)
                    for (int idx = 0; idx < cacheMFI.Length; idx++)
                        if (cacheMFI[idx].Period == period && cacheMFI[idx].EqualsInput(input))
                            return cacheMFI[idx];

                MFI indicator = new MFI();
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

                MFI[] tmp = new MFI[cacheMFI == null ? 1 : cacheMFI.Length + 1];
                if (cacheMFI != null)
                    cacheMFI.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheMFI = tmp;
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
        /// The MFI (Money Flow Index) is a momentum indicator that measures the strength of money flowing in and out of a security.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MFI MFI(int period)
        {
            return _indicator.MFI(Input, period);
        }

        /// <summary>
        /// The MFI (Money Flow Index) is a momentum indicator that measures the strength of money flowing in and out of a security.
        /// </summary>
        /// <returns></returns>
        public Indicator.MFI MFI(Data.IDataSeries input, int period)
        {
            return _indicator.MFI(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The MFI (Money Flow Index) is a momentum indicator that measures the strength of money flowing in and out of a security.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MFI MFI(int period)
        {
            return _indicator.MFI(Input, period);
        }

        /// <summary>
        /// The MFI (Money Flow Index) is a momentum indicator that measures the strength of money flowing in and out of a security.
        /// </summary>
        /// <returns></returns>
        public Indicator.MFI MFI(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.MFI(input, period);
        }
    }
}
#endregion
