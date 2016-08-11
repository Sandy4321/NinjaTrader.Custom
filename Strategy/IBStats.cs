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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Enter the description of your strategy here
    /// </summary>
    [Description("Record Instrument Stats")]
    public class IBStats : Strategy
    {
        #region Variables

        private string path = "c:\\log\\ibStats.log";
        private MySqlConnection dbConn;

        Dictionary<int, IBDataSeries> dIB30 = new Dictionary<int, IBDataSeries>();
        Dictionary<int, IBDataSeries> dIB60 = new Dictionary<int, IBDataSeries>();
        Dictionary<int, IBDataSeries> dIBEOD = new Dictionary<int, IBDataSeries>();

        int idxIB30 = 0;
        int idxIB60 = 0;
        int idxIBEOD = 0;
        int ibStartTime = 093000;
        int ib30Time = 100000;
        int ib60Time = 103000;
        int ibEndTime = 155959;

        bool recordIB30 = true;
        bool recordIB60 = true;
        bool recordIBEOD = true;


        int emaPrimaryPeriod = 20;
        int emaReferencePeriod = 50;

        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = true;
            Enabled = true;
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
                switch (ex.Number)
                {
                    case 0:
                        Print("Cannot connect to server. Contact administrator");
                        break;
                    case 1045:
                        Print("Invalid username/passwor, please try again");
                        break;
                }
            }

        }
        protected override void OnTermination()
        {
            File.WriteAllText(path, string.Empty);
            dbConn.Close();
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            //string kObj = "OnBarUpdate";
            if (Bars == null) return;

            IBDataSeries ib30;
            IBDataSeries ib60;
            IBDataSeries ibEOD;

            #region IBPrior
            if (Bars.BarsSinceSession == 0)
            {
                recordIB30 = true;
                recordIBEOD = true;
                recordIB60 = true;

                ib30 = new IBDataSeries()
                {
                    StartBarSinceSession = Bars.BarsSinceSession,
                    TradeDate = Bars.GetTradingDayFromLocal(Time[0]).ToShortDateString(),
                    PriorDayOpen = Bars.GetDayBar(1).Open,
                    PriorDayHigh = Bars.GetDayBar(1).High,
                    PriorDayLow = Bars.GetDayBar(1).Low,
                    PriorDayClose = Bars.GetDayBar(1).Close,
                    PriorVolume = Bars.GetDayBar(1).Volume,
                    CurrDayOpen = Open[0],
                    IBOpen = Open[0]
                };
                ib60 = new IBDataSeries()
                {
                    StartBarSinceSession = Bars.BarsSinceSession,
                    TradeDate = Bars.GetTradingDayFromLocal(Time[0]).ToShortDateString(),
                    PriorDayOpen = Bars.GetDayBar(1).Open,
                    PriorDayHigh = Bars.GetDayBar(1).High,
                    PriorDayLow = Bars.GetDayBar(1).Low,
                    PriorDayClose = Bars.GetDayBar(1).Close,
                    PriorVolume = Bars.GetDayBar(1).Volume,
                    CurrDayOpen = Open[0],
                    IBOpen = Open[0]
                };
                ibEOD = new IBDataSeries()
                {
                    StartBarSinceSession = Bars.BarsSinceSession,
                    TradeDate = Bars.GetTradingDayFromLocal(Time[0]).ToShortDateString(),
                    PriorDayOpen = Bars.GetDayBar(1).Open,
                    PriorDayHigh = Bars.GetDayBar(1).High,
                    PriorDayLow = Bars.GetDayBar(1).Low,
                    PriorDayClose = Bars.GetDayBar(1).Close,
                    PriorVolume = Bars.GetDayBar(1).Volume,
                    CurrDayOpen = Open[0],
                    IBOpen = Open[0]
                };
                dIB30.Add(idxIB30, ib30);
                dIB60.Add(idxIB60, ib60);
                dIBEOD.Add(idxIBEOD, ibEOD);

                idxIB30++;
                idxIB60++;
                idxIBEOD++;

                //kLog(kObj, "PRIOR", String.Format(" {0}    prior. O {2}   H {3}   L {4}    C {5}   V {6}  R {7}",
                //    ib.StartBarSinceSession,
                //    ib.TradeDate,
                //    ib.PriorDayOpen.ToString("0.00"),
                //    ib.PriorDayHigh.ToString("0.00"),
                //    ib.PriorDayLow.ToString("0.00"),
                //    ib.PriorDayClose.ToString("0.00"),
                //    ib.PriorVolume.ToString("0"),
                //    (ib.PriorDayClose - ib.PriorDayOpen).ToString("0.00")
                //    ));
            }
            #endregion

            #region IB30
            if (ToTime(Time[0]) >= ib30Time && recordIB30 == true)
            {
                double ibHigh = 0;
                double ibLow = 9999;
                int emaCrossCount = 0;
                int ibBullCount = 0;
                int ibBearCount = 0;
                int ibDojiCount = 0;
                double ibVolume = 0;
                string name = "IB30";


                for (int i = 0; i < Bars.BarsSinceSession; i++)
                {
                    ibHigh = Math.Max(ibHigh, High[i]);
                    ibLow = Math.Min(ibLow, Low[i]);
                    ibVolume = ibVolume + Volume[i];

                    if (Close[i] - Open[i] > 0) ibBullCount++;
                    else if (Close[i] - Open[i] < 0) ibBearCount++;
                    else if (Close[i] - Open[i] == 0) ibDojiCount++;

                    if ((CrossAbove(EMA(emaPrimaryPeriod), EMA(emaReferencePeriod), i)) || (CrossBelow(EMA(emaPrimaryPeriod), EMA(emaReferencePeriod), i))) emaCrossCount++;
                }

                if (dIB30.Count > 0)
                {
                    dIB30[dIB30.Count - 1].IBClose = Close[0];
                    dIB30[dIB30.Count - 1].IBEndBar = Bars.BarsSinceSession;
                    dIB30[dIB30.Count - 1].IBLow = ibLow;
                    dIB30[dIB30.Count - 1].IBVolume = ibVolume;
                    dIB30[dIB30.Count - 1].IBHigh = ibHigh;
                    dIB30[dIB30.Count - 1].IBBullCount = ibBullCount;
                    dIB30[dIB30.Count - 1].IBBearCount = ibBearCount;
                    dIB30[dIB30.Count - 1].IBDojiCount = ibDojiCount;
                    dIB30[dIB30.Count - 1].IBEmaPrimaryAtClose = EMA(emaPrimaryPeriod)[0];
                    dIB30[dIB30.Count - 1].IBEmaReferenceAtClose = EMA(emaReferencePeriod)[0];
                    dIB30[dIB30.Count - 1].IBEmaCrossCount = emaCrossCount;


                    string insertIB30 = String.Format("Insert into algoIB values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}')",
                        dIB30[dIB30.Count - 1].TradeDate.ToString()+"-"+name,
                        dIB30[dIB30.Count - 1].TradeDate,
                        name,
                        dIB30[dIB30.Count - 1].IBOpen,
                        dIB30[dIB30.Count - 1].IBHigh,
                        dIB30[dIB30.Count - 1].IBLow,
                        dIB30[dIB30.Count - 1].IBClose,
                        dIB30[dIB30.Count - 1].IBVolume,
                        dIB30[dIB30.Count - 1].StartBarSinceSession,
                        dIB30[dIB30.Count - 1].IBEndBar,
                        dIB30[dIB30.Count - 1].IBBullCount,
                        dIB30[dIB30.Count - 1].IBBearCount,
                        dIB30[dIB30.Count - 1].IBDojiCount,
                        dIB30[dIB30.Count - 1].IBEmaCrossCount,
                        dIB30[dIB30.Count - 1].IBEmaPrimaryAtClose.ToString("0.00"),
                        dIB30[dIB30.Count - 1].IBEmaReferenceAtClose.ToString("0.00")
                        );

                    Print(insertIB30);

                    MySqlCommand insertIBQuery30 = new MySqlCommand(insertIB30, dbConn);
                    insertIBQuery30.ExecuteNonQuery();


                    recordIB30 = false;  // to prevent double prints
                }
            }
            #endregion
            #region IB60
            if (ToTime(Time[0]) >= ib60Time && recordIB60 == true)
            {
                double ibHigh = 0;
                double ibLow = 9999;
                int emaCrossCount = 0;
                int ibBullCount = 0;
                int ibBearCount = 0;
                int ibDojiCount = 0;
                double ibVolume = 0;
                string name = "IB60";


                for (int i = 0; i < Bars.BarsSinceSession; i++)
                {
                    ibHigh = Math.Max(ibHigh, High[i]);
                    ibLow = Math.Min(ibLow, Low[i]);
                    ibVolume = ibVolume + Volume[i];

                    if (Close[i] - Open[i] > 0) ibBullCount++;
                    else if (Close[i] - Open[i] < 0) ibBearCount++;
                    else if (Close[i] - Open[i] == 0) ibDojiCount++;

                    if ((CrossAbove(EMA(emaPrimaryPeriod), EMA(emaReferencePeriod), i)) || (CrossBelow(EMA(emaPrimaryPeriod), EMA(emaReferencePeriod), i))) emaCrossCount++;
                }

                if (dIB60.Count > 0)
                {
                    dIB60[dIB60.Count - 1].IBClose = Close[0];
                    dIB60[dIB60.Count - 1].IBEndBar = Bars.BarsSinceSession;
                    dIB60[dIB60.Count - 1].IBLow = ibLow;
                    dIB60[dIB60.Count - 1].IBVolume = ibVolume;
                    dIB60[dIB60.Count - 1].IBHigh = ibHigh;
                    dIB60[dIB60.Count - 1].IBBullCount = ibBullCount;
                    dIB60[dIB60.Count - 1].IBBearCount = ibBearCount;
                    dIB60[dIB60.Count - 1].IBDojiCount = ibDojiCount;
                    dIB60[dIB60.Count - 1].IBEmaPrimaryAtClose = EMA(emaPrimaryPeriod)[0];
                    dIB60[dIB60.Count - 1].IBEmaReferenceAtClose = EMA(emaReferencePeriod)[0];
                    dIB60[dIB60.Count - 1].IBEmaCrossCount = emaCrossCount;

                    string insertIB60 = String.Format("Insert into algoIB values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}')",
                        dIB60[dIB60.Count - 1].TradeDate.ToString() + "-" + name,
                        dIB60[dIB60.Count - 1].TradeDate,
                        name,
                        dIB60[dIB60.Count - 1].IBOpen,
                        dIB60[dIB60.Count - 1].IBHigh,
                        dIB60[dIB60.Count - 1].IBLow,
                        dIB60[dIB60.Count - 1].IBClose,
                        dIB60[dIB60.Count - 1].IBVolume,
                        dIB60[dIB60.Count - 1].StartBarSinceSession,
                        dIB60[dIB60.Count - 1].IBEndBar,
                        dIB60[dIB60.Count - 1].IBBullCount,
                        dIB60[dIB60.Count - 1].IBBearCount,
                        dIB60[dIB60.Count - 1].IBDojiCount,
                        dIB60[dIB60.Count - 1].IBEmaCrossCount,
                        dIB60[dIB60.Count - 1].IBEmaPrimaryAtClose.ToString("0.00"),
                        dIB60[dIB60.Count - 1].IBEmaReferenceAtClose.ToString("0.00")
                        );

                    Print(insertIB60);
                    try
                    {
                        MySqlCommand insertIBQuery60 = new MySqlCommand(insertIB60, dbConn);
                        insertIBQuery60.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Print(ex.ToString());
                    }
                    recordIB60 = false;  // to prevent double prints
                }
            }

            #endregion
            #region IBEOD
            if (ToTime(Time[0]) >= ibEndTime && recordIBEOD == true)
            {
                double ibHigh = 0;
                double ibLow = 9999;
                int emaCrossCount = 0;
                int ibBullCount = 0;
                int ibBearCount = 0;
                int ibDojiCount = 0;
                double ibVolume = 0;
                string name = "IBEOD";


                for (int i = 0; i < Bars.BarsSinceSession; i++)
                {
                    ibHigh = Math.Max(ibHigh, High[i]);
                    ibLow = Math.Min(ibLow, Low[i]);
                    ibVolume = ibVolume + Volume[i];

                    if (Close[i] - Open[i] > 0) ibBullCount++;
                    else if (Close[i] - Open[i] < 0) ibBearCount++;
                    else if (Close[i] - Open[i] == 0) ibDojiCount++;

                    if ((CrossAbove(EMA(emaPrimaryPeriod), EMA(emaReferencePeriod), i)) || (CrossBelow(EMA(emaPrimaryPeriod), EMA(emaReferencePeriod), i))) emaCrossCount++;
                }

                if (dIBEOD.Count > 0)
                {
                    dIBEOD[dIBEOD.Count - 1].IBClose = Close[0];
                    dIBEOD[dIBEOD.Count - 1].IBEndBar = Bars.BarsSinceSession;
                    dIBEOD[dIBEOD.Count - 1].IBLow = ibLow;
                    dIBEOD[dIBEOD.Count - 1].IBVolume = ibVolume;
                    dIBEOD[dIBEOD.Count - 1].IBHigh = ibHigh;
                    dIBEOD[dIBEOD.Count - 1].IBBullCount = ibBullCount;
                    dIBEOD[dIBEOD.Count - 1].IBBearCount = ibBearCount;
                    dIBEOD[dIBEOD.Count - 1].IBDojiCount = ibDojiCount;
                    dIBEOD[dIBEOD.Count - 1].IBEmaPrimaryAtClose = EMA(emaPrimaryPeriod)[0];
                    dIBEOD[dIBEOD.Count - 1].IBEmaReferenceAtClose = EMA(emaReferencePeriod)[0];
                    dIBEOD[dIBEOD.Count - 1].IBEmaCrossCount = emaCrossCount;


                    string insertIBEOD = String.Format("Insert into algoIB values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}')",
                        dIBEOD[dIBEOD.Count - 1].TradeDate.ToString() + "-" + name,
                        dIBEOD[dIBEOD.Count - 1].TradeDate,
                        name,
                        dIBEOD[dIBEOD.Count - 1].IBOpen,
                        dIBEOD[dIBEOD.Count - 1].IBHigh,
                        dIBEOD[dIBEOD.Count - 1].IBLow,
                        dIBEOD[dIBEOD.Count - 1].IBClose,
                        dIBEOD[dIBEOD.Count - 1].IBVolume,
                        dIBEOD[dIBEOD.Count - 1].StartBarSinceSession,
                        dIBEOD[dIBEOD.Count - 1].IBEndBar,
                        dIBEOD[dIBEOD.Count - 1].IBBullCount,
                        dIBEOD[dIBEOD.Count - 1].IBBearCount,
                        dIBEOD[dIBEOD.Count - 1].IBDojiCount,
                        dIBEOD[dIBEOD.Count - 1].IBEmaCrossCount,
                        dIBEOD[dIBEOD.Count - 1].IBEmaPrimaryAtClose.ToString("0.00"),
                        dIBEOD[dIBEOD.Count - 1].IBEmaReferenceAtClose.ToString("0.00")
                        );

                    Print(insertIBEOD);
                    MySqlCommand insertIBQuery = new MySqlCommand(insertIBEOD, dbConn);
                    insertIBQuery.ExecuteNonQuery();

                    recordIBEOD = false;  // to prevent double prints
                }
            }

            #endregion
        }
        private void kLog(string ClassMethod, string MsgType, string Msg)
        {
            string _msgOut = (String.Format("{0}  {1} [{2}]  {3}  [{4}]  {5}",
                Time[0].ToString("dd/MM/yyyy"),
                Time[0].ToString("HH:mm:ss"),
                Bars.BarsSinceSession,
                MsgType,
                ClassMethod,
                Msg)) + Environment.NewLine;
            File.AppendAllText(path, _msgOut);
        }
        private void kLog(string Msg)
        {
            string _msgOut = (String.Format("{0}",
                Msg)) + Environment.NewLine;
            File.AppendAllText(path, _msgOut);
        }
    }
    public class IBDataSeries
    {
        public string TradeDate { get; set; }
        public int StartBarSinceSession { get; set; }
        public int EndOfDayBar { get; set; }

        public int IBEndBar { get; set; }
        public double IBHigh { get; set; }
        public double IBLow { get; set; }
        public double IBOpen { get; set; }
        public double IBClose { get; set; }
        public double IBVolume { get; set; }
        public int IBBullCount { get; set; }
        public int IBBearCount { get; set; }
        public int IBDojiCount { get; set; }
        public int IBEmaCrossCount { get; set; }
        public double IBEmaPrimaryAtClose { get; set; }
        public double IBEmaReferenceAtClose { get; set; }

        public double PriorVolume { get; set; }
        public double CurrVolume { get; set; }
        public double PriorDayClose { get; set; }
        public double PriorDayOpen { get; set; }
        public double PriorDayHigh { get; set; }
        public double PriorDayLow { get; set; }
        public double CurrDayClose { get; set; }
        public double CurrDayOpen { get; set; }
        public double CurrDayHigh { get; set; }
        public double CurrDayLow { get; set; }
        public int CurrBullCount { get; set; }
        public int CurrBearCount { get; set; }
        public int CurrDojiCount { get; set; }

    }
}



