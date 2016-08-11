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
	[Gui.Design.DisplayName("max. win/loss ratio")]
	public class MaxWinLossRatio : OptimizationType
	{
		/// <summary>
		/// Return the performance value of a backtesting result.
		/// </summary>
		/// <param name="systemPerformance"></param>
		/// <returns></returns>
		public override double GetPerformanceValue(SystemPerformance systemPerformance)
		{
			if (systemPerformance.AllTrades.LosingTrades.TradesPerformance.Percent.AvgProfit == 0)
				return 1;
			else
				return systemPerformance.AllTrades.WinningTrades.TradesPerformance.Percent.AvgProfit / Math.Abs(systemPerformance.AllTrades.LosingTrades.TradesPerformance.Percent.AvgProfit);
		}
	}
}
