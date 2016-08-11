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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;


// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Enter the description of your new custom indicator here
    /// </summary>
    [Description("Enter the description of your new custom indicator here")]
    public class kEconData : Indicator
    {
        #region Variables
            private List<string[]> EconDataList = new List<string[]>();
            private EconData CurrentEconData;
            private TimeSpan amPMConverter = new TimeSpan(12, 00, 00);

            private SolidBrush textBrush = new SolidBrush(Color.DimGray);
            private Font textFont = new Font("Arial", 30);
            
            private StringFormat stringFormat = new StringFormat();
            private float textWidth = 0;
            private float textHeight = 0;
            private float noConTextWidth = 0;
            private float noConTextHeight = 0;
            private string todayDataText = "";
            private string indicatorEconDataList = "";
            private string[] dataArray;
            private DateTime tableLoopDate;
            private DateTime tableLoopTimestamp;
            private string tableLoopTime;
            private string tableLoopCurrency;
            private string tableLoopImpact;
            private string tableLoopEvent;
            private string tableLoopForcast;
            private string tableLoopPrevious;
            private string tableLoopActual;
            string[] singleLineDataSet;
            string previousTime = "";

            private string scarpEconDataLine = "";
        private int dataSetCounter = 0;


        #endregion

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
        protected override void OnTermination()
        {
            textBrush.Dispose();
            textFont.Dispose();
            stringFormat.Dispose();
        }
        protected override void OnStartUp()
        {
            //// I am going to house the webscraping code in this section
            {

                string currentDate = DateTime.Now.ToString("MMM").ToLower() +DateTime.Now.Day.ToString() +"." +DateTime.Now.Year.ToString();
                //Print("Current economic data list query date: " + currentDate);

                //string date = "may18";
                string year = DateTime.Now.Year.ToString();
                string previousTime = "";
                //string url = "http://www.forexfactory.com/calendar.php?day=" + date + "." + year;
                //string url = "http://www.forexfactory.com/calendar.php?day=" + currentDate;
                string url = "http://www.forexfactory.com/calendar.php?day=" + "may13.2016";
                //string url = "http://www.forexfactory.com/calendar.php?month=this";
                Print("Scarping the following url: " + url);


                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader websr = new StreamReader(response.GetResponseStream());
                string sourceCode = websr.ReadToEnd();
                
                int startIndex = sourceCode.IndexOf("calendar__row");
                int endIndex = sourceCode.LastIndexOf("calendar__row");
                sourceCode = sourceCode.Substring(startIndex, endIndex - startIndex);

                MatchCollection m1 = Regex.Matches(sourceCode, @"calendar__cell calendar__\s*(.+?)</td>", RegexOptions.Singleline);
                websr.Close();
                foreach (Match m in m1)
                {
                    string matchData = m.Groups[1].Value;

                    StringBuilder parseHtmlTags = new StringBuilder(matchData);
                    string newSB =
                        parseHtmlTags
                            .Replace("span", "")
                            .Replace("a class=", "")
                            .Replace("class", "")
                            .Replace("<", "")
                            .Replace(">", "")
                            .Replace("/", "")
                            .Replace(" div ", "")
                            .Replace(" =\"calendar__event-title\"", "")
                            .Replace("=\"date\"", "")
                            .Replace("\"a", "")
                            .Replace("calendar__impact calendar__impact--medium\"  title=\"Medium Impact Expected\" =", "")
                            .Replace("calendar__impact calendar__impact--low\"  title=\"Low Impact Expected\" =", "")
                            .Replace("calendar__impact calendar__impact--high\"  title=\"High Impact Expected\" =", "")
                            .Replace("=\"worse\"", "")
                            .Replace("=\"better\"", "")
                            .Replace("am", ":00 AM")
                            .Replace("pm", ":00 PM")
                            .Replace ("All Day","00:01 AM")
                            .Replace ("Tentative","00:01 AM")
                            .Replace ("  title=","holiday")
                            .Replace ("Crude Oil Inventories","")
                            .ToString();

                        string[] dataArray = parseHtmlTags.ToString().Split('"');

                        if (parseHtmlTags.ToString().Contains("date") && parseHtmlTags.ToString().Length > 10) //case when date is not blank
                            tableLoopDate = DateTime.Parse(dataArray[1].ToString().Remove(0, 4) + " " + year);

                        if (parseHtmlTags.ToString().Contains("time"))
                        {
                            if (parseHtmlTags.ToString().Contains(":") || parseHtmlTags.ToString()== "")
                            {
                                if (parseHtmlTags.ToString().Length > 10)
                                    tableLoopTime = dataArray[1].ToString();

                                if (parseHtmlTags.ToString().Length > 20)
                                    tableLoopTime = "00:01 AM";
                            }

                            scarpEconDataLine = tableLoopDate.ToShortDateString() + " " + tableLoopTime.ToString();
                            tableLoopTimestamp = DateTime.Parse(tableLoopDate.ToShortDateString() + " " + tableLoopTime.ToString());     
                        }

                        if (parseHtmlTags.ToString().Contains("currency"))
                            tableLoopCurrency = dataArray[1].ToString();
                        
                        if (parseHtmlTags.ToString().Contains("impact"))
                            tableLoopImpact = dataArray[1].ToString();
                        
                        if (parseHtmlTags.ToString().Contains("event"))
                            tableLoopEvent = dataArray[1].ToString();
                        
                        if (parseHtmlTags.ToString().Contains("actual"))
                            tableLoopActual = dataArray[1].ToString();

                        if (parseHtmlTags.ToString().Contains("forecast"))
                            tableLoopForcast = dataArray[1].ToString();

                        if (parseHtmlTags.ToString().Contains("previous"))
                        {
                            // This is the end of the data set
                            if (parseHtmlTags.ToString().Contains("revised"))
                                tableLoopPrevious = dataArray[5].ToString();
                            else
                                tableLoopPrevious = dataArray[1].ToString();

                            singleLineDataSet = new string[] { tableLoopTimestamp.ToString(), tableLoopCurrency, tableLoopImpact, tableLoopEvent, tableLoopActual, tableLoopForcast, tableLoopPrevious };
                            EconDataList.Add(singleLineDataSet);
                        }

                }
                Print("");
                foreach (string[] element in EconDataList)
                {
                    if (element[2] == "high" || element[2] == "holiday")
                    {
                        if (element[1] == "USD")
                        {
                            if (previousTime == (DateTime.Parse(element[0].ToString()).ToString("HH:mm")) || DateTime.Parse(element[0].ToString()).ToString("HH:mm") == "00:01")
                            {
                                Print(String.Format("{0}   {1}", "         ", element[3].ToString()));
                                indicatorEconDataList += (String.Format("{0}   {1} {2}", "         ", element[3].ToString(), System.Environment.NewLine));
                            }
                            else
                            {
                                Print(String.Format("{0}   {1}", DateTime.Parse(element[0].ToString()).ToString("HH:mm"), element[3].ToString()));
                                indicatorEconDataList += (String.Format("{0}   {1} {2}", DateTime.Parse(element[0].ToString()).ToString("HH:mm"), element[3].ToString(), System.Environment.NewLine));
                            }
                            previousTime = DateTime.Parse(element[0].ToString()).ToString("HH:mm");

                        }
                    }
                }
            }
        }
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
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
    public class EconData
    {
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public string Impact { get; set; }
        public string Actual { get; set; }
        public string Description { get; set; }
        public string Forcast { get; set; }
        public string Previous { get; set; }
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private kEconData[] cachekEconData = null;

        private static kEconData checkkEconData = new kEconData();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public kEconData kEconData()
        {
            return kEconData(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public kEconData kEconData(Data.IDataSeries input)
        {
            if (cachekEconData != null)
                for (int idx = 0; idx < cachekEconData.Length; idx++)
                    if (cachekEconData[idx].EqualsInput(input))
                        return cachekEconData[idx];

            lock (checkkEconData)
            {
                if (cachekEconData != null)
                    for (int idx = 0; idx < cachekEconData.Length; idx++)
                        if (cachekEconData[idx].EqualsInput(input))
                            return cachekEconData[idx];

                kEconData indicator = new kEconData();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                kEconData[] tmp = new kEconData[cachekEconData == null ? 1 : cachekEconData.Length + 1];
                if (cachekEconData != null)
                    cachekEconData.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachekEconData = tmp;
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
        public Indicator.kEconData kEconData()
        {
            return _indicator.kEconData(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.kEconData kEconData(Data.IDataSeries input)
        {
            return _indicator.kEconData(input);
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
        public Indicator.kEconData kEconData()
        {
            return _indicator.kEconData(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.kEconData kEconData(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.kEconData(input);
        }
    }
}
#endregion
