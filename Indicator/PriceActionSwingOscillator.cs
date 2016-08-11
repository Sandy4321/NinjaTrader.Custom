// #############################################################
// #														   #
// #                PriceActionSwingOscillator                 #
// #														   #
// #     24.09.2012 by dorschden, die.unendlichkeit@gmx.de     #
// #														   #
// #############################################################
// 

#region Updates
// Version 15 - 24.09.2012 - Initial public release
// - Include PriceActionSwingTrend
// - Add developing swing volume (repainting)
#endregion

#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using PriceActionSwingOscillator.Utility;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// PriceActionSwingOscillator shows the trend direction, swing relation or developing swing volume. !!! The volume is repainting. !!!
    /// </summary>
    [Description("PriceActionSwingOscillator shows the trend direction, swing relation or developing swing volume. !!! The volume is repainting. !!!")]
    public class PriceActionSwingOscillator : Indicator
    {
        #region Variables
        //#####################################################################
        #region Parameters
        //=====================================================================
        private int swingSize = 7;
        private SwingTypes swingType = SwingTypes.Standard;
        private int dtbStrength = 15;
        //=====================================================================
        #endregion

        #region DataSeries
        // Public =============================================================
        private DataSeries swingTrend;
        private DataSeries swingRelation;
        private DataSeries volumeHighSeries;
        private DataSeries volumeLowSeries;
        //=====================================================================
        #endregion

        #region Enums
        //=====================================================================
        public enum BarType
        {
            UpBar,
            DnBar,
            InsideBar,
            OutsideBar,
        }
        public enum Relation
        {
            Double,
            Higher,
            Lower,
        }
        public enum Show
        {
            Trend,
            Relation,
            Volume,
        }
        //=====================================================================
        #endregion

        #region Features
        //=====================================================================
        private bool ignoreSwings = true;
        //=====================================================================
        #endregion

        #region Gann Swings
        // Bars ===============================================================
        private BarType barType = BarType.InsideBar;
        private int consecutiveBars = 0;
        private int consecutiveBarNumber = 0;
        private double consecutiveBarValue = 0;
        private Color dnBarColour = Color.Red;
        private Color upBarColour = Color.Lime;
        private Color insideBarColour = Color.Black;
        private Color outsideBarColour = Color.Blue;
        // Swings =============================================================
        private bool useBreakouts = true;
        private bool ignoreInsideBars = true;
        //=====================================================================
        private bool stopOutsideBarCalc = false;
        //=====================================================================
        #endregion

        #region Settings
        // Bars ===============================================================
        private Show showOscillator = Show.Volume;
        private bool useOldTrend = true;
        private int oldTrend = 0;
        //=====================================================================
        #endregion

        #region Swing values
        // Swing calculation ======================================================================
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
        private int trendChangeBar = 0;
        private int swingSlope = 0;
        // Swing values ===========================================================================
        private double curHigh = 0.0;
        private int curHighBar = 0;
        private Relation curHighRelation = Relation.Higher;
        private double curLow = 0.0;
        private int curLowBar = 0;
        private Relation curLowRelation = Relation.Higher;
        private double lastHigh = 0.0;
        private int lastHighBar = 0;
        private Relation lastHighRelation = Relation.Higher;
        private double lastLow = 0.0;
        private int lastLowBar = 0;
        private Relation lastLowRelation = Relation.Higher;
        //=========================================================================================
        #endregion
        //#####################################################################
        #endregion

        #region Initialize()
        //###################################################################
        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(new Pen(Color.Firebrick, 20), PlotStyle.Square, "RTDoubleTop"));
            Add(new Plot(new Pen(Color.Red, 20), PlotStyle.Square, "RTDownTrend"));
            Add(new Plot(new Pen(Color.Gold, 20), PlotStyle.Square, "RTNoWhere"));
            Add(new Plot(new Pen(Color.Green, 20), PlotStyle.Square, "RTUpTrend"));
            Add(new Plot(new Pen(Color.Lime, 20), PlotStyle.Square, "RTDoubleBottom"));

            Add(new Plot(new Pen(Color.Lime, 2), PlotStyle.Bar, "VHigh"));
            Add(new Plot(new Pen(Color.Red, 2), PlotStyle.Bar, "VLow"));
            Add(new Plot(new Pen(Color.Lime, 2), PlotStyle.Square, "VHighCurrent"));
            Add(new Plot(new Pen(Color.Red, 2), PlotStyle.Square, "VLowCurrent"));

            AutoScale = true;
            BarsRequired = 2;
            CalculateOnBarClose = true;
            Overlay = false;
            PriceTypeSupported = false;

            swingTrend = new DataSeries(this);
            swingRelation = new DataSeries(this);
            volumeHighSeries = new DataSeries(this);
            volumeLowSeries = new DataSeries(this);
        }
        //###################################################################
        #endregion

        #region OnStartUp()
        //#####################################################################
        /// <summary>
        /// This method is used to initialize any variables or resources. 
        /// This method is called only once immediately prior to the start of the script, 
        /// but after the Initialize() method.
        /// </summary>
        protected override void OnStartUp()
        {
            decimal increment = Convert.ToDecimal(Instrument.MasterInstrument.TickSize);
            int incrementLength = increment.ToString().Length;
            decimalPlaces = 0;
            if (incrementLength == 1)
                decimalPlaces = 0;
            else if (incrementLength > 2)
                decimalPlaces = incrementLength - 2;
        }
        //#####################################################################
        #endregion

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            #region Initialize varibles
            //=================================================================
            if (FirstTickOfBar)
            {
                stopOutsideBarCalc = false;

                if (CurrentBar == 1)
                {
                    curHighBar = curLowBar = CurrentBar;
                    curHigh = High[1];
                    curLow = Low[1];
                }
                volumeHighSeries.Set(0);
                volumeLowSeries.Set(0);
            }
            // Set new/update high/low back to false, to avoid function 
            // calls which depends on them
            newHigh = newLow = updateHigh = updateLow = false;

            // Checks to make sure we have enough bars to calculate the indicator   
            if (CurrentBar < swingSize)
                return;
            //=================================================================
            #endregion

            #region Swing calculation
            // Swing calculation ==============================================
            switch (swingType)
            {
                case SwingTypes.Standard:
                    #region Standard Swing
                    if (swingSlope == 1 && High[0] <= curHigh)
                        newHigh = false;
                    else
                        newHigh = true;

                    if (swingSlope == -1 && Low[0] >= curLow)
                        newLow = false;
                    else
                        newLow = true;

                    // CalculatOnBarClose == true =================================================
                    if (CalculateOnBarClose)
                    {
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
                            if (swingSlope == -1)
                                newHigh = false;
                            else
                                newLow = false;
                        }
                    }
                    // CalculatOnBarClose == false ================================================
                    else
                    {
                        if (FirstTickOfBar)
                            newSwing = 0;

                        if (newSwing != -1)
                        {
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
                                if (newHigh)
                                    newSwing = 1;
                            }
                        }

                        if (newSwing != 1)
                        {
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
                                if (newLow)
                                    newSwing = -1;
                            }
                        }

                        if (newSwing == 1)
                            newLow = false;
                        if (newSwing == -1)
                            newHigh = false;
                    }

                    // Set new high or low ========================================================
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
                    // ============================================================================
                    #endregion
                    break;
                case SwingTypes.Gann:
                    #region Set bar property
                    //=================================================================
                    if (High[0] > High[1])
                    {
                        if (Low[0] < Low[1])
                            barType = BarType.OutsideBar;
                        else
                            barType = BarType.UpBar;
                    }
                    else
                    {
                        if (Low[0] < Low[1])
                            barType = BarType.DnBar;
                        else
                            barType = BarType.InsideBar;
                    }
                    //===================================================================
                    #endregion

                    #region Gann Swing
                    // Gann swing =================================================================
                    #region Up swing
                    // Up Swing ===================================================================
                    if (swingSlope == 1)
                    {
                        switch (barType)
                        {
                            // Up bar =============================================================
                            case BarType.UpBar:
                                consecutiveBars = 0;
                                consecutiveBarValue = 0.0;
                                if (High[0] > curHigh)
                                {
                                    updateHigh = true;
                                    CalcUpSwing(CurrentBar, High[0], true);
                                    if ((consecutiveBars + 1) == swingSize)
                                        stopOutsideBarCalc = true;
                                }
                                break;
                            // Down bar ===========================================================
                            case BarType.DnBar:
                                if (consecutiveBarNumber != CurrentBar)
                                {
                                    if (consecutiveBarValue == 0.0)
                                    {
                                        consecutiveBars++;
                                        consecutiveBarNumber = CurrentBar;
                                        consecutiveBarValue = Low[0];
                                    }
                                    else if (Low[0] < consecutiveBarValue)
                                    {
                                        consecutiveBars++;
                                        consecutiveBarNumber = CurrentBar;
                                        consecutiveBarValue = Low[0];
                                    }
                                }
                                else if (Low[0] < consecutiveBarValue)
                                    consecutiveBarValue = Low[0];
                                if (consecutiveBars == swingSize || (useBreakouts && Low[0] < curLow))
                                {
                                    consecutiveBars = 0;
                                    consecutiveBarValue = 0.0;
                                    newLow = true;
                                    bar = CurrentBar - LowestBar(Low, CurrentBar - curHighBar);
                                    price = Low[LowestBar(Low, CurrentBar - curHighBar)];
                                    CalcDnSwing(bar, price, false);
                                }
                                break;
                            // Inside bar =========================================================
                            case BarType.InsideBar:
                                if (!ignoreInsideBars)
                                {
                                    consecutiveBars = 0;
                                    consecutiveBarValue = 0.0;
                                }
                                break;
                            // Outside bar ========================================================
                            case BarType.OutsideBar:
                                if (High[0] > curHigh)
                                {
                                    updateHigh = true;
                                    CalcUpSwing(CurrentBar, High[0], true);
                                }
                                else if (!stopOutsideBarCalc)
                                {
                                    if (consecutiveBarNumber != CurrentBar)
                                    {
                                        if (consecutiveBarValue == 0.0)
                                        {
                                            consecutiveBars++;
                                            consecutiveBarNumber = CurrentBar;
                                            consecutiveBarValue = Low[0];
                                        }
                                        else if (Low[0] < consecutiveBarValue)
                                        {
                                            consecutiveBars++;
                                            consecutiveBarNumber = CurrentBar;
                                            consecutiveBarValue = Low[0];
                                        }
                                    }
                                    else if (Low[0] < consecutiveBarValue)
                                        consecutiveBarValue = Low[0];
                                    if (consecutiveBars == swingSize || (useBreakouts && Low[0] < curLow))
                                    {
                                        consecutiveBars = 0;
                                        consecutiveBarValue = 0.0;
                                        newLow = true;
                                        bar = CurrentBar - LowestBar(Low, CurrentBar - curHighBar);
                                        price = Low[LowestBar(Low, CurrentBar - curHighBar)];
                                        CalcDnSwing(bar, price, false);
                                    }
                                }
                                break;
                        }
                    }
                    // End up swing ===============================================================
                    #endregion

                    #region Down swing
                    // Down swing =================================================================
                    else
                    {
                        switch (barType)
                        {
                            // Dwon bar ===========================================================
                            case BarType.DnBar:
                                consecutiveBars = 0;
                                consecutiveBarValue = 0.0;
                                if (Low[0] < curLow)
                                {
                                    updateLow = true;
                                    CalcDnSwing(CurrentBar, Low[0], true);
                                    if ((consecutiveBars + 1) == swingSize)
                                        stopOutsideBarCalc = true;
                                }
                                break;
                            // Up bar =============================================================
                            case BarType.UpBar:
                                if (consecutiveBarNumber != CurrentBar)
                                {
                                    if (consecutiveBarValue == 0.0)
                                    {
                                        consecutiveBars++;
                                        consecutiveBarNumber = CurrentBar;
                                        consecutiveBarValue = High[0];
                                    }
                                    else if (High[0] > consecutiveBarValue)
                                    {
                                        consecutiveBars++;
                                        consecutiveBarNumber = CurrentBar;
                                        consecutiveBarValue = High[0];
                                    }
                                }
                                else if (High[0] > consecutiveBarValue)
                                    consecutiveBarValue = High[0];
                                if (consecutiveBars == swingSize || (useBreakouts && High[0] > curHigh))
                                {
                                    consecutiveBars = 0;
                                    consecutiveBarValue = 0.0;
                                    newHigh = true;
                                    bar = CurrentBar - HighestBar(High, CurrentBar - curLowBar);
                                    price = High[HighestBar(High, CurrentBar - curLowBar)];
                                    CalcUpSwing(bar, price, false);
                                }
                                break;
                            // Inside bar =========================================================
                            case BarType.InsideBar:
                                if (!ignoreInsideBars)
                                {
                                    consecutiveBars = 0;
                                    consecutiveBarValue = 0.0;
                                }
                                break;
                            // Outside bar ========================================================
                            case BarType.OutsideBar:
                                if (Low[0] < curLow)
                                {
                                    updateLow = true;
                                    CalcDnSwing(CurrentBar, Low[0], true);
                                }
                                else if (!stopOutsideBarCalc)
                                {
                                    if (consecutiveBarNumber != CurrentBar)
                                    {
                                        if (consecutiveBarValue == 0.0)
                                        {
                                            consecutiveBars++;
                                            consecutiveBarNumber = CurrentBar;
                                            consecutiveBarValue = High[0];
                                        }
                                        else if (High[0] > consecutiveBarValue)
                                        {
                                            consecutiveBars++;
                                            consecutiveBarNumber = CurrentBar;
                                            consecutiveBarValue = High[0];
                                        }
                                    }
                                    else if (High[0] > consecutiveBarValue)
                                        consecutiveBarValue = High[0];
                                    if (consecutiveBars == swingSize || (useBreakouts && High[0] > curHigh))
                                    {
                                        consecutiveBars = 0;
                                        consecutiveBarValue = 0.0;
                                        newHigh = true;
                                        bar = CurrentBar - HighestBar(High, CurrentBar - curLowBar);
                                        price = High[HighestBar(High, CurrentBar - curLowBar)];
                                        CalcUpSwing(bar, price, false);
                                    }
                                }
                                break;
                        }
                    }
                    // End down swing =============================================================
                    #endregion
                    // End Gann swing =============================================================
                    #endregion
                    break;
            }
            // End swing calculation ==========================================
            #endregion

            #region Swing trend
            if ((curHighRelation == Relation.Higher && curLowRelation == Relation.Higher)
                    || (swingTrend[1] == 1 && swingSlope == 1)
                    || (ignoreSwings && swingTrend[1] == 1 && curLowRelation == Relation.Higher)
                    || (((swingTrend[1] == 1) || (swingSlope == 1 && curHighRelation == Relation.Higher))
                        && curLowRelation == Relation.Double))
                swingTrend.Set(1);
            else if ((curHighRelation == Relation.Lower && curLowRelation == Relation.Lower)
                    || (swingTrend[1] == -1 && swingSlope == -1)
                    || (ignoreSwings && swingTrend[1] == -1 && curHighRelation == Relation.Lower)
                    || (((swingTrend[1] == -1) || (swingSlope == -1 && curLowRelation == Relation.Lower))
                        && curHighRelation == Relation.Double))
                swingTrend.Set(-1);
            else
                swingTrend.Set(0);
            #endregion

            #region Swing relation
            if (curLowRelation == Relation.Double)
                swingRelation.Set(2);
            else if (curHighRelation == Relation.Double)
                swingRelation.Set(-2);
            else if (curHighRelation == Relation.Higher && curLowRelation == Relation.Higher)
                swingRelation.Set(1);
            else if (curHighRelation == Relation.Lower && curLowRelation == Relation.Lower)
                swingRelation.Set(-1);
            else
                swingRelation.Set(0);
            #endregion      
            
            #region Draw
            //=====================================================================
            switch (showOscillator)
            {
                case Show.Trend:
                    #region Trend
                    int trend = Convert.ToInt32(swingTrend[0]);
                    switch (trend)
                    {
                        case -1:
                            RTDownTrend.Set(1);
                            oldTrend = -1;
                            break;
                        case 1:
                            oldTrend = 1;
                            RTUpTrend.Set(1);
                            break;
                        default:
                            if (useOldTrend)
                            {
                                if (oldTrend == 1)
                                    RTDownTrend.Set(1);
                                else if (oldTrend == -1)
                                    RTUpTrend.Set(1);
                            }
                            else
                                RTNoWhere.Set(1);
                            break;
                    }
                    #endregion
                    break;
                case Show.Relation:
                    #region Relation
                    int relation = Convert.ToInt32(swingRelation[0]);
                    switch (relation)
                    {
                        case -2:
                            RTDoubleTop.Set(relation);
                            break;
                        case -1:
                            RTDownTrend.Set(relation);
                            break;
                        case 0:
                            RTNoWhere.Set(relation);
                            break;
                        case 1:
                            RTUpTrend.Set(relation);
                            break;
                        case 2:
                            RTDoubleBottom.Set(relation);
                            break;
                        default:
                            RTNoWhere.Set(relation);
                            break;
                    }
                    #endregion
                    break;
                case Show.Volume:
                    #region Volume
                    if (newHigh == true)
                    {
                        if (updateHigh == false)
                        {
                            double swingVolume = 0.0;
                            for (int i = CurrentBar - curLowBar - 1; i > -1; i--)
                            {
                                swingVolume = swingVolume + Volume[i];
                                volumeHighSeries.Set(i, swingVolume);
                                volumeLowSeries.Set(i, 0);

                                VLow.Reset(i);
                                VHighCurrent.Reset(i);
                                VHigh.Set(i, swingVolume);
                            }
                            swingVolume = 0.0;
                            for (int i = CurrentBar - curHighBar - 1; i > -1; i--)
                            {
                                swingVolume = swingVolume + Volume[i];
                                volumeLowSeries.Set(i, swingVolume);
                                VLowCurrent.Set(i, swingVolume);
                            }
                        }
                        else
                        {
                            double tmp = VolumeHighSeries[1] + Volume[0];
                            VolumeHighSeries.Set(tmp);
                            for (int i = CurrentBar - curLowBar - 1; i > -1; i--)
                            {
                                VLowCurrent.Reset(i);
                                volumeLowSeries.Set(i, 0);
                            }
                            VHigh.Set(tmp);
                        }
                    }
                    else if (newLow == true)
                    {
                        if (updateLow == false)
                        {
                            double swingVolume = 0.0;
                            for (int i = CurrentBar - curHighBar - 1; i > -1; i--)
                            {
                                swingVolume = swingVolume + Volume[i];
                                volumeLowSeries.Set(i, swingVolume);
                                volumeHighSeries.Set(i, 0);

                                VHigh.Reset(i);
                                VLowCurrent.Reset(i);
                                VLow.Set(i, swingVolume);
                            }
                            swingVolume = 0.0;
                            for (int i = CurrentBar - curLowBar - 1; i > -1; i--)
                            {
                                swingVolume = swingVolume + Volume[i];
                                volumeHighSeries.Set(i, swingVolume);
                                VHighCurrent.Set(i, swingVolume);
                            }
                        }
                        else
                        {
                            double tmp = VolumeLowSeries[1] + Volume[0];
                            VolumeLowSeries.Set(tmp);
                            for (int i = CurrentBar - curHighBar - 1; i > -1; i--)
                            {
                                VHighCurrent.Reset(i);
                                volumeHighSeries.Set(i, 0);
                            }
                            VLow.Set(tmp);
                        }
                    }
                    else
                    {
                        double tmpH = VolumeHighSeries[1] + Volume[0];
                        double tmpL = VolumeLowSeries[1] + Volume[0];
                        VolumeHighSeries.Set(tmpH);
                        volumeLowSeries.Set(tmpL);

                        if (swingSlope == -1)
                        {
                            VLow.Set(tmpL);
                            VHighCurrent.Set(tmpH);
                        }
                        else
                        {
                            VHigh.Set(tmpH);
                            VLowCurrent.Set(tmpL);
                        }

                    }
                    #endregion
                    break;
            }
            //=====================================================================
            #endregion
        }

        #region CalcDnSwing(int bar, double low, bool updateLow)
        //#####################################################################
        private void CalcDnSwing(int bar, double low, bool updateLow)
        {
            if (!updateLow)
            {
                lastLow = curLow;
                lastLowBar = curLowBar;
                lastLowRelation = curLowRelation;
                swingCounterDn++;
                swingSlope = -1;
                trendChangeBar = bar;
            }
            curLowBar = bar;
            curLow = Math.Round(low, decimalPlaces, MidpointRounding.AwayFromZero);

            double dtbOffset = ATR(14)[CurrentBar - curLowBar] * dtbStrength / 100;
            if (curLow > lastLow - dtbOffset && curLow < lastLow + dtbOffset)
                curLowRelation = Relation.Double;
            else if (curLow < lastLow)
                curLowRelation = Relation.Lower;
            else
                curLowRelation = Relation.Higher;          
        }
        //#####################################################################
        #endregion

        #region CalcUpSwing(int bar, double high, bool updateHigh)
        //#####################################################################
        private void CalcUpSwing(int bar, double high, bool updateHigh)
        {
            if (!updateHigh)
            {
                lastHigh = curHigh;
                lastHighBar = curHighBar;
                lastHighRelation = curHighRelation;
                swingCounterUp++;
                swingSlope = 1;
                trendChangeBar = bar;
            }
            curHighBar = bar;
            curHigh = Math.Round(high, decimalPlaces, MidpointRounding.AwayFromZero);
            double dtbOffset = ATR(14)[CurrentBar - curHighBar] * dtbStrength / 100;
            if (curHigh > lastHigh - dtbOffset && curHigh < lastHigh + dtbOffset)
                curHighRelation = Relation.Double;
            else if (curHigh < lastHigh)
                curHighRelation = Relation.Lower;
            else
                curHighRelation = Relation.Higher;                    
        }
        //#####################################################################
        #endregion

        #region Properties
        //#####################################################################
        #region Plots
        //=====================================================================
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries RTDoubleTop
        {
            get { return Values[0]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries RTDownTrend
        {
            get { return Values[1]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries RTNoWhere
        {
            get { return Values[2]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries RTUpTrend
        {
            get { return Values[3]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries RTDoubleBottom
        {
            get { return Values[4]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries VHigh
        {
            get { return Values[5]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries VLow
        {
            get { return Values[6]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries VHighCurrent
        {
            get { return Values[7]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries VLowCurrent
        {
            get { return Values[8]; }
        }
        //===================================================================
        #endregion

        #region DataSeries
        // DataSeries =========================================================
        /// <summary>
        /// Represents the price position in relation to the swings.
        /// -2 = DT | -1 = LL and LH | 0 = price is nowhere | 1 = HH and HL | 2 = DB
        /// </summary>
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        public DataSeries SwingRelation
        {
            get { return swingRelation; }
        }
        /// <summary>
        /// Represents the trend. -1 = LL/LH | 0 = no trend | 1 = HH/HL
        /// </summary>
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        public DataSeries SwingTrend
        {
            get { return swingTrend; }
        }
        /// <summary>
        /// Represents the swing high volume. 
        /// </summary>
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        public DataSeries VolumeHighSeries
        {
            get { return volumeHighSeries; }
        }
        /// <summary>
        /// Represents the swing low volume. 
        /// </summary>
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        public DataSeries VolumeLowSeries
        {
            get { return volumeLowSeries; }
        }
        //=====================================================================
        #endregion

        #region Parameters
        //=====================================================================
        /// <summary>
        /// Represents the swing size. e.g. 1 = small swings and 5 = bigger swings.
        /// </summary>
        [GridCategory("Parameters")]
        [Description("Represents the swing size. e.g. 1 = small swings and 5 = bigger swings.")]
        [Gui.Design.DisplayName("Swing size")]
        public int SwingSize
        {
            get { return swingSize; }
            set { swingSize = Math.Max(1, value); }
        }
        /// <summary>
        /// Represents the swing type. Standard | Gann
        /// </summary>
        [GridCategory("Parameters")]
        [Description("Represents the swing type. Standard | Gann")]
        [Gui.Design.DisplayName("Swing type")]
        public SwingTypes SwingType
        {
            get { return swingType; }
            set { swingType = value; }
        }
        /// <summary>
        /// Represents the double top/-bottom strength.
        /// </summary>
        [GridCategory("Parameters")]
        [Description("Represents the double top/-bottom strength. Increase the value to get more DB/DT.")]
        [Gui.Design.DisplayName("Double top/-bottom strength")]
        public int DtbStrength
        {
            get { return dtbStrength; }
            set { dtbStrength = Math.Max(1, value); }
        }
        //=====================================================================
        #endregion

        #region Gann Swings
        //=====================================================================
        [Category("Gann Swings")]
        [Description("Indicates if inside bars are ignored. If set to true it is possible that between consecutive up/down bars are inside bars. Only used if calculationSize > 1.")]
        [Gui.Design.DisplayName("Ignore inside Bars")]
        public bool IgnoreInsideBars
        {
            get { return ignoreInsideBars; }
            set { ignoreInsideBars = value; }
        }
        [Category("Gann Swings")]
        [Description("Indicates if the swings are updated if the last swing high/low is broken. Only used if calculationSize > 1.")]
        [Gui.Design.DisplayName("Use breakouts")]
        public bool UseBreakouts
        {
            get { return useBreakouts; }
            set { useBreakouts = value; }
        }
        //===================================================================
        #endregion

        #region Settings
        //=====================================================================
        [Category("Settings")]
        [Description("Represents which swing indication is shown. Trend | Relation | Volume (repainting).")]
        [Gui.Design.DisplayName("Choose indication")]
        public Show ShowOscillator
        {
            get { return showOscillator; }
            set { showOscillator = value; }
        }
        [Category("Settings")]
        [Description("Indicates if the trend direction is changed when the old trend ends or whether a new trend must start first.")]
        [Gui.Design.DisplayNameAttribute("Trend change")]
        public bool UseOldTrend
        {
            get { return useOldTrend; }
            set { useOldTrend = value; }
        }
        //===================================================================
        #endregion
        //#####################################################################
        #endregion
    }
}

#region PriceActionSwingOscillator.Utility
namespace PriceActionSwingOscillator.Utility
{
    /// <summary>
    /// Represents the possible swing calculation types. Standard | Gann
    /// </summary>
    public enum SwingTypes
    {
        /// <summary>
        /// Represents the standard swing calculation.
        /// </summary>
        Standard,
        /// <summary>
        /// Represents the Gann swing calculation.
        /// </summary>
        Gann,
    }
}
#endregion

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private PriceActionSwingOscillator[] cachePriceActionSwingOscillator = null;

        private static PriceActionSwingOscillator checkPriceActionSwingOscillator = new PriceActionSwingOscillator();

        /// <summary>
        /// PriceActionSwingOscillator shows the trend direction, swing relation or developing swing volume. !!! The volume is repainting. !!!
        /// </summary>
        /// <returns></returns>
        public PriceActionSwingOscillator PriceActionSwingOscillator(int dtbStrength, int swingSize, SwingTypes swingType)
        {
            return PriceActionSwingOscillator(Input, dtbStrength, swingSize, swingType);
        }

        /// <summary>
        /// PriceActionSwingOscillator shows the trend direction, swing relation or developing swing volume. !!! The volume is repainting. !!!
        /// </summary>
        /// <returns></returns>
        public PriceActionSwingOscillator PriceActionSwingOscillator(Data.IDataSeries input, int dtbStrength, int swingSize, SwingTypes swingType)
        {
            if (cachePriceActionSwingOscillator != null)
                for (int idx = 0; idx < cachePriceActionSwingOscillator.Length; idx++)
                    if (cachePriceActionSwingOscillator[idx].DtbStrength == dtbStrength && cachePriceActionSwingOscillator[idx].SwingSize == swingSize && cachePriceActionSwingOscillator[idx].SwingType == swingType && cachePriceActionSwingOscillator[idx].EqualsInput(input))
                        return cachePriceActionSwingOscillator[idx];

            lock (checkPriceActionSwingOscillator)
            {
                checkPriceActionSwingOscillator.DtbStrength = dtbStrength;
                dtbStrength = checkPriceActionSwingOscillator.DtbStrength;
                checkPriceActionSwingOscillator.SwingSize = swingSize;
                swingSize = checkPriceActionSwingOscillator.SwingSize;
                checkPriceActionSwingOscillator.SwingType = swingType;
                swingType = checkPriceActionSwingOscillator.SwingType;

                if (cachePriceActionSwingOscillator != null)
                    for (int idx = 0; idx < cachePriceActionSwingOscillator.Length; idx++)
                        if (cachePriceActionSwingOscillator[idx].DtbStrength == dtbStrength && cachePriceActionSwingOscillator[idx].SwingSize == swingSize && cachePriceActionSwingOscillator[idx].SwingType == swingType && cachePriceActionSwingOscillator[idx].EqualsInput(input))
                            return cachePriceActionSwingOscillator[idx];

                PriceActionSwingOscillator indicator = new PriceActionSwingOscillator();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.DtbStrength = dtbStrength;
                indicator.SwingSize = swingSize;
                indicator.SwingType = swingType;
                Indicators.Add(indicator);
                indicator.SetUp();

                PriceActionSwingOscillator[] tmp = new PriceActionSwingOscillator[cachePriceActionSwingOscillator == null ? 1 : cachePriceActionSwingOscillator.Length + 1];
                if (cachePriceActionSwingOscillator != null)
                    cachePriceActionSwingOscillator.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachePriceActionSwingOscillator = tmp;
                return indicator;
            }
        }
    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// PriceActionSwingOscillator shows the trend direction, swing relation or developing swing volume. !!! The volume is repainting. !!!
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PriceActionSwingOscillator PriceActionSwingOscillator(int dtbStrength, int swingSize, SwingTypes swingType)
        {
            return _indicator.PriceActionSwingOscillator(Input, dtbStrength, swingSize, swingType);
        }

        /// <summary>
        /// PriceActionSwingOscillator shows the trend direction, swing relation or developing swing volume. !!! The volume is repainting. !!!
        /// </summary>
        /// <returns></returns>
        public Indicator.PriceActionSwingOscillator PriceActionSwingOscillator(Data.IDataSeries input, int dtbStrength, int swingSize, SwingTypes swingType)
        {
            return _indicator.PriceActionSwingOscillator(input, dtbStrength, swingSize, swingType);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// PriceActionSwingOscillator shows the trend direction, swing relation or developing swing volume. !!! The volume is repainting. !!!
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PriceActionSwingOscillator PriceActionSwingOscillator(int dtbStrength, int swingSize, SwingTypes swingType)
        {
            return _indicator.PriceActionSwingOscillator(Input, dtbStrength, swingSize, swingType);
        }

        /// <summary>
        /// PriceActionSwingOscillator shows the trend direction, swing relation or developing swing volume. !!! The volume is repainting. !!!
        /// </summary>
        /// <returns></returns>
        public Indicator.PriceActionSwingOscillator PriceActionSwingOscillator(Data.IDataSeries input, int dtbStrength, int swingSize, SwingTypes swingType)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.PriceActionSwingOscillator(input, dtbStrength, swingSize, swingType);
        }
    }
}
#endregion
