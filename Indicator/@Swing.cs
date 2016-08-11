// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.Collections;
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
    /// The Swing indicator plots lines that represents the swing high and low points.
    /// </summary>
    [Description("The Swing indicator plots lines that represents the swing high and low points.")]
    [Gui.Design.DisplayName("Swing (High/Low)")]
    public class Swing : Indicator
    {
        #region Variables
		private double		currentSwingHigh	= 0; 
		private double		currentSwingLow		= 0; 
		private ArrayList	lastHighCache;
		private double		lastSwingHighValue	= 0;
		private ArrayList	lastLowCache;
		private double		lastSwingLowValue	= 0;
		private int			saveCurrentBar		= -1;
		private int			strength			= 5;
		private DataSeries	swingHighSeries;
		private DataSeries	swingHighSwings;
		private DataSeries	swingLowSeries;
		private DataSeries	swingLowSwings;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Green, PlotStyle.Dot, "Swing high"));
            Add(new Plot(Color.Orange, PlotStyle.Dot, "Swing low"));
			Plots[0].Pen.Width		= 2;
			Plots[0].Pen.DashStyle	= DashStyle.Dot;
			Plots[1].Pen.Width		= 2;
			Plots[1].Pen.DashStyle	= DashStyle.Dot;
            
			lastHighCache = new ArrayList();
			lastLowCache = new ArrayList();

			swingHighSeries	= new DataSeries(this);
			swingHighSwings	= new DataSeries(this);
			swingLowSeries	= new DataSeries(this);
			swingLowSwings	= new DataSeries(this);

			DisplayInDataBox	= false;
			PaintPriceMarkers	= false;
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (saveCurrentBar != CurrentBar)
			{
				swingHighSwings.Set(0);	// initializing important internal
				swingLowSwings.Set(0);	// initializing important internal

				swingHighSeries.Set(0);	// initializing important internal
				swingLowSeries.Set(0);	// initializing important internal

				lastHighCache.Add(High[0]);
				if (lastHighCache.Count > (2 * strength) + 1)
					lastHighCache.RemoveAt(0);
				lastLowCache.Add(Low[0]);
				if (lastLowCache.Count > (2 * strength) + 1)
					lastLowCache.RemoveAt(0);

				if (lastHighCache.Count == (2 * strength) + 1)
				{
					bool isSwingHigh = true;
					double swingHighCandidateValue = (double) lastHighCache[strength];
					for (int i=0; i < strength; i++)
						if ((double) lastHighCache[i] >= swingHighCandidateValue - double.Epsilon)
							isSwingHigh = false;

					for (int i=strength+1; i < lastHighCache.Count; i++)
						if ((double) lastHighCache[i] > swingHighCandidateValue - double.Epsilon)
							isSwingHigh = false;

					swingHighSwings.Set(strength, isSwingHigh ? swingHighCandidateValue : 0.0);
					if (isSwingHigh)
						lastSwingHighValue = swingHighCandidateValue;
		
					if (isSwingHigh)
					{
						currentSwingHigh = swingHighCandidateValue;
						for (int i=0; i <= strength; i++)
							SwingHighPlot.Set(i, currentSwingHigh);
					}
					else if (High[0] > currentSwingHigh)
					{
						currentSwingHigh = 0.0;
						SwingHighPlot.Reset();
					}
					else 
						SwingHighPlot.Set(currentSwingHigh);

					if (isSwingHigh)
					{
						for (int i=0; i<=strength; i++)
							swingHighSeries.Set(i, lastSwingHighValue);
					}
					else 
					{ 
						swingHighSeries.Set(lastSwingHighValue);
					}
				}

				if (lastLowCache.Count == (2 * strength) + 1)
				{
					bool isSwingLow = true;
					double swingLowCandidateValue = (double) lastLowCache[strength];
					for (int i=0; i < strength; i++)
						if ((double) lastLowCache[i] <= swingLowCandidateValue + double.Epsilon)
							isSwingLow = false;

					for (int i=strength+1; i < lastLowCache.Count; i++)
						if ((double) lastLowCache[i] < swingLowCandidateValue + double.Epsilon)
							isSwingLow = false;

					swingLowSwings.Set(strength, isSwingLow ? swingLowCandidateValue : 0.0);
					if (isSwingLow)
						lastSwingLowValue = swingLowCandidateValue;

					if (isSwingLow)
					{
						currentSwingLow = swingLowCandidateValue;
						for (int i=0; i <= strength; i++)
							SwingLowPlot.Set(i, currentSwingLow);
					}
					else if (Low[0] < currentSwingLow)
					{
						currentSwingLow = double.MaxValue;
						SwingLowPlot.Reset();
					}
					else
						SwingLowPlot.Set(currentSwingLow);

					if (isSwingLow)
					{
						for (int i=0; i<=strength; i++)
							swingLowSeries.Set(i, lastSwingLowValue);
					}
					else
					{ 
						swingLowSeries.Set(lastSwingLowValue);
					}
				}

				saveCurrentBar = CurrentBar;
			}
			else
			{
				if (High[0] > High[strength] && swingHighSwings[strength] > 0.0)
				{
					swingHighSwings.Set(strength, 0.0);
					for (int i=0; i<=strength; i++)
						SwingHighPlot.Reset(i);
					currentSwingHigh = 0.0;
				}
				else if (High[0] > High[strength] && currentSwingHigh != 0.0)
				{
					SwingHighPlot.Reset();
					currentSwingHigh = 0.0;
				}
				else if (High[0] <= currentSwingHigh)
					SwingHighPlot.Set(currentSwingHigh);

				if (Low[0] < Low[strength] && swingLowSwings[strength] > 0.0)
				{
					swingLowSwings.Set(strength, 0.0);
					for (int i=0; i<=strength; i++)
						SwingLowPlot.Reset(i);
					currentSwingLow = double.MaxValue;
				}
				else if (Low[0] < Low[strength] && currentSwingLow != double.MaxValue)
				{
					SwingLowPlot.Reset();
					currentSwingLow = double.MaxValue;
				} 
				else if (Low[0] >= currentSwingLow)
					SwingLowPlot.Set(currentSwingLow);
			}
        }

        #region Functions
		/// <summary>
		/// Returns the number of bars ago a swing low occurred. Returns a value of -1 if a swing low is not found within the look back period.
		/// </summary>
		/// <param name="barsAgo"></param>
		/// <param name="instance"></param>
		/// <param name="lookBackPeriod"></param>
		/// <returns></returns>
		public int SwingLowBar(int barsAgo, int instance, int lookBackPeriod) 
		{
			if (instance < 1)
				throw new Exception(GetType().Name + ".SwingLowBar: instance must be greater/equal 1 but was " + instance);
			else if (barsAgo < 0)
				throw new Exception(GetType().Name + ".SwingLowBar: barsAgo must be greater/equal 0 but was " + barsAgo);
			else if (barsAgo >= Count)
				throw new Exception(GetType().Name + ".SwingLowBar: barsAgo out of valid range 0 through " + (Count - 1) + ", was " + barsAgo + ".");

			Update();

			for (int idx=CurrentBar - barsAgo - strength; idx >= CurrentBar - barsAgo - strength - lookBackPeriod; idx--)
			{
				if (idx < 0)
					return -1;
				if (idx >= swingLowSwings.Count)
					continue;				

				if (swingLowSwings.Get(idx).Equals(0.0))			
					continue;

				if (instance == 1) // 1-based, < to be save
					return CurrentBar - idx;	

				instance--;
			}
	
			return -1;
		}
		
		/// <summary>
		/// Returns the number of bars ago a swing high occurred. Returns a value of -1 if a swing high is not found within the look back period.
		/// </summary>
		/// <param name="barsAgo"></param>
		/// <param name="instance"></param>
		/// <param name="lookBackPeriod"></param>
		/// <returns></returns>
		public int SwingHighBar(int barsAgo, int instance, int lookBackPeriod) 
		{
			if (instance < 1)
				throw new Exception(GetType().Name + ".SwingHighBar: instance must be greater/equal 1 but was " + instance);
			else if (barsAgo < 0)
				throw new Exception(GetType().Name + ".SwingHighBar: barsAgo must be greater/equal 0 but was " + barsAgo);
			else if (barsAgo >= Count)
				throw new Exception(GetType().Name + ".SwingHighBar: barsAgo out of valid range 0 through " + (Count - 1) + ", was " + barsAgo + ".");

			Update();

			for (int idx=CurrentBar - barsAgo - strength; idx >= CurrentBar - barsAgo - strength - lookBackPeriod; idx--)
			{
				if (idx < 0)
					return -1;
				if (idx >= swingHighSwings.Count)
					continue;				

				if (swingHighSwings.Get(idx).Equals(0.0))			
					continue;

				if (instance <= 1) // 1-based, < to be save
					return CurrentBar - idx;	

				instance--;
			}

	
			return -1;
		}

        #endregion

        #region Properties
        [Description("Number of bars required on each side of the swing point.")]
        [GridCategory("Parameters")]
        public int Strength
        {
            get { return strength; }
            set { strength = Math.Max(1, value); }
        }
		
		/// <summary>
		/// Gets the high swings.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries SwingHigh
		{
			get 
			{ 
				Update();
				return swingHighSeries; 
			}
		}

		private DataSeries SwingHighPlot
		{
			get 
			{
				Update();
				return Values[0]; 
			}
		}

		/// <summary>
		/// Gets the low swings.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries SwingLow
		{
			get 
			{ 
				Update();
				return swingLowSeries; 
			}
		}

		private DataSeries SwingLowPlot
		{
			get 
			{
				Update();
				return Values[1]; 
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
        private Swing[] cacheSwing = null;

        private static Swing checkSwing = new Swing();

        /// <summary>
        /// The Swing indicator plots lines that represents the swing high and low points.
        /// </summary>
        /// <returns></returns>
        public Swing Swing(int strength)
        {
            return Swing(Input, strength);
        }

        /// <summary>
        /// The Swing indicator plots lines that represents the swing high and low points.
        /// </summary>
        /// <returns></returns>
        public Swing Swing(Data.IDataSeries input, int strength)
        {
            if (cacheSwing != null)
                for (int idx = 0; idx < cacheSwing.Length; idx++)
                    if (cacheSwing[idx].Strength == strength && cacheSwing[idx].EqualsInput(input))
                        return cacheSwing[idx];

            lock (checkSwing)
            {
                checkSwing.Strength = strength;
                strength = checkSwing.Strength;

                if (cacheSwing != null)
                    for (int idx = 0; idx < cacheSwing.Length; idx++)
                        if (cacheSwing[idx].Strength == strength && cacheSwing[idx].EqualsInput(input))
                            return cacheSwing[idx];

                Swing indicator = new Swing();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Strength = strength;
                Indicators.Add(indicator);
                indicator.SetUp();

                Swing[] tmp = new Swing[cacheSwing == null ? 1 : cacheSwing.Length + 1];
                if (cacheSwing != null)
                    cacheSwing.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSwing = tmp;
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
        /// The Swing indicator plots lines that represents the swing high and low points.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Swing Swing(int strength)
        {
            return _indicator.Swing(Input, strength);
        }

        /// <summary>
        /// The Swing indicator plots lines that represents the swing high and low points.
        /// </summary>
        /// <returns></returns>
        public Indicator.Swing Swing(Data.IDataSeries input, int strength)
        {
            return _indicator.Swing(input, strength);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Swing indicator plots lines that represents the swing high and low points.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Swing Swing(int strength)
        {
            return _indicator.Swing(Input, strength);
        }

        /// <summary>
        /// The Swing indicator plots lines that represents the swing high and low points.
        /// </summary>
        /// <returns></returns>
        public Indicator.Swing Swing(Data.IDataSeries input, int strength)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Swing(input, strength);
        }
    }
}
#endregion
