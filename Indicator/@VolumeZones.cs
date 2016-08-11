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
	/// Price by Volume.
	/// </summary>
	[Description("Volume Zones plots a horizontal histogram that overlays a price chart. The histogram bars stretch from left to right starting at the left side of the chart. The length of each bar is determined by the cumulative total of all volume bars for the periods during which the price fell within the vertical range of the histogram bar.")]
	public class VolumeZones : Indicator
	{
		#region Variables
		internal struct VolumeInfo
		{
			public double up;
			public double down;
			public double total;
		}

		private SolidBrush		barBrushDown;
		private SolidBrush		barBrushUp;
		private int				barCount		= 10;
		private Color			barColorDown	= Color.Red;
		private Color			barColorUp		= Color.Lime;
		private int				barSpacing		= 1;
		private bool			drawLines		= false;
		private Color			lineColor		= Color.Black;
		private Pen				linePen			= null;
		private int				transparency	= 80;
		private VolumeInfo[]	volumeInfo		= new VolumeInfo[20];
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			ChartOnly				= true;
			Overlay					= true;
			ZOrder					= -1;
			CalculateOnBarClose		= false;
			RecalculateColors();
			this.BarsRequired = 0;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
		}

        #region Miscellaneous
		/// <summary>
        /// Overload this method to handle the termination of an indicator. Use this method to dispose of any resources vs overloading the Dispose() method.
		/// </summary>
		protected override void OnTermination()
		{
			if (barBrushDown != null) 
				barBrushDown.Dispose();
			if (barBrushUp != null)
				barBrushUp.Dispose();
		}

		private int GetYPos(double price, Rectangle bounds, double min, double max)
		{
			return ChartControl.GetYByValue(this, price);
		}

		/// <summary>
		/// Called when the indicator is plotted.
		/// </summary>
		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{
			int lastBar		= Math.Min(this.LastBarIndexPainted, Bars.Count - 1);
			int barsPainted = ChartControl.BarsPainted;
			int firstBar	= this.FirstBarIndexPainted; 

			// Find highest and lowest price points
			double highPrice = 0;
			double lowPrice  = double.MaxValue;

			for (int idx = firstBar; idx <= lastBar && idx >= 0; idx++)
			{
				highPrice = Math.Max(highPrice, Bars.GetHigh(idx));
				lowPrice  = Math.Min(lowPrice , Bars.GetLow(idx));
			}

			int volumeBarCount	= this.BarCount;
			double priceRange	= highPrice - lowPrice;
			double priceBoxSize = priceRange / volumeBarCount;

			double volumeMax = 0;

			// Pass 1: Fill all VolumeInfo structures with appropriate data
			for (int i = 0; i < volumeBarCount; i++)
			{
				double priceUpper = lowPrice + priceBoxSize * (i + 1);
				double priceLower = lowPrice + priceBoxSize * i;

				double priceVolumeUp   = 0;
				double priceVolumeDown = 0;

				for (int idx = firstBar; idx <= lastBar; idx++)
				{
					double checkPrice;
					switch (PriceType)
					{
						case PriceType.Open:		checkPrice = Bars.GetOpen(idx); break;
						case PriceType.Close:		checkPrice = Bars.GetClose(idx); break;
						case PriceType.High:		checkPrice = Bars.GetHigh(idx);	break;
						case PriceType.Low:			checkPrice = Bars.GetLow(idx);	break;
						case PriceType.Median:		checkPrice = (Bars.GetHigh(idx) + Bars.GetLow(idx)) / 2; break;
						case PriceType.Typical:		checkPrice = (Bars.GetHigh(idx) + Bars.GetLow(idx) + Bars.GetClose(idx)) / 3; break;
						case PriceType.Weighted:	checkPrice = (Bars.GetHigh(idx) + Bars.GetLow(idx) + 2 * Bars.GetClose(idx)) / 4; break;
						default:					checkPrice = Bars.GetClose(idx); break;
					}

					if (checkPrice >= priceLower && checkPrice < priceUpper)
					{
						if (Bars.GetOpen(idx) < Bars.GetClose(idx))
							priceVolumeUp += Bars.GetVolume(idx);
						else
							priceVolumeDown += Bars.GetVolume(idx);
					}
				}

				volumeInfo[i].up	= priceVolumeUp;
				volumeInfo[i].down	= priceVolumeDown;
				volumeInfo[i].total = (double) priceVolumeUp + (double) priceVolumeDown;

				volumeMax = Math.Max(volumeMax, volumeInfo[i].total);
			}

			// Pass 2: Paint the volume bars
			for (int i = 0; i < Math.Min(volumeBarCount, lastBar - firstBar + 1); i++)
			{
				double priceUpper = lowPrice + priceBoxSize * (i + 1);
				double priceLower = lowPrice + priceBoxSize * i;

				int yUpper = GetYPos(priceUpper, bounds, min, max) + barSpacing;
				int yLower = GetYPos(priceLower, bounds, min, max);

				int barWidthUp   = (int) ((bounds.Width / 2) * (volumeInfo[i].up   / volumeMax));
				int barWidthDown = (int) ((bounds.Width / 2) * (volumeInfo[i].down / volumeMax));

				if (barWidthDown == int.MinValue || barWidthUp == int.MinValue)		// overflow?
					continue;

				int barWidth = barWidthUp + barWidthDown;

				graphics.FillRectangle(barBrushUp, new Rectangle(
						bounds.X, yUpper, 
						barWidthUp,	Math.Abs(yUpper - yLower)));

				graphics.FillRectangle(barBrushDown, new Rectangle(
						bounds.X + barWidthUp, yUpper, 
						barWidthDown, Math.Abs(yUpper - yLower)));

				if (drawLines == true) 
				{
					// Lower line
					graphics.DrawLine(linePen, 
						bounds.X,				 yLower,
						bounds.X + bounds.Width, yLower);

					// Upper line (only at the very top)
					if (i == volumeBarCount - 1) 
						graphics.DrawLine(linePen,
							bounds.X,					yUpper,						
							bounds.X + bounds.Width,	yUpper);
				}
			}
		}

		private void RecalculateColors() 
		{
			int	alpha = (int) (255.0 * ((100.0 - transparency) / 100.0));
			if (barBrushUp != null)
				barBrushUp.Dispose();
			barBrushUp   = new SolidBrush(Color.FromArgb(alpha, barColorUp.R, barColorUp.G, barColorUp.B));

			if (barBrushDown != null)
				barBrushDown.Dispose();
			barBrushDown = new SolidBrush(Color.FromArgb(alpha, barColorDown.R, barColorDown.G, barColorDown.B));
	
			if (linePen == null)
				linePen = new Pen(Color.FromArgb(alpha, lineColor.R, lineColor.G, lineColor.B));
		}
        #endregion

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Numbers of volume bars to display.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Bar count")]
		public int BarCount
		{
			get { return barCount; }
			set 
			{ 
				barCount = Math.Min(20, Math.Max(2,  value));

				if (barCount > volumeInfo.Length) 
					volumeInfo = new VolumeInfo[barCount];
			}
		}

		/// <summary>
		/// </summary>
		[XmlIgnore()]
		[Description("Color of volume bars representing down volume.")]
		[GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("Down color")]
		public Color BarColorDown
		{
			get { return barColorDown; }
			set { barColorDown = value; RecalculateColors(); }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		public string BarColorDownSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(barColorDown); }
			set { barColorDown = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore()]
		[Description("Color of volume bars representing up volume.")]
		[GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("Up color")]
		public Color BarColorUp
		{
			get { return barColorUp; }
			set { barColorUp = value; RecalculateColors(); }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		public string BarColorUpSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(barColorUp); }
			set { barColorUp = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Description("Spacing between the volume bars.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Bar spacing")]
		public int BarSpacing
		{
			get { return barSpacing; }
			set 
			{ 
				barSpacing = Math.Min(5, Math.Max(0,  value));
			}
		}

		/// <summary>
		/// </summary>
		[Description("Draw horizontal lines between the volume bars.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Draw lines")]
		public bool DrawLines
		{
			get { return drawLines; }
			set { drawLines = value; }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore()]
		[Description("Color of the line between the volume bars.")]
		[GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("Line color")]
		public Color LineColor
		{
			get { return lineColor; }
			set { lineColor = value; RecalculateColors(); }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		public string LineColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(lineColor); }
			set { lineColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Description("The percentage of transparency of the volume bars.")]
		[GridCategory("Parameters")]
		public int Transparency
		{
			get { return transparency; }
			set 
			{ 
				transparency = Math.Max(0 , value); 
				transparency = Math.Min(90, value);
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
        private VolumeZones[] cacheVolumeZones = null;

        private static VolumeZones checkVolumeZones = new VolumeZones();

        /// <summary>
        /// Volume Zones plots a horizontal histogram that overlays a price chart. The histogram bars stretch from left to right starting at the left side of the chart. The length of each bar is determined by the cumulative total of all volume bars for the periods during which the price fell within the vertical range of the histogram bar.
        /// </summary>
        /// <returns></returns>
        public VolumeZones VolumeZones(Color barColorDown, Color barColorUp, int barCount, int barSpacing, bool drawLines, Color lineColor, int transparency)
        {
            return VolumeZones(Input, barColorDown, barColorUp, barCount, barSpacing, drawLines, lineColor, transparency);
        }

        /// <summary>
        /// Volume Zones plots a horizontal histogram that overlays a price chart. The histogram bars stretch from left to right starting at the left side of the chart. The length of each bar is determined by the cumulative total of all volume bars for the periods during which the price fell within the vertical range of the histogram bar.
        /// </summary>
        /// <returns></returns>
        public VolumeZones VolumeZones(Data.IDataSeries input, Color barColorDown, Color barColorUp, int barCount, int barSpacing, bool drawLines, Color lineColor, int transparency)
        {
            if (cacheVolumeZones != null)
                for (int idx = 0; idx < cacheVolumeZones.Length; idx++)
                    if (cacheVolumeZones[idx].BarColorDown == barColorDown && cacheVolumeZones[idx].BarColorUp == barColorUp && cacheVolumeZones[idx].BarCount == barCount && cacheVolumeZones[idx].BarSpacing == barSpacing && cacheVolumeZones[idx].DrawLines == drawLines && cacheVolumeZones[idx].LineColor == lineColor && cacheVolumeZones[idx].Transparency == transparency && cacheVolumeZones[idx].EqualsInput(input))
                        return cacheVolumeZones[idx];

            lock (checkVolumeZones)
            {
                checkVolumeZones.BarColorDown = barColorDown;
                barColorDown = checkVolumeZones.BarColorDown;
                checkVolumeZones.BarColorUp = barColorUp;
                barColorUp = checkVolumeZones.BarColorUp;
                checkVolumeZones.BarCount = barCount;
                barCount = checkVolumeZones.BarCount;
                checkVolumeZones.BarSpacing = barSpacing;
                barSpacing = checkVolumeZones.BarSpacing;
                checkVolumeZones.DrawLines = drawLines;
                drawLines = checkVolumeZones.DrawLines;
                checkVolumeZones.LineColor = lineColor;
                lineColor = checkVolumeZones.LineColor;
                checkVolumeZones.Transparency = transparency;
                transparency = checkVolumeZones.Transparency;

                if (cacheVolumeZones != null)
                    for (int idx = 0; idx < cacheVolumeZones.Length; idx++)
                        if (cacheVolumeZones[idx].BarColorDown == barColorDown && cacheVolumeZones[idx].BarColorUp == barColorUp && cacheVolumeZones[idx].BarCount == barCount && cacheVolumeZones[idx].BarSpacing == barSpacing && cacheVolumeZones[idx].DrawLines == drawLines && cacheVolumeZones[idx].LineColor == lineColor && cacheVolumeZones[idx].Transparency == transparency && cacheVolumeZones[idx].EqualsInput(input))
                            return cacheVolumeZones[idx];

                VolumeZones indicator = new VolumeZones();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BarColorDown = barColorDown;
                indicator.BarColorUp = barColorUp;
                indicator.BarCount = barCount;
                indicator.BarSpacing = barSpacing;
                indicator.DrawLines = drawLines;
                indicator.LineColor = lineColor;
                indicator.Transparency = transparency;
                Indicators.Add(indicator);
                indicator.SetUp();

                VolumeZones[] tmp = new VolumeZones[cacheVolumeZones == null ? 1 : cacheVolumeZones.Length + 1];
                if (cacheVolumeZones != null)
                    cacheVolumeZones.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheVolumeZones = tmp;
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
        /// Volume Zones plots a horizontal histogram that overlays a price chart. The histogram bars stretch from left to right starting at the left side of the chart. The length of each bar is determined by the cumulative total of all volume bars for the periods during which the price fell within the vertical range of the histogram bar.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolumeZones VolumeZones(Color barColorDown, Color barColorUp, int barCount, int barSpacing, bool drawLines, Color lineColor, int transparency)
        {
            return _indicator.VolumeZones(Input, barColorDown, barColorUp, barCount, barSpacing, drawLines, lineColor, transparency);
        }

        /// <summary>
        /// Volume Zones plots a horizontal histogram that overlays a price chart. The histogram bars stretch from left to right starting at the left side of the chart. The length of each bar is determined by the cumulative total of all volume bars for the periods during which the price fell within the vertical range of the histogram bar.
        /// </summary>
        /// <returns></returns>
        public Indicator.VolumeZones VolumeZones(Data.IDataSeries input, Color barColorDown, Color barColorUp, int barCount, int barSpacing, bool drawLines, Color lineColor, int transparency)
        {
            return _indicator.VolumeZones(input, barColorDown, barColorUp, barCount, barSpacing, drawLines, lineColor, transparency);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Volume Zones plots a horizontal histogram that overlays a price chart. The histogram bars stretch from left to right starting at the left side of the chart. The length of each bar is determined by the cumulative total of all volume bars for the periods during which the price fell within the vertical range of the histogram bar.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolumeZones VolumeZones(Color barColorDown, Color barColorUp, int barCount, int barSpacing, bool drawLines, Color lineColor, int transparency)
        {
            return _indicator.VolumeZones(Input, barColorDown, barColorUp, barCount, barSpacing, drawLines, lineColor, transparency);
        }

        /// <summary>
        /// Volume Zones plots a horizontal histogram that overlays a price chart. The histogram bars stretch from left to right starting at the left side of the chart. The length of each bar is determined by the cumulative total of all volume bars for the periods during which the price fell within the vertical range of the histogram bar.
        /// </summary>
        /// <returns></returns>
        public Indicator.VolumeZones VolumeZones(Data.IDataSeries input, Color barColorDown, Color barColorUp, int barCount, int barSpacing, bool drawLines, Color lineColor, int transparency)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.VolumeZones(input, barColorDown, barColorUp, barCount, barSpacing, drawLines, lineColor, transparency);
        }
    }
}
#endregion
