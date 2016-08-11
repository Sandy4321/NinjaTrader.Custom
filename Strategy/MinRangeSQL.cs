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
using System.Collections;
using MySql.Data.MySqlClient;

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Enter the description of your strategy here
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class MinRangeSQL : Strategy
    {
        #region Variables
        private double MaxRange = 0.00;
        private DateTime EndTimeOfMaxRange;
        private MySqlConnection dbConn;
        private Dictionary<DateTime, double> storeRangeData = new Dictionary<DateTime, double>();
        private string insertQuery = "";
        private int timePeriod;
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
            dbConn.Open();
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            MaxRange = Math.Abs(High[0] - Low[0]);
            EndTimeOfMaxRange = Time[0];

            if (Bars.BarsSinceSession == 2)
            {
                //SET THE TIME PERIOD FOR THE CHART IN QUESTION FOR RECORDING. 
                timePeriod = 60;
            }

            storeRangeData.Add(EndTimeOfMaxRange, MaxRange);
  
        }
        protected override void OnTermination()
        {
            foreach (KeyValuePair<DateTime, double> element in storeRangeData)
            {
                Print(element.Key.ToString());
                Print(element.Value.ToString());
                insertQuery = (String.Format("Insert into RangeTime values ('{0}','{1}','{2}')",element.Key,element.Value,timePeriod));
                Print(insertQuery);
                MySqlCommand insertRangeData = new MySqlCommand(insertQuery, dbConn);
                insertRangeData.ExecuteNonQuery();
            }
        }

        #region Properties
        #endregion
    }
}
