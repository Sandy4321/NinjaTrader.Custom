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
using MySql.Data.MySqlClient;

#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Sample connect to connect to MySQL and run Queries
    /// </summary>
    [Description("Sample connect to connect to MySQL and run Queries")]
    public class MySQL : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int myInput0 = 1; // Default setting for MyInput0
        // User defined variables (add any user defined variables below)

        private MySqlConnection dbConn;
 

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
            dbConn = new MySqlConnection("server=localhost;database=dbtestprototype;uid=ninjatrader;password=Password1;");
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

            base.OnStartUp();
        }
        protected override void OnTermination()
        {
            try
            {
                dbConn.Close();
                Print(" Database connection closed");
            }
            catch (MySqlException)
            {
                Print("Exception during close");
            }
            


            base.OnTermination();

        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            //if (CurrentBar < 0) return;

            string queryInsertMarketData = ("Insert into marketdata values ('" + 
                Time[0] + "'," + "'" +
                Instrument.FullName+"'," +
                Open[0] + "," +
                High[0] + "," +
                Low[0] + "," +
                Close[0]+");");
            Print(queryInsertMarketData);


            MySqlCommand queryCmdInsert = new MySqlCommand(queryInsertMarketData, dbConn);
            queryCmdInsert.BeginExecuteNonQuery();


        }


        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int MyInput0
        {
            get { return myInput0; }
            set { myInput0 = Math.Max(1, value); }
        }
        #endregion
    }


}
