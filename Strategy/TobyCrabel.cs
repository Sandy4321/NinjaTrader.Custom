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
    /// The strategy identifies the IB range for the price action and bets that any breakout of the IB range will fail. (Inverse logic of Toby Crabel)
    /// In the event that the breakout succeeds, which is less frequent than trading range days. Exit the position at IB range derived stop loss price
    /// In the event that a swing stop loss is hit, adjust the scalp portion, (which usually targets the open price, of the bar prior to the entry) to
    /// target the IB range profit target on the next entry. If the target is achieved the scalp portion goes back to normal. (An exit on close, where
    /// the profit target is not reaches, will not reset the orginal scalp target.
    ///
    /// Entry probability is set to enter only if todays IB High/Low price is contained in the prior IB range and the ADX is within set tolerance.
    /// If current IB range completely engulfs the prior IB range, do no enter limit orders. Also, if the IB range is very small, the day potentially
    /// can be a breakout day and therefore the strategy is disabled.
    /// </summary>
    [Description("Inverse Toby Crabel Swing Strategy")]
    public class TobyCrabel : Strategy
    {
        private int scalpQty = 1;
        private int swingQty = 1;
        private double stopLossDivisor = 2; //Adjust the stoploss yellow line which is derieved off the IB range
        private double lastOpenEntryDiffPx = 0.25; //Set original Scalp profit target to open before entry (only if open >=  lastOpenEntryDiffPx)
        private TimeSpan ib30Time = new TimeSpan(10, 00, 00);

        private int adxPeriod = 14;
        private double enableEntryBelowADX = 30.0;
        private double enableEntryAboveIBRange = 3.00;
        private int ibBarSinceSession = 0;

        //LBR310
        #region Required Variables
        private IOrder scalpShortLimitEntry = null;     //Short Limit IOrders
        private IOrder scalpShortProfitTarget = null;
        private IOrder scalpShortStopLoss = null;
        private IOrder swingShortLimitEntry = null;
        private IOrder swingShortProfitTarget = null;
        private IOrder swingShortStopLoss = null;

        private IOrder swingShortEntry = null;
        private IOrder swingLongEntry = null;

        private IOrder scalpLongLimitEntry = null;      //Long Limit IOrders
        private IOrder scalpLongProfitTarget = null;
        private IOrder scalpLongStopLoss = null;
        private IOrder swingLongLimitEntry = null;
        private IOrder swingLongProfitTarget = null;
        private IOrder swingLongStopLoss = null;
        private bool IsFirstEntryLong = false;

        private IOrder swingStopLossLong = null;
        private double lastEntryScalpPrice = 0;
        private double lastEntrySwingPrice = 0;
        private double lastExitPrice = 0;
        private double lastOpenBullPrice = 0;
        private double lastOpenBearPrice = 0;
        private bool convertScalpToSwing = false;

        private DateTime lineStartTime;
        private DateTime lineEndTime;
        private DateTime lastEntryScalpTime;
        private string lastEntryScalpName;
        private string lastEntrySwingName;
        private DateTime lastEntrySwingTime;
        private DateTime lastExitTime;
        private int dayIndex = 0;
        private MySqlConnection dbConn;
        private InitialBalance ib30;
        private IBStatistics inReport = new IBStatistics();
        private Dictionary<int, InitialBalance> dIB30 = new Dictionary<int, InitialBalance>();
        private bool recordFirstItemOnly = true;
        private bool enableTrendStrategy = true;
        private bool enableRangeStrategy = true;

        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            //TraceOrders = true;
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
            //MySqlCommand delete = new MySqlCommand("delete from ibstats", dbConn);
            //delete.ExecuteNonQuery();
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
                    scalpLongLimitEntry = scalpShortLimitEntry = swingLongLimitEntry = swingShortLimitEntry = swingShortEntry = swingLongEntry = null;
                }

                //if (Position.MarketPosition != MarketPosition.Flat)
                //{
                //    if (IsFirstEntryLong == false && Close[0] >= (ib30.IBHigh - (ib30.IBHigh - ib30.IBLow) / 2) && swingShortLimitEntry != null && scalpShortLimitEntry == null)
                //        scalpShortLimitEntry = SubmitOrder(0, OrderAction.SellShort, OrderType.Stop, scalpQty, 0, (ib30.IBHigh - (ib30.IBHigh - ib30.IBLow) / 2), "IBEntry", "SC.BEAR.STP");
                //    if (IsFirstEntryLong == true && (Close[0] <= ib30.IBLow + (ib30.IBHigh - ib30.IBLow) / 2) && swingLongLimitEntry != null && scalpLongLimitEntry == null)
                //        scalpLongLimitEntry = SubmitOrder(0, OrderAction.Buy, OrderType.Stop, scalpQty, 0, (ib30.IBLow + (ib30.IBHigh - ib30.IBLow) / 2), "IBEntry", "SC.BULL.STP");
                //}

                if (Time[0].TimeOfDay >= ib30Time && recordFirstItemOnly == true)
                {
                    ibBarSinceSession = Bars.BarsSinceSession;
                    lineStartTime = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, 9, 30, 00);
                    lineEndTime = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, 15, 59, 59);

                    ib30 = new InitialBalance()
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

                    if (probabilityRange() == true)             // Limit Orders for Trading Range Days
                    {
                        if (enableRangeStrategy)      // Long Entry Orders (OCO)
                        {
                            if (Close[0] >= ib30.IBHigh && swingShortLimitEntry == null)
                                swingShortLimitEntry = SubmitOrder(0, OrderAction.SellShort, OrderType.Market, swingQty, 0, 0, "IBEntrySwing", "SW.BEAR.MKT");
                            if (Close[0] < ib30.IBHigh && swingShortLimitEntry == null)
                                swingShortLimitEntry = SubmitOrder(0, OrderAction.SellShort, OrderType.Limit, swingQty, ib30.IBHigh, 0, "IBEntrySwing", "SW.BEAR.LMT");
                        }
                        if (enableRangeStrategy)    // Short Entry Orders (OCO)
                        {
                            if (Close[0] <= ib30.IBLow && swingLongLimitEntry == null)
                                swingLongLimitEntry = SubmitOrder(0, OrderAction.Buy, OrderType.Market, swingQty, 0, 0, "IBEntrySwing", "SW.BULL.MKT");
                            if (Close[0] > ib30.IBLow && swingLongLimitEntry == null)
                                swingLongLimitEntry = SubmitOrder(0, OrderAction.Buy, OrderType.Limit, swingQty, ib30.IBLow, 0, "IBEntrySwing", "SW.BULL.LMT");

                        }
                    }

                    //if (probabilityTrend() == true)             // Stop Orders for Trending Range Days
                    //{
                    //    if (enableTrendStrategy)      // Long Entry Orders (OCO)
                    //    {
                    //        if (Close[0] >= ib30.IBHigh && swingLongEntry == null)
                    //            swingLongEntry = SubmitOrder(0, OrderAction.Buy, OrderType.Market, swingQty, 0, 0, "IBEntrySwing", "SW.BULL.MKT");
                    //        if (Close[0] < ib30.IBHigh && swingLongEntry == null)
                    //            swingLongEntry = SubmitOrder(0, OrderAction.Buy, OrderType.Stop, swingQty, 0,ib30.IBHigh, "IBEntrySwing", "SW.BULL.STP");
                    //    }
                    //    if (enableTrendStrategy)    // Short Entry Orders (OCO)
                    //    {
                    //        if (Close[0] <= ib30.IBLow && swingShortEntry == null)
                    //            swingShortEntry = SubmitOrder(0, OrderAction.SellShort, OrderType.Market, swingQty, 0, 0, "IBEntrySwing", "SW.BEAR.MKT");
                    //        if (Close[0] > ib30.IBLow && swingShortEntry == null)
                    //            swingShortEntry = SubmitOrder(0, OrderAction.SellShort, OrderType.Stop, swingQty, 0,ib30.IBLow, "IBEntrySwing", "SW.BEAR.STP");
                    //    }
                    //}

                    #region Draw Objects
                    DrawLine("IBHigh" + Time[0].Date, false, lineStartTime, ib30.IBHigh, lineEndTime, ib30.IBHigh, Color.Aqua, DashStyle.Solid, 2);
                    DrawLine("IBLow" + Time[0].Date, false, lineStartTime, ib30.IBLow, lineEndTime, ib30.IBLow, Color.Coral, DashStyle.Solid, 2);
                    DrawLine("IBHighTarget" + Time[0].Date, false, lineStartTime, (ib30.IBHigh + Math.Abs(ib30.IBLow - ib30.IBHigh) / stopLossDivisor), lineEndTime, (ib30.IBHigh + Math.Abs(ib30.IBLow - ib30.IBHigh) / stopLossDivisor), Color.Yellow, DashStyle.Dash, 1);
                    DrawLine("IBLowTarget" + Time[0].Date, false, lineStartTime, ib30.IBLow - Math.Abs(ib30.IBLow - ib30.IBHigh) / stopLossDivisor, lineEndTime, ib30.IBLow - Math.Abs(ib30.IBLow - ib30.IBHigh) / stopLossDivisor, Color.Yellow, DashStyle.Dash, 1);

                    if (Close[0] <= (ib30.IBLow + (Math.Abs(ib30.IBHigh - ib30.IBLow)) / 2))
                        DrawText("IBRnage" + Time[0].Date, String.Format("ib.{0}   adx.{1}", (ib30.IBHigh - ib30.IBLow).ToString("0.00"), ADX(14)[0].ToString("0"), ib30.IBVolume.ToString("0")), 0, ib30.IBHigh + 2 * TickSize, Color.Black);
                    else
                        DrawText("IBRnage" + Time[0].Date, String.Format("ib.{0}   adx.{1}", (ib30.IBHigh - ib30.IBLow).ToString("0.00"), ADX(14)[0].ToString("0"), ib30.IBVolume.ToString("0")), 0, ib30.IBLow - 2 * TickSize, Color.Black);
                    #endregion

                    if (swingLongLimitEntry != null && swingLongLimitEntry.Filled > 0)
                    {
                        double ibSMA = SMA(ibBarSinceSession)[0];
                        DrawText("IBRnage" + Time[0].Date, String.Format("adx.{0}   sma.{1}", ADX(14)[0].ToString("0"), ibSMA.ToString("0.00")), 0, ib30.IBHigh + 2 * TickSize, Color.Black);
                    }
                    else if (swingShortLimitEntry != null && swingShortLimitEntry.Filled > 0)
                    {
                        double ibSMA = SMA(ibBarSinceSession)[0];
                        DrawText("IBRnage" + Time[0].Date, String.Format("adx.{0}   sma.{1}", ADX(14)[0].ToString("0"), ibSMA.ToString("0.00")), 0, ib30.IBLow - 2 * TickSize, Color.Black);
                    }
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
                if (scalpShortLimitEntry == execution.Order || scalpLongLimitEntry == execution.Order ||
                    swingShortLimitEntry == execution.Order || swingLongLimitEntry == execution.Order)    // set entry price for PnL stats
                {
                    if (scalpShortLimitEntry == execution.Order || scalpLongLimitEntry == execution.Order)
                    {
                        lastEntryScalpPrice = execution.Order.AvgFillPrice;
                        lastEntryScalpTime = Time[0];
                        lastEntryScalpName = execution.Order.Name;
                    }

                    if (swingShortLimitEntry == execution.Order || swingLongLimitEntry == execution.Order)
                    {
                        lastEntrySwingPrice = execution.Order.AvgFillPrice;
                        lastEntrySwingTime = Time[0];
                        lastEntrySwingName = execution.Order.Name;
                    }

                    //if (convertScalpToSwing == false)
                    //{
                    //    for (int i = 0; i < Bars.BarsSinceSession; i++)
                    //    {
                    //        lastOpenBearPrice = lastOpenBullPrice = Open[i];
                    //        if (Math.Abs(lastOpenBullPrice - lastEntryScalpPrice) >= lastOpenEntryDiffPx)
                    //            break;
                    //    }
                    //}
                    //else if (convertScalpToSwing == true)
                    //{
                    //    lastOpenBullPrice = ib30.IBLow;
                    //    lastOpenBearPrice = ib30.IBHigh;
                    //}
                }
                else if (scalpShortLimitEntry != execution.Order || scalpLongLimitEntry != execution.Order ||
                    swingShortLimitEntry != execution.Order || swingLongLimitEntry != execution.Order)   // set exit prices for PnL stats
                {
                    lastExitPrice = execution.Order.AvgFillPrice;
                    lastExitTime = Time[0];

                    inReport.IBInstrument = Instrument.MasterInstrument.Name;
                    inReport.IBDate = Time[0];
                    inReport.IBOpen = ib30.IBOpen;
                    inReport.IBHigh = ib30.IBHigh;
                    inReport.IBLow = ib30.IBLow;
                    inReport.IBClose = ib30.IBClose;

                    inReport.IBExitTime = lastExitTime;
                    inReport.IBEndTime = ib30.IBEndTime;

                    inReport.IBExitPrice = lastExitPrice;
                    inReport.IBComm = Instrument.MasterInstrument.GetCommission(1, Provider.InteractiveBrokers) * execution.Order.Filled;
                    inReport.IBPointValue = Instrument.MasterInstrument.PointValue;
                    inReport.IBQuantity = execution.Order.Filled;
                    inReport.IBExitName = execution.Order.Name;



                    if (swingLongProfitTarget == execution.Order || swingLongStopLoss == execution.Order ||
                        swingShortProfitTarget == execution.Order || swingShortStopLoss == execution.Order)
                    {
                        if (inReport.IBDirection == IBStatistics.Direction.Long)
                            inReport.IBPnL = lastExitPrice - lastEntrySwingPrice;
                        else if (inReport.IBDirection == IBStatistics.Direction.Short)
                            inReport.IBPnL = lastEntrySwingPrice - lastExitPrice;
                        inReport.IBEntryTime = lastEntrySwingTime;
                        inReport.IBEntryPrice = lastEntrySwingPrice;
                        inReport.IBEntryName = lastEntrySwingName;
                    }
                    if (scalpLongProfitTarget == execution.Order || scalpLongStopLoss == execution.Order ||
                        scalpShortProfitTarget == execution.Order || scalpShortStopLoss == execution.Order)
                    {
                        if (inReport.IBDirection == IBStatistics.Direction.Long)
                            inReport.IBPnL = lastExitPrice - lastEntryScalpPrice;
                        else if (inReport.IBDirection == IBStatistics.Direction.Short)
                            inReport.IBPnL = lastEntryScalpPrice - lastExitPrice;
                        inReport.IBEntryTime = lastEntryScalpTime;
                        inReport.IBEntryPrice = lastEntryScalpPrice;
                        inReport.IBEntryName = lastEntryScalpName;
                    }

                    if (execution.Order.Name == "Exit on close")
                    {
                        if (execution.Order.Filled > 1)
                        {
                            if (inReport.IBDirection == IBStatistics.Direction.Long)
                                inReport.IBPnL = ((lastExitPrice - lastEntrySwingPrice) + (lastExitPrice - lastEntryScalpPrice)) / execution.Order.Filled;
                            else if (inReport.IBDirection == IBStatistics.Direction.Short)
                                inReport.IBPnL = ((lastEntrySwingPrice - lastExitPrice) + (lastEntryScalpPrice - lastExitPrice)) / execution.Order.Filled;
                            inReport.IBEntryPrice = (lastEntryScalpPrice + lastEntrySwingPrice) / execution.Order.Filled;
                        }

                    }

                    #region Insert PnL Report Stats

                    try
                    {
                        string insertStatement = String.Format("Insert into ibstats values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}')",
                            inReport.IBInstrument,
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
                            inReport.IBQuantity,
                            inReport.IBDirection,
                            inReport.IBComm,
                            inReport.IBPointValue,
                            inReport.IBExitName
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
            if (scalpShortLimitEntry != null && scalpShortLimitEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled ||
                execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                scalpShortStopLoss = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, execution.Order.Filled, 0, (ib30.IBHigh + Math.Abs(ib30.IBLow - ib30.IBHigh) / 2), "LongExit", "SC.BEAR.SL");
                scalpShortProfitTarget = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, execution.Order.Filled, ib30.IBLow, 0, "LongExit", "SC.BEAR.PT");
                inReport.IBDirection = IBStatistics.Direction.Short;
            }
            if (swingShortLimitEntry != null && swingShortLimitEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled ||
                execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                IsFirstEntryLong = false;
                swingShortStopLoss = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, execution.Order.Filled, 0, (ib30.IBHigh + Math.Abs(ib30.IBLow - ib30.IBHigh) / 2), "SwingLongExit", "SW.BEAR.SL");
                swingShortProfitTarget = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, execution.Order.Filled, ib30.IBLow, 0, "SwingLongExit", "SW.BEAR.PT");
                inReport.IBDirection = IBStatistics.Direction.Short;
            }
            if (scalpLongLimitEntry != null && scalpLongLimitEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled ||
                execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                scalpLongStopLoss = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Filled, 0, (ib30.IBLow - Math.Abs(ib30.IBLow - ib30.IBHigh) / 2), "ShortExit", "SC.BULL.SL");
                scalpLongProfitTarget = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Filled, ib30.IBHigh, 0, "ShortExit", "SC.BULL.PT");
                inReport.IBDirection = IBStatistics.Direction.Long;
            }
            if (swingLongLimitEntry != null && swingLongLimitEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled ||
                execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                IsFirstEntryLong = true;
                swingLongStopLoss = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Filled, 0, (ib30.IBLow - Math.Abs(ib30.IBLow - ib30.IBHigh) / 2), "SwingShortExit", "SW.BULL.SL");
                swingLongProfitTarget = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Filled, ib30.IBHigh, 0, "SwingShortExit", "SW.BULL.PT");
                inReport.IBDirection = IBStatistics.Direction.Long;
            }

            //This code needs to be changed. LONG
            if (swingLongEntry != null && swingLongEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled ||
                execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                IsFirstEntryLong = true;
                swingLongStopLoss = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Filled, 0, (ib30.IBLow), "SwingLongExit", "SW.BULL.SL");
                swingLongProfitTarget = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Filled, (ib30.IBHigh + Math.Abs(ib30.IBLow - ib30.IBHigh) * 2), 0, "SwingLongExit", "SW.BULL.PT");
                inReport.IBDirection = IBStatistics.Direction.Long;
            }
            if (swingShortEntry != null && swingShortEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled ||
                execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                IsFirstEntryLong = false;
                swingShortStopLoss = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, execution.Order.Filled, 0, (ib30.IBHigh), "SwingShortExit", "SW.BEAR.SL");
                swingShortProfitTarget = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, execution.Order.Filled, (ib30.IBLow - Math.Abs(ib30.IBLow - ib30.IBHigh) * 2), 0, "SwingShortExit", "SW.BEAR.PT");
                inReport.IBDirection = IBStatistics.Direction.Short;
            }

            //if ((swingLongStopLoss == execution.Order && execution.Order.OrderState == OrderState.Filled) || (swingShortStopLoss == execution.Order && execution.Order.OrderState == OrderState.Filled))
            //    convertScalpToSwing = false; //In the event that swing position resulted in a loss, next few days have a higher probability of trading range behaviour
            //if ((swingLongProfitTarget == execution.Order && execution.Order.OrderState == OrderState.Filled) || (swingShortProfitTarget == execution.Order && execution.Order.OrderState == OrderState.Filled))
            //    convertScalpToSwing = false;
        }
        private bool probabilityRange()
        {

            if (dIB30.Count > 1 &&      // Only enable the strategy if today's IB Range is within yesterdays IB Range
                ((ib30.IBHigh < dIB30[dIB30.Count - 2].IBHigh && ib30.IBHigh > dIB30[dIB30.Count - 2].IBLow) ||
                ib30.IBLow < dIB30[dIB30.Count - 2].IBHigh && ib30.IBLow > dIB30[dIB30.Count - 2].IBLow))
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
            else
                return false;
        }
        private bool probabilityTrend()
        {

            if (dIB30.Count > 1 &&      // Only enable the strategy if today's IB Range is within yesterdays IB Range
                ((ib30.IBHigh < dIB30[dIB30.Count - 2].IBLow ||
                ib30.IBLow > dIB30[dIB30.Count - 2].IBHigh)))
            {
                return false;
                //    if (Math.Abs(ib30.IBHigh - ib30.IBLow) > enableEntryAboveIBRange)
                //    {
                //        if (ADX(adxPeriod)[0] > enableEntryBelowADX)
                //            return true;
                //        else
                //            return false;
                //    }
                //    else
                //        return false;
                //}
                //else
                //    return false;
            }
            return false;
        }


        [Description("Scalp Quantity")]
        [GridCategory("Orders")]
        public int ScalpQuantity
        {
            get { return scalpQty; }
            set { scalpQty = Math.Max(0, value); }
        }
        [Description("Swing Quantity")]
        [GridCategory("Orders")]
        public int SwingQuantity
        {
            get { return swingQty; }
            set { swingQty = Math.Max(1, value); }
        }
        [Description("Stoploss Divisor Offset")]
        [GridCategory("Orders")]
        public double StoplossDivisor
        {
            get { return stopLossDivisor; }
            set { stopLossDivisor = value; }
        }
        [Description("Enable Trend Strategy")]
        [GridCategory("Orders")]
        public bool EnableTrendStategy
        {
            get { return enableTrendStrategy; }
            set { enableTrendStrategy = value; }
        }
        [Description("Enable Range Strategy")]
        [GridCategory("Orders")]
        public bool EnableRangeStrategy
        {
            get { return enableRangeStrategy; }
            set { enableRangeStrategy = value; }
        }
        [Description("ADX period to only trade during trading range")]
        [GridCategory("EntrySettings")]
        public int ADXPeriod
        {
            get { return adxPeriod; }
            set { adxPeriod = value; }
        }
        [Description("ADX limiting value")]
        [GridCategory("EntrySettings")]
        public double ADXLimitValue
        {
            get { return enableEntryBelowADX; }
            set { enableEntryBelowADX = value; }
        }
        [Description("Minimum IB Range for Entry")]
        [GridCategory("EntrySettings")]
        public double RequiredIBDiff
        {
            get { return enableEntryAboveIBRange; }
            set { enableEntryAboveIBRange = value; }
        }
        [Description("Minimum scalp profit target")]
        [GridCategory("EntrySettings")]
        public double MinimumScalpTarget
        {
            get { return lastOpenEntryDiffPx; }
            set { lastOpenEntryDiffPx = value; }
        }

    }
    public class InitialBalance
    {
        public DateTime IBStartTime { get; set; }
        public DateTime IBEndTime { get; set; }
        public double IBHigh { get; set; }
        public double IBLow { get; set; }
        public double IBOpen { get; set; }
        public double IBClose { get; set; }
        public double IBVolume { get; set; }
		public double IBVolumeUp { get; set; }
		public double IBVolumeDown { get; set; }
        public DateTime IBDate { get; set; }
    }
    public class IBStatistics
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
        public string IBInstrument { get; set; }
        public int IBQuantity { get; set; }
        public string IBEntryName { get; set; }
        public string IBExitName { get; set; }
        public double IBComm { get; set; }
        public double IBPointValue { get; set; }
        public enum Direction
        {
            Long,
            Short,
        }
    }
}




