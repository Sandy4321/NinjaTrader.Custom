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
using System.IO;
using System.Linq;

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Enter the description of your strategy here
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class dstEconCal : Strategy
    {
        #region Variables
        private MySqlConnection dbConn;
        //private string selectHighImpactEventQuery = "select dateTimeFormat,case when eventItem like '%FOMC Member%' then 'FOMC Member Speaks'else eventItem end as eventItem from economicData where impact ='H' and currency = 'USD' and dateTimeFormat between '2015-01-01' and '2015-12-31'";
        private string selectHighImpactEventQuery = "select dateTimeFormat,case when eventItem like '%FOMC Member%' then 'FOMC Member Speaks'else eventItem end as eventItem, impact from economicData where currency = 'USD'";
        private List<DateTime> listHighImpactEvents = new List<DateTime>();
        private List<string> insertHighImpactEventtoDB = new List<string>();
        private DateTime min5Spread;
        private DateTime min10Spread;
        private DateTime min15Spread;
        private double threeBarsMaxSpread = 0;
        private DateTime insertDateFormatTime;

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
        protected override void OnStartUp()
        {
            dbConn = new MySqlConnection("server=localhost;database=algo;uid=root;password=Password1;");
            dbConn.Open();

            using (MySqlCommand queryHighImpactItem = new MySqlCommand(selectHighImpactEventQuery, dbConn))
            {
                using (MySqlDataReader reader = queryHighImpactItem.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime dstModifiedTime = (TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Parse(reader.GetString(0))) == true) ?
                            DateTime.Parse(reader.GetString(0)).AddHours(1) : DateTime.Parse(reader.GetString(0));

                        string insertDSTModifiedData = String.Format("Insert into dstEconData values ('{0}','USD','{2}','{1}','0','0')", dstModifiedTime, reader.GetString(1),reader.GetString(2));

                        insertHighImpactEventtoDB.Add(insertDSTModifiedData);
                        

                        listHighImpactEvents.Add(dstModifiedTime);
                    }
                }
            }

            foreach (string insertSQL in insertHighImpactEventtoDB)
            {
                MySqlCommand dstEconData = new MySqlCommand(insertSQL,dbConn);
                        dstEconData.ExecuteNonQuery();
            }
        }
        protected override void OnBarUpdate()
        {

            if (listHighImpactEvents.Contains(Time[0]))
            {
                threeBarsMaxSpread = 0;

                min5Spread = Time[0].AddMinutes(5);
                min10Spread = Time[0].AddMinutes(10);
                min15Spread = Time[0].AddMinutes(15);

                threeBarsMaxSpread = Math.Max(threeBarsMaxSpread, Math.Abs(High[0] - Low[0]));
                insertDateFormatTime = Time[0];
            }

            if (Time[0] == min5Spread)
            {
                threeBarsMaxSpread = Math.Max(threeBarsMaxSpread, Math.Abs(High[0] - Low[0]));
            }
            if (Time[0] == min10Spread)
            {
                threeBarsMaxSpread = Math.Max(threeBarsMaxSpread, Math.Abs(High[0] - Low[0]));
            }
            if (Time[0] == min15Spread)
            {
                threeBarsMaxSpread = Math.Max(threeBarsMaxSpread, Math.Abs(High[0] - Low[0]));
                double threeBarRange = Math.Abs(Close[0] - Open[2]);
                MyThreeBarSpreadInsert(insertDateFormatTime, threeBarsMaxSpread, threeBarRange);
            }

        }
        protected override void OnTermination()
        {
 
                 //MySqlCommand deleteDSTEconData = new MySqlCommand("delete from dstEconData",dbConn);
                 //deleteDSTEconData.ExecuteNonQuery();
            dbConn.Close();
        }
        private void MyThreeBarSpreadInsert(DateTime time, double spread, double range)
        {
            string updateSpreadSQL = String.Format("update dstEconData set threeBarMaxSpread={0},threeBarRange={2} where estDateTime='{1}';", spread, time, range);
            MySqlCommand updateSQL = new MySqlCommand(updateSpreadSQL, dbConn);
            updateSQL.ExecuteNonQuery();
        }

        #region Properties
        #endregion
    }
}
