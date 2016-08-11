#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using MySql.Data.MySqlClient;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Strategy;
#endregion
using System.Collections.Generic;
using System.Security.Cryptography;
using log4net.Util;

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Analysis on price move after economic release. 
    /// </summary>
    [Description("Analysis on price move after economic release. ")]
    public class headmapEconomicEvent : Strategy
    {
        #region Variables
        private MySqlConnection dbConnection = new MySqlConnection("server=localhost;database=algo;uid=root;password=Password1;");
        //private Dictionary<string, List<string[]>> economicEvents = new Dictionary<string, List<string[]>>();
        List<string[]> currentEvents = new List<string[]>();

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
        protected override void OnBarUpdate()
        {

            if (Bars.FirstBarOfSession)
            {
                string CurrentDate = Time[0].ToShortDateString();
                currentEvents =  this.GetEconomicEventData(CurrentDate);
            }

            foreach (var element in currentEvents)
            {
                if (Time[0] == DateTime.Parse(element[0]))
                {
                    Print(Time[0]);
                    
                    if (element[1] == "USD")
                    { 
                        if (element[2] == "High")
                        {
                            DrawDot("HighDot" + DateTime.Parse(element[0]), true, Time[0], High[0] + 4, Color.Red);
                            Print(string.Format("{0}  {1}  {2}  {3}  {4}  {5} ", DateTime.Parse(element[0]), element[1], element[2], element[3], element[4], element[5]));
                        }
                        else if (element[2] == "Medium")
                        {
                            DrawDot("MediumDot" + DateTime.Parse(element[0]), true, Time[0], High[0] + 4, Color.Orange);
                            Print(string.Format("{0}  {1}  {2}  {3}  {4}  {5} ", DateTime.Parse(element[0]), element[1], element[2], element[3], element[4], element[5]));
                        }
                        else if (element[2] == "Low")
                        {
                            DrawDot("LowDot" + DateTime.Parse(element[0]), true, Time[0], High[0] + 4, Color.Yellow);
                            Print(string.Format("{0}  {1}  {2}  {3}  {4}  {5} ", DateTime.Parse(element[0]), element[1], element[2], element[3], element[4], element[5]));
                        }
                    }
                }
            }

        }

  
        protected override void OnStartUp()
        {

        }

        private List<string[]> GetEconomicEventData(string date)
        {
            dbConnection.Open();

            string query = string.Format("select concat(concat(dateNum,' '),timeEst), currency, impact, event, actual,forecast from economiceventdata where datenum = '{0}'", date);
            List<string[]> recordMatrix = new List<string[]>();
            

            using (MySqlCommand cmd = new MySqlCommand(query, dbConnection))
            using (MySqlDataReader reader = cmd.ExecuteReader())
                while (reader.Read())
                {
                    if (reader.GetString(0).Contains("AM") || reader.GetString(0).Contains("PM"))
                    {
                        string[] record = new string[6];
                        DateTime estModifiedTime = (TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Parse(reader.GetString(0))) == true) ?
                            DateTime.Parse(reader.GetString(0)).AddHours(1) : DateTime.Parse(reader.GetString(0));

                        record[0] = estModifiedTime.ToString();
                        record[1] = reader.GetString(1);
                        record[2] = reader.GetString(2);
                        record[3] = reader.GetString(3);
                        record[4] = reader.GetString(4);
                        record[5] = reader.GetString(5);

                        recordMatrix.Add(record);
                    }
                    
                }
            dbConnection.Close();
            return recordMatrix;
        }

        #region Properties
        #endregion
    }
}
