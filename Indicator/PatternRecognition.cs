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
    /// Candle Stick Pattern Recognittion
    /// </summary>

	
    [Description("Candle Stick Pattern Recognittion")]
    public class PatternRecognition : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int bbPeriods = 14; // Default setting for MyInput0
			private int sTDev = 2;
		
			private DataSeries bullDoji;
			private DataSeries bearDoji;
			private DataSeries bullBreakouts;
			private DataSeries bearBreakouts;
			private DataSeries bearishEngulfing;
			private DataSeries bullishEngulfing;
			private DataSeries threeOutsideDown;
			private DataSeries threeOutsideUp;
			private DataSeries darkCloudCover;
			private DataSeries eveningDojiStar;
			private DataSeries morningDojiStar;
			private DataSeries bearishHarami;
			private DataSeries bullishHarami;
			private DataSeries threeInsideDown;
			private DataSeries threeInsideUp;
			private DataSeries threeBlackCrows;
			private DataSeries threeWhiteSoldiers;
			private DataSeries eveningStar;
			private DataSeries piercingLine;
			private DataSeries insidebar;
		
		
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "Plot0"));
            CalculateOnBarClose	= true;
            Overlay				= true;
            PriceTypeSupported	= false;
			

			
			bullDoji			=	new DataSeries(this);
			bearDoji			=	new DataSeries(this);
			bullBreakouts		=	new DataSeries(this);
			bearBreakouts		=	new DataSeries(this);
			bearishEngulfing	=	new DataSeries(this);
			bullishEngulfing	=	new DataSeries(this);
			threeOutsideDown	=	new DataSeries(this);
			threeOutsideUp		=	new DataSeries(this);
			darkCloudCover		=	new DataSeries(this);
			eveningDojiStar		=	new DataSeries(this);
			morningDojiStar		=	new DataSeries(this);
			bearishHarami		=	new DataSeries(this);
			bullishHarami		=	new DataSeries(this);
			threeInsideDown		=	new DataSeries(this);
			threeInsideUp		=	new DataSeries(this);
			threeBlackCrows		=	new DataSeries(this);
			threeWhiteSoldiers	=	new DataSeries(this);
			eveningStar			=	new DataSeries(this);
			piercingLine		=	new DataSeries(this);
			insidebar			=	new DataSeries(this);
			
			
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar < 4) return;
			
	
/////////////////Doji			
			if ((Open[0] - Close[0]) == 0 )
			{
				if (Open[1] < Close [1])
				{
					DrawArrowDown("MyArrowDown"+CurrentBar, 0, High[0], Color.Red);
					DrawText("Bear Doji"+CurrentBar, "Bear Doji", 0, High[0]+5* TickSize, Color.Red);
					bearDoji.Set(1);
				}
				if (Open[1] > Close [1])
				{
					DrawArrowUp("MyArrowUp"+CurrentBar, 0, Low[0], Color.Lime);
					DrawText("BUll Doji"+CurrentBar, "BUll Doji", 0, Low[0]-5* TickSize, Color.Blue);
					bullDoji.Set(1);
				}
				
			}
			else
			{
					bearDoji.Set(0);
					bullDoji.Set(0);
			}
/////////////////Doji			
			
/////////////////Breakouts	
			if (Close[0] > Bollinger(sTDev, bbPeriods).Upper[0] && Close[1] > Bollinger(sTDev, bbPeriods).Upper[1] 
				&& Close [2] < Bollinger(sTDev, bbPeriods).Upper[2]
				)
				{
					DrawArrowUp("MyArrowUp"+CurrentBar, 0, Low[0], Color.Lime);
					DrawText("BUll Breakout"+CurrentBar, "BUll Breakout", 0, Low[0]-5* TickSize, Color.Blue);
					bullBreakouts.Set(1);
				}
			else	
				bullBreakouts.Set(0);
				
			if (Close[0] < Bollinger(sTDev, bbPeriods).Lower[0] && Close[1] < Bollinger(sTDev, bbPeriods).Lower[1] 
				&& Close [2] > Bollinger(sTDev, bbPeriods).Lower[2]
				)
				{
					DrawArrowDown("MyArrowDown"+CurrentBar, 0, High[0], Color.Red);
					DrawText("Bear Breakout"+CurrentBar, "Bear Breakout", 0, High[0]+5* TickSize, Color.Red);
					bearBreakouts.Set(1);
				}
			else
				bearBreakouts.Set(0);	
/////////////////Breakouts	
				 
