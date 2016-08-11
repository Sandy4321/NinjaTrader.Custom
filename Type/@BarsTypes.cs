// 
// Copyright (C) 2006, NinjaTrader LLC <ninjatrader@ninjatrader.com>.
//
#region Using declarations
using System;
using System.ComponentModel;
#endregion

// This namespace holds all bars types. Do not change it.
namespace NinjaTrader.Data
{
	/// <summary>
	/// </summary>
	public class DayBarsType : BarsType
	{
		private static bool		registered		= Register(new DayBarsType());
		private DateTime		cacheSessionEnd = Cbi.Globals.MinDate;

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
			if (bars.Count == 0)
				if (isRealtime && bars.Session.SessionsOfDay.Length > 0)
				{
					DateTime barTime;
					bars.Session.GetSessionDate(time, false, out barTime, out cacheSessionEnd);
					AddBar(bars, open, high, low, close, barTime, volume, true);
				}
				else
					AddBar(bars, open, high, low, close, time.Date, volume, isRealtime);
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
					UpdateBar(bars, open, high, low, close, barTime, volume, isRealtime);
				else
					AddBar(bars, open, high, low, close, barTime, volume, isRealtime);
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="barsData"></param>
		public override void ApplyDefaults(Gui.Chart.BarsData barsData)
		{
			barsData.DaysBack		= 365;
			barsData.Period.Value	= 1;
		}

		/// <summary>
		/// </summary>
		public override PeriodType BuiltFrom
		{
			get { return PeriodType.Day; }
		}

		/// <summary>
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartDataBoxDate(DateTime time)
		{
			return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
		}

		/// <summary>
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartLabel(Gui.Chart.ChartControl chartControl, DateTime time)
		{
			return time.ToString(chartControl.LabelFormatDay, Cbi.Globals.CurrentCulture);
		}

		/// <summary>
		/// Here is how you restrict the selectable chart styles by bars type
		/// </summary>
		public override Gui.Chart.ChartStyleType[] ChartStyleTypesSupported
		{
			get { return new[] { Gui.Chart.ChartStyleType.Box, Gui.Chart.ChartStyleType.CandleStick, Gui.Chart.ChartStyleType.HiLoBars, Gui.Chart.ChartStyleType.LineOnClose, 
				Gui.Chart.ChartStyleType.OHLC, Gui.Chart.ChartStyleType.Custom0, Gui.Chart.ChartStyleType.Custom1, Gui.Chart.ChartStyleType.Custom2, Gui.Chart.ChartStyleType.Custom3,
				Gui.Chart.ChartStyleType.Custom4, Gui.Chart.ChartStyleType.Custom5, Gui.Chart.ChartStyleType.Custom6, Gui.Chart.ChartStyleType.Custom7, Gui.Chart.ChartStyleType.Custom8,
				Gui.Chart.ChartStyleType.Custom9, Gui.Chart.ChartStyleType.Final0, Gui.Chart.ChartStyleType.Final1, Gui.Chart.ChartStyleType.Final2, Gui.Chart.ChartStyleType.Final3,
				Gui.Chart.ChartStyleType.Final4 }; }
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new DayBarsType();
		}

		/// <summary>
		/// </summary>
		public DayBarsType() : base(PeriodType.Day){}

		/// <summary>
		/// </summary>
		public override int DefaultValue
		{ 
			get { return 1; }
		}

		/// <summary>
		/// </summary>
		public override string DisplayName
		{
			get { return Period.Id.ToString(); }
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <param name="barsBack"></param>
		/// <returns></returns>
		public override int GetInitialLookBackDays(Period period, int barsBack)
		{ 
			return (int) Math.Ceiling(((double) period.Value * barsBack * 7.0) / 4.5);
		}
	
		/// <summary>
		/// </summary>
		public override double GetPercentComplete(Bars bars, DateTime now)
		{
			return now.Date <= bars.TimeLastBar.Date
						? 1.0 - (bars.TimeLastBar.AddDays(1).Subtract(now).TotalDays/bars.Period.Value)
						: 1;
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
			properties.Remove(properties.Find("BasePeriodType", true));
			properties.Remove(properties.Find("BasePeriodValue", true));
			properties.Remove(properties.Find("PointAndFigurePriceType", true));
			properties.Remove(properties.Find("ReversalType", true));
			properties.Remove(properties.Find("Value2", true));

			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Value", "\r\rValue");

			return properties;
		}

		/// <summary>
		/// </summary>
		public override bool IsIntraday
		{
			get { return false; }
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <returns></returns>
		public override string ToString(Period period)
		{
			return (period.Value == 1 ? "Daily" : period.Value + " Day") + (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty);
		}
	}

	/// <summary>
	/// </summary>
	public class KagiBarsType : BarsType
	{
		private enum			Trend					{ Up, Down, Undetermined }

		private double			anchorPrice				= double.MinValue;
		private DateTime		cacheSessionEnd			= Cbi.Globals.MinDate;
		private bool			endOfBar				= false;
		private DateTime		prevTime				= Cbi.Globals.MinDate;
		private static bool		registered				= Register(new KagiBarsType());
		private double			reversalPoint			= double.MinValue;
		private int				tmpCount				= 0;
		private int				tmpDayCount				= 0;
		private int				tmpTickCount			= 0;
		private DateTime		tmpTime					= Cbi.Globals.MinDate;
		private long			tmpVolume				= 0;
		private Trend			trend					= Trend.Undetermined;
		private long			volumeCount				= 0;

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
					bars.LastPrice	= bars.GetClose(bars.Count - 1);
					anchorPrice		= bars.LastPrice;
				}

			switch (bars.Period.BasePeriodType)
			{
				case PeriodType.Day:
					tmpTime = time.Date; // will be modified for realtime only
					if (isRealtime && time >= cacheSessionEnd /* on realtime include60 is always false */)
					{
						bars.Session.GetSessionDate(time, false, out tmpTime, out cacheSessionEnd);
						if (tmpTime < time.Date) tmpTime = time.Date; // make sure timestamps are ascending
					}

					if (prevTime != tmpTime) tmpDayCount++;

					if (tmpDayCount < bars.Period.BasePeriodValue
						|| (!isRealtime && bars.Count > 0 && tmpTime == bars.TimeLastBar.Date)
						|| (isRealtime && bars.Count > 0 && tmpTime <= bars.TimeLastBar.Date))
						endOfBar = false;
					else
					{
						prevTime = tmpTime;
						endOfBar = true;
					}

					break;

				case PeriodType.Minute:

					if (tmpTime == Cbi.Globals.MinDate)
						prevTime = tmpTime = TimeToBarTimeMinute(bars, time, bars.Session.NextBeginTime, bars.Period.BasePeriodValue, isRealtime);

					if (!isRealtime && time <= tmpTime || isRealtime && time < tmpTime)
						endOfBar = false;
					else
					{
						prevTime	= tmpTime;
						tmpTime		= TimeToBarTimeMinute(bars, time, bars.Session.NextBeginTime, bars.Period.BasePeriodValue, isRealtime);
						endOfBar	= true;
					}
					break;

				case PeriodType.Volume:
					if (tmpTime == Cbi.Globals.MinDate)
					{
						tmpVolume	= volume;
						endOfBar	= tmpVolume >= bars.Period.BasePeriodValue;
						prevTime	= tmpTime = time;
						if (endOfBar) tmpVolume = 0;
						break;
					}

					tmpVolume	+= volume;
					endOfBar	= tmpVolume >= bars.Period.BasePeriodValue;
					if (endOfBar)
					{
						prevTime	= tmpTime;
						tmpVolume	= 0;
					}
                    tmpTime = time;
                    break;

                case PeriodType.Tick:
                    if (tmpTime == Cbi.Globals.MinDate || bars.Period.BasePeriodValue == 1)
                    {
                        prevTime        = tmpTime == Cbi.Globals.MinDate ? time : tmpTime;
                        tmpTime         = time;
                        tmpTickCount    = bars.Period.BasePeriodValue == 1 ? 0 : 1;
                        endOfBar        = bars.Period.BasePeriodValue == 1;
                        break;
                    }

                    if (tmpTickCount < bars.Period.BasePeriodValue)
                    {
                        tmpTime         = time;
                        endOfBar        = false;
                        tmpTickCount++;
                    }
                    else
                    {
                        prevTime        = tmpTime;
                        tmpTime         = time;
                        endOfBar        = true;
                        tmpTickCount    = 1;
                    }
                    break;

                case PeriodType.Month:
					if (tmpTime == Cbi.Globals.MinDate)
						prevTime = tmpTime = TimeToBarTimeMonth(time, bars.Period.BasePeriodValue);

					if (time.Month <= tmpTime.Month && time.Year == tmpTime.Year || time.Year < tmpTime.Year)
						endOfBar = false;
					else
					{
						prevTime	= tmpTime;
						endOfBar	= true;
						tmpTime		= TimeToBarTimeMonth(time, bars.Period.BasePeriodValue);
					}
					break;

				case PeriodType.Second:
					if (tmpTime == Cbi.Globals.MinDate)
					{
						prevTime = tmpTime = TimeToBarTimeSecond(bars, time,
																	new DateTime(	bars.Session.NextBeginTime.Year,
																					bars.Session.NextBeginTime.Month,
																					bars.Session.NextBeginTime.Day,
																					bars.Session.NextBeginTime.Hour,
																					bars.Session.NextBeginTime.Minute, 0),
																	bars.Period.BasePeriodValue);
					}
					if ((bars.Period.Value > 1 && time < tmpTime) || (bars.Period.Value == 1 && time <= tmpTime))
						endOfBar	= false;
					else
					{
						prevTime	= tmpTime;
						tmpTime		= TimeToBarTimeSecond(bars, time, bars.Session.NextBeginTime, bars.Period.BasePeriodValue);
						endOfBar	= true;
					}
					break;

				case PeriodType.Week:
					if (tmpTime == Cbi.Globals.MinDate)
						prevTime = tmpTime = TimeToBarTimeWeek(time.Date, tmpTime.Date, bars.Period.BasePeriodValue);
					if (time.Date <= tmpTime.Date)
						endOfBar = false;
					else
					{
						prevTime	= tmpTime;
						endOfBar	= true;
						tmpTime		= TimeToBarTimeWeek(time.Date, tmpTime.Date, bars.Period.BasePeriodValue);
					}
					break;

				case PeriodType.Year:
					if (tmpTime == Cbi.Globals.MinDate)
						prevTime = tmpTime = TimeToBarTimeYear(time, bars.Period.Value);
					if (time.Year <= tmpTime.Year)
						endOfBar = false;
					else
					{
						prevTime	= tmpTime;
						endOfBar	= true;
						tmpTime		= TimeToBarTimeYear(time, bars.Period.Value);
					}
					break;
			}
			#endregion
			#region Kagi Logic

			reversalPoint = bars.Period.ReversalType == ReversalType.Tick ? bars.Period.Value * bars.Instrument.MasterInstrument.TickSize : bars.Period.Value * 0.01 * anchorPrice;

			if (bars.Count == 0 || (IsIntraday && ((bars.Period.BasePeriodType != PeriodType.Second && bars.IsNewSession(time, isRealtime))
									|| (bars.Period.BasePeriodType == PeriodType.Second && bars.IsNewSession(tmpTime, isRealtime)))))
			{
				if (bars.Count > 0)
				{
					double		lastOpen		= bars.GetOpen(bars.Count - 1);
					double		lastHigh		= bars.GetHigh(bars.Count - 1);
					double		lastLow			= bars.GetLow(bars.Count - 1);
					double		lastClose		= bars.GetClose(bars.Count - 1);

					if (bars.Count == tmpCount)
						CalculateKagiBar(bars, lastOpen, lastHigh, lastLow, lastClose, prevTime, volume, isRealtime);
				}

				AddBar(bars, close, close, close, close, tmpTime, volume, isRealtime);
				anchorPrice		= close;
				trend			= Trend.Undetermined;
				prevTime		= tmpTime;
				volumeCount		= 0;
				bars.LastPrice	= close;
				tmpCount		= bars.Count;
				return;
			}

			Bar		bar		= (Bar)bars.Get(bars.Count - 1);
			double	c		= bar.Close;
			double	o		= bar.Open;
			double	h		= bar.High;
			double	l		= bar.Low;

			if (endOfBar)
				CalculateKagiBar(bars, o, h, l, c, prevTime, volume, isRealtime);
			else
				volumeCount += volume;

			bars.LastPrice	= close;
			tmpCount		= bars.Count;

			#endregion
		}

