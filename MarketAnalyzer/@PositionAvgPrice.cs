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
	public class PositionAvgPrice : NinjaTrader.MarketAnalyzer.Column
	{
		#region Variables
		private string		accountName		= Connection.SimulationAccountName;
		#endregion

		/// <summary>
		/// This method is used to configure the market analyzer column and is called once before any event mathod is called.
		/// </summary>
		protected override void Initialize()
		{
			CalculateOnBarCloseConfigurable	= false;
			FormatDecimals					= 0;
			RequiresBars					= false;
		}

		/// <summary>
		/// Called on connection status change
		/// </summary>
		/// <param name="e"></param>
		protected override void OnConnectionStatus(ConnectionStatusEventArgs e)
		{
			Value		= 0;
			BackColor	= Color.Empty;		// reset color
			lock (Cbi.Globals.Connections)
				foreach (Connection connection in Cbi.Globals.Connections)
					if (connection.Status == ConnectionStatus.Connected || connection.Status == ConnectionStatus.ConnectionLost)
						lock (connection.Accounts)
							foreach (Account account in connection.Accounts)
								if (account.Name == AccountName && account.Positions.FindByInstrument(Instrument) != null)
									Value = account.Positions.FindByInstrument(Instrument).AvgPrice;
		}

		/// <summary>
		/// Called when a position is modified.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPositionUpdate(Cbi.PositionUpdateEventArgs e)
		{
			if (!e.Position.Instrument.IsEqual(Instrument) || e.Account.Name != AccountName)
				return;

			Value = (e.Operation == Operation.Remove ? 0 : e.AvgPrice);
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Selected account")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayName("Selected account")]
		[TypeConverter(typeof(Gui.Design.AccountNameConverter))]
		public string AccountName
		{
			get { return accountName; }
			set { accountName = value; }
		}
		#endregion

		#region Miscellaneous
		/// <summary>
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected override string Format(double value)
		{
			return value.ToString();
		}
		#endregion
	}
}
