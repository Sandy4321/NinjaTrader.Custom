// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
//
#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;
#endregion

// This namespace holds all chart styles. Do not change it.
namespace NinjaTrader.Gui.Chart
{
	/// <summary>
	/// </summary>
	public class BoxStyle : ChartStyle
	{
		private static bool			registered			= Chart.ChartStyle.Register(new BoxStyle());
		
		private	SolidBrush			downBrush	= new SolidBrush(Color.Red);
		private	SolidBrush			upBrush		= new SolidBrush(Color.LightGreen);

		/// <summary>
		/// </summary>
		public BoxStyle() : base(ChartStyleType.Box)
		{
			this.DownColor		= Color.Red;
			this.UpColor		= Color.LightGreen;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			BoxStyle ret		= new BoxStyle();
			ret.Pen				= Gui.Globals.Clone(Pen);
			ret.Pen2			= Gui.Globals.Clone(Pen2);
			ret.DownColor		= DownColor;
			ret.UpColor			= UpColor;
			return ret;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override string DisplayName
		{ 
			get { return "Box"; }
		}

		/// <summary>
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			downBrush.Dispose();
			upBrush.Dispose();
		}

		/// <summary>
		/// </summary>
		/// <param name="barWidth"></param>
		/// <returns></returns>
		public override int GetBarPaintWidth(int barWidth)
		{
			// middle line + 2 * half of the body width + 2 * border line
			return (int) (1 + 2 * (barWidth) + 2 * Pen.Width);
		}

		/// <summary>
		/// </summary>
		/// <param name="propertyDescriptor"></param>
		/// <param name="chartStyle"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public override PropertyDescriptorCollection GetProperties(PropertyDescriptor propertyDescriptor, ChartStyle chartStyle, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = base.GetProperties(propertyDescriptor, chartStyle, attributes);
			properties.Remove(properties.Find("BarWidthUI", true));

