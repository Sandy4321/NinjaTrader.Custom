// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Plots the open, high, low and close values from the session starting on the prior day.
    /// </summary>
    [Description("Plots the open, high, low and close values from the session starting on the prior day.")]
    public class PriorDayOHLC : Indicator
    {
        #region Variables

        // Wizard generated variables
        // User defined variables (add any user defined variables below)
		private DateTime 	currentDate 	= Cbi.Globals.MinDate;
		private double		currentOpen		= 0;
        private double		currentHigh		= 0;
		private double		currentLow		= 0;
		private double		currentClose	= 0;
		private double		priordayOpen	= 0;
		private double		priordayHigh	= 0;
		private double		priordayLow		= 0;
		private double		priordayClose	= 0;
		private bool		showOpen		= true;
		private bool		showHigh		= true;
		private bool		showLow			= true;
		private bool		showClose		= true;
		#endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Orange, PlotStyle.Hash, "Prior Open"));
            Add(new Plot(Color.Green, PlotStyle.Hash, "Prior High"));
            Add(new Plot(Color.Red, PlotStyle.Hash, "Prior Low"));
            Add(new Plot(Color.Firebrick, PlotStyle.Hash, "Prior Close"));

			Plots[0].Pen.DashStyle = DashStyle.Dash;
			Plots[3].Pen.DashStyle = DashStyle.Dash;
			
			AutoScale 			= false;
            Overlay				= true;	  // Plots the indicator on top of price
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (Bars == null)
				return;

			if (!Bars.BarsType.IsIntraday)
			{
				DrawTextFixed("error msg", "PriorDayOHLC only works on intraday intervals", TextPosition.BottomRight);
				return;
			}

            //MY PIECE OF CODE TO ONLY INCLUDE THE LINES FOR THE 5MIN CHART
            if (!((BarsPeriod.Id == PeriodType.Minute) && (BarsPeriod.Value == 5)))
            {
                return;
            }

			// If the current data is not the same date as the current bar then its a new session
			if (currentDate != Bars.GetTradingDayFromLocal(Time[0]) || currentOpen == 0)
			{
				// The current day OHLC values are now the prior days value so set
				// them to their respect indicator series for plotting
				if (currentOpen != 0)
				{
					priordayOpen	= currentOpen;
					priordayHigh	= currentHigh;
					priordayLow		= currentLow;
					priordayClose	= currentClose;

					if (ShowOpen)  PriorOpen.Set(priordayOpen);
					if (ShowHigh)  PriorHigh.Set(priordayHigh);
            		if (ShowLow)   PriorLow.Set(priordayLow);
            		if (ShowClose) PriorClose.Set(priordayClose);
				}
				
				// Initilize the current day settings to the new days data
				currentOpen 	= 	Open[0];
				currentHigh 	= 	High[0];
				currentLow		=	Low[0];
				currentClose	=	Close[0];

				currentDate 	= 	Bars.GetTradingDayFromLocal(Time[0]); 
			}
			else // The current day is the same day
			{
				// Set the current day OHLC values
				currentHigh 	= 	Math.Max(currentHigh, High[0]);
				currentLow		= 	Math.Min(currentLow, Low[0]);
				currentClose	=	Close[0];
				
                if (ShowOpen) PriorOpen.Set(priordayOpen);
                if (ShowHigh) PriorHigh.Set(priordayHigh);
                if (ShowLow) PriorLow.Set(priordayLow);
                if (ShowClose) PriorClose.Set(priordayClose);
			}
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries PriorOpen
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries PriorHigh
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries PriorLow
        {
            get { return Values[2]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries PriorClose
        {
            get { return Values[3]; }
        }
		
		[Browsable(true)]
		[Gui.Design.DisplayNameAttribute("Show open")]
        public bool ShowOpen
        {
            get { return showOpen; }
			set { showOpen = value; }
        }
		
		[Browsable(true)]
		[Gui.Design.DisplayNameAttribute("Show high")]
        public bool ShowHigh
        {
            get { return showHigh; }
			set { showHigh = value; }
        }
		
		[Browsable(true)]
		[Gui.Design.DisplayNameAttribute("Show low")]
        public bool ShowLow
        {
            get { return showLow; }
			set { showLow = value; }
        }
		
		[Browsable(true)]
		[Gui.Design.DisplayNameAttribute("Show close")]
        public bool ShowClose
        {
            get { return showClose; }
			set { showClose = value; }
        }

        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private PriorDayOHLC[] cachePriorDayOHLC = null;

        private static PriorDayOHLC checkPriorDayOHLC = new PriorDayOHLC();

        /// <summary>
        /// Plots the open, high, low and close values from the session starting on the prior day.
        /// </summary>
        /// <returns></returns>
        public PriorDayOHLC PriorDayOHLC()
        {
            return PriorDayOHLC(Input);
        }

        /// <summary>
        /// Plots the open, high, low and close values from the session starting on the prior day.
        /// </summary>
        /// <returns></returns>
        public PriorDayOHLC PriorDayOHLC(Data.IDataSeries input)
        {
            if (cachePriorDayOHLC != null)
                for (int idx = 0; idx < cachePriorDayOHLC.Length; idx++)
                    if (cachePriorDayOHLC[idx].EqualsInput(input))
                        return cachePriorDayOHLC[idx];

            lock (checkPriorDayOHLC)
            {
                if (cachePriorDayOHLC != null)
                    for (int idx = 0; idx < cachePriorDayOHLC.Length; idx++)
                        if (cachePriorDayOHLC[idx].EqualsInput(input))
                            return cachePriorDayOHLC[idx];

                PriorDayOHLC indicator = new PriorDayOHLC();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                PriorDayOHLC[] tmp = new PriorDayOHLC[cachePriorDayOHLC == null ? 1 : cachePriorDayOHLC.Length + 1];
                if (cachePriorDayOHLC != null)
                    cachePriorDayOHLC.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachePriorDayOHLC = tmp;
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
        /// Plots the open, high, low and close values from the session starting on the prior day.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PriorDayOHLC PriorDayOHLC()
        {
            return _indicator.PriorDayOHLC(Input);
        }

        /// <summary>
        /// Plots the open, high, low and close values from the session starting on the prior day.
        /// </summary>
        /// <returns></returns>
        public Indicator.PriorDayOHLC PriorDayOHLC(Data.IDataSeries input)
        {
            return _indicator.PriorDayOHLC(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Plots the open, high, low and close values from the session starting on the prior day.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PriorDayOHLC PriorDayOHLC()
        {
            return _indicator.PriorDayOHLC(Input);
        }

        /// <summary>
        /// Plots the open, high, low and close values from the session starting on the prior day.
        /// </summary>
        /// <returns></returns>
        public Indicator.PriorDayOHLC PriorDayOHLC(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.PriorDayOHLC(input);
        }
    }
}
#endregion
