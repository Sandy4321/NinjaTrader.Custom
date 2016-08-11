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
using System.Collections.Generic;
using MySql.Data.MySqlClient;

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// CL Strategy for Scalping
    /// </summary>
    [Description("Inverse Toby Crabel Swing Strategy")]
    public class CLTobyCrabel : Strategy
    {
        private int scalpQty = 1;
        private int swingQty = 1;
        private double stopLossDivisor = 0.5; //Adjust the stoploss yellow line which is derieved off the IB range
        private double lastOpenEntryDiffPx = 0.02; //Set original Scalp profit target to open before entry (only if open >=  lastOpenEntryDiffPx)
        private TimeSpan ib30Time = new TimeSpan(10, 00, 00);
        
        private int adxPeriod = 14;
        private double enableEntryBelowADX = 60.0;
        private double enableEntryAboveIBRange = 0.10;


        #region Required Variables
        private IOrder scalpLongEntry = null;     //Short Limit IOrders
        private IOrder scalpLongProfitTarget2 = null;
        private IOrder scalpLongStopLoss2 = null;
        private IOrder swingLongEntry = null;
        private IOrder swingLongProfitTarget2 = null;
        private IOrder swingLongStopLoss2 = null;

        private IOrder scalpShortEntry = null;      //Long Limit IOrders
        private IOrder scalpShortProfitTarget2 = null;
        private IOrder scalpShortStopLoss2 = null;
        private IOrder swingShortEntry = null;
        private IOrder swingShortProfitTarget2 = null;
        private IOrder swingShortStopLoss2 = null;

        private IOrder swingStopLossLong = null;
        private double lastEntryPrice = 0;
        private double lastExitPrice = 0;
        private double lastOpenBullPrice = 0;
        private double lastOpenBearPrice = 0;
        private bool convertScalpToSwing = false;

        private DateTime lineStartTime;
        private DateTime lineEndTime;
        private DateTime lastEntryTime;
        private DateTime lastExitTime;
        private int dayIndex = 0;
        private MySqlConnection dbConn;
        private CLInitialBalance ib30;
        private CLIBStatistics inReport = new CLIBStatistics();
        private Dictionary<int, CLInitialBalance> dIB30 = new Dictionary<int, CLInitialBalance>();
        private bool recordFirstItemOnly = true;

        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            TraceOrders = true;
            CalculateOnBarClose = true;
            EntriesPerDirection = 3;
            Enabled = true;
            Unmanaged = true;

        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        ///
        #region Connect and disconnect database and IO
        protected override void OnStartUp()
        {

            dbConn = new MySqlConnection("server=localhost;database=algo;uid=root;password=Password1;");
            try
            {
                dbConn.Open();
                Print("Database connection established successfully...");
            }
            catch (MySqlException ex)
            {
                Print(ex.ToString());
            }
        }
        protected override void OnTermination()
        {
            MySqlCommand delete = new MySqlCommand("delete from ibstats", dbConn);
            delete.ExecuteNonQuery();
            dbConn.Close();
        }
        #endregion
        protected override void OnBarUpdate()
        {
            if (BarsInProgress == 0)
            {
                double ibHigh = 0;
                double ibLow = 9999;
                double ibCummVolume = 0;
                lastOpenBullPrice = 0;
                lastOpenBearPrice = 0;

                if (Bars.BarsSinceSession == 0)
                {
                    recordFirstItemOnly = true;
                    scalpShortEntry = scalpLongEntry = null;
                }

                if (Position.MarketPosition == MarketPosition.Long)
                {

                }
                else if (Position.MarketPosition == MarketPosition.Short && BarsSinceEntry(0, "SW.BEAR.STP", 1) >= 10)
                {
                    if (swingShortStopLoss2 != null) ChangeOrder(swingShortStopLoss2, swingShortStopLoss2.Quantity, 0, Close[0]);
                }


                if (Time[0].TimeOfDay >= ib30Time && recordFirstItemOnly == true)
                {
                    lineStartTime = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, 9, 30, 00);
                    lineEndTime = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, 15, 59, 59);

                    ib30 = new CLInitialBalance()
                    {
                        IBStartTime = Time[Bars.BarsSinceSession],
                        IBEndTime = Time[0],
                        IBClose = Close[0],
                        IBOpen = Open[Bars.BarsSinceSession]
                    };

                    for (int i = 0; i <= Bars.BarsSinceSession; i++)
                    {
                        ibHigh = Math.Max(ibHigh, High[i]);
                        ibLow = Math.Min(ibLow, Low[i]);
                        ibCummVolume = ibCummVolume + Volume[i];
                    }

                    ib30.IBLow = ibLow;
                    ib30.IBHigh = ibHigh;
                    ib30.IBVolume = ibCummVolume;

                    dIB30.Add(dayIndex, ib30);
                    dayIndex++;

                    recordFirstItemOnly = false;

                    if (probability() == true)
                    {
                        if (scalpLongEntry == null)      // Long Entry Orders (OCO)
                        {
                            if (Close[0] >= ib30.IBHigh && scalpLongEntry == null)
                            {
                                scalpLongEntry = SubmitOrder(0, OrderAction.Buy, OrderType.Market, scalpQty, 0, 0, "IBEntry", "BULL.MKT");
                                swingLongEntry = SubmitOrder(0, OrderAction.Buy, OrderType.Market, swingQty, 0, 0, "IBEntrySwing", "SW.BULL.MKT");
                            }
                            if (Close[0] < ib30.IBHigh && scalpLongEntry == null)
                            {
                                scalpLongEntry = SubmitOrder(0, OrderAction.Buy, OrderType.Stop, scalpQty, 0,ib30.IBHigh, "IBEntry", "BEAR.STP");
                                swingLongEntry = SubmitOrder(0, OrderAction.Buy, OrderType.Stop, swingQty, 0,ib30.IBHigh, "IBEntrySwing", "SW.BEAR.STP");
                            }
                        }
                        if (scalpShortEntry == null)    // Short Entry Orders (OCO)
                        {
                            if (Close[0] <= ib30.IBLow && scalpShortEntry == null)
                            {
                                scalpShortEntry = SubmitOrder(0, OrderAction.SellShort, OrderType.Market, scalpQty, 0, 0, "IBEntry", "BEAR.MKT");
                                swingShortEntry = SubmitOrder(0, OrderAction.SellShort, OrderType.Market, swingQty, 0, 0, "IBEntrySwing", "SW.BEAR.MKT");
                            }
                            if (Close[0] > ib30.IBLow && scalpShortEntry == null)
                            {
                                scalpShortEntry = SubmitOrder(0, OrderAction.SellShort, OrderType.Stop, scalpQty, 0, ib30.IBLow, "IBEntry", "BEAR.STP");
                                swingShortEntry = SubmitOrder(0, OrderAction.SellShort, OrderType.Stop, swingQty, 0, ib30.IBLow, "IBEntrySwing", "SW.BEAR.STP");
                            }
                        }                 
                    }
                    #region Draw Objects
                    DrawLine("IBHigh" + Time[0].Date, false, lineStartTime, ib30.IBHigh, lineEndTime, ib30.IBHigh, Color.Aqua, DashStyle.Solid, 2);
                    DrawLine("IBLow" + Time[0].Date, false, lineStartTime, ib30.IBLow, lineEndTime, ib30.IBLow, Color.Coral, DashStyle.Solid, 2);
                    DrawLine("IBHighTarget" + Time[0].Date, false, lineStartTime, (ib30.IBHigh + Math.Abs(ib30.IBLow - ib30.IBHigh) / stopLossDivisor), lineEndTime, (ib30.IBHigh + Math.Abs(ib30.IBLow - ib30.IBHigh) / stopLossDivisor), Color.Yellow, DashStyle.Dash, 1);
                    DrawLine("IBLowTarget" + Time[0].Date, false, lineStartTime, ib30.IBLow - Math.Abs(ib30.IBLow - ib30.IBHigh) / stopLossDivisor, lineEndTime, ib30.IBLow - Math.Abs(ib30.IBLow - ib30.IBHigh) / stopLossDivisor, Color.Yellow, DashStyle.Dash, 1);

                    if (Close[0] <= (ib30.IBLow + (Math.Abs(ib30.IBHigh - ib30.IBLow)) / 2))
                        DrawText("IBRnage" + Time[0].Date, String.Format("ib.{0}   adx.{1}", (ib30.IBHigh - ib30.IBLow).ToString("0.00"), ADX(14)[0].ToString("0"),ib30.IBVolume.ToString("0")), 0, ib30.IBHigh + 2*TickSize, Color.Black);
                    else
                        DrawText("IBRnage" + Time[0].Date, String.Format("ib.{0}   adx.{1}", (ib30.IBHigh - ib30.IBLow).ToString("0.00"), ADX(14)[0].ToString("0"),ib30.IBVolume.ToString("0")), 0, ib30.IBLow - 2 * TickSize, Color.Black);
                    #endregion
                }
            }
        }
        protected override void OnOrderUpdate(IOrder order)
        {
        }
        protected override void OnExecution(IExecution execution)
        {
            if (execution.Order.OrderState == OrderState.Filled)
            {
                if (scalpLongEntry == execution.Order || scalpShortEntry == execution.Order ||
                    swingLongEntry == execution.Order || swingShortEntry == execution.Order)    // set entry price for PnL stats
                {
                    lastEntryPrice = execution.Order.AvgFillPrice;
                    lastEntryTime = Time[0];

                    if (convertScalpToSwing == false)
                    {
                        for (int i = 0; i < Bars.BarsSinceSession; i++)
                        {
                            lastOpenBearPrice = lastOpenBullPrice = Math.Abs(Open[i]-lastEntryPrice);
                            if (Math.Abs(lastOpenBullPrice - lastEntryPrice) >= lastOpenEntryDiffPx)
                                break;
                        }
                    }
                    else if (convertScalpToSwing == true)
                    {
                        lastOpenBullPrice = ib30.IBLow;
                        lastOpenBearPrice = ib30.IBHigh;
                    }
                }
                else if (scalpLongEntry != execution.Order || scalpShortEntry != execution.Order ||
                    swingLongEntry != execution.Order || swingShortEntry != execution.Order)   // set exit prices for PnL stats
                {
                    lastExitPrice = execution.Order.AvgFillPrice;
                    lastExitTime = Time[0];

                    inReport.IBDate = Time[0];
                    inReport.IBOpen = ib30.IBOpen;
                    inReport.IBHigh = ib30.IBHigh;
                    inReport.IBLow = ib30.IBLow;
                    inReport.IBClose = ib30.IBClose;
                    inReport.IBEntryTime = lastEntryTime;
                    inReport.IBExitTime = lastExitTime;
                    inReport.IBEndTime = ib30.IBEndTime;
                    inReport.IBEntryPrice = lastEntryPrice;
                    inReport.IBExitPrice = lastExitPrice;

                    if (inReport.IBDirection == CLIBStatistics.Direction.Up)
                        inReport.IBPnL = lastExitPrice - lastEntryPrice;
                    else if (inReport.IBDirection == CLIBStatistics.Direction.Down)
                        inReport.IBPnL = lastEntryPrice - lastExitPrice;

                    #region Insert PnL Report Stats
                    try
                    {
                        string insertStatement = String.Format("Insert into ibstats values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')",
                            inReport.IBDate,
                            inReport.IBOpen,
                            inReport.IBHigh,
                            inReport.IBLow,
                            inReport.IBClose,
                            inReport.IBEntryTime,
                            inReport.IBExitTime,
                            inReport.IBEndTime,
                            inReport.IBPnL,
                            inReport.IBEntryPrice,
                            inReport.IBExitPrice,
                            inReport.IBDirection
                               );

                        //Print(insertStatement);
                        MySqlCommand insertIBQuery30 = new MySqlCommand(insertStatement, dbConn);
                        insertIBQuery30.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Print(ex.ToString());
                    }
                    #endregion
                }
            }
            if (scalpLongEntry != null && scalpLongEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled ||
                execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                scalpLongStopLoss2 = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Filled,0,ib30.IBLow, "LongExit", "SC.BULL.SL");
                scalpLongProfitTarget2 = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Filled, execution.Order.AvgFillPrice + lastOpenBullPrice, 0, "LongExit", "SC.BULL.PT");
                inReport.IBDirection = CLIBStatistics.Direction.Down;
            }
            if (swingLongEntry != null && swingLongEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled ||
                execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                swingLongStopLoss2 = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Filled, 0, ib30.IBLow, "SwingLongExit", "SW.BULL.SL");
                swingLongProfitTarget2 = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Filled, (ib30.IBHigh + Math.Abs(ib30.IBLow - ib30.IBHigh) / stopLossDivisor), 0, "SwingLongExit", "SW.BULL.PT");
            }
            if (scalpShortEntry != null && scalpShortEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled ||
                execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                scalpShortStopLoss2 = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, execution.Order.Filled, 0, ib30.IBHigh, "ShortExit", "SC.BEAR.SL");
                scalpShortProfitTarget2 = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, execution.Order.Filled, execution.Order.AvgFillPrice-lastOpenBearPrice, 0, "ShortExit", "SC.BEARR.PT");
                inReport.IBDirection = CLIBStatistics.Direction.Up;
            }
            if (swingShortEntry != null && swingShortEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled ||
                execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                swingShortStopLoss2 = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, execution.Order.Filled, 0,ib30.IBHigh, "SwingShortExit", "SW.BEAR.SL");
                swingShortProfitTarget2 = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, execution.Order.Filled, (ib30.IBLow - Math.Abs(ib30.IBLow - ib30.IBHigh) / stopLossDivisor), 0, "SwingShortExit", "SW.BEAR.PT");
            }
            if (scalpLongProfitTarget2 == execution.Order && scalpLongProfitTarget2.OrderState == OrderState.Filled)
            {
                Print("ScalpLongProfitTarget LOOP A");
                Print(ib30.IBHigh - lastOpenBullPrice);
                ChangeOrder(swingLongStopLoss2, swingLongStopLoss2.Quantity, 0, ib30.IBHigh);
            }
            if (scalpShortProfitTarget2 == execution.Order && scalpShortProfitTarget2.OrderState == OrderState.Filled)
            {
                Print("Loop B");
                Print(ib30.IBLow + lastOpenBearPrice);
                ChangeOrder(swingShortStopLoss2, swingShortStopLoss2.Quantity, 0, ib30.IBLow);
            }


        }
        private bool probability()
        {
                if (Math.Abs(ib30.IBHigh - ib30.IBLow) > enableEntryAboveIBRange)
                {
                    if (ADX(adxPeriod)[0] < enableEntryBelowADX)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
        }
    }
    public class CLInitialBalance
    {
        public DateTime IBStartTime { get; set; }
        public DateTime IBEndTime { get; set; }
        public double IBHigh { get; set; }
        public double IBLow { get; set; }
        public double IBOpen { get; set; }
        public double IBClose { get; set; }
        public double IBVolume { get; set; }
    }
    public class CLIBStatistics
    {
        public DateTime IBDate { get; set; }
        public double IBOpen { get; set; }
        public double IBClose { get; set; }
        public double IBHigh { get; set; }
        public double IBLow { get; set; }
        public DateTime IBEntryTime { get; set; }
        public DateTime IBExitTime { get; set; }
        public DateTime IBEndTime { get; set; }
        public double IBPnL { get; set; }
        public double IBEntryPrice { get; set; }
        public double IBExitPrice { get; set; }
        public Direction IBDirection { get; set; }

        public enum Direction
        {
            Up,
            Down,
        }
    }
}

