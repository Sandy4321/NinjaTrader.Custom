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
	public class RealizedProfitLoss : NinjaTrader.MarketAnalyzer.Column
	{
		#region Variables
		private string					accountName		= Connection.SimulationAccountName;
		private	ExecutionCollection		executions		= new ExecutionCollection();
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
			Value = 0;
			lock (Cbi.Globals.Connections)
				foreach (Connection connection in Cbi.Globals.Connections)
					if (connection.Status == ConnectionStatus.Connected || connection.Status == ConnectionStatus.ConnectionLost)
						lock (connection.Accounts)
							foreach (Account account in connection.Accounts)
								if (account.Name == AccountName)
								{
									executions.Clear();
									lock (account.Executions)
										foreach (Execution tmp in account.Executions)
											if (tmp.Instrument.IsEqual(Instrument))
												executions.Add(tmp);

									Value = Strategy.SystemPerformance.Calculate(executions, Cbi.Commission.ApplyCommissionToProfitLoss).AllTrades.TradesPerformance.Currency.CumProfit;
								}
		}

		/// <summary>
		/// Called on a new fill.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnExecutionUpdate(ExecutionUpdateEventArgs e)
		{
			if (e.Account.Name != AccountName || !e.Instrument.IsEqual(Instrument))
				return;

			executions.Add(e.Execution);
			Value = Strategy.SystemPerformance.Calculate(executions, Cbi.Commission.ApplyCommissionToProfitLoss).AllTrades.TradesPerformance.Currency.CumProfit;
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
			if (CellConditions.Count == 0)
				ForeColor = (value >= 0 ? Color.Empty : Color.Red);
			return Gui.Globals.FormatCurrency(value, Instrument.MasterInstrument.Currency);
		}
		#endregion
	}
}