/////////////////Engulf					 

       // Check for Bearish Engulfing pattern
       		if((Close[1] > Open[1]) && (Open[0] > Close[0]) 
				&& (Open[0] >= Close[1]) && (Open[1] >= Close[0]) 
				&& ((Open[0] - Close[0]) > (Close[1] - Open[1]))) 
         		{
					DrawArrowDown("MyArrowDown"+CurrentBar, 0, High[0], Color.Red);
					DrawText("Bear Engulfs"+CurrentBar, "Bear Engulfs", 0, High[0]+5* TickSize, Color.Red);
					bearishEngulfing.Set(1);
				}
			else
					bearishEngulfing.Set(0);
		
		// Check for Bullish Engulfing pattern
       		if((Open[1] > Close[1]) && (Close[0] > Open[0]) 
				&& (Close[0] >= Open[1]) && (Close[1] >= Open[0]) 
				&& ((Close[0] - Open[0]) > (Open[1] - Close[1]))) 
         		{
					DrawArrowUp("MyArrowUp"+CurrentBar, 0, Low[0], Color.Lime);
					DrawText("BUll Engulfs"+CurrentBar, "BUll Engulfs", 0, Low[0]-5* TickSize, Color.Blue);
					bullishEngulfing.Set(1);
				}
			else
					bullishEngulfing.Set(0);
/////////////////Engulf			
		
		
		
       // Check for a Three Outside Down pattern
       	if((Close[2] > Open[2]) && (Open[1] > Close[1]) 
				&& (Open[1] >= Close[2]) && (Open[2] >= Close[1]) 
				&& ((Open[1] - Close[1]) > (Close[2] - Open[2])) 
				&& (Open[0] > Close[0]) && (Close[0] < Close[1])) 
         		{
					DrawArrowUp("MyArrowUp"+CurrentBar, 0, Low[0], Color.Lime);
					DrawText("BUll Three Outside Down"+CurrentBar, "BUll Three Outside Down", 0, Low[0]-5* TickSize, Color.Blue);
					threeOutsideDown.Set(1);
				}
			else
					threeOutsideDown.Set(0);	
				
////		  Check for Three Outside Up pattern
       	if((Open[2] > Close[2]) && (Close[1] > Open[1]) 
				&& (Close[1] >= Open[2]) && (Close[2] >= Open[1]) 
				&& ((Close[1] - Open[1]) > (Open[2] - Close[2])) 
				&& (Close[0] > Open[0]) && (Close[0] > Close[1])) 
         		{
					DrawArrowUp("MyArrowUp"+CurrentBar, 0, Low[0], Color.Lime);
					DrawText("BUll Three Outside Up"+CurrentBar, "BUll Three Outside Up", 0, Low[0]-5* TickSize, Color.Blue);
					threeOutsideUp.Set(1);
				}
			else
					threeOutsideUp.Set(0);		
			
		
