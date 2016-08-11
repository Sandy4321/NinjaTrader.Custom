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
	public class TradedContracts : NinjaTrader.MarketAnalyzer.Column
	{
		#region Variables
		private string		accountName		= Connection.SimulationAccountName;
		#endregion

		/// <summary>
		/// This method is used to configure the market analyzer column and is called once before any event method is called.
		/// </summary>
		protected override void Initialize()
		{
			CalculateOnBarCloseConfigurable	= false;
			RequiresBars					= false;
			ShowInTotalRow					= true;
		}

		/// <summary>
		/// Called on connection status change
		/// </summary>
		/// <param name="e"></param>
		protected override void OnConnectionStatus(ConnectionStatusEventArgs e)
		{
			if (e.Status == ConnectionStatus.Connected || e.OldStatus == ConnectionStatus.Connecting)
			{
				lock (e.Connection.Accounts)
					foreach (Account account in e.Connection.Accounts)
						if (account.Name == AccountName)
						{
							Value = 0;
							lock (account.Executions)
								foreach (Execution execution in account.Executions)
									if (execution.Instrument.IsEqual(Instrument) && execution.Time.Date == DateTime.Now.Date)
										Value += execution.Quantity;
						}
			}
			else if (e.Status == ConnectionStatus.Disconnected)
			{
				lock (e.Connection.Accounts)
					foreach (Account account in e.Connection.Accounts)
						if (account.Name == AccountName)
							Value = 0;
			}
		}

		/// <summary>
		/// Called on order fill.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnExecutionUpdate(ExecutionUpdateEventArgs e)
		{
			if (e.Operation == Operation.Insert && e.Execution.Instrument.IsEqual(Instrument) && e.Execution.Account.Name == AccountName)
				Value += e.Execution.Quantity;
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
