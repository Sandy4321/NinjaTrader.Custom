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
    /// Simple MA Cross System
    /// </summary>
    [Description("Simple MA Cross System")]
    public class Tutorial1 : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int fast = 5; // Default setting for Fast
        private int slow = 20; // Default setting for Slow
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            Add(SMA(Fast));
            Add(SMA(Fast));

            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Condition set 1
            if (CrossAbove(SMA(Fast), SMA(Slow), 1))
            {
                EnterLong(DefaultQuantity, "");
            }

            // Condition set 2
            if (CrossBelow(SMA(Fast), SMA(Slow), 1))
            {
                EnterShort(DefaultQuantity, "");
            }
        }

        #region Properties
        [Description("Fast MA Period")]
        [GridCategory("Parameters")]
        public int Fast
        {
            get { return fast; }
            set { fast = Math.Max(1, value); }
        }

        [Description("Slow MA Period")]
        [GridCategory("Parameters")]
        public int Slow
        {
            get { return slow; }
            set { slow = Math.Max(1, value); }
        }
        #endregion
    }
}
