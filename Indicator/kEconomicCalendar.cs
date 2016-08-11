
using System.Linq;

#region Using declarationsusing System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using System;
#endregion
using MySql.Data.MySqlClient;
using System.Collections.Generic;


// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Enter the description of your new custom indicator here
    /// </summary>
    [Description("Enter the description of your new custom indicator here")]
    public class kEconomicCalendar : Indicator
    {
        #region Variables
        private MySqlConnection dbConn = new MySqlConnection("server = localhost; database=algo;uid=root;password=Password1;");
        List<string[]> listEventData = new List<string[]>(); 
        //private Dictionary<string,>dictEvents = new 
        
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Overlay				= true;

        }

        protected override void OnStartUp()
        {
            dbConn.Open();
            DateTime startDate = new DateTime(2016,02,14);
            DateTime endDate = new DateTime(2016,02,17);

            string query  = string.Format("select* from economiceventdata where dateNum between '{0}' and '{1}' order by " +
                                          "concat(concat(dateNum, ' '), timeEst) desc",startDate,endDate);
            
            using (MySqlCommand command = new MySqlCommand(query, dbConn))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        string[] record = new string[8];
                        record[0] = reader.GetString(0);
                        record[1] = reader.GetString(1);
                        record[2] = reader.GetString(2);
                        record[3] = reader.GetString(3);
                        record[4] = reader.GetString(4);
                        record[5] = reader.GetString(5);
                        record[6] = reader.GetString(6);
                        record[7] = reader.GetString(7);

                        listEventData.Add(record);

                    }
                }
            }

            foreach (string[] s in listEventData)
            {
                if (s[2].Contains("AM") || s[2].Contains("PM")) // I need to modify this so that It will show holiday for future event. 
                {
                    if (s[3].Contains("USD"))
                        if (s[4].Contains("High") || s[4].Contains("Holiday"))
                        Print(string.Format("{0}  {1}  {2}  {3}  {4}  {5}", DateTime.Parse(s[1]+ " " + s[2]).ToString("yyyy-MM-dd HH:mm"),s[3],s[4],s[5],s[6],s[7]));

                }
                if (s[4].Contains("Holiday") && (s[3].Contains("USD") || s[3].Contains("EUR")))
                {
                    Print(string.Format("{0}  {1}  {2}  {3}  {4}  {5}", "                                 ", s[3], s[4], s[5], s[6], s[7]));
                }
            }


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
        private kEconomicCalendar[] cachekEconomicCalendar = null;

        private static kEconomicCalendar checkkEconomicCalendar = new kEconomicCalendar();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public kEconomicCalendar kEconomicCalendar()
        {
            return kEconomicCalendar(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public kEconomicCalendar kEconomicCalendar(Data.IDataSeries input)
        {
            if (cachekEconomicCalendar != null)
                for (int idx = 0; idx < cachekEconomicCalendar.Length; idx++)
                    if (cachekEconomicCalendar[idx].EqualsInput(input))
                        return cachekEconomicCalendar[idx];

            lock (checkkEconomicCalendar)
            {
                if (cachekEconomicCalendar != null)
                    for (int idx = 0; idx < cachekEconomicCalendar.Length; idx++)
                        if (cachekEconomicCalendar[idx].EqualsInput(input))
                            return cachekEconomicCalendar[idx];

                kEconomicCalendar indicator = new kEconomicCalendar();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                kEconomicCalendar[] tmp = new kEconomicCalendar[cachekEconomicCalendar == null ? 1 : cachekEconomicCalendar.Length + 1];
                if (cachekEconomicCalendar != null)
                    cachekEconomicCalendar.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachekEconomicCalendar = tmp;
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
        public Indicator.kEconomicCalendar kEconomicCalendar()
        {
            return _indicator.kEconomicCalendar(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.kEconomicCalendar kEconomicCalendar(Data.IDataSeries input)
        {
            return _indicator.kEconomicCalendar(input);
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
        public Indicator.kEconomicCalendar kEconomicCalendar()
        {
            return _indicator.kEconomicCalendar(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.kEconomicCalendar kEconomicCalendar(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.kEconomicCalendar(input);
        }
    }
}
#endregion
