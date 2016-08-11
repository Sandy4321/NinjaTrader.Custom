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
	/// Fisher Transform. The Fisher Transform has sharp and distinct turning points that occur in a timely fashion. The resulting peak swings are used to identify price reversals.
	/// </summary>
	[Description("The Fisher Transform has sharp and distinct turning points that occur in a timely fashion. The resulting peak swings are used to identify price reversals.")]
	public class FisherTransform : Indicator
	{
		#region Variables
		private int				period	= 10;
		private DataSeries	tmpSeries;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Blue, 2), PlotStyle.Bar, "FisherTransform"));

			tmpSeries			= new DataSeries(this);
			Overlay				= false;
		}
		
		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			double fishPrev;
			double tmpValuePrev;

			if (CurrentBar == 0) 
			{
				fishPrev		= 0;
				tmpValuePrev	= 0;
			} 
			else
			{
				fishPrev		= Value[1];
				tmpValuePrev	= tmpSeries[1];
			}

			double minLo = MIN(Input, Period)[0];
			double maxHi = MAX(Input, Period)[0];

			double num1 = maxHi - minLo;

			// Guard agains infinite numbers and div by zero
			num1 = (num1 < TickSize / 10 ? TickSize / 10 : num1);

			double tmpValue = 0.66 * ((Input[0] - minLo) / num1 - 0.5) + 0.67 * tmpValuePrev;

			if (tmpValue > 0.99)
				tmpValue = 0.999;
			if (tmpValue < -0.99) 
				tmpValue = -0.999;
			
			tmpSeries.Set(tmpValue);

			double fishValue = 0.5 * Math.Log((1 + tmpValue) / (1 - tmpValue)) + 0.5 * fishPrev;
			Value.Set(fishValue);
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Period used for calculations.")]
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
        private FisherTransform[] cacheFisherTransform = null;

        private static FisherTransform checkFisherTransform = new FisherTransform();

        /// <summary>
        /// The Fisher Transform has sharp and distinct turning points that occur in a timely fashion. The resulting peak swings are used to identify price reversals.
        /// </summary>
        /// <returns></returns>
        public FisherTransform FisherTransform(int period)
        {
            return FisherTransform(Input, period);
        }

        /// <summary>
        /// The Fisher Transform has sharp and distinct turning points that occur in a timely fashion. The resulting peak swings are used to identify price reversals.
        /// </summary>
        /// <returns></returns>
        public FisherTransform FisherTransform(Data.IDataSeries input, int period)
        {
            if (cacheFisherTransform != null)
                for (int idx = 0; idx < cacheFisherTransform.Length; idx++)
                    if (cacheFisherTransform[idx].Period == period && cacheFisherTransform[idx].EqualsInput(input))
                        return cacheFisherTransform[idx];

            lock (checkFisherTransform)
            {
                checkFisherTransform.Period = period;
                period = checkFisherTransform.Period;

                if (cacheFisherTransform != null)
                    for (int idx = 0; idx < cacheFisherTransform.Length; idx++)
                        if (cacheFisherTransform[idx].Period == period && cacheFisherTransform[idx].EqualsInput(input))
                            return cacheFisherTransform[idx];

                FisherTransform indicator = new FisherTransform();
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

                FisherTransform[] tmp = new FisherTransform[cacheFisherTransform == null ? 1 : cacheFisherTransform.Length + 1];
                if (cacheFisherTransform != null)
                    cacheFisherTransform.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheFisherTransform = tmp;
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
        /// The Fisher Transform has sharp and distinct turning points that occur in a timely fashion. The resulting peak swings are used to identify price reversals.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.FisherTransform FisherTransform(int period)
        {
            return _indicator.FisherTransform(Input, period);
        }

        /// <summary>
        /// The Fisher Transform has sharp and distinct turning points that occur in a timely fashion. The resulting peak swings are used to identify price reversals.
        /// </summary>
        /// <returns></returns>
        public Indicator.FisherTransform FisherTransform(Data.IDataSeries input, int period)
        {
            return _indicator.FisherTransform(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Fisher Transform has sharp and distinct turning points that occur in a timely fashion. The resulting peak swings are used to identify price reversals.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.FisherTransform FisherTransform(int period)
        {
            return _indicator.FisherTransform(Input, period);
        }

        /// <summary>
        /// The Fisher Transform has sharp and distinct turning points that occur in a timely fashion. The resulting peak swings are used to identify price reversals.
        /// </summary>
        /// <returns></returns>
        public Indicator.FisherTransform FisherTransform(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.FisherTransform(input, period);
        }
    }
}
#endregion
