#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// RSI with a Stop Loss and Profit Target
    /// </summary>
    [Description("RSI with a Stop Loss and Profit Target")]
    public class Tutorial2 : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int rSIPeriod = 14; // Default setting for RSIPeriod
        private int rISSmooth = 3; // Default setting for RISSmooth
        private int profitTarget = 12; // Default setting for ProfitTarget
        private int stopLoss = 6; // Default setting for StopLoss
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = true;

            Add(RSI(rSIPeriod, rISSmooth));
            SetStopLoss(CalculationMode.Ticks, stopLoss);
            SetProfitTarget(CalculationMode.Ticks, profitTarget);

        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {

            if (CurrentBar < rSIPeriod) return;

            if (CrossAbove(RSI(rSIPeriod, rISSmooth), 20, 1))
            {
                EnterLong();
            }

        }

        #region Properties
        [Description("RSI Period")]
        [GridCategory("Parameters")]
        public int RSIPeriod
        {
            get { return rSIPeriod; }
            set { rSIPeriod = Math.Max(1, value); }
        }

        [Description("RSI Smooth")]
        [GridCategory("Parameters")]
        public int RISSmooth
        {
            get { return rISSmooth; }
            set { rISSmooth = Math.Max(1, value); }
        }

        [Description("Profit Target Offset")]
        [GridCategory("Parameters")]
        public int ProfitTarget
        {
            get { return profitTarget; }
            set { profitTarget = Math.Max(1, value); }
        }

        [Description("Stop Loss Offset")]
        [GridCategory("Parameters")]
        public int StopLoss
        {
            get { return stopLoss; }
            set { stopLoss = Math.Max(1, value); }
        }
        #endregion
    }
}
