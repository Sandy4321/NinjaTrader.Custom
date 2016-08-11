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
	public class NetChange : NinjaTrader.MarketAnalyzer.Column
	{
		#region Variables
		private Strategy.PerformanceUnit	unit	= Strategy.PerformanceUnit.Percent;
		#endregion

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
		protected override void OnMarketData(MarketDataEventArgs e)
		{
			if (e.MarketDataType != MarketDataType.Last || e.MarketData.LastClose == null)
				return;

			switch (Unit)
			{
				case Strategy.PerformanceUnit.Currency:		Value = (e.Price - e.MarketData.LastClose.Price) * Instrument.MasterInstrument.PointValue;	break;
				case Strategy.PerformanceUnit.Points:		Value = (e.Price - e.MarketData.LastClose.Price);											break;	
				default:									Value = (e.Price - e.MarketData.LastClose.Price) / e.MarketData.LastClose.Price;			break;
			}
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Unit")]
		[Category("General")]
		public Strategy.PerformanceUnit Unit
		{
			get { return unit; }
			set { unit = value; }
		}
		#endregion

		#region Miscellaneous
		/// <summary>
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected override string Format(double value)
		{
			switch (Unit)
			{
				case Strategy.PerformanceUnit.Currency:	return Gui.Globals.FormatCurrency(value, Instrument.MasterInstrument.Currency);
				case Strategy.PerformanceUnit.Points:	return value.ToString(Gui.Globals.GetTickFormatString(Instrument.MasterInstrument.TickSize));
				default:								return (value * 100).ToString("0.00") + "%";
			}
		}
		#endregion
	}
}