			// here is how you change the display name of the property on the properties grid
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "DownColor",		"\r\r\rColor for down bars");
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Pen",			"\r\r\rUp bars outline");
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Pen2",			"\r\r\rDown bars outline");
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "UpColor",		"\r\r\rColor for up bars");

			return properties;
		}

		/// <summary>
		/// </summary>
		public override bool IsTransparent
		{
			get { return UpColor == Color.Transparent && DownColor == Color.Transparent && Pen.Color == Color.Transparent; }
		}

		/// <summary>
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="graphics"></param>
		/// <param name="bars"></param>
		/// <param name="panelIdx"></param>
		/// <param name="fromIdx"></param>
		/// <param name="toIdx"></param>
		/// <param name="bounds"></param>
		/// <param name="max"></param>
		/// <param name="min"></param>
		public override void PaintBars(ChartControl chartControl, Graphics graphics, Data.Bars bars, int panelIdx, int fromIdx, int toIdx, Rectangle bounds, double max, double min)
		{
			if (downBrush.Color != DownColor)
				downBrush.Color = DownColor;

			if (upBrush.Color != UpColor)
				upBrush.Color = UpColor;

			int			x;
			Color		barColor;
		    int			high;
		    double		highVal;
		    int			low;
		    double		lowVal;
			int			boxStartPosition	= -1;
			bool		drawLastBar			= false;
			int			chartMinX			= chartControl.CanvasRight - chartControl.BarMarginRight - GetBarPaintWidth(BarWidthUI) / 2  - ((chartControl.BarsPainted - 1) * chartControl.BarSpace);
			Color		candleOutlineColorScript;

			if (toIdx >= 0 && toIdx < bars.Count - 1)
				toIdx++;

			for (int idx = fromIdx; idx <= toIdx; idx++)
			{
				if (idx == 0)
					continue;

				barColor	= chartControl.GetBarOverrideColor(bars, idx);
				highVal		= bars.GetHigh(idx);
				high		= chartControl.GetYByValue(bars, highVal);
				lowVal		= bars.GetLow(idx);
				low			= chartControl.GetYByValue(bars, lowVal);
				x			= chartControl.GetXByBarIdx(bars, idx);
				candleOutlineColorScript = chartControl.GetCandleOutlineOverrideColor(bars, idx);

				if (idx == fromIdx)
					boxStartPosition = chartMinX;
				else
					boxStartPosition = chartControl.GetXByBarIdx(bars, idx - 1);
				
				boxStartPosition = (boxStartPosition < chartMinX ? chartMinX : boxStartPosition);
				if (x == boxStartPosition)
					continue;

				int width = Math.Max(2, Math.Abs(x - boxStartPosition));
				if (boxStartPosition + width > bounds.Width + bounds.X)
				    width = (bounds.X + bounds.Width) - boxStartPosition;

		        if (bars.GetClose(idx) >= bars.GetOpen(idx))
		        {
					Color oldBrushColor	= upBrush.Color;
					Color oldPenColor	= Pen.Color;

					if (barColor != Color.Empty)
						upBrush.Color = barColor;
					if (candleOutlineColorScript != Color.Empty)
						Pen.Color = candleOutlineColorScript;
					else if (barColor != Color.Empty)
						Pen.Color = barColor;

					width -= (int) Pen.Width;
		            graphics.FillRectangle(upBrush, boxStartPosition,	high,	width, low - high);
		            graphics.DrawRectangle(Pen,		boxStartPosition,	high,	width, low - high);

					if (barColor != Color.Empty)
						upBrush.Color = oldBrushColor;
					if (candleOutlineColorScript != Color.Empty || barColor != Color.Empty)
						Pen.Color = oldPenColor;
				}
		        else
				{
					Color oldBrushColor	= downBrush.Color;
					Color oldPenColor	= Pen2.Color;

					if (barColor != Color.Empty)
						downBrush.Color = barColor;
					if (candleOutlineColorScript != Color.Empty)
						Pen2.Color = candleOutlineColorScript;
					else if (barColor != Color.Empty)
						Pen2.Color = barColor;

					width -= (int) Pen2.Width;
					graphics.FillRectangle(downBrush,	boxStartPosition, high,	width, low - high);
		            graphics.DrawRectangle(Pen2,		boxStartPosition, high,	width, low - high);

					if (barColor != Color.Empty)
						downBrush.Color = oldBrushColor;
					if (candleOutlineColorScript != Color.Empty || barColor != Color.Empty)
						Pen2.Color = oldPenColor;
				}
				
				if (!drawLastBar)
					drawLastBar = true;
			}
		}
	}

	/// <summary>
	/// This implementation is a courtesy of Al Slane, an active NinjaTrader user. Thanks a lot for your contribution.
	/// </summary>
	public class OpenCloseStyle : ChartStyle 
	{
		private static bool	registered	= Chart.ChartStyle.Register(new OpenCloseStyle());
		private	SolidBrush	brush		= new SolidBrush(Color.LightGreen);

		/// <summary>
		/// </summary>
		public OpenCloseStyle() : base(ChartStyleType.OpenClose) 
		{
			BarWidth	= 3;
			UpColor		= Color.LightGreen;
			DownColor	= Color.Red;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone() 
		{
			OpenCloseStyle clone = new OpenCloseStyle();
			clone.BarWidth	= BarWidth;
			clone.UpColor	= UpColor;
			clone.DownColor	= DownColor;
			clone.Pen		= Gui.Globals.Clone(Pen);
			clone.Pen2		= Gui.Globals.Clone(Pen2);
			return clone;
		}

		/// <summary>
		/// </summary>
		public override string DisplayName 
		{ 
			get { return "Open/Close"; }
		}

		/// <summary>
		/// </summary>
		public override void Dispose() 
		{
			brush.Dispose();
			base.Dispose();
		}

		/// <summary>
		/// </summary>
		/// <param name="barWidth"></param>
		/// <returns></returns>
		public override int GetBarPaintWidth(int barWidth) 
		{
			// middle line + 2 * half of the body width + 2 * border line
			return (int) (1 + 2 * (barWidth - 1) + 2 * Pen.Width);
		}

		/// <summary>
		/// </summary>
		/// <param name="propertyDescriptor"></param>
		/// <param name="chartStyle"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public override PropertyDescriptorCollection GetProperties(PropertyDescriptor propertyDescriptor, ChartStyle chartStyle, Attribute[] attributes) 
		{
			PropertyDescriptorCollection properties = base.GetProperties(propertyDescriptor, chartStyle, attributes);

			// here is how you change the display name of the property on the properties grid
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "BarWidthUI",	"\r\r\rBar width");
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "DownColor",		"\r\r\rColor for down bars");
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Pen",			"\r\r\rUp bars outline");
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Pen2",			"\r\r\rDown bars outline");
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "UpColor",		"\r\r\rColor for up bars");

			return properties;
		}

		/// <summary>
		/// </summary>
		public override bool IsTransparent 
		{
			get { return UpColor == Color.Transparent && DownColor == Color.Transparent && Pen.Color == Color.Transparent && Pen2.Color == Color.Transparent; }
		}

		/// <summary>
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="graphics"></param>
		/// <param name="bars"></param>
		/// <param name="panelIdx"></param>
		/// <param name="fromIdx"></param>
		/// <param name="toIdx"></param>
		/// <param name="bounds"></param>
		/// <param name="max"></param>
		/// <param name="min"></param>
		public override void PaintBars(ChartControl chartControl, Graphics graphics, Data.Bars bars, int panelIdx, int fromIdx, int toIdx, Rectangle bounds, double max, double min) 
		{
			int		boxSize	 = bars.Period.Value;
			double	tickSize = bars.Instrument.MasterInstrument.TickSize;
			double	offset	 = boxSize * tickSize;

			int		barWidthValue = bars.BarsData.ChartStyle.BarWidthUI;
			int		barWidth	  = GetBarPaintWidth(barWidthValue);
			int     lastBarIdx    = bars.Count - 1;

			for (int idx = fromIdx; idx <= toIdx; idx++) 
			{
				double open      = bars.GetOpen(idx);
				double close     = bars.GetClose(idx);
			    bool   upBar     = open < close;

				int x      = chartControl.GetXByBarIdx(bars, idx);
				int openY  = chartControl.GetYByValue(bars, open);
				int closeY = chartControl.GetYByValue(bars, close);
				
				Pen pen = upBar ? Pen : Pen2;
				Color orgPenColor = pen.Color;
				Color barColor = chartControl.GetBarOverrideColor(bars, idx);
				Color olColor  = chartControl.GetCandleOutlineOverrideColor(bars, idx);
				if (olColor != Color.Empty) 
					pen.Color = olColor;
				else if (barColor != Color.Empty) 
					pen.Color = barColor;
				
				if (openY == closeY)
					graphics.DrawLine(pen, x - barWidth / 2, closeY, x + barWidth / 2, closeY);
				else 
				{
					brush.Color = barColor != Color.Empty ? barColor : upBar ? UpColor : DownColor;

					int minY = Math.Min(openY, closeY);
					int maxY = Math.Max(openY, closeY);
					
					graphics.FillRectangle(brush, x - (barWidth / 2) + 1, minY + 1, barWidth - 1, maxY - minY - 1);
					graphics.DrawRectangle(pen,   x - (barWidth / 2) + (pen.Width / 2), minY, barWidth - pen.Width, maxY - minY);
				}
				pen.Color = orgPenColor;
			}
		}
	}

	/// <summary>
	/// </summary>
	public class CandleStyle : ChartStyle
	{
		private static bool			registered	= Chart.ChartStyle.Register(new CandleStyle());
		
		private	SolidBrush			downBrush	= new SolidBrush(Color.Red);
		private	SolidBrush			upBrush		= new SolidBrush(Color.LightGreen);

		/// <summary>
		/// </summary>
		public CandleStyle() : base(ChartStyleType.CandleStick)
		{
			this.BarWidth		= 3;
			this.DownColor		= Color.Red;
			this.UpColor		= Color.LightGreen;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			CandleStyle ret		= new CandleStyle();
			ret.BarWidth		= BarWidth;
			ret.DownColor		= DownColor;
			ret.Pen				= Gui.Globals.Clone(Pen);
			ret.Pen2			= Gui.Globals.Clone(Pen2);
			ret.UpColor			= UpColor;
			return ret;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override string DisplayName
		{ 
			get { return "Candlestick"; }
		}

		/// <summary>
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			downBrush.Dispose();
			upBrush.Dispose();
		}

		/// <summary>
		/// </summary>
		/// <param name="barWidth"></param>
		/// <returns></returns>
		public override int GetBarPaintWidth(int barWidth)
		{
			// middle line + 2 * half of the body width + 2 * border line
			return (int) (1 + 2 * (barWidth - 1) + 2 * Pen.Width);
		}

		/// <summary>
		/// </summary>
		/// <param name="propertyDescriptor"></param>
		/// <param name="chartStyle"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public override PropertyDescriptorCollection GetProperties(PropertyDescriptor propertyDescriptor, ChartStyle chartStyle, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = base.GetProperties(propertyDescriptor, chartStyle, attributes);

			// here is how you change the display name of the property on the properties grid
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "BarWidthUI",	"\r\r\rBar width");
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "DownColor",		"\r\r\rColor for down bars");
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Pen",			"\r\r\rCandle outline");
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Pen2",			"\r\r\rWick");
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "UpColor",		"\r\r\rColor for up bars");

			return properties;
		}

		/// <summary>
		/// </summary>
		public override bool IsTransparent
		{
			get { return UpColor == Color.Transparent && DownColor == Color.Transparent && Pen.Color == Color.Transparent; }
		}

		/// <summary>
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="graphics"></param>
		/// <param name="bars"></param>
		/// <param name="panelIdx"></param>
		/// <param name="fromIdx"></param>
		/// <param name="toIdx"></param>
		/// <param name="bounds"></param>
		/// <param name="max"></param>
		/// <param name="min"></param>
		public override void PaintBars(ChartControl chartControl, Graphics graphics, Data.Bars bars, int panelIdx, int fromIdx, int toIdx, Rectangle bounds, double max, double min)
		{
			if (downBrush.Color != DownColor)
				downBrush.Color = DownColor;

			if (upBrush.Color != UpColor)
				upBrush.Color = UpColor;

			Color		barColor;
			int			barWidthValue = bars.BarsData.ChartStyle.BarWidthUI;
			int			barWidth;
			SolidBrush	brush		= upBrush;
			int			close;
			double		closeValue;
			int			high;
			int			low;
			Color		oldPenColor	= Pen.Color;
			Color		pen2Color	= Pen2.Color;
			int			open;
			double		openValue;
			int			x;

			for (int idx = fromIdx; idx <= toIdx; idx++)
			{
				barColor	= chartControl.GetBarOverrideColor(bars, idx);
				barWidth	= GetBarPaintWidth(barWidthValue);
				closeValue	= bars.GetClose(idx);
				close		= chartControl.GetYByValue(bars, closeValue);
				high		= chartControl.GetYByValue(bars, bars.GetHigh(idx));
				low			= chartControl.GetYByValue(bars, bars.GetLow(idx));
				openValue	= bars.GetOpen(idx);
				open		= chartControl.GetYByValue(bars, openValue);
				x			= chartControl.GetXByBarIdx(bars, idx);

				Color	candleOutlineColorScript		= chartControl.GetCandleOutlineOverrideColor(bars, idx);
				bool	isCandleOutlineColorScriptSet	= (chartControl.GetCandleOutlineOverrideColor(bars, idx) != Color.Empty);

				Pen.Color = oldPenColor;
				if (barColor != Color.Empty)
					Pen.Color = (candleOutlineColorScript != Color.Empty ? candleOutlineColorScript : barColor);
				else if (candleOutlineColorScript != Color.Empty)
					Pen.Color = candleOutlineColorScript;

				if (open == close)
					graphics.DrawLine(Pen, x - barWidth / 2, close, x + barWidth / 2, close);
				else
				{
					brush = closeValue >= openValue ? upBrush : downBrush;
					Color oldColor = brush.Color;
					if (barColor != Color.Empty)
						brush.Color = barColor;

					graphics.FillRectangle(brush, x - barWidth / 2 + 1, Math.Min(close, open) + 1,
						barWidth - 1, Math.Max(open, close) - Math.Min(close, open) - 1);

					if (barColor != Color.Empty)
						brush.Color = oldColor;

					graphics.DrawRectangle(Pen, x - (barWidth / 2) + (Pen.Width / 2), Math.Min(close, open), barWidth - Pen.Width, Math.Max(open, close) - Math.Min(close, open));
				}

				Pen2.Color = isCandleOutlineColorScriptSet ? candleOutlineColorScript : pen2Color;

				if (high < Math.Min(open, close))
					graphics.DrawLine(Pen2, x, high, x, Math.Min(open, close));

				if (low > Math.Max(open, close))
					graphics.DrawLine(Pen2, x, low, x, Math.Max(open, close));
			}
			Pen.Color = oldPenColor;
			Pen2.Color = pen2Color;
		}
	}

	/// <summary>
	/// </summary>
	public class HiLoBarsStyle : ChartStyle
	{
		private static bool		registered	= Chart.ChartStyle.Register(new HiLoBarsStyle());

		private	Pen				downPen		= new Pen(Color.Red, 1);
		private	Pen				upPen		= new Pen(Color.LimeGreen, 1);

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			HiLoBarsStyle ret	= new HiLoBarsStyle();
			ret.BarWidth		= BarWidth;
			ret.DownColor		= DownColor;
			ret.Pen				= Gui.Globals.Clone(Pen);
			ret.UpColor			= UpColor;
			return ret;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override string DisplayName
		{ 
			get { return "HiLo"; }
		}

		/// <summary>
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			downPen.Dispose();
			upPen.Dispose();
		}

		/// <summary>
		/// </summary>
		/// <param name="barWidth"></param>
		/// <returns></returns>
		public override int GetBarPaintWidth(int barWidth)
		{
			// middle line + 2 * open/close lines
			return (int) (upPen.Width + 2 * (2 + barWidth));
		}

		/// <summary>
		/// </summary>
		/// <param name="propertyDescriptor"></param>
		/// <param name="chartStyle"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public override PropertyDescriptorCollection GetProperties(PropertyDescriptor propertyDescriptor, ChartStyle chartStyle, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = base.GetProperties(propertyDescriptor, chartStyle, attributes);
			properties.Remove(properties.Find("Pen", true));
			properties.Remove(properties.Find("Pen2", true));

			// here is how you change the display name of the property on the properties grid
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "BarWidthUI",	"\r\r\rBar width");
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "DownColor",		"\r\r\rColor for down lines");
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "UpColor",		"\r\r\rColor for up lines");

			return properties;
		}

		/// <summary>
		/// </summary>
		public HiLoBarsStyle() : base(ChartStyleType.HiLoBars)
		{
			this.DownColor	= Color.Red;
			this.UpColor	= Color.LightGreen;
		}

		/// <summary>
		/// </summary>
		public override bool IsTransparent
		{
			get { return UpColor == Color.Transparent && DownColor == Color.Transparent; }
		}

		/// <summary>
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="graphics"></param>
		/// <param name="bars"></param>
		/// <param name="panelIdx"></param>
		/// <param name="fromIdx"></param>
		/// <param name="toIdx"></param>
		/// <param name="bounds"></param>
		/// <param name="max"></param>
		/// <param name="min"></param>
		public override void PaintBars(ChartControl chartControl, Graphics graphics, Data.Bars bars, int panelIdx, int fromIdx, int toIdx, Rectangle bounds, double max, double min)
		{	
			if (downPen.Color != DownColor)
				downPen.Color = DownColor;

			if (upPen.Color != UpColor)
				upPen.Color = UpColor;

			Color	barColor;
			int		barWidth		= bars.BarsData.ChartStyle.BarWidthUI;
			Color	ellipseColor	= UpColor;
			int		high;
			int		low;
			Color	oldColor;
			Pen		pen;
			int		penHalfWidth;
			int		penWidth;
			int		x;

			if (Math.Max(1, barWidth) != upPen.Width)
			{
				downPen		= new Pen(DownColor, Math.Max(1, barWidth));
				upPen		= new Pen(UpColor, Math.Max(1, barWidth));
			}

			for (int idx = fromIdx; idx <= toIdx; idx++)
			{
				barColor		= chartControl.GetBarOverrideColor(bars, idx);
				x				= chartControl.GetXByBarIdx(bars, idx);
				high			= chartControl.GetYByValue(bars, bars.GetHigh(idx));
				low				= chartControl.GetYByValue(bars, bars.GetLow(idx));
				pen				= (bars.GetClose(idx) >= bars.GetOpen(idx) ? upPen : downPen);
				oldColor		= pen.Color;
				penWidth		= (int)pen.Width;
				penHalfWidth	= (int)(pen.Width / 2);
				ellipseColor	= UpColor;

				if (barColor != Color.Empty)
				{
					pen.Color = barColor;
					ellipseColor = barColor;
				}

				if (high == low)
				{
					SolidBrush tmpBrush = new SolidBrush(ellipseColor);
					graphics.FillEllipse(tmpBrush, x - penHalfWidth - 1, Math.Min(high, low) - penHalfWidth - 1, pen.Width + 1, pen.Width + 1);
					tmpBrush.Dispose();
				}
				else
					graphics.DrawLine(pen, x, Math.Min(high, low) - penHalfWidth, x, Math.Max(high, low) + penHalfWidth);

				if (barColor != Color.Empty)
					pen.Color = oldColor;
			}
		}
	}

    /// <summary>
    /// </summary>
    public class KagiLineStyle : ChartStyle
    {
        private static bool registered = Register(new KagiLineStyle());
        private bool        thickLine  = false;

        /// <summary>
        /// </summary>
        public KagiLineStyle() : base(ChartStyleType.KagiLine)
        {
            DownColor  = Color.Red;
            UpColor    = Color.Green;
            Pen.Color  = Color.Green;
            Pen2.Color = Color.Red;
            Pen.Width  = 3;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new KagiLineStyle
                        {
                            BarWidth = BarWidth,
                            Pen      = Gui.Globals.Clone(Pen),
                            Pen2     = Gui.Globals.Clone(Pen2)
                        };
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string DisplayName
        {
            get { return "Kagi Line"; }
        }

        /// <summary>
        /// </summary>
        /// <param name="barWidth"></param>
        /// <returns></returns>
        public override int GetBarPaintWidth(int barWidth)
        {
            // middle line + 2 * half of the body width + 2 * border line
            return (int)(1 + 2 * (barWidth) + 2 * Pen2.Width);
        }

        /// <summary>
        /// </summary>
        /// <param name="propertyDescriptor"></param>
        /// <param name="chartStyle"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public override PropertyDescriptorCollection GetProperties(PropertyDescriptor propertyDescriptor, ChartStyle chartStyle, Attribute[] attributes)
        {
            PropertyDescriptorCollection properties = base.GetProperties(propertyDescriptor, chartStyle, attributes);
            properties.Remove(properties.Find("BarWidthUI", true));
            properties.Remove(properties.Find("DownColor", true));
            properties.Remove(properties.Find("UpColor", true));

            // here is how you change the display name of the property on the properties grid
            Design.DisplayNameAttribute.SetDisplayName(properties, "Pen", "\r\r\rThick line");
            Design.DisplayNameAttribute.SetDisplayName(properties, "Pen2", "\r\r\rThin line");

            return properties;
        }

        /// <summary>
        /// </summary>
        public override bool IsTransparent
        {
            get { return UpColor == Color.Transparent && DownColor == Color.Transparent && Pen.Color == Color.Transparent; }
        }

        /// <summary>
        /// </summary>
        /// <param name="chartControl"></param>
        /// <param name="graphics"></param>
        /// <param name="bars"></param>
        /// <param name="panelIdx"></param>
        /// <param name="fromIdx"></param>
        /// <param name="toIdx"></param>
        /// <param name="bounds"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        public override void PaintBars(ChartControl chartControl, Graphics graphics, Data.Bars bars, int panelIdx, int fromIdx, int toIdx, Rectangle bounds, double max, double min)
        {
			if (fromIdx > toIdx) // happens when all bars are off screen
				return;

            //Find start of session
            int sessionStartIndex = fromIdx;
            int thickOffsetLeft   = (int)(Pen.Width * 0.5);
            int thinOffsetLeft    = (int)(Pen2.Width * 0.5);
            int thickOffsetRight  = thickOffsetLeft + (Pen.Width > 2.0 && (int)Pen.Width % 2 == 1 ? 1 : 0);
            int thinOffsetRight   = thinOffsetLeft + (Pen2.Width > 2.0 && (int)Pen2.Width % 2 == 1 ? 1 : 0);

            while (sessionStartIndex > 0 && bars.BarsType.IsIntraday)
            {
                if (bars.IsFirstBarOfSession(sessionStartIndex))
                    break;
                sessionStartIndex--;
            }

			if (sessionStartIndex < 0) // happens when all bars are off screen
				return;

            //Determine start thick/thin
            thickLine = (bars.GetClose(sessionStartIndex) > bars.GetOpen(sessionStartIndex));

            //determine the next bar coming up if thick or thin. 
            for (int k = sessionStartIndex + 1; k < fromIdx; k++)
            {
                double closeTest = bars.GetClose(k);
                if (closeTest > bars.GetOpen(k))
                {
                    if (Math.Max(bars.GetOpen(k - 1), bars.GetClose(k - 1)) < closeTest)
                    {
                        //to Thick line
                        thickLine = true;
                    }
                }
                else if (closeTest < Math.Min(bars.GetOpen(k - 1), bars.GetClose(k - 1)))
                {
                    //to Thin line
                    thickLine = false;
                }
            }

            if (toIdx >= 0 && toIdx < bars.Count - 1)
                toIdx++;

            for (int idx = fromIdx; idx <= toIdx; idx++)
            {
                double openVal      = bars.GetOpen(idx);
                int    open         = chartControl.GetYByValue(bars, openVal);
                double closeVal     = bars.GetClose(idx);
                int    close        = chartControl.GetYByValue(bars, closeVal);
                int    x            = chartControl.GetXByBarIdx(bars, idx);
                double prevOpenVal  = (idx == 0 ? openVal : bars.GetOpen(idx - 1));
                double prevCloseVal = (idx == 0 ? closeVal : bars.GetClose(idx - 1));

                int startPosition;
                if (idx == 0 && toIdx >= 1)
                {
                    int x0        = chartControl.GetXByBarIdx(bars, 0);
                    int x1        = chartControl.GetXByBarIdx(bars, 1);
                    int diffX     = Math.Max(1, x1 - x0);
                    startPosition = x0 - diffX;
                }
                else
                    startPosition = idx == fromIdx ? bounds.X : chartControl.GetXByBarIdx(bars, idx - 1);

                startPosition = (startPosition < bounds.X ? bounds.X : startPosition);

                if (bars.BarsType.IsIntraday && bars.IsFirstBarOfSession(idx))
                {
                    //FirstBar
                    if (closeVal > openVal)
                    {
                        graphics.DrawLine(Pen, x, open, x, close);
                        thickLine = true;
                    }
                    else
                    {
                        graphics.DrawLine(Pen2, x, open, x, close);
                        thickLine = false;
                    }
                }
                else
                    if (closeVal > openVal)
                    {
                        if (closeVal <= Math.Max(prevCloseVal, prevOpenVal))
                        {
                            //maintain previous thickness
                            if (thickLine)
                            {
                                graphics.DrawLine(Pen, x, open, x, close);
                                graphics.DrawLine(Pen, startPosition - thickOffsetLeft, open, x + thickOffsetRight, open);
                            }
                            else
                            {
                                graphics.DrawLine(Pen2, x, open, x, close);
                                graphics.DrawLine(Pen2, startPosition - thinOffsetLeft, open, x + thinOffsetRight, open);
                            }
                        }
                        else if (closeVal > Math.Max(prevCloseVal, prevOpenVal))
                        {
                            //to Thick Line
                            double transitionPoint = Math.Max(prevCloseVal, prevOpenVal);
                            int    offsetLeft      = thickLine ? thickOffsetLeft : thinOffsetLeft;
                            int    offsetRight     = thickLine ? thickOffsetRight : thinOffsetRight;
                            graphics.DrawLine(Pen, x, close, x, chartControl.GetYByValue(bars, transitionPoint));
                            graphics.DrawLine((thickLine ? Pen : Pen2), x, chartControl.GetYByValue(bars, transitionPoint), x, open);
                            graphics.DrawLine((thickLine ? Pen : Pen2), startPosition - offsetLeft, open, x + offsetRight, open);

                            thickLine = true;
                        }
                    }
                    else
                    {
                        if (Math.Min(prevCloseVal, prevOpenVal) <= closeVal)
                        {
                            //maintain previous thickness
                            if (thickLine)
                            {
                                graphics.DrawLine(Pen, x, open, x, close);
                                graphics.DrawLine(Pen, startPosition - thickOffsetLeft, open, x + thickOffsetRight, open);
                            }
                            else
                            {
                                graphics.DrawLine(Pen2, x, open, x, close);
                                graphics.DrawLine(Pen2, startPosition - thinOffsetLeft, open, x + thinOffsetRight, open);
                            }
                        }
                        else if (closeVal < Math.Min(prevCloseVal, prevOpenVal))
                        {
                            //to Thin line
                            double transitionPoint = Math.Min(prevCloseVal, prevOpenVal);
                            int    offsetLeft      = thickLine ? thickOffsetLeft : thinOffsetLeft;
                            int    offsetRight     = thickLine ? thickOffsetRight : thinOffsetRight;
                            graphics.DrawLine((thickLine ? Pen : Pen2), startPosition - offsetLeft, open, x + offsetRight, open);
                            graphics.DrawLine((thickLine ? Pen : Pen2), x, open, x, chartControl.GetYByValue(bars, transitionPoint));
                            graphics.DrawLine(Pen2, x, chartControl.GetYByValue(bars, transitionPoint), x, close);

                            thickLine = false;
                        }
                    }
            }
        }
    }

	/// <summary>
	/// </summary>
	public class LineOnCloseStyle : ChartStyle
	{
		private static bool	registered	= Chart.ChartStyle.Register(new LineOnCloseStyle());

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			LineOnCloseStyle ret	= new LineOnCloseStyle();
			ret.BarWidth			= BarWidth;
			ret.Pen					= Gui.Globals.Clone(Pen);
			ret.DownColor			= DownColor;
			ret.UpColor				= UpColor;
			return ret;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override string DisplayName
		{ 
			get { return "Line on Close"; }
		}

		/// <summary>
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();
		}

		/// <summary>
		/// </summary>
		/// <param name="barWidth"></param>
		/// <returns></returns>
		public override int GetBarPaintWidth(int barWidth)
		{
			// middle line + 2 * open/close lines
			return (int) (Pen.Width + 2 * (2 + barWidth));
		}

		/// <summary>
		/// </summary>
		/// <param name="propertyDescriptor"></param>
		/// <param name="chartStyle"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public override PropertyDescriptorCollection GetProperties(PropertyDescriptor propertyDescriptor, ChartStyle chartStyle, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = base.GetProperties(propertyDescriptor, chartStyle, attributes);
			properties.Remove(properties.Find("DownColor", true));
			properties.Remove(properties.Find("Pen", true));
			properties.Remove(properties.Find("Pen2", true));

			// here is how you change the display name of the property on the properties grid
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "BarWidthUI",	"\r\r\r\rLine width");
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "UpColor",		"\r\r\rColor");

			return properties;
		}

		/// <summary>
		/// </summary>
		public override bool IsTransparent
		{
			get { return UpColor == Color.Transparent; }
		}

		/// <summary>
		/// </summary>
		public LineOnCloseStyle() : base(ChartStyleType.LineOnClose)
		{
			this.UpColor	= Color.Black;
		}

		/// <summary>
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="graphics"></param>
		/// <param name="bars"></param>
		/// <param name="panelIdx"></param>
		/// <param name="fromIdx"></param>
		/// <param name="toIdx"></param>
		/// <param name="bounds"></param>
		/// <param name="max"></param>
		/// <param name="min"></param>
		public override void PaintBars(ChartControl chartControl, Graphics graphics, Data.Bars bars, int panelIdx, int fromIdx, int toIdx, Rectangle bounds, double max, double min)
		{
			if (fromIdx >= toIdx) // DrawLines needs at least 2 elements to draw line
				return; 

			System.Drawing.Pen				pen		= new Pen(UpColor, bars.BarsData.ChartStyle.BarWidthUI);
			System.Collections.ArrayList	points	= new System.Collections.ArrayList();

			if (fromIdx > 0)			fromIdx--;
			if (toIdx < bars.Count - 1) toIdx++;

			for (int idx = fromIdx; idx <= toIdx; idx++)
			{
				int		x		= chartControl.GetXByBarIdx(bars, idx);
				Point	point	= new Point(x, chartControl.GetYByValue(bars, bars.GetClose(idx)));

				points.Add(point);
			}

			if (points.Count == 0)
				return;

			System.Drawing.Drawing2D.SmoothingMode	oldSmoothingMode = graphics.SmoothingMode;
			graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			graphics.DrawLines(pen, (Point[])points.ToArray(typeof(Point)));
			graphics.SmoothingMode = oldSmoothingMode;
		}
	}

	/// <summary>
	/// </summary>
	public class OhlcStyle : ChartStyle
	{
		private static bool		registered	= Chart.ChartStyle.Register(new OhlcStyle());

		private	Pen				downPen		= new Pen(Color.Red, 1);
		private	Pen				upPen		= new Pen(Color.LimeGreen, 1);

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			OhlcStyle ret		= new OhlcStyle();
			ret.BarWidth		= BarWidth;
			ret.DownColor		= DownColor;
			ret.Pen				= Gui.Globals.Clone(Pen);
			ret.UpColor			= UpColor;
			return ret;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override string DisplayName
		{ 
			get { return "OHLC"; }
		}

		/// <summary>
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();
			downPen.Dispose();
			upPen.Dispose();
		}

		/// <summary>
		/// </summary>
		/// <param name="barWidth"></param>
		/// <returns></returns>
		public override int GetBarPaintWidth(int barWidth)
		{
			// middle line + 2 * open/close lines
			return (int) (upPen.Width + 2 * (2 + barWidth));
		}

		/// <summary>
		/// </summary>
		/// <param name="propertyDescriptor"></param>
		/// <param name="chartStyle"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public override PropertyDescriptorCollection GetProperties(PropertyDescriptor propertyDescriptor, ChartStyle chartStyle, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = base.GetProperties(propertyDescriptor, chartStyle, attributes);
			properties.Remove(properties.Find("Pen", true));
			properties.Remove(properties.Find("Pen2", true));

			// here is how you change the display name of the property on the properties grid
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "BarWidthUI",	"\r\r\rBar width");
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "DownColor",		"\r\r\rColor for down bars");
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "UpColor",		"\r\r\rColor for up bars");

			return properties;
		}

		/// <summary>
		/// </summary>
		public override bool IsTransparent
		{
			get { return DownColor == Color.Transparent && UpColor == Color.Transparent; }
		}

		/// <summary>
		/// </summary>
		public OhlcStyle() : base(ChartStyleType.OHLC)
		{
			this.DownColor	= Color.Red;
			this.UpColor	= Color.LightGreen;
		}

		/// <summary>
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="graphics"></param>
		/// <param name="bars"></param>
		/// <param name="panelIdx"></param>
		/// <param name="fromIdx"></param>
		/// <param name="toIdx"></param>
		/// <param name="bounds"></param>
		/// <param name="max"></param>
		/// <param name="min"></param>
		public override void PaintBars(ChartControl chartControl, Graphics graphics, Data.Bars bars, int panelIdx, int fromIdx, int toIdx, Rectangle bounds, double max, double min)
		{
			Pen tmpUpPen	= Gui.Globals.Clone(this.upPen);	// work on copy, not on original object, otherwise issues with chart bars flickering on OHLC
			Pen tmpDownPen	= Gui.Globals.Clone(this.downPen);	// work on copy, not on original object, otherwise issues with chart bars flickering on OHLC

			if (tmpDownPen.Color != DownColor)
				tmpDownPen.Color = DownColor;

			if (tmpUpPen.Color != UpColor)
				tmpUpPen.Color = UpColor;

			Color	barColor;
			int		barWidth		= bars.BarsData.ChartStyle.BarWidthUI;
			int		close;
			double	closeValue;
			int		high;
			int		low;
			Color	oldColor		= Color.Empty;
			int		open;
			double	openValue;
			Pen		pen				= tmpUpPen;
			int		penWidth;
			int		penHalfWidth;
			int		x;

			if (Math.Max(1, barWidth - 2) != tmpUpPen.Width)
			{
				tmpDownPen	= new Pen(DownColor, Math.Max(1, barWidth - 2));
				tmpUpPen	= new Pen(UpColor, Math.Max(1, barWidth - 2));
			}

			for (int idx = fromIdx; idx <= toIdx; idx++)
			{
				barColor		= chartControl.GetBarOverrideColor(bars, idx);
				closeValue		= bars.GetClose(idx);
				close			= chartControl.GetYByValue(bars, closeValue);
				high			= chartControl.GetYByValue(bars, bars.GetHigh(idx));
				low				= chartControl.GetYByValue(bars, bars.GetLow(idx));
				openValue		= bars.GetOpen(idx);
				open			= chartControl.GetYByValue(bars, openValue);
				pen				= (closeValue >= openValue ? tmpUpPen : tmpDownPen);
				penHalfWidth	= (int) (pen.Width / 2);
				penWidth		= (int) pen.Width;
				x				= chartControl.GetXByBarIdx(bars, idx);

				if (barColor != Color.Empty)
				{
					oldColor	= pen.Color;
					pen.Color	= barColor;
				}

				pen.Width = penWidth;	// important to alleviate selecting the bar series

				graphics.DrawLine(pen, x,											Math.Min(high, low) - penHalfWidth,		x,											Math.Max(high, low) + penHalfWidth);
				graphics.DrawLine(pen, x - Math.Max(1, barWidth),					open,									x,											open);
				graphics.DrawLine(pen, x,											close,									x + Math.Max(1, barWidth),					close);

				if (barColor != Color.Empty)
					pen.Color = oldColor;
			}

			tmpDownPen.Dispose();	
			tmpUpPen.Dispose();	
		}
	}

	/// <summary>
	/// </summary>
	public class PointAndFigureStyle : ChartStyle
	{
		private Pen			downPen			= new Pen(Color.Red);
		private SolidBrush	hitBrush		= new SolidBrush(Color.FromArgb(2, 255, 255, 255)); // 'almost transparent' (0, 255, 255, 255)
		private Pen			pen;
		private static bool	registered		= Chart.ChartStyle.Register(new PointAndFigureStyle());
		private bool		trendDetermined = false;
		private Pen			upPen			= new Pen(Color.Green);

		/// <summary>
		/// </summary>
		public PointAndFigureStyle() : base(ChartStyleType.PointAndFigure)
		{
			this.DownColor		= Color.Red;
			this.UpColor		= Color.Green;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			PointAndFigureStyle ret	= new PointAndFigureStyle();
			ret.BarWidth	= BarWidth;
			ret.DownColor	= DownColor;
			ret.LineWidth	= LineWidth;
			ret.UpColor		= UpColor;
			return ret;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override string DisplayName
		{ 
			get { return "Point & Figure"; }
		}

		/// <summary>
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			downPen.Dispose();
			hitBrush.Dispose();
			upPen.Dispose();

			if (pen != null)
				pen.Dispose();
		}

		/// <summary>
		/// </summary>
		/// <param name="barWidth"></param>
		/// <returns></returns>
		public override int GetBarPaintWidth(int barWidth)
		{
			return (int) (1 + 2 * (barWidth) + 2 * Pen.Width);
		}

		/// <summary>
		/// </summary>
		[Gui.Design.DisplayName("\r\r\rLine width")]
		[Category("\rChart Style")]
		[XmlIgnore()]
		public int LineWidth
		{
			get { return (int) Pen.Width; }
			set { Pen.Width = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		/// <param name="propertyDescriptor"></param>
		/// <param name="chartStyle"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public override PropertyDescriptorCollection GetProperties(PropertyDescriptor propertyDescriptor, ChartStyle chartStyle, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = base.GetProperties(propertyDescriptor, chartStyle, attributes);
			properties.Remove(properties.Find("Pen", true));
			properties.Remove(properties.Find("Pen2", true));

			// here is how you change the display name of the property on the properties grid
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "DownColor",		"\r\rDown color");
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "UpColor",		"\r\rUp color");

			return properties;
		}

		/// <summary>
		/// </summary>
		public override bool IsTransparent
		{
			get { return UpColor == Color.Transparent && DownColor == Color.Transparent && Pen.Color == Color.Transparent; }
		}

		/// <summary>
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="graphics"></param>
		/// <param name="bars"></param>
		/// <param name="panelIdx"></param>
		/// <param name="fromIdx"></param>
		/// <param name="toIdx"></param>
		/// <param name="bounds"></param>
		/// <param name="max"></param>
		/// <param name="min"></param>
		public override void PaintBars(ChartControl chartControl, Graphics graphics, Data.Bars bars, int panelIdx, int fromIdx, int toIdx, Rectangle bounds, double max, double min)
		{   
			if (downPen.Color != DownColor)
		        downPen.Color = DownColor;

		    if (upPen.Color != UpColor)
				upPen.Color = UpColor;

			if (pen == null)
				pen = upPen;

			//Data.IBar	bar;
			int			barWidth;
		    int			barWidthValue	= bars.BarsData.ChartStyle.BarWidthUI;
			int			boxDrawCount;
			double		boxHeightActual	= Math.Floor(10000000.0 * (double) bars.Period.Value * bars.Instrument.MasterInstrument.TickSize) / 10000000.0;
			int			boxSize			= (int) Math.Round((double) (bounds.Height) / Math.Round((double) (ChartControl.MaxMinusMin(max, min) / boxHeightActual), 0));
			int			diff;
			int			open;
			double		openVal;
			int			close;
			double		closeVal;
			int			nextBox;
			int			x;

			System.Drawing.Drawing2D.SmoothingMode	oldSmoothingMode	= graphics.SmoothingMode;
			graphics.SmoothingMode										= System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			trendDetermined												= false;

			for (int idx = fromIdx; idx <= toIdx; idx++)
			{
				barWidth		= GetBarPaintWidth(barWidthValue);
				closeVal		= bars.GetClose(idx);
				openVal			= bars.GetOpen(idx);

				boxDrawCount	= (openVal == closeVal ? 1 : (int) Math.Round((Math.Abs(openVal - closeVal) / boxHeightActual), 0) + 1);
				close			= chartControl.GetYByValue(bars, closeVal);
				open			= chartControl.GetYByValue(bars, openVal);

				nextBox			= Math.Min(open, close);
				x				= chartControl.GetXByBarIdx(bars, idx);

				diff			= (Math.Abs(open - close) + boxSize) - (int) Math.Round((double)(boxSize * boxDrawCount));

				if (closeVal == openVal)
				{
					if (idx == 0)
					{
						graphics.DrawRectangle(downPen, new Rectangle(x - barWidth / 2 + 1, nextBox - (boxSize / 2) + 2, barWidth - 1, boxSize - 2));

						graphics.DrawLine(upPen,
						x - barWidth / 2,
						nextBox - (boxSize / 2) + 1,
						x + barWidth / 2,
						nextBox - (boxSize / 2) + boxSize - 1);

						graphics.DrawLine(upPen,
						x - barWidth / 2,
						nextBox - (boxSize / 2) + boxSize - 1,
						x + barWidth / 2,
						nextBox - (boxSize / 2) + 1);
						continue;
					}
					else if (!trendDetermined)
					{
						Data.Bar lastBar = (Data.Bar) bars.Get(idx - 1);
						if (lastBar.Open == lastBar.Close)
						{
							if(bars.GetHigh(idx) < lastBar.High)
								pen = downPen;
						}
						else 
							pen = (lastBar.Open < lastBar.Close ? downPen : upPen);

						trendDetermined = true;
					}
					else
						pen = (pen == upPen ? downPen : upPen);
				}
				else 
					pen = (closeVal > openVal ? upPen : downPen);

				float oldPenWidth		= Pen.Width;
				pen.Width				= LineWidth;
				
				for (int k = 0; k < boxDrawCount; k++)
				{
					if (diff != 0)
					{
						nextBox += (diff > 0 ? 1 : -1);
						diff	+= (diff > 0 ? -1 : 1);
					}

					if (pen == downPen)
					{
						Rectangle box = new Rectangle(x - barWidth / 2, nextBox - (boxSize / 2) + 2 , barWidth - 1, boxSize - 2);
						graphics.FillEllipse(hitBrush, box);
						graphics.DrawEllipse(pen, box);
					}
					else
					{
						graphics.DrawLine(pen,
						x - barWidth / 2,
						nextBox - (boxSize / 2) + 2,
						x + barWidth / 2,
						nextBox - (boxSize / 2) + boxSize - 2);

						graphics.DrawLine(pen,
						x - barWidth / 2,
						nextBox - (boxSize / 2) + boxSize - 2,
						x + barWidth / 2,
						nextBox - (boxSize / 2) + 2);
					}

					nextBox += boxSize;
				}
				pen.Width				= oldPenWidth;
			}
			graphics.SmoothingMode	= oldSmoothingMode;
		}
	}
}
