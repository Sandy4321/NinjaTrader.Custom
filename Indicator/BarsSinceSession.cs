#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Enter the description of your new custom indicator here
    /// </summary>
    [Description( "Enter the description of your new custom indicator here" )]
    public class BarsSinceSession : Indicator
    {
        #region Variables
        int barcount = 1;
        int _skipSize = 1;



        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
           
            Overlay                      = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
               if (BarsArray[0].FirstBarOfSession)
                     {
                           barcount = 1;
                     }
                      double price;
                      if (barcount % 2 == 0)
                     {
                           price = High[0] + TickSize * 2;
                     }
                      else {price = Low[0] - TickSize * 2;}

            if (barcount % SkipSize == 0)
            {
                base.DrawText("txt" + CurrentBar, barcount.ToString(), 0, price, Color.Gray);
            }
              barcount += 1;
              }

        #region Properties
        [ Description("Number of bars to skip" )]
        [ GridCategory("Parameters" )]
        public int SkipSize
        {
            get { return _skipSize; }
            set { _skipSize = value ; }
        }
        #endregion
        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private BarsSinceSession[] cacheBarsSinceSession = null;

        private static BarsSinceSession checkBarsSinceSession = new BarsSinceSession();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public BarsSinceSession BarsSinceSession()
        {
            return BarsSinceSession(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public BarsSinceSession BarsSinceSession(Data.IDataSeries input)
        {
            if (cacheBarsSinceSession != null)
                for (int idx = 0; idx < cacheBarsSinceSession.Length; idx++)
                    if (cacheBarsSinceSession[idx].EqualsInput(input))
                        return cacheBarsSinceSession[idx];

            lock (checkBarsSinceSession)
            {
                if (cacheBarsSinceSession != null)
                    for (int idx = 0; idx < cacheBarsSinceSession.Length; idx++)
                        if (cacheBarsSinceSession[idx].EqualsInput(input))
                            return cacheBarsSinceSession[idx];

                BarsSinceSession indicator = new BarsSinceSession();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                BarsSinceSession[] tmp = new BarsSinceSession[cacheBarsSinceSession == null ? 1 : cacheBarsSinceSession.Length + 1];
                if (cacheBarsSinceSession != null)
                    cacheBarsSinceSession.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheBarsSinceSession = tmp;
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
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BarsSinceSession BarsSinceSession()
        {
            return _indicator.BarsSinceSession(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.BarsSinceSession BarsSinceSession(Data.IDataSeries input)
        {
            return _indicator.BarsSinceSession(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BarsSinceSession BarsSinceSession()
        {
            return _indicator.BarsSinceSession(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.BarsSinceSession BarsSinceSession(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.BarsSinceSession(input);
        }
    }
}
#endregion
