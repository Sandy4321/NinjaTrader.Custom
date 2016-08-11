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
    [Description("Enter the description of your strategy here")]
    public class ParaSAR : Strategy
    {

        private int scalpQty = 1;
        private int swingQty = 1;
        //private int numOfEntries = 0;
        private int Idx = 0;

        private IOrder scalpEntry = null;
        private IOrder scalpStopLoss = null;
        private IOrder scalpProfitTarget = null;
        private double entryPrice;
        private double lastEntryPrice;
        private string scalpEntryName = "SCBE.EN";
        private string scalpProfitName = "SCBE.PT";
        private string scalpStopName = "SCBE.SL";

        private double scalpTarget = 0;
        private int exceedBarCount = 2;
        //private double profitPerTick = 12.5;
        private double orgStopPxInTicks = 5;
        private double orgProfitPxInTicks = 5;

        Dictionary<int, ParaSARStdPoints> lastThreeBearStdPoints = new Dictionary<int, ParaSARStdPoints>();



        private IOrder swingEntry = null;
        private IOrder swingStopLoss = null;
        private IOrder swingProfitTarget = null;
        private string swingEntryName = "SWBE.EN";
        private string swingProfitName = "SWBE.PT";
        private string swingStopName = "SWBE.SL";
        private string swingBreakevenName = "SCRATCH";
        private string exceedTTL = "TTL";
        private double swingTargetinTicks = 16;

        public struct ParaSARStdPoints
        {
            public int CurBar;
            public double StdValue;

            public ParaSARStdPoints(int bar, double stdV)
            {
                CurBar = bar;
                StdValue = stdV;
            }
            public override string ToString()
            {
                return (String.Format("[{0}]   {1}", CurBar,StdValue));
            }
        }

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            TraceOrders = true;
            CalculateOnBarClose = true;
            Enabled = true;
            ExitOnClose = false;
            //ExitOnCloseSeconds = 60;
            EntriesPerDirection = 10;

        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            if (Close[0] < ParabolicSAR(0.02, 0.2, 0.02)[0] && Close[1] > ParabolicSAR(0.02, 0.2, 0.02)[1])
            {
                ParaSARStdPoints sig = new ParaSARStdPoints(Bars.BarsSinceSession, StdDev(ParabolicSAR(0.02, 0.2, 0.02), 14)[0]);
                lastThreeBearStdPoints.Add(Idx, sig);
                Idx++;
                Print(Idx);
            }

            if (BearValidStdDevSeq())
            {
                BackColor = Color.Yellow;
                scalpEntry = EnterShort(scalpQty, scalpEntryName);
                swingEntry = EnterShort(swingQty, swingEntryName);
            }
            if (BullValidStdDevSeq())
            {
                //BackColor = Color.Aqua;
            }
        }
        private bool BearValidStdDevSeq()
        {
            if (lastThreeBearStdPoints != null && lastThreeBearStdPoints.Count ==3)
            {
                Print("In BearBalidStdSeq Loop");
                if (Close[0] < ParabolicSAR(0.02, 0.2, 0.02)[0] && //StdDev(ParabolicSAR(0.02, 0.2, 0.02), 14)[0] > 0.0005 &&
                    Close[1] > ParabolicSAR(0.02, 0.2, 0.02)[1] && Close[2] > ParabolicSAR(0.02, 0.2, 0.02)[2]

                    &&

                    lastThreeBearStdPoints[2].StdValue > lastThreeBearStdPoints[1].StdValue && 
                    lastThreeBearStdPoints[1].StdValue > lastThreeBearStdPoints[0].StdValue &&

                    lastThreeBearStdPoints[2].CurBar - lastThreeBearStdPoints[0].CurBar >50
                    //&& Close[0] < EMA(100)[0]
                    )
                {
                    foreach (KeyValuePair<int, ParaSARStdPoints> key in lastThreeBearStdPoints)
                    {
                        ParaSARStdPoints result = key.Value;
                        Print("Printing the foreach loop");
                        Print(result.ToString());
                    }
                    lastThreeBearStdPoints.Clear();
                    Idx = 0;
                    Print("Clearing inner lopp Dict");
                    return true;
                }
                lastThreeBearStdPoints.Clear();
                Idx = 0;
                Print("Clearing Outerloop Dict");
                return false;
            }
            return false;
        }
        private bool BullValidStdDevSeq()
        {
            if (Close[0] > ParabolicSAR(0.02, 0.2, 0.02)[0] && StdDev(ParabolicSAR(0.02, 0.2, 0.02), 14)[0] > 0.0005 &&
                Close[1] < ParabolicSAR(0.02, 0.2, 0.02)[1] && Close[2] < ParabolicSAR(0.02, 0.2, 0.02)[2]
                //&& Close[0] > EMA(100)[0]
                )
            {
                return true;
            }
            return false;
        }

        protected override void OnExecution(IExecution execution)
        {
            if (scalpEntry != null && scalpEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled ||
                execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                entryPrice = scalpEntry.AvgFillPrice;
                Print(string.Format("[{0}]  Short the Market  {1}", Bars.BarsSinceSession, entryPrice));
                scalpStopLoss = ExitShortStop(0, true, execution.Order.Filled, Instrument.MasterInstrument.Round2TickSize(execution.Order.AvgFillPrice + orgStopPxInTicks * TickSize), scalpStopName, scalpEntryName);
                scalpProfitTarget = ExitShortLimit(0, true, execution.Order.Filled, Instrument.MasterInstrument.Round2TickSize(execution.Order.AvgFillPrice - orgProfitPxInTicks * TickSize), scalpProfitName, scalpEntryName);
                scalpEntry = null;
            }
            if (swingEntry != null && swingEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled ||
                execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                swingStopLoss = ExitShortStop(0, true, execution.Order.Filled, Instrument.MasterInstrument.Round2TickSize(execution.Order.AvgFillPrice + orgStopPxInTicks * TickSize), swingStopName, swingEntryName);
                swingProfitTarget = ExitShortLimit(0, true, execution.Order.Filled, Instrument.MasterInstrument.Round2TickSize(execution.Order.AvgFillPrice - 3 * orgProfitPxInTicks * TickSize), swingProfitName, swingEntryName);
                swingEntry = null;
            }
            if (scalpProfitTarget == execution.Order && scalpProfitTarget.OrderState == OrderState.Filled)
            {
                try
                {
                    Print(string.Format("[{0}]    breakeven price: {1}   close: {2}", Bars.BarsSinceSession, entryPrice, Close[0]));
                    // set swing position to breakeven on successful scalph fill. Move Stop to Market - 1 tick if breakeven stop order cannot be placed successfully
                    if (Close[0] < entryPrice) swingStopLoss = ExitShortStop(0, true, swingStopLoss.Quantity, entryPrice, swingBreakevenName, swingEntryName);
                    else if (Close[0] >= entryPrice) ExitShortStop(0, true, swingStopLoss.Quantity, Close[0] + 1 * TickSize, swingBreakevenName + " MKT", swingEntryName);

                    Print("Moving swing stop to breakeven");
                }
                catch (Exception ex)
                {
                    Print("Exception caused by moving swing stop loss position" + Environment.NewLine + ex.ToString());
                }
                //entryPrice = 0;
            }
            if (scalpStopLoss == execution.Order && scalpStopLoss.OrderState == OrderState.Filled || scalpProfitTarget == execution.Order && scalpProfitTarget.OrderState == OrderState.Filled)
            {
                scalpStopLoss = scalpProfitTarget = null;
                Print(execution.Order.ToString());
            }
            if (swingStopLoss == execution.Order && swingStopLoss.OrderState == OrderState.Filled || swingProfitTarget == execution.Order && swingProfitTarget.OrderState == OrderState.Filled)
            {
                swingStopLoss = swingProfitTarget = null;
                Print(execution.Order.ToString());
            }

        }
    }

    //public class 
}
