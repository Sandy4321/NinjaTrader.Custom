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

namespace NinjaTrader.Strategy
{
    [Description("Enter the description of your strategy here")]
    public class SwingStorm : Strategy
    {

        #region Swing Variables
        private int swingSize = 7;
        private int dtbStrength = 15;
        private List<Swings> swingHighs = new List<Swings>();
        private List<Swings> swingLows = new List<Swings>();
        private int decimalPlaces = 0;
        private int bar = 0;
        private double price = 0.0;
        private int newSwing = 0;
        private bool newHigh = false;
        private bool newLow = false;
        private bool updateHigh = false;
        private bool updateLow = false;
        private int swingCounterDn = 0;
        private int swingCounterUp = 0;
        private int upCount = 0;
        private int downCount = 0;
        private int trendChangeBar = 0;
        private int swingSlope = 0;
        private double curHigh = 0.0;
        private int curHighBar = 0;
        private int curHighDuration = 0;
        private int curHighLength = 0;
        private Relation curHighRelation = Relation.Higher;
        private Relation lastHighRelation = Relation.Higher;
        private double curLow = 0.0;
        private int curLowBar = 0;
        private int curLowDuration = 0;
        private int curLowLength = 0;
        private Relation curLowRelation = Relation.Higher;
        private double lastHigh = 0.0;
        private int lastHighBar = 0;
        private int lastHighDuration = 0;
        private int lastHighLength = 0;
        private double lastLow = 0.0;
        private int lastLowBar = 0;
        private double lastLowDuration;
        private int lastLowLength = 0;
        private Relation lastLowRelation = Relation.Higher;
        private Color textColourHigher = Color.Green;
        private Color textColourLower = Color.Red;
        private Color textColourDtb = Color.Gold;
        private Font textFont = new Font("Courier", 9, FontStyle.Regular);
        private Color zigZagColourUp = Color.Green;
        private Color zigZagColourDn = Color.Red;
        private DashStyle zigZagStyle = DashStyle.Dash;
        private int zigZagWidth = 1;
        private int offsetText = 20;


        private struct Swings
        {
            public int barNumber;
            public double price;
            public int length;
            public int duration;
            public Relation relation;

            public Swings(int swingBarNumber, double swingPrice,
                    int swingLength, int swingDuration, Relation swingRelation)
            {
                barNumber = swingBarNumber;
                price = swingPrice;
                length = swingLength;
                duration = swingDuration;
                relation = swingRelation;
            }

            public override string ToString()
            {
                return string.Format("bar: {0}   px: {1}   length: {2}   dur: {3}   rel: {4}", barNumber, price, length, duration, relation);
            }
        }

        private struct TrendChangeBarStats
        {
            public int _curBar;
            public double _open;
            public double _high;
            public double _low;
            public double _close;
            public Trend _trend;
            //public int _barsSinceLastTrend;

            public TrendChangeBarStats(int curBar, double open, double high, double low, double close, Trend direction) //, int bSinceLastTrend)
            {
                _curBar = curBar;
                _open = open;
                _high = high;
                _low = low;
                _close = close;
                _trend = direction;
                //_barsSinceLastTrend = bSinceLastTrend;
            }

            public override string ToString()
            {
                return string.Format("b. {0}   o. {1}   h. {2}   l. {3}   c. {4}   d. {5}", _curBar, _open, _high, _low, _close, _trend); //,_barsSinceLastTrend);
            }
        }

        public enum Relation
        {
            Double,
            Higher,
            Lower,
        }
        public enum Trend
        {
            Up,
            Down,
        }
        #endregion

        private int trendIsChangingBar = 0;
        private int doubleBottomBar = 0;
        private int dnHisIndex = 0;
        private int upHisIndex = 0;
        private Dictionary<int, TrendChangeBarStats> dnHistory = new Dictionary<int, TrendChangeBarStats>();
        private Dictionary<int, TrendChangeBarStats> upHistory = new Dictionary<int, TrendChangeBarStats>();
        private Dictionary<int, TrendChangeBarStats> bothHistory = new Dictionary<int, TrendChangeBarStats>();

        private Dictionary<int, TrendChangeBarStats> dT = new Dictionary<int, TrendChangeBarStats>();
        private int dtIndex = 0;
        private bool enableDTOrderEntry = false;
        private int isDTWithinBars = 12;


        #region Strategy Variables
        private IOrder scalpEntry = null;
        private IOrder scalpStopLoss = null;
        private IOrder scalpProfitTarget = null;
        private IOrder swingEntry = null;
        private IOrder swingStopLoss = null;
        private IOrder swingProfitTarget = null;

