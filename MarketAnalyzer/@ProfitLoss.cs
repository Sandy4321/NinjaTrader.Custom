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
	public class ProfitLoss : NinjaTrader.MarketAnalyzer.Column
	{
		#region Variables
		private string					accountName		= Connection.SimulationAccountName;
		private	ExecutionCollection		executions		= new ExecutionCollection();
		private	Position				position		= null;		// holds the position for the actual instrument
		private	double					realizedPL		= 0;
		private	double					unrealizedPL	= 0;
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
							lock (account.Positions)
								foreach (Position positionTmp in account.Positions)
									if (positionTmp.Instrument.IsEqual(Instrument))
										position = positionTmp;
			}
			else if (e.Status == ConnectionStatus.Disconnected)
			{
				if (position != null && position.Account.Connection == e.Connection)
				{
					position		= null;
					unrealizedPL	= 0;
					Value			= 0;
				}
			}

			realizedPL = 0;
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

									realizedPL = Strategy.SystemPerformance.Calculate(executions, Cbi.Commission.ApplyCommissionToProfitLoss).AllTrades.TradesPerformance.Currency.CumProfit;
								}

			Value = realizedPL + unrealizedPL;
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
			realizedPL = Strategy.SystemPerformance.Calculate(executions, Cbi.Commission.ApplyCommissionToProfitLoss).AllTrades.TradesPerformance.Currency.CumProfit;

			if (e.Execution.LastExit)
				unrealizedPL = 0;

			Value = realizedPL + unrealizedPL;
		}

		/// <summary>
		/// Called on each incoming market data tick.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMarketData(MarketDataEventArgs e)
		{
			double price = 0;
			if (position == null)
				unrealizedPL = 0;
			else if (position.Instrument.MasterInstrument.InstrumentType == InstrumentType.Currency || !Cbi.Position.UseLastPrice4PL)
			{
				if (e.MarketDataType == MarketDataType.Bid && position.MarketPosition == MarketPosition.Long)
					price = e.Price;
				else if (e.MarketDataType == MarketDataType.Ask && position.MarketPosition == MarketPosition.Short)
					price = e.Price;
				else
					return;

				unrealizedPL = position.GetProfitLoss(price, Strategy.PerformanceUnit.Currency);
			}
			else if (e.MarketDataType == MarketDataType.Last)
				unrealizedPL = position.GetProfitLoss(e.Price, Strategy.PerformanceUnit.Currency);
			else
				return;

			Value = realizedPL + unrealizedPL;
		}

		/// <summary>
		/// Called when a position is modified.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPositionUpdate(Cbi.PositionUpdateEventArgs e)
		{
			if (e.Position.Account.Name == AccountName && e.Position.Instrument.IsEqual(Instrument))
			{
				position = (e.Operation == Operation.Remove ? null : e.Position);
				if (position == null)
				{
					unrealizedPL = 0;
					Value = realizedPL + unrealizedPL;
				}
				else
				{
					double price = 0;
                    if (position.Instrument.MasterInstrument.InstrumentType == InstrumentType.Currency || !Cbi.Position.UseLastPrice4PL)
					{
						if (position.MarketPosition == MarketPosition.Long && position.Account.Connection.MarketDataStreams[e.Instrument].Bid != null)
							price = position.Account.Connection.MarketDataStreams[e.Instrument].Bid.Price;
						else if (position.MarketPosition == MarketPosition.Short && position.Account.Connection.MarketDataStreams[e.Instrument].Ask != null)
							price = position.Account.Connection.MarketDataStreams[e.Instrument].Ask.Price;
						else
							return;

						unrealizedPL = position.GetProfitLoss(price, Strategy.PerformanceUnit.Currency);
					}
					else if (position.Account.Connection.MarketDataStreams[e.Instrument].Last != null)
						unrealizedPL = position.GetProfitLoss(position.Account.Connection.MarketDataStreams[e.Instrument].Last.Price, Strategy.PerformanceUnit.Currency);
					else
						return;

					Value = realizedPL + unrealizedPL;
				}
			}
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
