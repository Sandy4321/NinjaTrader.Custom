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
using NinjaTrader.Gui.Chart;
#endregion
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Enter the description of your new custom indicator here
    /// </summary>
    [Description("Enter the description of your new custom indicator here")]
    public class kEconomicDataSet : Indicator
    {
        #region Variables
        private MySqlConnection dbConn = new MySqlConnection("server = localhost; database=algo;uid=root;password=Password1;");
        string queryYellenSpeaks = "select concat(concat(datenum,' '),timeEst) as datenum from economiceventdata where event = '  Fed Chair Yellen Speaks  ' order by concat(concat(datenum,' '),timeEst) desc";

        private string queryEmploymentChange =
            "select concat(concat(datenum,' '),timeEst),id,event,actual,forecast as datenum from economiceventdata where event = '  Non-Farm Employment Change  ' order by concat(concat(datenum,' '),timeEst) desc";


        //TO DO: This case was created for when the event does not have a actual vs. forcast data. 

        List<DateTime>  YellenSpeaksTime = new List<DateTime>();
        List<DateTime>  YellenSpeaksTimeHour = new List<DateTime>();
        List<String> YellenSpeaksTimeDaily = new   List<String>();
        

        List<DateTime> nonFarmTime = new List<DateTime>();
        List<DateTime> nonFarmTimeHour = new List<DateTime>();
        List<String> nonFarmTimeDaily = new List<String>();
        //TO DO: Create statement for when there is a actual and forcast event. 
        //I would like to conduct a statistical analysis of when the result is better or worse than expected. 


        Dictionary<string, List<EconomicData>> dictEvent = new Dictionary<string, List<EconomicData>>();

        private string queryUniqueDate = "select distinct(datenum) from economiceventdata order by datenum asc";


        private string query = "select* from economiceventdata where datenum between '2016-04-01' and '2016-09-01' order by datenum asc";
        private string currentDateIndex = "";
        private List<EconomicData> currentDataList = new List<EconomicData>();
        Dictionary<DateTime, List<EconomicData>> currentTimeEvent = new Dictionary<DateTime, List<EconomicData>>();
        Dictionary<DateTime,string> drawTextDictionary = new Dictionary<DateTime, string>(); 
        List<DateTime> listDate = new List<DateTime>();

        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Overlay				= false;
        }

        protected override void OnStartUp()
        {

            dbConn.Open();

            using (MySqlCommand command = new MySqlCommand(query, dbConn))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        
                        if (reader.GetString(2).Contains("PM") || reader.GetString(2).Contains("AM"))
                        {
                            
                            //Print(reader.GetString(1));
                            EconomicData data = new EconomicData()
                            {
                                Id = reader.GetUInt32(0),
                                Timestamp = DateTime.Parse(reader.GetString(1) + " " + reader.GetString(2)),
                                Currency = reader.GetString(3),
                                Impact = reader.GetString(4),
                                Event = reader.GetString(5),
                                Actual = reader.GetString(6),
                                Forecast = reader.GetString(7),
                                Previous = reader.GetString(8)
                            };

                            
                            if (!dictEvent.ContainsKey(reader.GetString(1))) //if it does not contain date
                            {
                                //Print(string.Format("{0}  |  {1}  |  {2}  |  {3}  |  {4}  |  {5}  |   {6}  |   {7} ", data.Id, data.Timestamp, data.Currency, data.Impact, data.Event, data.Actual, data.Forecast, data.Previous));

                                List<EconomicData> evenStats = new List<EconomicData>();
                                evenStats.Add(data);
                                dictEvent.Add(data.Timestamp.ToShortDateString(), evenStats);

                            }
                            else //if it contains the key, append the data
                            {

                                dictEvent[data.Timestamp.ToShortDateString()].Add(data);
                            }
                        }
                    }
                }

            }
                dbConn.Close();




            #region Yellen test case

            //TO DO: The database time is written in the current timezone. Therefore use the lastupdate time to verify the isDst and modifity the code accordingly. 

            dbConn.Open();
            using (MySqlCommand command = new MySqlCommand(queryYellenSpeaks, dbConn))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader.GetString(0).Contains("PM") || reader.GetString(0).Contains("AM"))
                        {

                            //if (BarsPeriod.Id == PeriodType.Minute) && (BarsPeriod.Value == 5))
                            //{
                            //Convert Time to EST. 
                            DateTime AddOnTime = DateTime.Now.AddHours(1);
                            DateTime SubtractTime = DateTime.Now.AddHours(-1);

                            //Print(String.Format("{0}      {1}     {2}", DateTime.Now, AddOnTime, SubtractTime));

                            DateTime sqlTime = DateTime.Parse(reader.GetString(0));
                            DateTime subtractTime = sqlTime.AddHours(-1);
                            DateTime modifiedTime =
                                (TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Parse(reader.GetString(0))) ==
                                 true)
                                    ? sqlTime
                                    : subtractTime;

                            //DateTime dstModifiedTime = (TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Parse(reader.GetString(0))) == true) ? DateTime.Parse(reader.GetString(0)).AddHours(1) : DateTime.Parse(reader.GetString(0)).AddHours(2);
                            //Print("SQL|EST " + sqlTime + "     " + modifiedTime + "     isDST     " + TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Parse(reader.GetString(0))));
                            //Print("");
                            //}

                            YellenSpeaksTime.Add(modifiedTime);
                        }
                    }
                }
            }

            using (MySqlCommand command = new MySqlCommand(queryEmploymentChange, dbConn))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader.GetString(0).Contains("PM") || reader.GetString(0).Contains("AM"))
                        {

                            //if (BarsPeriod.Id == PeriodType.Minute) && (BarsPeriod.Value == 5))
                            //{
                            //Convert Time to EST. 
                            DateTime AddOnTime = DateTime.Now.AddHours(1);
                            DateTime SubtractTime = DateTime.Now.AddHours(-1);

                            //Print(String.Format("{0}      {1}     {2}", DateTime.Now, AddOnTime, SubtractTime));

                            DateTime sqlTime = DateTime.Parse(reader.GetString(0));
                            DateTime subtractTime = sqlTime.AddHours(-1);
                            DateTime modifiedTime =
                                (TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Parse(reader.GetString(0))) ==
                                 true)
                                    ? sqlTime
                                    : subtractTime;

                            //DateTime dstModifiedTime = (TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Parse(reader.GetString(0))) == true) ? DateTime.Parse(reader.GetString(0)).AddHours(1) : DateTime.Parse(reader.GetString(0)).AddHours(2);
                            //Print("SQL|EST " + sqlTime + "     " + modifiedTime + "     isDST     " + TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Parse(reader.GetString(0))));
                            //Print("");
                            //}

                            nonFarmTime.Add(modifiedTime);
                        }
                    }
                }
            }
            dbConn.Close();

            #endregion


            #region Yellen test case 2


            foreach (DateTime time in YellenSpeaksTime)
            {

                YellenSpeaksTimeHour.Add(
                    DateTime.Parse(time.ToShortDateString() + " " + time.Hour + ":00:00 " + time.ToString("tt")));
                YellenSpeaksTimeDaily.Add(time.ToShortDateString());
                //YellenSpeaksTimeDaily.Add();
            }

            foreach (DateTime time in nonFarmTime)
            {

                nonFarmTimeHour.Add(
                    DateTime.Parse(time.ToShortDateString() + " " + time.Hour + ":00:00 " + time.ToString("tt")));
                nonFarmTimeDaily.Add(time.ToShortDateString());
                //YellenSpeaksTimeDaily.Add();
            }

            #endregion

        }

        protected override void OnBarUpdate()
        {
            
            if (Bars.BarsSinceSession == 0)
            {
                currentDateIndex = Time[0].ToShortDateString();
                //Print(currentDateIndex);

                try
                {
                    currentDataList = dictEvent[currentDateIndex];
                    
                    foreach (var data in currentDataList)
                    {
                        if (data.Currency == "USD" && (data.Impact == "High" || data.Impact == "Medium"))
                        {
                            //Print(string.Format("{0}  |  {1}  |  {2}  |  {3}  |  {4}  |  {5}  |   {6}  |   {7} ", data.Id, data.Timestamp, data.Currency, data.Impact, data.Event, data.Actual, data.Forecast, data.Previous));

                            if (!currentTimeEvent.ContainsKey(data.Timestamp)) //if it does not contain date
                            {
                                //Print(string.Format("{0}  |  {1}  |  {2}  |  {3}  |  {4}  |  {5}  |   {6}  |   {7} ", data.Id, data.Timestamp, data.Currency, data.Impact, data.Event, data.Actual, data.Forecast, data.Previous));

                                List<EconomicData> evenStats = new List<EconomicData>();
                                evenStats.Add(data);
                                currentTimeEvent.Add(data.Timestamp, evenStats);

                            }
                            else //if it contains the key, append the data
                            {

                                currentTimeEvent[data.Timestamp].Add(data);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Print("SKIP----------------------------------------------");
                }

                foreach (KeyValuePair<DateTime, List<EconomicData>> list in currentTimeEvent)
                {
                    //EconomicData getDate = new EconomicData();
                    DateTime getDate = list.Value[0].Timestamp;


                    //DateTime currentTime = 
                    string drawText = "";

                    //Print(list.Key);
                    foreach (EconomicData data in list.Value)
                    {

                        //Print(string.Format("{0}  |  {1}  |  {2}  |  {3}  |  {4}  |  {5}  |   {6}  |   {7} ", data.Id, data.Timestamp, data.Currency, data.Impact, data.Event, data.Actual, data.Forecast, data.Previous));

                        drawText = drawText + string.Format("{0}  {1} {2}  |  {3}  |  {4}  {5}", data.Event, Environment.NewLine, data.Actual, data.Forecast, data.Previous,Environment.NewLine);
                     
                    }
                    try
                    {
                        drawTextDictionary.Add(getDate, drawText);
                    }
                    catch (Exception ex)
                    {

                    }
                }

               foreach (KeyValuePair<DateTime,string> data in drawTextDictionary)
                {
                    //Print(data.Key);
                    //Print(data.Value);
                }

                listDate = drawTextDictionary.Keys.ToList();
            }


            //Print(Time[0]);

            /*
             * 
            if (currentTimeEvent.ContainsKey(Time[0]))
            {
                Print(Time[0]);
                foreach (EconomicData data in currentTimeEvent[Time[0]])
                {
                    Print(string.Format("{0}  |  {1}  |  {2}  |  {3}  |  {4}  |  {5}  |   {6}  |   {7} ", data.Id, data.Timestamp, data.Currency, data.Impact, data.Event, data.Actual, data.Forecast, data.Previous));
                }
            }
            */

            if (((BarsPeriod.Id == PeriodType.Minute) &&
                 ((BarsPeriod.Value == 1) || (BarsPeriod.Value == 5) || (BarsPeriod.Value == 10) ||
                  (BarsPeriod.Value == 15) || (BarsPeriod.Value == 30))))
            {
                if (listDate.Contains(Time[0]))
                {
                    Print(Time[0]);
                    Print(drawTextDictionary[Time[0]]);

                }

                //}
            }


            #region Yellen Drawing Object

                // Use this method for calculating your indicator values. Assign a value to each
                // plot below by replacing 'Close[0]' with your own formula.

                if (((BarsPeriod.Id == PeriodType.Minute) &&
                 ((BarsPeriod.Value == 1) || (BarsPeriod.Value == 5) || (BarsPeriod.Value == 10) ||
                  (BarsPeriod.Value == 15) || (BarsPeriod.Value == 30))))
            {
                if (YellenSpeaksTime.Contains(Time[0]))
                {

                    BarColor = Color.DarkRed;
                    YellenSpeaksTime.RemoveAt(YellenSpeaksTime.IndexOf(Time[0]));

                    DrawText("Yellen" + Time[0].Date, String.Format("Fed Chair Yellen Speaks"), 0, Low[0] - 2,
                        Color.Black);

                }
            }
            if (((BarsPeriod.Id == PeriodType.Minute) && ((BarsPeriod.Value == 60))))
            {
                if (YellenSpeaksTimeHour.Contains(Time[0]))
                {

                    BarColor = Color.DarkRed;
                    YellenSpeaksTimeHour.RemoveAt(YellenSpeaksTimeHour.IndexOf(Time[0]));

                    DrawText("Yellen" + Time[0].Date, String.Format("Fed Chair Yellen Speaks"), 0, Low[0] - 2,
                        Color.Black);

                }
            }
            if (((BarsPeriod.Id == PeriodType.Day) && ((BarsPeriod.Value == 1))))
            {
                Print(Time[0]);
                if (YellenSpeaksTimeDaily.Contains(Time[0].ToShortDateString()))
                {

                    BarColor = Color.DarkRed;
                    YellenSpeaksTimeDaily.RemoveAt(YellenSpeaksTimeDaily.IndexOf(Time[0].ToShortDateString()));

                    DrawText("Yellen" + Time[0].Date, String.Format("Fed Chair Yellen Speaks"), 0, Low[0] - 2,
                        Color.Black);

                }
            }


            #endregion



        }
        
        #region Properties

        #endregion
    }

    public class EconomicData
    {
        public UInt32 Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Currency { get; set; }
        public string Impact { get; set; }
        public string Event { get; set; }
        public string Actual { get; set; }
        public string Forecast { get; set; }
        public string Previous { get; set; }

    }


}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private kEconomicDataSet[] cachekEconomicDataSet = null;

        private static kEconomicDataSet checkkEconomicDataSet = new kEconomicDataSet();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public kEconomicDataSet kEconomicDataSet()
        {
            return kEconomicDataSet(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public kEconomicDataSet kEconomicDataSet(Data.IDataSeries input)
        {
            if (cachekEconomicDataSet != null)
                for (int idx = 0; idx < cachekEconomicDataSet.Length; idx++)
                    if (cachekEconomicDataSet[idx].EqualsInput(input))
                        return cachekEconomicDataSet[idx];

            lock (checkkEconomicDataSet)
            {
                if (cachekEconomicDataSet != null)
                    for (int idx = 0; idx < cachekEconomicDataSet.Length; idx++)
                        if (cachekEconomicDataSet[idx].EqualsInput(input))
                            return cachekEconomicDataSet[idx];

                kEconomicDataSet indicator = new kEconomicDataSet();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                kEconomicDataSet[] tmp = new kEconomicDataSet[cachekEconomicDataSet == null ? 1 : cachekEconomicDataSet.Length + 1];
                if (cachekEconomicDataSet != null)
                    cachekEconomicDataSet.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachekEconomicDataSet = tmp;
                return indicator;
            }
        }
    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.kEconomicDataSet kEconomicDataSet()
        {
            return _indicator.kEconomicDataSet(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.kEconomicDataSet kEconomicDataSet(Data.IDataSeries input)
        {
            return _indicator.kEconomicDataSet(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.kEconomicDataSet kEconomicDataSet()
        {
            return _indicator.kEconomicDataSet(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.kEconomicDataSet kEconomicDataSet(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.kEconomicDataSet(input);
        }
    }
}
#endregion
