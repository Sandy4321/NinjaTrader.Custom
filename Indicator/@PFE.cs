// 
// Copyright (C) 2008, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// The PFE (Polarized Fractal Efficiency) is an indicator that uses fractal geometry to determine how efficiently the price is moving.
	/// </summary>
	[Description("The PFE (Polarized Fractal Efficiency) is an indicator that uses fractal geometry to determine how efficiently the price is moving.")]
	public class PFE : Indicator
	{
		#region Variables
		private DataSeries  div;
		private int         period      = 14;
		private DataSeries  pfeSeries;
		private DataSeries  singlePfeSeries;
		private int         smooth      = 10;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "PFE"));
			Add(new Line(Color.FromKnownColor(KnownColor.Gray), 0, "Zero"));

			pfeSeries	    = new DataSeries(this);
			singlePfeSeries = new DataSeries(this);
			div			    = new DataSeries(this);
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			singlePfeSeries.Set(CurrentBar == 0 ? 1 : Math.Sqrt(Math.Pow((Input[1] - Input[0]), 2) + 1));
			div.Set(singlePfeSeries[0] + (CurrentBar > 0 ? div[1] : 0) - (CurrentBar >= Period ? singlePfeSeries[Period] : 0));

			if (CurrentBar < Period)
				return;

			pfeSeries.Set((Input[0] < Input[Period] ? -1 : 1) * (Math.Sqrt(Math.Pow(Input[0] - Input[Period], 2) + Math.Pow(Period, 2)) / div[0]));
			Value.Set(EMA(pfeSeries, Smooth)[0]);
		}

		#region Properties
		[Description("Period")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
		}

		[Description("Smooth")]
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
		private PFE[] cachePFE = null;

		private static PFE checkPFE = new PFE();

		/// <summary>
		/// The PFE (Polarized Fractal Efficiency) is an indicator that uses fractal geometry to determine how efficiently the price is moving.
		/// </summary>
		/// <returns></returns>
		public PFE PFE(int period, int smooth)
		{
			return PFE(Input, period, smooth);
		}

		/// <summary>
		/// The PFE (Polarized Fractal Efficiency) is an indicator that uses fractal geometry to determine how efficiently the price is moving.
		/// </summary>
		/// <returns></returns>
		public PFE PFE(Data.IDataSeries input, int period, int smooth)
		{
			if (cachePFE != null)
				for (int idx = 0; idx < cachePFE.Length; idx++)
					if (cachePFE[idx].Period == period && cachePFE[idx].Smooth == smooth && cachePFE[idx].EqualsInput(input))
						return cachePFE[idx];

			lock (checkPFE)
			{
				checkPFE.Period = period;
				period = checkPFE.Period;
				checkPFE.Smooth = smooth;
				smooth = checkPFE.Smooth;

				if (cachePFE != null)
					for (int idx = 0; idx < cachePFE.Length; idx++)
						if (cachePFE[idx].Period == period && cachePFE[idx].Smooth == smooth && cachePFE[idx].EqualsInput(input))
							return cachePFE[idx];

				PFE indicator = new PFE();
				indicator.BarsRequired = BarsRequired;
				indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
				indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
				indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
				indicator.Input = input;
				indicator.Period = period;
				indicator.Smooth = smooth;
				Indicators.Add(indicator);
				indicator.SetUp();

				PFE[] tmp = new PFE[cachePFE == null ? 1 : cachePFE.Length + 1];
				if (cachePFE != null)
					cachePFE.CopyTo(tmp, 0);
				tmp[tmp.Length - 1] = indicator;
				cachePFE = tmp;
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
		/// The PFE (Polarized Fractal Efficiency) is an indicator that uses fractal geometry to determine how efficiently the price is moving.
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.PFE PFE(int period, int smooth)
		{
			return _indicator.PFE(Input, period, smooth);
		}

		/// <summary>
		/// The PFE (Polarized Fractal Efficiency) is an indicator that uses fractal geometry to determine how efficiently the price is moving.
		/// </summary>
		/// <returns></returns>
		public Indicator.PFE PFE(Data.IDataSeries input, int period, int smooth)
		{
			return _indicator.PFE(input, period, smooth);
		}
	}
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
	public partial class Strategy : StrategyBase
	{
		/// <summary>
		/// The PFE (Polarized Fractal Efficiency) is an indicator that uses fractal geometry to determine how efficiently the price is moving.
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.PFE PFE(int period, int smooth)
		{
			return _indicator.PFE(Input, period, smooth);
		}

		/// <summary>
		/// The PFE (Polarized Fractal Efficiency) is an indicator that uses fractal geometry to determine how efficiently the price is moving.
		/// </summary>
		/// <returns></returns>
		public Indicator.PFE PFE(Data.IDataSeries input, int period, int smooth)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return _indicator.PFE(input, period, smooth);
		}
	}
}
#endregion
