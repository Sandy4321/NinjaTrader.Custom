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
	/// The Minimum shows the minimum of the last n bars.
	/// </summary>
	[Description("The Minimum shows the minimum of the last n bars.")]
	public class MIN : Indicator
	{
		#region Variables
        private int     lastBar;
        private double  lastMin;
        private int     period      = 14;
        private double  runningMin;
        private int     runningBar;
        private int     thisBar;
        #endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Green, "MIN"));
			Overlay = true;
		}
		
		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if(CurrentBar == 0)
			{
				runningMin  = Input[0];
                lastMin     = Input[0];
				runningBar  = 0;
                lastBar     = 0;
                thisBar     = 0;
				return;
			}

            if (CurrentBar - runningBar >= Period)
			{
				runningMin = double.MaxValue;
				for (int barsBack = Math.Min(CurrentBar, Period - 1); barsBack > 0; barsBack--)
					if(Input[barsBack] <= runningMin)
					{
						runningMin  = Input[barsBack];
						runningBar  = CurrentBar - barsBack;
                    }
			}

            if (thisBar != CurrentBar)
            {
                lastMin = runningMin;
                lastBar = runningBar;
                thisBar = CurrentBar;
            }

            if (Input[0] <= lastMin)
			{
				runningMin = Input[0];
				runningBar = CurrentBar;
			}
            else
            {
                runningMin = lastMin;
                runningBar = lastBar;
            }

			Value.Set(runningMin);
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set	{ period = Math.Max(1, value); }
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
        private MIN[] cacheMIN = null;

        private static MIN checkMIN = new MIN();

        /// <summary>
        /// The Minimum shows the minimum of the last n bars.
        /// </summary>
        /// <returns></returns>
        public MIN MIN(int period)
        {
            return MIN(Input, period);
        }

        /// <summary>
        /// The Minimum shows the minimum of the last n bars.
        /// </summary>
        /// <returns></returns>
        public MIN MIN(Data.IDataSeries input, int period)
        {
            if (cacheMIN != null)
                for (int idx = 0; idx < cacheMIN.Length; idx++)
                    if (cacheMIN[idx].Period == period && cacheMIN[idx].EqualsInput(input))
                        return cacheMIN[idx];

            lock (checkMIN)
            {
                checkMIN.Period = period;
                period = checkMIN.Period;

                if (cacheMIN != null)
                    for (int idx = 0; idx < cacheMIN.Length; idx++)
                        if (cacheMIN[idx].Period == period && cacheMIN[idx].EqualsInput(input))
                            return cacheMIN[idx];

                MIN indicator = new MIN();
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

                MIN[] tmp = new MIN[cacheMIN == null ? 1 : cacheMIN.Length + 1];
                if (cacheMIN != null)
                    cacheMIN.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheMIN = tmp;
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
        /// The Minimum shows the minimum of the last n bars.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MIN MIN(int period)
        {
            return _indicator.MIN(Input, period);
        }

        /// <summary>
        /// The Minimum shows the minimum of the last n bars.
        /// </summary>
        /// <returns></returns>
        public Indicator.MIN MIN(Data.IDataSeries input, int period)
        {
            return _indicator.MIN(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Minimum shows the minimum of the last n bars.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MIN MIN(int period)
        {
            return _indicator.MIN(Input, period);
        }

        /// <summary>
        /// The Minimum shows the minimum of the last n bars.
        /// </summary>
        /// <returns></returns>
        public Indicator.MIN MIN(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.MIN(input, period);
        }
    }
}
#endregion
