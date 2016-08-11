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
    /// Multi-Instrument sample strategy.
    /// </summary>
    [Description("Multi-Instrument sample strategy.")]
    public class SampleMultiInstrument : Strategy
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
			// Add an MSFT 1 minute Bars object to the strategy
			Add("MSFT", PeriodType.Minute, 1);
			
			// Note: Bars are added to the BarsArray and can be accessed via an index value
			// E.G. BarsArray[1] ---> Accesses the 1 minute Bars added above
			
			// Add RSI and ADX indicators to the chart for display
			// This only displays the indicators for the pimary Bars object (main instrument) on the chart
			Add(RSI(14, 0));
            Add(ADX(14));
			
			// Sets a 20 tick trailing stop for an open position
			SetTrailStop(CalculationMode.Ticks, 20);
			
			CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// OnBarUpdate() will be called on incoming tick events on all Bars objects added to the strategy
			// We only want to process events on our primary Bars object (main instrument) (index = 0) which 
			// is set when adding the strategy to a chart
			if (BarsInProgress != 0)
				return;
			
			// Checks if the 14 period ADX on both instruments are trending (above a value of 30)
			if (ADX(14)[0] > 30 && ADX(BarsArray[1], 14)[0] > 30)
			{
				// If RSI crosses above a value of 30 then enter a long position via a limit order
				if (CrossAbove(RSI(14, 0), 30, 1))
				{
					// Draws a square 1 tick above the high of the bar identifying when a limit order is issued
					DrawSquare("My square" + CurrentBar, false, 0, High[0] + 1 * TickSize, Color.DodgerBlue);
					
					// Enter a long position via a limit order at the current ask price
					EnterLongLimit(GetCurrentAsk(), "RSI");
				}
			}
			
			// Any open long position will exit if RSI crosses below a value of 75
			// This is in addition to the trail stop set in the Initialize() method
			if (CrossBelow(RSI(14, 0), 75, 1))
				ExitLong();				
        }

        #region Properties
        #endregion
    }
}
