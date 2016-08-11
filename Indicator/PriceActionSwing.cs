// #############################################################
// #														   #
// #                     PriceActionSwing                      #
// #														   #
// #     10.10.2012 by dorschden, die.unendlichkeit@gmx.de     #
// #														   #
// #############################################################
// 

#region Updates
// Version 12 - 12.07.2011
// Version 13 - 13.07.2011
// Version 14 - 13.06.2012
// - Split version PriceActionSwing & PriceActionSwingPro
// Version 15 - 24.09.2012
// - Add naked swing visualization
// - Improved time output visualization
// - Improved duration output visualization
// Version 16 - 10.10.2012
// - Fixed volume bug - volume bigger than Int32
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
using PriceActionSwing.Utility;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// PriceActionSwing calculates swings and visualize them in different ways
    /// and display several information about them. 
    /// </summary>
    [Description("PriceActionSwing calculates swings and visualize them in different ways and display several information about them.")]
    public class PriceActionSwing : Indicator
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
        // Private ============================================================
        private List<Swings> swingHighs = new List<Swings>();
        private List<Swings> swingLows = new List<Swings>();
        //=====================================================================
        #endregion

        #region Display
        //=====================================================================
        private SwingDurationTypes showSwingDuration = SwingDurationTypes.False;
        private bool showSwingLabel = false;
        private bool showSwingPercent = false;
        private SwingTimeTypes showSwingTime = SwingTimeTypes.False;
        private SwingVolumeTypes swingVolumeType = SwingVolumeTypes.False;
        private VisualizationTypes visualizationType = VisualizationTypes.DotsAndLines;
        private SwingLengthTypes swingLengthType = SwingLengthTypes.Ticks;
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
        /// <summary>
        /// Represents the possible swing length outputs.
        /// </summary>
        public enum SwingLengthTypes
        {
            /// <summary>
            /// Swing length is disabled.
            /// </summary>
            False,
            /// <summary>
            /// Swing length in points.
            /// </summary>
            Points,
            /// <summary>
            /// Swing price.
            /// </summary>
            Price,
            /// <summary>
            /// Swing length in percent.
            /// </summary>
            /// <returns></returns>
            Percent,
            /// <summary>
            /// Swing price and swing length in points.
            /// </summary>
            Price_And_Points,
            /// <summary>
            /// Swing price and swing length in ticks.
            /// </summary>
            Price_And_Ticks,
            /// <summary>
            /// Swing length in ticks.
            /// </summary>
            Ticks,
            /// <summary>
            /// Swing length in ticks and swing price.
            /// </summary>
            /// <returns></returns>
            Ticks_And_Price,
        }

        /// <summary>
        /// Represents the possible swing visualizations. 
        /// Dots | Zig-Zag lines | Dots and lines | Gann style
        /// </summary>
        public enum VisualizationTypes
        {
            /// <summary>
            /// Visualize swings with dots.
            /// </summary>
            Dots,
            /// <summary>
            /// Visualize swings with zigzag lines.
            /// </summary>
            ZigZagLines,
            /// <summary>
            /// Visualize swings with dots and zigzag lines.
            /// </summary>
            DotsAndLines,
            /// <summary>
            /// Visualize swings with zigzag lines.
            /// </summary>
            GannStyle,
        }

        /// <summary>
        /// Represents the possible swing volume outputs. 
        /// Absolute | Relativ | False
        /// </summary>
        public enum SwingVolumeTypes
        {
            /// <summary>
            /// Absolue swing volume.
            /// </summary>
            Absolute,
            /// <summary>
            /// Relative swing volume.
            /// </summary>
            Relativ,
            /// <summary>
            /// No swing volume.
            /// </summary>
            False,
        }

        /// <summary>
        /// Represents the possible swing time outputs. 
        /// Integer | HourMinute | HourMinuteSecond | DayMonth | False
        /// </summary>
        public enum SwingTimeTypes
        {
            /// <summary>
            /// Integer value.
            /// </summary>
            Integer,
            /// <summary>
            /// HH:MM.
            /// </summary>
            HourMinute,
            /// <summary>
            /// HH:MM:SS.
            /// </summary>
            HourMinuteSecond,
            /// <summary>
            /// DD.MM.
            /// </summary>
            DayMonth,
            /// <summary>
            /// No swing time.
            /// </summary>
            False,
        }

        public enum SwingDurationTypes
        {
            Bars,
            False,
            HoursMinutes,
            MinutesSeconds,
            HoursTotal,
            MinutesTotal,
            SecondsTotal,
            Days,
        }
        //=====================================================================
        #endregion

        #region Gann Swings
        // Bars ===============================================================
        private BarType barType = BarType.InsideBar;
        private int consecutiveBars = 0;
        private int consecutiveBarNumber = 0;
        private double consecutiveBarValue = 0;
        private bool paintBars = false;
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

        #region Naked swings
        //===================================================================
        private SortedList<double, int> nakedSwingHighsList = new SortedList<double, int>();
        private SortedList<double, int> nakedSwingLowsList = new SortedList<double, int>();
        private bool showNakedSwings = false;
        private bool showHistoricalNakedSwings = false;
        private int nakedSwingCounter = 0;
        private Color nakedSwingHighColor = Color.Red;
        private Color nakedSwingLowColor = Color.Blue;
        private DashStyle nakedSwingDashStyle = DashStyle.Solid;
        private int nakedSwingLineWidth = 1;
        //===================================================================
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
        private double signalBarVolumeDn = 0;
        private double signalBarVolumeUp = 0;
        private int swingCounterDn = 0;
        private int swingCounterUp = 0;
        private int upCount = 0;
        private int dnCount = 0;
        private int trendChangeBar = 0;
        private int swingSlope = 0;
        // Swing values ===========================================================================
        private double curHigh = 0.0;
        private int curHighBar = 0;
        private DateTime curHighDateTime;
        private int curHighDuration = 0;
        private int curHighLength = 0;
        private double curHighPercent = 0.0;
        private int curHighTime = 0;
        private long curHighVolume = 0;
        private Relation curHighRelation = Relation.Higher;
        private double curLow = 0.0;
        private int curLowBar = 0;
        private DateTime curLowDateTime;
        private int curLowDuration = 0;
        private int curLowLength = 0;
        private double curLowPercent = 0.0;
        private int curLowTime = 0;
        private long curLowVolume = 0;
        private Relation curLowRelation = Relation.Higher;
        private double lastHigh = 0.0;
        private int lastHighBar = 0;
        private DateTime lastHighDateTime;
        private int lastHighDuration = 0;
        private int lastHighLength = 0;
        private double lastHighPercent = 0.0;
        private int lastHighTime = 0;
        private long lastHighVolume = 0;
        private Relation lastHighRelation = Relation.Higher;
        private double lastLow = 0.0;
        private int lastLowBar = 0;
        private DateTime lastLowDateTime;
        private int lastLowDuration = 0;
        private int lastLowLength = 0;
        private double lastLowPercent = 0.0;
        private int lastLowTime = 0;
        private long lastLowVolume = 0;
        private Relation lastLowRelation = Relation.Higher;
        //=========================================================================================
        #endregion

        #region Visualize swings
        //=====================================================================
        private Color textColourHigher = Color.Green;
        private Color textColorLower = Color.Red;
        private Color textColourDtb = Color.Gold;
        private Font textFont = new Font("Courier", 9, FontStyle.Regular);
        private int textOffsetLength = 15;
        private int textOffsetPercent = 45;
        private int textOffsetLabel = 30;
        private int textOffsetVolume = 60;
        private int textOffsetTime = 75;
        private Color zigZagColourUp = Color.Green;
        private Color zigZagColourDn = Color.Red;
        private DashStyle zigZagStyle = DashStyle.Dash;
        private int zigZagWidth = 1;
        //=====================================================================
        #endregion

        #region Swing struct
        //===================================================================
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
        }
        //===================================================================
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
            Add(new Plot(new Pen(Color.Gold, 3), PlotStyle.Dot, "DoubleBottom"));
            Add(new Plot(new Pen(Color.Red, 3), PlotStyle.Dot, "LowerLow"));
            Add(new Plot(new Pen(Color.Green, 3), PlotStyle.Dot, "HigherLow"));

            Add(new Plot(new Pen(Color.Gold, 3), PlotStyle.Dot, "DoubleTop"));
            Add(new Plot(new Pen(Color.Red, 3), PlotStyle.Dot, "LowerHigh"));
            Add(new Plot(new Pen(Color.Green, 3), PlotStyle.Dot, "HigherHigh"));

            Add(new Plot(new Pen(Color.Blue, 1), PlotStyle.Square, "GannSwing"));

            AutoScale = true;
            BarsRequired = 2;
            CalculateOnBarClose = true;
            Overlay = true;
            PriceTypeSupported = false;
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
                    curHighDateTime = curLowDateTime = lastHighDateTime = lastLowDateTime = Time[CurrentBar];
                    Swings up = new Swings(curHighBar, curHigh, 0, 0, curHighRelation);
                    Swings dn = new Swings(curLowBar, curLow, 0, 0, curLowRelation);
                    swingHighs.Add(up);
                    swingLows.Add(dn);
                    upCount = swingHighs.Count;
                    dnCount = swingLows.Count;
                }
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

                    if (FirstTickOfBar && VisualizationTypes.GannStyle == visualizationType)
                    {
                        if (swingSlope == 1)
                            GannSwing.Set(curHigh);
                        else
                            GannSwing.Set(curLow);
                    }

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
                    if (paintBars)
                    {
                        if (High[0] > High[1])
                        {
                            if (Low[0] < Low[1])
                            {
                                barType = BarType.OutsideBar;
                                BarColor = outsideBarColour;
                            }
                            else
                            {
                                barType = BarType.UpBar;
                                BarColor = upBarColour;
                            }
                        }
                        else
                        {
                            if (Low[0] < Low[1])
                            {
                                barType = BarType.DnBar;
                                BarColor = dnBarColour;
                            }
                            else
                            {
                                barType = BarType.InsideBar;
                                BarColor = insideBarColour;
                            }
                        }
                    }
                    else
                    {
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
                    }
                    //===================================================================
                    #endregion

                    #region Gann Swing
                    // Gann swing =================================================================
                    #region Up swing
                    // Up Swing ===================================================================
                    if (swingSlope == 1)
                    {
                        if (FirstTickOfBar && VisualizationTypes.GannStyle == visualizationType)
                            GannSwing.Set(curHigh);

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
                        if (FirstTickOfBar && VisualizationTypes.GannStyle == visualizationType)
                            GannSwing.Set(curLow);

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
        }

        #region CalcDnSwing(int bar, double low, bool updateLow)
        //#####################################################################
        private void CalcDnSwing(int bar, double low, bool updateLow)
        {
            #region New and update Swing values
            if (!updateLow)
            {
                if (VisualizationTypes.GannStyle == visualizationType)
                {
                    for (int i = CurrentBar - trendChangeBar; i >= 0; i--)
                        GannSwing.Set(i, curHigh);
                    GannSwing.Set(low);
                }
                lastLow = curLow;
                lastLowBar = curLowBar;
                lastLowDateTime = curLowDateTime;
                lastLowDuration = curLowDuration;
                lastLowLength = curLowLength;
                lastLowPercent = curLowPercent;
                lastLowRelation = curLowRelation;
                lastLowTime = curLowTime;
                lastLowVolume = curLowVolume;
                swingCounterDn++;
                swingSlope = -1;
                trendChangeBar = bar;

                if (showNakedSwings == true)
                {
                    nakedSwingHighsList.Add(curHigh, curHighBar);
                    DrawRay("NakedSwingHigh" + curHigh.ToString(), false,
                        CurrentBar - curHighBar, curHigh, CurrentBar - curHighBar - 1, curHigh,
                        nakedSwingHighColor, nakedSwingDashStyle, nakedSwingLineWidth);
                }
            }
            else
            {
                if (VisualizationTypes.Dots == visualizationType
                    || VisualizationTypes.DotsAndLines == visualizationType)
                {
                    DoubleBottom.Reset(CurrentBar - curLowBar);
                    LowerLow.Reset(CurrentBar - curLowBar);
                    HigherLow.Reset(CurrentBar - curLowBar);
                }
                swingLows.RemoveAt(swingLows.Count - 1);
            }
            curLowBar = bar;
            curLow = Math.Round(low, decimalPlaces, MidpointRounding.AwayFromZero);
            curLowTime = ToTime(Time[CurrentBar - curLowBar]);
            curLowDateTime = Time[CurrentBar - curLowBar];
            
            curLowLength = Convert.ToInt32(Math.Round((curLow - curHigh) / TickSize,
                0, MidpointRounding.AwayFromZero));
            if (curHighLength != 0)
                curLowPercent = Math.Round(100.0 / curHighLength * Math.Abs(curLowLength), 1);
            curLowDuration = curLowBar - curHighBar;
            double dtbOffset = ATR(14)[CurrentBar - curLowBar] * dtbStrength / 100;
            if (curLow > lastLow - dtbOffset && curLow < lastLow + dtbOffset)
                curLowRelation = Relation.Double;
            else if (curLow < lastLow)
                curLowRelation = Relation.Lower;
            else
                curLowRelation = Relation.Higher;
            if (!CalculateOnBarClose)
                signalBarVolumeUp = Volume[0];
            double swingVolume = 0.0;
            for (int i = CurrentBar - curLowBar; i < CurrentBar - curLowBar + curLowDuration; i++)
                swingVolume = swingVolume + Volume[i];
            if (!CalculateOnBarClose)
                swingVolume = swingVolume + (Volume[CurrentBar - curHighBar]
                    - signalBarVolumeDn);
            if (SwingVolumeTypes.Relativ == swingVolumeType)
                swingVolume = Math.Round(swingVolume / curLowDuration, 0,
                    MidpointRounding.AwayFromZero);
            curLowVolume = Convert.ToInt64(swingVolume);
            #endregion

            #region Visualize swing
            switch (visualizationType)
            {
                case VisualizationTypes.Dots:
                    switch (curLowRelation)
                    {
                        case Relation.Higher:
                            HigherLow.Set(CurrentBar - curLowBar, curLow);
                            break;
                        case Relation.Lower:
                            LowerLow.Set(CurrentBar - curLowBar, curLow);
                            break;
                        case Relation.Double:
                            DoubleBottom.Set(CurrentBar - curLowBar, curLow);
                            break;
                    }
                    break;
                case VisualizationTypes.ZigZagLines:
                    DrawLine("ZigZagDown" + swingCounterDn, AutoScale, CurrentBar - curHighBar,
                        curHigh, CurrentBar - curLowBar, curLow, zigZagColourDn, zigZagStyle, zigZagWidth);
                    break;
                case VisualizationTypes.DotsAndLines:
                    switch (curLowRelation)
                    {
                        case Relation.Higher:
                            HigherLow.Set(CurrentBar - curLowBar, curLow);
                            break;
                        case Relation.Lower:
                            LowerLow.Set(CurrentBar - curLowBar, curLow);
                            break;
                        case Relation.Double:
                            DoubleBottom.Set(CurrentBar - curLowBar, curLow);
                            break;
                    }
                    DrawLine("ZigZagDown" + swingCounterDn, AutoScale, CurrentBar - curHighBar,
                        curHigh, CurrentBar - curLowBar, curLow, zigZagColourDn, zigZagStyle, zigZagWidth);
                    break;
                case VisualizationTypes.GannStyle:
                    for (int i = CurrentBar - trendChangeBar; i >= 0; i--)
                        GannSwing.Set(i, curLow);
                    break;
            }

            if (showNakedSwings == true && nakedSwingLowsList.Count > 0)
            {
                while (nakedSwingLowsList.Count > 0 
                    && nakedSwingLowsList.Keys[nakedSwingLowsList.Count - 1] >= curLow)
                {
                    int counter = nakedSwingLowsList.Count - 1;
                    double nakedSwingLowPrice = nakedSwingLowsList.Keys[counter];
                    RemoveDrawObject("NakedSwingLow" + nakedSwingLowPrice.ToString());
                    if (showHistoricalNakedSwings == true)
                    {
                        DrawLine("NakedSwingLow" + nakedSwingCounter++, false,
                            CurrentBar - nakedSwingLowsList.Values[counter], nakedSwingLowPrice,
                            CurrentBar - curLowBar, nakedSwingLowPrice, nakedSwingLowColor,
                            nakedSwingDashStyle, nakedSwingLineWidth);
                    }
                    nakedSwingLowsList.RemoveAt(counter);
                }
            }
            #endregion

            #region Swing value output
            string output = null;
            string swingLabel = null;
            Color textColor = Color.Transparent;
            if (SwingLengthTypes.False != swingLengthType)
            {
                switch (swingLengthType)
                {
                    case SwingLengthTypes.Ticks:
                        output = curLowLength.ToString();
                        break;
                    case SwingLengthTypes.Ticks_And_Price:
                        output = curLowLength.ToString() + " / " + curLow.ToString();
                        break;
                    case SwingLengthTypes.Points:
                        output = (curLowLength * TickSize).ToString();
                        break;
                    case SwingLengthTypes.Price:
                        output = curLow.ToString();
                        break;
                    case SwingLengthTypes.Price_And_Points:
                        output = curLow.ToString() + " / " + (curLowLength * TickSize).ToString();
                        break;
                    case SwingLengthTypes.Price_And_Ticks:
                        output = curLow.ToString() + " / " + curLowLength.ToString();
                        break;
                    case SwingLengthTypes.Percent:
                        output = (Math.Round((100.0 / curHigh * (curLowLength * TickSize)),
                            2, MidpointRounding.AwayFromZero)).ToString();
                        break;
                }
            }
            if (showSwingDuration != SwingDurationTypes.False)
            {
                string outputDuration = "";
                TimeSpan timeSpan;
                int hours, minutes, seconds = 0;
                switch (showSwingDuration)
                {
                    case SwingDurationTypes.Bars:
                        outputDuration = curLowDuration.ToString();
                        break;
                    case SwingDurationTypes.HoursMinutes:
                        timeSpan = curLowDateTime.Subtract(curHighDateTime);
                        hours = timeSpan.Hours;
                        minutes = timeSpan.Minutes;
                        if (hours == 0)
                            outputDuration = "0:" + minutes.ToString();
                        else if (minutes == 0)
                            outputDuration = hours + ":00";
                        else
                            outputDuration = hours + ":" + minutes;
                        break;
                    case SwingDurationTypes.MinutesSeconds:
                        timeSpan = curLowDateTime.Subtract(curHighDateTime);
                        minutes = timeSpan.Minutes;
                        seconds = timeSpan.Seconds;
                        if (minutes == 0)
                            outputDuration = "0:" + seconds.ToString();
                        else if (seconds == 0)
                            outputDuration = minutes + ":00";
                        else
                            outputDuration = minutes + ":" + seconds;
                        break;
                    case SwingDurationTypes.HoursTotal:
                        timeSpan = curLowDateTime.Subtract(curHighDateTime);
                        outputDuration = Math.Round(timeSpan.TotalHours, 1,
                            MidpointRounding.AwayFromZero).ToString();
                        break;
                    case SwingDurationTypes.MinutesTotal:
                        timeSpan = curLowDateTime.Subtract(curHighDateTime);
                        outputDuration = Math.Round(timeSpan.TotalMinutes, 1,
                            MidpointRounding.AwayFromZero).ToString();
                        break;
                    case SwingDurationTypes.SecondsTotal:
                        timeSpan = curLowDateTime.Subtract(curHighDateTime);
                        outputDuration = Math.Round(timeSpan.TotalSeconds, 1,
                            MidpointRounding.AwayFromZero).ToString();
                        break;
                    case SwingDurationTypes.Days:
                        timeSpan = curLowDateTime.Subtract(curHighDateTime);
                        outputDuration = Math.Round(timeSpan.TotalDays, 1,
                            MidpointRounding.AwayFromZero).ToString();
                        break;
                }
                if (SwingLengthTypes.False != swingLengthType)
                    output = output + " / " + outputDuration;
                else
                    output = outputDuration;
            }
            switch (curLowRelation)
            {
                case Relation.Higher:
                    swingLabel = "HL";
                    textColor = textColourHigher;
                    break;
                case Relation.Lower:
                    swingLabel = "LL";
                    textColor = textColorLower;
                    break;
                case Relation.Double:
                    swingLabel = "DB";
                    textColor = textColourDtb;
                    break;
            }
            if (output != null)
                DrawText("DnLength" + swingCounterDn, AutoScale, output.ToString(),
                    CurrentBar - curLowBar, curLow, -textOffsetLength, textColor, textFont,
                    StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
            if (showSwingLabel)
                DrawText("DnLabel" + swingCounterDn, AutoScale, swingLabel,
                    CurrentBar - curLowBar, curLow, -textOffsetLabel, textColor, textFont,
                    StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
            if (showSwingPercent && curLowPercent != 0)
            {
                DrawText("DnPerc" + swingCounterDn, AutoScale, curLowPercent.ToString() + "%",
                    CurrentBar - curLowBar, curLow, -textOffsetPercent, textColor, textFont,
                    StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
            }
            if (SwingTimeTypes.False != showSwingTime)
            {
                string timeOutput = "";
                switch (showSwingTime)
                {
                    case SwingTimeTypes.Integer:
                        timeOutput = curLowTime.ToString();
                        break;
                    case SwingTimeTypes.HourMinute:
                        timeOutput = string.Format("{0:t}", Time[CurrentBar - curLowBar]);
                        break;
                    case SwingTimeTypes.HourMinuteSecond:
                        timeOutput = string.Format("{0:T}", Time[CurrentBar - curLowBar]);
                        break;
                    case SwingTimeTypes.DayMonth:
                        timeOutput = string.Format("{0:dd.MM}", Time[CurrentBar - curHighBar]);
                        break;
                }
                DrawText("DnTime" + swingCounterDn, AutoScale, timeOutput,
                    CurrentBar - curLowBar, curLow, -textOffsetTime, textColor, textFont,
                    StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
            }
            if (SwingVolumeTypes.False != swingVolumeType)
            {
                DrawText("DnVolume" + swingCounterDn, AutoScale,
                    TruncIntToStr(Convert.ToInt32(swingVolume)),
                    CurrentBar - curLowBar, curLow, -textOffsetVolume, textColor, textFont,
                    StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
            }
            #endregion

            Swings dn = new Swings(curLowBar, curLow, curLowLength, curLowDuration, curLowRelation);
            swingLows.Add(dn);
            dnCount = swingLows.Count - 1;
        }
        //#####################################################################
        #endregion

        #region CalcUpSwing(int bar, double high, bool updateHigh)
        //#####################################################################
        private void CalcUpSwing(int bar, double high, bool updateHigh)
        {
            #region New and update swing values
            if (!updateHigh)
            {
                if (VisualizationTypes.GannStyle == visualizationType)
                {
                    for (int i = CurrentBar - trendChangeBar; i >= 0; i--)
                        GannSwing.Set(i, curLow);
                    GannSwing.Set(high);
                }
                lastHigh = curHigh;
                lastHighBar = curHighBar;
                lastHighDateTime = curHighDateTime;
                lastHighDuration = curHighDuration;
                lastHighLength = curHighLength;
                lastHighPercent = curHighPercent;
                lastHighRelation = curHighRelation;
                lastHighTime = curHighTime;
                lastHighVolume = curHighVolume;
                swingCounterUp++;
                swingSlope = 1;
                trendChangeBar = bar;

                if (showNakedSwings == true)
                {
                    nakedSwingLowsList.Add(curLow, curLowBar);
                    DrawRay("NakedSwingLow" + curLow.ToString(), false,
                        CurrentBar - curLowBar, curLow, CurrentBar - curLowBar - 1, curLow,
                        nakedSwingLowColor, nakedSwingDashStyle, nakedSwingLineWidth);
                }
            }
            else
            {
                if (VisualizationTypes.Dots == visualizationType
                    || VisualizationTypes.DotsAndLines == visualizationType)
                {
                    DoubleTop.Reset(CurrentBar - curHighBar);
                    HigherHigh.Reset(CurrentBar - curHighBar);
                    LowerHigh.Reset(CurrentBar - curHighBar);
                }
                swingHighs.RemoveAt(swingHighs.Count - 1);
            }
            curHighBar = bar;
            curHigh = Math.Round(high, decimalPlaces, MidpointRounding.AwayFromZero);
            curHighTime = ToTime(Time[CurrentBar - curHighBar]);
            curHighDateTime = Time[CurrentBar - curHighBar];

            curHighLength = Convert.ToInt32(Math.Round((curHigh - curLow) / TickSize,
                0, MidpointRounding.AwayFromZero));
            if (curLowLength != 0)
                curHighPercent = Math.Round(100.0 / Math.Abs(curLowLength) * curHighLength, 1);
            curHighDuration = curHighBar - curLowBar;
            double dtbOffset = ATR(14)[CurrentBar - curHighBar] * dtbStrength / 100;
            if (curHigh > lastHigh - dtbOffset && curHigh < lastHigh + dtbOffset)
                curHighRelation = Relation.Double;
            else if (curHigh < lastHigh)
                curHighRelation = Relation.Lower;
            else
                curHighRelation = Relation.Higher;
            if (!CalculateOnBarClose)
                signalBarVolumeDn = Volume[0];
            double swingVolume = 0.0;
            for (int i = CurrentBar - curHighBar; i < CurrentBar - curHighBar + curHighDuration; i++)
                swingVolume = swingVolume + Volume[i];
            if (!CalculateOnBarClose)
                swingVolume = swingVolume + (Volume[CurrentBar - curLowBar]
                    - signalBarVolumeUp);
            if (SwingVolumeTypes.Relativ == swingVolumeType)
                swingVolume = Math.Round(swingVolume / curHighDuration, 0,
                    MidpointRounding.AwayFromZero);
            curHighVolume = Convert.ToInt64(swingVolume);
            #endregion

            #region Visualize swing
            switch (visualizationType)
            {
                case VisualizationTypes.Dots:
                    switch (curHighRelation)
                    {
                        case Relation.Higher:
                            HigherHigh.Set(CurrentBar - curHighBar, curHigh);
                            break;
                        case Relation.Lower:
                            LowerHigh.Set(CurrentBar - curHighBar, curHigh);
                            break;
                        case Relation.Double:
                            DoubleTop.Set(CurrentBar - curHighBar, curHigh);
                            break;
                    }
                    break;
                case VisualizationTypes.ZigZagLines:
                    DrawLine("ZigZagUp" + swingCounterUp, AutoScale, CurrentBar - curLowBar,
                        curLow, CurrentBar - curHighBar, curHigh, zigZagColourUp, zigZagStyle, zigZagWidth);
                    break;
                case VisualizationTypes.DotsAndLines:
                    switch (curHighRelation)
                    {
                        case Relation.Higher:
                            HigherHigh.Set(CurrentBar - curHighBar, curHigh);
                            break;
                        case Relation.Lower:
                            LowerHigh.Set(CurrentBar - curHighBar, curHigh);
                            break;
                        case Relation.Double:
                            DoubleTop.Set(CurrentBar - curHighBar, curHigh);
                            break;
                    }
                    DrawLine("ZigZagUp" + swingCounterUp, AutoScale, CurrentBar - curLowBar,
                        curLow, CurrentBar - curHighBar, curHigh, zigZagColourUp, zigZagStyle, zigZagWidth);
                    break;
                case VisualizationTypes.GannStyle:
                    for (int i = CurrentBar - trendChangeBar; i >= 0; i--)
                        GannSwing.Set(i, high);
                    break;
            }

            if (showNakedSwings == true && nakedSwingHighsList.Count > 0)
            {
                while (nakedSwingHighsList.Count > 0 && nakedSwingHighsList.Keys[0] <= curHigh)
                {
                    double nakedSwingHighPrice = nakedSwingHighsList.Keys[0];
                    RemoveDrawObject("NakedSwingHigh" + nakedSwingHighPrice.ToString());
                    if (showHistoricalNakedSwings == true)
                    {
                        DrawLine("NakedSwingHigh" + nakedSwingCounter++, false,
                            CurrentBar - nakedSwingHighsList.Values[0], nakedSwingHighPrice,
                            CurrentBar - curHighBar, nakedSwingHighPrice, nakedSwingHighColor,
                            nakedSwingDashStyle, nakedSwingLineWidth);
                    }
                    nakedSwingHighsList.RemoveAt(0);
                }
            }
            #endregion

            #region Swing value output
            string output = null;
            string swingLabel = null;
            Color textColor = Color.Transparent;
            if (SwingLengthTypes.False != swingLengthType)
            {
                switch (swingLengthType)
                {
                    case SwingLengthTypes.Ticks:
                        output = curHighLength.ToString();
                        break;
                    case SwingLengthTypes.Ticks_And_Price:
                        output = curHighLength.ToString() + " / " + curHigh.ToString();
                        break;
                    case SwingLengthTypes.Points:
                        output = (curHighLength * TickSize).ToString();
                        break;
                    case SwingLengthTypes.Price:
                        output = curHigh.ToString();
                        break;
                    case SwingLengthTypes.Price_And_Points:
                        output = curHigh.ToString() + " / " + (curHighLength * TickSize).ToString();
                        break;
                    case SwingLengthTypes.Price_And_Ticks:
                        output = curHigh.ToString() + " / " + curHighLength.ToString();
                        break;
                    case SwingLengthTypes.Percent:
                        output = (Math.Round((100.0 / curLow * (curHighLength * TickSize)),
                            2, MidpointRounding.AwayFromZero)).ToString();
                        break;
                }
            }
            if (showSwingDuration != SwingDurationTypes.False)
            {
                string outputDuration = "";
                TimeSpan timeSpan;
                int hours, minutes, seconds = 0;
                switch (showSwingDuration)
                {
                    case SwingDurationTypes.Bars:
                        outputDuration = curHighDuration.ToString();
                        break;
                    case SwingDurationTypes.HoursMinutes:
                        timeSpan = curHighDateTime.Subtract(curLowDateTime);
                        hours = timeSpan.Hours;
                        minutes = timeSpan.Minutes;
                        if (hours == 0)
                            outputDuration = "0:" + minutes.ToString();
                        else if (minutes == 0)
                            outputDuration = hours + ":00";
                        else
                            outputDuration = hours + ":" + minutes;
                        break;
                    case SwingDurationTypes.MinutesSeconds:
                        timeSpan = curHighDateTime.Subtract(curLowDateTime);
                        minutes = timeSpan.Minutes;
                        seconds = timeSpan.Seconds;
                        if (minutes == 0)
                            outputDuration = "0:" + seconds.ToString();
                        else if (seconds == 0)
                            outputDuration = minutes + ":00";
                        else
                            outputDuration = minutes + ":" + seconds;
                        break;
                    case SwingDurationTypes.HoursTotal:
                        timeSpan = curHighDateTime.Subtract(curLowDateTime);
                        outputDuration = Math.Round(timeSpan.TotalHours, 1,
                            MidpointRounding.AwayFromZero).ToString();
                        break;
                    case SwingDurationTypes.MinutesTotal:
                        timeSpan = curHighDateTime.Subtract(curLowDateTime);
                        outputDuration = Math.Round(timeSpan.TotalMinutes, 1,
                            MidpointRounding.AwayFromZero).ToString();
                        break;
                    case SwingDurationTypes.SecondsTotal:
                        timeSpan = curHighDateTime.Subtract(curLowDateTime);
                        outputDuration = Math.Round(timeSpan.TotalSeconds, 1,
                            MidpointRounding.AwayFromZero).ToString();
                        break;
                    case SwingDurationTypes.Days:
                        timeSpan = curHighDateTime.Subtract(curLowDateTime);
                        outputDuration = Math.Round(timeSpan.TotalDays, 1,
                            MidpointRounding.AwayFromZero).ToString();
                        break;
                }
                if (SwingLengthTypes.False != swingLengthType)
                    output = output + " / " + outputDuration;
                else
                    output = outputDuration;
            }
            switch (curHighRelation)
            {
                case Relation.Higher:
                    swingLabel = "HH";
                    textColor = textColourHigher;
                    break;
                case Relation.Lower:
                    swingLabel = "LH";
                    textColor = textColorLower;
                    break;
                case Relation.Double:
                    swingLabel = "DT";
                    textColor = textColourDtb;
                    break;
            }
            if (output != null)
                DrawText("High" + swingCounterUp, AutoScale, output.ToString(),
                    CurrentBar - curHighBar, curHigh, textOffsetLength, textColor, textFont,
                    StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
            if (showSwingLabel)
                DrawText("UpLabel" + swingCounterUp, AutoScale, swingLabel,
                    CurrentBar - curHighBar, curHigh, textOffsetLabel, textColor, textFont,
                    StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
            if (showSwingPercent && curHighPercent != 0)
            {
                DrawText("UpPerc" + swingCounterUp, AutoScale, curHighPercent.ToString() + "%",
                    CurrentBar - curHighBar, curHigh, textOffsetPercent, textColor, textFont,
                    StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
            }
            if (SwingTimeTypes.False != showSwingTime)
            {
                string timeOutput = "";
                switch (showSwingTime)
                {
                    case SwingTimeTypes.Integer:
                        timeOutput = curHighTime.ToString();
                        break;
                    case SwingTimeTypes.HourMinute:
                        timeOutput = string.Format("{0:t}", Time[CurrentBar - curHighBar]);
                        break;
                    case SwingTimeTypes.HourMinuteSecond:
                        timeOutput = string.Format("{0:T}", Time[CurrentBar - curHighBar]);
                        break;
                    case SwingTimeTypes.DayMonth:
                        timeOutput = string.Format("{0:dd.MM}", Time[CurrentBar - curHighBar]);
                        break;
                }
                DrawText("UpTime" + swingCounterUp, AutoScale, timeOutput,
                    CurrentBar - curHighBar, curHigh, textOffsetTime, textColor, textFont,
                    StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
            }
            if (SwingVolumeTypes.False != swingVolumeType)
            {
                DrawText("UpVolume" + swingCounterUp, AutoScale,
                    TruncIntToStr(curHighVolume),
                    CurrentBar - curHighBar, curHigh, textOffsetVolume, textColor, textFont,
                    StringAlignment.Center, Color.Transparent, Color.Transparent, 0);
            }
            // ====================================================================================
            #endregion

            Swings up = new Swings(curHighBar, curHigh, curHighLength, curHighDuration, curHighRelation);
            swingHighs.Add(up);
            upCount = swingHighs.Count - 1;
        }
        //#####################################################################
        #endregion

        #region TruncIntToStr(long number)
        //#####################################################################
        /// <summary>
        /// Converts long integer numbers in a number-string format.
        /// </summary>
        private string TruncIntToStr(long number)
        {
            long numberAbs = Math.Abs(number);
            string output = "";
            double convertedNumber = 0.0;
            if (numberAbs > 1000000000)
            {
                convertedNumber = Math.Round(number / 1000000000.0, 1,
                    MidpointRounding.AwayFromZero);
                output = convertedNumber.ToString() + "B";
            }
            else if (numberAbs > 1000000)
            {
                convertedNumber = Math.Round(number / 1000000.0, 1,
                    MidpointRounding.AwayFromZero);
                output = convertedNumber.ToString() + "M";
            }
            else if (numberAbs > 1000)
            {
                convertedNumber = Math.Round(number / 1000.0, 1,
                    MidpointRounding.AwayFromZero);
                output = convertedNumber.ToString() + "K";
            }
            else
                output = number.ToString();

            return output;
        }
        //#####################################################################
        #endregion

        #region Properties
        //#####################################################################
        #region Plots
        // Plots ==============================================================
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries DoubleBottom
        {
            get { return Values[0]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LowerLow
        {
            get { return Values[1]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HigherLow
        {
            get { return Values[2]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries DoubleTop
        {
            get { return Values[3]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LowerHigh
        {
            get { return Values[4]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HigherHigh
        {
            get { return Values[5]; }
        }
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries GannSwing
        {
            get { return Values[6]; }
        }
        //=====================================================================
        #endregion

        #region Parameters
        //=====================================================================
        /// <summary>
        /// Represents the swing size. e.g. 1 = small swings and 5 = bigger swings.
        /// </summary>
#if NT7
        [GridCategory("Parameters")]
#else
        [Category("Parameters")]
#endif
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
#if NT7
        [GridCategory("Parameters")]
#else
        [Category("Parameters")]
#endif
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
#if NT7
        [GridCategory("Parameters")]
#else
        [Category("Parameters")]
#endif
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
        [Description("Indicates if the bars are colorized.")]
        [Gui.Design.DisplayName("Paint bars")]
        public bool PaintBars
        {
            get { return paintBars; }
            set { paintBars = value; }
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

        #region Naked swings
        //=====================================================================
        [Category("Naked swings")]
        [Description("Indicates if naked swing lines are shown.")]
        [Gui.Design.DisplayNameAttribute("Show naked swing lines")]
        public bool ShowNakedSwings
        {
            get { return showNakedSwings; }
            set { showNakedSwings = value; }
        }
        [Category("Naked swings")]
        [Description("Indicates if historical naked swing lines are shown.")]
        [Gui.Design.DisplayNameAttribute("Show historical naked swing lines")]
        public bool ShowHistoricalNakedSwings
        {
            get { return showHistoricalNakedSwings; }
            set { showHistoricalNakedSwings = value; }
        }
        [Category("Naked swings")]
        [Description("Represents the colour of the naked swing high lines.")]
        [Gui.Design.DisplayNameAttribute("Naked swing high colour")]
        public Color NakedSwingHighColor
        {
            get { return nakedSwingHighColor; }
            set { nakedSwingHighColor = value; }
        }
        [Browsable(false)]
        public string NakedSwingHighColorSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(nakedSwingHighColor); }
            set { nakedSwingHighColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
        [Category("Naked swings")]
        [Description("Represents the colour of the naked swing low lines.")]
        [Gui.Design.DisplayNameAttribute("Naked swing low colour")]
        public Color NakedSwingLowColor
        {
            get { return nakedSwingLowColor; }
            set { nakedSwingLowColor = value; }
        }
        [Browsable(false)]
        public string NakedSwingLowColorSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(nakedSwingLowColor); }
            set { nakedSwingLowColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
        [Category("Naked swings")]
        [Description("Represents the line style of the naked swing lines.")]
        [Gui.Design.DisplayNameAttribute("Naked swing line style")]
        public DashStyle NakedSwingDashStyle
        {
            get { return nakedSwingDashStyle; }
            set { nakedSwingDashStyle = value; }
        }
        [Category("Naked swings")]
        [Description("Represents the line width of the naked swing lines.")]
        [Gui.Design.DisplayNameAttribute("Naked swing line width")]
        public int NakedSwingLineWidth
        {
            get { return nakedSwingLineWidth; }
            set { nakedSwingLineWidth = Math.Max(1, value); }
        }
        //=====================================================================
        #endregion

        #region Swings values
        //=====================================================================
        [Category("Swing values")]
        [Description("Represents the swing duration type.")]
        [Gui.Design.DisplayName("Swing duration mode")]
        public SwingDurationTypes ShowSwingDuration
        {
            get { return showSwingDuration; }
            set { showSwingDuration = value; }
        }
        [Category("Swing values")]
        [Description("Indicates if the swing label is shown (HH, HL, LL, LH, DB, DT).")]
        [Gui.Design.DisplayName("Swing labels")]
        public bool ShowSwingLabel
        {
            get { return showSwingLabel; }
            set { showSwingLabel = value; }
        }
        [Category("Swing values")]
        [Description("Indicates if the swing length is shown and in which style.")]
        [Gui.Design.DisplayName("Swing length")]
        public SwingLengthTypes SwingLengthType
        {
            get { return swingLengthType; }
            set { swingLengthType = value; }
        }
        [Category("Swing values")]
        [Description("Indicates if the swing percentage in relation to the last swing is shown.")]
        [Gui.Design.DisplayName("Swing percentage")]
        public bool ShowSwingPercent
        {
            get { return showSwingPercent; }
            set { showSwingPercent = value; }
        }
        [Category("Swing values")]
        [Description("Represents the swing time format. HourMinute = HH:MM | HourMinuteSecond = HH:MM:SS | DayMonth = DD.MM")]
        [Gui.Design.DisplayName("Swing time")]
        public SwingTimeTypes ShowSwingTime
        {
            get { return showSwingTime; }
            set { showSwingTime = value; }
        }
        [Category("Swing values")]
        [Description("Represents the swing visualization type. Dots | Zig-Zag lines | Dots and lines | Gann style")]
        [Gui.Design.DisplayName("Swing visualization type")]
        public VisualizationTypes VisualizationType
        {
            get { return visualizationType; }
            set { visualizationType = value; }
        }
        [Category("Swing values")]
        [Description("Represents the swing volume mode.")]
        [Gui.Design.DisplayName("Swing volume")]
        public SwingVolumeTypes SwingVolumeType
        {
            get { return swingVolumeType; }
            set { swingVolumeType = value; }
        }
        //=====================================================================
        #endregion

        #region Visualize swings
        //=====================================================================
        [XmlIgnore()]
        [Category("Visualize swings")]
        [Description("Represents the text font for the swing value output.")]
        [Gui.Design.DisplayNameAttribute("Text font")]
        public Font TextFont
        {
            get { return textFont; }
            set { textFont = value; }
        }
        [Browsable(false)]
        public string TextFontSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableFont.ToString(textFont); }
            set { textFont = NinjaTrader.Gui.Design.SerializableFont.FromString(value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the text offset in pixel for the swing length/duration.")]
        [Gui.Design.DisplayNameAttribute("Text offset length / duration")]
        public int TextOffsetLength
        {
            get { return textOffsetLength; }
            set { textOffsetLength = Math.Max(1, value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the text offset in pixel for the swing labels.")]
        [Gui.Design.DisplayNameAttribute("Text offset swing labels")]
        public int TextOffsetLabel
        {
            get { return textOffsetLabel; }
            set { textOffsetLabel = Math.Max(1, value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the text offset in pixel for the retracement value.")]
        [Gui.Design.DisplayNameAttribute("Text offset percent")]
        public int TextOffsetPercent
        {
            get { return textOffsetPercent; }
            set { textOffsetPercent = Math.Max(1, value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the text offset in pixel for the time value.")]
        [Gui.Design.DisplayNameAttribute("Text offset time")]
        public int TextOffsetTime
        {
            get { return textOffsetTime; }
            set { textOffsetTime = Math.Max(1, value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the text offset in pixel for the swing volume.")]
        [Gui.Design.DisplayNameAttribute("Text offset volume")]
        public int TextOffsetVolume
        {
            get { return textOffsetVolume; }
            set { textOffsetVolume = Math.Max(1, value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the colour of the swing value output for higher swings.")]
        [Gui.Design.DisplayNameAttribute("Text colour higher")]
        public Color TextColourHigher
        {
            get { return textColourHigher; }
            set { textColourHigher = value; }
        }
        [Browsable(false)]
        public string TextColourHigherSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(textColourHigher); }
            set { textColourHigher = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the colour of the swing value output for lower swings.")]
        [Gui.Design.DisplayNameAttribute("Text colour lower")]
        public Color TextColourLower
        {
            get { return textColorLower; }
            set { textColorLower = value; }
        }
        [Browsable(false)]
        public string TextColourLowerSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(textColorLower); }
            set { textColorLower = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the colour of the swing value output for double bottoms/-tops.")]
        [Gui.Design.DisplayNameAttribute("Text colour DTB")]
        public Color TextColourDtb
        {
            get { return textColourDtb; }
            set { textColourDtb = value; }
        }
        [Browsable(false)]
        public string TextColourDTBSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(textColourDtb); }
            set { textColourDtb = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the colour of the zig-zag up lines.")]
        [Gui.Design.DisplayNameAttribute("Zig-Zag colour up")]
        public Color ZigZagColourUp
        {
            get { return zigZagColourUp; }
            set { zigZagColourUp = value; }
        }
        [Browsable(false)]
        public string ZigZagColourUpSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(zigZagColourUp); }
            set { zigZagColourUp = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the colour of the zig-zag down lines.")]
        [Gui.Design.DisplayNameAttribute("Zig-Zag colour down")]
        public Color ZigZagColourDn
        {
            get { return zigZagColourDn; }
            set { zigZagColourDn = value; }
        }
        [Browsable(false)]
        public string ZigZagColourDnSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(zigZagColourDn); }
            set { zigZagColourDn = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
        [Category("Visualize swings")]
        [Description("Represents the line style of the zig-zag lines.")]
        [Gui.Design.DisplayNameAttribute("Zig-Zag style")]
        public DashStyle ZigZagStyle
        {
            get { return zigZagStyle; }
            set { zigZagStyle = value; }
        }
        [Category("Visualize swings")]
        [Description("Represents the line width of the zig-zag lines.")]
        [Gui.Design.DisplayNameAttribute("Zig-Zag width")]
        public int ZigZagWidth
        {
            get { return zigZagWidth; }
            set { zigZagWidth = Math.Max(1, value); }
        }
        //=====================================================================
        #endregion
        //#####################################################################
        #endregion
    }
}

#region PriceActionSwing.Utility
namespace PriceActionSwing.Utility
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
        private PriceActionSwing[] cachePriceActionSwing = null;

        private static PriceActionSwing checkPriceActionSwing = new PriceActionSwing();

        /// <summary>
        /// PriceActionSwing calculates swings and visualize them in different ways and display several information about them.
        /// </summary>
        /// <returns></returns>
        public PriceActionSwing PriceActionSwing(int dtbStrength, int swingSize, SwingTypes swingType)
        {
            return PriceActionSwing(Input, dtbStrength, swingSize, swingType);
        }

        /// <summary>
        /// PriceActionSwing calculates swings and visualize them in different ways and display several information about them.
        /// </summary>
        /// <returns></returns>
        public PriceActionSwing PriceActionSwing(Data.IDataSeries input, int dtbStrength, int swingSize, SwingTypes swingType)
        {
            if (cachePriceActionSwing != null)
                for (int idx = 0; idx < cachePriceActionSwing.Length; idx++)
                    if (cachePriceActionSwing[idx].DtbStrength == dtbStrength && cachePriceActionSwing[idx].SwingSize == swingSize && cachePriceActionSwing[idx].SwingType == swingType && cachePriceActionSwing[idx].EqualsInput(input))
                        return cachePriceActionSwing[idx];

            lock (checkPriceActionSwing)
            {
                checkPriceActionSwing.DtbStrength = dtbStrength;
                dtbStrength = checkPriceActionSwing.DtbStrength;
                checkPriceActionSwing.SwingSize = swingSize;
                swingSize = checkPriceActionSwing.SwingSize;
                checkPriceActionSwing.SwingType = swingType;
                swingType = checkPriceActionSwing.SwingType;

                if (cachePriceActionSwing != null)
                    for (int idx = 0; idx < cachePriceActionSwing.Length; idx++)
                        if (cachePriceActionSwing[idx].DtbStrength == dtbStrength && cachePriceActionSwing[idx].SwingSize == swingSize && cachePriceActionSwing[idx].SwingType == swingType && cachePriceActionSwing[idx].EqualsInput(input))
                            return cachePriceActionSwing[idx];

                PriceActionSwing indicator = new PriceActionSwing();
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

                PriceActionSwing[] tmp = new PriceActionSwing[cachePriceActionSwing == null ? 1 : cachePriceActionSwing.Length + 1];
                if (cachePriceActionSwing != null)
                    cachePriceActionSwing.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachePriceActionSwing = tmp;
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
        /// PriceActionSwing calculates swings and visualize them in different ways and display several information about them.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PriceActionSwing PriceActionSwing(int dtbStrength, int swingSize, SwingTypes swingType)
        {
            return _indicator.PriceActionSwing(Input, dtbStrength, swingSize, swingType);
        }

        /// <summary>
        /// PriceActionSwing calculates swings and visualize them in different ways and display several information about them.
        /// </summary>
        /// <returns></returns>
        public Indicator.PriceActionSwing PriceActionSwing(Data.IDataSeries input, int dtbStrength, int swingSize, SwingTypes swingType)
        {
            return _indicator.PriceActionSwing(input, dtbStrength, swingSize, swingType);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// PriceActionSwing calculates swings and visualize them in different ways and display several information about them.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PriceActionSwing PriceActionSwing(int dtbStrength, int swingSize, SwingTypes swingType)
        {
            return _indicator.PriceActionSwing(Input, dtbStrength, swingSize, swingType);
        }

        /// <summary>
        /// PriceActionSwing calculates swings and visualize them in different ways and display several information about them.
        /// </summary>
        /// <returns></returns>
        public Indicator.PriceActionSwing PriceActionSwing(Data.IDataSeries input, int dtbStrength, int swingSize, SwingTypes swingType)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.PriceActionSwing(input, dtbStrength, swingSize, swingType);
        }
    }
}
#endregion
