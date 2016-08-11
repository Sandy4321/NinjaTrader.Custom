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
    /// MT Double Shot Crossover Demo
    /// </summary>
    [Description("MT Double Shot Crossover Demo shows how to use the derived custom strategy MTDoubleShotStrategy:MTDoubleShotStrategyBase class strcture - www.microtrends.co")]
    public class MTDoubleShotCrossoverDemo : MTDoubleShotStrategy
    {
        #region Variables
        // Wizard generated variables
        private EMA eMA = null;
		private SMA sMA = null;
		private WMA wMA = null;
        // User defined variables (add any user defined variables below)
		
        #endregion
		#region Events
        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			
			try
			{
				base.Initialize(); 
				#region instantiate indicator references
				eMA=_indicator.EMA(this.EMAPeriod); 
				eMA.Plots[0].Pen.Color=Color.Red;
				eMA.Plots[0].Pen.Width=2;  
				sMA=_indicator.SMA(this.SMAPeriod); 
				sMA.Plots[0].Pen.Color=Color.Blue;
				sMA.Plots[0].Pen.Width=3;  
				wMA=_indicator.WMA(this.WMAPeriod); 
				wMA.Plots[0].Pen.Color=Color.Gray;
				wMA.Plots[0].Pen.Width=4;  
				#endregion
				#region add indicators to chart 
				bool addEMA =(TradeSetupMode & 1)==1 ||  (TradeSetupMode & 2)==2  || this.TrailStopType==4;
				bool addSMA = (TradeSetupMode & 2)==2 || (TradeSetupMode & 4)==4 || this.TrailStopType==5;
				bool addWMA = (TradeSetupMode & 4)==4 || this.TrailStopType==6;
				
				#region GraphicsMode
				if(GraphicsMode) 
				{
					mTDoubleShotStrategyInfoBar=_indicator.MTDoubleShotStrategyInfoBar();
					mTDoubleShotStrategyVisualiser=_indicator.MTDoubleShotStrategyVisualiser();
					Add(mTDoubleShotStrategyInfoBar); 
					Add(mTDoubleShotStrategyVisualiser);		
					
					if(addEMA)
					{
						Add(eMA);
					}
					if(addSMA)
					{
						Add(sMA);
					}
					if(addWMA)
					{
						Add(wMA);
					}
				}
				#endregion
				#endregion
			} 
			catch(Exception ex)
			{
				Print(ex.ToString());
				Log(ex.ToString(),LogLevel.Alert);
			}
		}
		
		  /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			try
			{
				#region custom trailStop set
				if(this.TrailStopType==4)
				{
					base.TrailStopCustomPrice=eMA[0]; 
				}
				else if(this.TrailStopType==5)
				{
					base.TrailStopCustomPrice=sMA[0]; 
				}
				else if(this.TrailStopType==6)
				{
					base.TrailStopCustomPrice=wMA[0]; 
				}
				#endregion
				base.OnBarUpdate();
			}
			catch(Exception ex)
			{
				Print(ex.ToString()); 
			}
			
        }

		
		
		
		#endregion
		#region trade signal entry exit logic
		/// <summary>
		/// TradeEntryValidate override to change entry rules, uses bitwise 1,2,4,8,16,32,64,128,256,512,1024,2048 etc
		/// </summary>
		/// <param name="isLong"></param>
		/// <returns></returns>
		public override bool  TradeEntryValidate(bool isLong)
		{
			//Print("TradeEntryValidate");
			bool result =TradeSetupMode>0;
			if(isLong)
			{
				if(result && (TradeSetupMode & 1)==1)
				{
					result= CrossAbove(Close,eMA,1);
				}
				if(result && (TradeSetupMode & 2)==2)
				{
					result=Rising(eMA);
				}
				if(result && (TradeSetupMode & 4)==4)
				{
					result= CrossAbove(eMA,sMA,1);
				}
				if(result && (TradeSetupMode & 8)==8)
				{
					result= eMA[0]>sMA[0];
				}
				if(result && (TradeSetupMode & 16)==16)
				{
					result= CrossAbove(eMA,wMA,1);
				}
				if(result && (TradeSetupMode & 32)==32)
				{
					result= eMA[0]>wMA[0];
				}
				if(result && (TradeSetupMode & 64)==64)
				{
					result= CrossAbove(sMA,wMA,1);
				}
				if(result && (TradeSetupMode & 128)==128)
				{
					result=Rising(sMA);
				}
				if(result && (TradeSetupMode & 256)==256)
				{
					result= sMA[0]>wMA[0];
				}
				if(result && (TradeSetupMode & 512)==512)
				{
					result= Rising(wMA);
				}				
				
			}
			else
			{
				
				if(result && (TradeSetupMode & 1)==1)
				{
					result= CrossBelow(Close,eMA,1);
				}
				if(result && (TradeSetupMode & 2)==2)
				{
					result=Falling(eMA);
				}
				if(result && (TradeSetupMode & 4)==4)
				{
					result= CrossBelow(eMA,sMA,1);
				}
				if(result && (TradeSetupMode & 8)==8)
				{
					result= eMA[0]<sMA[0];
				}
				if(result && (TradeSetupMode & 16)==16)
				{
					result= CrossBelow(eMA,wMA,1);
				}
				if(result && (TradeSetupMode & 32)==32)
				{
					result= eMA[0]<wMA[0];
				}
				if(result && (TradeSetupMode & 64)==64)
				{
					result= CrossBelow(sMA,wMA,1);
				}
				if(result && (TradeSetupMode & 128)==128)
				{
					result=Falling(sMA);
				}
				if(result && (TradeSetupMode & 256)==256)
				{
					result= sMA[0]<wMA[0];
				}
				if(result && (TradeSetupMode & 512)==512)
				{
					result= Falling(wMA);
				}
			}
			return result;
		}		
		
		/// <summary>
		/// TradeFilterValidate override to change entry rules uses bitwise 1,2,4,8,16,32,64,128,256,512,1024,2048 etc
		/// </summary>
		/// <param name="isLong"></param>
		/// <returns></returns>
		public virtual bool TradeFilterValidate(bool isLong)
		{
			bool result =TradeSetupMode>0;
			//use bitwise 1,2,4,8,16,32,64,128,256,512,1024,2048 etc
			if(isLong)
			{
				if(result && (TradeSetupMode & 1)==1)
				{
					result=Rising(eMA);
				}
				if(result && (TradeSetupMode & 2)==2)
				{
					result=Rising(sMA);
				}
				if(result && (TradeSetupMode & 4)==4)
				{
					result= Rising(wMA);
				}				
				
			}
		else
			{
				
				if(result && (TradeSetupMode & 1)==1)
				{
					result=Falling(eMA);
				}
				if(result && (TradeSetupMode & 2)==2)
				{
					result=Falling(sMA);
				}
				if(result && (TradeSetupMode & 4)==4)
				{
					result= Falling(wMA);
				}				
				
			}
			
			return result;
		}		
		
		/// <summary>
		/// TradeExitValidate override to change exit rules, also hide the TradeExitValidate Property using the new keyword in any child class
		/// </summary>
		/// <returns></returns>
		public override bool TradeExitValidate()
		{
			bool result = base.TradeExitValidate();
			if(TradeExitModeBehaviour==0 && result)return true;
						
			//some other rule etc...
			return result;
		}			
		#endregion
        #region Properties
       
		#region  base class properties
		//note the usage of the new keyword
		
		[Description("Trade Setup Mode - Bitwise Flags 1 to 512 Integers 0 to 1023 - Logical AND - 0:off,1:Price Crossover fast ,2:Rising/falling fast,4: fast Crossover slow ,8:fast above/below slow, 16:fast Crossover Triple ,32:fast above/below triple, 64:slow crossover triple, 128:rising/falling slow , 256: slow above/below triple , 512:rsing/failling triple  - various combinations for demo purposes   - Hide to overide this property with the new keyword... Add flags together to switch on combinations - all=1023, e.g 1 + 4 = 5 - so you enter 5")]
        [GridCategory(" Signals")]
        [Gui.Design.DisplayNameAttribute("Trade Setup & Entry Mode")]
        public new int TradeSetupMode
        {
            get { return base.TradeSetupMode; }
            set { base.TradeSetupMode = Math.Max(0, Math.Min(1023, value)); }
        }
		
		[Description("Trade Filter Mode - Bitwise Flags 1 to 4 Integers 0 to 7 - Logical AND  1:Rising/Failling EMA,  2:Rising/Failling SMA, 4:Rising/Failling WMA")]
        [GridCategory(" Trade Rules")]
        [Gui.Design.DisplayNameAttribute("Trade Filter Mode")]
        public new int TradeFilterMode
        {
            get { return base.TradeFilterMode; }
            set { base.TradeFilterMode = value; }
        }
		
		[Description("Trail Stop Mode 0:Off, 1 Low/High 2:Prev Low/High, 3:Price, 4:Custom EMA, 5:Custom SMA, 6:Custom WMA")]
        [GridCategory("Strategy ATM Risk Man. Trailing Stops")]
		[Gui.Design.DisplayNameAttribute("Trailing Stop Type")]
        public new int TrailStopType
        {
            get { return base.TrailStopType; }
            set {  
					base.TrailStopType = Math.Max(0, Math.Min(6, value)); 
				}
        }
		#endregion
		#region Indciators
		
		
		private int	eMAPeriod=10;
		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		public int EMAPeriod
		{
			get { return eMAPeriod; }
			set { eMAPeriod = Math.Max(1, value); }
		}
		
		private int	sMAPeriod=20;
		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		public int SMAPeriod
		{
			get { return sMAPeriod; }
			set { sMAPeriod = Math.Max(1, value); }
		}
		
		private int	wMAPeriod=89;
		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		public int WMAPeriod
		{
			get { return wMAPeriod; }
			set { wMAPeriod = Math.Max(1, value); }
		}
		#endregion
		
        #endregion
    }
}
