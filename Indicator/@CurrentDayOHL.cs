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
    /// Plots the open, high, and low values from the session starting on the current day.
    /// </summary>
    [Description("Plots the open, high, and low values from the session starting on the current day.")]
    public class CurrentDayOHL : Indicator
    {
        #region Variables
		private DateTime 	currentDate 	    =   Cbi.Globals.MinDate;
		private double		currentOpen			=	double.MinValue;
        private double		currentHigh			=	double.MinValue;
		private double		currentLow			=	double.MaxValue;
		private bool		plotCurrentValue	=	false;
		private bool		showOpen			=	true;
		private bool		showHigh			=	true;
		private bool		showLow				=	true;
		#endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(new Pen(Color.Orange, 2), PlotStyle.Square, "Current Open"));
			Add(new Plot(new Pen(Color.Green, 2), PlotStyle.Square, "Current High"));
			Add(new Plot(new Pen(Color.Red, 2), PlotStyle.Square, "Current Low"));
			Plots[0].Pen.DashStyle = DashStyle.Dash;
			Plots[1].Pen.DashStyle = DashStyle.Dash;
			Plots[2].Pen.DashStyle = DashStyle.Dash;
			
			AutoScale 			= false;
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (!Bars.BarsType.IsIntraday)
			{
				DrawTextFixed("error msg", "CurrentDayOHL only works on intraday intervals", TextPosition.BottomRight);
				return;
			}
			
			bool sameDay = true;

			if (currentDate != Bars.GetTradingDayFromLocal(Time[0]) || currentOpen == double.MinValue)
			{
				currentOpen 	= 	Open[0];
				currentHigh 	= 	High[0];
				currentLow		=	Low[0];
				sameDay         =   false;
			}

			currentHigh 	= 	Math.Max(currentHigh, High[0]);
			currentLow		= 	Math.Min(currentLow, Low[0]);

			if (ShowOpen)
			{
				if (!PlotCurrentValue || !sameDay)
					CurrentOpen.Set(currentOpen);
				else
					for (int idx = 0; idx < CurrentOpen.Count; idx++)
						CurrentOpen.Set(idx, currentOpen);
			}

			if (ShowHigh)
			{
				if (!PlotCurrentValue || currentHigh != High[0])
					CurrentHigh.Set(currentHigh);
				else
					for (int idx = 0; idx < CurrentHigh.Count; idx++)
						CurrentHigh.Set(idx, currentHigh);
			}

			if (ShowLow)
			{
				if (!PlotCurrentValue || currentLow != Low[0])
					CurrentLow.Set(currentLow);
				else
					for (int idx = 0; idx < CurrentLow.Count; idx++)
						CurrentLow.Set(idx, currentLow);
			}
			
			currentDate 	= 	Bars.GetTradingDayFromLocal(Time[0]); 
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries CurrentOpen
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries CurrentHigh
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries CurrentLow
        {
            get { return Values[2]; }
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
		[Gui.Design.DisplayNameAttribute("Plot current value only")]
		public bool PlotCurrentValue
		{
			get { return plotCurrentValue; }
			set { plotCurrentValue = value; }
		}
		
		[Browsable(true)]
		[Gui.Design.DisplayNameAttribute("Show low")]
        public bool ShowLow
        {
            get { return showLow; }
			set { showLow = value; }
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
        private CurrentDayOHL[] cacheCurrentDayOHL = null;

        private static CurrentDayOHL checkCurrentDayOHL = new CurrentDayOHL();

        /// <summary>
        /// Plots the open, high, and low values from the session starting on the current day.
        /// </summary>
        /// <returns></returns>
        public CurrentDayOHL CurrentDayOHL()
        {
            return CurrentDayOHL(Input);
        }

        /// <summary>
        /// Plots the open, high, and low values from the session starting on the current day.
        /// </summary>
        /// <returns></returns>
        public CurrentDayOHL CurrentDayOHL(Data.IDataSeries input)
        {
            if (cacheCurrentDayOHL != null)
                for (int idx = 0; idx < cacheCurrentDayOHL.Length; idx++)
                    if (cacheCurrentDayOHL[idx].EqualsInput(input))
                        return cacheCurrentDayOHL[idx];

            lock (checkCurrentDayOHL)
            {
                if (cacheCurrentDayOHL != null)
                    for (int idx = 0; idx < cacheCurrentDayOHL.Length; idx++)
                        if (cacheCurrentDayOHL[idx].EqualsInput(input))
                            return cacheCurrentDayOHL[idx];

                CurrentDayOHL indicator = new CurrentDayOHL();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                CurrentDayOHL[] tmp = new CurrentDayOHL[cacheCurrentDayOHL == null ? 1 : cacheCurrentDayOHL.Length + 1];
                if (cacheCurrentDayOHL != null)
                    cacheCurrentDayOHL.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheCurrentDayOHL = tmp;
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
        /// Plots the open, high, and low values from the session starting on the current day.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.CurrentDayOHL CurrentDayOHL()
        {
            return _indicator.CurrentDayOHL(Input);
        }

        /// <summary>
        /// Plots the open, high, and low values from the session starting on the current day.
        /// </summary>
        /// <returns></returns>
        public Indicator.CurrentDayOHL CurrentDayOHL(Data.IDataSeries input)
        {
            return _indicator.CurrentDayOHL(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Plots the open, high, and low values from the session starting on the current day.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.CurrentDayOHL CurrentDayOHL()
        {
            return _indicator.CurrentDayOHL(Input);
        }

        /// <summary>
        /// Plots the open, high, and low values from the session starting on the current day.
        /// </summary>
        /// <returns></returns>
        public Indicator.CurrentDayOHL CurrentDayOHL(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.CurrentDayOHL(input);
        }
    }
}
#endregion
