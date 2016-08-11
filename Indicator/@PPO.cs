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
	/// The PPO (Percentage Price Oscillator) is based on two moving averages expressed as a percentage. The PPO is found by subtracting the longer MA from the shorter MA and then dividing the difference by the longer MA.
	/// </summary>
	[Description("The PPO (Percentage Price Oscillator) is based on two moving averages expressed as a percentage. The PPO is found by subtracting the longer MA from the shorter MA and then dividing the difference by the longer MA.")]
	public class PPO : Indicator
	{
		#region Variables
		private int					fast	= 12;
		private int					slow	= 26;
		private int					smooth	= 9;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Line(Color.DarkGray, 0, "Zero line"));
			Add(new Plot(Color.Black, "Default"));
			Add(new Plot(Color.Red, "Smoothed"));
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			double val = 100 * ((EMA(Fast)[0] - EMA(Slow)[0]) / EMA(Slow)[0]);
			Default.Set(val);
			Smoothed.Set(EMA(Value, smooth)[0]);
		}

		#region Properties
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
		[Description("Number of bars for fast EMA")]
		[GridCategory("Parameters")]
		public int Fast
		{
			get { return fast; }
			set { fast = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Number of bars for slow EMA")]
		[GridCategory("Parameters")]
		public int Slow
		{
			get { return slow; }
			set { slow = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Number of bars for smoothing")]
		[GridCategory("Parameters")]
		public int Smooth
		{
			get { return smooth; }
			set { smooth = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Smoothed
		{
			get { return Values[1]; }
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
        private PPO[] cachePPO = null;

        private static PPO checkPPO = new PPO();

        /// <summary>
        /// The PPO (Percentage Price Oscillator) is based on two moving averages expressed as a percentage. The PPO is found by subtracting the longer MA from the shorter MA and then dividing the difference by the longer MA.
        /// </summary>
        /// <returns></returns>
        public PPO PPO(int fast, int slow, int smooth)
        {
            return PPO(Input, fast, slow, smooth);
        }

        /// <summary>
        /// The PPO (Percentage Price Oscillator) is based on two moving averages expressed as a percentage. The PPO is found by subtracting the longer MA from the shorter MA and then dividing the difference by the longer MA.
        /// </summary>
        /// <returns></returns>
        public PPO PPO(Data.IDataSeries input, int fast, int slow, int smooth)
        {
            if (cachePPO != null)
                for (int idx = 0; idx < cachePPO.Length; idx++)
                    if (cachePPO[idx].Fast == fast && cachePPO[idx].Slow == slow && cachePPO[idx].Smooth == smooth && cachePPO[idx].EqualsInput(input))
                        return cachePPO[idx];

            lock (checkPPO)
            {
                checkPPO.Fast = fast;
                fast = checkPPO.Fast;
                checkPPO.Slow = slow;
                slow = checkPPO.Slow;
                checkPPO.Smooth = smooth;
                smooth = checkPPO.Smooth;

                if (cachePPO != null)
                    for (int idx = 0; idx < cachePPO.Length; idx++)
                        if (cachePPO[idx].Fast == fast && cachePPO[idx].Slow == slow && cachePPO[idx].Smooth == smooth && cachePPO[idx].EqualsInput(input))
                            return cachePPO[idx];

                PPO indicator = new PPO();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Fast = fast;
                indicator.Slow = slow;
                indicator.Smooth = smooth;
                Indicators.Add(indicator);
                indicator.SetUp();

                PPO[] tmp = new PPO[cachePPO == null ? 1 : cachePPO.Length + 1];
                if (cachePPO != null)
                    cachePPO.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachePPO = tmp;
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
        /// The PPO (Percentage Price Oscillator) is based on two moving averages expressed as a percentage. The PPO is found by subtracting the longer MA from the shorter MA and then dividing the difference by the longer MA.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PPO PPO(int fast, int slow, int smooth)
        {
            return _indicator.PPO(Input, fast, slow, smooth);
        }

        /// <summary>
        /// The PPO (Percentage Price Oscillator) is based on two moving averages expressed as a percentage. The PPO is found by subtracting the longer MA from the shorter MA and then dividing the difference by the longer MA.
        /// </summary>
        /// <returns></returns>
        public Indicator.PPO PPO(Data.IDataSeries input, int fast, int slow, int smooth)
        {
            return _indicator.PPO(input, fast, slow, smooth);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The PPO (Percentage Price Oscillator) is based on two moving averages expressed as a percentage. The PPO is found by subtracting the longer MA from the shorter MA and then dividing the difference by the longer MA.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PPO PPO(int fast, int slow, int smooth)
        {
            return _indicator.PPO(Input, fast, slow, smooth);
        }

        /// <summary>
        /// The PPO (Percentage Price Oscillator) is based on two moving averages expressed as a percentage. The PPO is found by subtracting the longer MA from the shorter MA and then dividing the difference by the longer MA.
        /// </summary>
        /// <returns></returns>
        public Indicator.PPO PPO(Data.IDataSeries input, int fast, int slow, int smooth)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.PPO(input, fast, slow, smooth);
        }
    }
}
#endregion
