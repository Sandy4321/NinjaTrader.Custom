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
using System.IO;


// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Enter the description of your strategy here
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class eodPNL : Strategy
    {
        #region Variables
        private MySqlConnection dbConn;
        private int readerIdx = 0;
        Dictionary<DateTime,pnlRange> dailyPNLRange = new Dictionary<DateTime,pnlRange>();
        private string queryString = "";//"select left(entryTime,10),profit from trades where entryTime between '2016-02-04 10:10:37 PM' and '2016-02-05 10:10:37 PM'";
        //private string queryString = "select left(entryTime,10),profit from trades";
        private string queryTradeDate = "select (left(entryTime,10)), count(*) from csvtrades group by (left(entryTime,10))";
                                    private double pnlMax = 0;
                                    private double pnlMin = 0;
                            private double pnlOpen = 0;
                            private double pnlClose = 0;
                            private double pnlSummation = 0;
        private DateTime pnlDate;
        private pnlRange todayPNLRange;
        private TradeCsvReader csvData;
        //private List<DateTime> tradeDate = new List<DateTime>();
        private Dictionary<DateTime, Int16> tradeDateStat = new Dictionary<DateTime, Int16>();
        private Dictionary<int, string> dictTradeEntries = new Dictionary<int, string>();
        private List<String> insertEODPNL = new List<String>();
        private List<String> insertOffsetPNL = new List<String>();
        private int lossingTradeCount =0;
        private int winningTradeCount =0;
        private double lossingInDollar = 0;
        private double winningInDollar = 0;
        private DateTime? winningTime = null;
        private DateTime? lossingTime = null;
        private int tradeIndex = 0;

        private double pnlNewClose = 0;

        private double min5High = 0.00;
        private double min5Low = 0.00;
        private double min10High = 0.00;
        private double min10Low = 0.00;
        private double min15High = 0.00;
        private double min15Low = 0.00;
        private double min30High = 0.00;
        private double min30Low = 0.00;
        private double min60High = 0.00;
        private double min60Low = 0.00;

        private double Min5Range =0.00;
        private double Min10Range =0.00;
        private double Min15Range = 0.00;
        private double Min30Range = 0.00;
        private double Min60Range = 0.00;
        private int offsetIndex = 0;
        private double firstClose = 0.00;
        

        
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = true;
            offsetIndex = 0;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        ///         protected override void OnStartUp()
        ///         
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

            MySqlCommand deleteEodTrades = new MySqlCommand("delete from eodTrades", dbConn);
            deleteEodTrades.ExecuteNonQuery();

            MySqlCommand deletefromOffsetPNL = new MySqlCommand("delete from offsetPNL", dbConn);
            deleteEodTrades.ExecuteNonQuery();




            StreamReader sr = new StreamReader(@"C:\Users\Karunyan\Documents\Reports\nn.csv");
            string data = sr.ReadLine();
            while (data != null)
            {
                //Print(data);
                string[] dataArray = data.Split(',');

                if (dataArray[0] != "Trade-#")
                {
                    csvData = new TradeCsvReader()
                    {
                        csvTradeNum = Int32.Parse(dataArray[0]),
                        csvInstrument = (dataArray[1]),
                        csvAccount = (dataArray[2]),
                        csvStrategy = (dataArray[3]),
                        csvMarketPos = (dataArray[4]),
                        csvQuantity = Int32.Parse(dataArray[5]),
                        csvEntryPrice = Double.Parse(dataArray[6]),
                        csvExitPrice = Double.Parse(dataArray[7]),
                        csvEntryTime = DateTime.Parse(dataArray[8]),
                        csvExitTime = DateTime.Parse(dataArray[9]),
                        csvEntryName = (dataArray[10]),
                        csvExitName = (dataArray[11]),
                        csvProfit = Double.Parse(dataArray[12]),
                        csvCumProfit = Double.Parse(dataArray[13]),
                        csvCommission = Double.Parse(dataArray[14]),
                        csvMAE = Double.Parse(dataArray[15]),
                        csvMFE = Double.Parse(dataArray[16]),
                        csvETD = Double.Parse(dataArray[17]),
                        csvBars = Int32.Parse(dataArray[18])
                    };
                     string insertOffSetPNLQuery = (String.Format("Insert into csvtrades values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}');",
                         csvData.csvTradeNum, csvData.csvInstrument, csvData.csvAccount, csvData.csvStrategy, csvData.csvMarketPos, csvData.csvQuantity, csvData.csvEntryPrice, csvData.csvExitPrice, csvData.csvEntryTime,
                         csvData.csvExitTime, csvData.csvEntryName, csvData.csvExitName, csvData.csvProfit, csvData.csvCumProfit, csvData.csvCommission, csvData.csvMAE, csvData.csvMFE, csvData.csvETD, csvData.csvBars));

                     dictTradeEntries.Add(tradeIndex, insertOffSetPNLQuery);
                    tradeIndex++;
                }

                data = sr.ReadLine();
            }

            foreach (KeyValuePair<int, string> element in dictTradeEntries)
            {
                try
                {
                    MySqlCommand insertOffsetPNLtoDB = new MySqlCommand(element.Value, dbConn);
                    insertOffsetPNLtoDB.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    Print(ex.ToString());
                }
                
            }

            using (MySqlCommand commandQueryTradeDate = new MySqlCommand(queryTradeDate, dbConn))
            {
                using (MySqlDataReader reader = commandQueryTradeDate.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        tradeDateStat.Add(reader.GetDateTime(0), reader.GetInt16(1));
                    }
                }
            }
            foreach (KeyValuePair<DateTime, Int16> dateElement in tradeDateStat)
            {
                winningTradeCount = lossingTradeCount = 0;
                lossingInDollar = winningInDollar = 0.00;
                winningTime = lossingTime = null;
                queryString = ("select left(entryTime,10),profit from csvtrades where left(entryTime,10) ='" + dateElement.Key.ToShortDateString() + "'");
                Print(queryString);

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

                        }
                           todayPNLRange = new pnlRange()
                            {
                                High = pnlMax,
                                Low = pnlMin,
                                Open = pnlOpen,
                                Close = pnlClose,
                                Trades = dateElement.Value,
                                WinTrades = winningTradeCount,
                                WinGross = Math.Round(winningInDollar,2),
                                //WinAvgTime = winningTime,
                                LossTrades = lossingTradeCount,
                                LossGross = Math.Round(lossingInDollar,2)
                                //LossAvgTime = lossingTime
                            };
                    }
                }

                string insertPNL = (String.Format("Insert into eodTrades values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                    pnlDate.ToShortDateString(), todayPNLRange.High, todayPNLRange.Low, todayPNLRange.Open, todayPNLRange.Close, dateElement.Value, winningTradeCount, lossingTradeCount, winningInDollar, lossingInDollar));
                
                insertEODPNL.Add(insertPNL);
                dailyPNLRange.Add(pnlDate, todayPNLRange);
                pnlOpen = pnlClose = 0;
                pnlSummation = pnlMax = pnlMin = 0;

            }


            foreach (KeyValuePair<DateTime, pnlRange> offsetDict in dailyPNLRange)
            {
                if (offsetIndex == 0)
                {
                    string insertOffSetPNLQuery = (String.Format("Insert into offsetPNL values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                    offsetDict.Key.ToShortDateString(), offsetDict.Value.High, offsetDict.Value.Low, offsetDict.Value.Open, offsetDict.Value.Close, offsetDict.Value.Trades,
                        offsetDict.Value.WinTrades,offsetDict.Value.LossTrades,offsetDict.Value.WinGross,offsetDict.Value.LossGross));
                    Print(insertOffSetPNLQuery);
                    firstClose = offsetDict.Value.Close;
                    insertOffsetPNL.Add(insertOffSetPNLQuery);
                    
                }
                else
                {
                    double newOpen = offsetDict.Value.Open + firstClose;
                    double newHigh = offsetDict.Value.High + firstClose;
                    double newLow = offsetDict.Value.Low + firstClose;
                    double newClose = offsetDict.Value.Close + firstClose;
                    firstClose = newClose;

                    string insertOffSetPNLQuery = (String.Format("Insert into offsetPNL values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                    offsetDict.Key.ToShortDateString(), newHigh, newLow, newOpen, newClose, offsetDict.Value.Trades,
                        offsetDict.Value.WinTrades,offsetDict.Value.LossTrades,offsetDict.Value.WinGross,offsetDict.Value.LossGross));
                    insertOffsetPNL.Add(insertOffSetPNLQuery);
                    
                }
                offsetIndex++;
            }
            foreach (String element in insertOffsetPNL)
            {
                try
                {
                    MySqlCommand insertOffsetPNLtoDB = new MySqlCommand(element, dbConn);
                    insertOffsetPNLtoDB.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    Print(ex.ToString());
                }
            }
                    
      
                
            
        }
        protected override void OnBarUpdate()
        {

        }

        #region Properties
 
        #endregion
    }
    public class pnlRange
    {
        public double High { get; set; }
        public double Low { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public int Trades { get; set; }
        public int WinTrades { get; set; }
        public double WinGross { get; set; }
        //public DateTime WinAvgTime { get; set; }
        public int LossTrades { get; set; }
        public double LossGross { get; set; }
        //public DateTime LossAvgTime { get; set; }

    }
    public class TradeCsvReader
    {
        public int csvTradeNum { get; set; }
        public string csvInstrument { get; set; }
        public string csvAccount { get; set; }
        public string csvStrategy { get; set; }
        public string csvMarketPos { get; set; }
        public int csvQuantity { get; set; }
        public double csvEntryPrice { get; set; }
        public double csvExitPrice { get; set; }
        public DateTime csvEntryTime { get; set; }
        public DateTime csvExitTime { get; set; }
        public string csvEntryName { get; set; }
        public string csvExitName { get; set; }
        public double csvProfit { get; set; }
        public double csvCumProfit { get; set; }
        public double csvCommission { get; set; }
        public double csvMAE { get; set; }
        public double csvMFE { get; set; }
        public double csvETD { get; set; }
        public int csvBars { get; set; }
    }

}
