// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// The RSI (Relative Strength Index) is a price-following oscillator that ranges between 0 and 100.
	/// </summary>
	[Description("The RSI (Relative Strength Index) is a price-following oscillator that ranges between 0 and 100.")]
	public class RSI : Indicator
	{
		#region Variables
		private DataSeries					avgUp;
		private DataSeries					avgDown;
		private DataSeries					down;
		private int								period	= 14;
		private int								smooth	= 3;
		private DataSeries					up;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Green, "RSI"));
			Add(new Plot(Color.Orange, "Avg"));

			Add(new Line(System.Drawing.Color.DarkViolet, 30, "Lower"));
			Add(new Line(System.Drawing.Color.YellowGreen, 70, "Upper"));

			avgUp				= new DataSeries(this);
			avgDown				= new DataSeries(this);
			down				= new DataSeries(this);
			up					= new DataSeries(this);
		}

		/// <summary>
		/// Calculates the indicator value(s) at the current index.
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				down.Set(0);
				up.Set(0);

                if (Period < 3)
                    Avg.Set(50);
				return;
			}

			down.Set(Math.Max(Input[1] - Input[0], 0));
			up.Set(Math.Max(Input[0] - Input[1], 0));

			if ((CurrentBar + 1) < Period) 
			{
				if ((CurrentBar + 1) == (Period - 1))
					Avg.Set(50);
				return;
			}

			if ((CurrentBar + 1) == Period) 
			{
				// First averages 
				avgDown.Set(SMA(down, Period)[0]);
				avgUp.Set(SMA(up, Period)[0]);
			}  
			else 
			{
				// Rest of averages are smoothed
				avgDown.Set((avgDown[1] * (Period - 1) + down[0]) / Period);
				avgUp.Set((avgUp[1] * (Period - 1) + up[0]) / Period);
			}

			double rsi	  = avgDown[0] == 0 ? 100 : 100 - 100 / (1 + avgUp[0] / avgDown[0]);
			double rsiAvg = (2.0 / (1 + Smooth)) * rsi + (1 - (2.0 / (1 + Smooth))) * Avg[1];

			Avg.Set(rsiAvg);
			Value.Set(rsi);
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Avg
		{
			get { return Values[1]; }
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
		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
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
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	public partial class Indicator : IndicatorBase
	{
		private RSI[] cacheRSI = null;

		private static RSI checkRSI = new RSI();

		/// <summary>
		/// The RSI (Relative Strength Index) is a price-following oscillator that ranges between 0 and 100.
		/// </summary>
		/// <returns></returns>
		public RSI RSI(int period, int smooth)
		{
			return RSI(Input, period, smooth);
		}

		/// <summary>
		/// The RSI (Relative Strength Index) is a price-following oscillator that ranges between 0 and 100.
		/// </summary>
		/// <returns></returns>
		public RSI RSI(Data.IDataSeries input, int period, int smooth)
		{
			if (cacheRSI != null)
				for (int idx = 0; idx < cacheRSI.Length; idx++)
					if (cacheRSI[idx].Period == period && cacheRSI[idx].Smooth == smooth && cacheRSI[idx].EqualsInput(input))
						return cacheRSI[idx];

			lock (checkRSI)
			{
				checkRSI.Period = period;
				period = checkRSI.Period;
				checkRSI.Smooth = smooth;
				smooth = checkRSI.Smooth;

				if (cacheRSI != null)
					for (int idx = 0; idx < cacheRSI.Length; idx++)
						if (cacheRSI[idx].Period == period && cacheRSI[idx].Smooth == smooth && cacheRSI[idx].EqualsInput(input))
							return cacheRSI[idx];

				RSI indicator = new RSI();
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

				RSI[] tmp = new RSI[cacheRSI == null ? 1 : cacheRSI.Length + 1];
				if (cacheRSI != null)
					cacheRSI.CopyTo(tmp, 0);
				tmp[tmp.Length - 1] = indicator;
				cacheRSI = tmp;
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
		/// The RSI (Relative Strength Index) is a price-following oscillator that ranges between 0 and 100.
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.RSI RSI(int period, int smooth)
		{
			return _indicator.RSI(Input, period, smooth);
		}

		/// <summary>
		/// The RSI (Relative Strength Index) is a price-following oscillator that ranges between 0 and 100.
		/// </summary>
		/// <returns></returns>
		public Indicator.RSI RSI(Data.IDataSeries input, int period, int smooth)
		{
			return _indicator.RSI(input, period, smooth);
		}
	}
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
	public partial class Strategy : StrategyBase
	{
		/// <summary>
		/// The RSI (Relative Strength Index) is a price-following oscillator that ranges between 0 and 100.
		/// </summary>
		/// <returns></returns>
		[Gui.Design.WizardCondition("Indicator")]
		public Indicator.RSI RSI(int period, int smooth)
		{
			return _indicator.RSI(Input, period, smooth);
		}

		/// <summary>
		/// The RSI (Relative Strength Index) is a price-following oscillator that ranges between 0 and 100.
		/// </summary>
		/// <returns></returns>
		public Indicator.RSI RSI(Data.IDataSeries input, int period, int smooth)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return _indicator.RSI(input, period, smooth);
		}
	}
}
#endregion