//       // Check for a Dark Cloud Cover pattern
       	if((Close[1] > Open[1]) && (((Close[1] + Open[1]) / 2) > Close[0]) 
				&& (Open[0] > Close[0]) && (Open[0] > Close[1]) && (Close[0] > Open[1]) 
				&& ((Open[0] - Close[0]) / (0.001 + (High[0] - Low[0])) > 0.6)) 
         		{
					DrawArrowDown("MyArrowDown"+CurrentBar, 0, High[0], Color.Red);
					DrawText("Bear Dark Cloud Cover"+CurrentBar, "Bear Dark Cloud Cover", 0, High[0]+5* TickSize, Color.Red);
					darkCloudCover.Set(1);
				}   
			else
					darkCloudCover.Set(0);			
		
		
       // Check for Evening Doji Star pattern
       		if((Close[2] > Open[2]) && ((Close[2] - Open[2]) / (0.001 + High[2] - Low[2]) > 0.6) 
				&& (Close[2] < Open[1]) && (Close[1] > Open[1]) 
				&& ((High[1]-Low[1]) > (3*(Close[1] - Open[1]))) 
				&& (Open[0] > Close[0]) && (Open[0] < Open[1])) 
         		{
					DrawArrowDown("MyArrowDown"+CurrentBar, 0, High[0], Color.Red);
					DrawText("Bear Evening Doji"+CurrentBar, "Bear Evening Doji", 0, High[0]+5* TickSize, Color.Red);
					eveningDojiStar.Set(1);
				}     
			else
				eveningDojiStar.Set(0);
				
		// Check for Morning Doji Star
       		if((Open[2] > Close[2]) && ((Open[2] - Close[2]) / (0.001 + High[2] - Low[2]) > 0.6) 
				&& (Close[2] > Open[1]) && (Open[1] > Close[1]) 
				&& ((High[1] - Low[1]) > (3*(Close[1] - Open[1]))) && (Close[0] > Open[0]) 
				&& (Open[0] > Open[1])) 
         		{
					DrawArrowUp("MyArrowUp"+CurrentBar, 0, Low[0], Color.Lime);
					DrawText("BUll Morning Doji Star"+CurrentBar, "BUll Morning Doji Star", 0, Low[0]-5* TickSize, Color.Blue);
					morningDojiStar.Set(1);
				}
			else
				morningDojiStar.Set(0);
				
       // Check for Bearish Harami pattern
       		if((Close[1] > Open[1]) && (Open[0] > Close[0]) 
				&& (Open[0] <= Close[1]) && (Open[1] <= Close[0]) 
				&& ((Open[0] - Close[0]) < (Close[1] - Open[1]))) 
         		{
					DrawArrowDown("MyArrowDown"+CurrentBar, 0, High[0], Color.Red);
					DrawText("Bear Harami"+CurrentBar, "Bear Harami", 0, High[0]+5* TickSize, Color.Red);
					bearishHarami.Set(1);
				}
			else
					bearishHarami.Set(0);
				
		 // Check for Bullish Harami pattern
       		if((Open[1] > Close[1]) && (Close[0] > Open[0]) 
				&& (Close[0] <= Open[1]) && (Close[1] <= Open[0]) 
				&& ((Close[0] - Open[0]) < (Open[1] - Close[1]))) 
         		{
					DrawArrowUp("MyArrowUp"+CurrentBar, 0, Low[0], Color.Lime);
					DrawText("BUll Harami"+CurrentBar, "BUll Harami", 0, Low[0]-5* TickSize, Color.Blue);
					bullishHarami.Set(1);
				}
			else
					bullishHarami.Set(0);
		
		
		
       // Check for Three Inside Down pattern
       	if((Close[2] > Open[2]) && (Open[1] > Close[1]) 
				&& (Open[1] <= Close[2]) && (Open[2] <= Close[1]) 
				&& ((Open[1] - Close[1]) < (Close[2] - Open[2])) 
				&& (Open[0] > Close[0]) && (Close[0] < Close[1]) 
				&& (Open[0] < Open[1])) 
         		{
					DrawArrowDown("MyArrowDown"+CurrentBar, 0, High[0], Color.Red);
					DrawText("Bear Three Inside Down"+CurrentBar, "Bear Three Inside Down", 0, High[0]+5* TickSize, Color.Red);
					threeInsideDown.Set(1);
				}  
			else
				threeInsideDown.Set(0);		
				
//		// Check for Three Inside Up pattern
       	if((Open[2] > Close[2]) && (Close[1] > Open[1]) 
				&& (Close[1] <= Open[2]) && (Close[2] <= Open[1]) 
				&& ((Close[1] - Open[1]) < (Open[2] - Close[2])) 
				&& (Close[0] > Open[0]) && (Close[0] > Close[1]) 
				&& (Open[0] > Open[1])) 
         		{
					DrawArrowUp("MyArrowUp"+CurrentBar, 0, Low[0], Color.Lime);
					DrawText("BUll Three Inside Up"+CurrentBar, "BUll Three Inside Up", 0, Low[0]-5* TickSize, Color.Blue);
					threeInsideUp.Set(1);
				}  
			else
				threeInsideUp.Set(0);
		
		
//       // Check for Three Black Crows pattern
       	if((Open[0] > Close[0]*1.01) && (Open[1] > Close[1]*1.01) 
				&& (Open[2] > Close[2]*1.01) && (Close[0] < Close[1]) 
				&& (Close[1] < Close[2]) && (Open[0] > Close[1]) 
				&& (Open[0] < Open[1]) && (Open[1] > Close[2]) && (Open[1] < Open[2]) 
				&& (((Close[0] - Low[0]) / (High[0] - Low[0])) < 0.2) 
				&& (((Close[1] - Low[1]) / (High[1] - Low[1])) < 0.2) 
				&& (((Close[2] - Low[2]) / (High[2] - Low[2])) < 0.2))
				{
					DrawArrowDown("MyArrowDown"+CurrentBar, 0, High[0], Color.Red);
					DrawText("Bear Three Black Crows"+CurrentBar, "Bear Three Black Crows", 0, High[0]+5* TickSize, Color.Red);
					threeBlackCrows.Set(1);
				}
 			else
					threeBlackCrows.Set(0);
				
