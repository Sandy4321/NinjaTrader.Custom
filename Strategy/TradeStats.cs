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
    /// 
    /// </summary>
    [Description("")]
    public class TradeStats : Strategy
    {
        #region Variables
            private MySqlConnection dbConn;
            private List<string> listInsertTradeStatement = new List<string>();
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
            MySqlCommand deleteEodTrades = new MySqlCommand("delete from eodTrades", dbConn);
            MySqlCommand deletefromOffsetPNL = new MySqlCommand("delete from offsetPNL", dbConn);

            deleteEodTrades.ExecuteNonQuery();
            deleteEodTrades.ExecuteNonQuery();

            StreamReader sr = new StreamReader(@"C:\Users\Karunyan\Documents\Reports\nn.csv");
            string data = sr.ReadLine();
            while (data != null)
            {
                string[] dataArray = data.Split(',');
                if (dataArray[0] != "Trade-#")
                {
                    string insertTradeQuery = (String.Format("Insert into csvtrades values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}');",
                        Int32.Parse(dataArray[0]), (dataArray[1]), (dataArray[2]), (dataArray[3]), (dataArray[4]), Int32.Parse(dataArray[5]), Double.Parse(dataArray[6]), Double.Parse(dataArray[7]), 
                        DateTime.Parse(dataArray[8]), DateTime.Parse(dataArray[9]), (dataArray[10]), (dataArray[11]), Double.Parse(dataArray[12]), Double.Parse(dataArray[13]), Double.Parse(dataArray[14]), 
                        Double.Parse(dataArray[15]), Double.Parse(dataArray[16]), Double.Parse(dataArray[17]), Int32.Parse(dataArray[18])));

                    listInsertTradeStatement.Add(insertTradeQuery);
                }
                data = sr.ReadLine();

            }

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
