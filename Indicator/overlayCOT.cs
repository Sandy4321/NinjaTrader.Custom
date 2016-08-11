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
    public class overlayCOT : Indicator
    {
        #region Variables
        Dictionary<string, double> vixDataSet = new Dictionary<string, double>();
        private MySqlConnection dbConn = new MySqlConnection("server = localhost; database=algo;uid=root;password=Password1;");
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "Plot0"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "Plot1"));
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkViolet), PlotStyle.Line, "Plot3"));
            Overlay				= false;
        }

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
                        vixDataSet.Add(reader.GetString(0), reader.GetDouble(1));
                    }
                }
            }
            dbConn.Close();
        }
    

        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
            Plot0.Set(Close[0]);
            Plot1.Set(Close[0]);
            Plot3.Set(Close[0]);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot0
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot1
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot3
        {
            get { return Values[2]; }
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
        private overlayCOT[] cacheoverlayCOT = null;

        private static overlayCOT checkoverlayCOT = new overlayCOT();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public overlayCOT overlayCOT()
        {
            return overlayCOT(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public overlayCOT overlayCOT(Data.IDataSeries input)
        {
            if (cacheoverlayCOT != null)
                for (int idx = 0; idx < cacheoverlayCOT.Length; idx++)
                    if (cacheoverlayCOT[idx].EqualsInput(input))
                        return cacheoverlayCOT[idx];

            lock (checkoverlayCOT)
            {
                if (cacheoverlayCOT != null)
                    for (int idx = 0; idx < cacheoverlayCOT.Length; idx++)
                        if (cacheoverlayCOT[idx].EqualsInput(input))
                            return cacheoverlayCOT[idx];

                overlayCOT indicator = new overlayCOT();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                overlayCOT[] tmp = new overlayCOT[cacheoverlayCOT == null ? 1 : cacheoverlayCOT.Length + 1];
                if (cacheoverlayCOT != null)
                    cacheoverlayCOT.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheoverlayCOT = tmp;
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
        public Indicator.overlayCOT overlayCOT()
        {
            return _indicator.overlayCOT(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.overlayCOT overlayCOT(Data.IDataSeries input)
        {
            return _indicator.overlayCOT(input);
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
        public Indicator.overlayCOT overlayCOT()
        {
            return _indicator.overlayCOT(Input);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.overlayCOT overlayCOT(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.overlayCOT(input);
        }
    }
}
#endregion
