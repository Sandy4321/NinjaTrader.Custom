// 
// Copyright (C) 2007, NinjaTrader LLC <ninjatrader@ninjatrader.com>.
//
#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
#endregion

// This namespace holds all tick filters. Do not change it.
namespace NinjaTrader.Data
{
	/// <summary>
	/// Default tick filter.
	/// </summary>
	public class DefaultTickFilter : TickFilter
	{
		#region Variables
		private		int		badTickCounter	= 0;
		#endregion
		
		/// <summary>
		/// This method is used to filter ticks is called once for every incoming tick.
		/// </summary>
		/// <returns>true = tick is valid, false = tick is invalid</returns>
		public override bool OnTick()
		{
			// After 2 or more "bad" ticks we suppose market just gapped up
			if (Math.Abs(Price - LastPrice) / LastPrice > FilterOffset && ++badTickCounter < 3)
				return false;

			badTickCounter = 0;
			return true;
		}
	}
}
