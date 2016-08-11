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
using System.Collections.Generic;
using MySql.Data.MySqlClient;

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Enter the description of your new custom indicator here
    /// </summary>
    [Description("Enter the description of your new custom indicator here")]
    public class kTickAnalysis : Indicator
    {
        #region Variables
        private MySqlConnection dbConn;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        /// 
        protected override void OnStartUp()
        {
            dbConn = new MySqlConnection("server=localhost;database=algo;uid=root;password=Password1;");
            try
            {
                dbConn.Open();
                Print("Database connection established successfully...");
            }
            catch (MySqlException ex)
            {
                Print(ex.ToString());
            }
        }
        protected override void OnBarUpdate()
        {
            if (Bars.BarsSinceSession == 1)
            {
                Print(Time[0]);
            }

        }

        #region Properties

        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private kTickAnalysis[] cachekTickAnalysis = null;

        private static kTickAnalysis checkkTickAnalysis = new kTickAnalysis();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public kTickAnalysis kTickAnalysis()
        {
            return kTickAnalysis(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public kTickAnalysis kTickAnalysis(Data.IDataSeries input)
        {
            if (cachekTickAnalysis != null)
                for (int idx = 0; idx < cachekTickAnalysis.Length; idx++)
                    if (cachekTickAnalysis[idx].EqualsInput(input))
                        return cachekTickAnalysis[idx];

            lock (checkkTickAnalysis)
            {
                if (cachekTickAnalysis != null)
                    for (int idx = 0; idx < cachekTickAnalysis.Length; idx++)
                        if (cachekTickAnalysis[idx].EqualsInput(input))
                            return cachekTickAnalysis[idx];

                kTickAnalysis indicator = new kTickAnalysis();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                kTickAnalysis[] tmp = new kTickAnalysis[cachekTickAnalysis == null ? 1 : cachekTickAnalysis.Length + 1];
                if (cachekTickAnalysis != null)
                    cachekTickAnalysis.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachekTickAnalysis = tmp;
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
        public Indicator.kTickAnalysis kTickAnalysis()
        {
            return _indicator.kTickAnalysis(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.kTickAnalysis kTickAnalysis(Data.IDataSeries input)
        {
            return _indicator.kTickAnalysis(input);
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
        public Indicator.kTickAnalysis kTickAnalysis()
        {
            return _indicator.kTickAnalysis(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.kTickAnalysis kTickAnalysis(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.kTickAnalysis(input);
        }
    }
}
#endregion
