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

namespace NinjaTrader.Indicator
{
	[Description("Custom Session Access")]
    public class CustomSessionAccess : Indicator
    {
        #region Variables
			private string _Template = "";
		    private DateTime sessionBegin;
			private DateTime sessionEnd;

			public class SessionTemplateConverter : StringConverter 
			{	
				public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
				public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)	{ return false; }
				public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
				
				{	string[] sarr = new string[0];
					foreach (Session data in NinjaTrader.Db.Globals.Engine.GetObjectSet(typeof(Session), ""))
					{	
						Array.Resize<string>(ref sarr, sarr.Length+1);	
						sarr[sarr.Length-1] = data.TemplateName;
					}	
					return new StandardValuesCollection( sarr );
				}	
			}
			#endregion

        protected override void Initialize()
        {
            Overlay				= true;
		}

        protected override void OnStartUp()
		{
			if (_Template == "")
				DrawTextFixed("tag", "Please first select a session", TextPosition.BottomRight);
		}
		
        protected override void OnBarUpdate()
        {
			if (Bars.FirstBarOfSession)
			{
				foreach (Session data in NinjaTrader.Db.Globals.Engine.GetObjectSet(typeof(Session), ""))
				{
					if (data.TemplateName.CompareTo(_Template) == 0)
					{
						data.GetNextBeginEnd(BarsArray[0], 0, out sessionBegin, out sessionEnd);
						DrawTextFixed("tag", _Template + ": " + sessionBegin + " " + sessionEnd, TextPosition.BottomRight);
					}
				}
			}
		}

        #region Properties
		
		[Description("Select Sessions")]
		[GridCategory("Custom Session Select")]
		[TypeConverter(typeof(SessionTemplateConverter))]
		public string Template				
		{	
			get { return _Template; }	
			set {_Template = value; }
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
        private CustomSessionAccess[] cacheCustomSessionAccess = null;

        private static CustomSessionAccess checkCustomSessionAccess = new CustomSessionAccess();

        /// <summary>
        /// Custom Session Access
        /// </summary>
        /// <returns></returns>
        public CustomSessionAccess CustomSessionAccess(string template)
        {
            return CustomSessionAccess(Input, template);
        }

        /// <summary>
        /// Custom Session Access
        /// </summary>
        /// <returns></returns>
        public CustomSessionAccess CustomSessionAccess(Data.IDataSeries input, string template)
        {
            if (cacheCustomSessionAccess != null)
                for (int idx = 0; idx < cacheCustomSessionAccess.Length; idx++)
                    if (cacheCustomSessionAccess[idx].Template == template && cacheCustomSessionAccess[idx].EqualsInput(input))
                        return cacheCustomSessionAccess[idx];

            lock (checkCustomSessionAccess)
            {
                checkCustomSessionAccess.Template = template;
                template = checkCustomSessionAccess.Template;

                if (cacheCustomSessionAccess != null)
                    for (int idx = 0; idx < cacheCustomSessionAccess.Length; idx++)
                        if (cacheCustomSessionAccess[idx].Template == template && cacheCustomSessionAccess[idx].EqualsInput(input))
                            return cacheCustomSessionAccess[idx];

                CustomSessionAccess indicator = new CustomSessionAccess();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Template = template;
                Indicators.Add(indicator);
                indicator.SetUp();

                CustomSessionAccess[] tmp = new CustomSessionAccess[cacheCustomSessionAccess == null ? 1 : cacheCustomSessionAccess.Length + 1];
                if (cacheCustomSessionAccess != null)
                    cacheCustomSessionAccess.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheCustomSessionAccess = tmp;
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
        /// Custom Session Access
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.CustomSessionAccess CustomSessionAccess(string template)
        {
            return _indicator.CustomSessionAccess(Input, template);
        }

        /// <summary>
        /// Custom Session Access
        /// </summary>
        /// <returns></returns>
        public Indicator.CustomSessionAccess CustomSessionAccess(Data.IDataSeries input, string template)
        {
            return _indicator.CustomSessionAccess(input, template);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Custom Session Access
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.CustomSessionAccess CustomSessionAccess(string template)
        {
            return _indicator.CustomSessionAccess(Input, template);
        }

        /// <summary>
        /// Custom Session Access
        /// </summary>
        /// <returns></returns>
        public Indicator.CustomSessionAccess CustomSessionAccess(Data.IDataSeries input, string template)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.CustomSessionAccess(input, template);
        }
    }
}
#endregion
