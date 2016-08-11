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
	/// </summary>
	public partial class Strategy : StrategyBase
	{
		private NinjaTrader.Indicator.ADX _indicator = new NinjaTrader.Indicator.ADX();

		/// <summary>
		/// </summary>
		public Strategy()
		{
			_indicator.Input	= Input;
			_indicator.Strategy	= this;
		}

		/// <summary>
		/// </summary>
		public override int BarsRequired
		{
			get { return base.BarsRequired; }
			set { base.BarsRequired = _indicator.BarsRequired = value; }
		}

		/// <summary>
		/// </summary>
		public override bool CalculateOnBarClose
		{
			get { return base.CalculateOnBarClose; }
			set { base.CalculateOnBarClose = _indicator.CalculateOnBarClose = value; }
		}
		
		/// <summary>
		/// </summary>
		public override void Dispose()
		{
			_indicator.Dispose();
			base.Dispose();
		}

		/// <summary>
		/// </summary>
		public override bool ForceMaximumBarsLookBack256
		{
			get { return base.ForceMaximumBarsLookBack256; }
			set { base.ForceMaximumBarsLookBack256 = _indicator.ForceMaximumBarsLookBack256 = value; }
		}

		/// <summary>
		/// </summary>
		public override IndicatorBase LeadIndicator
		{
			get	{ return _indicator; }
		}

		/// <summary>
		/// </summary>
		public override Data.MaximumBarsLookBack MaximumBarsLookBack
		{
			get { return base.MaximumBarsLookBack; }
			set { base.MaximumBarsLookBack = _indicator.MaximumBarsLookBack = value; }
		}
	}
}
			