        private double dtScalpProfitTarget = 0.0;
        private double dtSwingProfitTarget = 0.0;
        private double dtStopLoss = 0.0;
        private double entryPrice = 0.0;
        #endregion

        protected override void Initialize()
        {
            CalculateOnBarClose = true;
            BarsRequired = 2;
            EntriesPerDirection = 2;
            ExitOnClose = false;

        }
        protected override void OnStartUp()
        {
            int incrementLength = (Convert.ToDecimal(Instrument.MasterInstrument.TickSize)).ToString().Length;
            if (incrementLength == 1)
                decimalPlaces = 0;
            else if (incrementLength > 2)
                decimalPlaces = incrementLength - 2;
        }
        private bool IsProbabilityTrue()
        {
            if (
                dT[dT.Count - 1]._close - dT[dT.Count - 1]._open > Math.Abs(Close[2] - Open[2]) &&
                //dT[dT.Count - 1]._close - dT[dT.Count - 1]._open > Math.Abs(Close[3] - Open[3]) &&
            Close[0] < Open[0] //&& Math.Abs(Low[0]-Close[0]) <Math.Abs(Open[0]-Close[0])
                )
            {
                dtScalpProfitTarget = Math.Abs(High[0] - Low[0]);
                dtSwingProfitTarget = dtScalpProfitTarget * 3;
                return true;
            }
            return false;
        }

