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

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Scalp outside up or ourside down bar when the outside triggers to trap traders on the first three minutes and reverses up in the last two minutes
    /// </summary>
    [Description("Scalp outside up or ourside down bar when the outside triggers to trap traders on the first three minutes and reverses up in the last two minutes")]
    public class outsideBar : Strategy
    {
        #region Variables
        private int quantity = 1;
        private double rewardToRisk =1;
        private IOrder LongEntry = null;
        private IOrder LongEntryProfitTarget = null;
        private IOrder LongEntryStopLoss = null;

        private IOrder ShortEntry = null;
        private double priorHighLongEntryPrice;
        private double profitTargetLongEntryPriceOffset;
        private double stoplossLongEntryPrice;
        private double stopLossPrice;
        
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = true;
            Unmanaged = true;
            Add("ES 06-16", PeriodType.Minute, 1);

        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {

            if (BarsInProgress == 0)
            {
                if (Bars.BarsSinceSession == 0)
                {
                    LongEntry = LongEntryProfitTarget = LongEntryStopLoss = null;
                }
            }

           /* if (Position.MarketPosition == MarketPosition.Flat && LongEntry !=null)
            {
                LongEntry = LongEntryProfitTarget = LongEntryStopLoss = null;
            }*/

            //Outside-Up Bar Case

            if (BarsInProgress == 0)
            {
                if (Closes[0][0] > Highs[0][1] && Lows[0][0] < Lows[0][1])
                {
                    BackColor = Color.Aqua;
                    if (LongEntry == null)
                    {
                        priorHighLongEntryPrice = Closes[0][0];
                        stopLossPrice = Lows[0][0];
                        stopLossPrice = priorHighLongEntryPrice-4;
                        stoplossLongEntryPrice = priorHighLongEntryPrice - Lows[0][0];
                        //profitTargetLongEntryPriceOffset = priorHighLongEntryPrice + stoplossLongEntryPrice * rewardToRisk;
                        profitTargetLongEntryPriceOffset = priorHighLongEntryPrice + 4.00;

                        if (Close[0] > EMA(20)[0])
                            LongEntry = SubmitOrder(0, OrderAction.Buy, OrderType.Market, quantity, 0, 0, "Outside-Up", "Outside-Up");
                    }
                }
            }

 
        }

        protected override void OnExecution(IExecution execution)
        {
            if (LongEntry != null && LongEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled ||
                execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                //profitTargetLongEntryPriceOffset = (profitTargetLongEntryPriceOffset * rewardToRisk < 2 ? 2 : profitTargetLongEntryPriceOffset * rewardToRisk);
                //Print(profitTargetLongEntryPriceOffset);
                LongEntryStopLoss = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Filled, 0, stopLossPrice, "Outside-Up1", "p.Outside-Up");
                LongEntryProfitTarget = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Filled, profitTargetLongEntryPriceOffset, 0, "Outside-Up1", "s.Outside-Up");
            }


            if (LongEntry != null && LongEntryProfitTarget != null && LongEntryStopLoss != null)
            {
                if (LongEntryStopLoss == execution.Order || LongEntryProfitTarget == execution.Order)
                    if ((execution.Order.OrderState == OrderState.Filled ||
                        execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
                        LongEntry = LongEntryProfitTarget = LongEntryStopLoss = null;
            }
        }
        #region Properties
        #endregion
    }
}
