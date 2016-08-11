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
    public class H2Buy : Strategy
    {
        #region Variables
            private double profitFactor = 2;
            private IOrder orderEntry = null;
            private IOrder orderProfitTarget = null;
            private IOrder orderStopLoss = null;
            private double signalStop = 0.0;
            private double profitTarget = 0.0;
            private double entryPrice = 0.0;
            private int orderQty = 1;
        #endregion

        protected override void Initialize()
        {
            CalculateOnBarClose = true;
            Unmanaged = true;
            orderEntry = orderProfitTarget = orderStopLoss = null;
            signalStop = profitTarget = entryPrice = 0.0;
        }

        protected override void OnBarUpdate()
        {
            if ((High[0] >= High[1] + TickSize) && orderEntry == null)
            {

                if (Close[0] < High[1] - TickSize)
                    orderEntry = SubmitOrder(0, OrderAction.Buy, OrderType.Stop, orderQty, 0, High[1], "H2Entry", "H2");
                if (Close[0] >= High[1])
                    orderEntry = SubmitOrder(0, OrderAction.Buy, OrderType.Market, orderQty, 0, 0, "H2Entry", "H2");

                if (orderEntry != null)
                {
                    entryPrice = orderEntry.AvgFillPrice; 
                    signalStop = MIN(Low, 4)[0] - TickSize;
                    profitTarget = Bars.Instrument.MasterInstrument.Round2TickSize(entryPrice + Math.Abs(entryPrice-signalStop) * profitFactor);
                    
                    Print("Entry: " + entryPrice + " |  Stop Loss: " + signalStop + "   Profit Target: " + profitTarget);
                    Print("Risk Exposure: " + (-1*Math.Abs(entryPrice - signalStop)) * Instrument.MasterInstrument.PointValue + " Profit Potential: " + (Math.Abs(profitTarget - entryPrice)) * Instrument.MasterInstrument.PointValue);
                }
            }
        }
        protected override void OnExecution(IExecution execution)
        {
            if (orderEntry != null && orderEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                orderStopLoss = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Filled, 0, signalStop, "H2Exit", "H2.SL");
                orderProfitTarget = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Filled, profitTarget, 0, "H2Exit", "H2.PT");
            }
        }

        #region Properties
        #endregion
    }
}
