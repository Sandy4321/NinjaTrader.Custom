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
    /// This Indicator represents the Darvas Box.
    /// </summary>
	[Description("The Darvas Boxes were taken from the pages of Nicolas Darvas book, How I Made $2,000,000 in the Stock Market. The boxes are used to normalize a trend. A 'buy' signal would be indicated when the price of the stock exceeds the top of the box. A 'sell' signal would be indicated when the price of the stock falls below the bottom of the box.")]
    public class Darvas : Indicator
    {
        #region Variables
		private double				boxTop				= double.MinValue;
		private double				boxBottom			= double.MaxValue;
		private bool				buySignal			= false;
		private double				currentBarHigh		= double.MinValue;
        private double				currentBarLow		= double.MaxValue;
		private	bool				isRealtime			= false;
		private int     			savedCurrentBar		= -1;
		private bool				sellSignal			= false;
		private int					startBarActBox		= 0;
        private int					state				= 0;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Lime, PlotStyle.Square, "Upper"));
			Add(new Plot(Color.Red, PlotStyle.Square, "Lower"));
            
            Overlay = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			BuySignal	= false;
			SellSignal	= false;

			if (savedCurrentBar == -1) // Init
			{
				currentBarHigh = High[0];
				currentBarLow = Low[0];
				state = GetNextState();
				savedCurrentBar = CurrentBar;
			}
			else if (savedCurrentBar != CurrentBar)
			{	// Is new bar ?
				currentBarHigh	= (isRealtime && !CalculateOnBarClose) ? High[1] : High[0];
				currentBarLow	= (isRealtime && !CalculateOnBarClose) ? Low[1] : Low[0];

				// today buy/sell signal is triggered
				if ((state == 5 && currentBarHigh > boxTop) || (state == 5 && currentBarLow < boxBottom))
				{
					if (state == 5 && currentBarHigh > boxTop)
						BuySignal	= true;
					else
						SellSignal	= true;

					state = 0;
					startBarActBox = CurrentBar;
				}

				state = GetNextState();
				// draw with today
				if (boxBottom == double.MaxValue)
					for (int i = CurrentBar - startBarActBox; i >= 0; i--)
					{						
						Upper.Set(i, boxTop);
						Lower.Reset(i);
					}				
				else
					for (int i = CurrentBar - startBarActBox; i >= 0; i--)
					{
						Upper.Set(i, boxTop);
						Lower.Set(i, boxBottom);
					}

				savedCurrentBar = CurrentBar;
			}
			else
			{
				isRealtime = true;

				// today buy/sell signal is triggered
				if ((state == 5 && currentBarHigh > boxTop) || (state == 5 && currentBarLow < boxBottom))
				{
					if (state == 5 && currentBarHigh > boxTop)
						BuySignal	= true;
					else
						SellSignal	= true;

					startBarActBox = CurrentBar + 1;
					state = 0;
				}

				// draw with today
				if (boxBottom == double.MaxValue)
				{
					Upper.Set(boxTop);
					Lower.Reset();
				}
				else
				{
					Upper.Set(boxTop);
					Lower.Set(boxBottom);
				} 
			}
		}

        #region Properties
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public bool BuySignal
        {
            get 
			{ 
				Update();
				return buySignal; 
			}
			set { buySignal = value; }
        }

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Lower
        {
            get { return Values[1]; }
        }

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public bool SellSignal
        {
            get 
			{ 
				Update();
				return sellSignal; 
			}
			set { sellSignal = value; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Upper
        {
            get { return Values[0]; }
        }
        #endregion

		#region Miscellaneous
		private int GetNextState() 
        {
			switch (state)
			{
				case 0:
					boxTop = currentBarHigh;
					boxBottom = double.MaxValue;
					return 1;
				
				case 1:
					if (boxTop > currentBarHigh)
						return 2;
					else
					{
						boxTop = currentBarHigh;
						return 1;
					}

				case 2:
					if (boxTop > currentBarHigh)
					{
						boxBottom = currentBarLow;
						return 3;
					}
					else
					{
						boxTop = currentBarHigh;
						return 1;
					}
				
				case 3:
					if (boxTop > currentBarHigh)
					{
						if (boxBottom < currentBarLow)
							return 4;
						else
						{
							boxBottom = currentBarLow;
							return 3;
						}
					}
					else
					{
						boxTop = currentBarHigh;
						boxBottom = double.MaxValue;
						return 1;
					}

				case 4:
					if (boxTop > currentBarHigh)
					{
						if (boxBottom < currentBarLow)
							return 5;
						else
						{
							boxBottom = currentBarLow;
							return 3;
						}
					}
					else
					{
						boxTop = currentBarHigh;
						boxBottom = double.MaxValue;
						return 1;
					}

				case 5:
					return 5;

				default:			// should not happen
					return state;
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
        private Darvas[] cacheDarvas = null;

        private static Darvas checkDarvas = new Darvas();

        /// <summary>
        /// The Darvas Boxes were taken from the pages of Nicolas Darvas book, How I Made $2,000,000 in the Stock Market. The boxes are used to normalize a trend. A 'buy' signal would be indicated when the price of the stock exceeds the top of the box. A 'sell' signal would be indicated when the price of the stock falls below the bottom of the box.
        /// </summary>
        /// <returns></returns>
        public Darvas Darvas()
        {
            return Darvas(Input);
        }

        /// <summary>
        /// The Darvas Boxes were taken from the pages of Nicolas Darvas book, How I Made $2,000,000 in the Stock Market. The boxes are used to normalize a trend. A 'buy' signal would be indicated when the price of the stock exceeds the top of the box. A 'sell' signal would be indicated when the price of the stock falls below the bottom of the box.
        /// </summary>
        /// <returns></returns>
        public Darvas Darvas(Data.IDataSeries input)
        {
            if (cacheDarvas != null)
                for (int idx = 0; idx < cacheDarvas.Length; idx++)
                    if (cacheDarvas[idx].EqualsInput(input))
                        return cacheDarvas[idx];

            lock (checkDarvas)
            {
                if (cacheDarvas != null)
                    for (int idx = 0; idx < cacheDarvas.Length; idx++)
                        if (cacheDarvas[idx].EqualsInput(input))
                            return cacheDarvas[idx];

                Darvas indicator = new Darvas();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                Darvas[] tmp = new Darvas[cacheDarvas == null ? 1 : cacheDarvas.Length + 1];
                if (cacheDarvas != null)
                    cacheDarvas.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheDarvas = tmp;
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
        /// The Darvas Boxes were taken from the pages of Nicolas Darvas book, How I Made $2,000,000 in the Stock Market. The boxes are used to normalize a trend. A 'buy' signal would be indicated when the price of the stock exceeds the top of the box. A 'sell' signal would be indicated when the price of the stock falls below the bottom of the box.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Darvas Darvas()
        {
            return _indicator.Darvas(Input);
        }

        /// <summary>
        /// The Darvas Boxes were taken from the pages of Nicolas Darvas book, How I Made $2,000,000 in the Stock Market. The boxes are used to normalize a trend. A 'buy' signal would be indicated when the price of the stock exceeds the top of the box. A 'sell' signal would be indicated when the price of the stock falls below the bottom of the box.
        /// </summary>
        /// <returns></returns>
        public Indicator.Darvas Darvas(Data.IDataSeries input)
        {
            return _indicator.Darvas(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Darvas Boxes were taken from the pages of Nicolas Darvas book, How I Made $2,000,000 in the Stock Market. The boxes are used to normalize a trend. A 'buy' signal would be indicated when the price of the stock exceeds the top of the box. A 'sell' signal would be indicated when the price of the stock falls below the bottom of the box.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Darvas Darvas()
        {
            return _indicator.Darvas(Input);
        }

        /// <summary>
        /// The Darvas Boxes were taken from the pages of Nicolas Darvas book, How I Made $2,000,000 in the Stock Market. The boxes are used to normalize a trend. A 'buy' signal would be indicated when the price of the stock exceeds the top of the box. A 'sell' signal would be indicated when the price of the stock falls below the bottom of the box.
        /// </summary>
        /// <returns></returns>
        public Indicator.Darvas Darvas(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Darvas(input);
        }
    }
}
#endregion
