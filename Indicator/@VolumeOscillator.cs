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
	/// The Volume Oscillator measures volume by calculating the difference of a fast and a slow moving average of volume. The Volume Oscillator can provide insight into the strength or weakness of a price trend. A positive value suggests there is enough market support to continue driving price activity in the direction of the current trend. A negative value suggests there is a lack of support, that prices may begin to become stagnant or reverse.
	/// </summary>
	[Description("The Volume Oscillator measures volume by calculating the difference of a fast and a slow moving average of volume. The Volume Oscillator can provide insight into the strength or weakness of a price trend. A positive value suggests there is enough market support to continue driving price activity in the direction of the current trend. A negative value suggests there is a lack of support, that prices may begin to become stagnant or reverse.")]
	public class VolumeOscillator : Indicator
	{
		#region Variables
		private int			fast	= 12;
		private int			slow	= 26;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Blue, 2), PlotStyle.Bar, "VolumeOscillator"));

			Overlay				= false;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			Value.Set(SMA(Volume, Fast)[0] - SMA(Volume, Slow)[0]);
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Number of bars for fast SMA")]
		[GridCategory("Parameters")]
		public int Fast
		{
			get { return fast; }
			set { fast = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Number of bars for slow SMA")]
		[GridCategory("Parameters")]
		public int Slow
		{
			get { return slow; }
			set { slow = Math.Max(1, value); }
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
        private VolumeOscillator[] cacheVolumeOscillator = null;

        private static VolumeOscillator checkVolumeOscillator = new VolumeOscillator();

        /// <summary>
        /// The Volume Oscillator measures volume by calculating the difference of a fast and a slow moving average of volume. The Volume Oscillator can provide insight into the strength or weakness of a price trend. A positive value suggests there is enough market support to continue driving price activity in the direction of the current trend. A negative value suggests there is a lack of support, that prices may begin to become stagnant or reverse.
        /// </summary>
        /// <returns></returns>
        public VolumeOscillator VolumeOscillator(int fast, int slow)
        {
            return VolumeOscillator(Input, fast, slow);
        }

        /// <summary>
        /// The Volume Oscillator measures volume by calculating the difference of a fast and a slow moving average of volume. The Volume Oscillator can provide insight into the strength or weakness of a price trend. A positive value suggests there is enough market support to continue driving price activity in the direction of the current trend. A negative value suggests there is a lack of support, that prices may begin to become stagnant or reverse.
        /// </summary>
        /// <returns></returns>
        public VolumeOscillator VolumeOscillator(Data.IDataSeries input, int fast, int slow)
        {
            if (cacheVolumeOscillator != null)
                for (int idx = 0; idx < cacheVolumeOscillator.Length; idx++)
                    if (cacheVolumeOscillator[idx].Fast == fast && cacheVolumeOscillator[idx].Slow == slow && cacheVolumeOscillator[idx].EqualsInput(input))
                        return cacheVolumeOscillator[idx];

            lock (checkVolumeOscillator)
            {
                checkVolumeOscillator.Fast = fast;
                fast = checkVolumeOscillator.Fast;
                checkVolumeOscillator.Slow = slow;
                slow = checkVolumeOscillator.Slow;

                if (cacheVolumeOscillator != null)
                    for (int idx = 0; idx < cacheVolumeOscillator.Length; idx++)
                        if (cacheVolumeOscillator[idx].Fast == fast && cacheVolumeOscillator[idx].Slow == slow && cacheVolumeOscillator[idx].EqualsInput(input))
                            return cacheVolumeOscillator[idx];

                VolumeOscillator indicator = new VolumeOscillator();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Fast = fast;
                indicator.Slow = slow;
                Indicators.Add(indicator);
                indicator.SetUp();

                VolumeOscillator[] tmp = new VolumeOscillator[cacheVolumeOscillator == null ? 1 : cacheVolumeOscillator.Length + 1];
                if (cacheVolumeOscillator != null)
                    cacheVolumeOscillator.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheVolumeOscillator = tmp;
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
        /// The Volume Oscillator measures volume by calculating the difference of a fast and a slow moving average of volume. The Volume Oscillator can provide insight into the strength or weakness of a price trend. A positive value suggests there is enough market support to continue driving price activity in the direction of the current trend. A negative value suggests there is a lack of support, that prices may begin to become stagnant or reverse.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolumeOscillator VolumeOscillator(int fast, int slow)
        {
            return _indicator.VolumeOscillator(Input, fast, slow);
        }

        /// <summary>
        /// The Volume Oscillator measures volume by calculating the difference of a fast and a slow moving average of volume. The Volume Oscillator can provide insight into the strength or weakness of a price trend. A positive value suggests there is enough market support to continue driving price activity in the direction of the current trend. A negative value suggests there is a lack of support, that prices may begin to become stagnant or reverse.
        /// </summary>
        /// <returns></returns>
        public Indicator.VolumeOscillator VolumeOscillator(Data.IDataSeries input, int fast, int slow)
        {
            return _indicator.VolumeOscillator(input, fast, slow);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Volume Oscillator measures volume by calculating the difference of a fast and a slow moving average of volume. The Volume Oscillator can provide insight into the strength or weakness of a price trend. A positive value suggests there is enough market support to continue driving price activity in the direction of the current trend. A negative value suggests there is a lack of support, that prices may begin to become stagnant or reverse.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolumeOscillator VolumeOscillator(int fast, int slow)
        {
            return _indicator.VolumeOscillator(Input, fast, slow);
        }

        /// <summary>
        /// The Volume Oscillator measures volume by calculating the difference of a fast and a slow moving average of volume. The Volume Oscillator can provide insight into the strength or weakness of a price trend. A positive value suggests there is enough market support to continue driving price activity in the direction of the current trend. A negative value suggests there is a lack of support, that prices may begin to become stagnant or reverse.
        /// </summary>
        /// <returns></returns>
        public Indicator.VolumeOscillator VolumeOscillator(Data.IDataSeries input, int fast, int slow)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.VolumeOscillator(input, fast, slow);
        }
    }
}
#endregion
