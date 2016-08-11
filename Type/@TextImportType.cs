// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
//
#region Using declarations
using System;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NinjaTrader.Cbi;
#endregion

// This namespace holds all import types and is required. Do not change it.
namespace NinjaTrader.Data
{
	/// <summary>
	/// </summary>
	[Gui.Design.DisplayName("NinjaTrader (timestamps in import file(s) represent end of bar time)")]
	public class TextImportType : ImportType
	{
		private	CultureInfo		cultureInfo;
		private	int				currentInstrumentIdx	= -1;
		private	bool			endOfBarTimestamps		= true;					// NinjaTrader standard	
		private string[]		fileNames;
		private	bool			firstLine				= true;
		private	char[]			quotes					= new char[] { '"' };
		private	StreamReader	reader;
		private	Regex			regex;
		private	string			separator				= string.Empty;

		/// <summary>
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			if (reader != null)
				try		{ reader.Close(); }
				catch	{}
			reader = null;
		}

		/// <summary>
		/// NinjaTrader as default timestamps bars at the end of spanned period.
		/// </summary>
		public bool EndOfBarTimestamps
		{
			get { return endOfBarTimestamps; }
			set { endOfBarTimestamps = value; }
		}

		/// <summary>
		/// </summary>
		public string[] FileNames
		{
			get { return fileNames; }
			set { fileNames = value; }
		}

		/// <summary>
		/// This method is used to configure the import type and is called once before any data is imported
		/// </summary>
		public override bool Initialize()
		{
			OpenFileDialog openDialog	= new OpenFileDialog();
			openDialog.InitialDirectory	= LastImportDirectory;
			openDialog.RestoreDirectory	= true;
			openDialog.Title			= "Import Historical Data";
			openDialog.Filter			= "Any|*.*";
			openDialog.Multiselect		= true;

			if (openDialog.ShowDialog() != DialogResult.OK || openDialog.FileNames.Length == 0)
				return false;
			LastImportDirectory		= new FileInfo(openDialog.FileNames[0]).Directory.FullName;

			currentInstrumentIdx	= -1;
			fileNames				= openDialog.FileNames;
			return true;
		}

		/// <summary>
		/// Read the next line of data to import.
		/// </summary>
		/// <returns>TRUE: line read successfully. FALSE: stop reading data from that file, move to next import file.</returns>
		public override bool NextDataPoint()
		{
			if (reader == null)
				return false;

			while (true)
			{
				CurrentData = reader.ReadLine();
				if (CurrentData == null)
				{
					try		{ reader.Close(); }
					catch	{};
					reader = null;
					return false;
				}

				CurrentData = CurrentData.Trim();
				if (CurrentData.Length == 0)
					continue;

				if (firstLine)
				{
					separator = string.Empty;
					foreach (string separatorTmp in new string[] { ";", "," })
						if (new Regex(separatorTmp + "\"?[^" + separatorTmp + "]+\"?" + separatorTmp).Match(CurrentData).Success)
						{
							separator = separatorTmp;
							break;
						}

					if (separator.Length == 0)
					{
						Cbi.LogEventArgs.ProcessEventArgs(new Cbi.LogEventArgs(Instrument.FullName + ": Import field separator could not be identified.", Cbi.LogLevel.Error));
						try		{ reader.Close(); }
						catch	{};
						reader = null;
						throw new ApplicationException();
					}

					regex = new Regex("\"?[^" + separator + "]+\"?");
					firstLine = false;
				}
	
				MatchCollection matches = regex.Matches(CurrentData);
				if (matches.Count == 0)				// skip to next lines if current line is empty
					continue;

				// skip to next line if first char is not a digit
				string timeField = matches[0].Value.Trim(quotes).Trim().Replace("-", string.Empty).Replace(":", string.Empty).Replace(" ", string.Empty);
				if (timeField.Length == 0 || !char.IsDigit(timeField[0]))
					continue;

				if (matches.Count < 6 && matches.Count != 3)
				{
					Cbi.LogEventArgs.ProcessEventArgs(new Cbi.LogEventArgs(Instrument.FullName + ": Unexpected number of fields in line '" + (DataPoints + 1) + "', should be 6 or 3", Cbi.LogLevel.Error));
					try		{ reader.Close(); }
					catch	{};
					reader = null;
					throw new ApplicationException();
				}

				// set actual PeriodType on reading the first line of data
				if (DataPoints == 0)
				{
					if (matches.Count >= 6 && timeField.Length == 8)
						PeriodType = Data.PeriodType.Day;
					else if (matches.Count >= 6 && (timeField.Length == 12 || timeField.Length == 14))
						PeriodType = Data.PeriodType.Minute;
					else if (matches.Count == 3 && timeField.Length == 14)
						PeriodType = Data.PeriodType.Tick;
				}

				Time = Cbi.Globals.MinDate;
				try
				{
					if (PeriodType == Data.PeriodType.Day)
						Time = new DateTime (Convert.ToInt32(timeField.Substring(0, 4)),
												Convert.ToInt32(timeField.Substring(4, 2)),
												Convert.ToInt32(timeField.Substring(6, 2)));
					else if (PeriodType == Data.PeriodType.Minute)
					{
						Time = new DateTime (Convert.ToInt32(timeField.Substring(0, 4)),
												Convert.ToInt32(timeField.Substring(4, 2)),
												Convert.ToInt32(timeField.Substring(6, 2)),
												Convert.ToInt32(timeField.Substring(8, 2)),
												Convert.ToInt32(timeField.Substring(10, 2)),
												0);
						if (!EndOfBarTimestamps)
							Time = Time.AddMinutes(1);
					}
					else if (PeriodType == Data.PeriodType.Tick)
						Time = new DateTime (Convert.ToInt32(timeField.Substring(0, 4)),
												Convert.ToInt32(timeField.Substring(4, 2)),
												Convert.ToInt32(timeField.Substring(6, 2)),
												Convert.ToInt32(timeField.Substring(8, 2)),
												Convert.ToInt32(timeField.Substring(10, 2)),
												Convert.ToInt32(timeField.Substring(12, 2)));
				}
				catch (Exception exp)
				{
					Cbi.LogEventArgs.ProcessEventArgs(new Cbi.LogEventArgs(Instrument.FullName + ": Date/Time format error in line " + (DataPoints + 1) + ": " 
						+ exp.Message + ": '" + CurrentData + "'", Cbi.LogLevel.Error));
					try		{ reader.Close(); }
					catch	{};
					reader = null;
					throw new ApplicationException();
				}

				if (cultureInfo == null)
				{
					ArrayList	cultureInfos	= new ArrayList();
					CultureInfo	tmp				= null;
					try     { tmp = new CultureInfo("en-US"); cultureInfos.Add(tmp); }
					catch	{};
					try     { tmp = (CultureInfo) Cbi.Globals.CurrentCulture.Clone(); cultureInfos.Add(tmp); }
					catch	{};
					try     { tmp = new CultureInfo("de-DE"); cultureInfos.Add(tmp); }
					catch	{};

					foreach (CultureInfo cultureInfoTmp in cultureInfos)
					{
						// turn of number grouping, since the number grouping character could be a decimal separator for a different culture
						cultureInfoTmp.NumberFormat.NumberGroupSeparator = string.Empty;

						try
						{
							Open		= Convert.ToDouble(matches[1].Value.Trim(quotes).Trim(), cultureInfoTmp);
							cultureInfo = cultureInfoTmp;
							break;
						}
						catch {}
					}

					if (cultureInfo == null)
					{
						Cbi.LogEventArgs.ProcessEventArgs(new Cbi.LogEventArgs(Instrument.FullName + ": Numeric price format not supported.", Cbi.LogLevel.Error));
						try		{ reader.Close(); }
						catch	{};
						reader = null;
						throw new ApplicationException();
					}
				}

				try
				{
					Open	= Convert.ToDouble(matches[1].Value.Trim(quotes).Trim(), cultureInfo);
					High	= (PeriodType == Data.PeriodType.Tick ? Open : Convert.ToDouble(matches[2].Value.Trim(quotes).Trim(), cultureInfo));
					Low		= (PeriodType == Data.PeriodType.Tick ? Open : Convert.ToDouble(matches[3].Value.Trim(quotes).Trim(), cultureInfo));
					Close	= (PeriodType == Data.PeriodType.Tick ? Open : Convert.ToDouble(matches[4].Value.Trim(quotes).Trim(), cultureInfo));
					Volume	= Convert.ToInt64(matches[PeriodType == Data.PeriodType.Tick ? 2 : 5].Value.Trim(quotes).Trim(), cultureInfo);
					return true;
				}
				catch (Exception exp)
				{
					Cbi.LogEventArgs.ProcessEventArgs(new Cbi.LogEventArgs(Instrument.FullName + ": Format error in line " + (DataPoints + 1) + ": " 
						+ exp.Message + ": '" + CurrentData + "'", Cbi.LogLevel.Error));
					try		{ reader.Close(); }
					catch	{};
					reader = null;
					throw new ApplicationException();
				}

				// keep going until finally 1 line of data was read
			}
		}

