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
    /// HeikenAshi technique discussed in the article 'Using Heiken-Ashi Technique' in February 2004 issue of TASC magazine.
    /// </summary>
    [Description("HeikenAshi technique discussed in the article 'Using Heiken-Ashi Technique' in February 2004 issue of TASC magazine.")]
    public class HeikenAshi : Indicator
    {
        #region Variables

        private Color           barColorDown         = Color.Red;
        private Color           barColorUp           = Color.Lime;
        private SolidBrush      brushDown            = null;
        private SolidBrush      brushUp              = null;
        private Color           shadowColor          = Color.Black;
        private Pen             shadowPen            = null;
        private int             shadowWidth          = 1;

        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Gray, PlotStyle.Line, "HAOpen"));
            Add(new Plot(Color.Gray, PlotStyle.Line, "HAHigh"));
            Add(new Plot(Color.Gray, PlotStyle.Line, "HALow"));
            Add(new Plot(Color.Gray, PlotStyle.Line, "HAClose"));
            PaintPriceMarkers   = false;
            CalculateOnBarClose = false;
            PlotsConfigurable   = false;
            Overlay             = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (Displacement + (CalculateOnBarClose ? 1 : 0) > 0 && CurrentBar > 0 && BarColorSeries[1] != Color.Transparent)
				InitColorSeries();

			BarColorSeries.Set(Math.Max(0, CurrentBar + Math.Max(0, Displacement) + (CalculateOnBarClose ? 1 : 0)), Color.Transparent);
			CandleOutlineColorSeries.Set(Math.Max(0, CurrentBar + Math.Max(0, Displacement) + (CalculateOnBarClose ? 1 : 0)), Color.Transparent);

			if (CurrentBar == 0)
            {
                HAOpen.Set(Open[0]);
                HAHigh.Set(High[0]);
                HALow.Set(Low[0]);
                HAClose.Set(Close[0]);
                return;
            }

            HAClose.Set((Open[0] + High[0] + Low[0] + Close[0]) * 0.25); // Calculate the close
            HAOpen.Set((HAOpen[1] + HAClose[1]) * 0.5); // Calculate the open
            HAHigh.Set(Math.Max(High[0], HAOpen[0])); // Calculate the high
            HALow.Set(Math.Min(Low[0], HAOpen[0])); // Calculate the low
        }

        #region Properties

        [Browsable(false)]
        [XmlIgnore]
        public DataSeries HAOpen
        {
            get { return Values[0]; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public DataSeries HAHigh
        {
            get { return Values[1]; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public DataSeries HALow
        {
            get { return Values[2]; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public DataSeries HAClose
        {
            get { return Values[3]; }
        }

        [XmlIgnore]
        [Description("Color of down bars.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Down color")]
        public Color BarColorDown
        {
            get { return barColorDown; }
            set { barColorDown = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string BarColorDownSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(barColorDown); }
            set { barColorDown = Gui.Design.SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [XmlIgnore]
        [Description("Color of up bars.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Up color")]
        public Color BarColorUp
        {
            get { return barColorUp; }
            set { barColorUp = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string BarColorUpSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(barColorUp); }
            set { barColorUp = Gui.Design.SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [XmlIgnore]
        [Description("Color of shadow line.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Shadow color")]
        public Color ShadowColor
        {
            get { return shadowColor; }
            set { shadowColor = value; }
        }

        /// <summary>
        /// </summary>
        [Browsable(false)]
        public string ShadowColorSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(shadowColor); }
            set { shadowColor = Gui.Design.SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Width of shadow line.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("Shadow width")]
        public int ShadowWidth
        {
            get { return shadowWidth; }
            set { shadowWidth = Math.Max(value, 1); }
        }

        #endregion

        #region Miscellaneous

		private void InitColorSeries()
		{
			for (int i = 0; i <= CurrentBar + Displacement + (CalculateOnBarClose ? 1 : 0); i++)
			{
				BarColorSeries.Set(i, Color.Transparent);
				CandleOutlineColorSeries.Set(i, Color.Transparent);
			}
		}

        protected override void OnStartUp()
        {
            if (ChartControl == null || Bars == null)
                return;

            brushUp                             = new SolidBrush(barColorUp);
            brushDown                           = new SolidBrush(barColorDown);
            shadowPen                           = new Pen(shadowColor, shadowWidth);
        }

        protected override void OnTermination()
        {
            if (brushUp != null) brushUp.Dispose();
            if (brushDown != null) brushDown.Dispose();
            if (shadowPen != null) shadowPen.Dispose();
        }

        public override void GetMinMaxValues(ChartControl chartControl, ref double min, ref double max)
        {
            if (Bars == null || ChartControl == null)
                return;

            for (int idx = FirstBarIndexPainted; idx <= LastBarIndexPainted; idx++)
            {
                double tmpHigh = HAHigh.Get(idx);
                double tmpLow = HALow.Get(idx);

                if (tmpHigh != 0 && tmpHigh > max)
                    max = tmpHigh;
                if (tmpLow != 0 && tmpLow < min)
                    min = tmpLow;
            }
        }

        public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
        {
            if (Bars == null || ChartControl == null)
                return;

            int barPaintWidth = Math.Max(3, 1 + 2 * ((int)Bars.BarsData.ChartStyle.BarWidth - 1) + 2 * shadowWidth);

            for (int idx = FirstBarIndexPainted; idx <= LastBarIndexPainted; idx++)
            {
                if (idx - Displacement < 0 || idx - Displacement >= BarsArray[0].Count || (!ChartControl.ShowBarsRequired && idx - Displacement < BarsRequired))
                    continue;
                double valH = HAHigh.Get(idx);
                double valL = HALow.Get(idx);
                double valC = HAClose.Get(idx);
                double valO = HAOpen.Get(idx);
                int x = ChartControl.GetXByBarIdx(BarsArray[0], idx);
                int y1 = ChartControl.GetYByValue(this, valO);
                int y2 = ChartControl.GetYByValue(this, valH);
                int y3 = ChartControl.GetYByValue(this, valL);
                int y4 = ChartControl.GetYByValue(this, valC);

                graphics.DrawLine(shadowPen, x, y2, x, y3);

                if (y4 == y1)
                    graphics.DrawLine(shadowPen, x - barPaintWidth / 2, y1, x + barPaintWidth / 2, y1);
                else
                {
                    if (y4 > y1)
                        graphics.FillRectangle(brushDown, x - barPaintWidth / 2, y1, barPaintWidth, y4 - y1);
                    else
                        graphics.FillRectangle(brushUp, x - barPaintWidth / 2, y4, barPaintWidth, y1 - y4);
                    graphics.DrawRectangle(shadowPen, (x - barPaintWidth / 2) + (shadowPen.Width / 2), Math.Min(y4, y1), barPaintWidth - shadowPen.Width, Math.Abs(y4 - y1));
                }
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
        private HeikenAshi[] cacheHeikenAshi = null;

        private static HeikenAshi checkHeikenAshi = new HeikenAshi();

        /// <summary>
        /// HeikenAshi technique discussed in the article 'Using Heiken-Ashi Technique' in February 2004 issue of TASC magazine.
        /// </summary>
        /// <returns></returns>
        public HeikenAshi HeikenAshi()
        {
            return HeikenAshi(Input);
        }

        /// <summary>
        /// HeikenAshi technique discussed in the article 'Using Heiken-Ashi Technique' in February 2004 issue of TASC magazine.
        /// </summary>
        /// <returns></returns>
        public HeikenAshi HeikenAshi(Data.IDataSeries input)
        {
            if (cacheHeikenAshi != null)
                for (int idx = 0; idx < cacheHeikenAshi.Length; idx++)
                    if (cacheHeikenAshi[idx].EqualsInput(input))
                        return cacheHeikenAshi[idx];

            lock (checkHeikenAshi)
            {
                if (cacheHeikenAshi != null)
                    for (int idx = 0; idx < cacheHeikenAshi.Length; idx++)
                        if (cacheHeikenAshi[idx].EqualsInput(input))
                            return cacheHeikenAshi[idx];

                HeikenAshi indicator = new HeikenAshi();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                HeikenAshi[] tmp = new HeikenAshi[cacheHeikenAshi == null ? 1 : cacheHeikenAshi.Length + 1];
                if (cacheHeikenAshi != null)
                    cacheHeikenAshi.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheHeikenAshi = tmp;
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
        /// HeikenAshi technique discussed in the article 'Using Heiken-Ashi Technique' in February 2004 issue of TASC magazine.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.HeikenAshi HeikenAshi()
        {
            return _indicator.HeikenAshi(Input);
        }

        /// <summary>
        /// HeikenAshi technique discussed in the article 'Using Heiken-Ashi Technique' in February 2004 issue of TASC magazine.
        /// </summary>
        /// <returns></returns>
        public Indicator.HeikenAshi HeikenAshi(Data.IDataSeries input)
        {
            return _indicator.HeikenAshi(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// HeikenAshi technique discussed in the article 'Using Heiken-Ashi Technique' in February 2004 issue of TASC magazine.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.HeikenAshi HeikenAshi()
        {
            return _indicator.HeikenAshi(Input);
        }

        /// <summary>
        /// HeikenAshi technique discussed in the article 'Using Heiken-Ashi Technique' in February 2004 issue of TASC magazine.
        /// </summary>
        /// <returns></returns>
        public Indicator.HeikenAshi HeikenAshi(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.HeikenAshi(input);
        }
    }
}
#endregion
