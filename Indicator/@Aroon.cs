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
	/// The Aroon Indicator was developed by Tushar Chande. Its comprised of two plots one measuring the number of periods since the most recent x-period high (Aroon Up) and the other measuring the number of periods since the most recent x-period low (Aroon Down).
	/// </summary>
	[Description("The Aroon Indicator was developed by Tushar Chande. Its comprised of two plots one measuring the number of periods since the most recent x-period high (Aroon Up) and the other measuring the number of periods since the most recent x-period low (Aroon Down).")]
	public class Aroon : Indicator
	{
		#region Variables
        private int     period          = 14;
        private double  runningMax;
        private int     runningMaxBar;
        private double  runningMin;
        private int     runningMinBar;
        #endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Green, "Up"));
			Add(new Plot(Color.DarkViolet, "Down"));
			Add(new Line(Color.DarkGray, 30, "Lower"));
			Add(new Line(Color.DarkGray, 70, "Upper"));
		}

		/// <summary>
		/// Calculates the indicator value(s) at the current index.
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				Down.Set(0);
				Up.Set(0);
                runningMax    = High[0];
                runningMin    = Low[0];
                runningMaxBar = 0;
                runningMinBar = 0;
                return;
			}

		    int		back	= Math.Min(Period, CurrentBar);

            if (CurrentBar - runningMaxBar >= Period)
            {
                runningMax = double.MinValue;
                for (int barsBack = back; barsBack > 0; barsBack--)
                    if (High[barsBack] >= runningMax)
                    {
                        runningMax = High[barsBack];
                        runningMaxBar = CurrentBar - barsBack;
                    }
            }

            if (CurrentBar - runningMinBar >= Period)
            {
                runningMin = double.MaxValue;
                for (int barsBack = back; barsBack > 0; barsBack--)
                    if (Low[barsBack] <= runningMin)
                    {
                        runningMin = Low[barsBack];
                        runningMinBar = CurrentBar - barsBack;
                    }
            }

            if (High[0] >= runningMax)
            {
                runningMax    = High[0];
                runningMaxBar = CurrentBar;
            }

            if (Low[0] <= runningMin)
            {
                runningMin    = Low[0];
                runningMinBar = CurrentBar;
            }

            Up.Set(100 * ((double)(back - (CurrentBar - runningMaxBar)) / back));
            Down.Set(100 * ((double)(back - (CurrentBar - runningMinBar)) / back));
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Down
		{
			get { return Values[1]; }
		}

		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries Up
		{
			get { return Values[0]; }
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
        private Aroon[] cacheAroon = null;

        private static Aroon checkAroon = new Aroon();

        /// <summary>
        /// The Aroon Indicator was developed by Tushar Chande. Its comprised of two plots one measuring the number of periods since the most recent x-period high (Aroon Up) and the other measuring the number of periods since the most recent x-period low (Aroon Down).
        /// </summary>
        /// <returns></returns>
        public Aroon Aroon(int period)
        {
            return Aroon(Input, period);
        }

        /// <summary>
        /// The Aroon Indicator was developed by Tushar Chande. Its comprised of two plots one measuring the number of periods since the most recent x-period high (Aroon Up) and the other measuring the number of periods since the most recent x-period low (Aroon Down).
        /// </summary>
        /// <returns></returns>
        public Aroon Aroon(Data.IDataSeries input, int period)
        {
            if (cacheAroon != null)
                for (int idx = 0; idx < cacheAroon.Length; idx++)
                    if (cacheAroon[idx].Period == period && cacheAroon[idx].EqualsInput(input))
                        return cacheAroon[idx];

            lock (checkAroon)
            {
                checkAroon.Period = period;
                period = checkAroon.Period;

                if (cacheAroon != null)
                    for (int idx = 0; idx < cacheAroon.Length; idx++)
                        if (cacheAroon[idx].Period == period && cacheAroon[idx].EqualsInput(input))
                            return cacheAroon[idx];

                Aroon indicator = new Aroon();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                Aroon[] tmp = new Aroon[cacheAroon == null ? 1 : cacheAroon.Length + 1];
                if (cacheAroon != null)
                    cacheAroon.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheAroon = tmp;
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
        /// The Aroon Indicator was developed by Tushar Chande. Its comprised of two plots one measuring the number of periods since the most recent x-period high (Aroon Up) and the other measuring the number of periods since the most recent x-period low (Aroon Down).
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Aroon Aroon(int period)
        {
            return _indicator.Aroon(Input, period);
        }

        /// <summary>
        /// The Aroon Indicator was developed by Tushar Chande. Its comprised of two plots one measuring the number of periods since the most recent x-period high (Aroon Up) and the other measuring the number of periods since the most recent x-period low (Aroon Down).
        /// </summary>
        /// <returns></returns>
        public Indicator.Aroon Aroon(Data.IDataSeries input, int period)
        {
            return _indicator.Aroon(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Aroon Indicator was developed by Tushar Chande. Its comprised of two plots one measuring the number of periods since the most recent x-period high (Aroon Up) and the other measuring the number of periods since the most recent x-period low (Aroon Down).
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.Aroon Aroon(int period)
        {
            return _indicator.Aroon(Input, period);
        }

        /// <summary>
        /// The Aroon Indicator was developed by Tushar Chande. Its comprised of two plots one measuring the number of periods since the most recent x-period high (Aroon Up) and the other measuring the number of periods since the most recent x-period low (Aroon Down).
        /// </summary>
        /// <returns></returns>
        public Indicator.Aroon Aroon(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Aroon(input, period);
        }
    }
}
#endregion
