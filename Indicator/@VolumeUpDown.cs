// 
// Copyright (C) 2007, NinjaTrader LLC <www.ninjatrader.com>.
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
	/// Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar
	/// </summary>
	[Description("Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar")]
	public class VolumeUpDown : Indicator
	{
		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Lime, 2), PlotStyle.Bar, "UpVolume"));
			Add(new Plot(new Pen(Color.Red, 2), PlotStyle.Bar, "DownVolume"));
			Add(new Line(Color.DarkGray, 0, "Zero line"));
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (Close[0] >= Open[0])
			{
				Values[0].Set(Volume[0]);
				Values[1].Reset();
			}
			else
			{
				Values[1].Set(Volume[0]);
				Values[0].Reset();
			}
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private VolumeUpDown[] cacheVolumeUpDown = null;

        private static VolumeUpDown checkVolumeUpDown = new VolumeUpDown();

        /// <summary>
        /// Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar
        /// </summary>
        /// <returns></returns>
        public VolumeUpDown VolumeUpDown()
        {
            return VolumeUpDown(Input);
        }

        /// <summary>
        /// Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar
        /// </summary>
        /// <returns></returns>
        public VolumeUpDown VolumeUpDown(Data.IDataSeries input)
        {
            if (cacheVolumeUpDown != null)
                for (int idx = 0; idx < cacheVolumeUpDown.Length; idx++)
                    if (cacheVolumeUpDown[idx].EqualsInput(input))
                        return cacheVolumeUpDown[idx];

            lock (checkVolumeUpDown)
            {
                if (cacheVolumeUpDown != null)
                    for (int idx = 0; idx < cacheVolumeUpDown.Length; idx++)
                        if (cacheVolumeUpDown[idx].EqualsInput(input))
                            return cacheVolumeUpDown[idx];

                VolumeUpDown indicator = new VolumeUpDown();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                VolumeUpDown[] tmp = new VolumeUpDown[cacheVolumeUpDown == null ? 1 : cacheVolumeUpDown.Length + 1];
                if (cacheVolumeUpDown != null)
                    cacheVolumeUpDown.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheVolumeUpDown = tmp;
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
        /// Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolumeUpDown VolumeUpDown()
        {
            return _indicator.VolumeUpDown(Input);
        }

        /// <summary>
        /// Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar
        /// </summary>
        /// <returns></returns>
        public Indicator.VolumeUpDown VolumeUpDown(Data.IDataSeries input)
        {
            return _indicator.VolumeUpDown(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolumeUpDown VolumeUpDown()
        {
            return _indicator.VolumeUpDown(Input);
        }

        /// <summary>
        /// Variation of the VOL (Volume) indicator that colors the volume histogram different color depending if the current bar is up or down bar
        /// </summary>
        /// <returns></returns>
        public Indicator.VolumeUpDown VolumeUpDown(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.VolumeUpDown(input);
        }
    }
}
#endregion
