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
	/// The Sum shows the summation of the last n data points.
	/// </summary>
	[Description("The Sum shows the summation of the last n data points.")]
	public class SUM : Indicator
	{
		#region Variables
		private int		period	= 14;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Green, "SUM"));
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			Value.Set(Input[0] + (CurrentBar > 0 ? Value[1] : 0) - (CurrentBar >= Period ? Input[Period] : 0));
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
        private SUM[] cacheSUM = null;

        private static SUM checkSUM = new SUM();

        /// <summary>
        /// The Sum shows the summation of the last n data points.
        /// </summary>
        /// <returns></returns>
        public SUM SUM(int period)
        {
            return SUM(Input, period);
        }

        /// <summary>
        /// The Sum shows the summation of the last n data points.
        /// </summary>
        /// <returns></returns>
        public SUM SUM(Data.IDataSeries input, int period)
        {
            if (cacheSUM != null)
                for (int idx = 0; idx < cacheSUM.Length; idx++)
                    if (cacheSUM[idx].Period == period && cacheSUM[idx].EqualsInput(input))
                        return cacheSUM[idx];

            lock (checkSUM)
            {
                checkSUM.Period = period;
                period = checkSUM.Period;

                if (cacheSUM != null)
                    for (int idx = 0; idx < cacheSUM.Length; idx++)
                        if (cacheSUM[idx].Period == period && cacheSUM[idx].EqualsInput(input))
                            return cacheSUM[idx];

                SUM indicator = new SUM();
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

                SUM[] tmp = new SUM[cacheSUM == null ? 1 : cacheSUM.Length + 1];
                if (cacheSUM != null)
                    cacheSUM.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSUM = tmp;
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
        /// The Sum shows the summation of the last n data points.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SUM SUM(int period)
        {
            return _indicator.SUM(Input, period);
        }

        /// <summary>
        /// The Sum shows the summation of the last n data points.
        /// </summary>
        /// <returns></returns>
        public Indicator.SUM SUM(Data.IDataSeries input, int period)
        {
            return _indicator.SUM(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Sum shows the summation of the last n data points.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SUM SUM(int period)
        {
            return _indicator.SUM(Input, period);
        }

        /// <summary>
        /// The Sum shows the summation of the last n data points.
        /// </summary>
        /// <returns></returns>
        public Indicator.SUM SUM(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SUM(input, period);
        }
    }
}
#endregion
