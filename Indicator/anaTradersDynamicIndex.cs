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
	/// The Traders Dynamic Index (TDI) by Dean Malone is a trend following momentum indicator based on the RSI. /// </summary>
	[Description("The Traders Dynamic Index (TDI) by Dean Malone is a trend following momentum indicator based on the RSI.")]
	public class anaTradersDynamicIndex : Indicator
	{
		#region Variables
		
		private int				rsiPeriod 			= 13;
		private int				pricePeriod			= 2;
		private int				signalPeriod		= 7;
		private int				bandPeriod			= 34;
		private double			stdDevNumber		= 1.62;
		private double			stdDevValue			= 0.0;
		private double			avg					= 0.0;
		private Color			main				= Color.Lime;
		private Color			signal				= Color.Red;
		private Color			bbAverage			= Color.Gold;
		private Color			bbUpper				= Color.MediumPurple;
		private Color			bbLower				= Color.MediumPurple;
		private Color			baselineColor		= Color.DarkGray;
		private Color			upperlineColor		= Color.DarkGray;
		private Color			lowerlineColor		= Color.DarkGray;		
		private int 			plot0Width 			= 2;
		private PlotStyle 		plot0Style		 	= PlotStyle.Line;
		private DashStyle 		dash0Style 			= DashStyle.Solid;
		private int 			plot1Width 			= 2;
		private PlotStyle 		plot1Style 			= PlotStyle.Line;
		private DashStyle 		dash1Style 			= DashStyle.Solid;
		private int 			plot2Width 			= 2;
		private PlotStyle 		plot2Style 			= PlotStyle.Line;
		private DashStyle		dash2Style 			= DashStyle.Solid;
		private int 			plot3Width 			= 1;
		private PlotStyle		plot3Style 			= PlotStyle.Line;
		private DashStyle 		dash3Style 			= DashStyle.Solid;
		private int 			plot4Width 			= 3;
		private PlotStyle 		plot4Style 			= PlotStyle.Square;
		private DashStyle 		dash4Style 			= DashStyle.Dash;
		private int				valueUpperLine		= 68;
		private int				valueLowerLine		= 32;
		private int				baselineWidth		= 1;
		private DashStyle		baselineStyle		= DashStyle.Dash;
		private int				upperlineWidth		= 1;
		private DashStyle		upperlineStyle		= DashStyle.Dash;
		private int				lowerlineWidth		= 1;
		private DashStyle		lowerlineStyle		= DashStyle.Dash;
			
		private RSI DYNRSI;
		private SMA DYNPrice;
		private SMA DYNSignal; 
		private SMA	DYNAverage; 
		private StdDev SDBB;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Gray, 2), PlotStyle.Line, "PriceLine"));
			Add(new Plot(new Pen(Color.Gray, 2), PlotStyle.Line, "Signalline"));
			Add(new Plot(new Pen(Color.Gray, 2), PlotStyle.Line, "Average"));
			Add(new Plot(new Pen(Color.Gray, 1), PlotStyle.Line, "Upper"));
			Add(new Plot(new Pen(Color.Gray, 1), PlotStyle.Line, "Lower"));
			Add(new Line(Color.Gray, 50, "Baseline"));
			Add(new Line(Color.Gray, 68, "Upper"));
			Add(new Line(Color.Gray, 32, "Lower"));
			
			PlotsConfigurable = false;
			LinesConfigurable = false;
		}
	
		/// <summary>
		/// Calculates the indicator value(s) at the current index.
		/// </summary>
		protected override void OnStartUp()
		{
			DYNRSI = RSI(Input,RSIPeriod,1);
			DYNPrice = SMA(DYNRSI,PricePeriod);
			DYNSignal = SMA(DYNRSI,SignalPeriod);
			DYNAverage = SMA(DYNRSI, BandPeriod);
			SDBB = StdDev(DYNRSI,BandPeriod);
			Plots[0].Pen.Color = main;
			Plots[1].Pen.Color = signal;
			Plots[2].Pen.Color = bbAverage;
			Plots[3].Pen.Color = bbUpper;
			Plots[4].Pen.Color = bbLower;
			Plots[0].Pen.Width = plot0Width;
			Plots[0].PlotStyle = plot0Style;
			Plots[0].Pen.DashStyle = dash0Style;
			Plots[1].Pen.Width= plot1Width;
			Plots[1].PlotStyle = plot1Style;
			Plots[1].Pen.DashStyle = dash1Style;
			Plots[2].Pen.Width = plot2Width;
			Plots[2].PlotStyle = plot2Style;
			Plots[2].Pen.DashStyle = dash2Style;
			Plots[3].Pen.Width= plot3Width;
			Plots[3].PlotStyle = plot3Style;
			Plots[3].Pen.DashStyle = dash3Style;
			Plots[4].Pen.Width= plot3Width;
			Plots[4].PlotStyle = plot3Style;
			Plots[4].Pen.DashStyle = dash3Style;
			Lines[0].Pen.Color = baselineColor;
			Lines[1].Pen.Color = upperlineColor;
			Lines[2].Pen.Color = lowerlineColor;
			Lines[0].Pen.Width = baselineWidth;
			Lines[0].Pen.DashStyle = baselineStyle;
			Lines[1].Value = valueUpperLine;
			Lines[1].Pen.Width = upperlineWidth;
			Lines[1].Pen.DashStyle = upperlineStyle;
			Lines[2].Value = valueLowerLine;
			Lines[2].Pen.Width = lowerlineWidth;
			Lines[2].Pen.DashStyle = lowerlineStyle;
		}
		
		protected override void OnBarUpdate()
		{
			avg = DYNAverage[0];
			stdDevValue = SDBB[0];
			PriceLine.Set(DYNPrice[0]);
			SignalLine.Set(DYNSignal[0]);
			Average.Set(avg);
			Upper.Set(avg + stdDevNumber * stdDevValue);
			Lower.Set(avg - stdDevNumber * stdDevValue);
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries PriceLine
		{
			get { return Values[0]; }
		}

        /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries SignalLine
		{
			get { return Values[1]; }
		}

       /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Average
		{
			get { return Values[2]; }
		}

        /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Upper
		{
			get { return Values[3]; }
		}

        /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Lower
		{
			get { return Values[4]; }
		}

		/// <summary>
		/// </summary>
		[Description("Period for RSI")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Period for RSI")]
		public int RSIPeriod
		{
			get { return rsiPeriod; }
			set { rsiPeriod = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Period for Priceline")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Period for priceline")]
		public int PricePeriod
		{
			get { return pricePeriod; }
			set { pricePeriod = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Period for Signalline")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Period for signalline")]
		public int SignalPeriod
		{
			get { return signalPeriod; }
			set { signalPeriod = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Band Period for Bollinger Band")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Period for volabands")]
		public int BandPeriod
		{
			get { return bandPeriod; }
			set { bandPeriod = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Number of standard deviations")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("# std. dev.")]
		public double StdDevNumber
		{
			get { return stdDevNumber; }
			set { stdDevNumber = Math.Max(0, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Select Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("Priceline")]
		public Color Main
		{
			get { return main; }
			set { main = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string MainSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(main); }
			set { main = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Description("Select Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("Signalline")]
		public Color Signal
		{
			get { return signal; }
			set { signal = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string SignalSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(signal); }
			set { signal = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Description("Select Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("Bollinger Average")]
		public Color BBAverage
		{
			get { return bbAverage; }
			set { bbAverage = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string BBAverageSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(bbAverage); }
			set { bbAverage = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Description("Select Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("Bollinger Upper Band")]
		public Color BBUpper
		{
			get { return bbUpper; }
			set { bbUpper = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string BBUpperSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(bbUpper); }
			set { bbUpper = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Description("Select Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("Bollinger Lower Band")]
		public Color BBLower
		{
			get { return bbLower; }
			set { bbLower = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string BBLowerSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(bbLower); }
			set { bbLower = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Description("Width for Priceline.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Line Width Priceline")]
		public int Plot0Width
		{
			get { return plot0Width; }
			set { plot0Width = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("PlotStyle for Priceline.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Plot Style Priceline")]
		public PlotStyle Plot0Style
		{
			get { return plot0Style; }
			set { plot0Style = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for Priceline.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Dash Style Priceline")]
		public DashStyle Dash0Style
		{
			get { return dash0Style; }
			set { dash0Style = value; }
		} 
		
		/// <summary>
		/// </summary>
		[Description("Width for Signalline.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Line Width Signal")]
		public int Plot1Width
		{
			get { return plot1Width; }
			set { plot1Width = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("PlotStyle for Signalline.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Plot Style Signal")]
		public PlotStyle Plot1Style
		{
			get { return plot1Style; }
			set { plot1Style = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for Signalline.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Dash Style Signal")]
		public DashStyle Dash1Style
		{
			get { return dash1Style; }
			set { dash1Style = value; }
		} 

		/// <summary>
		/// </summary>
		[Description("Width for Midband.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Line Width Midband")]
		public int Plot2Width
		{
			get { return plot2Width; }
			set { plot2Width = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("PlotStyle for Midband.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Plot Style Midband")]
		public PlotStyle Plot2Style
		{
			get { return plot2Style; }
			set { plot2Style = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for MidBand.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Dash Style Midband")]
		public DashStyle Dash2Style
		{
			get { return dash2Style; }
			set { dash2Style = value; }
		} 
		
		/// <summary>
		/// </summary>
		[Description("Width for Bollinger Bands.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Line Width BBAnds")]
		public int Plot3Width
		{
			get { return plot3Width; }
			set { plot3Width = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("PlotStyle for Bollinger Bands.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Plot Style BBAnds")]
		public PlotStyle Plot3Style
		{
			get { return plot3Style; }
			set { plot3Style = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for Bollinger Bands.")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Dash Style BBands")]
		public DashStyle Dash3Style
		{
			get { return dash3Style; }
			set { dash3Style = value; }
		} 
		
		/// <summary>
		/// </summary>
		[Description("Select Color")]
		[Category("Line Colors")]
		[Gui.Design.DisplayName("Baseline")]
		public Color BaselineColor
		{
			get { return baselineColor; }
			set { baselineColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string BaselineColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(baselineColor); }
			set { baselineColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
				
		/// <summary>
		/// </summary>
		[Description("Select Color")]
		[Category("Line Colors")]
		[Gui.Design.DisplayName("Upper line")]
		public Color UpperlineColor
		{
			get { return upperlineColor; }
			set { upperlineColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string UpperlineColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(upperlineColor); }
			set { upperlineColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
				
		/// <summary>
		/// </summary>
		[Description("Select Color")]
		[Category("Line Colors")]
		[Gui.Design.DisplayName("Lower line")]
		public Color LowerlineColor
		{
			get { return lowerlineColor; }
			set { lowerlineColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string LowerlineColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(lowerlineColor); }
			set { lowerlineColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// </summary>
		[Description("Value for upper line.")]
		[Category("Line Parameters")]
		[Gui.Design.DisplayNameAttribute("Value upper line")]
		public int ValueUpperLine
		{
			get { return valueUpperLine; }
			set { valueUpperLine = Math.Min(100, Math.Max(50, value)); }
		}
		
		/// </summary>
		[Description("Value for lower line.")]
		[Category("Line Parameters")]
		[Gui.Design.DisplayNameAttribute("Value lower line")]
		public int ValueLowerLine
		{
			get { return valueLowerLine; }
			set { valueLowerLine = Math.Min(50, Math.Max(0, value)); }
		}
		
		/// </summary>
		[Description("Width for baseline.")]
		[Category("Line Parameters")]
		[Gui.Design.DisplayNameAttribute("Width baseline")]
		public int BaselineWidth
		{
			get { return baselineWidth; }
			set { baselineWidth = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for baseline.")]
		[Category("Line Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash style baseline")]
		public DashStyle BaselineStyle
		{
			get { return baselineStyle; }
			set { baselineStyle = value; }
		} 		
		
		/// </summary>
		[Description("Width for upper line.")]
		[Category("Line Parameters")]
		[Gui.Design.DisplayNameAttribute("Width upper line")]
		public int UpperlineWidth
		{
			get { return upperlineWidth; }
			set { upperlineWidth = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for upper line.")]
		[Category("Line Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash style upper line")]
		public DashStyle UpperlineStyle
		{
			get { return upperlineStyle; }
			set { upperlineStyle = value; }
		} 		
		
		/// </summary>
		[Description("Width for lower line.")]
		[Category("Line Parameters")]
		[Gui.Design.DisplayNameAttribute("Width lower line")]
		public int LowerlineWidth
		{
			get { return lowerlineWidth; }
			set { lowerlineWidth = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for lower line.")]
		[Category("Line Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash style lower line")]
		public DashStyle LowerlineStyle
		{
			get { return lowerlineStyle; }
			set { lowerlineStyle = value; }
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
        private anaTradersDynamicIndex[] cacheanaTradersDynamicIndex = null;

        private static anaTradersDynamicIndex checkanaTradersDynamicIndex = new anaTradersDynamicIndex();

        /// <summary>
        /// The Traders Dynamic Index (TDI) by Dean Malone is a trend following momentum indicator based on the RSI.
        /// </summary>
        /// <returns></returns>
        public anaTradersDynamicIndex anaTradersDynamicIndex(int bandPeriod, int pricePeriod, int rSIPeriod, int signalPeriod, double stdDevNumber)
        {
            return anaTradersDynamicIndex(Input, bandPeriod, pricePeriod, rSIPeriod, signalPeriod, stdDevNumber);
        }

        /// <summary>
        /// The Traders Dynamic Index (TDI) by Dean Malone is a trend following momentum indicator based on the RSI.
        /// </summary>
        /// <returns></returns>
        public anaTradersDynamicIndex anaTradersDynamicIndex(Data.IDataSeries input, int bandPeriod, int pricePeriod, int rSIPeriod, int signalPeriod, double stdDevNumber)
        {
            if (cacheanaTradersDynamicIndex != null)
                for (int idx = 0; idx < cacheanaTradersDynamicIndex.Length; idx++)
                    if (cacheanaTradersDynamicIndex[idx].BandPeriod == bandPeriod && cacheanaTradersDynamicIndex[idx].PricePeriod == pricePeriod && cacheanaTradersDynamicIndex[idx].RSIPeriod == rSIPeriod && cacheanaTradersDynamicIndex[idx].SignalPeriod == signalPeriod && Math.Abs(cacheanaTradersDynamicIndex[idx].StdDevNumber - stdDevNumber) <= double.Epsilon && cacheanaTradersDynamicIndex[idx].EqualsInput(input))
                        return cacheanaTradersDynamicIndex[idx];

            lock (checkanaTradersDynamicIndex)
            {
                checkanaTradersDynamicIndex.BandPeriod = bandPeriod;
                bandPeriod = checkanaTradersDynamicIndex.BandPeriod;
                checkanaTradersDynamicIndex.PricePeriod = pricePeriod;
                pricePeriod = checkanaTradersDynamicIndex.PricePeriod;
                checkanaTradersDynamicIndex.RSIPeriod = rSIPeriod;
                rSIPeriod = checkanaTradersDynamicIndex.RSIPeriod;
                checkanaTradersDynamicIndex.SignalPeriod = signalPeriod;
                signalPeriod = checkanaTradersDynamicIndex.SignalPeriod;
                checkanaTradersDynamicIndex.StdDevNumber = stdDevNumber;
                stdDevNumber = checkanaTradersDynamicIndex.StdDevNumber;

                if (cacheanaTradersDynamicIndex != null)
                    for (int idx = 0; idx < cacheanaTradersDynamicIndex.Length; idx++)
                        if (cacheanaTradersDynamicIndex[idx].BandPeriod == bandPeriod && cacheanaTradersDynamicIndex[idx].PricePeriod == pricePeriod && cacheanaTradersDynamicIndex[idx].RSIPeriod == rSIPeriod && cacheanaTradersDynamicIndex[idx].SignalPeriod == signalPeriod && Math.Abs(cacheanaTradersDynamicIndex[idx].StdDevNumber - stdDevNumber) <= double.Epsilon && cacheanaTradersDynamicIndex[idx].EqualsInput(input))
                            return cacheanaTradersDynamicIndex[idx];

                anaTradersDynamicIndex indicator = new anaTradersDynamicIndex();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BandPeriod = bandPeriod;
                indicator.PricePeriod = pricePeriod;
                indicator.RSIPeriod = rSIPeriod;
                indicator.SignalPeriod = signalPeriod;
                indicator.StdDevNumber = stdDevNumber;
                Indicators.Add(indicator);
                indicator.SetUp();

                anaTradersDynamicIndex[] tmp = new anaTradersDynamicIndex[cacheanaTradersDynamicIndex == null ? 1 : cacheanaTradersDynamicIndex.Length + 1];
                if (cacheanaTradersDynamicIndex != null)
                    cacheanaTradersDynamicIndex.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheanaTradersDynamicIndex = tmp;
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
        /// The Traders Dynamic Index (TDI) by Dean Malone is a trend following momentum indicator based on the RSI.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaTradersDynamicIndex anaTradersDynamicIndex(int bandPeriod, int pricePeriod, int rSIPeriod, int signalPeriod, double stdDevNumber)
        {
            return _indicator.anaTradersDynamicIndex(Input, bandPeriod, pricePeriod, rSIPeriod, signalPeriod, stdDevNumber);
        }

        /// <summary>
        /// The Traders Dynamic Index (TDI) by Dean Malone is a trend following momentum indicator based on the RSI.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaTradersDynamicIndex anaTradersDynamicIndex(Data.IDataSeries input, int bandPeriod, int pricePeriod, int rSIPeriod, int signalPeriod, double stdDevNumber)
        {
            return _indicator.anaTradersDynamicIndex(input, bandPeriod, pricePeriod, rSIPeriod, signalPeriod, stdDevNumber);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Traders Dynamic Index (TDI) by Dean Malone is a trend following momentum indicator based on the RSI.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaTradersDynamicIndex anaTradersDynamicIndex(int bandPeriod, int pricePeriod, int rSIPeriod, int signalPeriod, double stdDevNumber)
        {
            return _indicator.anaTradersDynamicIndex(Input, bandPeriod, pricePeriod, rSIPeriod, signalPeriod, stdDevNumber);
        }

        /// <summary>
        /// The Traders Dynamic Index (TDI) by Dean Malone is a trend following momentum indicator based on the RSI.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaTradersDynamicIndex anaTradersDynamicIndex(Data.IDataSeries input, int bandPeriod, int pricePeriod, int rSIPeriod, int signalPeriod, double stdDevNumber)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.anaTradersDynamicIndex(input, bandPeriod, pricePeriod, rSIPeriod, signalPeriod, stdDevNumber);
        }
    }
}
#endregion
