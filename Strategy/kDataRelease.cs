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
    /// Caclulate the spread per bar and correlate to data release
    /// </summary>
    [Description("Caclulate the spread per bar and correlate to data release")]
    public class kDataRelease : Strategy
    {
        
        #region Variables
        private MySqlConnection dbConn;
        private SpreadandVolume currentBar;
        private Dictionary<int, SpreadandVolume> dSpreadandVolume = new Dictionary<int, SpreadandVolume>();

        int idxSpreadandVolume = 0;
        int barGapCounter = 0;

        private IOrder swingBearEntry = null;
        private IOrder SwingBullEntry = null;
        private IOrder SwingBullEntryStopLoss = null;
        private IOrder SwingBullEntryProfitTarget = null;
        private IOrder swingBearEntryStopLoss =null;
        private IOrder swingBearEntryProfitTarget = null;



        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = true;
            Unmanaged = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            if (Bars.BarsSinceSession == 0)
            {
                swingBearEntry = SwingBullEntry = null;
            }


            //if (Position.MarketPosition == MarketPosition.Long)
            //{
            //    ExitLong();
            //}
            //if (Position.MarketPosition == MarketPosition.Short)
            //{
            //    ExitShort();
            //}


            if (CalculateOnBarClose == true)
            {

                currentBar = new SpreadandVolume()
                {

                    DateTime = Time[0],
                    High = High[0],
                    Low = Low[0],
                    Volume = Volume[0]
                };

//Database inster statemenets and logic     
       
                //Store data information
                
                /*
                Print(String .Format("d. {0}  h. {1}  l. {2}  v. {3}",currentBar.DateTime.ToShortDateString() + " " + 
                    currentBar.DateTime.ToString("HH:mm:ss") ,currentBar.High,currentBar.Low,currentBar.Volume));
                */
                dSpreadandVolume.Add(idxSpreadandVolume, currentBar);
                idxSpreadandVolume++;

               
                string insertSpreadandVolumeData = String.Format("Insert into dataSpreadandVolume values ('{0}','{1}','{2}','{3}')",
                    dSpreadandVolume[dSpreadandVolume.Count - 1].DateTime.ToShortDateString() + " " + 
                    dSpreadandVolume[dSpreadandVolume.Count - 1].DateTime.ToString("HH:mm:ss"),
                    dSpreadandVolume[dSpreadandVolume.Count - 1].High,
                    dSpreadandVolume[dSpreadandVolume.Count-1].Low,
                    dSpreadandVolume[dSpreadandVolume.Count-1].Volume
                        );

                MySqlCommand insertDataQuery = new MySqlCommand(insertSpreadandVolumeData, dbConn);
                insertDataQuery.ExecuteNonQuery(); 
            }

            if (FirstTickOfBar == true)
            {
                //Bullish case
                if (Low[0] >= High[1]+TickSize*1 && High[1] == Close[1])
                {
                    barGapCounter++;
                    Print("Bar gap" + barGapCounter);

                    SwingBullEntry = SubmitOrder(0, OrderAction.Buy, OrderType.Market, 1, 0, 0, "IBEntrySwing", "SW.BULL.MKT");

                }
                //Bearish case
                if (High[0] <= Low[1] - TickSize * 1 && Low[1] == Close[1])
                {
                    barGapCounter++;
                    Print("Bar gap" + barGapCounter);

                    swingBearEntry = SubmitOrder(0, OrderAction.SellShort, OrderType.Market, 1, 0, 0, "IBEntrySwing", "SW.BEAR.MKT");
                }


            }




        }

        protected override void OnExecution(IExecution execution)
        {
            if (SwingBullEntry != null && SwingBullEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled ||
                 execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                //SwingBullEntryStopLoss = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, execution.Order.Filled, 0,execution.Order.AvgFillPrice-TickSize*4, "LongExit", "SC.BEAR.SL");
                //SwingBullEntryProfitTarget = SubmitOrder(0, OrderAction.Sell, OrderType.Limit, execution.Order.Filled, execution.Order.AvgFillPrice + TickSize * 4, 0, "LongExit", "SC.BEAR.PT");
            }
            if (swingBearEntry != null && swingBearEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled ||
     execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                //swingBearEntryStopLoss = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, execution.Order.Filled, 0, execution.Order.AvgFillPrice + TickSize * 4, "LongExit", "SC.BEAR.SL");
                //swingBearEntryProfitTarget = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Limit, execution.Order.Filled, execution.Order.AvgFillPrice - TickSize * 4, 0, "LongExit", "SC.BEAR.PT");
            }
        }
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

            MySqlCommand delete = new MySqlCommand("delete from dataSpreadandVolume", dbConn);
            delete.ExecuteNonQuery();
            dbConn.Close();
            Print("Delete data from table and close database connection...");
        }


        #region Properties
        #endregion
    }
    public class SpreadandVolume
    {
        public DateTime DateTime { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Volume { get; set; }
    }
}