		private void CalculateKagiBar(Bars bars, double o, double h, double l, double c, DateTime barTime, long volume, bool isRealtime)
		{
			switch (trend)
			{
				case Trend.Up:
					if (bars.Instrument.MasterInstrument.Compare(bars.LastPrice, anchorPrice - reversalPoint) <= 0)
					{
						AddBar(bars, anchorPrice, anchorPrice, bars.LastPrice, bars.LastPrice, barTime, volumeCount, isRealtime);
						anchorPrice		= bars.LastPrice;
						trend			= Trend.Down;
					}
					else if (bars.Instrument.MasterInstrument.Compare(bars.LastPrice, anchorPrice) > 0)
					{
						UpdateBar(bars, o, bars.LastPrice, l, bars.LastPrice, barTime, volumeCount, isRealtime);
						anchorPrice = bars.LastPrice;
					}
					else
						UpdateBar(bars, o, h, l, c, barTime, volumeCount, isRealtime);
					break;
				case Trend.Down:
					if (bars.Instrument.MasterInstrument.Compare(bars.LastPrice, anchorPrice + reversalPoint) >= 0)
					{
						AddBar(bars, anchorPrice, bars.LastPrice, anchorPrice, bars.LastPrice, barTime, volumeCount, isRealtime);
						anchorPrice = bars.LastPrice;
						trend = Trend.Up;
					}
					else if (bars.Instrument.MasterInstrument.Compare(bars.LastPrice, anchorPrice) < 0)
					{
						UpdateBar(bars, o, h, bars.LastPrice, bars.LastPrice, barTime, volumeCount, isRealtime);
						anchorPrice = bars.LastPrice;
					}
					else
						UpdateBar(bars, o, h, l, c, barTime, volumeCount, isRealtime);
					break;
				default:
					UpdateBar(bars, o, bars.LastPrice, bars.LastPrice, bars.LastPrice, barTime, volumeCount, isRealtime);
					anchorPrice = bars.LastPrice;
					trend		= bars.Instrument.MasterInstrument.Compare(bars.LastPrice, o) < 0 ? Trend.Down
								: bars.Instrument.MasterInstrument.Compare(bars.LastPrice, o) > 0 ? Trend.Up : Trend.Undetermined;
					break;
			}
			volumeCount = volume;
		}

		/// <summary>
		/// </summary>
		/// <param name="barsData"></param>
		public override void ApplyDefaults(Gui.Chart.BarsData barsData)
		{
			barsData.Period.Value = 2;
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
				case PeriodType.Tick	:
				case PeriodType.Second	:
				case PeriodType.Volume	:
				case PeriodType.Minute	:
				case PeriodType.Day		: return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
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
			get { return new[] { Gui.Chart.ChartStyleType.KagiLine }; }
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new KagiBarsType();
		}

		/// <summary>
		/// </summary>
		public KagiBarsType() : base(PeriodType.Kagi) { }

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
			get { return "Kagi"; }
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
			properties.Remove(properties.Find("PointAndFigurePriceType", true));
			properties.Remove(properties.Find("Value2", true));

