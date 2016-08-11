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

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// This is not a strategy. It is a means to test out random code samples
    /// </summary>
    [Description("This is not a strategy. It is a means to test out random code samples")]
    public class ResearchCode : Strategy
    {
        
        #region Variables
        // Wizard generated variables
        private int myInput0 = 1; // Default setting for MyInput0
        // User defined variables (add any user defined variables below)

        private int decimalPlaces = 0;

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
            decimal increment = Convert.ToDecimal(Instrument.MasterInstrument.TickSize);
            Print(Instrument.MasterInstrument.TickSize);
            Print("increment    " + increment);
            int incrementLength = increment.ToString().Length;
            Print(incrementLength);
            decimalPlaces = 0;
            if (incrementLength == 1)
            {
                decimalPlaces = 0;
            }
            else if (incrementLength > 2)
            {
                decimalPlaces = incrementLength - 1;
            }
            Print(decimalPlaces);

        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
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
