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
    /// Enter the description of your strategy here
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class matlabTradeReport : Strategy
    {
        #region Variables
            private MySqlConnection dbConn;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = true;
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

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            if (Performance.RealtimeTrades.Count > 0)
            {
                Trade lastTrade = Performance.RealtimeTrades[Performance.RealtimeTrades.Count - 1];
                double lastProfit = lastTrade.ProfitCurrency * lastTrade.Quantity;
                Print("The last trade's profit is " + lastProfit);
            }
        }

        #region Properties
        #endregion
    }
}
