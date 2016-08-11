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
	/// Volume is simply the number of shares (or contracts) traded during a specified time frame (e.g. hour, day, week, month, etc).
	/// </summary>
	[Description("Volume is simply the number of shares (or contracts) traded during a specified time frame (e.g. hour, day, week, month, etc).")]
	public class VOL : Indicator
	{
		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Blue, 2), PlotStyle.Bar, "Volume"));
			Add(new Line(Color.DarkGray, 0, "Zero line"));
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			Value.Set(Volume[0]);
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private VOL[] cacheVOL = null;

        private static VOL checkVOL = new VOL();

        /// <summary>
        /// Volume is simply the number of shares (or contracts) traded during a specified time frame (e.g. hour, day, week, month, etc).
        /// </summary>
        /// <returns></returns>
        public VOL VOL()
        {
            return VOL(Input);
        }

        /// <summary>
        /// Volume is simply the number of shares (or contracts) traded during a specified time frame (e.g. hour, day, week, month, etc).
        /// </summary>
        /// <returns></returns>
        public VOL VOL(Data.IDataSeries input)
        {
            if (cacheVOL != null)
                for (int idx = 0; idx < cacheVOL.Length; idx++)
                    if (cacheVOL[idx].EqualsInput(input))
                        return cacheVOL[idx];

            lock (checkVOL)
            {
                if (cacheVOL != null)
                    for (int idx = 0; idx < cacheVOL.Length; idx++)
                        if (cacheVOL[idx].EqualsInput(input))
                            return cacheVOL[idx];

                VOL indicator = new VOL();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                VOL[] tmp = new VOL[cacheVOL == null ? 1 : cacheVOL.Length + 1];
                if (cacheVOL != null)
                    cacheVOL.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheVOL = tmp;
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
        /// Volume is simply the number of shares (or contracts) traded during a specified time frame (e.g. hour, day, week, month, etc).
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VOL VOL()
        {
            return _indicator.VOL(Input);
        }

        /// <summary>
        /// Volume is simply the number of shares (or contracts) traded during a specified time frame (e.g. hour, day, week, month, etc).
        /// </summary>
        /// <returns></returns>
        public Indicator.VOL VOL(Data.IDataSeries input)
        {
            return _indicator.VOL(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Volume is simply the number of shares (or contracts) traded during a specified time frame (e.g. hour, day, week, month, etc).
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VOL VOL()
        {
            return _indicator.VOL(Input);
        }

        /// <summary>
        /// Volume is simply the number of shares (or contracts) traded during a specified time frame (e.g. hour, day, week, month, etc).
        /// </summary>
        /// <returns></returns>
        public Indicator.VOL VOL(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.VOL(input);
        }
    }
}
#endregion
