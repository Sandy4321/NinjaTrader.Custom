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

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// The strategy identifies the IB range for the price action and bets that any breakout of the IB range will fail. (Inverse logic of Toby Crabel)
    /// In the event that the breakout succeeds, which is less frequent than trading range days. Exit the position at IB range derived stop loss price
    /// In the event that a swing stop loss is hit, adjust the scalp portion, (which usually targets the open price, of the bar prior to the entry) to
    /// target the IB range profit target on the next entry. If the target is achieved the scalp portion goes back to normal. (An exit on close, where
    /// the profit target is not reaches, will not reset the orginal scalp target.
    ///
    /// Entry Probability is set to enter only if todays IB High/Low price is contained in the prior IB range and the ADX is within set tolerance.
    /// If current IB range completely engulfs the prior IB range, do no enter limit orders. Also, if the IB range is very small, the day potentially
    /// can be a breakout day and therefore the strategy is disabled.
    /// </summary>
    [Description("Inverse Toby Crabel Swing Strategy")]
    public class TCPinBars : Strategy
    {

        private double slDiv = 2; //Adjust the stoploss yellow line which is derieved off the IB range
        private TimeSpan IB30Time = new TimeSpan(10, 00, 00);
        private TimeSpan IBCloseTrading = new TimeSpan(12, 00, 00);

        private int adxPeriod = 14;
        private double enableEntryBelowADX = 30.0;
        private double enableEntryAboveIBRange = 3.00;

        private double BarCounter = 0;
        private bool ConfirmationNextBar = false;

        #region Required Variables

        //private bool IsFirstEntryLong = false;

        private DateTime LineStartTime;
        private DateTime LineEndTime;

        private int dayIdx = 0;
        private InitialBalance IB30;
        private IBStatistics inReport = new IBStatistics();
        private Dictionary<int, InitialBalance> DIB30 = new Dictionary<int, InitialBalance>();
        private bool recFirstItemOnly = true;

        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            //TraceOrders = true;
            CalculateOnBarClose = true;
            EntriesPerDirection = 3;
            Enabled = true;
            Unmanaged = true;

        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        ///

        protected override void OnBarUpdate()
        {

            if (IB30 != null && Time[0].TimeOfDay >= IB30Time && Time[0].TimeOfDay <= IBCloseTrading)
            {
                if (ConfirmationNextBar == true)
                {
                    if (Close[0] > Open[0] && Low[0] < Open[0])
                    {
                        BackColor = Color.Tomato;
                        BarCounter++;
                        Print(BarCounter);
                    }

                    ConfirmationNextBar = false;
                }

                if (Low[0] < IB30.IBLow && Close[0] > IB30.IBLow && (Close[0] < Open[0])
                    && Low[0] < Bollinger(2,14).Lower[0])
                {
                    BackColor = Color.Yellow;

                    ConfirmationNextBar = true;

                }
            }
            if (BarsInProgress == 0)
            {
                double ibHigh = 0;
                double ibLow = 9999;
                double ibCummVolume = 0;


                if (Bars.BarsSinceSession == 0)
                {
                    recFirstItemOnly = true;
                }

                if (Time[0].TimeOfDay >= IB30Time && recFirstItemOnly == true)
                {
                    LineStartTime = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, 9, 30, 00);
                    LineEndTime = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, 15, 59, 59);

                    IB30 = new InitialBalance()
                    {
                        IBStartTime = Time[Bars.BarsSinceSession],
                        IBEndTime = Time[0],
                        IBClose = Close[0],
                        IBOpen = Open[Bars.BarsSinceSession]
                    };

                    for (int i = 0; i <= Bars.BarsSinceSession; i++)
                    {
                        ibHigh = Math.Max(ibHigh, High[i]);
                        ibLow = Math.Min(ibLow, Low[i]);
                        ibCummVolume = ibCummVolume + Volume[i];
                    }

                    IB30.IBLow = ibLow;
                    IB30.IBHigh = ibHigh;
                    IB30.IBVolume = ibCummVolume;

                    DIB30.Add(dayIdx, IB30);
                    dayIdx++;

                    recFirstItemOnly = false;

                    if (Probability() == true)
                    {

                    }
                    #region Draw Objects
                    DrawLine("IBHigh" + Time[0].Date, false, LineStartTime, IB30.IBHigh, LineEndTime, IB30.IBHigh, Color.Aqua, DashStyle.Solid, 2);
                    DrawLine("IBLow" + Time[0].Date, false, LineStartTime, IB30.IBLow, LineEndTime, IB30.IBLow, Color.Coral, DashStyle.Solid, 2);
                    DrawLine("IBHighTarget" + Time[0].Date, false, LineStartTime, (IB30.IBHigh + Math.Abs(IB30.IBLow - IB30.IBHigh) / slDiv), LineEndTime, (IB30.IBHigh + Math.Abs(IB30.IBLow - IB30.IBHigh) / slDiv), Color.Yellow, DashStyle.Dash, 1);
                    DrawLine("IBLowTarget" + Time[0].Date, false, LineStartTime, IB30.IBLow - Math.Abs(IB30.IBLow - IB30.IBHigh) / slDiv, LineEndTime, IB30.IBLow - Math.Abs(IB30.IBLow - IB30.IBHigh) / slDiv, Color.Yellow, DashStyle.Dash, 1);

                    if (Close[0] <= (IB30.IBLow + (Math.Abs(IB30.IBHigh - IB30.IBLow)) / 2))
                        DrawText("IBRnage" + Time[0].Date, String.Format("ib.{0}   adx.{1}", (IB30.IBHigh - IB30.IBLow).ToString("0.00"), ADX(14)[0].ToString("0"), IB30.IBVolume.ToString("0")), 0, IB30.IBHigh + 2 * TickSize, Color.Black);
                    else
                        DrawText("IBRnage" + Time[0].Date, String.Format("ib.{0}   adx.{1}", (IB30.IBHigh - IB30.IBLow).ToString("0.00"), ADX(14)[0].ToString("0"), IB30.IBVolume.ToString("0")), 0, IB30.IBLow - 2 * TickSize, Color.Black);
                    #endregion
                }
            }
        }
        protected override void OnOrderUpdate(IOrder order)
        {
        }
        protected override void OnExecution(IExecution execution)
        {
        }
        private bool Probability()
        {
            return true;
        }
    }
}



