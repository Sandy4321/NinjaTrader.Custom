#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Draws vertical lines at specified time intervals
    /// </summary>
    [Description("Draws vertical lines at specified time intervals")]
    public class TimeLines : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int numMin = 5; // Default setting for NumMin
        // User defined variables (add any user defined variables below)
			private Color 		lineColor = Color.Blue;
			private DashStyle	lineStyle = DashStyle.Dot;
			private int			lineWidth = 2;
			private bool	setupComplete = false;
		
		// internal variables
			private DateTime	markTime;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			BarsRequired		= 0;
            CalculateOnBarClose	= true;
            Overlay				= true;
            PriceTypeSupported	= false;
			ChartOnly			= true;
			DrawOnPricePanel 	= true;
			DisplayInDataBox 	= false;
			
			// remember DateTime.Now is ALWAYS in local computer time, so cannot use: FAILS replay
			//markTime = DateTime.Now;
			//markTime = markTime.AddSeconds(-markTime.Second);  // truncate time to minutes
			//markTime = markTime.AddMilliseconds(-markTime.Millisecond);
			
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {	
			if (CurrentBar<1) return;		
			
			if (!setupComplete) // run once to set up the chart existing bars
			{						
				int i = CurrentBar;		// i = barsago value
				markTime = Time[i];		//  initialize the marker to the beginning bar
				markTime = markTime.AddSeconds(-markTime.Second);  // truncate time to whole minutes
				markTime = markTime.AddMilliseconds(-markTime.Millisecond);
				markTime = markTime.AddMinutes(-markTime.Minute);  // truncate time to whole hours
			
				while (i>0)	// look for the next boundary 
				{
					do markTime  = markTime.AddMinutes(numMin);	 // and update marker for next Time Line
					while (DateTime.Compare(markTime,Time[i])<=0);				
										// t1,t2 ==    earlier <  same =   later >
					DrawVerticalLine("T"+(CurrentBar-i), i, lineColor, lineStyle, lineWidth);					
					i--;  // move forward to next bar
				}				
				setupComplete = true;   // all done with set up
			}
							
		  	// remember DateTime.Now is ALWAYS in local computer time, so use Time[0] as current bar time
			                  // t1,t2 ==    earlier <  same =   later >
		  	if (DateTime.Compare(markTime,Time[0])<=0)	// Time has reached or passed the next marker, so
		  	{
		  		DrawVerticalLine("T"+CurrentBar, 0, lineColor, lineStyle, lineWidth);	
				do 
		  			markTime  = markTime.AddMinutes(numMin);	 // and update marker for next Time Line
				while (DateTime.Compare(markTime,Time[0])<=0);
		  	}
      	 		}

        #region Properties

        [Description("number of minutes to separate the vertical Time Lines")]
        [Category("Parameters")]
        public int Minutes
        {
            get { return numMin; }
            set { numMin = Math.Max(1, value); }
        }
		
	 	[Description("Color of the vertical Time Line")]
        [Category("Line Parameters")]
		public Color LineColor
		{
     		get { return lineColor; }
     		set { lineColor = value; }
		}
		[Browsable(false)]
		public string LineColorSerialize
		{
     		get { return NinjaTrader.Gui.Design.SerializableColor.ToString(lineColor); }
     		set { lineColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}	
		
		[Description("Style of the vertical Time Line")]
        [Category("Line Parameters")]
		public DashStyle LineStyle
		{
     		get { return lineStyle; }
     		set { lineStyle = value; }
		}
		
		[Description("Width of the vertical Time Line")]
        [Category("Line Parameters")]
        public int LineWidth
        {
            get { return lineWidth; }
            set { lineWidth = Math.Max(1, value); }
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
        private TimeLines[] cacheTimeLines = null;

        private static TimeLines checkTimeLines = new TimeLines();

        /// <summary>
        /// Draws vertical lines at specified time intervals
        /// </summary>
        /// <returns></returns>
        public TimeLines TimeLines(int minutes)
        {
            return TimeLines(Input, minutes);
        }

        /// <summary>
        /// Draws vertical lines at specified time intervals
        /// </summary>
        /// <returns></returns>
        public TimeLines TimeLines(Data.IDataSeries input, int minutes)
        {
            if (cacheTimeLines != null)
                for (int idx = 0; idx < cacheTimeLines.Length; idx++)
                    if (cacheTimeLines[idx].Minutes == minutes && cacheTimeLines[idx].EqualsInput(input))
                        return cacheTimeLines[idx];

            lock (checkTimeLines)
            {
                checkTimeLines.Minutes = minutes;
                minutes = checkTimeLines.Minutes;

                if (cacheTimeLines != null)
                    for (int idx = 0; idx < cacheTimeLines.Length; idx++)
                        if (cacheTimeLines[idx].Minutes == minutes && cacheTimeLines[idx].EqualsInput(input))
                            return cacheTimeLines[idx];

                TimeLines indicator = new TimeLines();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Minutes = minutes;
                Indicators.Add(indicator);
                indicator.SetUp();

                TimeLines[] tmp = new TimeLines[cacheTimeLines == null ? 1 : cacheTimeLines.Length + 1];
                if (cacheTimeLines != null)
                    cacheTimeLines.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTimeLines = tmp;
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
        /// Draws vertical lines at specified time intervals
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TimeLines TimeLines(int minutes)
        {
            return _indicator.TimeLines(Input, minutes);
        }

        /// <summary>
        /// Draws vertical lines at specified time intervals
        /// </summary>
        /// <returns></returns>
        public Indicator.TimeLines TimeLines(Data.IDataSeries input, int minutes)
        {
            return _indicator.TimeLines(input, minutes);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Draws vertical lines at specified time intervals
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TimeLines TimeLines(int minutes)
        {
            return _indicator.TimeLines(Input, minutes);
        }

        /// <summary>
        /// Draws vertical lines at specified time intervals
        /// </summary>
        /// <returns></returns>
        public Indicator.TimeLines TimeLines(Data.IDataSeries input, int minutes)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TimeLines(input, minutes);
        }
    }
}
#endregion
