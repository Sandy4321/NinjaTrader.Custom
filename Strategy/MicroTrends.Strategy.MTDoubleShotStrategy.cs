// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

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
using NinjaTrader.Strategy;
#endregion


// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
	/// <summary>
	/// </summary>
	abstract public partial class MTDoubleShotStrategy : MTDoubleShotStrategyBase
	{
		
		protected NinjaTrader.Indicator.MTDoubleShotStrategyInfoBar mTDoubleShotStrategyInfoBar = null;
		protected NinjaTrader.Indicator.MTDoubleShotStrategyVisualiser mTDoubleShotStrategyVisualiser = null;
		protected NinjaTrader.Indicator.ADX _indicator = new NinjaTrader.Indicator.ADX();

		/// <summary>
		/// </summary>
		public MTDoubleShotStrategy()
		{
			_indicator.Input	= Input;
			_indicator.Strategy	= this;
		}

		/// <summary>
		/// </summary>
		public override int BarsRequired
		{
			get { return base.BarsRequired; }
			set { base.BarsRequired = _indicator.BarsRequired = value; }
		}

		/// <summary>
		/// </summary>
		public override bool CalculateOnBarClose
		{
			get { return base.CalculateOnBarClose; }
			set { base.CalculateOnBarClose = _indicator.CalculateOnBarClose = value; }
		}
		
		/// <summary>
		/// </summary>
		public override void Dispose()
		{
			_indicator.Dispose();
			base.Dispose();
		}

		/// <summary>
		/// </summary>
		public override bool ForceMaximumBarsLookBack256
		{
			get { return base.ForceMaximumBarsLookBack256; }
			set { base.ForceMaximumBarsLookBack256 = _indicator.ForceMaximumBarsLookBack256 = value; }
		}

		/// <summary>
		/// </summary>
		public override IndicatorBase LeadIndicator
		{
			get	{ return _indicator; }
		}

		/// <summary>
		/// </summary>
		public override Data.MaximumBarsLookBack MaximumBarsLookBack
		{
			get { return base.MaximumBarsLookBack; }
			set { base.MaximumBarsLookBack = _indicator.MaximumBarsLookBack = value; }
		}
		
		
		protected override void OnPositionUpdate(IPosition position)
		{
			mTDoubleShotStrategyVisualiser.Position=position;
			base.OnPositionUpdate(position);
		}
		
		
		
		
		public override void SetStatsAndVisuals()
		{
			
			mTDoubleShotStrategyInfoBar.Value.Set(Performance.AllTrades.TradesPerformance.Currency.CumProfit);    
			mTDoubleShotStrategyInfoBar.Position=this.Position;  
			//visuals
			if(Historical)
			{
				if(orderStop1!=null && (orderStop1.OrderState==OrderState.Accepted ||  orderStop1.OrderState==OrderState.Working))
				{
					mTDoubleShotStrategyVisualiser.StopLoss1.Set(orderStop1.StopPrice);   
				}
				
				if(orderStop2!=null && (orderStop2.OrderState==OrderState.Accepted ||  orderStop2.OrderState==OrderState.Working))
				{
					mTDoubleShotStrategyVisualiser.StopLoss2.Set(orderStop2.StopPrice);   
				}
				
				if(orderTarget1!=null && (orderTarget1.OrderState==OrderState.Accepted ||  orderTarget1.OrderState==OrderState.Working))
				{
					mTDoubleShotStrategyVisualiser.Target1.Set(orderTarget1.LimitPrice);   
				}
				
				if(orderTarget2!=null && (orderTarget2.OrderState==OrderState.Accepted ||  orderTarget2.OrderState==OrderState.Working))
				{
					mTDoubleShotStrategyVisualiser.Target2.Set(orderTarget2.LimitPrice);   
				}
				return;
			}
			
			if(orderStop1!=null && (orderStop1.OrderState==OrderState.Accepted ||  orderStop1.OrderState==OrderState.Working))
			{
				mTDoubleShotStrategyVisualiser.StopLoss1.Set(orderStop1.StopPrice);   
			}
			else
			{
				mTDoubleShotStrategyVisualiser.StopLoss1.Reset();   
			}
			
			if(orderStop2!=null && (orderStop2.OrderState==OrderState.Accepted ||  orderStop2.OrderState==OrderState.Working))
			{
				mTDoubleShotStrategyVisualiser.StopLoss2.Set(orderStop2.StopPrice);   
			}
			else
			{
				mTDoubleShotStrategyVisualiser.StopLoss2.Reset();   
			}
			
			if(orderTarget1!=null && (orderTarget1.OrderState==OrderState.Accepted ||  orderTarget1.OrderState==OrderState.Working))
			{
				mTDoubleShotStrategyVisualiser.Target1.Set(orderTarget1.LimitPrice);   
			}
			else
			{
				mTDoubleShotStrategyVisualiser.Target1.Reset();   
			}
			
			if(orderTarget2!=null && (orderTarget2.OrderState==OrderState.Accepted ||  orderTarget2.OrderState==OrderState.Working))
			{
				mTDoubleShotStrategyVisualiser.Target2.Set(orderTarget2.LimitPrice);   
			}
			else
			{
				mTDoubleShotStrategyVisualiser.Target2.Reset();   
			}
			
			
			
			
			
		}
		
		
		public override void SetEntryOrderVisuals(bool isLong, double price)
		{
			if(isLong)
			{
				mTDoubleShotStrategyVisualiser.BuyOrder.Set(price);   
			}
			else
			{
				mTDoubleShotStrategyVisualiser.SellOrder.Set(price); 
			}
		}
		
		
	}
}
			