			// here is how you change the display name of the property on the properties grid
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Value", "\r\rReversal");

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
				case PeriodType.Day		: return string.Format("{0} {1} Kagi{2}",		period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Daily" : "Day"), (period.MarketDataType != MarketDataType.Last ? string.Format(" - {0}", period.MarketDataType) : string.Empty));
				case PeriodType.Minute	: return string.Format("{0} Min Kagi{1}",		period.BasePeriodValue, (period.MarketDataType != MarketDataType.Last ? string.Format(" - {0}", period.MarketDataType) : string.Empty));
				case PeriodType.Month	: return string.Format("{0} {1} Kagi{2}",		period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Monthly" : "Month"), (period.MarketDataType != MarketDataType.Last ? string.Format(" - {0}", period.MarketDataType) : string.Empty));
				case PeriodType.Second	: return string.Format("{0} {1} Kagi{2}",		period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Second" : "Seconds"), (period.MarketDataType != MarketDataType.Last ? string.Format(" - {0}", period.MarketDataType) : string.Empty));
				case PeriodType.Tick	: return string.Format("{0} Tick Kagi{1}",		period.BasePeriodValue, (period.MarketDataType != MarketDataType.Last ? string.Format(" - {0}", period.MarketDataType) : string.Empty));
				case PeriodType.Volume	: return string.Format("{0} Volume Kagi{1}",	period.BasePeriodValue, (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Week	: return string.Format("{0} {1} Kagi{2}",		period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Weekly" : "Weeks"), (period.MarketDataType != MarketDataType.Last ? string.Format(" - {0}", period.MarketDataType) : string.Empty));
				case PeriodType.Year	: return string.Format("{0} {1} Kagi{2}",		period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Yearly" : "Years"), (period.MarketDataType != MarketDataType.Last ? string.Format(" - {0}", period.MarketDataType) : string.Empty));
				default					: return string.Format("{0} {1}Kagi{2}",		period.BasePeriodValue, BuiltFrom, (period.MarketDataType != MarketDataType.Last ? string.Format(" - {0}", period.MarketDataType) : string.Empty));
			}
		}

		private static DateTime TimeToBarTimeMinute(Bars bars, DateTime time, DateTime periodStart, int periodValue, bool isRealtime)
		{
			DateTime barTimeStamp = isRealtime ? periodStart.AddMinutes(periodValue + Math.Floor(Math.Floor(Math.Max(0, time.Subtract(periodStart).TotalMinutes)) / periodValue) * periodValue) : periodStart.AddMinutes(Math.Ceiling(Math.Ceiling(Math.Max(0, time.Subtract(periodStart).TotalMinutes)) / periodValue) * periodValue);
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
			DateTime barTimeStamp = periodStart.AddSeconds(Math.Ceiling(Math.Ceiling(Math.Max(0, time.AddSeconds(periodValue > 1 ? 1 : 0 /* sec 0 into bar 1 - n */).Subtract(periodStart).TotalSeconds)) / periodValue) * periodValue);
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

	/// <summary>
	/// </summary>
	public class LineBreak : BarsType
	{
		private double			anchorPrice			= double.MinValue;
		private bool			firstBarOfSession	= true;
		private	bool			newSession			= false;
		private	int				newSessionIdx		= 0;
		private static bool		registered			= Register(new LineBreak());
		private double			switchPrice			= double.MinValue;
		private int				tmpCount			= 0;
		private int				tmpDayCount			= 0;
		private	int				tmpTickCount		= 0;
		private	DateTime		tmpTime				= Cbi.Globals.MinDate;
		private long			tmpVolume			= 0;
		private bool			upTrend				= true;

		/// <summary>
		/// 
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
			if (bars.Count == 0 && tmpTime != Cbi.Globals.MinDate) // reset caching when live request trimmed existing bars
				tmpTime = Cbi.Globals.MinDate;

			bool endOfBar = true;
			if (tmpTime == Cbi.Globals.MinDate)
			{
				tmpTime			= time;
				tmpDayCount		= 1;
				tmpTickCount	= 1;
			}
			else if (bars.Count < tmpCount && bars.Count == 0) // reset cache when bars are trimmed
			{
				tmpTime			= Cbi.Globals.MinDate;
				tmpVolume		= 0;
				tmpDayCount		= 0;
				tmpTickCount	= 0;
			}
			else if (bars.Count < tmpCount && bars.Count > 0) // reset cache when bars are trimmed
			{
				tmpTime			= bars.GetTime(bars.Count - 1); 
				tmpVolume		= bars.GetVolume(bars.Count - 1);
				tmpTickCount	= bars.TickCount;
				tmpDayCount		= bars.DayCount;
			}

			switch (bars.Period.BasePeriodType)
			{
				case PeriodType.Day:
					{
						if (bars.Count == 0 || (bars.Count > 0 && (bars.TimeLastBar.Month < time.Month || bars.TimeLastBar.Year < time.Year)))
						{
							tmpTime			= time.Date;
							bars.LastPrice	= close;
							newSession		= true;
						}
						else
						{
							tmpTime			= time.Date;
							tmpVolume		+= volume;
							bars.LastPrice	= close;
							tmpDayCount++;

							if (tmpDayCount < bars.Period.BasePeriodValue || (bars.Count > 0 && bars.TimeLastBar.Date == time.Date))
								endOfBar = false;
						}
						break;
					}
				case PeriodType.Minute:
					{
						if (bars.Count == 0 || bars.IsNewSession(time, isRealtime))
						{
							tmpTime		= TimeToBarTimeMinute(bars, time, bars.Session.NextBeginTime, bars.Period.BasePeriodValue, isRealtime);
							newSession	= true;
							tmpVolume	= 0;
						}
						else
						{
							if (isRealtime && time < bars.TimeLastBar || !isRealtime && time <= bars.TimeLastBar)
							{
								tmpTime		= bars.TimeLastBar;
								endOfBar	= false;
							}
							else
								tmpTime		= TimeToBarTimeMinute(bars, time, bars.Session.NextBeginTime, bars.Period.BasePeriodValue, isRealtime);

							tmpVolume		+= volume;
						}
						break;
					}
				case PeriodType.Month:
					{
						if (tmpTime == Cbi.Globals.MinDate)
						{
							tmpTime		= TimeToBarTimeMonth(time, bars.Period.BasePeriodValue);

							if (bars.Count == 0)
								break;

							endOfBar = false;
						}
						else if ((time.Month <= tmpTime.Month && time.Year == tmpTime.Year) || time.Year < tmpTime.Year)
						{
							tmpVolume		+= volume;
							bars.LastPrice	= close;
							endOfBar		= false;
						}
						break;
					}
				case PeriodType.Second:
					{
						if (bars.IsNewSession(time, isRealtime))
						{
							tmpTime = TimeToBarTimeSecond(bars, time, new DateTime(bars.Session.NextBeginTime.Year, bars.Session.NextBeginTime.Month, bars.Session.NextBeginTime.Day, bars.Session.NextBeginTime.Hour, bars.Session.NextBeginTime.Minute, 0), bars.Period.BasePeriodValue);

							if (bars.Count == 0)
								break;

							endOfBar	= false;
							newSession	= true;
						}
						else if (time <= tmpTime)
						{
							tmpVolume		+= volume;
							bars.LastPrice	= close;
							endOfBar		= false;
						}
						else
							tmpTime = TimeToBarTimeSecond(bars, time, bars.Session.NextBeginTime, bars.Period.BasePeriodValue);
						break;
					}
				case PeriodType.Tick:
					{
						if (bars.IsNewSession(time, isRealtime))
						{
							newSession = true;
							tmpTime = time;
							tmpTickCount = 1;

							if (bars.Count == 0)
								break;

							endOfBar = false;
						}
						else if (bars.Period.BasePeriodValue > 1 && tmpTickCount < bars.Period.BasePeriodValue)
						{
							tmpTime			= time;
							tmpVolume		+= volume;
							tmpTickCount++;
							bars.LastPrice	= close;
							endOfBar		= false;
						}
						else
							tmpTime = time; // there can't be a situation when new ticks go into old bar, this would mean peeking into future. Fixed in NT7B14 20100416 CH
						break;
					}
				case PeriodType.Volume:
					{
						if (bars.IsNewSession(time, isRealtime))
							newSession = true;
						else if (bars.Count == 0 && volume > 0)
							break;
						else
						{
							tmpVolume += volume;
							if (tmpVolume < bars.Period.BasePeriodValue)
							{
								bars.LastPrice = close;
								endOfBar = false;
							}
							else if (tmpVolume == 0)
								endOfBar = false;
						}

						tmpTime = time; // there can't be a situation when new ticks go into old bar, this would mean peeking into future. Fixed in NT7B14 20100416 CH

						break;
					}
				case PeriodType.Week:
					{
						if (tmpTime == Cbi.Globals.MinDate)
						{
							tmpTime			= TimeToBarTimeWeek(time.Date, tmpTime.Date, bars.Period.BasePeriodValue);

							if (bars.Count == 0)
								break;

							endOfBar = false;
						}
						else if (time.Date <= tmpTime.Date)
						{
							tmpVolume		+= volume;
							bars.LastPrice	= close;
							endOfBar		= false;
						}
						break;
					}
				case PeriodType.Year:
					{
						if (tmpTime == Cbi.Globals.MinDate)
						{
							tmpTime			= TimeToBarTimeYear(time, bars.Period.Value);

							if (bars.Count == 0)
								break;

							endOfBar = false;
						}
						else if (time.Year <= tmpTime.Year)
						{
							tmpVolume		+= volume;
							bars.LastPrice	= close;
							endOfBar		= false;
						}
						break;
					}
				default:
					break;
			}

			if (bars.Count == 0 || (newSession && IsIntraday))
			{
				AddBar(bars, open, close, close, close, tmpTime, volume, isRealtime);
				upTrend				= (open < close);
				newSessionIdx		= bars.Count - 1;
				newSession			= false;
				firstBarOfSession	= true;
				anchorPrice			= close;
				switchPrice			= open;
			}
			else if (firstBarOfSession && endOfBar == false)
			{
				double prevOpen		= bars.GetOpen(bars.Count - 1);
				bars.RemoveLastBar(isRealtime);
				AddBar(bars, prevOpen, close, close, close, tmpTime, tmpVolume, isRealtime);
				upTrend				= (prevOpen < close);
				anchorPrice			= close;
			}
			else
			{
				int		breakCount		= bars.Period.Value;
				Bar		bar				= (Bar)bars.Get(bars.Count - 1);
				double	breakMax		= double.MinValue;
				double	breakMin		= double.MaxValue;

				if (firstBarOfSession)
				{
					AddBar(bars, anchorPrice, close, close, close, tmpTime, volume, isRealtime);
					firstBarOfSession	= false;
					tmpVolume			= volume;
					tmpTime				= Cbi.Globals.MinDate;
					return;
				}

				if (bars.Count - newSessionIdx - 1 < breakCount)
					breakCount = bars.Count - (newSessionIdx + 1);

				for (int k = 1; k <= breakCount; k++)
				{
					Bar tmp			= (Bar)bars.Get(bars.Count - k - 1);
					breakMax		= Math.Max(breakMax, tmp.Open);
					breakMax		= Math.Max(breakMax, tmp.Close);
					breakMin		= Math.Min(breakMin, tmp.Open);
					breakMin		= Math.Min(breakMin, tmp.Close);
				}

				bars.LastPrice = close;

				if (upTrend)
					if (endOfBar)
					{
						bool adding = false;
						if (bars.Instrument.MasterInstrument.Compare(bar.Close, anchorPrice) > 0)
						{
							anchorPrice = bar.Close;
							switchPrice = bar.Open;
							tmpVolume = volume;
							adding = true;
						}
						else
							if (bars.Instrument.MasterInstrument.Compare(breakMin, bar.Close) > 0)
							{
								anchorPrice = bar.Close;
								switchPrice = bar.Open;
								tmpVolume = volume;
								upTrend = false;
								adding = true;
							}

						if (adding)
						{
							double tmpOpen = upTrend ? Math.Min(Math.Max(switchPrice, close), anchorPrice) : Math.Max(Math.Min(switchPrice, close), anchorPrice);
							AddBar(bars, tmpOpen, close, close, close, tmpTime, volume, isRealtime);
						}
						else
						{
							bars.RemoveLastBar(isRealtime);
							double tmpOpen = Math.Min(Math.Max(switchPrice, close), anchorPrice);
							AddBar(bars, tmpOpen, close, close, close, tmpTime, tmpVolume, isRealtime);
						}
					}
					else
					{
						bars.RemoveLastBar(isRealtime);
						double tmpOpen = Math.Min(Math.Max(switchPrice, close), anchorPrice);
						AddBar(bars, tmpOpen, close, close, close, tmpTime, tmpVolume, isRealtime);
					}
				else
					if (endOfBar)
					{
						bool adding = false;
						if (bars.Instrument.MasterInstrument.Compare(bar.Close, anchorPrice) < 0)
						{
							anchorPrice		= bar.Close;
							switchPrice		= bar.Open;
							tmpVolume		= volume;
							adding			= true;
						}
						else
							if (bars.Instrument.MasterInstrument.Compare(breakMax, bar.Close) < 0)
							{
								anchorPrice		= bar.Close;
								switchPrice		= bar.Open;
								tmpVolume		= volume;
								upTrend			= true;
								adding			= true;
							}

						if (adding)
						{
							double tmpOpen = upTrend ? Math.Min(Math.Max(switchPrice, close), anchorPrice) : Math.Max(Math.Min(switchPrice, close), anchorPrice);
							AddBar(bars, tmpOpen, close, close, close, tmpTime, volume, isRealtime);
						}
						else
						{
							bars.RemoveLastBar(isRealtime);
							double tmpOpen = Math.Max(Math.Min(switchPrice, close), anchorPrice);
							AddBar(bars, tmpOpen, close, close, close, tmpTime, tmpVolume, isRealtime);
						}
					}
					else
					{
						bars.RemoveLastBar(isRealtime);
						double tmpOpen = Math.Max(Math.Min(switchPrice, close), anchorPrice);
						AddBar(bars, tmpOpen, close, close, close, tmpTime, tmpVolume, isRealtime);
					}
			}

			if (endOfBar)
				tmpTime = Cbi.Globals.MinDate;

			tmpCount = bars.Count;
		}

		/// <summary>
		/// </summary>
		/// <param name="barsData"></param>
		public override void ApplyDefaults(Gui.Chart.BarsData barsData)
		{
			barsData.Period.Value	= 3;

			switch (barsData.Period.BasePeriodType)
			{
				case PeriodType.Day		:		barsData.Period.BasePeriodValue = 1;	barsData.DaysBack = 365;	break;
				case PeriodType.Minute	:		barsData.Period.BasePeriodValue = 1;	barsData.DaysBack = 5;		break;
				case PeriodType.Month	:		barsData.Period.BasePeriodValue = 1;	barsData.DaysBack = 5475;	break;
				case PeriodType.Second	:		barsData.Period.BasePeriodValue = 30;	barsData.DaysBack = 3;		break;
				case PeriodType.Tick	:		barsData.Period.BasePeriodValue = 150;	barsData.DaysBack = 3;		break;
				case PeriodType.Volume	:		barsData.Period.BasePeriodValue = 1000;	barsData.DaysBack = 3;		break;
				case PeriodType.Week	:		barsData.Period.BasePeriodValue = 1;	barsData.DaysBack = 1825;	break;
				case PeriodType.Year	:		barsData.Period.BasePeriodValue = 1;	barsData.DaysBack = 15000;	break;
				default					:																			break;
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
					case PeriodType.Day		:		return PeriodType.Day; 
					case PeriodType.Minute	:		return PeriodType.Minute; 
					case PeriodType.Month	:		return PeriodType.Day; 
					case PeriodType.Second	:		return PeriodType.Tick; 
					case PeriodType.Tick	:		return PeriodType.Tick; 
					case PeriodType.Volume	:		return PeriodType.Tick; 
					case PeriodType.Week	:		return PeriodType.Day; 
					case PeriodType.Year	:		return PeriodType.Day;
					default					:		return PeriodType.Minute;
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
				case PeriodType.Day		:		return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
				case PeriodType.Minute	:		return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
				case PeriodType.Month	:		return string.Format("{0}/{1}", time.Month, time.Year);
				case PeriodType.Second	:		return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
				case PeriodType.Tick	:		return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
				case PeriodType.Volume	:		return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
				case PeriodType.Week	:		return string.Format("{0}/{1}", Gui.Globals.GetCalendarWeek(time), time.Year);
				case PeriodType.Year	:		return time.Year.ToString();
				default					:		return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);

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
					case PeriodType.Day		:		return time.ToString(chartControl.LabelFormatDay,		Cbi.Globals.CurrentCulture);
					case PeriodType.Minute	:		return time.ToString(chartControl.LabelFormatMinute,	Cbi.Globals.CurrentCulture);
					case PeriodType.Month	:		return time.ToString(chartControl.LabelFormatMonth,		Cbi.Globals.CurrentCulture);
					case PeriodType.Second	:		return time.ToString(chartControl.LabelFormatSecond,	Cbi.Globals.CurrentCulture);
					case PeriodType.Tick	:		return time.ToString(chartControl.LabelFormatTick,		Cbi.Globals.CurrentCulture);
					case PeriodType.Volume	:		return time.ToString(chartControl.LabelFormatTick,		Cbi.Globals.CurrentCulture);
					case PeriodType.Week	:		return time.ToString(chartControl.LabelFormatDay,		Cbi.Globals.CurrentCulture);
					case PeriodType.Year	:		return time.ToString(chartControl.LabelFormatYear,		Cbi.Globals.CurrentCulture);
					default					:		return time.ToString(chartControl.LabelFormatDay,		Cbi.Globals.CurrentCulture);
				}
		}

		/// <summary>
		/// Here is how you restrict the selectable chart styles by bars type
		/// </summary>
		public override Gui.Chart.ChartStyleType[] ChartStyleTypesSupported
		{
			get { return new[] { Gui.Chart.ChartStyleType.OpenClose }; }
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new LineBreak();
		}

		/// <summary>
		/// </summary>
		public LineBreak() : base(PeriodType.LineBreak)
		{
			Period.Value = 3;
		}

		/// <summary>
		/// </summary>
		public override int DefaultValue
		{ 
			get 
			{ 
				switch (Period.BasePeriodType)
				{
					case PeriodType.Second:		return 30; 
					case PeriodType.Tick:		return 150; 
					case PeriodType.Volume:		return 1000; 
					default:					return 1;
				} 
			}
		}

		/// <summary>
		/// </summary>
		public override string DisplayName
		{
			get { return Period.Id.ToString(); }
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
				case PeriodType.Day		:		return new DayBarsType()	.GetInitialLookBackDays(period, barsBack); 
				case PeriodType.Minute	:		return new MinuteBarsType()	.GetInitialLookBackDays(period, barsBack); 
				case PeriodType.Month	:		return new MonthBarsType()	.GetInitialLookBackDays(period, barsBack); 
				case PeriodType.Second	:		return new SecondBarsType()	.GetInitialLookBackDays(period, barsBack); 
				case PeriodType.Tick	:		return new TickBarsType()	.GetInitialLookBackDays(period, barsBack); 
				case PeriodType.Volume	:		return new VolumeBarsType()	.GetInitialLookBackDays(period, barsBack); 
				case PeriodType.Week	:		return new WeekBarsType()	.GetInitialLookBackDays(period, barsBack); 
				case PeriodType.Year	:		return new YearBarsType()	.GetInitialLookBackDays(period, barsBack); 
				default					:		return new MinuteBarsType()	.GetInitialLookBackDays(period, barsBack); 
			}
		}

		/// <summary>
		/// </summary>
		public override double GetPercentComplete(Bars bars, DateTime now)
		{
			throw new ApplicationException("GetPercentComplete not supported in " + DisplayName);
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
			properties.Remove(properties.Find("PointAndFigurePriceType", true));
			properties.Remove(properties.Find("ReversalType", true));
			properties.Remove(properties.Find("Value2", true));

			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Value", "\r\rLine Breaks");

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
					case PeriodType.Day		:		return false; 
					case PeriodType.Minute	:		return true; 
					case PeriodType.Month	:		return false; 
					case PeriodType.Second	:		return true; 
					case PeriodType.Tick	:		return true; 
					case PeriodType.Volume	:		return true; 
					case PeriodType.Week	:		return false; 
					case PeriodType.Year	:		return false; 
					default					:		return false; 
				}
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

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <returns></returns>
		public override string ToString(Period period)
		{
			switch (Period.BasePeriodType)
			{
				case PeriodType.Day:		return string.Format("{0} {1} LineBreak{2}", period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Daily" : "Day"), (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Minute:		return string.Format("{0} Min LineBreak{1}", period.BasePeriodValue, (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Month:		return string.Format("{0} {1} LineBreak{2}", period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Monthly" : "Month"), (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Second:		return string.Format("{0} {1} LineBreak{2}", period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Second" : "Seconds"), (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Tick:		return string.Format("{0} Tick LineBreak{1}", period.BasePeriodValue, (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Volume:		return string.Format("{0} Volume LineBreak{1}", period.BasePeriodValue, (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Week:		return string.Format("{0} {1} LineBreak{2}", period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Weekly" : "Weeks"), (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Year:		return string.Format("{0} {1} LineBreak{2}", period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Yearly" : "Years"), (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				default:					return string.Format("{0} {1} LineBreak{2}", period.BasePeriodValue, BuiltFrom, (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
			}
		}
	}

	/// <summary>
	/// </summary>
	public class MinuteBarsType : BarsType
	{
		private static bool		registered		= Register(new MinuteBarsType());

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
			if (bars.Count == 0)
				AddBar(bars, open, high, low, close, TimeToBarTime(bars, time, bars.Session.NextBeginTime, bars.Period.Value, isRealtime), volume, isRealtime);
			else
			{
				if (isRealtime && time < bars.TimeLastBar)
					UpdateBar(bars, open, high, low, close, bars.TimeLastBar, volume, true);
				else if (!isRealtime && time <= bars.TimeLastBar)
					UpdateBar(bars, open, high, low, close, bars.TimeLastBar, volume, false); 
				else
				{
					time = TimeToBarTime(bars, time, bars.Session.NextBeginTime, bars.Period.Value, isRealtime);
					AddBar(bars, open, high, low, close, time, volume, isRealtime);
				}
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="barsData"></param>
		public override void ApplyDefaults(Gui.Chart.BarsData barsData)
		{
			barsData.DaysBack		= 5;
			barsData.Period.Value	= 1;
		}

		/// <summary>
		/// </summary>
		public override PeriodType BuiltFrom
		{
			get { return PeriodType.Minute; }
		}

		/// <summary>
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartDataBoxDate(DateTime time)
		{
			return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
		}

		/// <summary>
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartLabel(Gui.Chart.ChartControl chartControl, DateTime time)
		{
			return time.ToString(chartControl.LabelFormatMinute, Cbi.Globals.CurrentCulture);
		}

		/// <summary>
		/// Here is how you restrict the selectable chart styles by bars type
		/// </summary>
		public override Gui.Chart.ChartStyleType[] ChartStyleTypesSupported
		{
			get { return new[] { Gui.Chart.ChartStyleType.Box, Gui.Chart.ChartStyleType.CandleStick, Gui.Chart.ChartStyleType.HiLoBars, Gui.Chart.ChartStyleType.LineOnClose, 
				Gui.Chart.ChartStyleType.OHLC, Gui.Chart.ChartStyleType.Custom0, Gui.Chart.ChartStyleType.Custom1, Gui.Chart.ChartStyleType.Custom2, Gui.Chart.ChartStyleType.Custom3,
				Gui.Chart.ChartStyleType.Custom4, Gui.Chart.ChartStyleType.Custom5, Gui.Chart.ChartStyleType.Custom6, Gui.Chart.ChartStyleType.Custom7, Gui.Chart.ChartStyleType.Custom8,
				Gui.Chart.ChartStyleType.Custom9, Gui.Chart.ChartStyleType.Final0, Gui.Chart.ChartStyleType.Final1, Gui.Chart.ChartStyleType.Final2, Gui.Chart.ChartStyleType.Final3,
				Gui.Chart.ChartStyleType.Final4}; }
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new MinuteBarsType();
		}

		/// <summary>
		/// </summary>
		public override int DefaultValue
		{ 
			get { return 1; }
		}

		/// <summary>
		/// </summary>
		public override string DisplayName
		{
			get { return Period.Id.ToString(); }
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <param name="barsBack"></param>
		/// <returns></returns>
		public override int GetInitialLookBackDays(Period period, int barsBack)
		{ 
			return (int) Math.Max(1, Math.Ceiling(barsBack / Math.Max(1, 8.0 * 60 / period.Value)) * 7.0 / 5.0);	// 8 hours
		}

		/// <summary>
		/// </summary>
		public override double GetPercentComplete(Bars bars, DateTime now)
		{
			return now <= bars.TimeLastBar ? 1.0 - (bars.TimeLastBar.Subtract(now).TotalMinutes/bars.Period.Value) : 1;
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
			properties.Remove(properties.Find("BasePeriodType", true));
			properties.Remove(properties.Find("BasePeriodValue", true));
			properties.Remove(properties.Find("PointAndFigurePriceType", true));
			properties.Remove(properties.Find("ReversalType", true));
			properties.Remove(properties.Find("Value2", true));

			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Value", "\r\rValue");

			return properties;
		}

		/// <summary>
		/// </summary>
		public override bool IsIntraday
		{
			get { return true; }
		}

		/// <summary>
		/// </summary>
		public MinuteBarsType() : base(PeriodType.Minute)
		{
		}

		private static DateTime TimeToBarTime(Bars bars, DateTime time, DateTime periodStart, int periodValue, bool isRealtime)
		{
			DateTime barTimeStamp = isRealtime 
				? periodStart.AddMinutes(periodValue + Math.Floor(Math.Floor(Math.Max(0, time.Subtract(periodStart).TotalMinutes)) / periodValue) * periodValue) 
				: periodStart.AddMinutes(Math.Ceiling(Math.Ceiling(Math.Max(0, time.Subtract(periodStart).TotalMinutes)) / periodValue) * periodValue);
			if (bars.Session.SessionsOfDay.Length > 0 && barTimeStamp > bars.Session.NextEndTime)
				barTimeStamp = bars.Session.NextEndTime;
			return barTimeStamp;
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <returns></returns>
		public override string ToString(Period period)
		{
			return string.Format("{0} Min{1}", period.Value, (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
		}
	}

	/// <summary>
	/// </summary>
	public class MonthBarsType : BarsType
	{
		private static bool		registered		= Register(new MonthBarsType());

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
			if (bars.Count == 0)
				AddBar(bars, open, high, low, close, TimeToBarTime(time, bars.Period.Value), volume, isRealtime);
			else
			{
				if ((time.Month <= bars.TimeLastBar.Month && time.Year == bars.TimeLastBar.Year) || time.Year < bars.TimeLastBar.Year)
					UpdateBar(bars, open, high, low, close, bars.TimeLastBar, volume, isRealtime);
				else
					AddBar(bars, open, high, low, close, TimeToBarTime(time, bars.Period.Value), volume, isRealtime);
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="barsData"></param>
		public override void ApplyDefaults(Gui.Chart.BarsData barsData)
		{
			barsData.DaysBack		= 5475;		
			barsData.Period.Value	= 1;
		}

		/// <summary>
		/// </summary>
		public override PeriodType BuiltFrom
		{
			get { return PeriodType.Day; }
		}

		/// <summary>
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartDataBoxDate(DateTime time)
		{
			return string.Format("{0}/{1}", time.Month, time.Year);
		}

		/// <summary>
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartLabel(Gui.Chart.ChartControl chartControl, DateTime time)
		{
			return time.ToString(chartControl.LabelFormatMonth, Cbi.Globals.CurrentCulture);
		}

		/// <summary>
		/// Here is how you restrict the selectable chart styles by bars type
		/// </summary>
		public override Gui.Chart.ChartStyleType[] ChartStyleTypesSupported
		{
			get { return new Gui.Chart.ChartStyleType[] { Gui.Chart.ChartStyleType.Box, Gui.Chart.ChartStyleType.CandleStick, Gui.Chart.ChartStyleType.HiLoBars, Gui.Chart.ChartStyleType.LineOnClose, 
				Gui.Chart.ChartStyleType.OHLC, Gui.Chart.ChartStyleType.Custom0, Gui.Chart.ChartStyleType.Custom1, Gui.Chart.ChartStyleType.Custom2, Gui.Chart.ChartStyleType.Custom3,
				Gui.Chart.ChartStyleType.Custom4, Gui.Chart.ChartStyleType.Custom5, Gui.Chart.ChartStyleType.Custom6, Gui.Chart.ChartStyleType.Custom7, Gui.Chart.ChartStyleType.Custom8,
				Gui.Chart.ChartStyleType.Custom9, Gui.Chart.ChartStyleType.Final0, Gui.Chart.ChartStyleType.Final1, Gui.Chart.ChartStyleType.Final2, Gui.Chart.ChartStyleType.Final3,
				Gui.Chart.ChartStyleType.Final4 }; }
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new MonthBarsType();
		}

		/// <summary>
		/// </summary>
		public override int DefaultValue
		{ 
			get { return 1; }
		}

		/// <summary>
		/// </summary>
		public override string DisplayName
		{
			get { return Period.Id.ToString(); }
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <param name="barsBack"></param>
		/// <returns></returns>
		public override int GetInitialLookBackDays(Period period, int barsBack)
		{
			return period.Value * barsBack * 31;
		}

		/// <summary>
		/// </summary>
		public override double GetPercentComplete(Bars bars, DateTime now)
		{
			if (now.Date <= bars.TimeLastBar.Date)
			{
				int month = now.Month;
				int daysInMonth = (month == 2) ? (DateTime.IsLeapYear(now.Year) ? 29 : 28) : (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12 ? 31 : 30);
				return (daysInMonth - (bars.TimeLastBar.Date.AddDays(1).Subtract(now).TotalDays / bars.Period.Value)) / daysInMonth; // not exact
			}
			return 1;
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
			properties.Remove(properties.Find("BasePeriodType", true));
			properties.Remove(properties.Find("BasePeriodValue", true));
			properties.Remove(properties.Find("PointAndFigurePriceType", true));
			properties.Remove(properties.Find("ReversalType", true));
			properties.Remove(properties.Find("Value2", true));

			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Value", "\r\rValue");

			return properties;
		}

		/// <summary>
		/// </summary>
		public override bool IsIntraday
		{
			get { return false; }
		}

		/// <summary>
		/// </summary>
		public MonthBarsType() : base(PeriodType.Month)
		{
		}

		private static DateTime TimeToBarTime(DateTime time, int periodValue)
		{
			DateTime result = new DateTime(time.Year, time.Month, 1); 
			for (int i = 0; i < periodValue; i++)
				result = result.AddMonths(1);

			return result.AddDays(-1);
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <returns></returns>
		public override string ToString(Period period)
		{
			return (period.Value == 1 ? "Monthly" : period.Value + " Month") + (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty);
		}
	}

	/// <summary>
	/// </summary>
	public class PointAndFigureBarsType : BarsType
	{
		private			enum		Trend				{ Up, Down, Undetermined }

		private			double		anchorPrice			= double.MinValue;
		private			double		boxSize				= double.MinValue;
		private			DateTime	cacheSessionEnd		= Cbi.Globals.MinDate;
		private			bool		endOfBar			= false;
		private			DateTime	prevTime			= Cbi.Globals.MinDate;
		private			DateTime	prevTimeD			= Cbi.Globals.MinDate;
		private static	bool		registered			= Register(new PointAndFigureBarsType());
		private			double		reversalSize		= double.MinValue;
		private			int			tmpCount			= 0;
		private			int			tmpDayCount			= 0;
		private			double		tmpHigh				= double.MinValue;
		private			double		tmpLow				= double.MinValue;
		private			int			tmpTickCount		= 0;
		private			DateTime	tmpTime				= Cbi.Globals.MinDate;
		private			long		tmpVolume			= 0;
		private			Trend		trend				= Trend.Undetermined;
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
					tmpTime = time.Date;
					if (isRealtime && time >= cacheSessionEnd)
					{
						tmpDayCount++;
						bars.Session.GetSessionDate(time, false, out tmpTime, out cacheSessionEnd);
						if (tmpTime < time.Date) tmpTime = time.Date; // make sure timestamps are ascending
					}

					if (!isRealtime && prevTimeD != tmpTime) tmpDayCount++;

					if ((!isRealtime && bars.Count > 0 && tmpTime == bars.TimeLastBar.Date)
						|| (isRealtime && bars.Count > 0 && tmpTime <= bars.TimeLastBar.Date)
						|| tmpDayCount < bars.Period.BasePeriodValue)
						endOfBar = false;
					else
					{
						prevTime	= prevTimeD == Cbi.Globals.MinDate ? tmpTime : prevTimeD;
						prevTimeD	= tmpTime;
						endOfBar	= true;
					}

					break;

				case PeriodType.Minute:

					if (tmpTime == Cbi.Globals.MinDate)
						prevTime = tmpTime = TimeToBarTimeMinute(bars, time, bars.Session.NextBeginTime, bars.Period.BasePeriodValue, isRealtime);

					if (!isRealtime && time <= tmpTime || isRealtime && time < tmpTime)
						endOfBar	= false;
					else
					{
						prevTime	= tmpTime;
						tmpTime		= TimeToBarTimeMinute(bars, time, bars.Session.NextBeginTime, bars.Period.BasePeriodValue, isRealtime);
						endOfBar	= true;
					}
					break;

				case PeriodType.Volume:
					if (tmpTime == Cbi.Globals.MinDate)
					{
						tmpVolume	= volume;
						endOfBar	= tmpVolume >= bars.Period.BasePeriodValue;
						prevTime	= tmpTime = time;
						if (endOfBar) 
							tmpVolume = 0;
						break;
					}

					tmpVolume += volume;
					endOfBar = tmpVolume >= bars.Period.BasePeriodValue;
					if (endOfBar)
					{
						prevTime = tmpTime;
						tmpVolume = 0;
						tmpTime = time;
					}
					break;

				case PeriodType.Month:
					if (tmpTime == Cbi.Globals.MinDate)
						prevTime	= tmpTime = TimeToBarTimeMonth(time, bars.Period.BasePeriodValue);

					if (time.Month <= tmpTime.Month && time.Year == tmpTime.Year || time.Year < tmpTime.Year)
						endOfBar	= false;
					else
					{
						prevTime	= tmpTime;
						endOfBar	= true;
						tmpTime		= TimeToBarTimeMonth(time, bars.Period.BasePeriodValue);
					}
					break;

				case PeriodType.Second:
					if (tmpTime == Cbi.Globals.MinDate)
					{
						prevTime = tmpTime = TimeToBarTimeSecond(bars, time,
																	new DateTime(bars.Session.NextBeginTime.Year,
																				bars.Session.NextBeginTime.Month,
																				bars.Session.NextBeginTime.Day,
																				bars.Session.NextBeginTime.Hour,
																				bars.Session.NextBeginTime.Minute, 0),
																	bars.Period.BasePeriodValue);
					}
					if (time <= tmpTime)
						endOfBar	= false;
					else
					{
						prevTime	= tmpTime;
						tmpTime		= TimeToBarTimeSecond(bars, time, bars.Session.NextBeginTime, bars.Period.BasePeriodValue);
						endOfBar	= true;
					}
					break;

				case PeriodType.Tick:
					if (tmpTime == Cbi.Globals.MinDate || bars.Period.BasePeriodValue == 1)
					{
						prevTime		= tmpTime = time;
						tmpTickCount	= bars.Period.BasePeriodValue == 1 ? 0 : 1;
						endOfBar		= bars.Period.BasePeriodValue == 1;
						break;
					}

					if (tmpTickCount < bars.Period.BasePeriodValue)
					{
						tmpTime			= time;
						endOfBar		= false;
						tmpTickCount++;
					}
					else
					{
						prevTime		= tmpTime;
						tmpTime			= time;
						endOfBar		= true;
						tmpTickCount	= 1;
					}
					break;

				case PeriodType.Week:
					if (tmpTime == Cbi.Globals.MinDate)
						prevTime = tmpTime = TimeToBarTimeWeek(time.Date, tmpTime.Date, bars.Period.BasePeriodValue);
					if (time.Date <= tmpTime.Date)
						endOfBar	= false;
					else
					{
						prevTime	= tmpTime;
						endOfBar	= true;
						tmpTime		= TimeToBarTimeWeek(time.Date, tmpTime.Date, bars.Period.BasePeriodValue);
					}
					break;

				case PeriodType.Year:
					if (tmpTime == Cbi.Globals.MinDate)
						prevTime = tmpTime = TimeToBarTimeYear(time, bars.Period.Value);
					if (time.Year <= tmpTime.Year)
						endOfBar	= false;
					else
					{
						prevTime	= tmpTime;
						endOfBar	= true;
						tmpTime		= TimeToBarTimeYear(time, bars.Period.Value);
					}
					break;
				default:
					break;
			}
			#endregion
			#region P&F logic
			double tickSize		= bars.Instrument.MasterInstrument.TickSize;
			boxSize				= Math.Floor(10000000.0 * bars.Period.Value * tickSize) / 10000000.0;
			reversalSize		= bars.Period.Value2 * boxSize;

			if (bars.Count == 0 || (IsIntraday && bars.IsNewSession(time, isRealtime)))
			{
				if (bars.Count > 0)
				{
					double		lastOpen	= bars.GetOpen(bars.Count - 1);
					double		lastHigh	= bars.GetHigh(bars.Count - 1);
					double		lastLow		= bars.GetLow(bars.Count - 1);
					double		lastClose	= bars.GetClose(bars.Count - 1);
					DateTime	lastTime	= bars.GetTime(bars.Count - 1);
					bars.LastPrice			= anchorPrice = lastClose;

					if (bars.Count == tmpCount)
						CalculatePfBar(bars, lastOpen, lastHigh, lastLow, lastClose, prevTime, lastTime, isRealtime);
				}

				AddBar(bars, close, close, close, close, tmpTime, volume, isRealtime);
				anchorPrice		= close;
				trend			= Trend.Undetermined;
				prevTime		= tmpTime;
				volumeCount		= 0;
				bars.LastPrice	= close;
				tmpCount		= bars.Count;
				tmpHigh			= high;
				tmpLow			= low;
				return;
			}

			Bar			bar		= (Bar)bars.Get(bars.Count - 1);
			double		c		= bar.Close;
			double		o		= bar.Open;
			double		h		= bar.High;
			double		l		= bar.Low;
			DateTime	t		= bar.Time;

			if (endOfBar)
			{
				CalculatePfBar(bars, o, h, l, c, prevTime, t, isRealtime);
				volumeCount		= volume;
				tmpHigh			= high;
				tmpLow			= low;
			}
			else
			{
				tmpHigh			= (high > tmpHigh ? high : tmpHigh);
				tmpLow			= (low < tmpLow ? low : tmpLow);
				volumeCount		+= volume;
			}

			bars.LastPrice		= close;
			tmpCount			= bars.Count;

			#endregion
		}

		private void CalculatePfBar(Bars bars, double o, double h, double l, double c, DateTime barTime, DateTime tTime, bool isRealtime)
		{
			if (bars.Period.PointAndFigurePriceType == PointAndFigurePriceType.Close)
			{
				switch (trend)
				{
					case Trend.Up:
						if (bars.Instrument.MasterInstrument.Compare(bars.LastPrice, anchorPrice - reversalSize) <= 0)
						{
							double newHigh	= anchorPrice - boxSize;
							double newLow	= anchorPrice - reversalSize;
							while (bars.Instrument.MasterInstrument.Compare(newLow - boxSize, bars.LastPrice) >= 0) newLow -= boxSize;
							newHigh			= bars.Instrument.MasterInstrument.Round2TickSize(newHigh);
							newLow			= bars.Instrument.MasterInstrument.Round2TickSize(newLow);
							anchorPrice		= newLow;
							trend			= Trend.Down;
							AddBar(bars, newHigh, newHigh, newLow, newLow, barTime, volumeCount, isRealtime);
						}
						else
							if (bars.Instrument.MasterInstrument.Compare(bars.LastPrice, anchorPrice + boxSize) >= 0)
							{
								double newHigh	= anchorPrice + boxSize;
								while (bars.Instrument.MasterInstrument.Compare(bars.LastPrice, newHigh + boxSize) >= 0) newHigh += boxSize;
								newHigh			= bars.Instrument.MasterInstrument.Round2TickSize(newHigh);
								anchorPrice		= newHigh;
								UpdateBar(bars, o, newHigh, l, newHigh, barTime, volumeCount, isRealtime);
							}
							else
								UpdateBar(bars, o, h, l, c, barTime, volumeCount, isRealtime);
						break;
					case Trend.Down:
						if (bars.Instrument.MasterInstrument.Compare(bars.LastPrice, anchorPrice + reversalSize) >= 0)
						{
							double newLow	= anchorPrice + boxSize;
							double newHigh	= anchorPrice + reversalSize;
							while (bars.Instrument.MasterInstrument.Compare(bars.LastPrice, newHigh + boxSize) >= 0) newHigh += boxSize;
							newHigh			= bars.Instrument.MasterInstrument.Round2TickSize(newHigh);
							newLow			= bars.Instrument.MasterInstrument.Round2TickSize(newLow);
							anchorPrice		= newHigh;
							trend			= Trend.Up;
							AddBar(bars, newLow, newHigh, newLow, newHigh, barTime, volumeCount, isRealtime);
						}
						else
							if (bars.Instrument.MasterInstrument.Compare(bars.LastPrice, anchorPrice - boxSize) <= 0)
							{
								double newLow	= anchorPrice - boxSize;
								while (bars.Instrument.MasterInstrument.Compare(newLow - boxSize, bars.LastPrice) >= 0) newLow -= boxSize;
								newLow			= bars.Instrument.MasterInstrument.Round2TickSize(newLow);
								anchorPrice		= newLow;
								UpdateBar(bars, o, h, newLow, newLow, barTime, volumeCount, isRealtime);
							}
							else
								UpdateBar(bars, o, h, l, c, barTime, volumeCount, isRealtime);
						break;
					default:
						if (bars.Instrument.MasterInstrument.Compare(bars.LastPrice, anchorPrice + boxSize) >= 0)
						{
							double newHigh	= anchorPrice + boxSize;
							while (bars.Instrument.MasterInstrument.Compare(bars.LastPrice, newHigh + boxSize) >= 0) newHigh += boxSize;
							newHigh			= bars.Instrument.MasterInstrument.Round2TickSize(newHigh);
							anchorPrice		= newHigh;
							trend			= Trend.Up;
							UpdateBar(bars, o, newHigh, l, newHigh, barTime, volumeCount, isRealtime);
						}
						else
							if (bars.Instrument.MasterInstrument.Compare(anchorPrice - boxSize, bars.LastPrice) >= 0)
							{
								double newLow	= anchorPrice - boxSize;
								while (bars.Instrument.MasterInstrument.Compare(newLow - boxSize, bars.LastPrice) >= 0) newLow -= boxSize;
								newLow			= bars.Instrument.MasterInstrument.Round2TickSize(newLow);
								anchorPrice		= newLow;
								trend			= Trend.Down;
								UpdateBar(bars, o, h, newLow, newLow, barTime, volumeCount, isRealtime);
							}
							else
								UpdateBar(bars, anchorPrice, anchorPrice, anchorPrice, anchorPrice, barTime, volumeCount, isRealtime);
						break;
				}
			}
			else
			{
				switch (trend)
				{
					case Trend.Up:
						bool updatedUp = false;
						if (bars.Instrument.MasterInstrument.Compare(tmpHigh, anchorPrice + boxSize) >= 0)
						{
							double newHigh	= anchorPrice;
							while (bars.Instrument.MasterInstrument.Compare(tmpHigh, newHigh + boxSize) >= 0) newHigh += boxSize;
							newHigh			= bars.Instrument.MasterInstrument.Round2TickSize(newHigh);
							updatedUp		= true;
							anchorPrice		= newHigh;
							long vol		= bars.Instrument.MasterInstrument.Compare(anchorPrice - reversalSize, tmpLow) >= 0 ? 0 : volumeCount;
							DateTime tt		= bars.Instrument.MasterInstrument.Compare(anchorPrice - reversalSize, tmpLow) >= 0 ? tTime : barTime;
							UpdateBar(bars, o, newHigh, l, newHigh, tt, vol, isRealtime);
						}
						if (bars.Instrument.MasterInstrument.Compare(anchorPrice - reversalSize, tmpLow) >= 0)
						{
							double newHigh	= anchorPrice - boxSize;
							double newLow	= anchorPrice - reversalSize;
							while (bars.Instrument.MasterInstrument.Compare(newLow - boxSize, tmpLow) >= 0) newLow -= boxSize;
							newHigh			= bars.Instrument.MasterInstrument.Round2TickSize(newHigh);
							newLow			= bars.Instrument.MasterInstrument.Round2TickSize(newLow);
							updatedUp		= true;
							anchorPrice		= newLow;
							trend			= Trend.Down;
							AddBar(bars, newHigh, newHigh, newLow, newLow, barTime, volumeCount, isRealtime);
						}
						if (!updatedUp)
						{
							UpdateBar(bars, o, h, l, c, barTime, volumeCount, isRealtime);
							anchorPrice = h;
						}
						break;
					case Trend.Down:
						bool updatedDn = false;
						if (bars.Instrument.MasterInstrument.Compare(tmpLow, anchorPrice - boxSize) <= 0)
						{
							double newLow	= anchorPrice;
							while (bars.Instrument.MasterInstrument.Compare(newLow - boxSize, tmpLow) >= 0) newLow -= boxSize;
							newLow			= bars.Instrument.MasterInstrument.Round2TickSize(newLow);
							updatedDn		= true;
							anchorPrice		= newLow;
							long vol		= bars.Instrument.MasterInstrument.Compare(tmpHigh, anchorPrice + reversalSize) >= 0 ? 0 : volumeCount;
							DateTime tt		= bars.Instrument.MasterInstrument.Compare(anchorPrice - reversalSize, tmpLow) >= 0 ? tTime : barTime;
							UpdateBar(bars, o, h, newLow, newLow, tt, vol, isRealtime);
						}
						if (bars.Instrument.MasterInstrument.Compare(tmpHigh, anchorPrice + reversalSize) >= 0)
						{
							double newLow	= anchorPrice + boxSize;
							double newHigh	= anchorPrice + reversalSize;
							while (bars.Instrument.MasterInstrument.Compare(tmpHigh, newHigh + boxSize) >= 0) newHigh += boxSize;
							newHigh			= bars.Instrument.MasterInstrument.Round2TickSize(newHigh);
							newLow			= bars.Instrument.MasterInstrument.Round2TickSize(newLow);
							updatedDn		= true;
							anchorPrice		= newHigh;
							trend			= Trend.Up;
							AddBar(bars, newLow, newHigh, newLow, newHigh, barTime, volumeCount, isRealtime);
						}
						if (!updatedDn)
						{
							UpdateBar(bars, o, h, l, c, barTime, volumeCount, isRealtime);
							anchorPrice = l;
						}
						break;
					default:
						if (bars.Instrument.MasterInstrument.Compare(bars.LastPrice, anchorPrice + boxSize) >= 0)
						{
							double newHigh	= anchorPrice + boxSize;
							while (bars.Instrument.MasterInstrument.Compare(bars.LastPrice, newHigh + boxSize) >= 0) newHigh += boxSize;
							newHigh			= bars.Instrument.MasterInstrument.Round2TickSize(newHigh);
							anchorPrice		= newHigh;
							trend			= Trend.Up;
							UpdateBar(bars, o, newHigh, l, newHigh, barTime, volumeCount, isRealtime);
						}
						else
							if (bars.Instrument.MasterInstrument.Compare(anchorPrice - boxSize, bars.LastPrice) >= 0)
							{
								double newLow	= anchorPrice - boxSize;
								while (bars.Instrument.MasterInstrument.Compare(newLow - boxSize, bars.LastPrice) >= 0) newLow -= boxSize;
								newLow			= bars.Instrument.MasterInstrument.Round2TickSize(newLow);
								anchorPrice		= newLow;
								trend			= Trend.Down;
								UpdateBar(bars, o, h, newLow, newLow, barTime, volumeCount, isRealtime);
							}
							else
								UpdateBar(bars, anchorPrice, anchorPrice, anchorPrice, anchorPrice, barTime, volumeCount, isRealtime);
						break;
				}
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="barsData"></param>
		public override void ApplyDefaults(Gui.Chart.BarsData barsData)
		{
			barsData.Period.Value				= 2;
			barsData.Period.Value2				= 3;
			barsData.Period.BasePeriodValue		= 4;

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
			get { return new[] { Gui.Chart.ChartStyleType.PointAndFigure }; }
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new PointAndFigureBarsType();
		}

		/// <summary>
		/// </summary>
		public PointAndFigureBarsType() : base(PeriodType.PointAndFigure) { }

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
			get { return Period.Id.ToString(); }
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

			// here is how you change the display name of the property on the properties grid
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Value", "\r\rBox size");
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Value2", "\r\rReversal");

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
				case PeriodType.Day		: return string.Format("{0} {1} PointAndFigure{2}", period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Daily" : "Day"), (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Minute	: return string.Format("{0} Min PointAndFigure{1}", period.BasePeriodValue, (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Month	: return string.Format("{0} {1} PointAndFigure{2}", period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Monthly" : "Month"), (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Second	: return string.Format("{0} {1} PointAndFigure{2}", period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Second" : "Seconds"), (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Tick	: return string.Format("{0} Tick PointAndFigure{1}", period.BasePeriodValue, (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Volume	: return string.Format("{0} Volume PointAndFigure{1}", period.BasePeriodValue, (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Week	: return string.Format("{0} {1} PointAndFigure{2}", period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Weekly" : "Weeks"), (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				case PeriodType.Year	: return string.Format("{0} {1} PointAndFigure{2}", period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Yearly" : "Years"), (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
				default					: return string.Format("{0} {1} PointAndFigure{2}", period.BasePeriodValue, BuiltFrom, (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
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

	/// <summary>
	/// </summary>
	public class RangeBarsType : BarsType
	{
		private static bool		registered		= Register(new RangeBarsType());

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
			if (bars.Count == 0 || bars.IsNewSession(time, isRealtime))
				AddBar(bars, open, high, low, close, time, volume, isRealtime);
			else
			{
				Bar			bar			= (Bar) bars.Get(bars.Count - 1); 
				double		tickSize	= bars.Instrument.MasterInstrument.TickSize;
				double		rangeValue	= Math.Floor(10000000.0 * bars.Period.Value * tickSize) / 10000000.0;

				if (bars.Instrument.MasterInstrument.Compare(close, bar.Low + rangeValue) > 0) 
				{
					bool	isFirstNewBar	= true;
					double	newClose		= bar.Low + rangeValue; // every bar closes either with high or low
                    if (bars.Instrument.MasterInstrument.Compare(bar.Close, newClose) < 0)
                        UpdateBar(bars, bar.Open, newClose, bar.Low, newClose, time, 0, isRealtime);

					// if still gap, fill with phantom bars
					double newBarOpen		= newClose + tickSize;
					while (bars.Instrument.MasterInstrument.Compare(close, newClose) > 0)
					{
						newClose			= Math.Min(close, newBarOpen + rangeValue);
						AddBar(bars, newBarOpen, newClose, newBarOpen, newClose, time, isFirstNewBar ? volume : 1, isRealtime);
						newBarOpen			= newClose + tickSize;
						isFirstNewBar		= false;
					}
				}
				else if (bars.Instrument.MasterInstrument.Compare(bar.High - rangeValue, close) > 0)
				{
					bool	isFirstNewBar	= true;
					double	newClose		= bar.High - rangeValue; // every bar closes either with high or low
                    if (bars.Instrument.MasterInstrument.Compare(bar.Close, newClose) > 0)
                        UpdateBar(bars, bar.Open, bar.High, newClose, newClose, time, 0, isRealtime);

					// if still gap, fill with phantom bars
					double newBarOpen = newClose - tickSize;
					while (bars.Instrument.MasterInstrument.Compare(newClose, close) > 0)
					{
						newClose		= Math.Max(close, newBarOpen - rangeValue);
						AddBar(bars, newBarOpen, newBarOpen, newClose, newClose, time, isFirstNewBar ? volume : 1, isRealtime);
						newBarOpen		= newClose - tickSize;
						isFirstNewBar	= false;
					}
				}
				else
					//UpdateBar(bars, open, high, low, close, time, volume, isRealtime);
					UpdateBar(bars, open, (close > bar.High ? close : bar.High), (close < bar.Low ? close : bar.Low), close, time, volume, isRealtime);
			}
			bars.LastPrice = close;
		}

		public override void ApplyDefaults(Gui.Chart.BarsData barsData)
		{
			barsData.DaysBack		= 3;
			barsData.Period.Value	= 4;
		}

		/// <summary>
		/// </summary>
		public override PeriodType BuiltFrom
		{
			get { return PeriodType.Tick; }
		}

		/// <summary>
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartDataBoxDate(DateTime time)
		{
			return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
		}

		/// <summary>
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartLabel(Gui.Chart.ChartControl chartControl, DateTime time)
		{
			return time.ToString(chartControl.LabelFormatTick, Cbi.Globals.CurrentCulture);
		}

		/// <summary>
		/// Here is how you restrict the selectable chart styles by bars type
		/// </summary>
		public override Gui.Chart.ChartStyleType[] ChartStyleTypesSupported
		{
			get { return new[] { Gui.Chart.ChartStyleType.Box, Gui.Chart.ChartStyleType.CandleStick, Gui.Chart.ChartStyleType.HiLoBars, Gui.Chart.ChartStyleType.LineOnClose, 
				Gui.Chart.ChartStyleType.OHLC, Gui.Chart.ChartStyleType.Custom0, Gui.Chart.ChartStyleType.Custom1, Gui.Chart.ChartStyleType.Custom2, Gui.Chart.ChartStyleType.Custom3,
				Gui.Chart.ChartStyleType.Custom4, Gui.Chart.ChartStyleType.Custom5, Gui.Chart.ChartStyleType.Custom6, Gui.Chart.ChartStyleType.Custom7, Gui.Chart.ChartStyleType.Custom8,
				Gui.Chart.ChartStyleType.Custom9, Gui.Chart.ChartStyleType.Final0, Gui.Chart.ChartStyleType.Final1, Gui.Chart.ChartStyleType.Final2, Gui.Chart.ChartStyleType.Final3,
				Gui.Chart.ChartStyleType.Final4 }; }
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new RangeBarsType();
		}

		/// <summary>
		/// </summary>
		public override int DefaultValue
		{ 
			get { return 4; }
		}

		/// <summary>
		/// </summary>
		public override string DisplayName
		{
			get { return Period.Id.ToString(); }
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <param name="barsBack"></param>
		/// <returns></returns>
		public override int GetInitialLookBackDays(Period period, int barsBack)
		{ 
			return 1;
		}	
			
		/// <summary>
		/// </summary>
		public override double GetPercentComplete(Bars bars, DateTime now)
		{
			throw new ApplicationException("GetPercentComplete not supported in " + DisplayName);
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
			properties.Remove(properties.Find("BasePeriodType", true));
			properties.Remove(properties.Find("BasePeriodValue", true));
			properties.Remove(properties.Find("PointAndFigurePriceType", true));
			properties.Remove(properties.Find("ReversalType", true));
			properties.Remove(properties.Find("Value2", true));

			// here is how you change the display name of the property on the properties grid
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Value", "\r\rValue");

			return properties;
		}

		/// <summary>
		/// </summary>
		public override bool IsIntraday
		{
			get { return true; }
		}

		/// <summary>
		/// </summary>
		public RangeBarsType() : base(PeriodType.Range)
		{
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <returns></returns>
		public override string ToString(Period period)
		{
			return string.Format("{0} Range{1}", period.Value, (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
		}
	}

	/// <summary>
	/// </summary>
	public class RenkoBarsType : BarsType
	{
		private static	bool	registered = Register(new RenkoBarsType());

		private			double	offset;
		private			double	renkoHigh;
		private			double	renkoLow;
		private			int		tmpCount = 0;

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
			offset = bars.Period.Value * bars.Instrument.MasterInstrument.TickSize;

			if (bars.Count < tmpCount && bars.Count > 0) // reset cache when bars are trimmed
			{
				renkoHigh	= bars.GetClose(bars.Count - 1) + offset;
				renkoLow	= bars.GetClose(bars.Count - 1) - offset;
			}

			if ((bars.Count == 0) || (bars.IsNewSession(time, isRealtime)))
			{
				if (bars.Count != 0)
				{
					// close out last bar in session and set open == close
					Bar lastBar = (Bar)bars.Get(bars.Count - 1);
					bars.RemoveLastBar(isRealtime);  // Note: bar is now just a local var and not in series!
					AddBar(bars, lastBar.Close, lastBar.Close, lastBar.Close, lastBar.Close, lastBar.Time, lastBar.Volume, isRealtime);
				}

				renkoHigh	= close + offset;
				renkoLow	= close - offset;
				AddBar(bars, close, close, close, close, time, volume, isRealtime);
				bars.LastPrice = close;
				return;
			}
			Bar bar = (Bar)bars.Get(bars.Count - 1);
			if (renkoHigh == 0 || renkoLow == 0)  //Not sure why, but happens
			{
				if (bars.Count == 1)
				{
					renkoHigh	= bar.Open + offset;
					renkoLow	= bar.Open - offset;
				}
				else if (bars.GetClose(bars.Count - 2) > bars.GetOpen(bars.Count - 2))
				{
					renkoHigh	= bars.GetClose(bars.Count - 2) + offset;
					renkoLow	= bars.GetClose(bars.Count - 2) - offset * 2;
				}
				else
				{
					renkoHigh	= bars.GetClose(bars.Count - 2) + offset * 2;
					renkoLow	= bars.GetClose(bars.Count - 2) - offset;
				}
			}
			if (bars.Instrument.MasterInstrument.Compare(close, renkoHigh) >= 0)
			{
				if (bars.Instrument.MasterInstrument.Compare(bar.Open, renkoHigh - offset) != 0
					|| bars.Instrument.MasterInstrument.Compare(bar.High, Math.Max(renkoHigh - offset, renkoHigh)) != 0
					|| bars.Instrument.MasterInstrument.Compare(bar.Low, Math.Min(renkoHigh - offset, renkoHigh)) != 0)
				{
					bars.RemoveLastBar(isRealtime);  // Note: bar is now just a local var and not in series!
					AddBar(bars, renkoHigh - offset, Math.Max(renkoHigh - offset, renkoHigh), Math.Min(renkoHigh - offset, renkoHigh), renkoHigh, time, bar.Volume + volume, isRealtime);
				}
				else
					UpdateBar(bars, renkoHigh - offset, Math.Max(renkoHigh - offset, renkoHigh), Math.Min(renkoHigh - offset, renkoHigh), renkoHigh, time, volume, isRealtime);

				renkoLow	= renkoHigh - 2.0 * offset;
				renkoHigh	= renkoHigh + offset;

				while (bars.Instrument.MasterInstrument.Compare(close, renkoHigh) >= 0)	// add empty bars to fill gap
				{
					AddBar(bars, renkoHigh - offset, Math.Max(renkoHigh - offset, renkoHigh), Math.Min(renkoHigh - offset, renkoHigh), renkoHigh, time, 1, isRealtime);
					renkoLow	= renkoHigh - 2.0 * offset;
					renkoHigh	= renkoHigh + offset;
				}

				// add final partial bar
				AddBar(bars, renkoHigh - offset, Math.Max(renkoHigh - offset, close), Math.Min(renkoHigh - offset, close), close, time, 1, isRealtime);
			}
			else
				if (bars.Instrument.MasterInstrument.Compare(close, renkoLow) <= 0)
				{
					if (bars.Instrument.MasterInstrument.Compare(bar.Open, renkoLow + offset) != 0
						|| bars.Instrument.MasterInstrument.Compare(bar.High, Math.Max(renkoLow + offset, renkoLow)) != 0
						|| bars.Instrument.MasterInstrument.Compare(bar.Low, Math.Min(renkoLow + offset, renkoLow)) != 0)
					{
						bars.RemoveLastBar(isRealtime);  // Note: bar is now just a local var and not in series!
						AddBar(bars, renkoLow + offset, Math.Max(renkoLow + offset, renkoLow), Math.Min(renkoLow + offset, renkoLow), renkoLow, time, bar.Volume + volume, isRealtime);
					}
					else
						UpdateBar(bars, renkoLow + offset, Math.Max(renkoLow + offset, renkoLow), Math.Min(renkoLow + offset, renkoLow), renkoLow, time, volume, isRealtime);

					renkoHigh	= renkoLow + 2.0 * offset;
					renkoLow	= renkoLow - offset;

					while (bars.Instrument.MasterInstrument.Compare(close, renkoLow) <= 0)	// add empty bars to fill gap
					{
						AddBar(bars, renkoLow + offset, Math.Max(renkoLow + offset, renkoLow), Math.Min(renkoLow + offset, renkoLow), renkoLow, time, 1, isRealtime);
						renkoHigh	= renkoLow + 2.0 * offset;
						renkoLow	= renkoLow - offset;
					}

					// add final partial bar
					AddBar(bars, renkoLow + offset, Math.Max(renkoLow + offset, close), Math.Min(renkoLow + offset, close), close, time, 1, isRealtime);
				}
				else    // Note: open does not really change
					UpdateBar(bars, close, close, close, close, time, volume, isRealtime);

			bars.LastPrice	= close;
			tmpCount		= bars.Count;
		}

		/// <summary>
		/// </summary>
		/// <param name="barsData"></param>
		public override void ApplyDefaults(Gui.Chart.BarsData barsData)
		{
			barsData.DaysBack = 3;
			barsData.Period.Value = 2;
		}

		/// <summary>
		/// </summary>
		public override PeriodType BuiltFrom
		{
			get { return PeriodType.Tick; }
		}

		/// <summary>
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartDataBoxDate(DateTime time)
		{
			return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
		}

		/// <summary>
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartLabel(Gui.Chart.ChartControl chartControl, DateTime time)
		{
			return time.ToString(chartControl.LabelFormatSecond, Cbi.Globals.CurrentCulture);
		}

		/// <summary>
		/// Here is how you restrict the selectable chart styles by bars type
		/// </summary>
		public override Gui.Chart.ChartStyleType[] ChartStyleTypesSupported
		{
			get { return new[] { Gui.Chart.ChartStyleType.OpenClose }; }
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new RenkoBarsType();
		}

		/// <summary>
		/// </summary>
		public override int DefaultValue
		{
			get { return 5; }
		}

		/// <summary>
		/// </summary>
		public override string DisplayName
		{
			get { return Period.Id.ToString(); }
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <param name="barsBack"></param>
		/// <returns></returns>
		public override int GetInitialLookBackDays(Period period, int barsBack)
		{
			return 1;
		}

		/// <summary>
		/// </summary>
		public override double GetPercentComplete(Bars bars, DateTime now)
		{
			throw new ApplicationException("GetPercentComplete not supported in " + DisplayName);
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
			properties.Remove(properties.Find("BasePeriodType", true));
			properties.Remove(properties.Find("BasePeriodValue", true));
			properties.Remove(properties.Find("PointAndFigurePriceType", true));
			properties.Remove(properties.Find("ReversalType", true));
			properties.Remove(properties.Find("Value2", true));

			// here is how you change the display name of the property on the properties grid
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Value", "\rBrick size");

			return properties;
		}

		/// <summary>
		/// </summary>
		public override bool IsIntraday
		{
			get { return true; }
		}

		/// <summary>
		/// </summary>
		public RenkoBarsType()
			: base(PeriodType.Renko)
		{
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <returns></returns>
		public override string ToString(Period period)
		{
			return String.Format("{0} Renko", period.Value);
		}
	}

	/// <summary>
	/// </summary>
	public class SecondBarsType : BarsType
	{
		private static bool		registered		= Register(new SecondBarsType());

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
			if (bars.Count == 0)
				AddBar(bars, open, high, low, close, TimeToBarTime(bars, time, new DateTime(bars.Session.NextBeginTime.Year, bars.Session.NextBeginTime.Month, bars.Session.NextBeginTime.Day, bars.Session.NextBeginTime.Hour, bars.Session.NextBeginTime.Minute, 0), bars.Period.Value), volume, isRealtime);
			else
			{
				if ((bars.Period.Value > 1 && time < bars.TimeLastBar) 
					|| (bars.Period.Value == 1 && time <= bars.TimeLastBar))
				{
					UpdateBar(bars, open, high, low, close, bars.TimeLastBar, volume, isRealtime);
				}
				else
				{
					time = TimeToBarTime(bars, time, bars.Session.NextBeginTime, bars.Period.Value);
					AddBar(bars, open, high, low, close, time, volume, isRealtime);
				}
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="barsData"></param>
		public override void ApplyDefaults(Gui.Chart.BarsData barsData)
		{
			barsData.DaysBack		= 3;
			barsData.Period.Value	= 30;
		}

		/// <summary>
		/// </summary>
		public override PeriodType BuiltFrom
		{
			get { return PeriodType.Tick; }
		}

		/// <summary>
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartDataBoxDate(DateTime time)
		{
			return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
		}

		/// <summary>
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartLabel(Gui.Chart.ChartControl chartControl, DateTime time)
		{
			return time.ToString(chartControl.LabelFormatSecond, Cbi.Globals.CurrentCulture);
		}

		/// <summary>
		/// Here is how you restrict the selectable chart styles by bars type
		/// </summary>
		public override Gui.Chart.ChartStyleType[] ChartStyleTypesSupported
		{
			get { return new[] { Gui.Chart.ChartStyleType.Box, Gui.Chart.ChartStyleType.CandleStick, Gui.Chart.ChartStyleType.HiLoBars, Gui.Chart.ChartStyleType.LineOnClose, 
				Gui.Chart.ChartStyleType.OHLC, Gui.Chart.ChartStyleType.Custom0, Gui.Chart.ChartStyleType.Custom1, Gui.Chart.ChartStyleType.Custom2, Gui.Chart.ChartStyleType.Custom3,
				Gui.Chart.ChartStyleType.Custom4, Gui.Chart.ChartStyleType.Custom5, Gui.Chart.ChartStyleType.Custom6, Gui.Chart.ChartStyleType.Custom7, Gui.Chart.ChartStyleType.Custom8,
				Gui.Chart.ChartStyleType.Custom9, Gui.Chart.ChartStyleType.Final0, Gui.Chart.ChartStyleType.Final1, Gui.Chart.ChartStyleType.Final2, Gui.Chart.ChartStyleType.Final3,
				Gui.Chart.ChartStyleType.Final4 }; }
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new SecondBarsType();
		}

		/// <summary>
		/// </summary>
		public override int DefaultValue
		{ 
			get { return 30; }
		}

		/// <summary>
		/// </summary>
		public override string DisplayName
		{
			get { return Period.Id.ToString(); }
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <param name="barsBack"></param>
		/// <returns></returns>
		public override int GetInitialLookBackDays(Period period, int barsBack)
		{ 
			return (int) Math.Max(1, Math.Ceiling(barsBack / Math.Max(1, 8.0 * 60 * 60 / period.Value)) * 7.0 / 5.0);	// 8 hours
		}

		/// <summary>
		/// </summary>
		public override double GetPercentComplete(Bars bars, DateTime now)
		{
			return now <= bars.TimeLastBar ? 1.0 - (bars.TimeLastBar.Subtract(now).TotalSeconds/bars.Period.Value) : 1;
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
			properties.Remove(properties.Find("BasePeriodType", true));
			properties.Remove(properties.Find("BasePeriodValue", true));
			properties.Remove(properties.Find("PointAndFigurePriceType", true));
			properties.Remove(properties.Find("ReversalType", true));
			properties.Remove(properties.Find("Value2", true));

			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Value", "\r\rValue");

			return properties;
		}

		/// <summary>
		/// </summary>
		public override bool IsIntraday
		{
			get { return true; }
		}

		/// <summary>
		/// </summary>
		public SecondBarsType() : base(PeriodType.Second)
		{
		}

		/// <summary>
		/// </summary>
		/// <param name="bars"></param>
		/// <param name="time"></param>
		/// <param name="periodStart"></param>
		/// <param name="periodValue"></param>
		/// <returns></returns>
		private static DateTime TimeToBarTime(Bars bars, DateTime time, DateTime periodStart, int periodValue)
		{
			DateTime barTimeStamp = periodStart.AddSeconds(Math.Ceiling(Math.Ceiling(Math.Max(0, time.AddSeconds(periodValue > 1 ? 1 : 0 /* sec 0 into bar 1 - n */).Subtract(periodStart).TotalSeconds)) / periodValue) * periodValue);
			if (bars.Session.SessionsOfDay.Length > 0 && barTimeStamp > bars.Session.NextEndTime)
				barTimeStamp = bars.Session.NextEndTime;
			return barTimeStamp;
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <returns></returns>
		public override string ToString(Period period)
		{
			return (period.Value == 1 ? "Second" : period.Value + " Seconds") + (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty);
		}
	}

	/// <summary>
	/// </summary>
	public class TickBarsType : BarsType
	{
		private static bool		registered		= Register(new TickBarsType());

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
			if (bars.Count == 0)
				AddBar(bars, open, high, low, close, time, volume, isRealtime);
			else
			{
				if (bars.Count > 0 && !bars.IsNewSession(time, isRealtime) && bars.Period.Value > 1 && bars.TickCount < bars.Period.Value)
					UpdateBar(bars, open, high, low, close, time, volume, isRealtime);	
				else
					AddBar(bars, open, high, low, close, time, volume, isRealtime); 
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="barsData"></param>
		public override void ApplyDefaults(Gui.Chart.BarsData barsData)
		{
			barsData.DaysBack		= 3;
			barsData.Period.Value	= 150;
		}

		/// <summary>
		/// </summary>
		public override PeriodType BuiltFrom
		{
			get { return PeriodType.Tick; }
		}

		/// <summary>
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartDataBoxDate(DateTime time)
		{
			return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
		}

		/// <summary>
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartLabel(Gui.Chart.ChartControl chartControl, DateTime time)
		{
			return time.ToString(chartControl.LabelFormatTick, Cbi.Globals.CurrentCulture);
		}

		/// <summary>
		/// Here is how you restrict the selectable chart styles by bars type
		/// </summary>
		public override Gui.Chart.ChartStyleType[] ChartStyleTypesSupported
		{
			get { return new[] { Gui.Chart.ChartStyleType.Box, Gui.Chart.ChartStyleType.CandleStick, Gui.Chart.ChartStyleType.HiLoBars, Gui.Chart.ChartStyleType.LineOnClose, 
				Gui.Chart.ChartStyleType.OHLC, Gui.Chart.ChartStyleType.Custom0, Gui.Chart.ChartStyleType.Custom1, Gui.Chart.ChartStyleType.Custom2, Gui.Chart.ChartStyleType.Custom3,
				Gui.Chart.ChartStyleType.Custom4, Gui.Chart.ChartStyleType.Custom5, Gui.Chart.ChartStyleType.Custom6, Gui.Chart.ChartStyleType.Custom7, Gui.Chart.ChartStyleType.Custom8,
				Gui.Chart.ChartStyleType.Custom9, Gui.Chart.ChartStyleType.Final0, Gui.Chart.ChartStyleType.Final1, Gui.Chart.ChartStyleType.Final2, Gui.Chart.ChartStyleType.Final3,
				Gui.Chart.ChartStyleType.Final4 }; }
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new TickBarsType();
		}

		/// <summary>
		/// </summary>
		public override int DefaultValue
		{ 
			get { return 150; }
		}

		/// <summary>
		/// </summary>
		public override string DisplayName
		{
			get { return Period.Id.ToString(); }
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <param name="barsBack"></param>
		/// <returns></returns>
		public override int GetInitialLookBackDays(Period period, int barsBack)
		{ 
			return 1;
		}
	
		/// <summary>
		/// </summary>
		public override double GetPercentComplete(Bars bars, DateTime now)
		{
			return (double) bars.TickCount / bars.Period.Value;
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
			properties.Remove(properties.Find("BasePeriodType", true));
			properties.Remove(properties.Find("BasePeriodValue", true));
			properties.Remove(properties.Find("PointAndFigurePriceType", true));
			properties.Remove(properties.Find("ReversalType", true));
			properties.Remove(properties.Find("Value2", true));

			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Value", "\r\rValue");

			return properties;
		}

		/// <summary>
		/// </summary>
		public override bool IsIntraday
		{
			get { return true; }
		}

		/// <summary>
		/// </summary>
		public TickBarsType() : base(PeriodType.Tick)
		{
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <returns></returns>
		public override string ToString(Period period)
		{
			return string.Format("{0} Tick{1}", period.Value, (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
		}
	}

	/// <summary>
	/// </summary>
	public class VolumeBarsType : BarsType
	{
		private static bool		registered		= Register(new VolumeBarsType());

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
			if (bars.Count == 0)
			{
				while (volume > bars.Period.Value)
				{
					AddBar(bars, open, high, low, close, time, bars.Period.Value, isRealtime);
					volume -= bars.Period.Value;
				}

				if (volume > 0)
					AddBar(bars, open, high, low, close, time, volume, isRealtime);
			}
			else
			{
				long volumeTmp = 0;
				if (!bars.IsNewSession(time, isRealtime))
				{
					volumeTmp = Math.Min(bars.Period.Value - bars.GetVolume(bars.Count - 1), volume);
					if (volumeTmp > 0)
						UpdateBar(bars, open, high, low, close, time, volumeTmp, isRealtime);
				}

				volumeTmp = volume - volumeTmp;
				while (volumeTmp > 0)
				{
					AddBar(bars, open, high, low, close, time, Math.Min(volumeTmp, bars.Period.Value), isRealtime);
					volumeTmp -= bars.Period.Value;
				}
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="barsData"></param>
		public override void ApplyDefaults(Gui.Chart.BarsData barsData)
		{
			barsData.DaysBack		= 3;
			barsData.Period.Value	= 1000;
		}

		/// <summary>
		/// </summary>
		public override PeriodType BuiltFrom
		{
			get { return PeriodType.Tick; }
		}

		/// <summary>
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartDataBoxDate(DateTime time)
		{
			return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
		}

		/// <summary>
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartLabel(Gui.Chart.ChartControl chartControl, DateTime time)
		{
			return time.ToString(chartControl.LabelFormatTick, Cbi.Globals.CurrentCulture);
		}

		/// <summary>
		/// Here is how you restrict the selectable chart styles by bars type
		/// </summary>
		public override Gui.Chart.ChartStyleType[] ChartStyleTypesSupported
		{
			get { return new[] { Gui.Chart.ChartStyleType.Box, Gui.Chart.ChartStyleType.CandleStick, Gui.Chart.ChartStyleType.HiLoBars, Gui.Chart.ChartStyleType.LineOnClose, 
				Gui.Chart.ChartStyleType.OHLC, Gui.Chart.ChartStyleType.Custom0, Gui.Chart.ChartStyleType.Custom1, Gui.Chart.ChartStyleType.Custom2, Gui.Chart.ChartStyleType.Custom3,
				Gui.Chart.ChartStyleType.Custom4, Gui.Chart.ChartStyleType.Custom5, Gui.Chart.ChartStyleType.Custom6, Gui.Chart.ChartStyleType.Custom7, Gui.Chart.ChartStyleType.Custom8,
				Gui.Chart.ChartStyleType.Custom9, Gui.Chart.ChartStyleType.Final0, Gui.Chart.ChartStyleType.Final1, Gui.Chart.ChartStyleType.Final2, Gui.Chart.ChartStyleType.Final3,
				Gui.Chart.ChartStyleType.Final4 }; }
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new VolumeBarsType();
		}

		/// <summary>
		/// </summary>
		public override int DefaultValue
		{ 
			get { return 1000; }
		}

		/// <summary>
		/// </summary>
		public override string DisplayName
		{
			get { return Period.Id.ToString(); }
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <param name="barsBack"></param>
		/// <returns></returns>
		public override int GetInitialLookBackDays(Period period, int barsBack)
		{ 
			return 1;
		}
	
		/// <summary>
		/// </summary>
		public override double GetPercentComplete(Bars bars, DateTime now)
		{
			return (double) bars.Get(bars.CurrentBar).Volume / bars.Period.Value;
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
			properties.Remove(properties.Find("BasePeriodType", true));
			properties.Remove(properties.Find("BasePeriodValue", true));
			properties.Remove(properties.Find("PointAndFigurePriceType", true));
			properties.Remove(properties.Find("ReversalType", true));
			properties.Remove(properties.Find("Value2", true));

			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Value", "\r\rValue");

			return properties;
		}

		/// <summary>
		/// </summary>
		public override bool IsIntraday
		{
			get { return true; }
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <returns></returns>
		public override string ToString(Period period)
		{
			return string.Format("{0} Volume{1}", period.Value, (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
		}

		/// <summary>
		/// </summary>
		public VolumeBarsType() : base(PeriodType.Volume){}
	}

	/// <summary>
	/// </summary>
	public class WeekBarsType : BarsType
	{
		private static bool		registered		= Register(new WeekBarsType());

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
			if (bars.Count == 0)
				AddBar(bars, open, high, low, close, TimeToBarTime(time, time.AddDays((6 - (((int) time.DayOfWeek + 1)%7)) + ((bars.Period.Value - 1)*7)), bars.Period.Value), volume, isRealtime);
			else if (time.Date <= bars.TimeLastBar.Date)
				UpdateBar(bars, open, high, low, close, bars.TimeLastBar, volume, isRealtime);
			else
				AddBar(bars, open, high, low, close, TimeToBarTime(time.Date, bars.TimeLastBar.Date, bars.Period.Value), volume, isRealtime);
		}

		/// <summary>
		/// </summary>
		/// <param name="barsData"></param>
		public override void ApplyDefaults(Gui.Chart.BarsData barsData)
		{
			barsData.DaysBack		= 1825;
			barsData.Period.Value	= 1;
		}

		/// <summary>
		/// </summary>
		public override PeriodType BuiltFrom
		{
			get { return PeriodType.Day; }
		}

		/// <summary>
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartDataBoxDate(DateTime time)
		{
			return string.Format("{0}/{1}", Gui.Globals.GetCalendarWeek(time), time.Year);
		}

		/// <summary>
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartLabel(Gui.Chart.ChartControl chartControl, DateTime time)
		{
			return time.ToString(chartControl.LabelFormatDay, Cbi.Globals.CurrentCulture);
		}

		/// <summary>
		/// Here is how you restrict the selectable chart styles by bars type
		/// </summary>
		public override Gui.Chart.ChartStyleType[] ChartStyleTypesSupported
		{
			get { return new[] { Gui.Chart.ChartStyleType.Box, Gui.Chart.ChartStyleType.CandleStick, Gui.Chart.ChartStyleType.HiLoBars, Gui.Chart.ChartStyleType.LineOnClose, 
				Gui.Chart.ChartStyleType.OHLC, Gui.Chart.ChartStyleType.Custom0, Gui.Chart.ChartStyleType.Custom1, Gui.Chart.ChartStyleType.Custom2, Gui.Chart.ChartStyleType.Custom3,
				Gui.Chart.ChartStyleType.Custom4, Gui.Chart.ChartStyleType.Custom5, Gui.Chart.ChartStyleType.Custom6, Gui.Chart.ChartStyleType.Custom7, Gui.Chart.ChartStyleType.Custom8,
				Gui.Chart.ChartStyleType.Custom9, Gui.Chart.ChartStyleType.Final0, Gui.Chart.ChartStyleType.Final1, Gui.Chart.ChartStyleType.Final2, Gui.Chart.ChartStyleType.Final3,
				Gui.Chart.ChartStyleType.Final4 }; }
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new WeekBarsType();
		}

		/// <summary>
		/// </summary>
		public override int DefaultValue
		{ 
			get { return 1; }
		}

		/// <summary>
		/// </summary>
		public override string DisplayName
		{
			get { return Period.Id.ToString(); }
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <param name="barsBack"></param>
		/// <returns></returns>
		public override int GetInitialLookBackDays(Period period, int barsBack)
		{ 
			return period.Value * barsBack * 7;
		}
	
		/// <summary>
		/// </summary>
		public override double GetPercentComplete(Bars bars, DateTime now)
		{
		    return now.Date <= bars.TimeLastBar.Date ? (7 - (bars.TimeLastBar.AddDays(1).Subtract(now).TotalDays/bars.Period.Value))/7 : 1;
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
			properties.Remove(properties.Find("BasePeriodType", true));
			properties.Remove(properties.Find("BasePeriodValue", true));
			properties.Remove(properties.Find("PointAndFigurePriceType", true));
			properties.Remove(properties.Find("ReversalType", true));
			properties.Remove(properties.Find("Value2", true));

			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Value", "\r\rValue");

			return properties;
		}

		/// <summary>
		/// </summary>
		public override bool IsIntraday
		{
			get { return false; }
		}

		/// <summary>
		/// </summary>
		/// <param name="time"></param>
		/// <param name="periodStart"></param>
		/// <param name="periodValue"></param>
		/// <returns></returns>
		protected override DateTime TimeToBarTime(DateTime time, DateTime periodStart, int periodValue)
		{
			return periodStart.Date.AddDays(Math.Ceiling(Math.Ceiling(time.Date.Subtract(periodStart.Date).TotalDays) / (periodValue * 7)) * (periodValue * 7)).Date;
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <returns></returns>
		public override string ToString(Period period)
		{
			return (period.Value == 1 ? "Weekly" : period.Value + " Weeks") + (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty);
		}

		/// <summary>
		/// </summary>
		public WeekBarsType() : base(PeriodType.Week)
		{
		}
	}

	/// <summary>
	/// </summary>
	public class YearBarsType : BarsType
	{
		private static bool		registered		= Register(new YearBarsType());

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
			if (bars.Count == 0)
				AddBar(bars, open, high, low, close, TimeToBarTime(time, bars.Period.Value), volume, isRealtime);
			else
			{
				if (time.Year <= bars.TimeLastBar.Year)
					UpdateBar(bars, open, high, low, close, bars.TimeLastBar, volume, isRealtime);
				else
					AddBar(bars, open, high, low, close, TimeToBarTime(time.Date, bars.Period.Value), volume, isRealtime);
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="barsData"></param>
		public override void ApplyDefaults(Gui.Chart.BarsData barsData)
		{
			barsData.DaysBack		= 15000;
			barsData.Period.Value	= 1;
		}

		/// <summary>
		/// </summary>
		public override PeriodType BuiltFrom
		{
			get { return PeriodType.Day; }
		}

		/// <summary>
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartDataBoxDate(DateTime time)
		{
			return time.Year.ToString();
		}

		/// <summary>
		/// </summary>
		/// <param name="chartControl"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public override string ChartLabel(Gui.Chart.ChartControl chartControl, DateTime time)
		{
			return time.ToString(chartControl.LabelFormatYear, Cbi.Globals.CurrentCulture);
		}

		/// <summary>
		/// Here is how you restrict the selectable chart styles by bars type
		/// </summary>
		public override Gui.Chart.ChartStyleType[] ChartStyleTypesSupported
		{
			get { return new[] { Gui.Chart.ChartStyleType.Box, Gui.Chart.ChartStyleType.CandleStick, Gui.Chart.ChartStyleType.HiLoBars, Gui.Chart.ChartStyleType.LineOnClose, 
				Gui.Chart.ChartStyleType.OHLC, Gui.Chart.ChartStyleType.Custom0, Gui.Chart.ChartStyleType.Custom1, Gui.Chart.ChartStyleType.Custom2, Gui.Chart.ChartStyleType.Custom3,
				Gui.Chart.ChartStyleType.Custom4, Gui.Chart.ChartStyleType.Custom5, Gui.Chart.ChartStyleType.Custom6, Gui.Chart.ChartStyleType.Custom7, Gui.Chart.ChartStyleType.Custom8,
				Gui.Chart.ChartStyleType.Custom9, Gui.Chart.ChartStyleType.Final0, Gui.Chart.ChartStyleType.Final1, Gui.Chart.ChartStyleType.Final2, Gui.Chart.ChartStyleType.Final3,
				Gui.Chart.ChartStyleType.Final4 }; }
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new YearBarsType();
		}

		/// <summary>
		/// </summary>
		public override int DefaultValue
		{ 
			get { return 1; }
		}

		/// <summary>
		/// </summary>
		public override string DisplayName
		{
			get { return Period.Id.ToString(); }
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <param name="barsBack"></param>
		/// <returns></returns>
		public override int GetInitialLookBackDays(Period period, int barsBack)
		{ 
			return period.Value * barsBack * 365;
		}
	
		/// <summary>
		/// </summary>
		public override double GetPercentComplete(Bars bars, DateTime now)
		{
			if (now.Date <= bars.TimeLastBar.Date)
			{							
				double daysInYear = DateTime.IsLeapYear(now.Year) ? 366 : 365;
				return (daysInYear - (bars.TimeLastBar.Date.AddDays(1).Subtract(now).TotalDays / bars.Period.Value)) / daysInYear;
			}
			return 1;
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
			properties.Remove(properties.Find("BasePeriodType", true));
			properties.Remove(properties.Find("BasePeriodValue", true));
			properties.Remove(properties.Find("PointAndFigurePriceType", true));
			properties.Remove(properties.Find("ReversalType", true));
			properties.Remove(properties.Find("Value2", true));

			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Value", "\r\rValue");

			return properties;
		}

		/// <summary>
		/// </summary>
		public override bool IsIntraday
		{
			get { return false; }
		}

		private static DateTime TimeToBarTime(DateTime time, int periodValue)
		{
			DateTime result = new DateTime(time.Year, 1, 1); 
			for (int i = 0; i < periodValue; i++)
				result = result.AddYears(1);

			return result.AddDays(-1);
		}

		/// <summary>
		/// </summary>
		/// <param name="period"></param>
		/// <returns></returns>
		public override string ToString(Period period)
		{
			return (period.Value == 1 ? "Yearly" : period.Value + " Years") + (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty);
		}

		/// <summary>
		/// </summary>
		public YearBarsType() : base(PeriodType.Year){}
	}
}
