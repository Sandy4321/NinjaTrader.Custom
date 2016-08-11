// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

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
using NinjaTrader.Strategy;
#endregion


// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// This is a sample multi-time frame strategy.
    /// </summary>
    [Description("This is a sample multi-time frame strategy.")]
    public class SampleMultiTimeFrame : Strategy
    {
        #region Variables
        // Wizard generated variables
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            // Add a 5 minute Bars object to the strategy
			Add(PeriodType.Minute, 5);
			
			// Add a 15 minute Bars object to the strategy
			Add(PeriodType.Minute, 15);
			
			// Note: Bars are added to the BarsArray and can be accessed via an index value
			// E.G. BarsArray[1] ---> Accesses the 5 minute Bars object added above
			
			// Add simple moving averages to the chart for display
			// This only displays the SMA's for the primary Bars object on the chart
            Add(SMA(5));
            Add(SMA(50));
			
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// OnBarUpdate() will be called on incoming tick events on all Bars objects added to the strategy
			// We only want to process events on our primary Bars object (index = 0) which is set when adding
			// the strategy to a chart
			if (BarsInProgress != 0)
				return;
			
			// Checks  if the 5 period SMA is above the 50 period SMA on both the 5 and 15 minute time frames
			if (SMA(BarsArray[1], 5)[0] > SMA(BarsArray[1], 50)[0] && SMA(BarsArray[2], 5)[0] > SMA(BarsArray[2], 50)[0])
			{
				// Checks for a cross above condition of the 5 and 50 period SMA on the primary Bars object and enters long
				if (CrossAbove(SMA(5), SMA(50), 1))
					EnterLong(1000, "SMA");
			}
			
			// Checks for a cross below condition of the 5 and 15 period SMA on the 15 minute time frame and exits long
			if (CrossBelow(SMA(BarsArray[2], 5), SMA(BarsArray[2], 50), 1))
				ExitLong(1000);
        }

        #region Properties
        #endregion
    }
}
