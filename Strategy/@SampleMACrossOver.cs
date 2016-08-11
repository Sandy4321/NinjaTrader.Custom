// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
	/// <summary>
	/// Simple moving average cross over strategy.
	/// </summary>
	[Description("Simple moving average cross over strategy.")]
	public class SampleMACrossOver : Strategy
	{
		#region Variables
		private int		fast	= 10;
		private int		slow	= 25;
		#endregion

		/// <summary>
		/// This method is used to configure the strategy and is called once before any strategy method is called.
		/// </summary>
		protected override void Initialize()
		{
			SMA(Fast).Plots[0].Pen.Color = Color.Orange;
			SMA(Slow).Plots[0].Pen.Color = Color.Green;

            Add(SMA(Fast));
            Add(SMA(Slow));

			CalculateOnBarClose	= true;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick).
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CrossAbove(SMA(Fast), SMA(Slow), 1))
			    EnterLong();
			else if (CrossBelow(SMA(Fast), SMA(Slow), 1))
			    EnterShort();
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Period for fast MA")]
		[GridCategory("Parameters")]
		public int Fast
		{
			get { return fast; }
			set { fast = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Period for slow MA")]
		[GridCategory("Parameters")]
		public int Slow
		{
			get { return slow; }
			set { slow = Math.Max(1, value); }
		}
		#endregion
	}
}
