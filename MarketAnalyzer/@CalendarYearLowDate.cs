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
using NinjaTrader.MarketAnalyzer;
#endregion

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
	/// <summary>
	/// </summary>
	public class CalendarYearLowDate : NinjaTrader.MarketAnalyzer.Column
	{
		/// <summary>
		/// This method is used to configure the market analyzer column and is called once before any event mathod is called.
		/// </summary>
		protected override void Initialize()
		{
			CalculateOnBarCloseConfigurable	= false;
			RequiresBars					= false;
		}

		/// <summary>
		/// Called on each incoming market data tick.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFundamentalData(FundamentalDataEventArgs e)
		{
			if (e.FundamentalDataType != FundamentalDataType.CalendarYearLowDate)
				return;

			// handle as numeric value to make sure sorting works regardless of actual formatting
			Value = e.DateTimeValue.Subtract(Cbi.Globals.MinDate).TotalDays;
		}

		#region Miscellaneous
		/// <summary>
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected override string Format(double value)
		{
			return Cbi.Globals.MinDate.AddDays(value).ToString(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
		}
		#endregion
	}
}
