// 
// Copyright (C) 2011, NinjaTrader LLC <www.ninjatrader.com>.
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
    /// Sample demonstrating the use of the OnOrderUpdate() method.
    /// </summary>
    [Description("Sample strategy demonstrating a use case involving the OnOrderUpdate() method")]
    public class SampleOnOrderUpdate : Strategy
    {
        #region Variables
		private IOrder entryOrder 	= null; // This variable holds an object representing our entry order
		private IOrder stopOrder 	= null; // This variable holds an object representing our stop loss order
		private IOrder targetOrder 	= null; // This variable holds an object representing our profit target order
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = true;
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// Submit an entry limit order if we currently don't have an entry order open
			if (entryOrder == null && Close[0] > Open[0])
			{
				/* The entryOrder object will take on a unique ID from our EnterLong()
				that we can use later for order identification purposes in the OnOrderUpdate() method. */
				entryOrder = EnterLong(1, "MyEntry");
			}
			
			/* If we have a long position and the current price is 4 ticks in profit, raise the stop-loss order to breakeven.
			We use (7 * (TickSize / 2)) to denote 4 ticks because of potential precision issues with doubles. Under certain
			conditions (4 * TickSize) could end up being 3.9999 instead of 4 if the TickSize was 1. Using our method of determining
			4 ticks helps cope with the precision issue if it does arise. */
			if (Position.MarketPosition == MarketPosition.Long && Close[0] >= Position.AvgPrice + (7 * (TickSize / 2)))
			{
				// Checks to see if our Stop Order has been submitted already
				if (stopOrder != null && stopOrder.StopPrice < Position.AvgPrice)
				{
					// Modifies stop-loss to breakeven
					stopOrder = ExitLongStop(0, true, stopOrder.Quantity, Position.AvgPrice, "MyStop", "MyEntry");
				}
			}
        }

        /// <summary>
        /// Called on each incoming order event
        /// </summary>
        protected override void OnOrderUpdate(IOrder order)
        {
			// Handle entry orders here. The entryOrder object allows us to identify that the order that is calling the OnOrderUpdate() method is the entry order.
			if (entryOrder != null && entryOrder == order)
			{	
				// Reset the entryOrder object to null if order was cancelled without any fill
				if (order.OrderState == OrderState.Cancelled && order.Filled == 0)
				{
					entryOrder = null;
				}
			}
        }
		
		/// <summary>
        /// Called on each incoming execution
        /// </summary>
        protected override void OnExecution(IExecution execution)
        {
			/* We advise monitoring OnExecution to trigger submission of stop/target orders instead of OnOrderUpdate() since OnExecution() is called after OnOrderUpdate()
			which ensures your strategy has received the execution which is used for internal signal tracking. */
			if (entryOrder != null && entryOrder == execution.Order)
			{
				if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
				{
					// Stop-Loss order 4 ticks below our entry price
					stopOrder 	= ExitLongStop(0, true, execution.Order.Filled, execution.Order.AvgFillPrice - 4 * TickSize, "MyStop", "MyEntry");
					
					// Target order 8 ticks above our entry price
					targetOrder = ExitLongLimit(0, true, execution.Order.Filled, execution.Order.AvgFillPrice + 8 * TickSize, "MyTarget", "MyEntry");
					
					// Resets the entryOrder object to null after the order has been filled
					if (execution.Order.OrderState != OrderState.PartFilled)
					{
						entryOrder 	= null;
					}
				}
			}
			
			// Reset our stop order and target orders' IOrder objects after our position is closed.
			if ((stopOrder != null && stopOrder == execution.Order) || (targetOrder != null && targetOrder == execution.Order))
			{
				if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled)
				{
					stopOrder = null;
					targetOrder = null;
				}
			}
		}

        /// <summary>
        /// Called on each incoming position event
        /// </summary>
        protected override void OnPositionUpdate(IPosition position)
        {
			// Print our current position to the lower right hand corner of the chart
			DrawTextFixed("MyTag", position.ToString(), TextPosition.BottomRight);
        }

        #region Properties
        #endregion
    }
}
