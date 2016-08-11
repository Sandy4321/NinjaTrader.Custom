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
	/// The Ease of Movement (EMV) indicator emphasizes days in which the stock is moving easily and minimizes the days in which the stock is finding it difficult to move. A buy signal is generated when the EMV crosses above zero, a sell signal when it crosses below zero. When the EMV hovers around zero, then there are small price movements and/or high volume, which is to say, the price is not moving easily.
	/// </summary>
	[Description("The Ease of Movement (EMV) indicator emphasizes days in which the stock is moving easily and minimizes the days in which the stock is finding it difficult to move. A buy signal is generated when the EMV crosses above zero, a sell signal when it crosses below zero. When the EMV hovers around zero, then there are small price movements and/or high volume, which is to say, the price is not moving easily.")]
	public class EaseOfMovement : Indicator
	{
		#region Variables
		private int				smoothing		= 14;
		private int				volumeDivisor	= 10000;
		private DataSeries emv;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Blue, "EaseOfMovement"));
			Add(new Line(Color.DarkGray, 0, "ZeroLine"));

			emv = new DataSeries(this);
		}

		/// <summary>
		/// Calculates the indicator value(s) at the current index.
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
				return;

			double midPoint = Median[0] - Median[1];
			double boxRatio = (Volume[0] / volumeDivisor) / (High[0] - Low[0]);

			emv.Set(midPoint / boxRatio);
			Value.Set(EMA(emv, smoothing)[0]);
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("The period used for EMA smoothing of this indicator.")]
		[GridCategory("Parameters")]
		public int Smoothing
		{
			get { return smoothing; }
			set { smoothing = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("The volume divisor helps to make the indicator values larger and easier to work with. The higher the divisor the larger the indicator numbers become.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Volume divisor")]
		public int VolumeDivisor
		{
			get { return volumeDivisor; }
			set { volumeDivisor = Math.Max(1, value); }
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
        private EaseOfMovement[] cacheEaseOfMovement = null;

        private static EaseOfMovement checkEaseOfMovement = new EaseOfMovement();

        /// <summary>
        /// The Ease of Movement (EMV) indicator emphasizes days in which the stock is moving easily and minimizes the days in which the stock is finding it difficult to move. A buy signal is generated when the EMV crosses above zero, a sell signal when it crosses below zero. When the EMV hovers around zero, then there are small price movements and/or high volume, which is to say, the price is not moving easily.
        /// </summary>
        /// <returns></returns>
        public EaseOfMovement EaseOfMovement(int smoothing, int volumeDivisor)
        {
            return EaseOfMovement(Input, smoothing, volumeDivisor);
        }

        /// <summary>
        /// The Ease of Movement (EMV) indicator emphasizes days in which the stock is moving easily and minimizes the days in which the stock is finding it difficult to move. A buy signal is generated when the EMV crosses above zero, a sell signal when it crosses below zero. When the EMV hovers around zero, then there are small price movements and/or high volume, which is to say, the price is not moving easily.
        /// </summary>
        /// <returns></returns>
        public EaseOfMovement EaseOfMovement(Data.IDataSeries input, int smoothing, int volumeDivisor)
        {
            if (cacheEaseOfMovement != null)
                for (int idx = 0; idx < cacheEaseOfMovement.Length; idx++)
                    if (cacheEaseOfMovement[idx].Smoothing == smoothing && cacheEaseOfMovement[idx].VolumeDivisor == volumeDivisor && cacheEaseOfMovement[idx].EqualsInput(input))
                        return cacheEaseOfMovement[idx];

            lock (checkEaseOfMovement)
            {
                checkEaseOfMovement.Smoothing = smoothing;
                smoothing = checkEaseOfMovement.Smoothing;
                checkEaseOfMovement.VolumeDivisor = volumeDivisor;
                volumeDivisor = checkEaseOfMovement.VolumeDivisor;

                if (cacheEaseOfMovement != null)
                    for (int idx = 0; idx < cacheEaseOfMovement.Length; idx++)
                        if (cacheEaseOfMovement[idx].Smoothing == smoothing && cacheEaseOfMovement[idx].VolumeDivisor == volumeDivisor && cacheEaseOfMovement[idx].EqualsInput(input))
                            return cacheEaseOfMovement[idx];

                EaseOfMovement indicator = new EaseOfMovement();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Smoothing = smoothing;
                indicator.VolumeDivisor = volumeDivisor;
                Indicators.Add(indicator);
                indicator.SetUp();

                EaseOfMovement[] tmp = new EaseOfMovement[cacheEaseOfMovement == null ? 1 : cacheEaseOfMovement.Length + 1];
                if (cacheEaseOfMovement != null)
                    cacheEaseOfMovement.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheEaseOfMovement = tmp;
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
        /// The Ease of Movement (EMV) indicator emphasizes days in which the stock is moving easily and minimizes the days in which the stock is finding it difficult to move. A buy signal is generated when the EMV crosses above zero, a sell signal when it crosses below zero. When the EMV hovers around zero, then there are small price movements and/or high volume, which is to say, the price is not moving easily.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.EaseOfMovement EaseOfMovement(int smoothing, int volumeDivisor)
        {
            return _indicator.EaseOfMovement(Input, smoothing, volumeDivisor);
        }

        /// <summary>
        /// The Ease of Movement (EMV) indicator emphasizes days in which the stock is moving easily and minimizes the days in which the stock is finding it difficult to move. A buy signal is generated when the EMV crosses above zero, a sell signal when it crosses below zero. When the EMV hovers around zero, then there are small price movements and/or high volume, which is to say, the price is not moving easily.
        /// </summary>
        /// <returns></returns>
        public Indicator.EaseOfMovement EaseOfMovement(Data.IDataSeries input, int smoothing, int volumeDivisor)
        {
            return _indicator.EaseOfMovement(input, smoothing, volumeDivisor);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Ease of Movement (EMV) indicator emphasizes days in which the stock is moving easily and minimizes the days in which the stock is finding it difficult to move. A buy signal is generated when the EMV crosses above zero, a sell signal when it crosses below zero. When the EMV hovers around zero, then there are small price movements and/or high volume, which is to say, the price is not moving easily.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.EaseOfMovement EaseOfMovement(int smoothing, int volumeDivisor)
        {
            return _indicator.EaseOfMovement(Input, smoothing, volumeDivisor);
        }

        /// <summary>
        /// The Ease of Movement (EMV) indicator emphasizes days in which the stock is moving easily and minimizes the days in which the stock is finding it difficult to move. A buy signal is generated when the EMV crosses above zero, a sell signal when it crosses below zero. When the EMV hovers around zero, then there are small price movements and/or high volume, which is to say, the price is not moving easily.
        /// </summary>
        /// <returns></returns>
        public Indicator.EaseOfMovement EaseOfMovement(Data.IDataSeries input, int smoothing, int volumeDivisor)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.EaseOfMovement(input, smoothing, volumeDivisor);
        }
    }
}
#endregion