//	  // Check for Three White Soldiers pattern
       	if((Close[0] > Open[0]*1.01) && (Close[1] > Open[1]*1.01) 
				&& (Close[2] > Open[2]*1.01) && (Close[0] > Close[1]) 
				&& (Close[1] > Close[2]) 
				&& (Open[0] < Close[1]) && (Open[0] > Open[1]) 
				&& (Open[1] < Close[2]) && (Open[1] > Open[2]) 
				&& (((High[0] - Close[0]) / (High[0] - Low[0])) < 0.2) 
				&& (((High[1] - Close[1]) / (High[1] - Low[1])) < 0.2) 
				&& (((High[2] - Close[2]) / (High[2] - Low[2])) < 0.2)) 
         		{
					DrawArrowUp("MyArrowUp"+CurrentBar, 0, Low[0], Color.Lime);
					DrawText("BUll Three White Soldiers"+CurrentBar, "BUll Three White Soldiers", 0, Low[0]-5* TickSize, Color.Blue);
					threeWhiteSoldiers.Set(1);
				}     	
			else
				threeWhiteSoldiers.Set(0);
		
		
		//Check for Evening Star Pattern
       		if((Close[2] > Open[2]) && ((Close[2] - Open[2]) / (0.001 + High[2] - Low[2]) > 0.6) 
				&& (Close[2] < Open[1]) && (Close[1] > Open[1]) 
				&& ((High[1] - Low[1]) > (3*(Close[1] - Open[1]))) && (Open[0] > Close[0]) 
				&& (Open[0] < Open[1])) 
         		{
					DrawArrowDown("MyArrowDown"+CurrentBar, 0, High[0], Color.Red);
					DrawText("Bear Evening Star"+CurrentBar, "Bear Evening Star", 0, High[0]+5* TickSize, Color.Red);
					eveningStar.Set(1);
				}
      		else
				eveningStar.Set(0);
      
          
       // Check for Piercing Line pattern
       		if((Close[1] < Open[1]) && (((Open[1] + Close[1]) / 2) < Close[0]) 
				&& (Open[0] < Close[0]) && (Open[0] < Close[1]) && (Close[0] < Open[1]) 
				&& ((Close[0] - Open[0]) / (0.001 + (High[0] - Low[0])) > 0.6)) 
         		{
					DrawArrowUp("MyArrowUp"+CurrentBar, 0, Low[0], Color.Lime);
					DrawText("BUll Piercing Line"+CurrentBar, "BUll Piercing Line", 0, Low[0]-5* TickSize, Color.Blue);
					piercingLine.Set(1);
				}       
     		else
				piercingLine.Set(0);
			
		
		// Check for InsideBar pattern
				
			if(High[0] < High[1] && Low[0] > Low[1])	
				
			{
				BarColor = Color.Yellow;
				insidebar.Set(1);
			}
			else
				insidebar.Set(0);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot0
        {
            get { return Values[0]; }
        }

        [Description("Bolinger Band Period")]
        [Category("Parameters")]
        public int BBPeriods
        {
            get { return bbPeriods; }
            set { bbPeriods = Math.Max(0, value); }
        }
		
		 [Description("Bolinger Band Std Deviation")]
        [Category("Parameters")]
        public int STDev
        {
            get { return sTDev; }
            set { sTDev = Math.Max(0, value); }
        }
		
		[Browsable(false)]
        [XmlIgnore()]
        public DataSeries BearDoji
        {
            get 
           { 
                Update();
                return bearDoji;
			 
           }
        }
		
				
		[Browsable(false)]
        [XmlIgnore()]
        public DataSeries BullDoji
        {
            get 
           { 
                Update();
                return bullDoji;
			 
           }
        }
		[Browsable(false)]
        [XmlIgnore()]
        public DataSeries BullBreakouts
        {
            get 
           { 
                Update();
                return bullBreakouts;
			 
           }
        }
		[Browsable(false)]
        [XmlIgnore()]
        public DataSeries BearBreakouts
        {
            get 
           { 
                Update();
                return bearBreakouts;
			 
           }
        }
		[Browsable(false)]
        [XmlIgnore()]
        public DataSeries BearishEngulfing
        {
            get 
           { 
                Update();
                return bearishEngulfing;
			 
           }
        }
		[Browsable(false)]
        [XmlIgnore()]
        public DataSeries BullishEngulfing
        {
            get 
           { 
                Update();
                return bullishEngulfing;
			 
           }
        }
	
		[Browsable(false)]
        [XmlIgnore()]
        public DataSeries EveningDojiStar
        {
            get 
           { 
                Update();
                return eveningDojiStar;
			 
           }
        }
		
		[Browsable(false)]
        [XmlIgnore()]
        public DataSeries MorningDojiStar
        {
            get 
           { 
                Update();
                return morningDojiStar;
			 
           }
        }
		[Browsable(false)]
        [XmlIgnore()]
        public DataSeries BearishHarami
        {
            get 
           { 
                Update();
                return bearishHarami;
			 
           }
        }
		[Browsable(false)]
        [XmlIgnore()]
        public DataSeries BullishHarami
        {
            get 
           { 
                Update();
                return bullishHarami;
			 
           }
        }
		
		[Browsable(false)]
        [XmlIgnore()]
        public DataSeries EveningStar
        {
            get 
           { 
                Update();
                return eveningStar;
			 
           }
        }
		[Browsable(false)]
        [XmlIgnore()]
        public DataSeries PiercingLine
        {
            get 
           { 
                Update();
                return piercingLine;
			 
           }
        }
		
		[Browsable(false)]
        [XmlIgnore()]
        public DataSeries InsideBar
        {
            get 
           { 
                Update();
                return insidebar;
			 
           }
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
        private PatternRecognition[] cachePatternRecognition = null;

        private static PatternRecognition checkPatternRecognition = new PatternRecognition();

        /// <summary>
        /// Candle Stick Pattern Recognittion
        /// </summary>
        /// <returns></returns>
        public PatternRecognition PatternRecognition(int bBPeriods, int sTDev)
        {
            return PatternRecognition(Input, bBPeriods, sTDev);
        }

        /// <summary>
        /// Candle Stick Pattern Recognittion
        /// </summary>
        /// <returns></returns>
        public PatternRecognition PatternRecognition(Data.IDataSeries input, int bBPeriods, int sTDev)
        {
            if (cachePatternRecognition != null)
                for (int idx = 0; idx < cachePatternRecognition.Length; idx++)
                    if (cachePatternRecognition[idx].BBPeriods == bBPeriods && cachePatternRecognition[idx].STDev == sTDev && cachePatternRecognition[idx].EqualsInput(input))
                        return cachePatternRecognition[idx];

            lock (checkPatternRecognition)
            {
                checkPatternRecognition.BBPeriods = bBPeriods;
                bBPeriods = checkPatternRecognition.BBPeriods;
                checkPatternRecognition.STDev = sTDev;
                sTDev = checkPatternRecognition.STDev;

                if (cachePatternRecognition != null)
                    for (int idx = 0; idx < cachePatternRecognition.Length; idx++)
                        if (cachePatternRecognition[idx].BBPeriods == bBPeriods && cachePatternRecognition[idx].STDev == sTDev && cachePatternRecognition[idx].EqualsInput(input))
                            return cachePatternRecognition[idx];

                PatternRecognition indicator = new PatternRecognition();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BBPeriods = bBPeriods;
                indicator.STDev = sTDev;
                Indicators.Add(indicator);
                indicator.SetUp();

                PatternRecognition[] tmp = new PatternRecognition[cachePatternRecognition == null ? 1 : cachePatternRecognition.Length + 1];
                if (cachePatternRecognition != null)
                    cachePatternRecognition.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachePatternRecognition = tmp;
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
        /// Candle Stick Pattern Recognittion
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PatternRecognition PatternRecognition(int bBPeriods, int sTDev)
        {
            return _indicator.PatternRecognition(Input, bBPeriods, sTDev);
        }

        /// <summary>
        /// Candle Stick Pattern Recognittion
        /// </summary>
        /// <returns></returns>
        public Indicator.PatternRecognition PatternRecognition(Data.IDataSeries input, int bBPeriods, int sTDev)
        {
            return _indicator.PatternRecognition(input, bBPeriods, sTDev);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Candle Stick Pattern Recognittion
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PatternRecognition PatternRecognition(int bBPeriods, int sTDev)
        {
            return _indicator.PatternRecognition(Input, bBPeriods, sTDev);
        }

        /// <summary>
        /// Candle Stick Pattern Recognittion
        /// </summary>
        /// <returns></returns>
        public Indicator.PatternRecognition PatternRecognition(Data.IDataSeries input, int bBPeriods, int sTDev)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.PatternRecognition(input, bBPeriods, sTDev);
        }
    }
}
#endregion
