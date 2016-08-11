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
using NinjaTrader.Gui.Chart;
using NinjaTrader.Strategy;
#endregion

namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Quickly set stoploss and entryprice and conduct santity check before placing the manually initiated second entry
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class L2Sell : Strategy
    {
        #region Variables
            private double profitFactor = 2;
            private IOrder orderEntry;
            private IOrder orderProfitTarget;
            private IOrder orderStopLoss;
            private double stopLossPx = 0.0;
            private double profitTargetPx = 0.0;
            private double entryPx = 0.0;
            private int orderQty = 1;
            private bool enableEntry = true;
        #endregion

        protected override void Initialize()
        {
            CalculateOnBarClose = true;
            Unmanaged = true;
            //orderEntry = orderProfitTarget = orderStopLoss = null;
            stopLossPx = profitTargetPx = entryPx = 0.0;
            enableEntry = true;
        }

        protected override void OnBarUpdate()
        {
            if (Historical)
                return;

            if (enableEntry == true)
            {
                orderEntry = SubmitOrder(0, OrderAction.SellShort, OrderType.Market, orderQty, 0, 0, "L2Entry" + Time[0], "L2");
                //entryPrice = orderEntry.AvgFillPrice;
                //signalStop = MIN(Low, 4)[0] + TickSize;
                //profitTarget = Bars.Instrument.MasterInstrument.Round2TickSize(Close[0] - Math.Abs(Close[0] - signalStop) * profitFactor);
                //Print("Entry: " + entryPx + " |  Stop Loss: " + stopLossPx + "   Profit Target: " + profitTargetPx);
                Print("Risk Exposure: " + (-1 * Math.Abs(entryPx - stopLossPx)) * Instrument.MasterInstrument.PointValue + " Profit Potential: " + (Math.Abs(profitTargetPx - entryPx)) * Instrument.MasterInstrument.PointValue);
            }
          /*  if ((Low[0] <= Low[1] - TickSize) && orderEntry == null)
            {
                if (Close[0] > Low[1] + TickSize)
                    orderEntry = SubmitOrder(0, OrderAction.SellShort, OrderType.Stop, orderQty, 0, Low[1], "L2Entry" + Time[0], "L2");
                if (Close[0] <= Low[1])
                    orderEntry = SubmitOrder(0, OrderAction.SellShort, OrderType.Market, orderQty, 0, 0, "L2Entry" + Time[0], "L2");

                if (orderEntry != null)
                {
                    entryPx = orderEntry.AvgFillPrice;
                    stopLossPx = MAX(High, 4)[0] + TickSize;
                    profitTargetPx = Bars.Instrument.MasterInstrument.Round2TickSize(entryPx - Math.Abs(entryPx - stopLossPx) * profitFactor);

                    Print("Entry: " + entryPx + " |  Stop Loss: " + stopLossPx + "   Profit Target: " + profitTargetPx);
                    Print("Diff Stop: " + (stopLossPx - entryPx) + " Diff Profit: " + (profitTargetPx - entryPx));
                    Print("Risk Exposure: " + (-1 * Math.Abs(entryPx - stopLossPx)) * Instrument.MasterInstrument.PointValue + " Profit Potential: " + (Math.Abs(profitTargetPx - entryPx)) * Instrument.MasterInstrument.PointValue);
                }
   
            }
   
           * */}
        protected override void OnExecution(IExecution execution)
        {
            if (orderEntry != null && orderEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                enableEntry = false;
                entryPx = orderEntry.AvgFillPrice;
                stopLossPx = MAX(High, 4)[0] + TickSize;
                profitTargetPx = Bars.Instrument.MasterInstrument.Round2TickSize(entryPx - (Math.Abs(entryPx - stopLossPx) * profitFactor));
                Print("Entry: " + entryPx + " |  Stop Loss: " + stopLossPx + "   Profit Target: " + profitTargetPx);
                Print("Diff Stop: " + (stopLossPx - entryPx) + " Diff Profit: " + (profitTargetPx - entryPx));

                orderStopLoss = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, execution.Order.Filled, 0, stopLossPx, "L2Exit" + orderEntry.OrderId, "L2.SL");
                orderProfitTarget = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, execution.Order.Filled, profitTargetPx, 0, "L2Exit" + orderEntry.OrderId, "L2.PT");
            }
        }

        #region Properties
        #endregion
    }
}
