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
	/// Chaikin Money Flow.
	/// </summary>
	[Description("Chaikin Money Flow.")]
	public class ChaikinMoneyFlow : Indicator
	{
		#region Variables
		private int						period		= 21;
		private	DataSeries			moneyFlow;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Orange, "Chaikin Money Flow"));
			
			moneyFlow			= new DataSeries(this);
		}

        /// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			moneyFlow.Set(Volume[0] * ((Close[0]  - Low[0]) - (High[0] - Close[0])) / ((High[0] - Low[0]) == 0 ? 1 : (High[0] - Low[0])));
			Value.Set(100 * SUM(moneyFlow, Period)[0] / SUM(Volume, Period)[0]);
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
        private ChaikinMoneyFlow[] cacheChaikinMoneyFlow = null;

        private static ChaikinMoneyFlow checkChaikinMoneyFlow = new ChaikinMoneyFlow();

        /// <summary>
        /// Chaikin Money Flow.
        /// </summary>
        /// <returns></returns>
        public ChaikinMoneyFlow ChaikinMoneyFlow(int period)
        {
            return ChaikinMoneyFlow(Input, period);
        }

        /// <summary>
        /// Chaikin Money Flow.
        /// </summary>
        /// <returns></returns>
        public ChaikinMoneyFlow ChaikinMoneyFlow(Data.IDataSeries input, int period)
        {
            if (cacheChaikinMoneyFlow != null)
                for (int idx = 0; idx < cacheChaikinMoneyFlow.Length; idx++)
                    if (cacheChaikinMoneyFlow[idx].Period == period && cacheChaikinMoneyFlow[idx].EqualsInput(input))
                        return cacheChaikinMoneyFlow[idx];

            lock (checkChaikinMoneyFlow)
            {
                checkChaikinMoneyFlow.Period = period;
                period = checkChaikinMoneyFlow.Period;

                if (cacheChaikinMoneyFlow != null)
                    for (int idx = 0; idx < cacheChaikinMoneyFlow.Length; idx++)
                        if (cacheChaikinMoneyFlow[idx].Period == period && cacheChaikinMoneyFlow[idx].EqualsInput(input))
                            return cacheChaikinMoneyFlow[idx];

                ChaikinMoneyFlow indicator = new ChaikinMoneyFlow();
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

                ChaikinMoneyFlow[] tmp = new ChaikinMoneyFlow[cacheChaikinMoneyFlow == null ? 1 : cacheChaikinMoneyFlow.Length + 1];
                if (cacheChaikinMoneyFlow != null)
                    cacheChaikinMoneyFlow.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheChaikinMoneyFlow = tmp;
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
        /// Chaikin Money Flow.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ChaikinMoneyFlow ChaikinMoneyFlow(int period)
        {
            return _indicator.ChaikinMoneyFlow(Input, period);
        }

        /// <summary>
        /// Chaikin Money Flow.
        /// </summary>
        /// <returns></returns>
        public Indicator.ChaikinMoneyFlow ChaikinMoneyFlow(Data.IDataSeries input, int period)
        {
            return _indicator.ChaikinMoneyFlow(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Chaikin Money Flow.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ChaikinMoneyFlow ChaikinMoneyFlow(int period)
        {
            return _indicator.ChaikinMoneyFlow(Input, period);
        }

        /// <summary>
        /// Chaikin Money Flow.
        /// </summary>
        /// <returns></returns>
        public Indicator.ChaikinMoneyFlow ChaikinMoneyFlow(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ChaikinMoneyFlow(input, period);
        }
    }
}
#endregion
