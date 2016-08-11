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
    public class ibCandles : Strategy
    {
        #region Variables
            private InitialBalance ib30;
            private InitialBalance ibEOD;
            private int ibBarSinceSession = 0;
            private Dictionary<int, InitialBalance> dIB30 = new Dictionary<int, InitialBalance>();
            private bool recordFirstItemOnly = true;
            private bool recordFirstEODOnly = true;
            private int dayIndex = 0;
            private double dailyOpen;
            private double dailyHigh;
            private double dailyLow;
            private double dailyClose;
            private TimeSpan dayEndTime = new TimeSpan(15, 59, 59);
            private TimeSpan ib30Time = new TimeSpan(09, 35, 00);
            private int timePeriod = 5;
            private MySqlConnection dbConn;
            private DateTime datenum;

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

            //MySqlCommand deleteRangeData = new MySqlCommand("delete from ibAnalysis", dbConn);
            //deleteRangeData.ExecuteNonQuery();
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            if (BarsInProgress == 0)
            {
                double ibHigh = 0;
                double ibLow = 9999;
                double dailyHigh = 0;
                double dailyLow = 9999;
                double ibCummVolume = 0;

                

                if (Bars.BarsSinceSession == 0)
                {
                    recordFirstItemOnly = true;
                    recordFirstEODOnly = true;
                    dailyOpen = Open[0];
                }


                if (Time[0].TimeOfDay >= ib30Time && recordFirstItemOnly == true)
                {
                    ibBarSinceSession = Bars.BarsSinceSession;
                    Print(Time[0].ToString());
                    Print(Bars.BarsSinceSession);
                    datenum = Time[0];

                    ib30 = new InitialBalance()
                    {
                        IBStartTime = Time[Bars.BarsSinceSession],
                        IBEndTime = Time[0],
                        IBClose = Close[0],
                        IBOpen = Open[Bars.BarsSinceSession],
                        IBDate = datenum
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
                }


                if (Time[0].TimeOfDay >= dayEndTime && recordFirstEODOnly == true)
                {

                    for (int i = 0; i <= Bars.BarsSinceSession; i++)
                    {
                        dailyHigh = Math.Max(dailyHigh, High[i]);
                        dailyLow = Math.Min(dailyLow, Low[i]);
                        //ibCummVolume = ibCummVolume + Volume[i];
                    }

                    Print(Time[0].ToString());
                    Print(Bars.BarsSinceSession);

                   // int sessionBarSince = Bars.BarsSinceSession;
                    //dailyHigh = High[HighestBar(High, sessionBarSince)];
                    //dailyLow = Low[LowestBar(Low, sessionBarSince)];
                    //dailyClose = Close[0];


                    ibEOD = new InitialBalance()
                    {
                        IBStartTime = Time[Bars.BarsSinceSession],
                        IBEndTime = Time[0],
                        IBClose = Close[0],
                        IBOpen = Open[Bars.BarsSinceSession],
                        IBDate = datenum
                    };

                    ibEOD.IBLow = dailyLow;
                    ibEOD.IBHigh = dailyHigh;

                    dIB30.Add(dayIndex, ibEOD);
                    dayIndex++;

                    recordFirstEODOnly = false;
                }
                 

            }
             

        #region Properties
        #endregion
    }
        protected override void OnTermination()
        {
            foreach (KeyValuePair<int, InitialBalance> element in dIB30)
            {
                //Print(element.Value.IBClose);
                string insertQuery = (String.Format("Insert into ibAnalysis values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", element.Key, element.Value.IBOpen, element.Value.IBHigh, element.Value.IBLow,element.Value.IBClose,timePeriod,element.Value.IBDate.ToShortDateString()));
                //Print(insertQuery);
                MySqlCommand insertRangeData = new MySqlCommand(insertQuery, dbConn);
                insertRangeData.ExecuteNonQuery();
            }
        }
}
    }
