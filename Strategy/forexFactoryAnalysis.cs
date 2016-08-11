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


// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Enter the description of your strategy here
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class forexFactoryAnalysis : Strategy
    {
        #region Variables
        private MySqlConnection dbConn;
        private List<string> insertForexFactoryList = new List<string>();
        private TimeSpan amPMConverter = new TimeSpan(12, 00, 00);
        private string selectHighImpactEvents = "select distinct(dateTimeFormat) from economicData where impact ='H' and currency = 'USD' and dateTimeFormat between '2015-01-01' and '2015-12-31'";
        private List<DateTime> listHighImpactTime = new List<DateTime>();
        private HighImpactEventData HIData;
        private DateTime min5Spread;
        private DateTime min10Spread;
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
        protected override void OnStartUp()
        {
            dbConn = new MySqlConnection("server=localhost;database=algo;uid=root;password=Password1;");
            dbConn.Open();

            StreamReader sr = new StreamReader(@"C:\Users\Karunyan\Documents\Reports\allnews.txt");
            string data = sr.ReadLine();
            while (data != null)
            {
                string[] dataArray = data.Split(',');
                
                TimeSpan convertTimeToReadable = TimeSpan.Parse(dataArray[1]);
                if (convertTimeToReadable > amPMConverter)
                    dataArray[1] = (convertTimeToReadable - amPMConverter) + " PM";
                else if (convertTimeToReadable < amPMConverter)
                    dataArray[1] = convertTimeToReadable + " AM";
                else if (convertTimeToReadable == amPMConverter)
                    dataArray[1] = convertTimeToReadable + " PM";





                string InsertForexFactoryToSQL = (String.Format("Insert into economicData values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','0','0','0');", dataArray[0], dataArray[1], dataArray[2], dataArray[3], dataArray[4], dataArray[5], dataArray[6],dataArray[7],(dataArray[0] + " " + dataArray[1])));
                insertForexFactoryList.Add(InsertForexFactoryToSQL);
                
                data = sr.ReadLine();
            }

            /* COMMENT THIS OUT WHEN YOU HAVE ALREADY SAVED THE DATA TO THE DATABASE
             */ 
            foreach (string element in insertForexFactoryList)
            {
                MySqlCommand insertStatement = new MySqlCommand(element, dbConn);
                insertStatement.ExecuteNonQuery();
            }
            //*/

            /* using (MySqlCommand queryHighImpactItem = new MySqlCommand(selectHighImpactEvents, dbConn))
             {
                 using (MySqlDataReader reader = queryHighImpactItem.ExecuteReader())
                 {
                     while (reader.Read())
                     {

                         DateTime dstModifiedTime = (TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Parse(reader.GetString(0))) == true) ?
                             DateTime.Parse(reader.GetString(0)).AddHours(1) : DateTime.Parse(reader.GetString(0));

                         listHighImpactTime.Add(DateTime.Parse(reader.GetString(0)));

                     }
                 }
             }*/

        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {/*
            if (listHighImpactTime.Contains(Time[0]))
            {
                threeBarsMaxSpread = 0;

                min5Spread = Time[0].AddMinutes(5);
                min10Spread = Time[0].AddMinutes(10);

                threeBarsMaxSpread = Math.Max(threeBarsMaxSpread, (High[0] - Low[0]));
                insertDateFormatTime = Time[0];
            }

            if (Time[0] == min5Spread)
            {
                threeBarsMaxSpread = Math.Max(threeBarsMaxSpread, (High[0] - Low[0]));
            }
            if (Time[0] == min10Spread)
            {
                threeBarsMaxSpread = Math.Max(threeBarsMaxSpread, (High[0] - Low[0]));
                double threeBarRange = Math.Abs(High[0]-Low[3]);
                MyThreeBarSpreadInsert(insertDateFormatTime, threeBarsMaxSpread, threeBarRange);
            }*/
        }
        protected override void OnTermination()
        {
            dbConn.Close();
        }

        private void MyThreeBarSpreadInsert(DateTime time, double spread, double range)
        {
            bool isDaylight = TimeZoneInfo.Local.IsDaylightSavingTime(time);

            int isDayLightInteger;

            if (isDaylight == true)
            {
                isDayLightInteger = 1;
            }
            else 
            {
                isDayLightInteger = 0;
            }
            
            string timeString = time.ToString();

            if (time.ToString().Length == 21)
            {
                timeString = time.ToShortDateString() + " 0" + time.ToLongTimeString();
            //    Print(timeString);
            }
            string updateSpreadSQL = String.Format("update economicData set threeBarMaxSpread={0},threeBarRange={2},isDST={3} where dateTimeFormat='{1}';", spread, timeString, range,isDayLightInteger);
            //Print(updateSpreadSQL);
            //MySqlCommand updateSQL = new MySqlCommand(updateSpreadSQL, dbConn);
            //updateSQL.ExecuteNonQuery();
        }


        #region Properties
        #endregion
    }
    public class HighImpactEventData
    {
       public DateTime eventTime { get; set; }
       public string eventItem { get; set; }
        public double eventSpread { get; set; }

        

    }
}
