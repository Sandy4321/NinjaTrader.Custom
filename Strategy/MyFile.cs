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
using System.IO;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Write Infomration to File Sample
    /// </summary>
    [Description("Write Infomration to File Sample")]
    public class MyFile : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int myInput0 = 1; // Default setting for MyInput0
        // User defined variables (add any user defined variables below)
        private string path = "C:\\log\\MyFile.log";
        

        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            string kObj = "BullGapBar.Initalize";
            CalculateOnBarClose = true;

            kLog(kObj, "FATAL",
            String.Format(" The current close is {0}", Close[0]));

        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            string kObj = "BullGapBar.OnBarUpdate";

            kLog(kObj,"INFO", 
                String.Format(" The current close is {0}", Close[0]));

        }

        protected override void OnTermination()
        {
            
            base.OnTermination();
            File.WriteAllText(path, string.Empty);
        }
        private void kLog(string ClassMethod, string MsgType, string Msg) // encapsulate print statement
        {
           string _msgOut = (String.Format("{0}  {1}    {2}    [{3}]    {4}",
               Time[0].ToString("dd/MM/yyyy"), 
               Time[0].ToString("HH:mm:ss"),
               MsgType,
               ClassMethod,
               Msg)) + Environment.NewLine;
           File.AppendAllText(path, _msgOut);

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
