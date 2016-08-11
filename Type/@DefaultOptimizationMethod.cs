// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
//
#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
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
	[Gui.Design.DisplayName("Default")]
	public class DefaultOptimizationMethod : OptimizationMethod
	{
		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			DefaultOptimizationMethod ret = new DefaultOptimizationMethod();
			ret.Parameters = (ParameterCollection) Parameters.Clone();

			return ret;
		}

		/// <summary>
		/// </summary>
		public override void Dispose()
		{
			// don't forget to call Dispose() on the base class before any custom handling
			base.Dispose();
		}

		/// <summary>
		/// Called before Optimize() to setup the optimizer.
		/// Always called in main thread which would allow for putting additional GUI in here.
		/// </summary>
		public override void Initialize()
		{
			// don't forget to call Initialize() on the base class before any custom handling
			base.Initialize();
		}

		/// <summary>
		/// Gets the number of iterations. Return 0 if not determinable.
		/// </summary>
		public override int Iterations
		{
			get
			{
				int iterations = 1;
				foreach (Parameter parameter in Parameters)
				{
					if (parameter.ParameterType != typeof(int) && parameter.ParameterType != typeof(double))
						continue;
					iterations *= 1 + (int) Math.Round((parameter.Max - Math.Min(parameter.Max, parameter.Min)) / parameter.Increment, 0);
				}

				return iterations;
			}
		}
		
		/// <summary>
		/// This methods iterates the parameters recursively. The actual back test is performed, as the last parameter is iterated.
		/// </summary>
		/// <param name="index"></param>
		private void Iterate(int index)
		{
			if (Parameters.Count == 0)
				return;

			Parameter parameter = Parameters[index];
			if (parameter.ParameterType != typeof(int) && parameter.ParameterType != typeof(double))
			{
				if (index == Parameters.Count - 1)			// last parameter ?
					RunIteration(null, null);				// run the iteration. Note: this method may run asynchronously if MultiThreadSupport == true
				else										// iterate next parameter
					Iterate(index + 1);
				return;
			}
			
			for (int i = 0; parameter.Min + i * parameter.Increment <= parameter.Max + parameter.Increment / 1000000; i++)
			{
				if (UserAbort)
					return;

				parameter.Value = parameter.Min + i * parameter.Increment;
				if (index == Parameters.Count - 1)			// last parameter ?
					RunIteration(null, null);				// run the iteration. Note: this method may run asynchronously if MultiThreadSupport == true
				else										// iterate next parameter
					Iterate(index + 1);
			}
		}

		/// <summary>
		/// Gets a value indicating if NinjaTrader should toggle internal logic to multi thread support.
		/// </summary>
		public override bool MultiThreadSupport
		{
			get { return (Strategy != null && Strategy.MultiThreadSupport && NinjaTrader.Cbi.Globals.ProcessorsEnabled > 1); }
		}

		/// <summary>
		/// Runs the optimizer on a given parameter set.
		/// Beware: this method might be called in a non-main, worker thread. E.g. driving GUI logic from here forward would cause issues.
		/// This is a brute-force optimizer. It runs back tests on every potential parameter combination.
		/// </summary>
		public override void Optimize()
		{
			Iterate(0);

			// Wait until all iterations are completed before returning. Set done=true to indicate we're done.
			// This method actually is required on asynchronous RunIteration calls if MultiThreadSupport == true
			WaitForIterationsCompleted(true);
		}
	}
}
