#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Enter the description of your new custom indicator here
    /// </summary>
    [Description("Enter the description of your new custom indicator here")]
    public class kStatsAndAlerts : Indicator
    {
        private SolidBrush textBrush = new SolidBrush(Color.DimGray);
        private Font textFont = new Font("Arial", 30);

        private StringFormat stringFormat = new StringFormat();
        private float textWidth = 0;
        private float textHeight = 0;
        private float noConTextWidth = 0;
        private float noConTextHeight = 0;
        private string indicatorEconDataList = "";
        private string insertEconData;
        private bool isFirstEntry = true;

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
        }
        protected override void OnStartUp()
        {
            string currentDate = DateTime.Now.ToString("MMM").ToLower() + DateTime.Now.Day.ToString() + "." + DateTime.Now.Year.ToString();
            string tomorrowDate = DateTime.Now.AddDays(1).ToString("MMM").ToLower() + DateTime.Now.AddDays(1).Day.ToString() + "."+DateTime.Now.AddDays(1).Year.ToString();

            string urlToday = "http://www.forexfactory.com/calendar.php?day=" + currentDate;
            string urlTomorrow = "http://www.forexfactory.com/calendar.php?day=" + tomorrowDate;
            //string urlToday = "http://www.forexfactory.com/calendar.php?day=" + "may17.2016";
            //string urlTomorrow = "http://www.forexfactory.com/calendar.php?day=" + "may18.2016";

            Print("A - Today (" + urlToday + ") ");
            string todayEvent = grabForexFactoryData(urlToday);

            Print("");
            Print("B - Tomorrow (" + urlTomorrow + ") ");
            string tomorrowEvent = grabForexFactoryData(urlTomorrow);

             int maxStringLength=0;
             
             using (StringReader getMaxLinePerLine = new StringReader(todayEvent + Environment.NewLine + tomorrowEvent))
             {
                 string line = "";
                 while (line != null)
                 {
                     line = getMaxLinePerLine.ReadLine();
                     if (line != null)
                     {
                         maxStringLength = Math.Max(maxStringLength, line.Length);
                     }
                 }
                 getMaxLinePerLine.Close();
             }
            

            //int maxStringLength = Math.Max(todayEvent.Length, tomorrowEvent.Length);
            string nextDaySpacer = "";
            if (tomorrowEvent.Length > 0)
            {
                for (int i = 0; i < maxStringLength-6; i++)
                {
                    nextDaySpacer += "..";
                }
                nextDaySpacer += " " + DateTime.Parse(tomorrowDate).ToString("MM") + "/" + DateTime.Parse(tomorrowDate).ToString("dd") + Environment.NewLine;
            }

            indicatorEconDataList = todayEvent + nextDaySpacer + tomorrowEvent;



        }
        private string grabForexFactoryData(string url)  // Method to parse the data from forexfactory given an url 
        {

            
            insertEconData = "";
            isFirstEntry = true;
            string parseDateRef = "";
            string parseTimeRef = "";
            
            List<string> listDate = new List<string>();
            List<string> listTime = new List<string>();
            List<string> listCurrency = new List<string>();
            List<string> listImpact = new List<string>();
            List<string> listEvent = new List<string>();
            List<string> listActual = new List<string>();
            
            List<string> listForcast = new List<string>();
            List<string> listPrevious = new List<string>();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();


            StreamReader websr = new StreamReader(response.GetResponseStream());
            string sourceCode = websr.ReadToEnd();

            int startIndex = sourceCode.IndexOf("calendar__row");
            int endIndex = sourceCode.LastIndexOf("calendar__row");
            sourceCode = sourceCode.Substring(startIndex, endIndex - startIndex);

            MatchCollection parseDate = Regex.Matches(sourceCode, @"calendar__cell calendar__date date"">\s*(.+?)</td>", RegexOptions.Singleline);
            MatchCollection parseTime = Regex.Matches(sourceCode, @"calendar__cell calendar__time time"">\s*(.+?)</td>", RegexOptions.Singleline);
            MatchCollection parseCurrency = Regex.Matches(sourceCode, @"calendar__cell calendar__currency currency"">s*(.+?)</td>", RegexOptions.Singleline);
            MatchCollection parseImpact = Regex.Matches(sourceCode, @"calendar__cell calendar__impact impact calendar__impact calendar__impact--\s*(.+?)</td>", RegexOptions.Singleline);
            MatchCollection parseEvent = Regex.Matches(sourceCode, @"td class=""calendar__cell calendar__event event""> <div> <span class=""calendar__event-title"">\s*(.+?)</span> </div> </td>", RegexOptions.Singleline);
            MatchCollection parseActual = Regex.Matches(sourceCode, @"td class=""calendar__cell calendar__actual actual"">\s*(.+?)</td>", RegexOptions.Singleline);
            MatchCollection parseForcast = Regex.Matches(sourceCode, @"td class=""calendar__cell calendar__forecast forecast"">\s*(.+?)</td>", RegexOptions.Singleline);
            MatchCollection parsePrevious = Regex.Matches(sourceCode, @"td class=""calendar__cell calendar__previous previous"">\s*(.+?)</td>", RegexOptions.Singleline);
            //MatchCollection parseNoEventDate = Regex.Matches(sourceCode, @"data-eventid=""\s*(.+?)""\s*(.+?) data-touchable> <td class=""calendar__cell calendar__date date"">", RegexOptions.Singleline);

            Print("Date");
            foreach (Match m in parseDate)
            {
                MatchCollection DateOnly = Regex.Matches(m.Groups[1].Value, @"<span>\s*(.+?)</span>", RegexOptions.Singleline);

                foreach (Match extract in DateOnly)
                    parseDateRef = extract.Groups[1].Value + " " + url.Substring(url.Length - 4);

                listDate.Add(parseDateRef);
            }

            Print("Time");
            foreach (Match m in parseTime)
            {
                if (m.Groups[1].Value.Length < 50) // when there is additional tag compoent becase time is blank
                    parseTimeRef = m.Groups[1].Value;

                parseTimeRef = parseTimeRef.Replace("am", " AM");
                parseTimeRef = parseTimeRef.Replace("pm", " PM");

                listTime.Add(parseTimeRef);
            }
            Print("Currency");
            foreach (Match m in parseCurrency)
                listCurrency.Add(m.Groups[1].Value);
            
            Print("Impact");
            foreach (Match m in parseImpact)
            {
                MatchCollection ImpactOnly = Regex.Matches(m.Groups[1].Value, @"lass=""\s*(.+?)""></span>", RegexOptions.Singleline);
                foreach (Match extract in ImpactOnly)
                    listImpact.Add(extract.Groups[1].Value);
            }

            Print("Event");
            foreach (Match m in parseEvent)
                listEvent.Add(m.Groups[1].Value);

            Print("Actual");
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

            Print("Forcast");
            foreach (Match m in parseForcast)
            {

                if (m.Groups[1].Value.Length < 50) // when there is additional tag compoent becase forcast numbers is blank
                    listForcast.Add(m.Groups[1].Value);
                else
                    listForcast.Add("");
            }

            Print("Previous");
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

            Print("String Statements");
            for (int i = 0; i <= listDate.Count - 1; i++)
            {
                if (listCurrency[i] == "USD" && (listImpact[i] == "high" || listImpact[i] == "holiday") && !listEvent[i].Contains("Crude Oil"))
                {
                    DateTime convertTextToTime;
                    if(listTime[i].Contains("AM") || listTime[i].Contains("PM"))
                    {
                        convertTextToTime = DateTime.Parse(listTime[i]);
                        listTime[i] = convertTextToTime.ToString("HH:mm");
                    }

                    if (isFirstEntry)
                    {
                        insertEconData += String.Format("{1}    {4}{8}",
                        listDate[i], listTime[i], listCurrency[i], listImpact[i], listEvent[i], listActual[i], listForcast[i], listPrevious[i], Environment.NewLine);
                        isFirstEntry = false;
                    }
                    else
                    {
                        if (listTime[i] == listTime[i - 1])
                        {
                            insertEconData += String.Format("{1}    {4}{8}",
                        listDate[i], "         ", listCurrency[i], listImpact[i], listEvent[i], listActual[i], listForcast[i], listPrevious[i], Environment.NewLine);
                        }
                        else
                        {
                            insertEconData += String.Format("{1}    {4}{8}",
                                listDate[i], listTime[i], listCurrency[i], listImpact[i], listEvent[i], listActual[i], listForcast[i], listPrevious[i], Environment.NewLine);
                        }
                    }

                    //Print(insertEconData);
                }
            }
            return insertEconData;

        }
        public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
        {
            textBrush.Color = ChartControl.AxisColor;
            textFont = (Font)ChartControl.Font.Clone();
            SizeF noConSize = graphics.MeasureString(indicatorEconDataList, textFont);
            noConTextWidth = noConSize.Width + 10;
            noConTextHeight = noConSize.Height + 5;
            graphics.DrawString(indicatorEconDataList, ChartControl.Font, textBrush, bounds.X + bounds.Width - noConTextWidth, bounds.Y + bounds.Height - noConTextHeight, stringFormat);
        }

        #region Properties

        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private kStatsAndAlerts[] cachekStatsAndAlerts = null;

        private static kStatsAndAlerts checkkStatsAndAlerts = new kStatsAndAlerts();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public kStatsAndAlerts kStatsAndAlerts()
        {
            return kStatsAndAlerts(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public kStatsAndAlerts kStatsAndAlerts(Data.IDataSeries input)
        {
            if (cachekStatsAndAlerts != null)
                for (int idx = 0; idx < cachekStatsAndAlerts.Length; idx++)
                    if (cachekStatsAndAlerts[idx].EqualsInput(input))
                        return cachekStatsAndAlerts[idx];

            lock (checkkStatsAndAlerts)
            {
                if (cachekStatsAndAlerts != null)
                    for (int idx = 0; idx < cachekStatsAndAlerts.Length; idx++)
                        if (cachekStatsAndAlerts[idx].EqualsInput(input))
                            return cachekStatsAndAlerts[idx];

                kStatsAndAlerts indicator = new kStatsAndAlerts();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                kStatsAndAlerts[] tmp = new kStatsAndAlerts[cachekStatsAndAlerts == null ? 1 : cachekStatsAndAlerts.Length + 1];
                if (cachekStatsAndAlerts != null)
                    cachekStatsAndAlerts.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachekStatsAndAlerts = tmp;
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
        public Indicator.kStatsAndAlerts kStatsAndAlerts()
        {
            return _indicator.kStatsAndAlerts(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.kStatsAndAlerts kStatsAndAlerts(Data.IDataSeries input)
        {
            return _indicator.kStatsAndAlerts(input);
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
        public Indicator.kStatsAndAlerts kStatsAndAlerts()
        {
            return _indicator.kStatsAndAlerts(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.kStatsAndAlerts kStatsAndAlerts(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.kStatsAndAlerts(input);
        }
    }
}
#endregion
