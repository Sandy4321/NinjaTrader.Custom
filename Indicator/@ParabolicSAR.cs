// 
// Copyright (C) 2009, NinjaTrader LLC <www.ninjatrader.com>.
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
    /// Parabolic SAR according to Stocks and Commodities magazine V 11:11 (477-479).
    /// </summary>
    [Description("Parabolic SAR")]
    public class ParabolicSAR : Indicator
    {
        #region Variables
		private double acceleration			= 0.02;
		private double accelerationStep		= 0.02;
		private double accelerationMax		= 0.2;

		private bool   longPosition;
		private double xp					= 0.0;		// Extreme Price
		private double af					= 0;		// Acceleration factor
		private double todaySAR				= 0;		// SAR value
		private double prevSAR				= 0;
		
		private int reverseBar				= 0;
		private double reverseValue			= 0;
		
		private int prevBar					= 0;
		private bool afIncreased			= false;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Orange, PlotStyle.Dot, "Parabolic SAR"));
            Overlay					= true;	// Plots the indicator on top of price
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar < 3) 
				return;
			
			else if (CurrentBar == 3)
			{
				// Determine initial position
				longPosition = High[0] > High[1] ? true : false;
				if (longPosition)
					xp = MAX(High, CurrentBar)[0];
				else
					xp = MIN(Low, CurrentBar)[0];
				af = Acceleration;
				Value.Set(xp + (longPosition ? -1 : 1) * ((MAX(High, CurrentBar)[0] - MIN(Low, CurrentBar)[0]) * af));
				return;
			}
			
			// Reset accelerator increase limiter on new bars
			if (afIncreased && prevBar != CurrentBar)
				afIncreased = false;
			
			// Current event is on a bar not marked as a reversal bar yet
			if (reverseBar != CurrentBar)
			{
				// SAR = SAR[1] + af * (xp - SAR[1])
				todaySAR = TodaySAR(Value[1] + af * (xp - Value[1]));
				for (int x = 1; x <= 2; x++)
				{
					if (longPosition)
					{
						if (todaySAR > Low[x])
							todaySAR = Low[x];
					}
					else
					{
						if (todaySAR < High[x])
							todaySAR = High[x];
					}
				}
				
				// Reverse position
				if ((longPosition && (Low[0] < todaySAR || Low[1] < todaySAR))
					|| (!longPosition && (High[0] > todaySAR || High[1] > todaySAR))) 
				{
					Value.Set(Reverse());
					return;
				}
				// Holding long position
				else if (longPosition)
				{
					// Process a new SAR value only on a new bar or if SAR value was penetrated.
					if (prevBar != CurrentBar || Low[0] < prevSAR)
					{
						Value.Set(todaySAR);
						prevSAR = todaySAR;
					}
					else
						Value.Set(prevSAR);
					
					if (High[0] > xp)
					{
						xp = High[0];
						AfIncrease();
					}
				}
				
				// Holding short position
				else if (!longPosition)
				{
					// Process a new SAR value only on a new bar or if SAR value was penetrated.
					if (prevBar != CurrentBar || High[0] > prevSAR)
					{
						Value.Set(todaySAR);
						prevSAR = todaySAR;
					}
					else
						Value.Set(prevSAR);
					
					if (Low[0] < xp)
					{
						xp = Low[0];
						AfIncrease();
					}
				}
			}
			
			// Current event is on the same bar as the reversal bar
			else
			{
				// Only set new xp values. No increasing af since this is the first bar.
				if (longPosition && High[0] > xp)
					xp = High[0];
				else if (!longPosition && Low[0] < xp)
					xp = Low[0];
				
				Value.Set(prevSAR);
				
				// SAR = SAR[1] + af * (xp - SAR[1])
				todaySAR = TodaySAR(longPosition ? Math.Min(reverseValue, Low[0]) : Math.Max(reverseValue, High[0]));
			}
			
			prevBar = CurrentBar;
		}

        #region Properties
        /// <summary>
        /// The initial acceleration factor
        /// </summary>
        [Description("The initial acceleration factor")]
        [GridCategory("Parameters")]
        public double Acceleration
        {
            get { return acceleration; }
            set { acceleration = Math.Max(0.00, value); }
        }

        /// <summary>
        /// The acceleration step factor
        /// </summary>
        [Description("The acceleration step factor")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Acceleration step")]
        public double AccelerationStep
        {
            get { return accelerationStep; }
            set { accelerationStep = Math.Max(0.02, value); }
        }

		/// <summary>
		/// The maximum acceleration
		/// </summary>
		[Description("The maximum acceleration")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Acceleration max")]
		public double AccelerationMax
		{
			get { return accelerationMax; }
			set { accelerationMax = Math.Max(0.02, value); }
		}
		#endregion

		#region Miscellaneous
		// Only raise accelerator if not raised for current bar yet
		private void AfIncrease()
		{
			if (!afIncreased)
			{
				af = Math.Min(AccelerationMax, af + AccelerationStep);
				afIncreased = true;
			}
			return;
		}
		
		// Additional rule. SAR for today can't be placed inside the bar of day - 1 or day - 2.
		private double TodaySAR(double todaySAR)
		{
			if (longPosition)
			{
				double lowestSAR = Math.Min(Math.Min(todaySAR, Low[0]), Low[1]);
				if (Low[0] > lowestSAR)
					todaySAR = lowestSAR;
				else
					todaySAR = Reverse();
			}
			else
			{
				double highestSAR = Math.Max(Math.Max(todaySAR, High[0]), High[1]);
				if (High[0] < highestSAR)
					todaySAR = highestSAR;
				else
					todaySAR = Reverse();
			}
			return todaySAR;
		}
		
		private double Reverse()
		{
			double todaySAR = xp;
			if ((longPosition && prevSAR > Low[0]) || (!longPosition && prevSAR < High[0]) || prevBar != CurrentBar)
			{
				longPosition = !longPosition;
				reverseBar = CurrentBar;
				reverseValue = xp;
				af = Acceleration;
				xp =longPosition ? High[0] : Low[0];
				prevSAR = todaySAR;
			}
			else
				todaySAR = prevSAR;
			return todaySAR;
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
        private ParabolicSAR[] cacheParabolicSAR = null;

        private static ParabolicSAR checkParabolicSAR = new ParabolicSAR();

        /// <summary>
        /// Parabolic SAR
        /// </summary>
        /// <returns></returns>
        public ParabolicSAR ParabolicSAR(double acceleration, double accelerationMax, double accelerationStep)
        {
            return ParabolicSAR(Input, acceleration, accelerationMax, accelerationStep);
        }

        /// <summary>
        /// Parabolic SAR
        /// </summary>
        /// <returns></returns>
        public ParabolicSAR ParabolicSAR(Data.IDataSeries input, double acceleration, double accelerationMax, double accelerationStep)
        {
            if (cacheParabolicSAR != null)
                for (int idx = 0; idx < cacheParabolicSAR.Length; idx++)
                    if (Math.Abs(cacheParabolicSAR[idx].Acceleration - acceleration) <= double.Epsilon && Math.Abs(cacheParabolicSAR[idx].AccelerationMax - accelerationMax) <= double.Epsilon && Math.Abs(cacheParabolicSAR[idx].AccelerationStep - accelerationStep) <= double.Epsilon && cacheParabolicSAR[idx].EqualsInput(input))
                        return cacheParabolicSAR[idx];

            lock (checkParabolicSAR)
            {
                checkParabolicSAR.Acceleration = acceleration;
                acceleration = checkParabolicSAR.Acceleration;
                checkParabolicSAR.AccelerationMax = accelerationMax;
                accelerationMax = checkParabolicSAR.AccelerationMax;
                checkParabolicSAR.AccelerationStep = accelerationStep;
                accelerationStep = checkParabolicSAR.AccelerationStep;

                if (cacheParabolicSAR != null)
                    for (int idx = 0; idx < cacheParabolicSAR.Length; idx++)
                        if (Math.Abs(cacheParabolicSAR[idx].Acceleration - acceleration) <= double.Epsilon && Math.Abs(cacheParabolicSAR[idx].AccelerationMax - accelerationMax) <= double.Epsilon && Math.Abs(cacheParabolicSAR[idx].AccelerationStep - accelerationStep) <= double.Epsilon && cacheParabolicSAR[idx].EqualsInput(input))
                            return cacheParabolicSAR[idx];

                ParabolicSAR indicator = new ParabolicSAR();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Acceleration = acceleration;
                indicator.AccelerationMax = accelerationMax;
                indicator.AccelerationStep = accelerationStep;
                Indicators.Add(indicator);
                indicator.SetUp();

                ParabolicSAR[] tmp = new ParabolicSAR[cacheParabolicSAR == null ? 1 : cacheParabolicSAR.Length + 1];
                if (cacheParabolicSAR != null)
                    cacheParabolicSAR.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheParabolicSAR = tmp;
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
        /// Parabolic SAR
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ParabolicSAR ParabolicSAR(double acceleration, double accelerationMax, double accelerationStep)
        {
            return _indicator.ParabolicSAR(Input, acceleration, accelerationMax, accelerationStep);
        }

        /// <summary>
        /// Parabolic SAR
        /// </summary>
        /// <returns></returns>
        public Indicator.ParabolicSAR ParabolicSAR(Data.IDataSeries input, double acceleration, double accelerationMax, double accelerationStep)
        {
            return _indicator.ParabolicSAR(input, acceleration, accelerationMax, accelerationStep);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Parabolic SAR
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ParabolicSAR ParabolicSAR(double acceleration, double accelerationMax, double accelerationStep)
        {
            return _indicator.ParabolicSAR(Input, acceleration, accelerationMax, accelerationStep);
        }

        /// <summary>
        /// Parabolic SAR
        /// </summary>
        /// <returns></returns>
        public Indicator.ParabolicSAR ParabolicSAR(Data.IDataSeries input, double acceleration, double accelerationMax, double accelerationStep)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ParabolicSAR(input, acceleration, accelerationMax, accelerationStep);
        }
    }
}
#endregion
