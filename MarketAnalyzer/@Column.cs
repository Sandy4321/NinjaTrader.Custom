// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.MarketAnalyzer;
#endregion

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
	/// <summary>
	/// </summary>
	public partial class Column : NinjaTrader.MarketAnalyzer.ColumnBase
	{
		private NinjaTrader.Indicator.ADX	_indicator = new NinjaTrader.Indicator.ADX();

		/// <summary>
		/// </summary>
		public Column()
		{
			_indicator.Input = Input;
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
