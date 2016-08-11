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
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace NinjaTrader.Strategy
{
    [Description("Enter the description of your strategy here")]
    public class MarketDataTick : Strategy
    {
        Dictionary<double, long> lastAsk = new Dictionary<double, long>();
        Dictionary<double, long> lastBid = new Dictionary<double, long>();


        SortedDictionary<double, LastTrade> cummLastTrade = new SortedDictionary<double, LastTrade>();

        private List<LadderRow> askRows = new List<LadderRow>();
        private List<LadderRow> bidRows = new List<LadderRow>();

        private List<LadderRowStats> BuyOrder = new List<LadderRowStats>();
        private List<LadderRowStats> SellOrder = new List<LadderRowStats>();
        List<LadderRow> lastSellOrders = null;
        List<LadderRow> lastBuyOrders = null;



        private long askLast = 0;
        private long bidLast = 0;

        private bool firstAskEvent = true;
        private bool firstBidEvent = true;

        public class LastTrade
        {
            public long CumulativeAsk { get; set; }
            public long CumulativeBid { get; set; }
        }

        [Serializable]
        private class LadderRow
        {
            public string MarketMaker;                  // relevant for stocks only
            public double Price;
            public long Volume;

            public LadderRow(double myPrice, long myVolume, string myMarketMaker)
            {
                MarketMaker = myMarketMaker;
                Price = myPrice;
                Volume = myVolume;
            }

            public override string ToString()
            {
                return (string.Format("{0}   {1}   {2}", MarketMaker, Price, Volume));
            }
        }

        private struct LadderRowStats
        {
            public string MarketMaker;                  // relevant for stocks only
            public double Price;
            public long BidVolume;
            public long BidDeltaVolume;
            public long AskVolume;
            public long AskDeltaVolume;
            public TimeSpan BarCompletionTime;
            public long BidCumalativeVolume;
            public long AskCumalativeVoume;

            public LadderRowStats(string mm, double px, long bidVol, long askVol, long bidDelta, long askDelta, long bidCummVol, long askCummVol, TimeSpan bartime)
            {
                MarketMaker = mm;
                Price = px;
                BidVolume = bidVol;
                AskVolume = askVol;
                BidDeltaVolume = bidVol;
                AskDeltaVolume = askDelta;
                BidCumalativeVolume = bidCummVol;
                AskCumalativeVoume = askCummVol;
                BarCompletionTime = bartime;
            }
        }

        protected override void Initialize()
        {
            CalculateOnBarClose = true;
        }

        protected override void OnBarUpdate()
        {
            #region Resting order history on last bar update
            if (bidRows != null)
            {
                //lastBuyOrders = new List<LadderRow>(bidRows);
                //Print("Saved old bidOrders");
            }
            else if (askRows != null)
            {
                // lastSellOrders = new List<LadderRow>(askRows);
            }
            #endregion

            if (Bars.BarsSinceSession == 0)
            {
                lastAsk.Clear();
                lastBid.Clear();
                cummLastTrade.Clear();
            }

            if (true) // Print the Last trade and OrderBook
            {

                //LastTrade
                long cummBidOrders = 0;
                long cummAskOrders = 0;
                long deltaAsk = 0;
                long deltaBid = 0;

                //lastSellOrders = askRows;
                //Order Book
                for (int idx = 0; idx < Math.Min(askRows.Count, bidRows.Count); idx++)
                {
                    cummBidOrders = cummBidOrders + bidRows[idx].Volume;
                    cummAskOrders = cummAskOrders + askRows[idx].Volume;
                    


                    if (lastSellOrders.Count >9 &&  lastBuyOrders.Count >9)
                    {
                        deltaAsk = deltaAsk + (askRows[idx].Volume - lastSellOrders[idx].Volume);
                        deltaBid = deltaBid + (bidRows[idx].Volume - lastBuyOrders[idx].Volume);

                        if (idx == 0)
                        {
                            if (cummLastTrade.ContainsKey(askRows[idx].Price))
                                Print(String.Format("Cumulative Trades at Inside Ask [{0}]    Ask: {1}   Bid: {2}   Last: {3}",
                                    askRows[idx].Price.ToString("0.00"), cummLastTrade[askRows[idx].Price].CumulativeAsk.ToString("D4"), cummLastTrade[askRows[idx].Price].CumulativeBid.ToString("D4"), askLast));
                            if (cummLastTrade.ContainsKey(bidRows[idx].Price))
                                Print(String.Format("Cumulative Trades at Inside Bid [{0}]    Ask: {1}   Bid: {2}   Last: {3}",
                                    bidRows[idx].Price.ToString("0.00"), cummLastTrade[bidRows[idx].Price].CumulativeAsk.ToString("D4"), cummLastTrade[bidRows[idx].Price].CumulativeBid.ToString("D4"), bidLast));
                        }

                        if (lastBuyOrders[0].Price == bidRows[0].Price)
                        {
                            //Print(String.Format("Current    {0}    [{1}] [{2}]    {3}", bidRows[idx].Volume.ToString("D4"), bidRows[idx].Price.ToString("0.00"), askRows[idx].Price.ToString("0.00"), askRows[idx].Volume.ToString("D4")));
                            Print(String.Format("\u0394 ({0})   {1}    [{2}] [{3}]    {4}   \u0394 ({5})",
                                (bidRows[idx].Volume - lastBuyOrders[idx].Volume).ToString("D4"),
                                lastBuyOrders[idx].Volume.ToString("D4"),
                                lastBuyOrders[idx].Price.ToString("0.00"),
                                lastSellOrders[idx].Price.ToString("0.00"),
                                lastSellOrders[idx].Volume.ToString("D4"),
                                (askRows[idx].Volume - lastSellOrders[idx].Volume).ToString("D4")));

                            if (idx==Math.Min(askRows.Count, bidRows.Count)-1)
                            {
                                Print(String.Format("\u0394 ({0})   {1}    CUMALATIVE INFO    {2}   \u0394 ({3})",
                                deltaBid.ToString("D4"),
                                cummBidOrders.ToString("D4"),
                                cummAskOrders.ToString("D4"),
                                deltaAsk.ToString("D4")));
                            }
                        }
                        else
                        {
                            Print("Tier was eatten" + Environment.NewLine);

                        }
                    }
                }
                Print(Environment.NewLine);
                lastSellOrders = DeepClone(askRows);
                lastBuyOrders = DeepClone(bidRows);
            }
            askLast = bidLast = 0;
        }
        protected override void OnMarketData(MarketDataEventArgs e)
        {
            if (e.MarketDataType == MarketDataType.Last)
            {
                if (e.Price == e.MarketData.Ask.Price)
                {
                    askLast = askLast + e.Volume;

                    if (cummLastTrade.ContainsKey(e.Price))
                    {
                        LastTrade updateAskTrade = new LastTrade()
                        {
                            CumulativeAsk = cummLastTrade[e.Price].CumulativeAsk + e.Volume,
                            CumulativeBid = cummLastTrade[e.Price].CumulativeBid
                        };
                        cummLastTrade[e.Price] = updateAskTrade;
                    }
                    else
                    {
                        LastTrade insertAskTrade = new LastTrade()
                        {
                            CumulativeAsk = e.Volume,

                        };
                        cummLastTrade[e.Price] = insertAskTrade;
                    }
                }
                if (e.Price == e.MarketData.Bid.Price)
                {
                    if (cummLastTrade.ContainsKey(e.Price))
                    {
                        bidLast = bidLast + e.Volume;
                        LastTrade updateBidTrade = new LastTrade()
                        {
                            CumulativeBid = cummLastTrade[e.Price].CumulativeBid + e.Volume,
                            CumulativeAsk = cummLastTrade[e.Price].CumulativeAsk
                        };
                        cummLastTrade[e.Price] = updateBidTrade;
                    }
                    else
                    {
                        LastTrade insertBidTrade = new LastTrade()
                        {
                            CumulativeBid = e.Volume,
                        };
                        cummLastTrade[e.Price] = insertBidTrade;
                    }
                }
            }
        }
        protected override void OnMarketDepth(MarketDepthEventArgs e)
        {
            List<LadderRow> rows = null;
            //List<LadderRowStats> restingOrders = null;

            // Checks to see if the Market Data is of the Ask type
            if (e.MarketDataType == MarketDataType.Ask)
            {
                rows = askRows;
                //restingOrders = SellOrder;


                // Due to race conditions, it is possible the first event is an Update operation instead of an Insert. When this happens, populate your Lists via e.MarketDepth first.
                if (firstAskEvent)
                {
                    if (e.Operation == Operation.Update)
                    {
                        // Lock the MarketDepth collection to prevent modification to the collection while we are still processing it
                        lock (e.MarketDepth.Ask)
                        {
                            for (int idx = 0; idx < e.MarketDepth.Ask.Count; idx++)
                            {
                                rows.Add(new LadderRow(e.MarketDepth.Ask[idx].Price, e.MarketDepth.Ask[idx].Volume, e.MarketDepth.Ask[idx].MarketMaker));
                            }
                        }
                    }
                    firstAskEvent = false;
                }
            }

            // Checks to see if the Market Data is of the Bid type
            else if (e.MarketDataType == MarketDataType.Bid)
            {
                rows = bidRows;

                // Due to race conditions, it is possible the first event is an Update operation instead of an Insert. When this happens, populate your Lists via e.MarketDepth first.
                if (firstBidEvent)
                {
                    if (e.Operation == Operation.Update)
                    {
                        // Lock the MarketDepth collection to prevent modification to the collection while we are still processing it
                        lock (e.MarketDepth.Bid)
                        {
                            for (int idx = 0; idx < e.MarketDepth.Bid.Count; idx++)
                                rows.Add(new LadderRow(e.MarketDepth.Bid[idx].Price, e.MarketDepth.Bid[idx].Volume, e.MarketDepth.Bid[idx].MarketMaker));
                        }
                    }
                    firstBidEvent = false;
                }
            }

            if (rows == null)
                return;

            // Checks to see if the action taken was an insertion into the ladder
            if (e.Operation == Operation.Insert)
            {
                // Add a new row at the end if the designated position is greater than our current ladder size
                if (e.Position >= rows.Count)
                    rows.Add(new LadderRow(e.Price, e.Volume, e.MarketMaker));

                // Insert a new row into our ladder at the designated position
                else
                    rows.Insert(e.Position, new LadderRow(e.Price, e.Volume, e.MarketMaker));
            }

            /* Checks to see if the action taken was a removal of itself from the ladder
           Note: Due to the multi threaded architecture of the NT core, race conditions could occur
           -> check if e.Position is within valid range */
            else if (e.Operation == Operation.Remove && e.Position < rows.Count)
                rows.RemoveAt(e.Position);

            /* Checks to see if the action taken was to update a data already on the ladder
           Note: Due to the multi threaded architecture of the NT core, race conditions could occur
           -> check if e.Position is within valid range */
            else if (e.Operation == Operation.Update)
            {
                rows[e.Position].MarketMaker = e.MarketMaker;
                rows[e.Position].Price = e.Price;
                rows[e.Position].Volume = e.Volume;
            }



            //Print(e.ToString());

        }
        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);

            }
        }

    }
}



