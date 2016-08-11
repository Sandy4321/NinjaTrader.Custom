// 
// Copyright (C) 2007, NinjaTrader LLC <www.ninjatrader.com>.
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
    /// Sends an alert to the alert window when a price level has been reached.
    /// </summary>
    [Description("Sends an alert to the alert window when a price level has been reached.")]
    public class PriceAlert : Indicator
    {
        #region Variables
        private double          fixedPrice           = 0;
        private double          price                = 0;
        private bool            showTriggerLine      = true;
        private bool            triggered            = false;
        private bool            triggerOnGreaterThan = false;
        private bool            triggerSet           = false;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Navy, PlotStyle.Line, "Trigger line"));
            ChartOnly           = true;
            AutoScale           = false;
            CalculateOnBarClose = false;
            DisplayInDataBox    = false;
            Overlay             = true;
        }

        protected override void OnStartUp()
        {
            triggered = fixedPrice > 0 ? false : true;
            price     = Instrument.MasterInstrument.Round2TickSize(fixedPrice);
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            if (CurrentBar < Bars.Count - 1)
            {
                Value.Set(price);
                return;
            }

			IHorizontalLine	alertLine	= null;
			IDrawObject		drawObject	= DrawObjects["TriggerLine"];
			if (drawObject != null && drawObject.DrawType == DrawType.HorizontalLine)
				alertLine = (drawObject as IHorizontalLine);

            if (showTriggerLine && alertLine == null)
            {
                alertLine        = DrawHorizontalLine("TriggerLine", AutoScale, fixedPrice > 0 ? fixedPrice : Input[0], Plots[0].Pen.Color, Plots[0].Pen.DashStyle, (int)Plots[0].Pen.Width);
                alertLine.Locked = false;
                price            = Instrument.MasterInstrument.Round2TickSize(alertLine.Y);
            }

            double lineVal = showTriggerLine ? Instrument.MasterInstrument.Round2TickSize(alertLine.Y) : price;

            if (showTriggerLine && lineVal != price)
            {
                price      = lineVal;
                triggered  = false;
                triggerSet = false;
            }

            if (alertLine != null)
            {
                Plots[0].Pen.Color		= alertLine.Pen.Color;
                Plots[0].Pen.Width		= alertLine.Pen.Width;
                Plots[0].Pen.DashStyle	= alertLine.Pen.DashStyle;
                alertLine.Y  = lineVal;
            }
            Value.Set(lineVal);

            if (Historical || triggered)
                return;

            if (!triggerSet)
            {
                triggerOnGreaterThan = Input[0] >= price - (TickSize * 0.5) ? false : true;
                triggerSet = true;
            }

            if ((triggerOnGreaterThan && Input[0] >= price - (TickSize * 0.5)) || (!triggerOnGreaterThan && Input[0] <= price + (TickSize * 0.5)))
            {
                triggered = true;
                Alert(DateTime.Now.Millisecond.ToString(), Cbi.Priority.Medium, "Price level '" + price + "' hit!", Cbi.Core.InstallDir + @"\sounds\Alert4.wav", 0, Color.Yellow, Color.Black);
            }
        }

        public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
        {
        }
        #region Properties
        /// <summary>
        /// </summary>
        [Description("The price alert level.")]
        [GridCategory("Parameters")]
        public double Price
        {
            get { return fixedPrice; }
            set { fixedPrice = Math.Max(0, value); }
        }

        [Browsable(true)]
        [Gui.Design.DisplayNameAttribute("Show trigger line")]
        [GridCategory("Parameters")]
        public bool ShowTriggerLine
        {
            get { return showTriggerLine; }
            set { showTriggerLine = value; }
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
        private PriceAlert[] cachePriceAlert = null;

        private static PriceAlert checkPriceAlert = new PriceAlert();

        /// <summary>
        /// Sends an alert to the alert window when a price level has been reached.
        /// </summary>
        /// <returns></returns>
        public PriceAlert PriceAlert_New(double price, bool showTriggerLine)
        {
            return PriceAlert_New(Input, price, showTriggerLine);
        }

        /// <summary>
        /// Sends an alert to the alert window when a price level has been reached.
        /// </summary>
        /// <returns></returns>
        public PriceAlert PriceAlert_New(Data.IDataSeries input, double price, bool showTriggerLine)
        {
            if (cachePriceAlert != null)
                for (int idx = 0; idx < cachePriceAlert.Length; idx++)
                    if (Math.Abs(cachePriceAlert[idx].Price - price) <= double.Epsilon && cachePriceAlert[idx].ShowTriggerLine == showTriggerLine && cachePriceAlert[idx].EqualsInput(input))
                        return cachePriceAlert[idx];

            lock (checkPriceAlert)
            {
                checkPriceAlert.Price = price;
                price = checkPriceAlert.Price;
                checkPriceAlert.ShowTriggerLine = showTriggerLine;
                showTriggerLine = checkPriceAlert.ShowTriggerLine;

                if (cachePriceAlert != null)
                    for (int idx = 0; idx < cachePriceAlert.Length; idx++)
                        if (Math.Abs(cachePriceAlert[idx].Price - price) <= double.Epsilon && cachePriceAlert[idx].ShowTriggerLine == showTriggerLine && cachePriceAlert[idx].EqualsInput(input))
                            return cachePriceAlert[idx];

                PriceAlert indicator = new PriceAlert();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Price = price;
                indicator.ShowTriggerLine = showTriggerLine;
                Indicators.Add(indicator);
                indicator.SetUp();

                PriceAlert[] tmp = new PriceAlert[cachePriceAlert == null ? 1 : cachePriceAlert.Length + 1];
                if (cachePriceAlert != null)
                    cachePriceAlert.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachePriceAlert = tmp;
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
        /// Sends an alert to the alert window when a price level has been reached.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PriceAlert PriceAlert_New(double price, bool showTriggerLine)
        {
            return _indicator.PriceAlert_New(Input, price, showTriggerLine);
        }

        /// <summary>
        /// Sends an alert to the alert window when a price level has been reached.
        /// </summary>
        /// <returns></returns>
        public Indicator.PriceAlert PriceAlert_New(Data.IDataSeries input, double price, bool showTriggerLine)
        {
            return _indicator.PriceAlert_New(input, price, showTriggerLine);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Sends an alert to the alert window when a price level has been reached.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PriceAlert PriceAlert_New(double price, bool showTriggerLine)
        {
            return _indicator.PriceAlert_New(Input, price, showTriggerLine);
        }

        /// <summary>
        /// Sends an alert to the alert window when a price level has been reached.
        /// </summary>
        /// <returns></returns>
        public Indicator.PriceAlert PriceAlert_New(Data.IDataSeries input, double price, bool showTriggerLine)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.PriceAlert_New(input, price, showTriggerLine);
        }
    }
}
#endregion
