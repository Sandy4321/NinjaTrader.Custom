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

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Enter the description of your strategy here
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class BullGapBarSwing : Strategy
    {
        #region Variables
        private string kClass = "BullBearGB.";
        private string path = "C:\\log\\BullBearGapBar.log";
        private int _emaPeriod = 20;
        private int _emaReference = 100;

        private int _barsAboveEMA = 20;
        private int _barBelowEMA = 20;
        bool isTrendUp;

        int _bullBarCounter;
        int _bearBarCounter;

        int _bullBarsSinceSignal = 0;
        int _bearBarsSinceSignal = 0;

        private double _bullProfitTarget = 0;
        private double _bearProfitTarget = 9999;
        private double _lastEntryPrice;
        private double _pullbackPercent = 0.25;
        private double _maxBullMFEPriceStop = 0;
        private double _maxBearMFEPriceStop = 9999;
        private double _lastBullProfitTarget = 0;
        private double _lastBearProfitTarget = 9999;


        private IOrder EnterBullScalpOrder = null;
        private IOrder EnterBullSwingOrder = null;
        private IOrder EnterBearScalpOrder = null;
        private IOrder EnterBearSwingOrder = null;

        private IOrder BullStopLossScalp = null;
        private IOrder BullStopLossSwing = null;
        private IOrder BullTakeProfitScalp = null;
        private IOrder BullTakeProfitSwing = null;

        private IOrder BearStopLossScalp = null;
        private IOrder BearStopLossSwing = null;
        private IOrder BearTakeProfitScalp = null;
        private IOrder BearTakeProfitSwing = null;

        Dictionary<int, GapBarDataSeries> _dictBullGBSeries = new Dictionary<int, GapBarDataSeries>();
        Dictionary<int, GapBarDataSeries> _dictBearGBSeries = new Dictionary<int, GapBarDataSeries>();
        Dictionary<int, EntryDataSeries> _dictBullEntrySwing = new Dictionary<int, EntryDataSeries>();
        Dictionary<int, EntryDataSeries> _dictBearEntrySwing = new Dictionary<int, EntryDataSeries>();
        Dictionary<int, CrossOverEmaPoints> _dictEmaCross = new Dictionary<int, CrossOverEmaPoints>();
        Dictionary<int, GapTrends> _dictGapBar = new Dictionary<int, GapTrends>();
        int _crossIndex = 0;
        int _gapbarIndex = 0;



        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            string kObj = kClass + "Initialize";
            kLog(kObj, "INFO", "Strating the BullBearGapBar Strategy...");

            CalculateOnBarClose = true;
            Add(EMA(EmaPeriod));
            EMA(EmaPeriod).Plots[0].Pen.Color = Color.Orange;


            EntriesPerDirection = 2;
            EntryHandling = EntryHandling.AllEntries;
            ExitOnClose = true;
            ExitOnCloseSeconds = 30;

        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {

            String kObj = kClass + "OnBarUpdate";
            double _maxBullMAE = 0;
            double _maxBullMFE = 0;
            double _maxBearMAE = 0;
            double _maxBearMFE = 0;


            if (Bars.BarsSinceSession == 0)
            {
                kLog(kObj, "INFO", String.Format("Start of session reset"));

                double _startPrimary = EMA(EmaPeriod)[0];
                double _startReference = EMA(_emaReference)[0];

                isTrendUp = (_startPrimary > _startReference) ? true : false;
                kLog(kObj, "INFO", String.Format("Primary EMA started above reference EMA: {0}", isTrendUp));

            }

            if (isTrendUp == true)
            {
                if (CrossBelow(EMA(EmaPeriod), EMA(_emaReference), 1) == true)
                {
                    CrossOverEmaPoints _crossOverSeries = new CrossOverEmaPoints()
                    {
                        cCrossIndex = Bars.BarsSinceSession,
                        cPrimaryTrend = CrossOverEmaPoints.Direction.Down
                    };

                    kLog(kObj, "INFO", String.Format("EMA Crossover bar {0}   trend {1}",
                        _crossOverSeries.cPrimaryTrend.ToString(),
                        _crossOverSeries.cCrossIndex.ToString()));
                    _dictEmaCross.Add(_crossIndex, _crossOverSeries);
                    _crossIndex++;
                    isTrendUp = false;

                }
            }
            else
            {
                if (CrossAbove(EMA(EmaPeriod), EMA(_emaReference), 1) == true)
                {
                    CrossOverEmaPoints _crossOverSeries = new CrossOverEmaPoints()
                    {
                        cCrossIndex = Bars.BarsSinceSession,
                        cPrimaryTrend = CrossOverEmaPoints.Direction.Up
                    };
                    kLog(kObj, "INFO", String.Format("EMA Crossover bar {0}   trend {1}",
                        _crossOverSeries.cPrimaryTrend.ToString(),
                        _crossOverSeries.cCrossIndex.ToString()));
                    _dictEmaCross.Add(_crossIndex, _crossOverSeries);
                    _crossIndex++;
                    isTrendUp = true;

                }

            }

            if (BarsSinceExit("Bull Swing") == 0)
            {
                kLog(kObj, "INFO", "Close price at exit " + Close[0]);
            }
            #region Bull Entry Trailing Stop Logic MFE
            if (Position.MarketPosition == MarketPosition.Long)
            {
                EntryDataSeries _eBullSwing = new EntryDataSeries()
                {
                    eBarSinceEntry = BarsSinceEntry("Bull Swing"),
                    eEntryPx = _lastEntryPrice,
                    eMarketPx = Close[0],
                    eMAE = (Close[0] <= _lastEntryPrice) ? ((_lastEntryPrice - Close[0]) / TickSize * 12.5) : 0,
                    eMFE = (Close[0] >= _lastEntryPrice) ? ((Close[0] - _lastEntryPrice) / TickSize * 12.5) : 0,
                    eBarsSinceSession = Bars.BarsSinceSession
                };
                _dictBullEntrySwing.Add(BarsSinceEntry("Bull Swing"), _eBullSwing);

                foreach (KeyValuePair<int, EntryDataSeries> eKeyValuePair in _dictBullEntrySwing)
                {
                    EntryDataSeries eBullSwing = eKeyValuePair.Value;
                    #region log eBullSwing
                    _maxBullMAE = Math.Max(_maxBullMAE, eBullSwing.eMAE);
                    _maxBullMFE = Math.Max(_maxBullMFE, eBullSwing.eMFE);
                    _maxBullMFEPriceStop = Instrument.MasterInstrument.Round2TickSize(_maxBullMFE / 12.5 * TickSize * _pullbackPercent) + eBullSwing.eEntryPx;
                    #endregion
                }

                EntryDataSeries eBullPrint = _dictBullEntrySwing[_dictBullEntrySwing.Count - 1];
                kLog(kObj, "BULL", String.Format("[e.{0}]   e {1}    mae {2}   mfe {3}   MAE {4}   MFE {5}   stop {6}   diff {7}   b {8}",
                    eBullPrint.eBarSinceEntry,
                    eBullPrint.eEntryPx.ToString("0.00"),
                    eBullPrint.eMAE.ToString("0.00"),
                    eBullPrint.eMFE.ToString("0.00"),
                    _maxBullMAE.ToString("0.00"),
                    _maxBullMFE.ToString("0.00"),
                    (_maxBullMFEPriceStop).ToString("0.00"),
                    (_maxBullMFEPriceStop - _lastEntryPrice).ToString("0.00"),
                    eBullPrint.eBarsSinceSession));
            }
            else
            {
                if (_dictBullEntrySwing != null)
                {
                    _maxBullMFE = 0;
                    _maxBullMAE = 0;
                    _dictBullEntrySwing.Clear();
                }

            }
            #endregion

            #region Bear Entry Trailing Stop Logic MFE
            if (Position.MarketPosition == MarketPosition.Short)
            {
                EntryDataSeries _eBearSwing = new EntryDataSeries()
                {
                    eBarSinceEntry = BarsSinceEntry("Bear Swing"),
                    eEntryPx = _lastEntryPrice,
                    eMarketPx = Close[0],
                    eMAE = (Close[0] >= _lastEntryPrice) ? ((Close[0] - _lastEntryPrice) / TickSize * 12.5) : 0,
                    eMFE = (Close[0] <= _lastEntryPrice) ? ((_lastEntryPrice - Close[0]) / TickSize * 12.5) : 0,
                    eBarsSinceSession = Bars.BarsSinceSession
                };
                _dictBearEntrySwing.Add(BarsSinceEntry("Bear Swing"), _eBearSwing);

                foreach (KeyValuePair<int, EntryDataSeries> eKeyValuePair in _dictBearEntrySwing)
                {
                    EntryDataSeries eBearSwing = eKeyValuePair.Value;
                    #region log eBearSwing
                    _maxBearMAE = Math.Max(_maxBearMAE, eBearSwing.eMAE);
                    _maxBearMFE = Math.Max(_maxBearMFE, eBearSwing.eMFE);
                    _maxBearMFEPriceStop = eBearSwing.eEntryPx - Instrument.MasterInstrument.Round2TickSize(_maxBearMFE / 12.5 * TickSize * _pullbackPercent);
                    #endregion
                }

                EntryDataSeries eBearPrint = _dictBearEntrySwing[_dictBearEntrySwing.Count - 1];
                kLog(kObj, "BEAR", String.Format("[e.{0}]   e {1}    mae {2}   mfe {3}   MAE {4}   MFE {5}   stop {6}   diff {7}   b {8}",
     eBearPrint.eBarSinceEntry,
     eBearPrint.eEntryPx.ToString("0.00"),
     eBearPrint.eMAE.ToString("0.00"),
     eBearPrint.eMFE.ToString("0.00"),
     _maxBearMAE.ToString("0.00"),
     _maxBearMFE.ToString("0.00"),
     (_maxBearMFEPriceStop).ToString("0.00"),
     (_lastEntryPrice - _maxBearMFEPriceStop).ToString("0.00"),
     eBearPrint.eBarsSinceSession));
                /*
                kLog(kObj, "BEAR", String.Format("[e.{0}]   e {1}   c {2}   mae {3}   mfe {4}     maxMAE: {5}   maxMFE {6}   maxMFEStop {7}   bar [ {8} ]",
                    eBearPrint.eBarSinceEntry,
                    eBearPrint.eEntryPx.ToString("0.00"),
                    eBearPrint.eMarketPx.ToString("0.00"),
                    eBearPrint.eMAE.ToString("0.00"),
                    eBearPrint.eMFE.ToString("0.00"),
                    _maxBearMAE.ToString("0.00"),
                    _maxBearMFE.ToString("0.00"),
                    (_maxBearMFEPriceStop).ToString(),
                    eBearPrint.eBarsSinceSession));
                */
            }
            else
            {
                if (_dictBearEntrySwing != null)
                {
                    _maxBearMFE = 0;
                    _maxBearMAE = 0;
                    _dictBearEntrySwing.Clear();
                }

            }
            #endregion

            #region Close position after 10 bars

            if (Position.MarketPosition != MarketPosition.Flat)
            {
                if ((Position.GetProfitLoss(Close[0], PerformanceUnit.Currency) <= 0) && BarsSinceEntry("Bull Scalp") >= 10)
                {
                    ExitLong();
                    _bullProfitTarget = 0;
                }
                if ((Position.GetProfitLoss(Close[0], PerformanceUnit.Currency) <= 0) && BarsSinceEntry("Bull Swing") >= 10)
                {
                    ExitLong();
                    _bullProfitTarget = 0;
                }
                if ((Position.GetProfitLoss(Close[0], PerformanceUnit.Currency) <= 0) && BarsSinceEntry("Bear Scalp") >= 10)
                {
                    ExitShort();
                    _bearProfitTarget = 9999;
                }

                if ((Position.GetProfitLoss(Close[0], PerformanceUnit.Currency) <= 0) && BarsSinceEntry("Bear Swing") >= 10)
                {
                    ExitShort();
                    _bearProfitTarget = 9999;
                }
            }
            #endregion

            //Set swing position to breakeven, after scalp profit target is hit
            if (Position.MarketPosition == MarketPosition.Long && Close[0] >= _lastBullProfitTarget && (BullStopLossScalp != null && BullStopLossScalp.StopPrice < _lastEntryPrice))
            {
                kLog(kObj, "ORDER", String.Format("Adjust Stoploss to breakeven entry price: {0}   and _lastBullProfit {1}", _lastEntryPrice.ToString("0.00"), _lastBullProfitTarget.ToString("0.00")));
                BullStopLossScalp = ExitLongStop(0, true, BullStopLossScalp.Quantity, _lastEntryPrice, "Bull Scalp Stoploss", "Bull Scalp");
            }
            if (Position.MarketPosition == MarketPosition.Long && Close[0] >= _lastBullProfitTarget && (BullStopLossSwing != null && BullStopLossSwing.StopPrice < _lastEntryPrice))
            {
                BullStopLossSwing = ExitLongStop(0, true, BullStopLossSwing.Quantity, _lastEntryPrice, "Bull Swing Stoploss", "Bull Swing");
            }
            if (Position.MarketPosition == MarketPosition.Short && Close[0] <= _lastBearProfitTarget && (BearStopLossScalp != null && BearStopLossScalp.StopPrice > _lastEntryPrice))
            {
                kLog(kObj, "ORDER", String.Format("Adjust Stoploss to breakeven entry price: {0}", _lastEntryPrice));
                BearStopLossScalp = ExitShortStop(0, true, BearStopLossScalp.Quantity, _lastEntryPrice, "Bear Scalp Stoploss", "Bear Scalp");
            }
            if (Position.MarketPosition == MarketPosition.Short && Close[0] <= _lastBearProfitTarget && (BearStopLossSwing != null && BearStopLossSwing.StopPrice > _lastEntryPrice))
            {
                BearStopLossSwing = ExitShortStop(0, true, BearStopLossSwing.Quantity, _lastEntryPrice, "Bear Swing Stoploss", "Bear Swing");
            }

            #region Trailing Stoploss logic

            if (Position.MarketPosition == MarketPosition.Long && (_maxBullMFEPriceStop) > _lastEntryPrice && (BullStopLossSwing != null && BullStopLossSwing.StopPrice < _maxBullMFEPriceStop))
            {
                if (_maxBullMFEPriceStop - _lastEntryPrice >= 1)
                {
                    BullStopLossSwing = ExitLongStop(0, true, BullStopLossSwing.Quantity, _maxBullMFEPriceStop, "Bull Swing Stoploss", "Bull Swing");
                    kLog(kObj, "STOP", String.Format("Move Bull Swing Stoploss to: {0}", _maxBullMFEPriceStop.ToString("0.00")));
                }
            }
            if (Position.MarketPosition == MarketPosition.Short && (_maxBearMFEPriceStop) < _lastEntryPrice && (BearStopLossSwing != null && BearStopLossSwing.StopPrice > _maxBearMFEPriceStop))
            {
                if (_lastEntryPrice - _maxBearMFEPriceStop >= 1)
                {
                    BearStopLossSwing = ExitShortStop(0, true, BearStopLossSwing.Quantity, _maxBearMFEPriceStop, "Bear Swing Stoploss", "Bear Swing");
                    kLog(kObj, "STOP", String.Format("Move Bear Swing Stoploss to: {0}", _maxBearMFEPriceStop.ToString("0.00")));
                }
            }

            #endregion

            if (Bars.BarsSinceSession <= EmaPeriod)
            {
                _bullBarCounter = 0;
                _bearBarCounter = 0;
            }

            #region Bullish case business logic

            if (Low[0] < EMA(EmaPeriod)[0] && _bullBarCounter >= BarsAboveEMA)
            {
                if (Bars.BarsSinceSession == _bullBarCounter)
                {
                    _bullBarCounter = 0;
                    if (_dictBullGBSeries != null) _dictBullGBSeries.Clear();
                }
                if (_bullBarCounter != 0)
                {
                    _bullBarsSinceSignal = Bars.BarsSinceSession - _bullBarsSinceSignal;

                    BackColor = Color.Yellow;
                    //CandleOutlineColor = Color.Chocolate;

                    for (int g = (_bullBarsSinceSignal + BarsAboveEMA); g > 0; g--)
                    {
                        GapBarDataSeries _gbBullData = new GapBarDataSeries()
                        {
                            gOpen = Open[g],
                            gHigh = High[g],
                            gLow = Low[g],
                            gClose = Close[g],
                            gVolume = Volume[g],
                            gEma = EMA(EmaPeriod)[g],
                            gEma100 = EMA(100)[g],
                            gTime = Time[g],
                            gBarNumAtEntry = Bars.BarsSinceSession
                        };
                        _lastBullProfitTarget = Math.Max(_lastBullProfitTarget, _gbBullData.gClose);
                        kLog(kObj, "MAX", String.Format("maxTarget {0}     bar {1}", _lastBullProfitTarget.ToString("0.00"), Bars.BarsSinceSession));
                        _dictBullGBSeries.Add(((_bullBarsSinceSignal + BarsAboveEMA) - g), _gbBullData);
                    }

                    if (_dictBullGBSeries != null)
                    {
                        if (BullProbabilityGauge(_dictBullGBSeries) == 100)
                        {
                            ExitShort("Bear Swing");
                            ExitShort("Bear Scalp");
                            _bearProfitTarget = 9999;
                            EnterBullScalpOrder = EnterLong(1, "Bull Scalp");
                            EnterBullSwingOrder = EnterLong(1, "Bull Swing");
                        }
                        _dictBullGBSeries.Clear();
                    }

                    foreach (KeyValuePair<int, GapBarDataSeries> gbKeyValuePair in _dictBullGBSeries)
                    {
                        GapBarDataSeries cBull = gbKeyValuePair.Value;
                        _bullProfitTarget = Math.Max(_bullProfitTarget, cBull.gClose);
                        #region log _gapBarSeries

                        kLog(kObj, "BULL", String.Format("[{0}]   c {1}   b {2}",
                            gbKeyValuePair.Key,
                            cBull.gClose.ToString("0.00"),
                            cBull.gBarNumAtEntry));

                        #endregion
                    }

            #endregion
                    _bullBarCounter = 0;
                    _bullBarsSinceSignal = 0;
                    kLog(kObj, "BULL", String.Format("Max profit target    [ {0} ]    LastProfitTarget {1}", _bullProfitTarget, _lastBullProfitTarget));
                }
            }

            #region Bearish case business logic

            if (High[0] > EMA(EmaPeriod)[0] && _bearBarCounter >= BarsBelowEMA)
            {
                if (Bars.BarsSinceSession == _bearBarCounter)
                {
                    _bearBarCounter = 0;
                    if (_dictBearGBSeries != null) _dictBearGBSeries.Clear();
                }
                if (_bearBarCounter != 0)
                {
                    _bearBarsSinceSignal = Bars.BarsSinceSession - _bearBarsSinceSignal;

                    BackColor = Color.Yellow;
                    //CandleOutlineColor = Color.Chocolate;


                    for (int g = (_bearBarsSinceSignal + BarsBelowEMA); g > 0; g--)
                    {
                        GapBarDataSeries _gbBearData = new GapBarDataSeries()
                        {
                            gOpen = Open[g],
                            gHigh = High[g],
                            gLow = Low[g],
                            gClose = Close[g],
                            gVolume = Volume[g],
                            gEma = EMA(EmaPeriod)[g],
                            gEma100 = EMA(100)[g],
                            gTime = Time[g],
                            gBarNumAtEntry = Bars.BarsSinceSession
                        };
                        _lastBearProfitTarget = Math.Min(_lastBearProfitTarget, _gbBearData.gClose);
                        kLog(kObj, "MIN", String.Format("minTarget {0}     bar {1}", _lastBearProfitTarget.ToString("0.00"), Bars.BarsSinceSession));
                        _dictBearGBSeries.Add(((_bearBarsSinceSignal + BarsBelowEMA) - g), _gbBearData);

                    }
                    if (_dictBearGBSeries != null)
                    {
                        if (BearProbabilityGauge(_dictBearGBSeries) == 100)
                        {
                            ExitLong("Bull Swing");
                            ExitLong("Bull Scalp");
                            _bullProfitTarget = 0;
                            EnterBearScalpOrder = EnterShort(1, "Bear Scalp");
                            EnterBearSwingOrder = EnterShort(1, "Bear Swing");
                        }
                        _dictBearGBSeries.Clear();
                    }



                    foreach (KeyValuePair<int, GapBarDataSeries> gbKeyValuePair in _dictBearGBSeries)
                    {

                        GapBarDataSeries cBear = gbKeyValuePair.Value;
                        _bearProfitTarget = Math.Min(_bearProfitTarget, cBear.gClose);

                        kLog(kObj, "BEAR", String.Format("[{0}]   c {1}   b {2}",
                            gbKeyValuePair.Key,
                            cBear.gClose.ToString("0.00"),
                            cBear.gBarNumAtEntry));
                    }

                    _bearBarCounter = 0;
                    _bearBarsSinceSignal = 0;
                    kLog(kObj, "Bear", String.Format("Max profit target    [ {0} ]    LastProfitTarget {1}", _bearProfitTarget, _lastBearProfitTarget));
                }
            }
            #endregion


            #region Close position if EMA crossover

            if (Position.MarketPosition != MarketPosition.Flat)                                 // Close position if trend is broken by EMA crossover
            {
                if ((EMA(EmaPeriod)[0] < EMA(100)[0]) && Position.MarketPosition == MarketPosition.Long)
                {
                    ExitLong("Bull Scalp");
                    ExitLong("Bull Swing");
                    _bullProfitTarget = 0;
                }
                else if ((EMA(EmaPeriod)[0] > EMA(100)[0]) && Position.MarketPosition == MarketPosition.Short)
                {
                    ExitShort("Bear Scalp");
                    ExitShort("Bear Swing");
                    _bearProfitTarget = 9999;
                }
            }

            #endregion


            //_bullBarCounter = (Low[0] >= EMA(EmaPeriod)[0]) ? _bullBarCounter + 1 : 0;

            if (Low[0] >= EMA(EmaPeriod)[0])
            {
                _bullBarCounter++;
            }
            else
            {
                _bullBarCounter = 0;
                _dictBullGBSeries.Clear();
            }
            //Bar counting operation

            if (High[0] <= EMA(EmaPeriod)[0])
            {
                _bearBarCounter++;
            }
            else
            {
                _bearBarCounter = 0;
                _dictBearGBSeries.Clear();
            }


            //_bearBarCounter = (High[0] <= EMA(EmaPeriod)[0]) ? _bearBarCounter + 1 : 0;

            if (_bullBarCounter == BarsAboveEMA)                                                 //Set background color and signal bar location
            {
                _bullBarsSinceSignal = Bars.BarsSinceSession;
                BackColor = Color.Aqua;
            }
            else if (_bearBarCounter == BarsBelowEMA)
            {
                _bearBarsSinceSignal = Bars.BarsSinceSession;
                BackColor = Color.LightCoral;
            }
        }
        private int TimeDiffInSeconds(DateTime current, DateTime previous)
        {
            TimeSpan _duration = (current - previous);
            int _timeDiffInSeconds = (_duration.Hours * 3600) + (_duration.Minutes * 60) + (_duration.Seconds);

            return _timeDiffInSeconds;
        }
        private int BullProbabilityGauge(Dictionary<int, GapBarDataSeries> bullBlock)
        {
            string kObj = kClass + "BullProbabilityGauge";

            kLog(kObj, "BULL", String.Format("Close[0] {0}   BullTarget {1}   Diff {2}   Bar {3}",
                Close[0].ToString("0.00"),
                _lastBullProfitTarget.ToString("0.00"),
                (_lastBullProfitTarget - Close[0]).ToString("0.00"),
                Bars.BarsSinceSession));

            //return ((bullBlock.Count < 25) &&
            //  ADX(20)[0] <= 45 &&
            //EMA(200)[0]>EMA(100)[0]) ? 100 : 0;
            //return 100;
            //return (ADX(20)[0] <= 45) ? 100 : 0;

            //if (_dictEmaCross != null && _dictEmaCross[_dictEmaCross.Count - 1].cPrimaryTrend == CrossOverEmaPoints.Direction.Up &&
             //  (Bars.BarsSinceSession-_dictEmaCross[_dictEmaCross.Count - 1].cCrossIndex) <= 10)

            {
                return (_lastBullProfitTarget - Close[0] >= 0.5) ? 100 : 0;
            }
            //return 0;

        }
        private int BearProbabilityGauge(Dictionary<int, GapBarDataSeries> bearBlock)
        {
            string kObj = kClass + "BearProbabilityGauge";
            /*
            return ((bearBlock.Count < 25) &&
                ADX(20)[0] <= 45 &&
                EMA(200)[0] < EMA(100)[0]) ? 100 : 0;
             * 
             *             //return (_lastBearProfitTarget - Close[0] >= 0.25) ? 100 : 0;
            //return (ADX(20)[0] <= 45) ? 100 : 0;
             */

            kLog(kObj, "BEAR", String.Format("Close[0] {0}   BearTarget {1}   Diff {2}   Bar {3}",
                Close[0].ToString("0.00"),
                _lastBearProfitTarget.ToString("0.00"),
                (_lastBearProfitTarget - Close[0]).ToString("0.00"),
                Bars.BarsSinceSession));

           // if (_dictEmaCross != null && _dictEmaCross[_dictEmaCross.Count - 1].cPrimaryTrend == CrossOverEmaPoints.Direction.Down &&
           //     (Bars.BarsSinceSession-_dictEmaCross[_dictEmaCross.Count - 1].cCrossIndex)<=10)
            {
                return (Close[0] - _lastBearProfitTarget >= 0.5) ? 100 : 0;
            }
            //return 0;
        }

        protected override void OnOrderUpdate(IOrder order)
        {
            String kObj = kClass + "OnOrderUpdate";

            #region Bull Order Status logging
            if (EnterBullScalpOrder != null && (EnterBullScalpOrder.OrderState == OrderState.Filled ||
                EnterBullScalpOrder.OrderState == OrderState.PartFilled || (EnterBullScalpOrder.OrderState == OrderState.Cancelled && EnterBullScalpOrder.Filled > 0)))
            {
                kLog(kObj, "TRADE", String.Format("Name: {0}              {1}   Action: {2}    FilledPrice: {3}   Qty: {4}   OrderType: {5}   Status: {6}",
                    EnterBullScalpOrder.Name,
                    EnterBullScalpOrder.Instrument,
                    EnterBullScalpOrder.OrderAction,
                    EnterBullScalpOrder.AvgFillPrice,
                    EnterBullScalpOrder.Filled,
                    EnterBullScalpOrder.OrderType,
                    EnterBullScalpOrder.OrderState
                    ));
            }
            if (EnterBullSwingOrder != null && (EnterBullSwingOrder.OrderState == OrderState.Filled ||
    EnterBullSwingOrder.OrderState == OrderState.PartFilled || (EnterBullSwingOrder.OrderState == OrderState.Cancelled && EnterBullSwingOrder.Filled > 0)))
            {
                kLog(kObj, "TRADE", String.Format("Name: {0}              {1}   Action: {2}    FilledPrice: {3}   Qty: {4}   OrderType: {5}   Status: {6}",
                    EnterBullSwingOrder.Name,
                    EnterBullSwingOrder.Instrument,
                    EnterBullSwingOrder.OrderAction,
                    EnterBullSwingOrder.AvgFillPrice,
                    EnterBullSwingOrder.Filled,
                    EnterBullSwingOrder.OrderType,
                    EnterBullSwingOrder.OrderState
                    ));
            }
            if (BullTakeProfitScalp != null && (BullTakeProfitScalp.OrderState == OrderState.Accepted))
            {
                kLog(kObj, "ORDER", String.Format("Name: {0}   {1}   Action: {2}   PriceLimit: {3}   Qty: {4}   OrderType: {5}   Status: {6}",
                    BullTakeProfitScalp.Name,
                    BullTakeProfitScalp.Instrument,
                    BullTakeProfitScalp.OrderAction,
                    BullTakeProfitScalp.LimitPrice,
                    BullTakeProfitScalp.Quantity,
                    BullTakeProfitScalp.OrderType,
                    BullTakeProfitScalp.OrderState
                    ));
            }
            if (BullTakeProfitSwing != null && (BullTakeProfitSwing.OrderState == OrderState.Accepted))
            {
                kLog(kObj, "ORDER", String.Format("Name: {0}   {1}   Action: {2}   PriceLimit: {3}   Qty: {4}   OrderType: {5}   Status: {6}",
                    BullTakeProfitSwing.Name,
                    BullTakeProfitSwing.Instrument,
                    BullTakeProfitSwing.OrderAction,
                    BullTakeProfitSwing.LimitPrice,
                    BullTakeProfitSwing.Quantity,
                    BullTakeProfitSwing.OrderType,
                    BullTakeProfitSwing.OrderState
                    ));
            }
            if (BullStopLossScalp != null && (BullStopLossScalp.OrderState == OrderState.Accepted))
            {
                kLog(kObj, "ORDER", String.Format("Name: {0}     {1}   Action: {2}   StopPrice:  {3}   Qty: {4}   OrderType: {5}   Status: {6}",
                    BullStopLossScalp.Name,
                    BullStopLossScalp.Instrument,
                    BullStopLossScalp.OrderAction,
                    BullStopLossScalp.StopPrice,
                    BullStopLossScalp.Quantity,
                    BullStopLossScalp.OrderType,
                    BullStopLossScalp.OrderState
                    ));
            }
            if (BullStopLossSwing != null && (BullStopLossSwing.OrderState == OrderState.Accepted))
            {
                kLog(kObj, "ORDER", String.Format("Name: {0}     {1}   Action: {2}   StopPrice:  {3}   Qty: {4}   OrderType: {5}   Status: {6}",
                    BullStopLossSwing.Name,
                    BullStopLossSwing.Instrument,
                    BullStopLossSwing.OrderAction,
                    BullStopLossSwing.StopPrice,
                    BullStopLossSwing.Quantity,
                    BullStopLossSwing.OrderType,
                    BullStopLossSwing.OrderState
                    ));
            }
            #region hide
            /*
            if (BullStopLossSwing != null && (BullStopLossSwing.OrderState == OrderState.Filled ||
    BullStopLossSwing.OrderState == OrderState.PartFilled || (BullStopLossSwing.OrderState == OrderState.Cancelled && BullStopLossSwing.Filled > 0)))
            {
                kLog(kObj, "TRADE", String.Format("Name: {0}              {1}   Action: {2}    FilledPrice: {3}   Qty: {4}   OrderType: {5}   Status: {6}",
                    BullStopLossSwing.Name,
                    BullStopLossSwing.Instrument,
                    BullStopLossSwing.OrderAction,
                    BullStopLossSwing.AvgFillPrice,
                    BullStopLossSwing.Filled,
                    BullStopLossSwing.OrderType,
                    BullStopLossSwing.OrderState
                    ));
            }
            if (BullStopLossScalp != null && (BullStopLossScalp.OrderState == OrderState.Filled ||
BullStopLossScalp.OrderState == OrderState.PartFilled || (BullStopLossScalp.OrderState == OrderState.Cancelled && BullStopLossScalp.Filled > 0)))
            {
                kLog(kObj, "TRADE", String.Format("Name: {0}              {1}   Action: {2}    FilledPrice: {3}   Qty: {4}   OrderType: {5}   Status: {6}",
                    BullStopLossScalp.Name,
                    BullStopLossScalp.Instrument,
                    BullStopLossScalp.OrderAction,
                    BullStopLossScalp.AvgFillPrice,
                    BullStopLossScalp.Filled,
                    BullStopLossScalp.OrderType,
                    BullStopLossScalp.OrderState
                    ));
            }*/
            #endregion

            if (BullTakeProfitScalp != null && BullTakeProfitScalp.OrderState == OrderState.Filled)
            {
                kLog(kObj, "TRADE", order.ToString());
            }
            if (BullTakeProfitSwing != null && (BullTakeProfitSwing.OrderState == OrderState.Filled))
            {
                kLog(kObj, "TRADE", order.ToString());
            }
            if (BullStopLossScalp != null && BullStopLossScalp.OrderState == OrderState.Filled)
            {
                kLog(kObj, "TRADE", order.ToString());
            }
            if (BullStopLossSwing != null && (BullStopLossSwing.OrderState == OrderState.Filled))
            {
                kLog(kObj, "TRADE", order.ToString());

            }
            #endregion
            #region Bear Order Status logging
            if (EnterBearScalpOrder != null && (EnterBearScalpOrder.OrderState == OrderState.Filled ||
                EnterBearScalpOrder.OrderState == OrderState.PartFilled || (EnterBearScalpOrder.OrderState == OrderState.Cancelled && EnterBearScalpOrder.Filled > 0)))
            {
                kLog(kObj, "TRADE", String.Format("Name: {0}              {1}   Action: {2}    FilledPrice: {3}   Qty: {4}   OrderType: {5}   Status: {6}",
                    EnterBearScalpOrder.Name,
                    EnterBearScalpOrder.Instrument,
                    EnterBearScalpOrder.OrderAction,
                    EnterBearScalpOrder.AvgFillPrice,
                    EnterBearScalpOrder.Filled,
                    EnterBearScalpOrder.OrderType,
                    EnterBearScalpOrder.OrderState
                    ));
            }
            if (BearTakeProfitScalp != null && (BearTakeProfitScalp.OrderState == OrderState.Filled ||
                BearTakeProfitScalp.OrderState == OrderState.PartFilled || (BearTakeProfitScalp.OrderState == OrderState.Cancelled && BearTakeProfitScalp.Filled > 0)))
            {
                kLog(kObj, "TRADE", order.ToString());
            }
            if (BearTakeProfitSwing != null && (BearTakeProfitSwing.OrderState == OrderState.Filled ||
BearTakeProfitSwing.OrderState == OrderState.PartFilled || (BearTakeProfitSwing.OrderState == OrderState.Cancelled && BearTakeProfitSwing.Filled > 0)))
            {
                kLog(kObj, "TRADE", order.ToString());
            }
            if (EnterBearSwingOrder != null && (EnterBearSwingOrder.OrderState == OrderState.Filled ||
    EnterBearSwingOrder.OrderState == OrderState.PartFilled || (EnterBearSwingOrder.OrderState == OrderState.Cancelled && EnterBearSwingOrder.Filled > 0)))
            {
                kLog(kObj, "TRADE", String.Format("Name: {0}              {1}   Action: {2}    FilledPrice: {3}   Qty: {4}   OrderType: {5}   Status: {6}",
                    EnterBearSwingOrder.Name,
                    EnterBearSwingOrder.Instrument,
                    EnterBearSwingOrder.OrderAction,
                    EnterBearSwingOrder.AvgFillPrice,
                    EnterBearSwingOrder.Filled,
                    EnterBearSwingOrder.OrderType,
                    EnterBearSwingOrder.OrderState
                    ));
            }
            if (BearTakeProfitScalp != null && (BearTakeProfitScalp.OrderState == OrderState.Accepted))
            {
                kLog(kObj, "ORDER", String.Format("Name: {0}   {1}   Action: {2}   PriceLimit: {3}   Qty: {4}   OrderType: {5}   Status: {6}",
                    BearTakeProfitScalp.Name,
                    BearTakeProfitScalp.Instrument,
                    BearTakeProfitScalp.OrderAction,
                    BearTakeProfitScalp.LimitPrice,
                    BearTakeProfitScalp.Quantity,
                    BearTakeProfitScalp.OrderType,
                    BearTakeProfitScalp.OrderState
                    ));
            }
            if (BearTakeProfitSwing != null && (BearTakeProfitSwing.OrderState == OrderState.Accepted))
            {
                kLog(kObj, "ORDER", String.Format("Name: {0}   {1}   Action: {2}   PriceLimit: {3}   Qty: {4}   OrderType: {5}   Status: {6}",
                    BearTakeProfitSwing.Name,
                    BearTakeProfitSwing.Instrument,
                    BearTakeProfitSwing.OrderAction,
                    BearTakeProfitSwing.LimitPrice,
                    BearTakeProfitSwing.Quantity,
                    BearTakeProfitSwing.OrderType,
                    BearTakeProfitSwing.OrderState
                    ));
            }
            if (BearStopLossScalp != null && (BearStopLossScalp.OrderState == OrderState.Accepted))
            {
                kLog(kObj, "ORDER", String.Format("Name: {0}     {1}   Action: {2}   StopPrice:  {3}   Qty: {4}   OrderType: {5}   Status: {6}",
                    BearStopLossScalp.Name,
                    BearStopLossScalp.Instrument,
                    BearStopLossScalp.OrderAction,
                    BearStopLossScalp.StopPrice,
                    BearStopLossScalp.Quantity,
                    BearStopLossScalp.OrderType,
                    BearStopLossScalp.OrderState
                    ));
            }
            if (BearStopLossSwing != null && (BearStopLossSwing.OrderState == OrderState.Accepted))
            {
                kLog(kObj, "ORDER", String.Format("Name: {0}     {1}   Action: {2}   StopPrice:  {3}   Qty: {4}   OrderType: {5}   Status: {6}",
                    BearStopLossSwing.Name,
                    BearStopLossSwing.Instrument,
                    BearStopLossSwing.OrderAction,
                    BearStopLossSwing.StopPrice,
                    BearStopLossSwing.Quantity,
                    BearStopLossSwing.OrderType,
                    BearStopLossSwing.OrderState
                    ));
            }
            #region hide2
            /*
            if (BearStopLossSwing != null && (BearStopLossSwing.OrderState == OrderState.Filled ||
    BearStopLossSwing.OrderState == OrderState.PartFilled || (BearStopLossSwing.OrderState == OrderState.Cancelled && BearStopLossSwing.Filled > 0)))
            {
                kLog(kObj, "TRADE", String.Format("Name: {0}              {1}   Action: {2}    FilledPrice: {3}   Qty: {4}   OrderType: {5}   Status: {6}",
                    BearStopLossSwing.Name,
                    BearStopLossSwing.Instrument,
                    BearStopLossSwing.OrderAction,
                    BearStopLossSwing.AvgFillPrice,
                    BearStopLossSwing.Filled,
                    BearStopLossSwing.OrderType,
                    BearStopLossSwing.OrderState
                    ));
            }
            if (BearStopLossScalp != null && (BearStopLossScalp.OrderState == OrderState.Filled ||
BearStopLossScalp.OrderState == OrderState.PartFilled || (BearStopLossScalp.OrderState == OrderState.Cancelled && BearStopLossScalp.Filled > 0)))
            {
                kLog(kObj, "TRADE", String.Format("Name: {0}              {1}   Action: {2}    FilledPrice: {3}   Qty: {4}   OrderType: {5}   Status: {6}",
                    BearStopLossScalp.Name,
                    BearStopLossScalp.Instrument,
                    BearStopLossScalp.OrderAction,
                    BearStopLossScalp.AvgFillPrice,
                    BearStopLossScalp.Filled,
                    BearStopLossScalp.OrderType,
                    BearStopLossScalp.OrderState
                    ));
            }*/
            #endregion
        }
        protected override void OnExecution(IExecution execution)
        {
            String kObj = kClass + "OnExecution";
            #region Bull Stop Loss Logic

            if (EnterBullScalpOrder != null && EnterBullScalpOrder == execution.Order)
            {
                _lastEntryPrice = EnterBullScalpOrder.AvgFillPrice;
                if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
                    BullStopLossScalp = ExitLongStop(0, true, execution.Order.Filled, execution.Order.AvgFillPrice - 8 * TickSize, "Bull Scalp Stoploss", "Bull Scalp");

            }
            if (EnterBullSwingOrder != null && EnterBullSwingOrder == execution.Order)
            {
                if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
                    BullStopLossSwing = ExitLongStop(0, true, execution.Order.Filled, execution.Order.AvgFillPrice - 8 * TickSize, "Bull Swing Stoploss", "Bull Swing");
            }
            #endregion

            #region Bull profit target logic

            if (EnterBullScalpOrder != null && EnterBullScalpOrder == execution.Order)
            {
                if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
                {
                    BullTakeProfitScalp = ExitLongLimit(0, true, execution.Order.Filled, _lastBullProfitTarget, "Bull Scalp Takeprofit", "Bull Scalp");
                }
            }
            if (EnterBullSwingOrder != null && EnterBullSwingOrder == execution.Order)
            {
                if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
                {
                    BullTakeProfitSwing = ExitLongLimit(0, true, execution.Order.Filled, execution.Order.AvgFillPrice + 16 * TickSize, "Bull Swing Takeprofit", "Bull Swing");
                }
            }

            #endregion

            #region OCO Stoploss takeprofit logic

            if ((BullStopLossScalp != null && BullStopLossScalp == execution.Order) || (BullTakeProfitScalp != null && BullTakeProfitScalp == execution.Order))
            {
                if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled)
                {
                    BullStopLossScalp = null;
                    BullTakeProfitScalp = null;
                }
            }
            if ((BullStopLossSwing != null && BullStopLossSwing == execution.Order) || (BullTakeProfitSwing != null && BullTakeProfitSwing == execution.Order))
            {
                if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled)
                {
                    BullStopLossSwing = null;
                    BullTakeProfitSwing = null;
                    _maxBullMFEPriceStop = 0;
                    _lastBullProfitTarget = 0;
                }
            }
            #endregion

            #region reset entry order
            if (EnterBullScalpOrder != null && EnterBullScalpOrder == execution.Order)
            {
                if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
                {
                    if (execution.Order.OrderState != OrderState.PartFilled)
                        EnterBullScalpOrder = null;

                }
            }
            if (EnterBullSwingOrder != null && EnterBullSwingOrder == execution.Order)
            {
                if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
                {
                    if (execution.Order.OrderState != OrderState.PartFilled)
                        EnterBullSwingOrder = null;

                }
            }

            #endregion

            #region Bear Stop loss logic

            if (EnterBearScalpOrder != null && EnterBearScalpOrder == execution.Order)
            {
                _lastEntryPrice = EnterBearScalpOrder.AvgFillPrice;
                if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
                    BearStopLossScalp = ExitShortStop(0, true, execution.Order.Filled, execution.Order.AvgFillPrice + 8 * TickSize, "Bear Scalp Stoploss", "Bear Scalp");

            }
            if (EnterBearSwingOrder != null && EnterBearSwingOrder == execution.Order)
            {
                if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
                    BearStopLossSwing = ExitShortStop(0, true, execution.Order.Filled, execution.Order.AvgFillPrice + 8 * TickSize, "Bear Swing Stoploss", "Bear Swing");
            }
            #endregion

            #region Bear profit target logic

            if (EnterBearSwingOrder != null && EnterBearSwingOrder == execution.Order)
            {
                if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
                {
                    BearTakeProfitScalp = ExitShortLimit(0, true, execution.Order.Filled, _lastBearProfitTarget, "Bear Scalp Takeprofit", "Bear Scalp");
                }
            }
            if (EnterBearSwingOrder != null && EnterBearSwingOrder == execution.Order)
            {
                if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
                    BearTakeProfitScalp = ExitShortLimit(0, true, execution.Order.Filled, execution.Order.AvgFillPrice - 16 * TickSize, "Bear Swing Takeprofit", "Bear Swing");
            }

            #endregion

            #region OCO Stoploss takeprofit logic

            if ((BearStopLossScalp != null && BearStopLossScalp == execution.Order) || (BearTakeProfitScalp != null && BearTakeProfitScalp == execution.Order))
            {
                if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled)
                {
                    BearStopLossScalp = null;
                    BearTakeProfitScalp = null;
                }
            }
            if ((BearStopLossSwing != null && BearStopLossSwing == execution.Order) || (BearStopLossSwing != null && BearStopLossSwing == execution.Order))
            {
                if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled)
                {
                    BearStopLossSwing = null;
                    BearTakeProfitSwing = null;
                    _maxBearMFEPriceStop = 9999;
                    _lastBearProfitTarget = 9999;
                }
            }
            #endregion

            #region reset entry order
            if (EnterBearScalpOrder != null && EnterBearScalpOrder == execution.Order)
            {
                if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
                {
                    if (execution.Order.OrderState != OrderState.PartFilled)
                        EnterBearScalpOrder = null;

                }
            }
            if (EnterBearSwingOrder != null && EnterBearSwingOrder == execution.Order)
            {
                if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
                {
                    if (execution.Order.OrderState != OrderState.PartFilled)
                        EnterBearSwingOrder = null;

                }
            }

            #endregion

        }
        protected override void OnTermination()
        {
            File.WriteAllText(path, string.Empty);
            base.OnTermination();
        }
        private void kLog(string ClassMethod, string MsgType, string Msg)
        {
            string _msgOut = (String.Format("{0}  {1}    {2}    [{3}]    {4}",
                Time[0].ToString("dd/MM/yyyy"),
                Time[0].ToString("HH:mm:ss"),
                MsgType,
                ClassMethod,
                Msg)) + Environment.NewLine;
            File.AppendAllText(path, _msgOut);
        }
        #region Properties
        [Description("EMA Period")]
        [GridCategory("Parameters")]
        public int EmaPeriod
        {
            get { return _emaPeriod; }
            set { _emaPeriod = value; }
        }
        [Description("Bars above EMA required")]
        [GridCategory("Parameters")]
        public int BarsAboveEMA
        {
            get { return _barsAboveEMA; }
            set { _barsAboveEMA = value; }
        }
        [Description("Bars below EMA required")]
        [GridCategory("Parameters")]
        public int BarsBelowEMA
        {
            get { return _barBelowEMA; }
            set { _barBelowEMA = value; }
        }
        #endregion
    }
    public class GapBarDataSeries
    {
        //public int gBarIndex { get; set; }
        public int gBarNumAtEntry { get; set; }
        public double gOpen { get; set; }
        public double gHigh { get; set; }
        public double gLow { get; set; }
        public double gClose { get; set; }
        public double gVolume { get; set; }
        public double gEma { get; set; }
        public double gEma100 { get; set; }
        public DateTime gTime { get; set; }
        public double gSlope { get; set; }
        public double gMaxTarget { get; set; }
    }
    public class EntryDataSeries
    {
        public int eBarSinceEntry { get; set; }
        public double eEntryPx { get; set; }
        public double eMarketPx { get; set; }
        public double eMAE { get; set; }
        public double eMFE { get; set; }
        public int eBarsSinceSession { get; set; }
    }
    public class CrossOverEmaPoints
    {
        public int cCrossIndex { get; set; }
        public int cNearEmaIndex { get; set; }
        public Direction cPrimaryTrend { get; set; }
        public double cPrimaryPx { get; set; }
        public double cReferencePx { get; set; }

        public enum Direction
        {
            Up,
            Down,
        }
    }
    public class GapTrends
    {
        public int cGapBarIndex { get; set; }
        public Direction cPrimaryTrend { get; set; }
        public enum Direction
        {
            Up,
            Down,
        }
    }
}







            #endregion

