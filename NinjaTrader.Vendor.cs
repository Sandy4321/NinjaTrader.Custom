// 
// Copyright (C) 2006, NinjaTrader LLC <ninjatrader@ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private JurikDMX[]				cacheJurikDMX		= null;
        private JurikJMA[]				cacheJurikJMA		= null;
        private JurikRSX[]				cacheJurikRSX		= null;
        private JurikVEL[]				cacheJurikVEL		= null;
        private WoodiesCCI[]			cacheWoodiesCCI		= null;
        private WoodiesPivots[]			cacheWoodiesPivots	= null;

        private static JurikDMX			checkJurikDMX		= new JurikDMX();
        private static JurikJMA			checkJurikJMA		= new JurikJMA();
        private static JurikRSX			checkJurikRSX		= new JurikRSX();
        private static JurikVEL			checkJurikVEL		= new JurikVEL();
        private static WoodiesCCI		checkWoodiesCCI		= new WoodiesCCI();
        private static WoodiesPivots	checkWoodiesPivots	= new WoodiesPivots();

        /// <summary>
        /// Jurik DMX (Directional Movement Index) is a smoother version of the technical indicator DMI, while retaining very fast response speed.
        /// </summary>
        /// <returns></returns>
        public JurikDMX JurikDMX(int smoothing)
        {
            return JurikDMX(Input, smoothing);
        }

        /// <summary>
        /// Jurik DMX (Directional Movement Index) is a smoother version of the technical indicator DMI, while retaining very fast response speed.
        /// </summary>
        /// <returns></returns>
        public JurikDMX JurikDMX(Data.IDataSeries input, int smoothing)
        {
            checkJurikDMX.Smoothing = smoothing;
            smoothing = checkJurikDMX.Smoothing;

            if (cacheJurikDMX != null)
                for (int idx = 0; idx < cacheJurikDMX.Length; idx++)
                    if (cacheJurikDMX[idx].Smoothing == smoothing && cacheJurikDMX[idx].EqualsInput(input))
                        return cacheJurikDMX[idx];

            JurikDMX indicator = new JurikDMX();
            indicator.SetUp();
            indicator.CalculateOnBarClose = CalculateOnBarClose;
            indicator.Input = input;
            indicator.Smoothing = smoothing;

            JurikDMX[] tmp = new JurikDMX[cacheJurikDMX == null ? 1 : cacheJurikDMX.Length + 1];
            if (cacheJurikDMX != null)
                cacheJurikDMX.CopyTo(tmp, 0);
            tmp[tmp.Length - 1] = indicator;
            cacheJurikDMX = tmp;
            Indicators.Add(indicator);

            return indicator;
        }

        /// <summary>
        /// Jurik JMA Adaptive Moving Average is a Moving Average Indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public JurikJMA JurikJMA(double phase, double smoothing)
        {
            return JurikJMA(Input, phase, smoothing);
        }

        /// <summary>
        /// Jurik JMA Adaptive Moving Average is a Moving Average Indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public JurikJMA JurikJMA(Data.IDataSeries input, double phase, double smoothing)
        {
            checkJurikJMA.Phase = phase;
            phase = checkJurikJMA.Phase;
            checkJurikJMA.Smoothing = smoothing;
            smoothing = checkJurikJMA.Smoothing;

            if (cacheJurikJMA != null)
                for (int idx = 0; idx < cacheJurikJMA.Length; idx++)
                    if (Math.Abs(cacheJurikJMA[idx].Phase - phase) <= double.Epsilon && Math.Abs(cacheJurikJMA[idx].Smoothing - smoothing) <= double.Epsilon && cacheJurikJMA[idx].EqualsInput(input))
                        return cacheJurikJMA[idx];

            JurikJMA indicator = new JurikJMA();
            indicator.SetUp();
            indicator.CalculateOnBarClose = CalculateOnBarClose;
            indicator.Input = input;
            indicator.Phase = phase;
            indicator.Smoothing = smoothing;

            JurikJMA[] tmp = new JurikJMA[cacheJurikJMA == null ? 1 : cacheJurikJMA.Length + 1];
            if (cacheJurikJMA != null)
                cacheJurikJMA.CopyTo(tmp, 0);
            tmp[tmp.Length - 1] = indicator;
            cacheJurikJMA = tmp;
            Indicators.Add(indicator);

            return indicator;
        }

        /// <summary>
        /// Jurik RSX (Relative Trend Strength Index) is a smoother version of the technical RSI indicator.
        /// </summary>
        /// <returns></returns>
        public JurikRSX JurikRSX(double smooth)
        {
            return JurikRSX(Input, smooth);
        }

        /// <summary>
        /// Jurik RSX (Relative Trend Strength Index) is a smoother version of the technical RSI indicator.
        /// </summary>
        /// <returns></returns>
        public JurikRSX JurikRSX(Data.IDataSeries input, double smooth)
        {
            checkJurikRSX.Smooth = smooth;
            smooth = checkJurikRSX.Smooth;

            if (cacheJurikRSX != null)
                for (int idx = 0; idx < cacheJurikRSX.Length; idx++)
                    if (Math.Abs(cacheJurikRSX[idx].Smooth - smooth) <= double.Epsilon && cacheJurikRSX[idx].EqualsInput(input))
                        return cacheJurikRSX[idx];

            JurikRSX indicator = new JurikRSX();
            indicator.SetUp();
            indicator.CalculateOnBarClose = CalculateOnBarClose;
            indicator.Input = input;
            indicator.Smooth = smooth;

            JurikRSX[] tmp = new JurikRSX[cacheJurikRSX == null ? 1 : cacheJurikRSX.Length + 1];
            if (cacheJurikRSX != null)
                cacheJurikRSX.CopyTo(tmp, 0);
            tmp[tmp.Length - 1] = indicator;
            cacheJurikRSX = tmp;
            Indicators.Add(indicator);

            return indicator;
        }

        /// <summary>
        /// Jurik VEL (Zero-Lag Velocity) is a smoother version of the technical indicator 'momentum' with the special feature that the smoothing process has added no additional lag to the original momentum indicator.
        /// </summary>
        /// <returns></returns>
        public JurikVEL JurikVEL(int depth)
        {
            return JurikVEL(Input, depth);
        }

        /// <summary>
        /// Jurik VEL (Zero-Lag Velocity) is a smoother version of the technical indicator 'momentum' with the special feature that the smoothing process has added no additional lag to the original momentum indicator.
        /// </summary>
        /// <returns></returns>
        public JurikVEL JurikVEL(Data.IDataSeries input, int depth)
        {
            checkJurikVEL.Depth = depth;
            depth = checkJurikVEL.Depth;

            if (cacheJurikVEL != null)
                for (int idx = 0; idx < cacheJurikVEL.Length; idx++)
                    if (cacheJurikVEL[idx].Depth == depth && cacheJurikVEL[idx].EqualsInput(input))
                        return cacheJurikVEL[idx];

            JurikVEL indicator = new JurikVEL();
            indicator.SetUp();
            indicator.CalculateOnBarClose = CalculateOnBarClose;
            indicator.Input = input;
            indicator.Depth = depth;

            JurikVEL[] tmp = new JurikVEL[cacheJurikVEL == null ? 1 : cacheJurikVEL.Length + 1];
            if (cacheJurikVEL != null)
                cacheJurikVEL.CopyTo(tmp, 0);
            tmp[tmp.Length - 1] = indicator;
            cacheJurikVEL = tmp;
            Indicators.Add(indicator);

            return indicator;
        }


        /// <summary>
        /// Woodies variation of the CCI.
        /// </summary>
        /// <returns></returns>
        public WoodiesCCI WoodiesCCI(int chopIndicatorWidth, int neutralBars, int period, int periodEma, int periodLinReg, int periodTurbo, int sideWinderLimit0, int sideWinderLimit1, int sideWinderWidth)
        {
            return WoodiesCCI(Input, chopIndicatorWidth, neutralBars, period, periodEma, periodLinReg, periodTurbo, sideWinderLimit0, sideWinderLimit1, sideWinderWidth);
        }

        /// <summary>
        /// Woodies variation of the CCI.
        /// </summary>
        /// <returns></returns>
        public WoodiesCCI WoodiesCCI(Data.IDataSeries input, int chopIndicatorWidth, int neutralBars, int period, int periodEma, int periodLinReg, int periodTurbo, int sideWinderLimit0, int sideWinderLimit1, int sideWinderWidth)
        {
            checkWoodiesCCI.ChopIndicatorWidth = chopIndicatorWidth;
            chopIndicatorWidth = checkWoodiesCCI.ChopIndicatorWidth;
            checkWoodiesCCI.NeutralBars = neutralBars;
            neutralBars = checkWoodiesCCI.NeutralBars;
            checkWoodiesCCI.Period = period;
            period = checkWoodiesCCI.Period;
            checkWoodiesCCI.PeriodEma = periodEma;
            periodEma = checkWoodiesCCI.PeriodEma;
            checkWoodiesCCI.PeriodLinReg = periodLinReg;
            periodLinReg = checkWoodiesCCI.PeriodLinReg;
            checkWoodiesCCI.PeriodTurbo = periodTurbo;
            periodTurbo = checkWoodiesCCI.PeriodTurbo;
            checkWoodiesCCI.SideWinderLimit0 = sideWinderLimit0;
            sideWinderLimit0 = checkWoodiesCCI.SideWinderLimit0;
            checkWoodiesCCI.SideWinderLimit1 = sideWinderLimit1;
            sideWinderLimit1 = checkWoodiesCCI.SideWinderLimit1;
            checkWoodiesCCI.SideWinderWidth = sideWinderWidth;
            sideWinderWidth = checkWoodiesCCI.SideWinderWidth;

            if (cacheWoodiesCCI != null)
                for (int idx = 0; idx < cacheWoodiesCCI.Length; idx++)
                    if (cacheWoodiesCCI[idx].ChopIndicatorWidth == chopIndicatorWidth && cacheWoodiesCCI[idx].NeutralBars == neutralBars && cacheWoodiesCCI[idx].Period == period && cacheWoodiesCCI[idx].PeriodEma == periodEma && cacheWoodiesCCI[idx].PeriodLinReg == periodLinReg && cacheWoodiesCCI[idx].PeriodTurbo == periodTurbo && cacheWoodiesCCI[idx].SideWinderLimit0 == sideWinderLimit0 && cacheWoodiesCCI[idx].SideWinderLimit1 == sideWinderLimit1 && cacheWoodiesCCI[idx].SideWinderWidth == sideWinderWidth && cacheWoodiesCCI[idx].EqualsInput(input))
                        return cacheWoodiesCCI[idx];

            WoodiesCCI indicator = new WoodiesCCI();
            indicator.SetUp();
            indicator.CalculateOnBarClose = CalculateOnBarClose;
            indicator.Input = input;
            indicator.ChopIndicatorWidth = chopIndicatorWidth;
            indicator.NeutralBars = neutralBars;
            indicator.Period = period;
            indicator.PeriodEma = periodEma;
            indicator.PeriodLinReg = periodLinReg;
            indicator.PeriodTurbo = periodTurbo;
            indicator.SideWinderLimit0 = sideWinderLimit0;
            indicator.SideWinderLimit1 = sideWinderLimit1;
            indicator.SideWinderWidth = sideWinderWidth;

            WoodiesCCI[] tmp = new WoodiesCCI[cacheWoodiesCCI == null ? 1 : cacheWoodiesCCI.Length + 1];
            if (cacheWoodiesCCI != null)
                cacheWoodiesCCI.CopyTo(tmp, 0);
            tmp[tmp.Length - 1] = indicator;
            cacheWoodiesCCI = tmp;
            Indicators.Add(indicator);

            return indicator;
        }

        /// <summary>
        /// Woodies Pivot Points.
        /// </summary>
        /// <returns></returns>
        public WoodiesPivots WoodiesPivots(Data.HLCCalculationMode priorDayHLC, int width)
        {
            return WoodiesPivots(Input, priorDayHLC, width);
        }

        /// <summary>
        /// Woodies Pivot Points.
        /// </summary>
        /// <returns></returns>
        public WoodiesPivots WoodiesPivots(Data.IDataSeries input, Data.HLCCalculationMode priorDayHLC, int width)
        {
            checkWoodiesPivots.PriorDayHLC = priorDayHLC;
            priorDayHLC = checkWoodiesPivots.PriorDayHLC;
            checkWoodiesPivots.Width = width;
            width = checkWoodiesPivots.Width;

            if (cacheWoodiesPivots != null)
                for (int idx = 0; idx < cacheWoodiesPivots.Length; idx++)
                    if (cacheWoodiesPivots[idx].PriorDayHLC == priorDayHLC && cacheWoodiesPivots[idx].Width == width && cacheWoodiesPivots[idx].EqualsInput(input))
                        return cacheWoodiesPivots[idx];

            WoodiesPivots indicator = new WoodiesPivots();
            indicator.BarsRequired = BarsRequired;
            indicator.CalculateOnBarClose = CalculateOnBarClose;
            indicator.Input = input;
            indicator.PriorDayHLC = priorDayHLC;
            indicator.Width = width;
            indicator.SetUp();

            WoodiesPivots[] tmp = new WoodiesPivots[cacheWoodiesPivots == null ? 1 : cacheWoodiesPivots.Length + 1];
            if (cacheWoodiesPivots != null)
                cacheWoodiesPivots.CopyTo(tmp, 0);
            tmp[tmp.Length - 1] = indicator;
            cacheWoodiesPivots = tmp;
            Indicators.Add(indicator);

            return indicator;
        }
    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// Jurik DMX (Directional Movement Index) is a smoother version of the technical indicator DMI, while retaining very fast response speed.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.JurikDMX JurikDMX(int smoothing)
        {
            return _indicator.JurikDMX(Input, smoothing);
        }

        /// <summary>
        /// Jurik DMX (Directional Movement Index) is a smoother version of the technical indicator DMI, while retaining very fast response speed.
        /// </summary>
        /// <returns></returns>
        public Indicator.JurikDMX JurikDMX(Data.IDataSeries input, int smoothing)
        {
            return _indicator.JurikDMX(input, smoothing);
        }

        /// <summary>
        /// Jurik JMA Adaptive Moving Average is a Moving Average Indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.JurikJMA JurikJMA(double phase, double smoothing)
        {
            return _indicator.JurikJMA(Input, phase, smoothing);
        }

        /// <summary>
        /// Jurik JMA Adaptive Moving Average is a Moving Average Indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public Indicator.JurikJMA JurikJMA(Data.IDataSeries input, double phase, double smoothing)
        {
            return _indicator.JurikJMA(input, phase, smoothing);
        }

        /// <summary>
        /// Jurik RSX (Relative Trend Strength Index) is a smoother version of the technical RSI indicator.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.JurikRSX JurikRSX(double smooth)
        {
            return _indicator.JurikRSX(Input, smooth);
        }

        /// <summary>
        /// Jurik RSX (Relative Trend Strength Index) is a smoother version of the technical RSI indicator.
        /// </summary>
        /// <returns></returns>
        public Indicator.JurikRSX JurikRSX(Data.IDataSeries input, double smooth)
        {
            return _indicator.JurikRSX(input, smooth);
        }

        /// <summary>
        /// Jurik VEL (Zero-Lag Velocity) is a smoother version of the technical indicator 'momentum' with the special feature that the smoothing process has added no additional lag to the original momentum indicator.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.JurikVEL JurikVEL(int depth)
        {
            return _indicator.JurikVEL(Input, depth);
        }

        /// <summary>
        /// Jurik VEL (Zero-Lag Velocity) is a smoother version of the technical indicator 'momentum' with the special feature that the smoothing process has added no additional lag to the original momentum indicator.
        /// </summary>
        /// <returns></returns>
        public Indicator.JurikVEL JurikVEL(Data.IDataSeries input, int depth)
        {
            return _indicator.JurikVEL(input, depth);
        }

        /// <summary>
        /// Woodies variation of the CCI.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.WoodiesCCI WoodiesCCI(int chopIndicatorWidth, int neutralBars, int period, int periodEma, int periodLinReg, int periodTurbo, int sideWinderLimit0, int sideWinderLimit1, int sideWinderWidth)
        {
            return _indicator.WoodiesCCI(Input, chopIndicatorWidth, neutralBars, period, periodEma, periodLinReg, periodTurbo, sideWinderLimit0, sideWinderLimit1, sideWinderWidth);
        }

        /// <summary>
        /// Woodies variation of the CCI.
        /// </summary>
        /// <returns></returns>
        public Indicator.WoodiesCCI WoodiesCCI(Data.IDataSeries input, int chopIndicatorWidth, int neutralBars, int period, int periodEma, int periodLinReg, int periodTurbo, int sideWinderLimit0, int sideWinderLimit1, int sideWinderWidth)
        {
            return _indicator.WoodiesCCI(input, chopIndicatorWidth, neutralBars, period, periodEma, periodLinReg, periodTurbo, sideWinderLimit0, sideWinderLimit1, sideWinderWidth);
        }	

        /// <summary>
        /// Woodies Pivot Points.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.WoodiesPivots WoodiesPivots(Data.HLCCalculationMode priorDayHLC, int width)
        {
            return _indicator.WoodiesPivots(Input, priorDayHLC, width);
        }

        /// <summary>
        /// Woodies Pivot Points.
        /// </summary>
        /// <returns></returns>
        public Indicator.WoodiesPivots WoodiesPivots(Data.IDataSeries input, Data.HLCCalculationMode priorDayHLC, int width)
        {
            return _indicator.WoodiesPivots(input, priorDayHLC, width);
        }
	}
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Jurik DMX (Directional Movement Index) is a smoother version of the technical indicator DMI, while retaining very fast response speed.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.JurikDMX JurikDMX(int smoothing)
        {
            return _indicator.JurikDMX(Input, smoothing);
        }

        /// <summary>
        /// Jurik DMX (Directional Movement Index) is a smoother version of the technical indicator DMI, while retaining very fast response speed.
        /// </summary>
        /// <returns></returns>
        public Indicator.JurikDMX JurikDMX(Data.IDataSeries input, int smoothing)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.JurikDMX(input, smoothing);
        }

        /// <summary>
        /// Jurik JMA Adaptive Moving Average is a Moving Average Indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.JurikJMA JurikJMA(double phase, double smoothing)
        {
            return _indicator.JurikJMA(Input, phase, smoothing);
        }

        /// <summary>
        /// Jurik JMA Adaptive Moving Average is a Moving Average Indicator that shows the average value of a security's price over a period of time.
        /// </summary>
        /// <returns></returns>
        public Indicator.JurikJMA JurikJMA(Data.IDataSeries input, double phase, double smoothing)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.JurikJMA(input, phase, smoothing);
        }

        /// <summary>
        /// Jurik RSX (Relative Trend Strength Index) is a smoother version of the technical RSI indicator.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.JurikRSX JurikRSX(double smooth)
        {
            return _indicator.JurikRSX(Input, smooth);
        }

        /// <summary>
        /// Jurik RSX (Relative Trend Strength Index) is a smoother version of the technical RSI indicator.
        /// </summary>
        /// <returns></returns>
        public Indicator.JurikRSX JurikRSX(Data.IDataSeries input, double smooth)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.JurikRSX(input, smooth);
        }

        /// <summary>
        /// Jurik VEL (Zero-Lag Velocity) is a smoother version of the technical indicator 'momentum' with the special feature that the smoothing process has added no additional lag to the original momentum indicator.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.JurikVEL JurikVEL(int depth)
        {
            return _indicator.JurikVEL(Input, depth);
        }
        /// <summary>
        /// Jurik VEL (Zero-Lag Velocity) is a smoother version of the technical indicator 'momentum' with the special feature that the smoothing process has added no additional lag to the original momentum indicator.
        /// </summary>
        /// <returns></returns>
        public Indicator.JurikVEL JurikVEL(Data.IDataSeries input, int depth)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.JurikVEL(input, depth);
        }

        /// <summary>
        /// Woodies variation of the CCI.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.WoodiesCCI WoodiesCCI(int chopIndicatorWidth, int neutralBars, int period, int periodEma, int periodLinReg, int periodTurbo, int sideWinderLimit0, int sideWinderLimit1, int sideWinderWidth)
        {
            return _indicator.WoodiesCCI(Input, chopIndicatorWidth, neutralBars, period, periodEma, periodLinReg, periodTurbo, sideWinderLimit0, sideWinderLimit1, sideWinderWidth);
        }

        /// <summary>
        /// Woodies variation of the CCI.
        /// </summary>
        /// <returns></returns>
        public Indicator.WoodiesCCI WoodiesCCI(Data.IDataSeries input, int chopIndicatorWidth, int neutralBars, int period, int periodEma, int periodLinReg, int periodTurbo, int sideWinderLimit0, int sideWinderLimit1, int sideWinderWidth)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.WoodiesCCI(input, chopIndicatorWidth, neutralBars, period, periodEma, periodLinReg, periodTurbo, sideWinderLimit0, sideWinderLimit1, sideWinderWidth);
        }

        /// <summary>
        /// Woodies Pivot Points.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.WoodiesPivots WoodiesPivots(Data.HLCCalculationMode priorDayHLC, int width)
        {
            return _indicator.WoodiesPivots(Input, priorDayHLC, width);
        }

        /// <summary>
        /// Woodies Pivot Points.
        /// </summary>
        /// <returns></returns>
        public Indicator.WoodiesPivots WoodiesPivots(Data.IDataSeries input, Data.HLCCalculationMode priorDayHLC, int width)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.WoodiesPivots(input, priorDayHLC, width);
        }
	}
}



