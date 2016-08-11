// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
//
#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
	/// <summary>
	/// </summary>
	[Gui.Design.DisplayName("max. probability")]
	public class MaxProbablity : OptimizationType
	{
		/// <summary>
		/// Return the performance value of a backtesting result.
		/// </summary>
		/// <param name="systemPerformance"></param>
		/// <returns></returns>
		public override double GetPerformanceValue(SystemPerformance systemPerformance)
		{
			if (systemPerformance.AllTrades.TradesCount <= 1 || systemPerformance.AllTrades.TradesPerformance.Percent.AvgProfit == 0)
				return 0;
			else
			{
				double div	= systemPerformance.AllTrades.TradesPerformance.Percent.StdDev / Math.Sqrt(systemPerformance.AllTrades.TradesCount);
				double t	= Stat.StudTp(systemPerformance.AllTrades.TradesPerformance.Percent.AvgProfit / div, systemPerformance.AllTrades.TradesCount - 1);
				return (div <= 0.5 ? 1 - t : t);
			}
		}
	}
}
