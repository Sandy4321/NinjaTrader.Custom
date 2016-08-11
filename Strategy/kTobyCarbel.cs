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
using System.Collections.Generic;
using MySql.Data.MySqlClient;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Enter the description of your strategy here
    /// </summary>
    [Description( "Finanalized Inverse Toby Carbel strategy" )]
    public class kTobyCrabel : Strategy
    {
        private int scalpQty = 1;
        private int swingQty = 1;
        private double stopLossDivisor = 2; //Adjust the stoploss yellow line which is derieved off the IB range
        private TimeSpan ib30Time = new TimeSpan(10, 00, 00);

        private int adxPeriod = 14;
        private double enableEntryBelowADX = 30.0;
        private double enableEntryAboveIBRange = 3.00;

        private MySqlConnection dbConn;
        private int dayIndex = 0;
        private InitialBalance ib30;
        private Dictionary <int, InitialBalance> dIB30 = new Dictionary< int, InitialBalance>();
        private Dictionary<int, Trade> dIB30Trades = new Dictionary<int, Trade>();
        double ibCummVolUp = 0.0;
        double ibCummVolDown = 0.0;

        #region IOrder Variable for Scalp and Swing Orders
        private bool enableScalpTrade = false;

        private IOrder scalpShortLimitEntry = null;
        private IOrder scalpShortProfitTarget = null;
        private IOrder scalpShortStopLoss = null;
        private IOrder swingShortLimitEntry = null;
        private IOrder swingShortProfitTarget = null;
        private IOrder swingShortStopLoss = null;

        private IOrder scalpLongLimitEntry = null;     
        private IOrder scalpLongProfitTarget = null;
        private IOrder scalpLongStopLoss = null;
        private IOrder swingLongLimitEntry = null;
        private IOrder swingLongProfitTarget = null;
        private IOrder swingLongStopLoss = null;
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
        protected override void OnStartUp()
        {
            dbConn = new MySqlConnection("server=localhost;database=algo;uid=root;password=Password1;" );
            try
            {
                dbConn.Open();
                Print( "Database connection established successfully..." );
            }
            catch (MySqlException ex)
            {
                Print(ex.ToString());
            }
        }
        protected override void OnTermination()
        {
            Print( "Historical trade count: " + Performance.AllTrades.Count);

            for (int idx = 1; idx <= Performance.AllTrades.Count; idx++)
            {
                Trade trade = Performance.AllTrades[Performance.AllTrades.Count - idx];
                dIB30Trades.Add(idx, trade);
                Print(trade.ToString());
            }
            MySqlCommand delete = new MySqlCommand( "delete from ibstats", dbConn);
            delete.ExecuteNonQuery();
            dbConn.Close();
        }

        protected override void OnBarUpdate()
        {
            if (Bars.BarsSinceSession == 0)
            {
                ibCummVolUp = ibCummVolDown = 0;
                scalpLongLimitEntry = scalpShortLimitEntry = swingLongLimitEntry = swingShortLimitEntry = swingLongProfitTarget = swingLongStopLoss =
                    scalpLongProfitTarget = scalpLongStopLoss = swingShortProfitTarget = swingShortStopLoss = scalpShortProfitTarget = scalpShortStopLoss = null;
            }

            if (Time[0].TimeOfDay >= ib30Time && Time[1].TimeOfDay < ib30Time)
            {
                ib30 = new InitialBalance ()
                {
                    IBStartTime = Time[Bars.BarsSinceSession],
                    IBEndTime = Time[0],
                    IBOpen = Open[Bars.BarsSinceSession],
                    IBHigh = MAX(High, Bars.BarsSinceSession)[0],
                    IBLow = MIN(Low, Bars.BarsSinceSession)[0],
                    IBClose = Close[0]
                };

                for (int i = 0; i <= Bars.BarsSinceSession; i++)
                {
                    if (Close[i] >= Open[i])
                        ibCummVolUp = ibCummVolUp + Volume[i];
                    else
                        ibCummVolDown = ibCummVolDown + Volume[i];
                }

                ib30.IBVolumeDown = ibCummVolDown;
                ib30.IBVolumeUp = ibCummVolUp;

                dIB30.Add(dayIndex, ib30);
                dayIndex++;

                if (probability() == true )
                {
                    enableScalpTrade = true;

                    if (true )      // Long Entry Orders (OCO)
                    {
                        if (Close[0] >= ib30.IBHigh && swingShortLimitEntry == null)
                            swingShortLimitEntry = SubmitOrder(0, OrderAction.SellShort, OrderType .Market, swingQty, 0, 0, "IBEntrySwing", "SW.BEAR.MKT");
                        if (Close[0] < ib30.IBHigh && swingShortLimitEntry == null)
                            swingShortLimitEntry = SubmitOrder(0, OrderAction.SellShort, OrderType .Limit, swingQty, ib30.IBHigh, 0, "IBEntrySwing", "SW.BEAR.LMT");
                    }
                    if (true )    // Short Entry Orders (OCO)
                    {
                        if (Close[0] <= ib30.IBLow && swingLongLimitEntry == null)
                            swingLongLimitEntry = SubmitOrder(0, OrderAction.Buy, OrderType.Market, swingQty, 0, 0, "IBEntrySwing" , "SW.BULL.MKT");
                        if (Close[0] > ib30.IBLow && swingLongLimitEntry == null)
                            swingLongLimitEntry = SubmitOrder(0, OrderAction.Buy, OrderType.Limit, swingQty, ib30.IBLow, 0, "IBEntrySwing" , "SW.BULL.LMT");
                    }
                }
                #region Draw Objects
                DateTime lineStartTime = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, 9, 30, 00);
                DateTime lineEndTime = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, 15, 59, 59);

                DrawLine( "IBHigh" + Time[0].Date, false , lineStartTime, ib30.IBHigh, lineEndTime, ib30.IBHigh, Color .Aqua, DashStyle.Solid, 2);
                DrawLine( "IBLow" + Time[0].Date, false , lineStartTime, ib30.IBLow, lineEndTime, ib30.IBLow, Color .Coral, DashStyle.Solid, 2);
                DrawLine( "IBHighTarget" + Time[0].Date, false , lineStartTime, (ib30.IBHigh + Math.Abs(ib30.IBLow - ib30.IBHigh) / stopLossDivisor), lineEndTime, (ib30.IBHigh + Math.Abs(ib30.IBLow - ib30.IBHigh) / stopLossDivisor), Color.Yellow, DashStyle .Dash, 1);
                DrawLine( "IBLowTarget" + Time[0].Date, false , lineStartTime, ib30.IBLow - Math.Abs(ib30.IBLow - ib30.IBHigh) / stopLossDivisor, lineEndTime, ib30.IBLow - Math.Abs(ib30.IBLow - ib30.IBHigh) / stopLossDivisor, Color.Yellow, DashStyle.Dash, 1);

                if (Close[0] <= (ib30.IBLow + (Math .Abs(ib30.IBHigh - ib30.IBLow)) / 2))
                    DrawText( "IBRnage" + Time[0].Date, String .Format("ib.{0}   adx.{1}", (ib30.IBHigh - ib30.IBLow).ToString( "0.00"), ADX(14)[0].ToString("0" ), ib30.IBVolume.ToString("0" )), 0, ib30.IBHigh + 2 * TickSize, Color.Black);
                else
                    DrawText( "IBRnage" + Time[0].Date, String .Format("ib.{0}   adx.{1}", (ib30.IBHigh - ib30.IBLow).ToString( "0.00"), ADX(14)[0].ToString("0" ), ib30.IBVolume.ToString("0" )), 0, ib30.IBLow - 2 * TickSize, Color.Black);
                #endregion
            }

            if (enableScalpTrade == true )
            {
                if (High[0] >= ib30.IBHigh)
                {
                    scalpShortLimitEntry = SubmitOrder(0, OrderAction.SellShort, OrderType.Stop, scalpQty, 0, (ib30.IBHigh - (ib30.IBHigh - ib30.IBLow) / 2), "IBEntry", "SC.BEAR.STP" );
                    enableScalpTrade = false;
                }
                if (Low[0] <= ib30.IBLow)
                {
                    scalpLongLimitEntry = SubmitOrder(0, OrderAction.Buy, OrderType.Stop, scalpQty, 0, (ib30.IBLow + (ib30.IBHigh - ib30.IBLow) / 2), "IBEntry", "SC.BULL.STP" );
                    enableScalpTrade = false;
                }
            }
        }
        protected override void OnExecution( IExecution execution)
        {
           if (scalpShortLimitEntry != null && scalpShortLimitEntry == execution.Order && (execution.Order.OrderState == OrderState .Filled ||
                execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState .Cancelled && OrderState.Filled > 0)))
            {
                scalpShortStopLoss = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, execution.Order.Filled, 0, (ib30.IBHigh + Math.Abs(ib30.IBLow - ib30.IBHigh) / 2), "LongExit" , "SC.BEAR.SL");
                scalpShortProfitTarget = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, execution.Order.Filled, ib30.IBLow, 0, "LongExit", "SC.BEAR.PT");
            }
            if (swingShortLimitEntry != null && swingShortLimitEntry == execution.Order && (execution.Order.OrderState == OrderState .Filled ||
                execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState .Cancelled && OrderState.Filled > 0)))
            {
                swingShortStopLoss = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, execution.Order.Filled, 0, (ib30.IBHigh + Math.Abs(ib30.IBLow - ib30.IBHigh) / 2), "SwingLongExit" , "SW.BEAR.SL");
                swingShortProfitTarget = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, execution.Order.Filled, ib30.IBLow, 0, "SwingLongExit", "SW.BEAR.PT");
            }
            if (scalpLongLimitEntry != null && scalpLongLimitEntry == execution.Order && (execution.Order.OrderState == OrderState .Filled ||
                execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState .Cancelled && OrderState.Filled > 0)))
            {
                scalpLongStopLoss = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Filled, 0, (ib30.IBLow - Math.Abs(ib30.IBLow - ib30.IBHigh) / 2), "ShortExit" , "SC.BULL.SL");
                scalpLongProfitTarget = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Filled, ib30.IBHigh, 0, "ShortExit", "SC.BULL.PT");
            }
            if (swingLongLimitEntry != null && swingLongLimitEntry == execution.Order && (execution.Order.OrderState == OrderState .Filled ||
                execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState .Cancelled && OrderState.Filled > 0)))
            {
                swingLongStopLoss = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Filled, 0, (ib30.IBLow - Math.Abs(ib30.IBLow - ib30.IBHigh) / 2), "SwingShortExit" , "SW.BULL.SL");
                swingLongProfitTarget = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Filled, ib30.IBHigh, 0, "SwingShortExit", "SW.BULL.PT");
            }
        }
        private bool probability()
        {
            if (dIB30.Count > 1 && ((ib30.IBHigh < dIB30[dIB30.Count - 2].IBHigh && ib30.IBHigh > dIB30[dIB30.Count - 2].IBLow) ||
                ib30.IBLow < dIB30[dIB30.Count - 2].IBHigh && ib30.IBLow > dIB30[dIB30.Count - 2].IBLow))
            {
                if (Math .Abs(ib30.IBHigh - ib30.IBLow) > enableEntryAboveIBRange)
                {
                    if (ADX(adxPeriod)[0] < enableEntryBelowADX)
                        return true ;
                    else
                        return false ;
                }
                else
                    return false ;
            }
            else
                return false ;
        }
    }
    public class kStats
    {
        public double ADXIB { get; set; }
        public double ADXEntry { get; set; }

    }
}
