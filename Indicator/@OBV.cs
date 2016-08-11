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
	/// OBV (On Balance Volume) is a running total of volume. It shows if volume is flowing into or out of a security. When the security closes higher than the previous close, all of the day's volume is considered up-volume. When the security closes lower than the previous close, all of the day's volume is considered down-volume.
	/// </summary>
	[Description("OBV (On Balance Volume) is a running total of volume. It shows if volume is flowing into or out of a security. When the security closes higher than the previous close, all of the day's volume is considered up-volume. When the security closes lower than the previous close, all of the day's volume is considered down-volume.")]
	public class OBV : Indicator
	{
		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Orange, "OBV"));
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
				Value.Set(0);
			else
			{
				if (Close[0] > Close[1])
					Value.Set(Value[1]+ Volume[0]);
				else if (Close[0]  < Close[1])
					Value.Set(Value[1] - Volume[0]);
				else
					Value.Set(Value[1]);
			}
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private OBV[] cacheOBV = null;

        private static OBV checkOBV = new OBV();

        /// <summary>
        /// OBV (On Balance Volume) is a running total of volume. It shows if volume is flowing into or out of a security. When the security closes higher than the previous close, all of the day's volume is considered up-volume. When the security closes lower than the previous close, all of the day's volume is considered down-volume.
        /// </summary>
        /// <returns></returns>
        public OBV OBV()
        {
            return OBV(Input);
        }

        /// <summary>
        /// OBV (On Balance Volume) is a running total of volume. It shows if volume is flowing into or out of a security. When the security closes higher than the previous close, all of the day's volume is considered up-volume. When the security closes lower than the previous close, all of the day's volume is considered down-volume.
        /// </summary>
        /// <returns></returns>
        public OBV OBV(Data.IDataSeries input)
        {
            if (cacheOBV != null)
                for (int idx = 0; idx < cacheOBV.Length; idx++)
                    if (cacheOBV[idx].EqualsInput(input))
                        return cacheOBV[idx];

            lock (checkOBV)
            {
                if (cacheOBV != null)
                    for (int idx = 0; idx < cacheOBV.Length; idx++)
                        if (cacheOBV[idx].EqualsInput(input))
                            return cacheOBV[idx];

                OBV indicator = new OBV();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                OBV[] tmp = new OBV[cacheOBV == null ? 1 : cacheOBV.Length + 1];
                if (cacheOBV != null)
                    cacheOBV.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheOBV = tmp;
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
        /// OBV (On Balance Volume) is a running total of volume. It shows if volume is flowing into or out of a security. When the security closes higher than the previous close, all of the day's volume is considered up-volume. When the security closes lower than the previous close, all of the day's volume is considered down-volume.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.OBV OBV()
        {
            return _indicator.OBV(Input);
        }

        /// <summary>
        /// OBV (On Balance Volume) is a running total of volume. It shows if volume is flowing into or out of a security. When the security closes higher than the previous close, all of the day's volume is considered up-volume. When the security closes lower than the previous close, all of the day's volume is considered down-volume.
        /// </summary>
        /// <returns></returns>
        public Indicator.OBV OBV(Data.IDataSeries input)
        {
            return _indicator.OBV(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// OBV (On Balance Volume) is a running total of volume. It shows if volume is flowing into or out of a security. When the security closes higher than the previous close, all of the day's volume is considered up-volume. When the security closes lower than the previous close, all of the day's volume is considered down-volume.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.OBV OBV()
        {
            return _indicator.OBV(Input);
        }

        /// <summary>
        /// OBV (On Balance Volume) is a running total of volume. It shows if volume is flowing into or out of a security. When the security closes higher than the previous close, all of the day's volume is considered up-volume. When the security closes lower than the previous close, all of the day's volume is considered down-volume.
        /// </summary>
        /// <returns></returns>
        public Indicator.OBV OBV(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.OBV(input);
        }
    }
}
#endregion