        protected override void OnBarUpdate()
        {

            if (enableDTOrderEntry == true)
            {
                //if (2 * Math.Abs(Close[0] - Open[0]) < Math.Abs(High[0] - Open[0]))
                if (IsProbabilityTrue())
                {

                    //dtScalpProfitTarget = Math.Abs(doubleTop[doubleTop.Count - 1]._high - doubleTop[doubleTop.Count - 1]._open);
                    //if (dnHistory[dnHistory.Count-1]._curBar - doubleTop[doubleTop.Count - 1]._curBar <= isDTWithinBars)
                    //{
                    //    dtScalpProfitTarget = 0.0010;//= Instrument.MasterInstrument.Round2TickSize(Math.Abs(doubleTop[doubleTop.Count - 1]._close - dnHistory[dnHistory.Count - 1]._close));
                    //    //Print(String.Format("ENABLE DT ENTRY:     PT TICKS {0}    SL PRICE {1}", dtScalpProfitTarget,curHigh));

                    scalpEntry = EnterShortStop(1,Low[0]-1*TickSize, "SC.DTEN");
                    swingEntry = EnterShortStop(1, Low[0] - 1 * TickSize, "SW.DTEN");
                }

                enableDTOrderEntry = false;
            }
            newHigh = newLow = updateHigh = updateLow = false;
            if (CurrentBar < swingSize) return;

            if (swingSlope == 1 && High[0] <= curHigh) newHigh = false;
            else newHigh = true;
            if (swingSlope == -1 && Low[0] >= curLow) newLow = false;
            else newLow = true;

            if (newHigh)
            {
                for (int i = 1; i < swingSize + 1; i++)
                {
                    if (High[0] <= High[i])
                    {
                        newHigh = false;
                        break;
                    }
                }
            }
            if (newLow)
            {
                for (int i = 1; i < swingSize + 1; i++)
                {
                    if (Low[0] >= Low[i])
                    {
                        newLow = false;
                        break;
                    }
                }
            }
            if (newHigh && newLow)
            {
                if (swingSlope == -1) newHigh = false;
                else newLow = false;
            }

            if (newHigh)
            {
                if (swingSlope != 1)
                {
                    bar = CurrentBar - HighestBar(High, CurrentBar - curLowBar);
                    price = High[HighestBar(High, CurrentBar - curLowBar)];
                    updateHigh = false;
                }
                else
                {
                    bar = CurrentBar;
                    price = High[0];
                    updateHigh = true;
                }
                CalcUpSwing(bar, price, updateHigh);
            }
            else if (newLow)
            {
                if (swingSlope != -1)
                {
                    bar = CurrentBar - LowestBar(Low, CurrentBar - curHighBar);
                    price = Low[LowestBar(Low, CurrentBar - curHighBar)];
                    updateLow = false;
                }
                else
                {
                    bar = CurrentBar;
                    price = Low[0];
                    updateLow = true;
                }
                CalcDnSwing(bar, price, updateLow);
            }
        }
        protected override void OnOrderUpdate(IOrder order)
        {
            if (order == swingStopLoss && order.OrderState == OrderState.Working)
            {
                Print(order.ToString());
            }
        }
        protected override void OnExecution(IExecution execution)
        {
            if (scalpEntry != null && scalpEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled ||
                execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                entryPrice = execution.Order.AvgFillPrice;
                scalpStopLoss = ExitShortStop(0, true, execution.Order.Filled, curHigh, "SC.SL", "SC.DTEN");
                scalpProfitTarget = ExitShortLimit(0, true, execution.Order.Filled, execution.Order.AvgFillPrice - dtScalpProfitTarget, "SC.PT", "SC.DTEN");
                scalpEntry = null;
            }
            if (swingEntry != null && swingEntry == execution.Order && (execution.Order.OrderState == OrderState.Filled ||
                execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && OrderState.Filled > 0)))
            {
                swingStopLoss = ExitShortStop(0, true, execution.Order.Filled, curHigh, "SW.SL", "SW.DTEN");
                swingProfitTarget = ExitShortLimit(0, true, execution.Order.Filled, execution.Order.AvgFillPrice - dtSwingProfitTarget, "SW.PT", "SW.DTEN");
                swingEntry = null;
            }
            if (scalpProfitTarget == execution.Order && scalpProfitTarget.OrderState == OrderState.Filled)
            {
                scalpProfitTarget = null;
                try
                {
                    if (Close[0] < entryPrice) swingStopLoss = ExitShortStop(0, true, swingStopLoss.Quantity, entryPrice, "SW.BK", "SW.DTEN");
                    else if (Close[0] >= entryPrice) ExitShortStop(0, true, swingStopLoss.Quantity, High[0] + 1 * TickSize, "SW.BK" + " MKT", "SW.DTEN");
                }
                catch (Exception ex)
                {
                    Print("Exception caused by moving swing stop loss position" + Environment.NewLine + ex.ToString());
                }
            }
            if (execution.Order.OrderState == OrderState.Filled)
            {
                Print(execution.Order.ToString());
            }
        }
        private void CalcDnSwing(int bar, double low, bool updateLow)
        {
            if (!updateLow)
            {
                lastLow = curLow;
                lastLowBar = curLowBar;
                lastLowDuration = curLowDuration;
                lastLowLength = curLowLength;
                lastLowRelation = curLowRelation;
                swingCounterDn++;
                swingSlope = -1;
                trendChangeBar = bar;

                TrendChangeBarStats dnStats = new TrendChangeBarStats(CurrentBar, Open[0], High[0], Low[0], Close[0], Trend.Down);
                dnHistory.Add(dnHisIndex, dnStats);
                bothHistory.Add(dnHisIndex + upHisIndex, dnStats);

                //Print(String.Format("{0}   [{1}]   {2}",Time[0].ToShortDateString(),Bars.BarsSinceSession,bothHistory[bothHistory.Count - 1].ToString()));
                dnHisIndex++;

                //BackColor = Color.Coral;
            }
            else
            {
                swingLows.RemoveAt(swingLows.Count - 1);
            }
            curLowBar = bar;
            curLow = Math.Round(low, decimalPlaces, MidpointRounding.AwayFromZero);
            curLowLength = Convert.ToInt32(Math.Round((curLow - curHigh) / TickSize, 0, MidpointRounding.AwayFromZero));
            curLowDuration = curLowBar - curHighBar;

            double dtbOffset = ATR(14)[CurrentBar - curLowBar] * dtbStrength / 100;
            if (curLow > lastLow - dtbOffset && curLow < lastLow + dtbOffset)
                curLowRelation = Relation.Double;
            else if (curLow < lastLow)
                curLowRelation = Relation.Lower;
            else
                curLowRelation = Relation.Higher;

            string swingLabel = null;
            switch (curLowRelation)
            {
                case Relation.Higher:
                    swingLabel = "HL";
                    break;
                case Relation.Lower:
                    swingLabel = "LL";
                    break;
                case Relation.Double:
                    swingLabel = "DB";
                    break;
            }
            if (lastHigh != 0.0 || lastLow != 0.0)
            {
                DrawLine("ZigZagDown" + swingCounterDn, true, CurrentBar - curHighBar, curHigh, CurrentBar - curLowBar, curLow, zigZagColourDn, zigZagStyle, zigZagWidth);
                string txtOutput = string.Format("{0}: {1} b.{2}", swingLabel, curLowLength.ToString(), curLowDuration.ToString());
                DrawText("DnLabel" + swingCounterDn, true, txtOutput, CurrentBar - curLowBar, curLow, -offsetText, Color.Black, textFont, StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
            }

            // On new low adjust the breakeven stop to higher low.

            if (swingStopLoss != null)
            {
                //Print (String.Format(" Current StopPrice {0}      lastHigh {1}",swingStopLoss.StopPrice,lastHigh));
                if (swingStopLoss.StopPrice > lastHigh && scalpProfitTarget == null)
                {
                    // Print(Bars.BarsSinceSession+"MOVING THE STOPLOSS");
                    try
                    {
                        if (Close[0] < lastHigh) swingStopLoss = ExitShortStop(0, true, swingStopLoss.Quantity, lastHigh, "SW.HL", "SW.DTEN");
                        else if (Close[0] >= lastHigh) ExitShortStop(0, true, swingStopLoss.Quantity, High[0] + 1 * TickSize, "SW.HL" + " MKT", "SW.DTEN");
                    }
                    catch (Exception ex)
                    {
                        Print("Exception caused by moving swing stop loss position" + Environment.NewLine + ex.ToString());
                    }
                }
            }
            // Print(String.Format("[{0}]    {1}", Bars.BarsSinceSession, lastHigh));

            Swings dn = new Swings(curLowBar, curLow, curLowLength, curLowDuration, curLowRelation);
            swingLows.Add(dn);
            downCount = swingLows.Count - 1;
        }
        private void CalcUpSwing(int bar, double high, bool updateHigh)
        {
            if (!updateHigh)
            {
                lastHigh = curHigh;
                lastHighBar = curHighBar;
                lastHighDuration = curHighDuration;
                lastHighRelation = curHighRelation;
                swingCounterUp++;
                swingSlope = 1;
                trendChangeBar = bar;

                TrendChangeBarStats upStats = new TrendChangeBarStats(CurrentBar, Open[0], High[0], Low[0], Close[0], Trend.Up);
                upHistory.Add(upHisIndex, upStats);
                bothHistory.Add(upHisIndex + dnHisIndex, upStats);
                upHisIndex++;
            }
            else
            {
                swingHighs.RemoveAt(swingHighs.Count - 1);
            }

            curHighBar = bar;
            curHigh = Math.Round(high, decimalPlaces, MidpointRounding.AwayFromZero);
            curHighLength = Convert.ToInt32(Math.Round((curHigh - curLow) / TickSize, 0, MidpointRounding.AwayFromZero));
            curHighDuration = curHighBar - curLowBar;

            double dtbOffset = ATR(14)[CurrentBar - curHighBar] * dtbStrength / 100;
            if (curHigh > lastHigh - dtbOffset && curHigh < lastHigh + dtbOffset)
            {
                curHighRelation = Relation.Double;
                BackColor = Color.Yellow;
                doubleBottomBar = CurrentBar;
                TrendChangeBarStats dtStats = new TrendChangeBarStats(CurrentBar, Open[0], High[0], Low[0], Close[0], Trend.Up);
                dT.Add(dtIndex, dtStats);
                //Print(Environment.NewLine + String.Format("{0}   [{1}]   {2}", Time[0].ToShortDateString(), Bars.BarsSinceSession, bothHistory[bothHistory.Count - 2].ToString()));
                //Print(String.Format("{0}   [{1}]   {2}   DOUBLETOP ALERT", Time[0].ToShortDateString(), Bars.BarsSinceSession, bothHistory[bothHistory.Count - 1].ToString()));
                dtIndex++;
                enableDTOrderEntry = true;
            }
            else if (curHigh < lastHigh)
                curHighRelation = Relation.Lower;
            else
                curHighRelation = Relation.Higher;

            string swingLabel = null;
            switch (curHighRelation)
            {
                case Relation.Higher:
                    swingLabel = "HH";
                    break;
                case Relation.Lower:
                    swingLabel = "LH";
                    break;
                case Relation.Double:
                    swingLabel = "DT";
                    break;
            }

            if (lastHigh != 0.0 || lastLow != 0.0)
            {
                DrawLine("ZigZagUp" + swingCounterUp, true, CurrentBar - curLowBar, curLow, CurrentBar - curHighBar, curHigh, zigZagColourUp, zigZagStyle, zigZagWidth);
                string txtOutput = string.Format("{0}: {1} b.{2}", swingLabel, curHighLength.ToString(), curHighDuration.ToString());
                DrawText("UpLabel" + swingCounterUp, true, txtOutput, CurrentBar - curHighBar, curHigh, offsetText, Color.Black, textFont, StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
            }


            Swings up = new Swings(curHighBar, curHigh, curHighLength, curHighDuration, curHighRelation);
            swingHighs.Add(up);
            upCount = swingHighs.Count - 1;
        }

    }
}


