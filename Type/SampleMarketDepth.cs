// 
// Copyright (C) 2010, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//
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

// Add this to the declarations. It allows for the use of ArrayLists.
using System.Collections.Generic;

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Sample demonstrating how you can store your own data book.
    /// </summary>
    [Description("Sample demonstrating how you can create your own Level II data book.")]
    public class SampleMarketDepth : Indicator
    {
        #region Variables
		private	List<LadderRow>		askRows	= new List<LadderRow>();
		private	List<LadderRow>		bidRows	= new List<LadderRow>();
		
		private bool firstAskEvent	= true;
		private bool firstBidEvent	= true;
        #endregion

		/// <summary>
		/// This class holds a row of the bid or ask ladder.
		/// </summary>
		private class LadderRow
		{
			public	string	MarketMaker;			// relevant for stocks only
			public	double	Price;
			public	long	Volume;

			public LadderRow(double myPrice, long myVolume, string myMarketMaker)
			{
				MarketMaker	= myMarketMaker;
				Price		= myPrice;
				Volume		= myVolume;
			}
		}

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose	= true;
            Overlay				= false;
            PriceTypeSupported	= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// Since the L2 data is only stored on real-time bars, there is no need to print L2 books on historical data
            if (Historical)
				return;
			
			// When the Close price crosses over the SMA, print the L2 books.
			if (CrossAbove(Close, SMA(5), 1))
			{
				// Prints the L2 Ask Book we created. Cycles through the whole List and prints the contained objects.
				Print("Ask Book");
				for (int idx = 0; idx < askRows.Count; idx++)
					Print("Ask Price=" + askRows[idx].Price + " Volume=" + askRows[idx].Volume + " Position=" + idx);
				
				// Prints the L2 Bid Book we created. Cycles through the whole List and prints the contained objects.
				Print("Bid Book");
				for (int idx = 0; idx < bidRows.Count; idx++)
					Print("Bid Price=" + bidRows[idx].Price + " Volume=" + bidRows[idx].Volume + " Position=" + idx);
			}
        }

        /// <summary>
        /// Called on each incoming real time market depth event
        /// </summary>
        protected override void OnMarketDepth(MarketDepthEventArgs e)
        {
			List<LadderRow> rows = null;

			// Checks to see if the Market Data is of the Ask type
			if (e.MarketDataType == MarketDataType.Ask)
			{
				rows = askRows;
				
				// Due to race conditions, it is possible the first event is an Update operation instead of an Insert. When this happens, populate your Lists via e.MarketDepth first.
				if (firstAskEvent)
				{
					if (e.Operation == Operation.Update)
					{
						// Lock the MarketDepth collection to prevent modification to the collection while we are still processing it
						lock (e.MarketDepth.Ask)
						{
							for (int idx = 0; idx < e.MarketDepth.Ask.Count; idx++)
								rows.Add(new LadderRow(e.MarketDepth.Ask[idx].Price, e.MarketDepth.Ask[idx].Volume, e.MarketDepth.Ask[idx].MarketMaker));
						}
					}
					firstAskEvent = false;
				}
			}
			
			// Checks to see if the Market Data is of the Bid type
			else if (e.MarketDataType == MarketDataType.Bid)
			{
				rows = bidRows;
				
				// Due to race conditions, it is possible the first event is an Update operation instead of an Insert. When this happens, populate your Lists via e.MarketDepth first.
				if (firstBidEvent)
				{
					if (e.Operation == Operation.Update)
					{
						// Lock the MarketDepth collection to prevent modification to the collection while we are still processing it
						lock (e.MarketDepth.Bid)
						{
							for (int idx = 0; idx < e.MarketDepth.Bid.Count; idx++)
								rows.Add(new LadderRow(e.MarketDepth.Bid[idx].Price, e.MarketDepth.Bid[idx].Volume, e.MarketDepth.Bid[idx].MarketMaker));
						}
					}
					firstBidEvent = false;
				}
			}

			if (rows == null)
				return;

			// Checks to see if the action taken was an insertion into the ladder
			if (e.Operation == Operation.Insert)
			{
				// Add a new row at the end if the designated position is greater than our current ladder size
				if (e.Position >= rows.Count)
					rows.Add(new LadderRow(e.Price, e.Volume, e.MarketMaker));
				
				// Insert a new row into our ladder at the designated position
				else
					rows.Insert(e.Position, new LadderRow(e.Price, e.Volume, e.MarketMaker));
			}
			
			/* Checks to see if the action taken was a removal of itself from the ladder
			Note: Due to the multi threaded architecture of the NT core, race conditions could occur
			-> check if e.Position is within valid range */
			else if (e.Operation == Operation.Remove && e.Position < rows.Count)
				rows.RemoveAt(e.Position);
			
			/* Checks to see if the action taken was to update a data already on the ladder
			Note: Due to the multi threaded architecture of the NT core, race conditions could occur
			-> check if e.Position is within valid range */
			else if (e.Operation == Operation.Update)
			{
				rows[e.Position].MarketMaker	= e.MarketMaker;
				rows[e.Position].Price			= e.Price;
				rows[e.Position].Volume			= e.Volume;
			}
        }

        #region Properties

        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private SampleMarketDepth[] cacheSampleMarketDepth = null;

        private static SampleMarketDepth checkSampleMarketDepth = new SampleMarketDepth();

        /// <summary>
        /// Sample demonstrating how you can create your own Level II data book.
        /// </summary>
        /// <returns></returns>
        public SampleMarketDepth SampleMarketDepth()
        {
            return SampleMarketDepth(Input);
        }

        /// <summary>
        /// Sample demonstrating how you can create your own Level II data book.
        /// </summary>
        /// <returns></returns>
        public SampleMarketDepth SampleMarketDepth(Data.IDataSeries input)
        {
            if (cacheSampleMarketDepth != null)
                for (int idx = 0; idx < cacheSampleMarketDepth.Length; idx++)
                    if (cacheSampleMarketDepth[idx].EqualsInput(input))
                        return cacheSampleMarketDepth[idx];

            lock (checkSampleMarketDepth)
            {
                if (cacheSampleMarketDepth != null)
                    for (int idx = 0; idx < cacheSampleMarketDepth.Length; idx++)
                        if (cacheSampleMarketDepth[idx].EqualsInput(input))
                            return cacheSampleMarketDepth[idx];

                SampleMarketDepth indicator = new SampleMarketDepth();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                SampleMarketDepth[] tmp = new SampleMarketDepth[cacheSampleMarketDepth == null ? 1 : cacheSampleMarketDepth.Length + 1];
                if (cacheSampleMarketDepth != null)
                    cacheSampleMarketDepth.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSampleMarketDepth = tmp;
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
        /// Sample demonstrating how you can create your own Level II data book.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SampleMarketDepth SampleMarketDepth()
        {
            return _indicator.SampleMarketDepth(Input);
        }

        /// <summary>
        /// Sample demonstrating how you can create your own Level II data book.
        /// </summary>
        /// <returns></returns>
        public Indicator.SampleMarketDepth SampleMarketDepth(Data.IDataSeries input)
        {
            return _indicator.SampleMarketDepth(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Sample demonstrating how you can create your own Level II data book.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SampleMarketDepth SampleMarketDepth()
        {
            return _indicator.SampleMarketDepth(Input);
        }

        /// <summary>
        /// Sample demonstrating how you can create your own Level II data book.
        /// </summary>
        /// <returns></returns>
        public Indicator.SampleMarketDepth SampleMarketDepth(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SampleMarketDepth(input);
        }
    }
}
#endregion
