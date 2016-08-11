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
    public class overlayVIX : Indicator
    {
        #region Variables
        Dictionary<string,double> vixDataSet = new Dictionary<string, double>();
        private MySqlConnection dbConn = new MySqlConnection("server = localhost; database=algo;uid=root;password=Password1;");
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.SlateGray), PlotStyle.Line, "Plot0"));
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnStartUp()
        {
            dbConn.Open();
            string query = "select datenum, close from yahoodailydata where symbol = '^VIX' order by datenum asc";
            using (MySqlCommand command = new MySqlCommand(query, dbConn))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        vixDataSet.Add(reader.GetString(0),reader.GetDouble(1));
                    }
                }
            }
            dbConn.Close();
        }
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
            if (vixDataSet.ContainsKey(Time[0].ToShortDateString()))
            {
                //Print(Time[0].ToString() + "    " + vixDataSet[Time[0].ToShortDateString()]);
                //double close = vixDataSet[Time[0].ToShortDateString()];
                Plot0.Set(vixDataSet[Time[0].ToShortDateString()]);
            }

        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot0
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
        private overlayVIX[] cacheoverlayVIX = null;

        private static overlayVIX checkoverlayVIX = new overlayVIX();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public overlayVIX overlayVIX()
        {
            return overlayVIX(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public overlayVIX overlayVIX(Data.IDataSeries input)
        {
            if (cacheoverlayVIX != null)
                for (int idx = 0; idx < cacheoverlayVIX.Length; idx++)
                    if (cacheoverlayVIX[idx].EqualsInput(input))
                        return cacheoverlayVIX[idx];

            lock (checkoverlayVIX)
            {
                if (cacheoverlayVIX != null)
                    for (int idx = 0; idx < cacheoverlayVIX.Length; idx++)
                        if (cacheoverlayVIX[idx].EqualsInput(input))
                            return cacheoverlayVIX[idx];

                overlayVIX indicator = new overlayVIX();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                overlayVIX[] tmp = new overlayVIX[cacheoverlayVIX == null ? 1 : cacheoverlayVIX.Length + 1];
                if (cacheoverlayVIX != null)
                    cacheoverlayVIX.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheoverlayVIX = tmp;
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
        public Indicator.overlayVIX overlayVIX()
        {
            return _indicator.overlayVIX(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.overlayVIX overlayVIX(Data.IDataSeries input)
        {
            return _indicator.overlayVIX(input);
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
        public Indicator.overlayVIX overlayVIX()
        {
            return _indicator.overlayVIX(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.overlayVIX overlayVIX(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.overlayVIX(input);
        }
    }
}
#endregion
