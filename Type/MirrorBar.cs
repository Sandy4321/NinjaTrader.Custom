#region Using declarations
using System;
using System.ComponentModel;
#endregion	
namespace NinjaTrader.Data
{
    /// <summary>
    /// </summary>
    public class MirrorBarsType : BarsType
    {
		private			double		anchorPrice			= double.MinValue;
		private			double		baseSize			= double.MinValue;
		private			DateTime	cacheSessionEnd		= Cbi.Globals.MinDate;
		private			bool		endOfBar			= false;
		private			DateTime	prevTime			= Cbi.Globals.MinDate;
		private			DateTime	prevTimeD			= Cbi.Globals.MinDate;
		private static	bool		registered			= Register(new MirrorBarsType());
		private			double		reversalSize		= double.MinValue;
		private			int			tmpCount			= 0;
		private			int			tmpDayCount			= 0;
		private			double		tmpHigh				= double.MinValue;
		private			double		tmpLow				= double.MinValue;
		private			int			tmpTickCount		= 0;
		private			DateTime	tmpTime				= Cbi.Globals.MinDate;
		private			long		tmpVolume			= 0;
		private			long		volumeCount			= 0;

		/// <summary>
		/// </summary>
		/// <param name="bars"></param>
		/// <param name="open"></param>
		/// <param name="high"></param>
		/// <param name="low"></param>
		/// <param name="close"></param>
		/// <param name="time"></param>
		/// <param name="volume"></param>
		/// <param name="isRealtime"></param>
		public override void Add(Bars bars, double open, double high, double low, double close, DateTime time, long volume, bool isRealtime)
		{
			#region Building Bars from Base Period

			if (bars.Count != tmpCount) // reset cache when bars are trimmed
				if (bars.Count == 0)
				{
					tmpTime			= Cbi.Globals.MinDate;
					tmpVolume		= 0;
					tmpDayCount		= 0;
					tmpTickCount	= 0;
				}
				else
				{
					tmpTime			= bars.GetTime(bars.Count - 1);
					tmpVolume		= bars.GetVolume(bars.Count - 1);
					tmpTickCount	= bars.TickCount;
					tmpDayCount		= bars.DayCount;
					bars.LastPrice	= anchorPrice = bars.GetClose(bars.Count - 1);
				}

			switch (bars.Period.BasePeriodType)
			{
				case PeriodType.Day:
                    if (bars.Count == 0)
                        if (isRealtime && bars.Session.SessionsOfDay.Length > 0)
                        {
                            DateTime barTime;
                            bars.Session.GetSessionDate(time, false, out barTime, out cacheSessionEnd);
                            AddBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, barTime, volume, true);
                        }
                        else
                            AddBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, time.Date, volume, isRealtime);
                    else
                    {
                        DateTime barTime;
                        if (!isRealtime)
                            barTime = time.Date;
                        else if (time >= cacheSessionEnd /* on realtime include60 is always false */)
                        {
                            bars.Session.GetSessionDate(time, false, out barTime, out cacheSessionEnd);
                            if (barTime < bars.TimeLastBar.Date)
                                barTime = bars.TimeLastBar.Date; // make sure timestamps are ascending
                        }
                        else
                            barTime = bars.TimeLastBar.Date; // make sure timestamps are ascending

                        if (bars.DayCount < bars.Period.Value
                                || (!isRealtime && bars.Count > 0 && barTime == bars.TimeLastBar.Date)
                                || (isRealtime && bars.Count > 0 && barTime <= bars.TimeLastBar.Date))
                            UpdateBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, barTime, volume, isRealtime);
                        else
                            AddBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, barTime, volume, isRealtime);
                    }

					break;

				case PeriodType.Minute:
                    if (bars.Count == 0)
                        AddBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, TimeToBarTimeMinute(bars, time, bars.Session.NextBeginTime, bars.Period.Value, isRealtime), volume, isRealtime);
                    else
                    {
                        if (isRealtime && time < bars.TimeLastBar)
                            UpdateBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, bars.TimeLastBar, volume, true);
                        else if (!isRealtime && time <= bars.TimeLastBar)
                            UpdateBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, bars.TimeLastBar, volume, false);
                        else
                        {
                            time = TimeToBarTimeMinute(bars, time, bars.Session.NextBeginTime, bars.Period.Value, isRealtime);
                            AddBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, time, volume, isRealtime);
                        }
                    }
					break;

				case PeriodType.Volume:
                    if (bars.Count == 0)
                    {
                        while (volume > bars.Period.Value)
                        {
                            AddBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, time, bars.Period.Value, isRealtime);
                            volume -= bars.Period.Value;
                        }

                        if (volume > 0)
                            AddBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, time, volume, isRealtime);
                    }
                    else
                    {
                        long volumeTmp = 0;
                        if (!bars.IsNewSession(time, isRealtime))
                        {
                            volumeTmp = Math.Min(bars.Period.Value - bars.GetVolume(bars.Count - 1), volume);
                            if (volumeTmp > 0)
                                UpdateBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, time, volumeTmp, isRealtime);
                        }

                        volumeTmp = volume - volumeTmp;
                        while (volumeTmp > 0)
                        {
                            AddBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, time, Math.Min(volumeTmp, bars.Period.Value), isRealtime);
                            volumeTmp -= bars.Period.Value;
                        }
                    }
					break;

				case PeriodType.Month:
                    if (bars.Count == 0)
                        AddBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, TimeToBarTimeMonth(time, bars.Period.Value), volume, isRealtime);
                    else
                    {
                        if ((time.Month <= bars.TimeLastBar.Month && time.Year == bars.TimeLastBar.Year) || time.Year < bars.TimeLastBar.Year)
                            UpdateBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, bars.TimeLastBar, volume, isRealtime);
                        else
                            AddBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, TimeToBarTimeMonth(time, bars.Period.Value), volume, isRealtime);
                    }
					break;

				case PeriodType.Second:
                    if (bars.Count == 0)
                        AddBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, TimeToBarTimeSecond(bars, time, new DateTime(bars.Session.NextBeginTime.Year, bars.Session.NextBeginTime.Month, bars.Session.NextBeginTime.Day, bars.Session.NextBeginTime.Hour, bars.Session.NextBeginTime.Minute, 0), bars.Period.Value), volume, isRealtime);
                    else
                    {
                        if ((bars.Period.Value > 1 && time < bars.TimeLastBar)
                            || (bars.Period.Value == 1 && time <= bars.TimeLastBar))
                        {
                            UpdateBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, bars.TimeLastBar, volume, isRealtime);
                        }
                        else
                        {
                            time = TimeToBarTimeSecond(bars, time, bars.Session.NextBeginTime, bars.Period.Value);
                            AddBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, time, volume, isRealtime);
                        }
                    }
					break;

				case PeriodType.Tick:
                    if (bars.Count == 0)
                        AddBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, time, volume, isRealtime);
                    else
                    {
                        if (bars.Count > 0 && !bars.IsNewSession(time, isRealtime) && bars.Period.Value > 1 && bars.TickCount < bars.Period.Value)
                            UpdateBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, time, volume, isRealtime);
                        else
                            AddBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, time, volume, isRealtime);
                    }
					break;

				case PeriodType.Week:
                    if (bars.Count == 0)
                        AddBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, TimeToBarTimeWeek(time, time.AddDays((6 - (((int)time.DayOfWeek + 1) % 7)) + ((bars.Period.Value - 1) * 7)), bars.Period.Value), volume, isRealtime);
                    else if (time.Date <= bars.TimeLastBar.Date)
                        UpdateBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, bars.TimeLastBar, volume, isRealtime);
                    else
                        AddBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, TimeToBarTimeWeek(time.Date, bars.TimeLastBar.Date, bars.Period.Value), volume, isRealtime);
					break;

				case PeriodType.Year:
                    if (bars.Count == 0)
                        AddBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, TimeToBarTimeYear(time, bars.Period.Value), volume, isRealtime);
                    else
                    {
                        if (time.Year <= bars.TimeLastBar.Year)
                            UpdateBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, bars.TimeLastBar, volume, isRealtime);
                        else
                            AddBar(bars, 0 - open, 0 - low, 0 - high, 0 - close, TimeToBarTimeYear(time.Date, bars.Period.Value), volume, isRealtime);
                    }
					break;
				default:
					break;
			}
			#endregion
		}

		/// <summary>
		/// </summary>
		/// <param name="barsData"></param>
		public override void ApplyDefaults(Gui.Chart.BarsData barsData)
		{
			switch (barsData.Period.BasePeriodType)
			{
				case PeriodType.Day		: barsData.Period.BasePeriodValue = 1;		barsData.DaysBack = 365;	break;
				case PeriodType.Minute	: barsData.Period.BasePeriodValue = 1;		barsData.DaysBack = 5;		break;
				case PeriodType.Month	: barsData.Period.BasePeriodValue = 1;		barsData.DaysBack = 5475;	break;
				case PeriodType.Second	: barsData.Period.BasePeriodValue = 30;		barsData.DaysBack = 3;		break;
				case PeriodType.Tick	: barsData.Period.BasePeriodValue = 150;	barsData.DaysBack = 3;		break;
				case PeriodType.Volume	: barsData.Period.BasePeriodValue = 1000;	barsData.DaysBack = 3;		break;
				case PeriodType.Week	: barsData.Period.BasePeriodValue = 1;		barsData.DaysBack = 1825;	break;
				case PeriodType.Year	: barsData.Period.BasePeriodValue = 1;		barsData.DaysBack = 15000;	break;
				default: break;
			}
		}

		/// <summary>
		/// </summary>
		public override PeriodType BuiltFrom
		{
			get
			{
				switch (Period.BasePeriodType)
				{
					case PeriodType.Tick	:
					case PeriodType.Second	:
					case PeriodType.Volume	: return PeriodType.Tick;
					case PeriodType.Minute	: return PeriodType.Minute;
					default					: return PeriodType.Day;
				}
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartDataBoxDate(DateTime time)
		{
			switch (Period.BasePeriodType)
			{
				case PeriodType.Week	: return string.Format("{0}/{1}", Gui.Globals.GetCalendarWeek(time), time.Year);
				case PeriodType.Month	: return string.Format("{0}/{1}", time.Month, time.Year);
				case PeriodType.Year	: return time.Year.ToString();
				default					: return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartLabel(Gui.Chart.ChartControl chartControl, DateTime time)
		{
			switch (Period.BasePeriodType)
			{
				case PeriodType.Day		: return time.ToString(chartControl.LabelFormatDay,		Cbi.Globals.CurrentCulture);
				case PeriodType.Minute	: return time.ToString(chartControl.LabelFormatMinute,	Cbi.Globals.CurrentCulture);
				case PeriodType.Month	: return time.ToString(chartControl.LabelFormatMonth,	Cbi.Globals.CurrentCulture);
				case PeriodType.Second	: return time.ToString(chartControl.LabelFormatSecond,	Cbi.Globals.CurrentCulture);
				case PeriodType.Tick	: return time.ToString(chartControl.LabelFormatTick,	Cbi.Globals.CurrentCulture);
				case PeriodType.Volume	: return time.ToString(chartControl.LabelFormatTick,	Cbi.Globals.CurrentCulture);
				case PeriodType.Week	: return time.ToString(chartControl.LabelFormatDay,		Cbi.Globals.CurrentCulture);
				case PeriodType.Year	: return time.ToString(chartControl.LabelFormatYear,	Cbi.Globals.CurrentCulture);
				default					: return time.ToString(chartControl.LabelFormatDay,		Cbi.Globals.CurrentCulture);
			}
		}

		/// <summary>
		/// Here is how you restrict the selectable chart styles by bars type
		/// </summary>
		public override Gui.Chart.ChartStyleType[] ChartStyleTypesSupported
		{
            get
            {
                return new[] { Gui.Chart.ChartStyleType.Box, Gui.Chart.ChartStyleType.CandleStick, Gui.Chart.ChartStyleType.HiLoBars, Gui.Chart.ChartStyleType.LineOnClose, 
				Gui.Chart.ChartStyleType.OHLC, Gui.Chart.ChartStyleType.Custom0, Gui.Chart.ChartStyleType.Custom1, Gui.Chart.ChartStyleType.Custom2, Gui.Chart.ChartStyleType.Custom3,
				Gui.Chart.ChartStyleType.Custom4, Gui.Chart.ChartStyleType.Custom5, Gui.Chart.ChartStyleType.Custom6, Gui.Chart.ChartStyleType.Custom7, Gui.Chart.ChartStyleType.Custom8,
				Gui.Chart.ChartStyleType.Custom9, Gui.Chart.ChartStyleType.Final0, Gui.Chart.ChartStyleType.Final1, Gui.Chart.ChartStyleType.Final2, Gui.Chart.ChartStyleType.Final3,
				Gui.Chart.ChartStyleType.Final4 };
            }
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new MirrorBarsType();
		}

		/// <summary>
		/// </summary>
		public MirrorBarsType() : base(PeriodType.Custom0) { }

		/// <summary>
		/// </summary>
		public override int DefaultValue
		{
			get
			{
				switch (Period.BasePeriodType)
				{
					case PeriodType.Second	: return 30;
					case PeriodType.Tick	: return 150;
					case PeriodType.Volume	: return 1000;
					default					: return 1;
				}
			}
		}

		/// <summary>
		/// </summary>
		public override string DisplayName
		{
			get { return "MirrorBars"; }
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <param name="barsBack"></param>
		/// <returns></returns>
		public override int GetInitialLookBackDays(Period period, int barsBack)
		{
			switch (Period.BasePeriodType)
			{
				case PeriodType.Day		: return new DayBarsType()		.GetInitialLookBackDays(period, barsBack);
				case PeriodType.Minute	: return new MinuteBarsType()	.GetInitialLookBackDays(period, barsBack);
				case PeriodType.Month	: return new MonthBarsType()	.GetInitialLookBackDays(period, barsBack);
				case PeriodType.Second	: return new SecondBarsType()	.GetInitialLookBackDays(period, barsBack);
				case PeriodType.Tick	: return new TickBarsType()		.GetInitialLookBackDays(period, barsBack);
				case PeriodType.Volume	: return new VolumeBarsType()	.GetInitialLookBackDays(period, barsBack);
				case PeriodType.Week	: return new WeekBarsType()		.GetInitialLookBackDays(period, barsBack);
				case PeriodType.Year	: return new YearBarsType()		.GetInitialLookBackDays(period, barsBack);
				default					: return new MinuteBarsType()	.GetInitialLookBackDays(period, barsBack);
			}
		}

		/// <summary>
		/// </summary>
		public override double GetPercentComplete(Bars bars, DateTime now)
		{
			throw new ApplicationException(string.Format("GetPercentComplete not supported in {0}", DisplayName));
		}

		/// <summary>
		/// </summary>
		/// <param name="propertyDescriptor"></param>
		/// <param name="period"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public override PropertyDescriptorCollection GetProperties(PropertyDescriptor propertyDescriptor, Period period, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = base.GetProperties(propertyDescriptor, period, attributes);

			// here is how you remove properties not needed for that particular bars type
			properties.Remove(properties.Find("ReversalType", true));
            properties.Remove(properties.Find("BasePeriodValue", true));
            properties.Remove(properties.Find("Value2", true));
           

			// here is how you change the display name of the property on the properties grid
            Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Value", "\r\rBase Period Value");
           
          
			

			return properties;
		}

		/// <summary>
		/// </summary>
		public override bool IsIntraday
		{
			get
			{
				switch (Period.BasePeriodType)
				{
					case PeriodType.Minute	:
					case PeriodType.Second	:
					case PeriodType.Tick	:
					case PeriodType.Volume	: return true;
					default					: return false;
				}
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <returns></returns>
		public override string ToString(Period period)
		{
			switch (Period.BasePeriodType)
			{
				case PeriodType.Day		: return string.Format("{0} {1} Mirror{2}", period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Daily" : "Day"), (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Minute	: return string.Format("{0} Min Mirror{1}", period.BasePeriodValue, (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Month	: return string.Format("{0} {1} Mirror{2}", period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Monthly" : "Month"), (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Second	: return string.Format("{0} {1} Mirror{2}", period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Second" : "Seconds"), (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Tick	: return string.Format("{0} Tick Mirror{1}", period.BasePeriodValue, (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Volume	: return string.Format("{0} Volume Mirror{1}", period.BasePeriodValue, (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Week	: return string.Format("{0} {1} Mirror{2}", period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Weekly" : "Weeks"), (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Year	: return string.Format("{0} {1} Mirror{2}", period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Yearly" : "Years"), (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				default					: return string.Format("{0} {1} Mirror{2}", period.BasePeriodValue, BuiltFrom, (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
			}
		}

		private static DateTime TimeToBarTimeMinute(Bars bars, DateTime time, DateTime periodStart, int periodValue, bool isRealtime)
		{
			DateTime barTimeStamp = isRealtime
										? periodStart.AddMinutes(periodValue + Math.Floor(Math.Floor(Math.Max(0, time.Subtract(periodStart).TotalMinutes)) / periodValue) * periodValue)
										: periodStart.AddMinutes(Math.Ceiling(Math.Ceiling(Math.Max(0, time.Subtract(periodStart).TotalMinutes)) / periodValue) * periodValue);
			if (bars.Session.SessionsOfDay.Length > 0 && barTimeStamp > bars.Session.NextEndTime)
				barTimeStamp = (bars.Session.NextEndTime <= Cbi.Globals.MinDate ? barTimeStamp : bars.Session.NextEndTime);
			return barTimeStamp;
		}

		private static DateTime TimeToBarTimeMonth(DateTime time, int periodValue)
		{
			DateTime result = new DateTime(time.Year, time.Month, 1);
			for (int i = 0; i < periodValue; i++)
				result = result.AddMonths(1);

			return result.AddDays(-1);
		}

		private static DateTime TimeToBarTimeSecond(Bars bars, DateTime time, DateTime periodStart, int periodValue)
		{
			DateTime barTimeStamp = periodStart.AddSeconds(Math.Ceiling(Math.Ceiling(Math.Max(0, time.Subtract(periodStart).TotalSeconds)) / periodValue) * periodValue);
			if (bars.Session.SessionsOfDay.Length > 0 && barTimeStamp > bars.Session.NextEndTime)
				barTimeStamp = (bars.Session.NextEndTime <= Cbi.Globals.MinDate ? barTimeStamp : bars.Session.NextEndTime);
			return barTimeStamp;
		}

		private static DateTime TimeToBarTimeWeek(DateTime time, DateTime periodStart, int periodValue)
		{
			return periodStart.Date.AddDays(Math.Ceiling(Math.Ceiling(time.Date.Subtract(periodStart.Date).TotalDays) / (periodValue * 7)) * (periodValue * 7)).Date;
		}

		private static DateTime TimeToBarTimeYear(DateTime time, int periodValue)
		{
			DateTime result = new DateTime(time.Year, 1, 1);
			for (int i = 0; i < periodValue; i++)
				result = result.AddYears(1);

			return result.AddDays(-1);
		}
    }
}