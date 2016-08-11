	#region Using declarations
	using System;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Xml.Serialization;
	using NinjaTrader.Cbi;
	using NinjaTrader.Data;
	using NinjaTrader.Gui.Chart;
	using System.Reflection;
	#endregion

	// This namespace holds all indicators and is required. Do not change it.
	namespace NinjaTrader.Indicator
	{
		/// <summary>
		/// Alert when price hits a trend line. Bid when price is below a trend line, and Ask when price is above a trend line. 
		/// </summary>
		[Description("Alert when price hits a trend line. Bid when price is below a trend line, and Ask when price is above a trend line. ")]
		public class TrendLineAlert : Indicator
		{
			#region Variables
			// Wizard generated variables
				private string trendLineTag = "trigger"; // Default setting for TrendLineTag
				private string alertSound = "CompiledSuccessfully.wav";
			// User defined variables (add any user defined variables below)
			private string type = null;
				private bool alert_done = false;
				double previous_bid = -1;
			private DateTime sTime1,sTime2;
			private double sY1,sY2;
			#endregion

			/// <summary>
			/// This method is used to configure the indicator and is called once before any bar data is loaded.
			/// </summary>
			protected override void Initialize()
			{
				//Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "Plot0"));
				Overlay				= true;
				CalculateOnBarClose = false;
			
			}

			/// <summary>
			/// Called on each bar update event (incoming tick)
			/// </summary>
			protected override void OnBarUpdate()
			{
			
				//If we are not processing the current bar, return
				if (CurrentBar !=Bars.GetBar(DateTime.Now)) {
					return;
				}
				
				//Get current bid 	
					double bid = Bars.GetClose(Bars.GetBar(DateTime.Now));
					if (previous_bid<0) {previous_bid = bid;}
					
					
				IDrawObject drawing_object = DrawObjects[trendLineTag];
			
			//Reset if drawing object is deleted or does not exist... 
			if (drawing_object ==null) {
				Type = null; AlertDone = false;
			} 
			
			if (drawing_object !=null && (drawing_object.DrawType==DrawType.Ray ||
										drawing_object.DrawType == DrawType.ExtendedLine ||
										drawing_object.DrawType == DrawType.Line)) {
				
											
				double y1, y2;
				//int bar1,bar2;							
				DateTime time1,time2;
											
				double end_bar_double = (double)typeof(ChartLine).GetProperty("endBarDouble",BindingFlags.NonPublic | BindingFlags.Instance).GetValue(drawing_object,null);
				double start_bar_double = (double)typeof(ChartLine).GetProperty("startBarDouble",BindingFlags.NonPublic | BindingFlags.Instance).GetValue(drawing_object,null);
				
			    if (drawing_object.DrawType==DrawType.Ray) {
					IRay ray = (IRay)drawing_object;
					y1 = ray.Anchor1Y;
					y2 = ray.Anchor2Y;
					time1 = ray.Anchor1Time;
					time2 = ray.Anchor2Time;
					//bar1 = ray.Anchor1BarsAgo;
					//bar2 = ray.Anchor2BarsAgo;
				}
				else if (drawing_object.DrawType ==DrawType.ExtendedLine) {
					IExtendedLine line = (IExtendedLine)drawing_object;
					y1 = line.StartY;
					y2 = line.EndY;
					time1 = line.StartTime;
					time2 = line.EndTime;
					//bar1 = line.StartBarsAgo;
					//bar2 = line.EndBarsAgo;
				} else  {
					ILine line = (ILine)drawing_object;
					y1 = line.StartY;
					y2 = line.EndY;
					time1 = line.StartTime;
					time2 = line.EndTime;
					//bar1 = line.StartBarsAgo;
					//bar2 = line.EndBarsAgo;
				}
					
				
				//Reset alert if object has moved
				if ((time1!= this.sTime1 || time2!=this.sTime2 || y1!=this.sY1 || y2 !=this.sY2) && this.sTime1 !=null && this.sTime2 !=null) {
					Type  = null; AlertDone = false; 
				} 
				
				if (AlertDone == false) {
					//Store anchor to detect if object has moved
					this.sTime1 = time1; this.sTime2 = time2; this.sY1 = y1; this.sY2 = y2;
					
					//Calculate price target		
					double y = y2-y1;
					double x = end_bar_double-start_bar_double;
					double slope = y/x;
		
					//double deltaY = bar2*slope;   //time difference in ticks * slope 
					double price_target = Math.Round(y1+slope * (CurrentBar-start_bar_double),5);

					//IF price is below current price target or straddles it, THEN we alert when the bid>=price_target
					if (Type == null && (price_target - previous_bid)>=0) {
						Type = "up";
					}
					if (Type == null && (previous_bid - price_target)>=0) {
						Type = "down";
					}
					
					previous_bid = bid;
	
					if (Type=="up" && bid>=price_target) {
						Alert("TrendLineAlert", NinjaTrader.Cbi.Priority.High, "Reached Trend Line @ " + price_target, AlertSound, 10, Color.Black, Color.Yellow);
						AlertDone = true;
					} 
					if (Type=="down" && bid<=price_target) {
						Alert("TrendLineAlert", NinjaTrader.Cbi.Priority.High, "Reached Trend Line @ " + price_target, AlertSound, 10, Color.Black, Color.Yellow);
						AlertDone = true; 
					}
				}
					
				}
			}

			
			
			protected bool AlertDone 
			{
				get { return alert_done;}
				set { alert_done = value;}
			}
			
			#region Properties
			[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
			[XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
			public DataSeries Plot0
			{
				get { return Values[0]; }
			}

			
			
			
			
			protected string Type 
			{
				get {return type;}
				set {type = value;}
			}
			
			[Description("")]
			[GridCategory("Parameters")]
			public string TrendLineTag
			{
				get { return trendLineTag; }
				set { trendLineTag = value; }
			}

			[Description("")]
			[GridCategory("Parameters")]
			public string AlertSound {
				get{return alertSound;}
				set{alertSound = value;
				}}
			
			#endregion
		}
	}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private TrendLineAlert[] cacheTrendLineAlert = null;

        private static TrendLineAlert checkTrendLineAlert = new TrendLineAlert();

        /// <summary>
        /// Alert when price hits a trend line. Bid when price is below a trend line, and Ask when price is above a trend line. 
        /// </summary>
        /// <returns></returns>
        public TrendLineAlert TrendLineAlert(string alertSound, string trendLineTag)
        {
            return TrendLineAlert(Input, alertSound, trendLineTag);
        }

        /// <summary>
        /// Alert when price hits a trend line. Bid when price is below a trend line, and Ask when price is above a trend line. 
        /// </summary>
        /// <returns></returns>
        public TrendLineAlert TrendLineAlert(Data.IDataSeries input, string alertSound, string trendLineTag)
        {
            if (cacheTrendLineAlert != null)
                for (int idx = 0; idx < cacheTrendLineAlert.Length; idx++)
                    if (cacheTrendLineAlert[idx].AlertSound == alertSound && cacheTrendLineAlert[idx].TrendLineTag == trendLineTag && cacheTrendLineAlert[idx].EqualsInput(input))
                        return cacheTrendLineAlert[idx];

            lock (checkTrendLineAlert)
            {
                checkTrendLineAlert.AlertSound = alertSound;
                alertSound = checkTrendLineAlert.AlertSound;
                checkTrendLineAlert.TrendLineTag = trendLineTag;
                trendLineTag = checkTrendLineAlert.TrendLineTag;

                if (cacheTrendLineAlert != null)
                    for (int idx = 0; idx < cacheTrendLineAlert.Length; idx++)
                        if (cacheTrendLineAlert[idx].AlertSound == alertSound && cacheTrendLineAlert[idx].TrendLineTag == trendLineTag && cacheTrendLineAlert[idx].EqualsInput(input))
                            return cacheTrendLineAlert[idx];

                TrendLineAlert indicator = new TrendLineAlert();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AlertSound = alertSound;
                indicator.TrendLineTag = trendLineTag;
                Indicators.Add(indicator);
                indicator.SetUp();

                TrendLineAlert[] tmp = new TrendLineAlert[cacheTrendLineAlert == null ? 1 : cacheTrendLineAlert.Length + 1];
                if (cacheTrendLineAlert != null)
                    cacheTrendLineAlert.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTrendLineAlert = tmp;
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
        /// Alert when price hits a trend line. Bid when price is below a trend line, and Ask when price is above a trend line. 
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TrendLineAlert TrendLineAlert(string alertSound, string trendLineTag)
        {
            return _indicator.TrendLineAlert(Input, alertSound, trendLineTag);
        }

        /// <summary>
        /// Alert when price hits a trend line. Bid when price is below a trend line, and Ask when price is above a trend line. 
        /// </summary>
        /// <returns></returns>
        public Indicator.TrendLineAlert TrendLineAlert(Data.IDataSeries input, string alertSound, string trendLineTag)
        {
            return _indicator.TrendLineAlert(input, alertSound, trendLineTag);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Alert when price hits a trend line. Bid when price is below a trend line, and Ask when price is above a trend line. 
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TrendLineAlert TrendLineAlert(string alertSound, string trendLineTag)
        {
            return _indicator.TrendLineAlert(Input, alertSound, trendLineTag);
        }

        /// <summary>
        /// Alert when price hits a trend line. Bid when price is below a trend line, and Ask when price is above a trend line. 
        /// </summary>
        /// <returns></returns>
        public Indicator.TrendLineAlert TrendLineAlert(Data.IDataSeries input, string alertSound, string trendLineTag)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TrendLineAlert(input, alertSound, trendLineTag);
        }
    }
}
#endregion