		/// <summary>
		/// Sets up the next instrument for importing
		/// </summary>
		/// <returns>TRUE: continue processing. Else, abort whole import.</returns>
		public override bool NextInstrument()
		{
			if (fileNames == null || ++currentInstrumentIdx >= fileNames.Length)
				return false;

			// Default setting for the instrument
			// If this methods returns and Instrument would be null, then NT would just skip this instrument and try the next one.
			// Thus erroneous instruments could be skipped without aborted the import at all.
			Instrument = null;

			System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileNames[currentInstrumentIdx]);

			try
			{
				reader = new StreamReader(fileNames[currentInstrumentIdx]);

				string instrumentName = fileInfo.Name.Replace("." + Data.MarketDataType.Last + ".", ".");
				instrumentName = instrumentName.Replace("." + Data.MarketDataType.Ask + ".", ".");
				instrumentName = instrumentName.Replace("." + Data.MarketDataType.Bid + ".", ".");
				Instrument = Data.ExternalAdapter.GetInstrument(fileInfo.Extension.Length == 4 && instrumentName.Length > fileInfo.Extension.Length
												? instrumentName.Substring(0, instrumentName.Length - fileInfo.Extension.Length) : instrumentName);
			}
			catch (Exception exp)
			{
				Cbi.LogEventArgs.ProcessEventArgs(new Cbi.LogEventArgs("Unable to read data from file '" + fileNames[currentInstrumentIdx] + "': " + exp.Message, Cbi.LogLevel.Error));
				return true;
			}

			if (Instrument == null)
			{
				Cbi.LogEventArgs.ProcessEventArgs(new Cbi.LogEventArgs("Unable to import file '" + fileNames[currentInstrumentIdx] + "'. Instrument is not supported by repository.", Cbi.LogLevel.Error));
				return true;
			}

			cultureInfo	= null;
			firstLine	= true;

			return true;
		}
	}

	/// <summary>
	/// </summary>
	[Gui.Design.DisplayName("NinjaTrader (timestamps in import file(s) represent start of bar time)")]
	public class TextAltImportType : TextImportType
	{
		/// <summary>
		/// </summary>
		public TextAltImportType()
		{
			EndOfBarTimestamps = false;
		}
	}
}
