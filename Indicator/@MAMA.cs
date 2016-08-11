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
	/// The MAMA (MESA Adaptive Moving Average) was developed by John Ehlers. It adapts to price movement in a new and unique way. The adaptation is based on the Hilbert Transform Discriminator. The adavantage of this method features fast attack average and a slow decay average. The MAMA + the FAMA (Following Adaptive Moving Average) lines only cross at major market reversals.
	/// </summary>
	[Description("The MAMA (MESA Adaptive Moving Average) was developed by John Ehlers. It adapts to price movement in a new and unique way. The adaptation is based on the Hilbert Transform Discriminator. The adavantage of this method features fast attack average and a slow decay average. The MAMA + the FAMA (Following Adaptive Moving Average) lines only cross at major market reversals.")]
	public class MAMA : Indicator
	{
		#region Variables
		private double				fastLimit	= 0.50;
		private double				slowLimit	= 0.05;

		private DataSeries		detrender;
		private DataSeries		period;
		private DataSeries		smooth;

		private DataSeries	    i1;
		private DataSeries	    i2;
		private DataSeries	    im;
		private DataSeries		q1;
		private DataSeries		q2;
		private DataSeries	    re;

		private DataSeries	    phase;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Black, "Default"));
			Add(new Plot(Color.Blue, "FAMA"));
			
			detrender		= new DataSeries(this);
			period			= new DataSeries(this);
			smooth			= new DataSeries(this);

			i1				= new DataSeries(this);
			i2				= new DataSeries(this);
			im				= new DataSeries(this);
			q1				= new DataSeries(this);
			q2				= new DataSeries(this);
			re				= new DataSeries(this);

			phase			= new DataSeries(this);

			Overlay			= true;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar < 6)
				return;
			else if (CurrentBar == 6)
			{
				Default.Set((High[0] + Low[0]) / 2);
				return;
			}
			
			smooth.Set((4 * Median[0] + 3 * Median[1] + 2 * Median[2] + Median[3]) / 10);
			detrender.Set((0.0962 * smooth[0] + 0.5769 * smooth[2] - 
						   0.5769 * smooth[4] - 0.0962 * smooth[6]) * (0.075*period[1] + 0.54));

			// Compute InPhase and Quadrature components
			q1.Set((0.0962 * detrender[0] + 0.5769 * detrender[2] - 
				    0.5769 * detrender[4] - 0.0962 * detrender[6]) * (0.075 * period[1] + 0.54));

			i1.Set(detrender[3]);

			// Advance the phase of i1 and q1 by 90}
			double jI = (0.0962 * i1[0] + 0.5769 * i1[2] - 0.5769 * i1[4] - 0.0962 * i1[6]) * (0.075 * period[1] + 0.54);
			double jQ = (0.0962 * q1[0] + 0.5769 * q1[2] - 0.5769 * q1[4] - 0.0962 * q1[6]) * (0.075 * period[1] + 0.54);

			// Phasor addition for 3 bar averaging}
			i2.Set(i1[0] - jQ);
			q2.Set(q1[0] + jI);

			// Smooth the I and Q components before applying the discriminator
			i2.Set(0.2 * i2[0] + 0.8 * i2[1]);
			q2.Set(0.2 * q2[0] + 0.8 * q2[1]);

			// Homodyne Discriminator
			re.Set(i2[0] * i2[1] + q2[0] * q2[1]);
			im.Set(i2[0] * q2[1] - q2[0] * i2[1]);

			re.Set(0.2 * re[0] + 0.8 * re[1]);
			im.Set(0.2 * im[0] + 0.8 * im[1]);

			if (im[0] != 0.0 && re[0] != 0.0) period.Set(360 / ((180 / Math.PI) * System.Math.Atan(im[0] / re[0])));
			if (period[0] > 1.5  * period[1]) period.Set(1.5  * period[1]);
			if (period[0] < 0.67 * period[1]) period.Set(0.67 * period[1]);
			if (period[0] < 6)  period.Set(6);
			if (period[0] > 50) period.Set(50);

			period.Set(0.2 * period[0] + 0.8 * period[1]);

			if (i1[0] != 0.0) phase.Set((180 / Math.PI) * System.Math.Atan(q1[0] / i1[0]));

			double deltaPhase = phase[1] - phase[0];
			if (deltaPhase < 1) deltaPhase = 1;

			double alpha = fastLimit / deltaPhase;
			if (alpha < slowLimit) alpha = slowLimit;

			// MAMA
			Default.Set(alpha * Median[0] + (1 - alpha) * Default[1]);

			// FAMA
			Fama.Set(0.5 * alpha * Value[0] + (1 - 0.5 * alpha) * Fama[1]);
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Fama
		{
			get { return Values[1]; }
		}

		/// <summary>
		/// </summary>
		[Description("Fast Limit. Upper limit of the alpha used in computing values.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Fast limit")]
		public double FastLimit
		{
			get { return fastLimit; }
			set { fastLimit = Math.Max(0.05, value); }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Default
		{
			get { return Values[0]; }
		}

		/// <summary>
		/// </summary>
		[Description("Slow Limit. Lower limit of the alpha used in computing values.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Slow limit")]
		public double SlowLimit
		{
			get { return slowLimit; }
			set { slowLimit = Math.Max(0.005, value); }
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
        private MAMA[] cacheMAMA = null;

        private static MAMA checkMAMA = new MAMA();

        /// <summary>
        /// The MAMA (MESA Adaptive Moving Average) was developed by John Ehlers. It adapts to price movement in a new and unique way. The adaptation is based on the Hilbert Transform Discriminator. The adavantage of this method features fast attack average and a slow decay average. The MAMA + the FAMA (Following Adaptive Moving Average) lines only cross at major market reversals.
        /// </summary>
        /// <returns></returns>
        public MAMA MAMA(double fastLimit, double slowLimit)
        {
            return MAMA(Input, fastLimit, slowLimit);
        }

        /// <summary>
        /// The MAMA (MESA Adaptive Moving Average) was developed by John Ehlers. It adapts to price movement in a new and unique way. The adaptation is based on the Hilbert Transform Discriminator. The adavantage of this method features fast attack average and a slow decay average. The MAMA + the FAMA (Following Adaptive Moving Average) lines only cross at major market reversals.
        /// </summary>
        /// <returns></returns>
        public MAMA MAMA(Data.IDataSeries input, double fastLimit, double slowLimit)
        {
            if (cacheMAMA != null)
                for (int idx = 0; idx < cacheMAMA.Length; idx++)
                    if (Math.Abs(cacheMAMA[idx].FastLimit - fastLimit) <= double.Epsilon && Math.Abs(cacheMAMA[idx].SlowLimit - slowLimit) <= double.Epsilon && cacheMAMA[idx].EqualsInput(input))
                        return cacheMAMA[idx];

            lock (checkMAMA)
            {
                checkMAMA.FastLimit = fastLimit;
                fastLimit = checkMAMA.FastLimit;
                checkMAMA.SlowLimit = slowLimit;
                slowLimit = checkMAMA.SlowLimit;

                if (cacheMAMA != null)
                    for (int idx = 0; idx < cacheMAMA.Length; idx++)
                        if (Math.Abs(cacheMAMA[idx].FastLimit - fastLimit) <= double.Epsilon && Math.Abs(cacheMAMA[idx].SlowLimit - slowLimit) <= double.Epsilon && cacheMAMA[idx].EqualsInput(input))
                            return cacheMAMA[idx];

                MAMA indicator = new MAMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.FastLimit = fastLimit;
                indicator.SlowLimit = slowLimit;
                Indicators.Add(indicator);
                indicator.SetUp();

                MAMA[] tmp = new MAMA[cacheMAMA == null ? 1 : cacheMAMA.Length + 1];
                if (cacheMAMA != null)
                    cacheMAMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheMAMA = tmp;
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
        /// The MAMA (MESA Adaptive Moving Average) was developed by John Ehlers. It adapts to price movement in a new and unique way. The adaptation is based on the Hilbert Transform Discriminator. The adavantage of this method features fast attack average and a slow decay average. The MAMA + the FAMA (Following Adaptive Moving Average) lines only cross at major market reversals.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MAMA MAMA(double fastLimit, double slowLimit)
        {
            return _indicator.MAMA(Input, fastLimit, slowLimit);
        }

        /// <summary>
        /// The MAMA (MESA Adaptive Moving Average) was developed by John Ehlers. It adapts to price movement in a new and unique way. The adaptation is based on the Hilbert Transform Discriminator. The adavantage of this method features fast attack average and a slow decay average. The MAMA + the FAMA (Following Adaptive Moving Average) lines only cross at major market reversals.
        /// </summary>
        /// <returns></returns>
        public Indicator.MAMA MAMA(Data.IDataSeries input, double fastLimit, double slowLimit)
        {
            return _indicator.MAMA(input, fastLimit, slowLimit);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The MAMA (MESA Adaptive Moving Average) was developed by John Ehlers. It adapts to price movement in a new and unique way. The adaptation is based on the Hilbert Transform Discriminator. The adavantage of this method features fast attack average and a slow decay average. The MAMA + the FAMA (Following Adaptive Moving Average) lines only cross at major market reversals.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MAMA MAMA(double fastLimit, double slowLimit)
        {
            return _indicator.MAMA(Input, fastLimit, slowLimit);
        }

        /// <summary>
        /// The MAMA (MESA Adaptive Moving Average) was developed by John Ehlers. It adapts to price movement in a new and unique way. The adaptation is based on the Hilbert Transform Discriminator. The adavantage of this method features fast attack average and a slow decay average. The MAMA + the FAMA (Following Adaptive Moving Average) lines only cross at major market reversals.
        /// </summary>
        /// <returns></returns>
        public Indicator.MAMA MAMA(Data.IDataSeries input, double fastLimit, double slowLimit)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.MAMA(input, fastLimit, slowLimit);
        }
    }
}
#endregion
