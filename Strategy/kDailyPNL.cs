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
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.IO;

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Enter the description of your strategy here
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class kDailyPNL : Strategy
    {

        private MySqlConnection dbConn;
        private List<string> processedCSVFileName = new List<string>();
        private string csvTradeDir = @"C:\Users\Karunyan\Documents\Reports\_ninjatraderTradeCSV";
        private string processedCsvTradeDir = @"C:\Users\Karunyan\Documents\Reports\_ninjatraderTradeCSV\_processed";
        bool moveFile = true;

        private List<DateTime> tradeDate = new List<DateTime>();
        private string selectTradeDates = "select distinct(left(entryTime,10)) from kDailyPNL_csvData order by  left(entryTime,10) asc;";
        
        Dictionary<DateTime,pnlRange> cummPNLOffsetInput = new Dictionary<DateTime,pnlRange>();
        bool isFirstDataElement = true;
        pnlRange dailyRanges;
         
        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void OnStartUp()
        {

            #region parse csv to trade table in mysql
            //===============================================================================================================
            //CVS TO SQL: This section includes the program to save new csv files into the database table for processing. 
            //===============================================================================================================

            moveFile = true;
            dbConn = new MySqlConnection("server=localhost;database=algo;uid=root;password=Password1;");
            dbConn.Open();

            //I want to loop through a source directory, grap any new filenames and extra the csv 
            //trade data and save it to the database

            string[] fileEntries = Directory.GetFiles(csvTradeDir);
            foreach (string fileName in fileEntries)
            {
                //Print("Saving the following file to sql: " + fileName);

                StreamReader sr = new StreamReader(Path.Combine(csvTradeDir, fileName));
                string data = sr.ReadLine();
                while (data != null)
                {
                    string[] dataArray = data.Split(',');
                    if (dataArray[0] != "Trade-#")
                    {
                        string insertCSVData = (String.Format("Insert into kDailyPNL_csvData values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}');",
                         (Int32.Parse(dataArray[0])+"."+dataArray[8] + "." + dataArray[9]), Int32.Parse(dataArray[0]), (dataArray[1]), (dataArray[2]), (dataArray[3]),(dataArray[4]), Int32.Parse(dataArray[5]), Double.Parse(dataArray[6]), Double.Parse(dataArray[7]), DateTime.Parse(dataArray[8]),
                         DateTime.Parse(dataArray[9]), (dataArray[10]), (dataArray[11]), Double.Parse(dataArray[12]), Double.Parse(dataArray[13]), Double.Parse(dataArray[14]), Double.Parse(dataArray[15]), 
                         Double.Parse(dataArray[16]), Double.Parse(dataArray[17]), Int32.Parse(dataArray[18])));
                        
                        //I would like to insert this part of the code into a thread so that it can save to the database in the background
                        //Print(insertCSVData);

                        try
                        {
                            MySqlCommand insertData = new MySqlCommand(insertCSVData, dbConn);
                            insertData.ExecuteNonQuery();
                        }
                        catch(Exception ex)
                        {
                           moveFile = false;
                           //Insead of not moving file, output the conflicting statement to text file. 
                           Print(insertCSVData.ToString());
                           Print(ex.ToString());
                        }

                    }
                    data = sr.ReadLine();
                }

                sr.Close();
                if(moveFile)
                    File.Move(fileName, Path.Combine(processedCsvTradeDir, Path.GetFileName(fileName)));
            }

            #endregion 

            #region calculate the daily trade range

            //===============================================================================================================
            //GRADE DAILY TRADES TO GENERATE THE DAYS RANGE
            //===============================================================================================================


            using (MySqlCommand commandQueryTradeDate = new MySqlCommand(selectTradeDates, dbConn))
            {
                using (MySqlDataReader reader = commandQueryTradeDate.ExecuteReader())
                    while (reader.Read())
                        tradeDate.Add(reader.GetDateTime(0));
            }

            foreach (DateTime data in tradeDate)
            {
                if (isFirstDataElement)
                {
                    string clearOldTradeRange = "delete from kDailyPNL_dayRange";
                    MySqlCommand deleteOldData = new MySqlCommand(clearOldTradeRange,dbConn);
                    deleteOldData.ExecuteNonQuery();

                    string clearOldCummMData = "delete from kDailyPNL_cumm";
                    MySqlCommand deleteOldCummData = new MySqlCommand(clearOldCummMData, dbConn);
                    deleteOldData.ExecuteNonQuery();

                    //Print(clearOldTradeRange);
                    Print("Starting the daily range inserts...");
                } 

                DateTime pnlDate;
                int winningTradeCount = 0;
                int lossingTradeCount = 0;
                double pnlSummation = 0;
                double lossingInDollar=0;
                double winningInDollar=0;
                double pnlMax = 0;
                double pnlMin=0;
                double pnlOpen=0;
                double pnlClose=0;
                string insertPNL = "";
                

                //DateTime? winningTime, lossingTime = null;
                string queryString = ("select left(entryTime,10),profit from kDailyPNL_csvData where left(entryTime,10) ='" + data.ToShortDateString() + "'");
                //Print(queryString);

                using (MySqlCommand command = new MySqlCommand(queryString, dbConn))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            if (reader.GetDouble(1) >= 0)
                            {
                                winningTradeCount++;
                                winningInDollar = winningInDollar + reader.GetDouble(1);

                            }
                            else
                            {
                                lossingTradeCount++;
                                lossingInDollar = lossingInDollar + reader.GetDouble(1);

                            }

                            pnlDate = reader.GetDateTime(0);
                            pnlSummation = pnlSummation + reader.GetDouble(1);
                            if (pnlSummation < pnlMin)
                                pnlMin = pnlSummation;

                            if (pnlSummation > pnlMax)
                                pnlMax = pnlSummation;

                            pnlClose = pnlSummation;

                            winningInDollar = Math.Round(winningInDollar, 2);
                            lossingInDollar = Math.Round(lossingInDollar, 2);
                            
                            insertPNL = (String.Format("Insert into kDailyPNL_dayRange values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}');",
                                pnlDate.ToShortDateString(), pnlMax, pnlMin, pnlOpen, pnlClose, winningTradeCount+lossingTradeCount, 
                                winningTradeCount, lossingTradeCount, winningInDollar, lossingInDollar));

                            dailyRanges = new pnlRange()
                            {
                                High = pnlMax,
                                Low = pnlMin,
                                Open = pnlOpen,
                                Close = pnlClose,
                                WinGross = winningInDollar,
                                LossGross = lossingInDollar,
                                WinTrades = winningTradeCount,
                                LossTrades = lossingTradeCount,
                                Trades = winningTradeCount+lossingTradeCount
                            };


                            

                            //Print(insertPNL);

                        }
                    }
                }
                cummPNLOffsetInput.Add(data, dailyRanges);
                try
                {
                    MySqlCommand insertDayRangeData = new MySqlCommand(insertPNL, dbConn);
                    insertDayRangeData.ExecuteNonQuery();
                }
                catch(Exception ex)
                {
                    Print(insertPNL);
                    Print(ex.ToString());
                }
                isFirstDataElement = false;

            }
            #endregion
            
            //===============================================================================================================
            //GENERATE THE CUMMALATIVE PNL
            //===============================================================================================================

            #region calculate the cummalative pnl offset using the daily range data

            //string selectTradeRangeData = "select tradedate,high,low,open,close from kDailyPNL_dayRange";
            double firstClose = 0.00;
            bool isFirstTradeDate = true;
            string insertOffSetPNLQuery;
            foreach (KeyValuePair<DateTime, pnlRange> dictData in cummPNLOffsetInput)
            {
                if (isFirstTradeDate)
                {
                    Print("Inserting Cumm. Stats...");
                    insertOffSetPNLQuery = (String.Format("Insert into kDailyPNL_cumm values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                    dictData.Key.ToShortDateString(), dictData.Value.High, dictData.Value.Low, dictData.Value.Open, dictData.Value.Close, dictData.Value.Trades,
                        dictData.Value.WinTrades, dictData.Value.LossTrades, dictData.Value.WinGross, dictData.Value.LossGross));
                    firstClose = dictData.Value.Close;
                    isFirstTradeDate = false;

                }
                else
                {
                    double newOpen = dictData.Value.Open + firstClose;
                    double newHigh = dictData.Value.High + firstClose;
                    double newLow = dictData.Value.Low + firstClose;
                    double newClose = dictData.Value.Close + firstClose;
                    firstClose = newClose;

                    insertOffSetPNLQuery = (String.Format("Insert into kDailyPNL_cumm values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                    dictData.Key.ToShortDateString(), newHigh, newLow, newOpen, newClose, dictData.Value.Trades,
                        dictData.Value.WinTrades, dictData.Value.LossTrades, dictData.Value.WinGross, dictData.Value.LossGross));
                }

                try
                {
                    MySqlCommand insertDayRangeData = new MySqlCommand(insertOffSetPNLQuery, dbConn);
                    insertDayRangeData.ExecuteNonQuery();
                }
                catch(Exception ex)
                {
                    Print(insertOffSetPNLQuery);
                    Print(ex.ToString());
                }
            }

            #endregion 

        }




            protected override void Initialize()
        {
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {

        }

        #region Properties
        #endregion
    }
}
