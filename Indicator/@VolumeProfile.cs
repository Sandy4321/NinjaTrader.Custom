// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.Collections.Generic;
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
	[Description("Plots a vertical histogram of volume by price.")]
	public class VolumeProfile : Indicator
	{
		#region Variables
		internal class VolumeInfoItem
		{
			public double up      = 0;
			public double down    = 0;
			public double neutral = 0;
		}

		private int							alpha;
        private double                      askPrice            = 0;
		private SolidBrush					barBrushDown;
		private SolidBrush					barBrushInactive	= null;
		private SolidBrush					barBrushNeutral;
		private SolidBrush					barBrushUp;
		private int							barSpacing			= 1;
        private double                      bidPrice            = 0;
		private bool						drawLines			= false;
		private Color						lineColor			= Color.Black;
		private Pen							linePen				= null;
	    private int							startBarIndex		= 0;
		private DateTime					startTime			= Cbi.Globals.MinDate;
		private int							transparency		= 80;
		private Color						volumeColorDown		= Color.Red;
		private Color						volumeColorNeutral	= Color.Gray;
		private Color						volumeColorUp		= Color.Lime;

		private readonly SortedDictionary<double, VolumeInfoItem>  volumeInfo = new SortedDictionary<double, VolumeInfoItem>();
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			ChartOnly			= true;
			Overlay				= true;
			CalculateOnBarClose = false;
			ZOrder				= -1;
			startTime			= DateTime.Now;

			RecalculateColors();
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			// Mark the indicator start bar
			if (Time[0].Ticks >= startTime.Ticks && startBarIndex == 0)
				startBarIndex = CurrentBar;
			if (startBarIndex > 0 && startBarIndex == CurrentBar)
				DrawDiamond("Start Tag", false, 0, High[0] + TickSize, Color.Turquoise);
		}

        #region Miscellaneous
		/// <summary>
        /// Overload this method to handle the termination of an indicator. Use this method to dispose of any resources vs overloading the Dispose() method.
		/// </summary>
		protected override void OnTermination()
		{
			if (barBrushDown     != null) barBrushDown.Dispose();
			if (barBrushInactive != null) barBrushInactive.Dispose();
			if (barBrushNeutral  != null) barBrushNeutral.Dispose();
			if (barBrushUp       != null) barBrushUp.Dispose();
            if (linePen          != null) linePen.Dispose();
		}

	    protected override void OnMarketData(MarketDataEventArgs e)
		{
            if (e.MarketDataType == MarketDataType.Ask)
            {
                askPrice = e.Price;
                return;
            }
		    if (e.MarketDataType == MarketDataType.Bid)
		    {
		        bidPrice = e.Price;
		        return;
		    }
		    if (e.MarketDataType != MarketDataType.Last || ChartControl == null || askPrice == 0 || bidPrice == 0)
		        return;

		    if (Bars != null && !Bars.Session.InSession(DateTime.Now, Bars.Period, true, Bars.BarsType))
				return;

			double	price	= e.Price;
			long	volume	= e.Volume;

			if (!volumeInfo.ContainsKey(price))
				volumeInfo.Add(price, new VolumeInfoItem());

			VolumeInfoItem volumeInfoItem = volumeInfo[price];

			if (price >= askPrice) 
				volumeInfoItem.up += volume;
			else 
                if (price <= bidPrice)
				    volumeInfoItem.down += volume;
			    else
				    volumeInfoItem.neutral += volume;
		}

		/// <summary>
		/// Called when the indicator is plotted.
		/// </summary>
		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{
			// Paranoia
			if (Bars == null || Bars.Instrument == null)
				return;

			double	tickSize	= Bars.Instrument.MasterInstrument.TickSize;

			// Check if we should 'gray out' the bars
            bool isInactive = (ChartControl.EquidistantBars && ChartControl.LastBarPainted < BarsArray[0].Count - 1
                || !ChartControl.EquidistantBars && ChartControl.LastBarTimePainted < Time[0]);

			double volumeMax = 0;

			// Figure out the max volume
			foreach (KeyValuePair<double, VolumeInfoItem> keyValue in volumeInfo)
			{
				double price = keyValue.Key;

				// Don't watch volume for prices outside the visible chart
				if (price > max  || price < min) continue;

				VolumeInfoItem  vii      = keyValue.Value;
				double 			total = vii.up + vii.down + vii.neutral;

				volumeMax                = Math.Max(volumeMax, total);
   			}

			if (volumeMax == 0) return;

			SolidBrush upBrush		= barBrushUp;
			SolidBrush downBrush	= barBrushDown;
			SolidBrush neutralBrush	= barBrushNeutral;

			if (isInactive)
			{
				if (barBrushInactive == null)
				{
					barBrushInactive = new SolidBrush(Color.FromArgb(Math.Min(255, alpha + 50), 
							ChartControl.PriceMarkerInactive.R, 
							ChartControl.PriceMarkerInactive.G, 
							ChartControl.PriceMarkerInactive.B));
				}

				upBrush = downBrush = neutralBrush = barBrushInactive;
			}

			int viiPosition = 0;

			// Plot 'em
			foreach (KeyValuePair<double, VolumeInfoItem> keyValue in volumeInfo)
			{
				viiPosition++;

				VolumeInfoItem vii  = keyValue.Value;

			    double priceLower   = keyValue.Key - tickSize / 2;
                int yLower          = ChartControl.GetYByValue(this, priceLower);
                int yUpper          = ChartControl.GetYByValue(this, priceLower + tickSize);
				int height          = Math.Max(1, Math.Abs(yUpper - yLower) - barSpacing);

 				int barWidthUp		= (int) ((bounds.Width / 2) * (vii.up      / volumeMax));
				int barWidthNeutral = (int) ((bounds.Width / 2) * (vii.neutral / volumeMax));
				int barWidthDown	= (int) ((bounds.Width / 2) * (vii.down    / volumeMax));

				int xpos            = bounds.X;

				graphics.FillRectangle(upBrush,      new Rectangle(xpos, yUpper, barWidthUp, height));
				xpos += barWidthUp;				
				graphics.FillRectangle(neutralBrush, new Rectangle(xpos, yUpper, barWidthNeutral, height));
				xpos += barWidthNeutral;
				graphics.FillRectangle(downBrush,    new Rectangle(xpos, yUpper, barWidthDown, height));

			    if (!drawLines) continue;
			    // Lower line
			    graphics.DrawLine(linePen, bounds.X, yLower-1, bounds.X + bounds.Width, yLower-1);

			    // Upper line (only at the very top)
			    if (viiPosition == volumeInfo.Count) 
			        graphics.DrawLine(linePen, bounds.X, yUpper-1, bounds.X + bounds.Width, yUpper-1);
			}
		}

		private void RecalculateColors() 
		{
			alpha			 = (int) (255.0 * ((100.0 - transparency) / 100.0));

			if (barBrushUp != null) barBrushUp.Dispose();
			barBrushUp = new SolidBrush(Color.FromArgb(alpha, volumeColorUp.R, volumeColorUp.G, volumeColorUp.B));

			if (barBrushNeutral != null) barBrushNeutral.Dispose();
			barBrushNeutral = new SolidBrush(Color.FromArgb(alpha, volumeColorNeutral.R, volumeColorNeutral.G, volumeColorNeutral.B));

			if (barBrushDown != null) barBrushDown.Dispose();
			barBrushDown = new SolidBrush(Color.FromArgb(alpha, volumeColorDown.R, volumeColorDown.G, volumeColorDown.B));

            if (linePen != null) linePen.Dispose();
			if (drawLines)
                linePen = new Pen(Color.FromArgb(alpha, lineColor.R, lineColor.G, lineColor.B));
		}
        #endregion

		#region Properties
		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Color of volume bars representing sell trades.")]
		[GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("Sell color")]
		public Color VolumeColorDown
		{
			get { return volumeColorDown; }
			set { volumeColorDown = value; RecalculateColors(); }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		public string VolumeColorDownSerialize
		{
			get { return Gui.Design.SerializableColor.ToString(volumeColorDown); }
			set { volumeColorDown = Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Color of volume bars representing trades in between the market.")]
		[GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("Neutral color")]
		public Color VolumeColorNeutral
		{
			get { return volumeColorNeutral; }
			set { volumeColorNeutral = value; RecalculateColors(); }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		public string VolumeColorNeutralSerialize
		{
			get { return Gui.Design.SerializableColor.ToString(volumeColorNeutral); }
			set { volumeColorNeutral = Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Color of volume bars representing buy trades.")]
		[GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("Buy color")]
		public Color VolumeColorUp
		{
			get { return volumeColorUp; }
			set { volumeColorUp = value; RecalculateColors(); }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		public string VolumeColorUpSerialize
		{
			get { return Gui.Design.SerializableColor.ToString(volumeColorUp); }
			set { volumeColorUp = Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Description("Spacing between the volume bars.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Bar spacing")]
		public int BarSpacing
		{
			get { return barSpacing; }
			set { barSpacing = Math.Min(5, Math.Max(0,  value)); }
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
		[XmlIgnore]
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
			get { return Gui.Design.SerializableColor.ToString(lineColor); }
			set { lineColor = Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Description("The percentage of transparency of the volume bars.")]
		[GridCategory("Parameters")]
		public int Transparency
		{
			get { return transparency; }
			set { transparency = Math.Min(90, Math.Max(0 , value)); }
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
        private VolumeProfile[] cacheVolumeProfile = null;

        private static VolumeProfile checkVolumeProfile = new VolumeProfile();

        /// <summary>
        /// Plots a vertical histogram of volume by price.
        /// </summary>
        /// <returns></returns>
        public VolumeProfile VolumeProfile(int barSpacing, bool drawLines, int transparency)
        {
            return VolumeProfile(Input, barSpacing, drawLines, transparency);
        }

        /// <summary>
        /// Plots a vertical histogram of volume by price.
        /// </summary>
        /// <returns></returns>
        public VolumeProfile VolumeProfile(Data.IDataSeries input, int barSpacing, bool drawLines, int transparency)
        {
            if (cacheVolumeProfile != null)
                for (int idx = 0; idx < cacheVolumeProfile.Length; idx++)
                    if (cacheVolumeProfile[idx].BarSpacing == barSpacing && cacheVolumeProfile[idx].DrawLines == drawLines && cacheVolumeProfile[idx].Transparency == transparency && cacheVolumeProfile[idx].EqualsInput(input))
                        return cacheVolumeProfile[idx];

            lock (checkVolumeProfile)
            {
                checkVolumeProfile.BarSpacing = barSpacing;
                barSpacing = checkVolumeProfile.BarSpacing;
                checkVolumeProfile.DrawLines = drawLines;
                drawLines = checkVolumeProfile.DrawLines;
                checkVolumeProfile.Transparency = transparency;
                transparency = checkVolumeProfile.Transparency;

                if (cacheVolumeProfile != null)
                    for (int idx = 0; idx < cacheVolumeProfile.Length; idx++)
                        if (cacheVolumeProfile[idx].BarSpacing == barSpacing && cacheVolumeProfile[idx].DrawLines == drawLines && cacheVolumeProfile[idx].Transparency == transparency && cacheVolumeProfile[idx].EqualsInput(input))
                            return cacheVolumeProfile[idx];

                VolumeProfile indicator = new VolumeProfile();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BarSpacing = barSpacing;
                indicator.DrawLines = drawLines;
                indicator.Transparency = transparency;
                Indicators.Add(indicator);
                indicator.SetUp();

                VolumeProfile[] tmp = new VolumeProfile[cacheVolumeProfile == null ? 1 : cacheVolumeProfile.Length + 1];
                if (cacheVolumeProfile != null)
                    cacheVolumeProfile.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheVolumeProfile = tmp;
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
        /// Plots a vertical histogram of volume by price.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolumeProfile VolumeProfile(int barSpacing, bool drawLines, int transparency)
        {
            return _indicator.VolumeProfile(Input, barSpacing, drawLines, transparency);
        }

        /// <summary>
        /// Plots a vertical histogram of volume by price.
        /// </summary>
        /// <returns></returns>
        public Indicator.VolumeProfile VolumeProfile(Data.IDataSeries input, int barSpacing, bool drawLines, int transparency)
        {
            return _indicator.VolumeProfile(input, barSpacing, drawLines, transparency);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Plots a vertical histogram of volume by price.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolumeProfile VolumeProfile(int barSpacing, bool drawLines, int transparency)
        {
            return _indicator.VolumeProfile(Input, barSpacing, drawLines, transparency);
        }

        /// <summary>
        /// Plots a vertical histogram of volume by price.
        /// </summary>
        /// <returns></returns>
        public Indicator.VolumeProfile VolumeProfile(Data.IDataSeries input, int barSpacing, bool drawLines, int transparency)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.VolumeProfile(input, barSpacing, drawLines, transparency);
        }
    }
}
#endregion
