// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
//
#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
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
	[Gui.Design.DisplayName("Liberal")]
	public class LiberalFillType : FillType
	{
		private		const double	epsilon			= 0.00000001;

		/// <summary>
		/// Processes a historical order and checks for potential fills.
		/// </summary>
		/// <param name="order">Historical order to process</param>
		public override void Fill(Order order)
		{
			/* *** Slippage ***
			* Slippage values are optionally set in the UI and only applied to market and stop market orders
			* Slippage can only be applied to if the bar the order is filled on could have accomodated slippage
			* 
			* *** Limit Orders ***
			* Limit orders are filled if the limit price is touched 
			*/

			if (order.OrderType == OrderType.Market)
			{
				if (order.OrderAction == Cbi.OrderAction.Buy || order.OrderAction == Cbi.OrderAction.BuyToCover)			// set fill price
					FillPrice = Math.Min(NextHigh, NextOpen + SlippagePoints);
				else
					FillPrice = Math.Max(NextLow, NextOpen - SlippagePoints);
			}
			else if (order.OrderType == OrderType.Limit)
			{
				// Orders are filled when traded through the limit price not at the limit price
				double nextLow	= NextLow;
				double nextHigh = NextHigh;
				if ((order.OrderAction == Cbi.OrderAction.Buy					&& order.LimitPrice >= nextLow - epsilon)
						|| (order.OrderAction == Cbi.OrderAction.BuyToCover	&& order.LimitPrice >= nextLow - epsilon)
						|| (order.OrderAction == Cbi.OrderAction.Sell			&& order.LimitPrice <= nextHigh + epsilon)
						|| (order.OrderAction == Cbi.OrderAction.SellShort	&& order.LimitPrice <= nextHigh + epsilon))
					FillPrice = (order.OrderAction == Cbi.OrderAction.Buy || order.OrderAction == Cbi.OrderAction.BuyToCover) ? Math.Min(order.LimitPrice, nextHigh) : Math.Max(order.LimitPrice, nextLow);												// set fill price
			}
			else if (order.OrderType == OrderType.Stop)
			{
				// Stop orders are triggered when traded at the stop price
				double nextLow	= NextLow;
				double nextHigh = NextHigh;
				double nextOpen	= NextOpen;
				if ((order.OrderAction == Cbi.OrderAction.Buy					&& order.StopPrice <= nextHigh + epsilon)
						|| (order.OrderAction == Cbi.OrderAction.BuyToCover	&& order.StopPrice <= nextHigh + epsilon)
						|| (order.OrderAction == Cbi.OrderAction.Sell			&& order.StopPrice >= nextLow - epsilon)
						|| (order.OrderAction == Cbi.OrderAction.SellShort	&& order.StopPrice >= nextLow - epsilon))
				{
					if (order.OrderAction == Cbi.OrderAction.Buy || order.OrderAction == Cbi.OrderAction.BuyToCover)
						FillPrice = order.StopPrice < nextOpen - epsilon ? Math.Min(nextHigh, nextOpen + SlippagePoints) : Math.Min(nextHigh, order.StopPrice + SlippagePoints);
					else
						FillPrice = order.StopPrice > nextOpen + epsilon ? Math.Max(nextLow, nextOpen - SlippagePoints) : Math.Max(nextLow, order.StopPrice - SlippagePoints);
				}
			}
			else if (order.OrderType == OrderType.StopLimit)
			{
				// Stop limit orders are triggered when traded at the stop price and traded through the limit price
				double nextLow	= NextLow;
				double nextHigh = NextHigh;
				if (!order.StopTriggered
					&& ((order.OrderAction == Cbi.OrderAction.Buy				&& order.StopPrice <= nextHigh + epsilon)
						|| (order.OrderAction == Cbi.OrderAction.BuyToCover	&& order.StopPrice <= nextHigh + epsilon)
						|| (order.OrderAction == Cbi.OrderAction.Sell			&& order.StopPrice >= nextLow - epsilon)
						|| (order.OrderAction == Cbi.OrderAction.SellShort	&& order.StopPrice >= nextLow - epsilon)))
					order.StopTriggered = true;													// stop limit order was triggered

				if (order.StopTriggered
					&& ((order.OrderAction == Cbi.OrderAction.Buy				&& order.LimitPrice >= nextLow - epsilon)
						|| (order.OrderAction == Cbi.OrderAction.BuyToCover	&& order.LimitPrice >= nextLow - epsilon)
						|| (order.OrderAction == Cbi.OrderAction.Sell			&& order.LimitPrice <= nextHigh + epsilon)
						|| (order.OrderAction == Cbi.OrderAction.SellShort	&& order.LimitPrice <= nextHigh + epsilon)))
					FillPrice = (order.OrderAction == Cbi.OrderAction.Buy || order.OrderAction == Cbi.OrderAction.BuyToCover) ? Math.Min(order.LimitPrice, nextHigh) : Math.Max(order.LimitPrice, nextLow);
			}
		}
	}
}
