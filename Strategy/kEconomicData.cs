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
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;
using System.IO;

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Enter the description of your strategy here
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class kEconomicData : Strategy
    {
        private MySqlConnection dbConn;
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

            string[] monthRange = { "jan"};//, "feb" };//,"mar","apr","may","jun","jul","aug","sep","oct","nov","dec"};
            string[] yearRange = { "2016" };

            string currentDate = DateTime.Now.ToString("MMM").ToLower() +DateTime.Now.Day.ToString() +"." +DateTime.Now.Year.ToString();
            string url;
            
            /*foreach(string year in yearRange)
            {
                //Print(month);
                foreach (string month in monthRange)
                {
                    url = "http://www.forexfactory.com/calendar.php?month=" + month + "." + year;
                    grabForexFactoryData(url);
                }
            }*/
             
            //url = "http://www.forexfactory.com/calendar.php?day=" + currentDate;
            url = "http://www.forexfactory.com/calendar.php?day=" + "may13.2016";
            //url = "http://www.forexfactory.com/calendar.php?month=" + "jan.201";
            Print(url);
            grabForexFactoryData(url);
 
        }



        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
        }
        protected override void OnTermination()
        {
            dbConn.Close();
        }

        private void grabForexFactoryData(string urlToday)  // Method to parse the data from forexfactory given an url 
        {
            string parseDateRef="";
            string parseTimeRef = "";
            List<string> listDate = new List<string>();
            List<string> listTime = new List<string>();
            List<string> listCurrency = new List<string>();
            List<string> listImpact = new List<string>();
            List<string> listEvent = new List<string>();
            List<string> listActual = new List<string>();
            List<string> listForcast = new List<string>();
            List<string> listPrevious = new List<string>();
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlToday);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();


            StreamReader websr = new StreamReader(response.GetResponseStream());
            string sourceCode = websr.ReadToEnd();

            int startIndex = sourceCode.IndexOf("calendar__row");
            int endIndex = sourceCode.LastIndexOf("calendar__row");
            sourceCode = sourceCode.Substring(startIndex, endIndex - startIndex);

            MatchCollection parseDate = Regex.Matches(sourceCode, @"calendar__cell calendar__date date"">\s*(.+?)</td>",RegexOptions.Singleline );
            MatchCollection parseTime = Regex.Matches(sourceCode, @"calendar__cell calendar__time time"">\s*(.+?)</td>", RegexOptions.Singleline);
            MatchCollection parseCurrency = Regex.Matches(sourceCode, @"calendar__cell calendar__currency currency"">s*(.+?)</td>", RegexOptions.Singleline);
            MatchCollection parseImpact = Regex.Matches(sourceCode, @"calendar__cell calendar__impact impact calendar__impact calendar__impact--\s*(.+?)</td>", RegexOptions.Singleline);
            MatchCollection parseEvent = Regex.Matches(sourceCode, @"td class=""calendar__cell calendar__event event""> <div> <span class=""calendar__event-title"">\s*(.+?)</span> </div> </td>", RegexOptions.Singleline);
            MatchCollection parseActual = Regex.Matches(sourceCode, @"td class=""calendar__cell calendar__actual actual"">\s*(.+?)</td>", RegexOptions.Singleline);
            MatchCollection parseForcast = Regex.Matches(sourceCode, @"td class=""calendar__cell calendar__forecast forecast"">\s*(.+?)</td>", RegexOptions.Singleline);
            MatchCollection parsePrevious= Regex.Matches(sourceCode, @"td class=""calendar__cell calendar__previous previous"">\s*(.+?)</td>", RegexOptions.Singleline);
            //MatchCollection parseNoEventDate = Regex.Matches(sourceCode, @"data-eventid=""\s*(.+?)""\s*(.+?) data-touchable> <td class=""calendar__cell calendar__date date"">", RegexOptions.Singleline);


            foreach (Match m in parseDate)
            {
                MatchCollection DateOnly = Regex.Matches(m.Groups[1].Value, @"<span>\s*(.+?)</span>", RegexOptions.Singleline);
                    
                foreach(Match extract in DateOnly)
                        parseDateRef = extract.Groups[1].Value + " " + urlToday.Substring(urlToday.Length - 4);
                
                listDate.Add(parseDateRef);
            }

            foreach (Match m in parseTime)
            {
                if (m.Groups[1].Value.Length < 50) // when there is additional tag compoent becase time is blank
                    parseTimeRef = m.Groups[1].Value;
                
                parseTimeRef = parseTimeRef.Replace("am", " AM");
                parseTimeRef = parseTimeRef.Replace("pm", " PM");

                listTime.Add(parseTimeRef);
            }

            foreach (Match m in parseCurrency)
                listCurrency.Add(m.Groups[1].Value);

            foreach (Match m in parseImpact)
            {
                MatchCollection ImpactOnly = Regex.Matches(m.Groups[1].Value, @"lass=""\s*(.+?)""></span>", RegexOptions.Singleline);
                foreach (Match extract in ImpactOnly)
                    listImpact.Add(extract.Groups[1].Value);
            }

            foreach (Match m in parseEvent)
                listEvent.Add(m.Groups[1].Value);


            foreach (Match m in parseActual)
            {
                if (m.Groups[1].Value.Length < 25)
                {
                    listActual.Add(m.Groups[1].Value);
                }
                else if (m.Groups[1].Value.Length < 50)
                {
                    MatchCollection ActualOnly = Regex.Matches(m.Groups[1].Value, @">\s*(.+?)</span>", RegexOptions.Singleline);
                    foreach (Match extract in ActualOnly)
                        listActual.Add(extract.Groups[1].Value);
                }
                else if (m.Groups[1].Value.Length == 61)
                {
                    listActual.Add("");
                }
            }

            foreach (Match m in parseForcast)
            {

                    if (m.Groups[1].Value.Length < 50) // when there is additional tag compoent becase forcast numbers is blank
                        listForcast.Add(m.Groups[1].Value);
                    else
                        listForcast.Add("");
            }

            foreach (Match m in parsePrevious)
            {
                if (m.Groups[1].Value.Length < 25)
                    listPrevious.Add(m.Groups[1].Value);
                else if (m.Groups[1].Value.Length == 55)
                   listPrevious.Add("");
                else if (m.Groups[1].Value.Length > 50) // when previous data is revised better, worse on ""
                {
                    MatchCollection ActualOnly = Regex.Matches(m.Groups[1].Value, @">\s*(.+?)</span>", RegexOptions.Singleline);
                    foreach (Match extract in ActualOnly)
                        listPrevious.Add(extract.Groups[1].Value);
                }
            }

            for (int i = 0; i <= listDate.Count-1 ; i++)
            {
               //Print(listForcast[i]);
                
                string insertEconData = String.Format("insert into kEconomicData values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');",
                    (listDate[i] + "." +listCurrency[i]+"."+ listTime[i] + "." + listEvent[i]), listDate[i], listTime[i], listCurrency[i], listImpact[i], listEvent[i], 
                    listActual[i], listForcast[i], listPrevious[i]);
                Print(insertEconData + Environment.NewLine);
              
              try
              {
                  MySqlCommand insertData = new MySqlCommand(insertEconData, dbConn);
                  insertData.ExecuteNonQuery();
              }
              catch (Exception ex)
              {
                  //Print(insertEconData.ToString());
                  //Print(ex.ToString());
              }
            }
        }

        #region Properties
        #endregion
    }
}
