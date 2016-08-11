using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using NinjaTrader.Indicator;
using NinjaTrader.Data;
using NinjaTrader.Cbi;

namespace t4t
{	
	partial interface ISymbolManager : IDisposable
	{
		void Initialize(string instrName);
	}
		
	[System.AttributeUsage(System.AttributeTargets.Property)]
	public class SpecificTo : System.Attribute
	{
		public string[] Name;

		public SpecificTo(params string[] param)
		{
			Name = param;
		}

        public SpecificTo(string param)
        {
            Name = new string[]{param};
        }
	}

	static class SymbolManagerList
	{
		public static List<string> Name = new List<string>();
        public static List<Instrument> Inst = new List<Instrument>();
 
		static SymbolManagerList()
		{
            NinjaTrader.Cbi.InstrumentList InstList = NinjaTrader.Cbi.InstrumentList.GetObject("Default");

            foreach (Instrument inst in InstList.Instruments)
            {
                Name.Add(inst.FullName);
                Inst.Add(inst);
			}
		}
	}

	public class SymbolManagerConverter : TypeConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			StandardValuesCollection cols = new StandardValuesCollection(t4t.SymbolManagerList.Name);
			return cols;
		}
	}

}

