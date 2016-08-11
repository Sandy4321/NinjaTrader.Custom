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
	/// The balance of power indicator measures the strength of the bulls vs. bears by assessing the ability of each to push price to an extreme level.
	/// </summary>
	[Description("The balance of power indicator measures the strength of the bulls vs. bears by assessing the ability of each to push price to an extreme level.")]
	public class BOP : Indicator
	{
		#region Variables
		private int				smooth	= 14;
		private DataSeries	bop;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Blue, 2), PlotStyle.Bar, "BOP"));

			bop					= new DataSeries(this);
			Overlay				= false;
		}
		
		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if ((High[0] - Low[0]) == 0) 
			{
				bop.Set(0);
			} 
			else 
			{
				bop.Set((Close[0] - Open[0]) / (High[0] - Low[0]));
			}

			Value.Set(SMA(bop, smooth)[0]);
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Periods to use for SMA smoothing of the indicator.")]
		[GridCategory("Parameters")]
		public int Smooth
		{
			get { return smooth; }
			set { smooth = Math.Max(1, value); }
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
        private BOP[] cacheBOP = null;

        private static BOP checkBOP = new BOP();

        /// <summary>
        /// The balance of power indicator measures the strength of the bulls vs. bears by assessing the ability of each to push price to an extreme level.
        /// </summary>
        /// <returns></returns>
        public BOP BOP(int smooth)
        {
            return BOP(Input, smooth);
        }

        /// <summary>
        /// The balance of power indicator measures the strength of the bulls vs. bears by assessing the ability of each to push price to an extreme level.
        /// </summary>
        /// <returns></returns>
        public BOP BOP(Data.IDataSeries input, int smooth)
        {
            if (cacheBOP != null)
                for (int idx = 0; idx < cacheBOP.Length; idx++)
                    if (cacheBOP[idx].Smooth == smooth && cacheBOP[idx].EqualsInput(input))
                        return cacheBOP[idx];

            lock (checkBOP)
            {
                checkBOP.Smooth = smooth;
                smooth = checkBOP.Smooth;

                if (cacheBOP != null)
                    for (int idx = 0; idx < cacheBOP.Length; idx++)
                        if (cacheBOP[idx].Smooth == smooth && cacheBOP[idx].EqualsInput(input))
                            return cacheBOP[idx];

                BOP indicator = new BOP();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Smooth = smooth;
                Indicators.Add(indicator);
                indicator.SetUp();

                BOP[] tmp = new BOP[cacheBOP == null ? 1 : cacheBOP.Length + 1];
                if (cacheBOP != null)
                    cacheBOP.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheBOP = tmp;
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
        /// The balance of power indicator measures the strength of the bulls vs. bears by assessing the ability of each to push price to an extreme level.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BOP BOP(int smooth)
        {
            return _indicator.BOP(Input, smooth);
        }

        /// <summary>
        /// The balance of power indicator measures the strength of the bulls vs. bears by assessing the ability of each to push price to an extreme level.
        /// </summary>
        /// <returns></returns>
        public Indicator.BOP BOP(Data.IDataSeries input, int smooth)
        {
            return _indicator.BOP(input, smooth);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The balance of power indicator measures the strength of the bulls vs. bears by assessing the ability of each to push price to an extreme level.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BOP BOP(int smooth)
        {
            return _indicator.BOP(Input, smooth);
        }

        /// <summary>
        /// The balance of power indicator measures the strength of the bulls vs. bears by assessing the ability of each to push price to an extreme level.
        /// </summary>
        /// <returns></returns>
        public Indicator.BOP BOP(Data.IDataSeries input, int smooth)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.BOP(input, smooth);
        }
    }
}
#endregion
