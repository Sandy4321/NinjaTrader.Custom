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
    /// Enter the description of your strategy here
    /// </summary>
    [Description("Testing multithread strategy for high frequency trading models")]
    public class ThreadStrategy : Strategy
    {

        Dictionary<int, BarPattern> pinBarPattern = new Dictionary<int, BarPattern>();
        private IOrder scalpEntry = null;
        private IOrder scalpStopLoss = null;
        private IOrder scalpProfitTarget = null;
       



        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = true;
            Enabled = true;
            DataSeries pinBar = new DataSeries(this);
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            if (scalpEntry != null && Position.MarketPosition != MarketPosition.Flat && BarsSinceEntry("ENTRY") >= 4)
            {
                ExitShort("ENTRY");
            }
            // Bull Pin bar
            //if (Close[1] < Open[1] && Math.Abs(High[1] - Open[1]) >  Math.Abs(Close[1] - Open[1]) &&
            //    Close[2] > Open[2] && Open[2] == Low[2] && Math.Abs(Close[2] - Open[2]) >= 0 && Close[0] < Open[0] && Close[0] == Low[0])
            
            //if (Close[1] == Open[0] && Bollinger(2,14).Upper[0] < Open[0] && Close[0] <= Math.Max(Close[0],Open[0]) - 2*(Math.Abs(Close[1]-Open[1])))
            if (Close[1] -Open[1] > 0.5 && Open[0] == Close[1] && Close[0] > Open[1] && Open[0]-Close[0] >0)
            {
                BackColor = Color.Aqua;

                if (true)
                {
                    scalpEntry = EnterShort(1, "ENTRY");
                }
            }
        }
        protected override void OnExecution(IExecution execution)
        {
            if (scalpEntry != null && scalpEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled ||
               execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                scalpStopLoss = ExitShortStop(0, true, execution.Order.Filled, Math.Max(Math.Max(High[0],High[1]),High[2]), "SL", "ENTRY");
                scalpProfitTarget = ExitShortLimit(0, true, execution.Order.Filled, scalpEntry.AvgFillPrice - 2*Math.Abs(Math.Max(Math.Max(High[0], High[1]), High[2])- scalpEntry.AvgFillPrice), "PT", "ENTRY");
                scalpEntry = null;
            }
        }
    }
    public class BarPattern
    {
        public int BarSinceSession { get; set; }
        public double Low { get; set; }
        public double High { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double AbsCloseOpenDiff { get; set; }
        public Direciton BarDirection { get; set; }
        public enum Direciton
        {
            UpBar,
            DownBar,
        }
        public double AbsTopTail { get; set; }
        public double  AbsBottomTail { get; set; }
    }

}