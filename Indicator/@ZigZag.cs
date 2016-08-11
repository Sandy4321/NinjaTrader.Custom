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
    /// The ZigZag indicator shows trend lines filtering out changes below a defined level. 
    /// </summary>
    [Description("The ZigZag indicator shows trend lines filtering out changes below a defined level. ")]
    public class ZigZag : Indicator
    {
        #region Variables
		private double			currentZigZagHigh	= 0;
		private double			currentZigZagLow	= 0;
		private DeviationType	deviationType		= DeviationType.Points;
		private double			deviationValue		= 0.5;
		private DataSeries		zigZagHighZigZags; 
		private DataSeries		zigZagLowZigZags; 
		private DataSeries		zigZagHighSeries; 
		private DataSeries		zigZagLowSeries; 
		private int				lastSwingIdx		= -1;
		private double			lastSwingPrice		= 0.0;
		private int				trendDir			= 0; // 1 = trend up, -1 = trend down, init = 0
		private bool			useHighLow			= false;

        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Blue, PlotStyle.Line, "ZigZag"));

			zigZagHighSeries	= new DataSeries(this, MaximumBarsLookBack.Infinite); 
			zigZagHighZigZags	= new DataSeries(this, MaximumBarsLookBack.Infinite); 
			zigZagLowSeries		= new DataSeries(this, MaximumBarsLookBack.Infinite); 
			zigZagLowZigZags	= new DataSeries(this, MaximumBarsLookBack.Infinite); 

			DisplayInDataBox	= false;
            Overlay				= true;
			PaintPriceMarkers	= false;
        }

		/// <summary>
		/// Returns the number of bars ago a zig zag low occurred. Returns a value of -1 if a zig zag low is not found within the look back period.
		/// </summary>
		/// <param name="barsAgo"></param>
		/// <param name="instance"></param>
		/// <param name="lookBackPeriod"></param>
		/// <returns></returns>
		public int LowBar(int barsAgo, int instance, int lookBackPeriod) 
		{
			if (instance < 1)
				throw new Exception(GetType().Name + ".LowBar: instance must be greater/equal 1 but was " + instance);
			else if (barsAgo < 0)
				throw new Exception(GetType().Name + ".LowBar: barsAgo must be greater/equal 0 but was " + barsAgo);
			else if (barsAgo >= Count)
				throw new Exception(GetType().Name + ".LowBar: barsAgo out of valid range 0 through " + (Count - 1) + ", was " + barsAgo + ".");

			Update();
			for (int idx = CurrentBar - barsAgo - 1; idx >= CurrentBar - barsAgo - 1 - lookBackPeriod; idx--)
			{
				if (idx < 0)
					return -1;
				if (idx >= zigZagLowZigZags.Count)
					continue;				

				if (zigZagLowZigZags.Get(idx).Equals(0.0))			
					continue;

				if (instance == 1) // 1-based, < to be save
					return CurrentBar - idx;	

				instance--;
			}
	
			return -1;
		}


		/// <summary>
		/// Returns the number of bars ago a zig zag high occurred. Returns a value of -1 if a zig zag high is not found within the look back period.
		/// </summary>
		/// <param name="barsAgo"></param>
		/// <param name="instance"></param>
		/// <param name="lookBackPeriod"></param>
		/// <returns></returns>
		public int HighBar(int barsAgo, int instance, int lookBackPeriod) 
		{
			if (instance < 1)
				throw new Exception(GetType().Name + ".HighBar: instance must be greater/equal 1 but was " + instance);
			else if (barsAgo < 0)
				throw new Exception(GetType().Name + ".HighBar: barsAgo must be greater/equal 0 but was " + barsAgo);
			else if (barsAgo >= Count)
				throw new Exception(GetType().Name + ".HighBar: barsAgo out of valid range 0 through " + (Count - 1) + ", was " + barsAgo + ".");

			Update();
			for (int idx = CurrentBar - barsAgo - 1; idx >= CurrentBar - barsAgo - 1 - lookBackPeriod; idx--)
			{
				if (idx < 0)
					return -1;
				if (idx >= zigZagHighZigZags.Count)
					continue;				

				if (zigZagHighZigZags.Get(idx).Equals(0.0))			
					continue;

				if (instance <= 1) // 1-based, < to be save
					return CurrentBar - idx;	

				instance--;
			}

			return -1;
		}

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar < 2) // need 3 bars to calculate Low/High
			{
				zigZagHighSeries.Set(0);
				zigZagHighZigZags.Set(0);
				zigZagLowSeries.Set(0);
				zigZagLowZigZags.Set(0);
				return;
			}

			// Initialization
			if (lastSwingPrice == 0.0)
				lastSwingPrice = Input[0];

			IDataSeries highSeries	= High;
			IDataSeries lowSeries	= Low;

			if (!useHighLow)
			{
				highSeries	= Input;
				lowSeries	= Input;
			}

			// Calculation always for 1-bar ago !

			double tickSize = Bars.Instrument.MasterInstrument.TickSize;
			bool isSwingHigh	= highSeries[1] >= highSeries[0] - double.Epsilon 
								&& highSeries[1] >= highSeries[2] - double.Epsilon;
			bool isSwingLow		= lowSeries[1] <= lowSeries[0] + double.Epsilon 
								&& lowSeries[1] <= lowSeries[2] + double.Epsilon;  
			bool isOverHighDeviation	= (deviationType == DeviationType.Percent && IsPriceGreater(highSeries[1], (lastSwingPrice * (1.0 + deviationValue * 0.01))))
										|| (deviationType == DeviationType.Points && IsPriceGreater(highSeries[1], lastSwingPrice + deviationValue));
			bool isOverLowDeviation		= (deviationType == DeviationType.Percent && IsPriceGreater(lastSwingPrice * (1.0 - deviationValue * 0.01), lowSeries[1]))
										|| (deviationType == DeviationType.Points && IsPriceGreater(lastSwingPrice - deviationValue, lowSeries[1]));

			double	saveValue	= 0.0;
			bool	addHigh		= false; 
			bool	addLow		= false; 
			bool	updateHigh	= false; 
			bool	updateLow	= false; 

			zigZagHighZigZags.Set(0);
			zigZagLowZigZags.Set(0);

			if (!isSwingHigh && !isSwingLow)
			{
				zigZagHighSeries.Set(currentZigZagHigh);
				zigZagLowSeries.Set(currentZigZagLow);
				return;
			}
			
			if (trendDir <= 0 && isSwingHigh && isOverHighDeviation)
			{	
				saveValue	= highSeries[1];
				addHigh		= true;
				trendDir	= 1;
			}	
			else if (trendDir >= 0 && isSwingLow && isOverLowDeviation)
			{	
				saveValue	= lowSeries[1];
				addLow		= true;
				trendDir	= -1;
			}	
			else if (trendDir == 1 && isSwingHigh && IsPriceGreater(highSeries[1], lastSwingPrice)) 
			{
				saveValue	= highSeries[1];
				updateHigh	= true;
			}
			else if (trendDir == -1 && isSwingLow && IsPriceGreater(lastSwingPrice, lowSeries[1])) 
			{
				saveValue	= lowSeries[1];
				updateLow	= true;
			}

			if (addHigh || addLow || updateHigh || updateLow)
			{
				if (updateHigh && lastSwingIdx >= 0)
				{
					zigZagHighZigZags.Set(CurrentBar - lastSwingIdx, 0);
					Value.Reset(CurrentBar - lastSwingIdx);
				}
				else if (updateLow && lastSwingIdx >= 0)
				{
					zigZagLowZigZags.Set(CurrentBar - lastSwingIdx, 0);
					Value.Reset(CurrentBar - lastSwingIdx);
				}

				if (addHigh || updateHigh)
				{
					zigZagHighZigZags.Set(1, saveValue);
					zigZagHighZigZags.Set(0, 0);

					currentZigZagHigh = saveValue;
					zigZagHighSeries.Set(1, currentZigZagHigh);
					Value.Set(1, currentZigZagHigh);
				}
				else if (addLow || updateLow) 
				{
					zigZagLowZigZags.Set(1, saveValue);
					zigZagLowZigZags.Set(0, 0);

					currentZigZagLow = saveValue;
					zigZagLowSeries.Set(1, currentZigZagLow);
					Value.Set(1, currentZigZagLow);
				}

				lastSwingIdx	= CurrentBar - 1;
				lastSwingPrice	= saveValue;
			}

			zigZagHighSeries.Set(currentZigZagHigh);
			zigZagLowSeries.Set(currentZigZagLow);
        }

        #region Properties
        [Description("Deviation in percent or points regarding on the deviation type")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName("Deviation value")]
        public double DeviationValue
        {
            get { return deviationValue; }
            set { deviationValue = Math.Max(0.0, value); }
        }

        [Description("Type of the deviation value")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName("Deviation type")]
        public DeviationType DeviationType
        {
            get { return deviationType; }
            set { deviationType = value; }
        }

        [Description("If true, high and low instead of selected price type is used to plot indicator.")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName("Use high and low")]
		[RefreshProperties(RefreshProperties.All)]
        public bool UseHighLow
        {
            get { return useHighLow; }
            set { useHighLow = value; }
        }

		/// <summary>
		/// Gets the ZigZag high points.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries ZigZagHigh
		{
			get 
			{ 
				Update();
				return zigZagHighSeries; 
			}
		}

		/// <summary>
		/// Gets the ZigZag low points.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries ZigZagLow
		{
			get 
			{ 
				Update();
				return zigZagLowSeries; 
			}
		}
        #endregion

		#region Miscellaneous

		/// <summary>
		/// #ENS#
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public override void GetMinMaxValues(Gui.Chart.ChartControl chartControl, ref double min, ref double max)
		{
			if (BarsArray[0] == null || ChartControl == null)
				return;

			for (int seriesCount = 0; seriesCount < Values.Length; seriesCount++)
			{
				for (int idx = this.FirstBarIndexPainted; idx <= this.LastBarIndexPainted; idx++)
				{
					if (zigZagHighZigZags.IsValidPlot(idx) && zigZagHighZigZags.Get(idx) != 0)
						max = Math.Max(max, zigZagHighZigZags.Get(idx));
					if (zigZagLowZigZags.IsValidPlot(idx) && zigZagLowZigZags.Get(idx) != 0)
						min = Math.Min(min, zigZagLowZigZags.Get(idx));
				}
			}
		}

		private bool IsPriceGreater(double a, double b)
		{
			if (a > b && a - b > TickSize / 2)
				return true; 
			else 
				return false;
		}

		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{
			if (Bars == null || ChartControl == null)
				return;

			IsValidPlot(Bars.Count - 1 + (CalculateOnBarClose ? -1 : 0)); // make sure indicator is calculated until last (existing) bar

			int preDiff = 1;
			for (int i = FirstBarIndexPainted - 1; i >= BarsRequired; i--)
			{
				if (i < 0)
					break;

				bool isHigh	= zigZagHighZigZags.IsValidPlot(i) && zigZagHighZigZags.Get(i) > 0;
				bool isLow	= zigZagLowZigZags.IsValidPlot(i) && zigZagLowZigZags.Get(i) > 0;
				
				if (isHigh || isLow)
					break;

				preDiff++;
			}

			int postDiff = 0;
			for (int i = LastBarIndexPainted; i <= zigZagHighZigZags.Count; i++)
			{
				if (i < 0)
					break;

				bool isHigh	= zigZagHighZigZags.IsValidPlot(i) && zigZagHighZigZags.Get(i) > 0;
				bool isLow	= zigZagLowZigZags.IsValidPlot(i) && zigZagLowZigZags.Get(i) > 0;

				if (isHigh || isLow)
					break;

				postDiff++;
			}

			bool linePlotted = false;
			using (GraphicsPath path = new GraphicsPath()) 
			{
				int		barWidth	= ChartControl.ChartStyle.GetBarPaintWidth(Bars.BarsData.ChartStyle.BarWidthUI);

				int		lastIdx		= -1; 
				double	lastValue	= -1; 

				for (int idx = this.FirstBarIndexPainted - preDiff; idx <= this.LastBarIndexPainted + postDiff; idx++)
				{
					if (idx - Displacement < 0 || idx - Displacement >= Bars.Count || (!ChartControl.ShowBarsRequired && idx - Displacement < BarsRequired))
						continue;

					bool isHigh	= zigZagHighZigZags.IsValidPlot(idx) && zigZagHighZigZags.Get(idx) > 0;
					bool isLow	= zigZagLowZigZags.IsValidPlot(idx) && zigZagLowZigZags.Get(idx) > 0;

					if (!isHigh && !isLow)
						continue;
					
					double value = isHigh ? zigZagHighZigZags.Get(idx) : zigZagLowZigZags.Get(idx);
					if (lastValue >= 0)
					{	
						int x0	= ChartControl.GetXByBarIdx(BarsArray[0], lastIdx);
						int x1	= ChartControl.GetXByBarIdx(BarsArray[0], idx);
						int y0	= ChartControl.GetYByValue(this, lastValue);
						int y1	= ChartControl.GetYByValue(this, value);

						path.AddLine(x0, y0, x1, y1);
						linePlotted = true;
					}

					// save as previous point
					lastIdx		= idx; 
					lastValue	= value; 
				}

				SmoothingMode oldSmoothingMode = graphics.SmoothingMode;
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
				graphics.DrawPath(Plots[0].Pen, path);
				graphics.SmoothingMode = oldSmoothingMode;
			}

			if (!linePlotted)
				DrawTextFixed("ZigZagErrorMsg", "ZigZag can't plot any values since the deviation value is too large. Please reduce it.", TextPosition.BottomRight);
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
        private ZigZag[] cacheZigZag = null;

        private static ZigZag checkZigZag = new ZigZag();

        /// <summary>
        /// The ZigZag indicator shows trend lines filtering out changes below a defined level. 
        /// </summary>
        /// <returns></returns>
        public ZigZag ZigZag(DeviationType deviationType, double deviationValue, bool useHighLow)
        {
            return ZigZag(Input, deviationType, deviationValue, useHighLow);
        }

        /// <summary>
        /// The ZigZag indicator shows trend lines filtering out changes below a defined level. 
        /// </summary>
        /// <returns></returns>
        public ZigZag ZigZag(Data.IDataSeries input, DeviationType deviationType, double deviationValue, bool useHighLow)
        {
            if (cacheZigZag != null)
                for (int idx = 0; idx < cacheZigZag.Length; idx++)
                    if (cacheZigZag[idx].DeviationType == deviationType && Math.Abs(cacheZigZag[idx].DeviationValue - deviationValue) <= double.Epsilon && cacheZigZag[idx].UseHighLow == useHighLow && cacheZigZag[idx].EqualsInput(input))
                        return cacheZigZag[idx];

            lock (checkZigZag)
            {
                checkZigZag.DeviationType = deviationType;
                deviationType = checkZigZag.DeviationType;
                checkZigZag.DeviationValue = deviationValue;
                deviationValue = checkZigZag.DeviationValue;
                checkZigZag.UseHighLow = useHighLow;
                useHighLow = checkZigZag.UseHighLow;

                if (cacheZigZag != null)
                    for (int idx = 0; idx < cacheZigZag.Length; idx++)
                        if (cacheZigZag[idx].DeviationType == deviationType && Math.Abs(cacheZigZag[idx].DeviationValue - deviationValue) <= double.Epsilon && cacheZigZag[idx].UseHighLow == useHighLow && cacheZigZag[idx].EqualsInput(input))
                            return cacheZigZag[idx];

                ZigZag indicator = new ZigZag();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.DeviationType = deviationType;
                indicator.DeviationValue = deviationValue;
                indicator.UseHighLow = useHighLow;
                Indicators.Add(indicator);
                indicator.SetUp();

                ZigZag[] tmp = new ZigZag[cacheZigZag == null ? 1 : cacheZigZag.Length + 1];
                if (cacheZigZag != null)
                    cacheZigZag.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheZigZag = tmp;
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
        /// The ZigZag indicator shows trend lines filtering out changes below a defined level. 
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZigZag ZigZag(DeviationType deviationType, double deviationValue, bool useHighLow)
        {
            return _indicator.ZigZag(Input, deviationType, deviationValue, useHighLow);
        }

        /// <summary>
        /// The ZigZag indicator shows trend lines filtering out changes below a defined level. 
        /// </summary>
        /// <returns></returns>
        public Indicator.ZigZag ZigZag(Data.IDataSeries input, DeviationType deviationType, double deviationValue, bool useHighLow)
        {
            return _indicator.ZigZag(input, deviationType, deviationValue, useHighLow);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The ZigZag indicator shows trend lines filtering out changes below a defined level. 
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.ZigZag ZigZag(DeviationType deviationType, double deviationValue, bool useHighLow)
        {
            return _indicator.ZigZag(Input, deviationType, deviationValue, useHighLow);
        }

        /// <summary>
        /// The ZigZag indicator shows trend lines filtering out changes below a defined level. 
        /// </summary>
        /// <returns></returns>
        public Indicator.ZigZag ZigZag(Data.IDataSeries input, DeviationType deviationType, double deviationValue, bool useHighLow)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.ZigZag(input, deviationType, deviationValue, useHighLow);
        }
    }
}
#endregion
