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
using MySql.Data.MySqlClient;

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Enter the description of your strategy here
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class ibCandleEOD : Strategy
    {
        #region Variables
        // Wizard generated variables
        // User defined variables (add any user defined variables below)
        #endregion
        int dayIndex = 1;
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
            try
            {
                dbConn.Open();
                Print("Database connection established successfully...");
            }
            catch (MySqlException ex)
            {
                Print(ex.ToString());
            }

            //MySqlCommand deleteRangeData = new MySqlCommand("delete from ibAnalysis", dbConn);
            //deleteRangeData.ExecuteNonQuery();
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            string insertQuery = (String.Format("Insert into ibAnalysis values ('{0}','{1}','{2}','{3}','{4}')", dayIndex, Open[0], High[0], Low[0], Close[0]));
            //Print(insertQuery);
            MySqlCommand insertRangeData = new MySqlCommand(insertQuery, dbConn);
            insertRangeData.ExecuteNonQuery();
            dayIndex = dayIndex + 2;
        }

        #region Properties
        #endregion
    }
}
