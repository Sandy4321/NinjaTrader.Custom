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
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace NinjaTrader.Strategy
{
    /// <summary>
    ///
    /// </summary>
    [Description("Bear Component of Gap Bar Strategy")]
    public class BearGapBar : Strategy
    {
        #region Variables
        private string path = "C:\\log\\bearGapBar.log";

        private int emaPrimPeriod = 20;
        private int emaRefPeriod = 100;
        private int minutePeriod = 5;
        private int minuteEmaPeriod = 5;

        private IOrder scalpEntry = null;
        private IOrder scalpStopLoss = null;
        private IOrder scalpProfitTarget = null;
        private int scalpQty = 2;
        private double entryPrice;
        private double lastEntryPrice;
        private string scalpEntryName = "SCBE.EN";
        private string scalpProfitName = "SCBE.PT";
        private string scalpStopName = "SCBE.SL";
        private double scalpTarget = 9999;
        private int exceedBarCount = 10;
        private double profitPerTick = 12.5;
        private double orgStopPxInTicks = 8;
        private double orgProfitPxInTicks = 16;


        private IOrder swingEntry = null;
        private IOrder swingStopLoss = null;
        private IOrder swingProfitTarget = null;
        private string swingEntryName = "SWBE.EN";
        private string swingProfitName = "SWBE.PT";
        private string swingStopName = "SWBE.SL";
        private string swingBreakevenName = "SCRATCH";
        private string swingMfeBreakevenName = "MFE STOP";
        private string swingEmaCrossoverName = "EMA CROSS";
        private string exceedTTL = "TTL";
        private int swingQty = 3;
        private double swingTargetinTicks = 16;
        private double mfeStopPrice = 9999;
        private double breakevenPrice = 9999;

        private double pullbackPercentage = 0.50;

        private int gbReqBars = 15;
        private int gbCounter = 0;
        private int barsSinceSignal = 0;
        private int idxSignalPoints = 0;
        private bool isTrendUp = false;

        Dictionary<int, GapBars> dGapBars = new Dictionary<int, GapBars>();
        Dictionary<int, InPosition> dInPosition = new Dictionary<int, InPosition>();
        Dictionary<int, SignalPoints> dSignalPoints = new Dictionary<int, SignalPoints>();
        Dictionary<int, HTFSignalPoints> dHTFSignalPoints = new Dictionary<int, HTFSignalPoints>();
        SignalPoints sigPoints;


        #endregion

        protected override void Initialize()
        {
            CalculateOnBarClose = true;
            //TraceOrders = true;
            EntriesPerDirection = 10;
            ExitOnClose = true;
            ExitOnCloseSeconds = 60;
            Enabled = true;
            Add(PeriodType.Minute, minutePeriod);
        }

        protected override void OnBarUpdate()
        {

            string kObj = "OnBarUpdate";

            if (BarsInProgress == 0)
            {
                if (Bars.BarsSinceSession == 0) StartOfSession();

                if (Position.MarketPosition == MarketPosition.Short)      // Early exit strategy
                {
                    CheckForTTLExceeded();
                    CheckMFEBreakeven();
                    CheckforEmaCrossover();
                }

                if (CrossBelow(EMA(emaPrimPeriod), EMA(emaRefPeriod), 1) == true ||
                    CrossAbove(EMA(emaPrimPeriod), EMA(emaRefPeriod), 1) == true) AddTrendSignal();


                if (High[0] > EMA(emaPrimPeriod)[0] && gbCounter >= gbReqBars && Bars.BarsSinceSession != gbCounter)
                {
                    BackColor = Color.Yellow;
                    barsSinceSignal = Bars.BarsSinceSession - barsSinceSignal;
                    for (int gb = gbCounter + barsSinceSignal; gb > 0; gb--)
                    {
                        GapBars sig = new GapBars()
                        {
                            Bar = Bars.BarsSinceSession - gb + 1,
                            Close = Close[gb]
                        };
                        dGapBars.Add((gbCounter + barsSinceSignal - gb), sig);
                        scalpTarget = Math.Min(scalpTarget, Close[gb]);
                        if (gb == gbCounter + barsSinceSignal) kLog(Environment.NewLine + Environment.NewLine);
                        if (gb > 1) kLog(kObj, "SIG", String.Format(" [{0}]   close {1}", (gbCounter + barsSinceSignal - gb), sig.Close.ToString("0.00")));
                        if (gb == 1) kLog(kObj, "SIG", String.Format(" [{0}]   close {1}   profit target {2}", (gbCounter + barsSinceSignal - gb), sig.Close.ToString("0.00"), scalpTarget.ToString("0.00")));
                    }
                    AddTrendSignal();
                    PrintTrendSignals();
                    EnterOnEMA();
                }
                else if (High[0] > EMA(emaPrimPeriod)[0] && gbCounter >= gbReqBars && Bars.BarsSinceSession == gbCounter)
                {
                    gbCounter = 0;
                }


                if (High[0] <= EMA(emaPrimPeriod)[0])
                {
                    gbCounter++;
                }
                else
                {
                    gbCounter = 0;
                    dGapBars.Clear();
                }

                if (gbCounter == gbReqBars)
                {
                    barsSinceSignal = Bars.BarsSinceSession;
                    BackColor = Color.LightCoral;
                }
            }
        }

        protected override void OnOrderUpdate(IOrder order)
        {
            string kObj = "OnOrderUpdate";
            if (scalpProfitTarget == order && (order.OrderState == OrderState.Working || order.OrderState == OrderState.Cancelled || order.OrderState == OrderState.Filled)) kLog(kObj, "ORD", order.ToString());
            if (swingProfitTarget == order && (order.OrderState == OrderState.Working || order.OrderState == OrderState.Cancelled || order.OrderState == OrderState.Filled)) kLog(kObj, "ORD", order.ToString());
            if (scalpStopLoss == order && (order.OrderState == OrderState.Working || order.OrderState == OrderState.Cancelled || order.OrderState == OrderState.Filled)) kLog(kObj, "ORD", order.ToString());
            if (swingStopLoss == order && (order.OrderState == OrderState.Working || order.OrderState == OrderState.Cancelled || order.OrderState == OrderState.Filled)) kLog(kObj, "ORD", order.ToString());
        }
        protected override void OnExecution(IExecution execution)
        {
            string kObj = "OnExecution";

            if (scalpEntry == execution.Order || swingEntry == execution.Order)
            {
                // set breakeven and last entry price to make mfe breakeven stop calculation easier
                mfeStopPrice = 9999;
                lastEntryPrice = entryPrice = breakevenPrice = execution.Order.AvgFillPrice;
                kLog(kObj, "EXE", execution.Order.ToString());
            }
            if (scalpEntry != null && scalpEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled ||
                execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                scalpStopLoss = ExitShortStop(0, true, execution.Order.Filled, Instrument.MasterInstrument.Round2TickSize(execution.Order.AvgFillPrice + orgStopPxInTicks * TickSize), scalpStopName, scalpEntryName);
                scalpProfitTarget = ExitShortLimit(0, true, execution.Order.Filled, scalpTarget, scalpProfitName, scalpEntryName);
                scalpEntry = null;
            }
            if (swingEntry != null && swingEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled ||
                execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                swingStopLoss = ExitShortStop(0, true, execution.Order.Filled, Instrument.MasterInstrument.Round2TickSize(execution.Order.AvgFillPrice + orgStopPxInTicks * TickSize), swingStopName, swingEntryName);
                swingProfitTarget = ExitShortLimit(0, true, execution.Order.Filled, Instrument.MasterInstrument.Round2TickSize(execution.Order.AvgFillPrice - orgProfitPxInTicks * TickSize), swingProfitName, swingEntryName);
                swingEntry = null;
            }
            if (scalpProfitTarget == execution.Order && scalpProfitTarget.OrderState == OrderState.Filled)
            {
                try
                {
                    // set swing position to breakeven on successful scalph fill. Move Stop to Market - 1 tick if breakeven stop order cannot be placed successfully
                    if (Close[0] < entryPrice) swingStopLoss = ExitShortStop(0, true, swingStopLoss.Quantity, entryPrice, swingBreakevenName, swingEntryName);
                    else if (Close[0] >= entryPrice) ExitShortStop(0, true, swingStopLoss.Quantity, Close[0] + 1 * TickSize, swingBreakevenName + " MKT", swingEntryName);

                    kLog(kObj, "ORD", swingStopLoss.ToString());
                    kLog(kObj, "ORD", String.Format("Move swing position to breakeven price {0}", entryPrice));
                }
                catch (Exception ex)
                {
                    kLog(kObj, "ERROR", "Exception caused by moving swing stop loss position" + Environment.NewLine + ex.ToString());
                }
                entryPrice = 0;
            }
            if (scalpStopLoss == execution.Order && scalpStopLoss.OrderState == OrderState.Filled || scalpProfitTarget == execution.Order && scalpProfitTarget.OrderState == OrderState.Filled) ClearScalpEntryInfo();
            if (swingStopLoss == execution.Order && swingStopLoss.OrderState == OrderState.Filled || swingProfitTarget == execution.Order && swingProfitTarget.OrderState == OrderState.Filled) ClearSwingEntryInfo();

        }
        protected override void OnTermination()
        {
            File.WriteAllText(path, string.Empty);
        }
        protected bool Probability()
        {
            if (dSignalPoints.Count > 2 && Close[0] - scalpTarget >= 0.50 && Close[0] - scalpTarget <= 1.25)
            {
                if (dSignalPoints[dSignalPoints.Count - 1].Trend == SignalPoints.Direction.Dn &&
                    dSignalPoints[dSignalPoints.Count - 2].Trend == SignalPoints.Direction.Dn &&
                    dSignalPoints[dSignalPoints.Count - 3].Trend == SignalPoints.Direction.Dn && dSignalPoints[dSignalPoints.Count - 3].EMAHasCrossed == true)
                {
                    return true;
                }
            }
            return false;
        }
        protected void ClearScalpEntryInfo()
        {
            scalpStopLoss = scalpProfitTarget = null;
            scalpTarget = 9999;
            dGapBars.Clear();
            dInPosition.Clear();
        }
        protected void ClearSwingEntryInfo()
        {
            swingProfitTarget = swingStopLoss = null;
            mfeStopPrice = 9999;
            dInPosition.Clear();
        }
        protected void CheckMFEBreakeven()
        {
            string kObj = "CheckMFEBreakeven";

            InPosition entryPosition = new InPosition()
            {
                BarsSinceEntry = BarsSinceEntry(0, swingEntryName, 0),
                BarsSinceSession = Bars.BarsSinceSession,
                MFE = (Close[0] <= lastEntryPrice) ? (lastEntryPrice - Close[0]) / TickSize * profitPerTick : 0,
                MAE = (Close[0] > lastEntryPrice) ? (Close[0] - lastEntryPrice) / TickSize * profitPerTick : 0
            };
            try
            {
                dInPosition.Add(entryPosition.BarsSinceSession, entryPosition);
            }
            catch (Exception ex)
            {
                kLog(kObj, "ERROR", " dInPosition is not empty" + Environment.NewLine + ex.ToString());
            }

            mfeStopPrice = Math.Min(mfeStopPrice, lastEntryPrice - Instrument.MasterInstrument.Round2TickSize(entryPosition.MFE / profitPerTick * TickSize * pullbackPercentage));

            if (breakevenPrice - mfeStopPrice >= 1) breakevenPrice = mfeStopPrice;

            kLog(kObj, "BULL", String.Format("[e.{0}]   mae {1}   mfe {2}   derived stop {3}   set {4}",
                entryPosition.BarsSinceEntry,
                entryPosition.MAE.ToString("0.00"),
                entryPosition.MFE.ToString("0.00"),
                mfeStopPrice,
                breakevenPrice
                ));

            if (swingStopLoss != null && swingStopLoss.StopPrice <= lastEntryPrice && swingStopLoss.StopPrice > breakevenPrice)
            {
                try
                {
                    swingStopLoss = ExitShortStop(0, true, swingQty, breakevenPrice, swingMfeBreakevenName, swingEntryName);
                }
                catch (Exception ex)
                {
                    kLog(kObj, "ERROR", "Exception caught when moving stop loss price to mfe breakeven" + Environment.NewLine + ex.ToString());
                }
            }
        }
        protected void AddTrendSignal()
        {
            if (isTrendUp == true)
            {
                if (CrossBelow(EMA(emaPrimPeriod), EMA(emaRefPeriod), 1) == true)
                {
                    sigPoints = new SignalPoints()
                    {
                        BarsSinceSession = Bars.BarsSinceSession,
                        EMAHasCrossed = true,
                        Trend = SignalPoints.Direction.Dn,
                        EMAPrimary = 0,
                        EMAReference = 0
                    };
                    dSignalPoints.Add(idxSignalPoints, sigPoints);
                    idxSignalPoints++;
                    isTrendUp = false;
                }
                if (CrossBelow(EMA(emaPrimPeriod), EMA(emaRefPeriod), 1) == false)
                {
                    sigPoints = new SignalPoints()
                    {
                        BarsSinceSession = Bars.BarsSinceSession,
                        EMAHasCrossed = false,
                        Trend = SignalPoints.Direction.Up,
                        EMAPrimary = EMA(emaPrimPeriod)[0],
                        EMAReference = EMA(emaRefPeriod)[0]
                    };
                    dSignalPoints.Add(idxSignalPoints, sigPoints);
                    idxSignalPoints++;
                }
            }
            else
            {
                if (CrossAbove(EMA(emaPrimPeriod), EMA(emaRefPeriod), 1) == true)
                {
                    sigPoints = new SignalPoints()
                    {
                        BarsSinceSession = Bars.BarsSinceSession,
                        EMAHasCrossed = true,
                        Trend = SignalPoints.Direction.Up,
                        EMAPrimary = 0,
                        EMAReference = 0
                    };
                    dSignalPoints.Add(idxSignalPoints, sigPoints);
                    idxSignalPoints++;
                    isTrendUp = true;
                }
                if (CrossAbove(EMA(emaPrimPeriod), EMA(emaRefPeriod), 1) == false)
                {
                    sigPoints = new SignalPoints()
                    {
                        BarsSinceSession = Bars.BarsSinceSession,
                        EMAHasCrossed = false,
                        Trend = SignalPoints.Direction.Dn,
                        EMAPrimary = EMA(emaPrimPeriod)[0],
                        EMAReference = EMA(emaRefPeriod)[0]
                    };
                    dSignalPoints.Add(idxSignalPoints, sigPoints);
                    idxSignalPoints++;
                }
            }
        }
        protected void PrintTrendSignals()
        {
            string kObj = "PrintTrendSignals";
            kLog("");
            foreach (KeyValuePair<int, SignalPoints> key in dSignalPoints)
            {
                SignalPoints sig = key.Value;
                kLog(kObj, "CROSS", String.Format("[{0}]   cross: {1}   trend: {2}   diff: {3}",
                    sig.BarsSinceSession,
                    sig.EMAHasCrossed.ToString().ToLower(),
                    sig.Trend.ToString().ToUpper(),
                    (sig.EMAPrimary - sig.EMAReference).ToString("0.00")
                    ));
            }
            kLog("");
        }
        protected void EnterOnEMA()
        {
            //string kObj = "EnterOnEMA";

            if (dGapBars != null)
            {
                if (Probability() == true)
                {
                    scalpEntry = EnterShort(scalpQty, scalpEntryName);
                    swingEntry = EnterShort(swingQty, swingEntryName);
                    kLog(Environment.NewLine + "Placed market entry order(s)" + Environment.NewLine);
                }
                dGapBars.Clear();
                gbCounter = barsSinceSignal = 0;
            }
        }
        protected void CheckForTTLExceeded()
        {
            string kObj = "CheckForEarlyExit";

            if (Position.GetProfitLoss(Close[0], PerformanceUnit.Points) <= 0 &&
                (BarsSinceEntry(0, scalpEntryName, 0) >= exceedBarCount || BarsSinceEntry(0, swingEntryName, 0) >= exceedBarCount))
            {
                kLog(kObj, "ORD", "Close position due to TTL bars exceeded");
                try
                {
                    ExitShort(exceedTTL, scalpEntryName);
                    ExitShort(exceedTTL, swingEntryName);
                    ClearScalpEntryInfo();
                    ClearSwingEntryInfo();
                }
                catch (Exception ex)
                {
                    kLog(kObj, "ERROR", "Exception caused by exiting on market when the bar since entry exceeds count" + Environment.NewLine + ex.ToString());
                }
            }
        }
        protected void CheckforEmaCrossover()
        {
            string kObj = "CheckforEmaCrossover";

            if (CrossBelow(EMA(emaPrimPeriod), (emaRefPeriod), 1) == true)
            {
                kLog(kObj, "ORD", "Close position due to EMA crossover");
                try
                {
                    ExitShort(swingEmaCrossoverName, scalpEntryName);
                    ExitShort(swingEmaCrossoverName, swingEntryName);
                    ClearScalpEntryInfo();
                    ClearSwingEntryInfo();
                }
                catch (Exception ex)
                {
                    kLog(kObj, "ERROR", "Exception caused by exiting at market for ema crossover" + Environment.NewLine + ex.ToString());
                }
            }
        }
        protected void StartOfSession()
        {
            string kObj = "StartOfSession";
            ClearScalpEntryInfo();
            ClearSwingEntryInfo();
            gbCounter = 0;
            dSignalPoints.Clear();

            double emaPrimValue = EMA(emaPrimPeriod)[0];
            double emaRefValue = EMA(emaRefPeriod)[0];
            isTrendUp = (emaPrimValue >= emaRefValue) ? true : false;
            idxSignalPoints = 0;

            sigPoints = new SignalPoints()
            {
                BarsSinceSession = Bars.BarsSinceSession,
                EMAPrimary = EMA(emaPrimPeriod)[0],
                EMAReference = EMA(emaRefPeriod)[0],
                Trend = (isTrendUp == true) ? SignalPoints.Direction.Up : SignalPoints.Direction.Dn,
                EMAHasCrossed = false
            };
            dSignalPoints.Add(sigPoints.BarsSinceSession, sigPoints);
            idxSignalPoints++;

            kLog(kObj, "INFO", "Generate initial signal points and clear data from previous day");

        }
        private void kLog(string ClassMethod, string MsgType, string Msg)
        {
            string _msgOut = (String.Format("{0}  {1} [{2}]  {3}  [{4}]  {5}",
                Time[0].ToString("dd/MM/yyyy"),
                Time[0].ToString("HH:mm:ss"),
                Bars.BarsSinceSession,
                MsgType,
                ClassMethod,
                Msg)) + Environment.NewLine;
            File.AppendAllText(path, _msgOut);
        }
        private void kLog(string Msg)
        {
            string _msgOut = (String.Format("{0}",
                Msg)) + Environment.NewLine;
            File.AppendAllText(path, _msgOut);
        }
    }
}






