// 
// Copyright (C) 2008, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//
#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Moving Average Envelopes
    /// </summary>
    [Description("Plots % envelopes around a moving average")]
    public class MAEnvelopes : Indicator
    {
        #region Variables
		private int				matype				= 3;
		private int				period				= 14;
		private double			envelopePercentage = 1.5;
		#endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			// Adds a plot for the MA values to be stored in
			Add(new Plot(Color.Blue, "Upper"));
			Add(new Plot(Color.Blue, "Middle"));
			Add(new Plot(Color.Blue, "Lower"));
			Plots[1].Pen.DashStyle = DashStyle.Dash;
			
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			double maValue = 0;

			switch (matype)
			{
				case 1:
				{	
					Middle.Set(maValue = EMA(Inputs[0], Period)[0]);
					break;
				}
				case 2:
				{
					Middle.Set(maValue = HMA(Inputs[0], Period)[0]);
					break;
				}
				case 3:
				{
					Middle.Set(maValue = SMA(Inputs[0], Period)[0]);
					break;
				}
				case 4:
				{
					Middle.Set(maValue = TMA(Inputs[0], Period)[0]);
					break;
				}
				case 5:
				{
					Middle.Set(maValue = TEMA(Inputs[0], Period)[0]);
					break;
				}
				case 6:
				{
					Middle.Set(maValue = WMA(Inputs[0], Period)[0]);
					break;
				}
			}
			
			Upper.Set(maValue + (maValue * EnvelopePercentage / 100));
			Lower.Set(maValue - (maValue * EnvelopePercentage / 100));
        }
		
        #region Properties
		[Description("1 = EMA, 2 = HMA, 3 = SMA, 4= TMA, 5 = TEMA, 6 = WMA")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Moving average type")]
		public int MAType
		{
			get { return matype; }
			set { matype = Math.Min(6, Math.Max(1, value)); }
		}
		
		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Period")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
		}
		[Description("Percentage around MA that envelopes will be drawn")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Envelope percent offset")]
		public double EnvelopePercentage
		{
			get { return envelopePercentage; }
			set { envelopePercentage = Math.Max(0.01, value); }
		}
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Upper
        {
            get { return Values[0]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Middle
        {
            get { return Values[1]; }
        }

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
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
        private MAEnvelopes[] cacheMAEnvelopes = null;

        private static MAEnvelopes checkMAEnvelopes = new MAEnvelopes();

        /// <summary>
        /// Plots % envelopes around a moving average
        /// </summary>
        /// <returns></returns>
        public MAEnvelopes MAEnvelopes(double envelopePercentage, int mAType, int period)
        {
            return MAEnvelopes(Input, envelopePercentage, mAType, period);
        }

        /// <summary>
        /// Plots % envelopes around a moving average
        /// </summary>
        /// <returns></returns>
        public MAEnvelopes MAEnvelopes(Data.IDataSeries input, double envelopePercentage, int mAType, int period)
        {
            if (cacheMAEnvelopes != null)
                for (int idx = 0; idx < cacheMAEnvelopes.Length; idx++)
                    if (Math.Abs(cacheMAEnvelopes[idx].EnvelopePercentage - envelopePercentage) <= double.Epsilon && cacheMAEnvelopes[idx].MAType == mAType && cacheMAEnvelopes[idx].Period == period && cacheMAEnvelopes[idx].EqualsInput(input))
                        return cacheMAEnvelopes[idx];

            lock (checkMAEnvelopes)
            {
                checkMAEnvelopes.EnvelopePercentage = envelopePercentage;
                envelopePercentage = checkMAEnvelopes.EnvelopePercentage;
                checkMAEnvelopes.MAType = mAType;
                mAType = checkMAEnvelopes.MAType;
                checkMAEnvelopes.Period = period;
                period = checkMAEnvelopes.Period;

                if (cacheMAEnvelopes != null)
                    for (int idx = 0; idx < cacheMAEnvelopes.Length; idx++)
                        if (Math.Abs(cacheMAEnvelopes[idx].EnvelopePercentage - envelopePercentage) <= double.Epsilon && cacheMAEnvelopes[idx].MAType == mAType && cacheMAEnvelopes[idx].Period == period && cacheMAEnvelopes[idx].EqualsInput(input))
                            return cacheMAEnvelopes[idx];

                MAEnvelopes indicator = new MAEnvelopes();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.EnvelopePercentage = envelopePercentage;
                indicator.MAType = mAType;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                MAEnvelopes[] tmp = new MAEnvelopes[cacheMAEnvelopes == null ? 1 : cacheMAEnvelopes.Length + 1];
                if (cacheMAEnvelopes != null)
                    cacheMAEnvelopes.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheMAEnvelopes = tmp;
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
        /// Plots % envelopes around a moving average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MAEnvelopes MAEnvelopes(double envelopePercentage, int mAType, int period)
        {
            return _indicator.MAEnvelopes(Input, envelopePercentage, mAType, period);
        }

        /// <summary>
        /// Plots % envelopes around a moving average
        /// </summary>
        /// <returns></returns>
        public Indicator.MAEnvelopes MAEnvelopes(Data.IDataSeries input, double envelopePercentage, int mAType, int period)
        {
            return _indicator.MAEnvelopes(input, envelopePercentage, mAType, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Plots % envelopes around a moving average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MAEnvelopes MAEnvelopes(double envelopePercentage, int mAType, int period)
        {
            return _indicator.MAEnvelopes(Input, envelopePercentage, mAType, period);
        }

        /// <summary>
        /// Plots % envelopes around a moving average
        /// </summary>
        /// <returns></returns>
        public Indicator.MAEnvelopes MAEnvelopes(Data.IDataSeries input, double envelopePercentage, int mAType, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.MAEnvelopes(input, envelopePercentage, mAType, period);
        }
    }
}
#endregion
