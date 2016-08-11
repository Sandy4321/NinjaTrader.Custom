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
    /// Enter the description of your strategy here
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class easyMethod : Strategy
    {
        private int swingQty = 1;
        private double atrFactor = 2.0;
        private IOrder swingShorEntry = null;
        private IOrder swingShortStopLoss = null;
        private IOrder swingShortProfitTarget = null;

        private bool isLongEnabled = true;
        private bool isShortEnabled = true;



        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = true;
            ExitOnClose = false;
            EntriesPerDirection = 3;
            Enabled = true;
            Unmanaged = false;  //Remember to change this setting when you mastered this code

        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            if (BarsInProgress == 0)
            {
                if (anaTradersDynamicIndex(34, 2, 13, 7, 1.62).PriceLine[0] > 50 &&
                    anaTradersDynamicIndex(34, 2, 13, 7, 1.62).PriceLine[0] > anaTradersDynamicIndex(34, 2, 13, 7, 1.62).SignalLine[0] &&
                    anaTradersDynamicIndex(34, 2, 13, 7, 1.62).PriceLine[0] > anaTradersDynamicIndex(34, 2, 13, 7, 1.62).Average[0] &&
                    (HeikenAshi().HAClose[0] > SMA(HeikenAshi().HAHigh, 5)[0]) &&
                    (SMA(HeikenAshi().HAHigh, 5)[0] > SMA(HeikenAshi().HAHigh, 5)[1]) && (SMA(HeikenAshi().HAHigh, 5)[1] > SMA(HeikenAshi().HAHigh, 5)[2]) &&   //You need to also validate a healty slope
                    isLongEnabled == true)
                //Need to add logic to get price channel High/Low
                //Use smothe moving average (Top of Channel = 5 period High; Bottom of Channel = 5 period Low)
                //Need to add second entry on Bollinger Break
                {
                    //BackColor = Color.LightSteelBlue;
                    EnterLong();
                    isLongEnabled = false;
                    isShortEnabled = true;

                }
                else if (anaTradersDynamicIndex(34, 2, 13, 7, 1.62).PriceLine[0] < 50 &&
                    anaTradersDynamicIndex(34, 2, 13, 7, 1.62).PriceLine[0] < anaTradersDynamicIndex(34, 2, 13, 7, 1.62).SignalLine[0] &&
                    anaTradersDynamicIndex(34, 2, 13, 7, 1.62).PriceLine[0] > anaTradersDynamicIndex(34, 2, 13, 7, 1.62).Average[0] &&
                    (HeikenAshi().HAClose[0] < SMA(HeikenAshi().HALow, 5)[0]) &&
                    (SMA(HeikenAshi().HAHigh, 5)[0] < SMA(HeikenAshi().HAHigh, 5)[1]) && (SMA(HeikenAshi().HAHigh, 5)[1] < SMA(HeikenAshi().HAHigh, 5)[2]) &&
                    isShortEnabled == true)
                //Need to add logic to get price channel High/Low
                //Need to add second entry on Bollinger Break
                {
                    //BackColor = Color.LightSalmon;
                    EnterShort();
                    isLongEnabled = true;
                    isShortEnabled = false;
                }


                if ((anaTradersDynamicIndex(34, 2, 13, 7, 1.62).PriceLine[0] > 68 || anaTradersDynamicIndex(34, 2, 13, 7, 1.62).PriceLine[0] < 32) &&
                       Position.MarketPosition != MarketPosition.Flat)
                {
                    if (anaTradersDynamicIndex(34, 2, 13, 7, 1.62).PriceLine[0] > anaTradersDynamicIndex(34, 2, 13, 7, 1.62).SignalLine[0])
                        ExitShort();
                    if (anaTradersDynamicIndex(34, 2, 13, 7, 1.62).PriceLine[0] < anaTradersDynamicIndex(34, 2, 13, 7, 1.62).SignalLine[0])
                        ExitLong();
                }

                if (HeikenAshi().HAClose[0] - HeikenAshi().HAOpen[0] > 0.0010 && HeikenAshi().HAHigh[0] - HeikenAshi().HAClose[0] > 0.0010 &&
                    Math.Abs(HeikenAshi().HAHigh[0] - HeikenAshi().HAClose[0]) > Math.Abs(HeikenAshi().HAClose[0] - HeikenAshi().HAOpen[0]))
                {
                    //BackColor = Color.Aqua;

                    //How do you get indicator value from a custom ind.

                    Print(anaTradersDynamicIndex(34, 2, 13, 7, 1.62).Upper[0]);
                    Print(anaTradersDynamicIndex(34, 2, 13, 7, 1.62).Average[0]);
                    Print(anaTradersDynamicIndex(34, 2, 13, 7, 1.62).Lower[0]);
                    Print(anaTradersDynamicIndex(34, 2, 13, 7, 1.62).PriceLine[0]);
                    Print(anaTradersDynamicIndex(34, 2, 13, 7, 1.62).SignalLine[0]);

                }
            }
        }
    }
}


