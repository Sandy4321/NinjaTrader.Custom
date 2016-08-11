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
	/// Directional Movement Index. Directional Movement Index is quite similiar to Welles Wilder's Relative Strength Index. The difference is the DMI uses variable time periods (from 3 to 30) vs. the RSI's fixed periods.
	/// </summary>
	[Description("The Directional Movement Index is quite similiar to Welles Wilder's Relative Strength Index. The difference is the DMI uses variable time periods (from 3 to 30) vs. the RSI's fixed periods.")]
	public class DMI : Indicator
	{
		#region Variables
		private int					period		= 14;
		private DataSeries		dmMinus;
		private DataSeries		dmPlus;
		private DataSeries		tr;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Green, "DMI"));

			dmMinus	= new DataSeries(this);
			dmPlus	= new DataSeries(this);
			tr		= new DataSeries(this);
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				dmMinus.Set(0);
				dmPlus.Set(0);
				tr.Set(High[0] - Low[0]);
				Value.Set(0);
			}
			else
			{
				dmMinus.Set(Low[1] - Low[0] > High[0] - High[1] ? Math.Max(Low[1] - Low[0], 0) : 0);
				dmPlus.Set(High[0] - High[1] > Low[1] - Low[0] ? Math.Max(High[0] - High[1], 0) : 0);
				tr.Set(Math.Max(High[0] - Low[0], Math.Max(Math.Abs(High[0] - Close[1]), Math.Abs(Low[0] - Close[1]))));

				double diPlus	= (SMA(tr, Period)[0] == 0) ? 0 : SMA(dmPlus, Period)[0] / SMA(tr, Period)[0];
				double diMinus	= (SMA(tr, Period)[0] == 0) ? 0 : SMA(dmMinus, Period)[0] / SMA(tr, Period)[0];

				Value.Set((diPlus + diMinus == 0) ? 0 : (diPlus - diMinus) / (diPlus + diMinus));
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
        private DMI[] cacheDMI = null;

        private static DMI checkDMI = new DMI();

        /// <summary>
        /// The Directional Movement Index is quite similiar to Welles Wilder's Relative Strength Index. The difference is the DMI uses variable time periods (from 3 to 30) vs. the RSI's fixed periods.
        /// </summary>
        /// <returns></returns>
        public DMI DMI(int period)
        {
            return DMI(Input, period);
        }

        /// <summary>
        /// The Directional Movement Index is quite similiar to Welles Wilder's Relative Strength Index. The difference is the DMI uses variable time periods (from 3 to 30) vs. the RSI's fixed periods.
        /// </summary>
        /// <returns></returns>
        public DMI DMI(Data.IDataSeries input, int period)
        {
            if (cacheDMI != null)
                for (int idx = 0; idx < cacheDMI.Length; idx++)
                    if (cacheDMI[idx].Period == period && cacheDMI[idx].EqualsInput(input))
                        return cacheDMI[idx];

            lock (checkDMI)
            {
                checkDMI.Period = period;
                period = checkDMI.Period;

                if (cacheDMI != null)
                    for (int idx = 0; idx < cacheDMI.Length; idx++)
                        if (cacheDMI[idx].Period == period && cacheDMI[idx].EqualsInput(input))
                            return cacheDMI[idx];

                DMI indicator = new DMI();
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

                DMI[] tmp = new DMI[cacheDMI == null ? 1 : cacheDMI.Length + 1];
                if (cacheDMI != null)
                    cacheDMI.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheDMI = tmp;
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
        /// The Directional Movement Index is quite similiar to Welles Wilder's Relative Strength Index. The difference is the DMI uses variable time periods (from 3 to 30) vs. the RSI's fixed periods.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DMI DMI(int period)
        {
            return _indicator.DMI(Input, period);
        }

        /// <summary>
        /// The Directional Movement Index is quite similiar to Welles Wilder's Relative Strength Index. The difference is the DMI uses variable time periods (from 3 to 30) vs. the RSI's fixed periods.
        /// </summary>
        /// <returns></returns>
        public Indicator.DMI DMI(Data.IDataSeries input, int period)
        {
            return _indicator.DMI(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Directional Movement Index is quite similiar to Welles Wilder's Relative Strength Index. The difference is the DMI uses variable time periods (from 3 to 30) vs. the RSI's fixed periods.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.DMI DMI(int period)
        {
            return _indicator.DMI(Input, period);
        }

        /// <summary>
        /// The Directional Movement Index is quite similiar to Welles Wilder's Relative Strength Index. The difference is the DMI uses variable time periods (from 3 to 30) vs. the RSI's fixed periods.
        /// </summary>
        /// <returns></returns>
        public Indicator.DMI DMI(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.DMI(input, period);
        }
    }
}
#endregion
