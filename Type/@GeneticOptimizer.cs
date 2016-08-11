// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using NinjaTrader.Cbi;
using Message = System.Windows.Forms.Message;

#endregion

namespace NinjaTrader.Strategy
{
	[Gui.Design.DisplayName("Genetic")]
	public class GeneticOptimizer : OptimizationMethod
	{
		private				double		crossoverRate		= 0.80;
		private				int			maxGenerations		= 5;
		private				double		minimumPerformance;
		private				double		mutationRate		= 0.02;
		private				double		mutationStrength	= 0.25;
		private				int			populationSize		= 25;
		private readonly	Random		random				= new Random();
		private				double		resetSize			= 0.03;
		private				double		stabilitySize		= 0.04;
		private				Chromosomes strategyParameters;
		private static		GoForm		options;

		/// <summary>
		/// </summary>
		public override object Clone()
		{
			return new GeneticOptimizer
						{
							Parameters			= (ParameterCollection)Parameters.Clone(),
							StrategyParameters	= StrategyParameters != null ? (Chromosomes)StrategyParameters.Clone() : null,
							MaxGenerations		= MaxGenerations,
							PopulationSize		= PopulationSize,
							MutationRate		= MutationRate,
							MutationStrength	= MutationStrength,
							StabilitySize		= StabilitySize,
							ResetSize			= ResetSize,
							MinimumPerformance	= MinimumPerformance,
							CrossoverRate		= CrossoverRate
						};
		}

		private Individual CreateRandomIndividual()
		{
			var parameters = new int[StrategyParameters.Count];
			for (int idx = 0; idx < StrategyParameters.Count; idx++)
				parameters[idx] = random.Next(StrategyParameters[idx].Count);
			return new Individual(parameters);
		}

		private void Crossover(int length, Individual mother, Individual father, out int[] son, out int[] daughter)
		{
			int position	= random.Next(StrategyParameters.Count);
			son				= new int[length];
			daughter		= new int[length];

			for (int i = 0; i < StrategyParameters.Count; i++)
				if (i < position)
				{
					son[i]		= father.GetChromosomeIndex(i);
					daughter[i] = mother.GetChromosomeIndex(i);
				}
				else
				{
					son[i]		= mother.GetChromosomeIndex(i);
					daughter[i] = father.GetChromosomeIndex(i);
				}
		}

		/// <summary>
		/// Called before Optimize() to setup the optimizer.
		/// Always called in main thread which would allow for putting additional GUI in here.
		/// </summary>
		public override void Initialize()
		{
			// don't forget to call Initialize() on the base class before any custom handling
			base.Initialize();
			bool optionWindowRequired	= false;
			StrategyParameters			= new Chromosomes(Strategy, true);
			if (StrategyParameters.HasBoolOrEnum)
			{
				optionWindowRequired = true;
				if (options == null)
					options = new GoForm(StrategyParameters);
			}

			if (optionWindowRequired && options.ShowDialog(Form.ActiveForm) != DialogResult.OK)
				StrategyParameters = new Chromosomes(Strategy, true);
		}

		/// <summary>
		/// Callback triggered as an iteration is completed. Note: it might be called from a differet thread if MultiThreadSupport == true.
		/// </summary>
		/// <param name="individual"></param>
		/// <param name="performanceValue"></param>
		public void IterationCompleted(object individual, double performanceValue)
		{
			((Individual)individual).Performance = performanceValue;
		}

		private int Mutate(double strength, int values, int value)
		{
			int minValue = Math.Max(0, value - (int)(values * strength));
			int maxValue = Math.Min(values, value + (int)(values * strength));
			return random.Next(minValue, maxValue);
		}

		/// <summary>
		/// Runs the optimizer on a given parameter set.
		/// Beware: this method might be called in a non-main, worker thread. E.g. driving GUI logic from here forward would cause issues.
		/// </summary>
		public override void Optimize()
		{
			Individual supremeIndividual = null;

			try
			{
				var realStabilitySize			= (int)Math.Ceiling(populationSize * stabilitySize);
				var realResetSize				= (int)Math.Ceiling(populationSize * resetSize);
				var numberOfStrategyParameters	= StrategyParameters.Count;
				var combinations				= StrategyParameters.CountCombinations;
				var siblings					= new Dictionary<Individual, bool>();
				var children					= new List<Individual>();
				var parents						= new List<Individual>();
				var prevStabilityScore			= double.NaN;

				// Start optimization process
				for (int generation = 1; generation <= maxGenerations; generation++)
				{
					//If Initial generation, seed the population with random individuals
					while (children.Count < populationSize && siblings.Count < combinations)
						ValidateIndividual(children, siblings, CreateRandomIndividual());

					foreach (Individual individual in children)
					{
						if (UserAbort)
							throw new AbortException();
						StrategyParameters.SetStrategyParameters(individual, Strategy, Parameters);
						RunIteration(IterationCompleted, individual);
					}

					WaitForIterationsCompleted(false); //Make sure that each individual completed its iteration

					parents.AddRange(children);
					parents.Sort();

					Individual generationBest = parents[0]; //This is the best performing individual since stability reset
					if (parents.Count > 0 && generationBest != null &&
						(supremeIndividual == null || supremeIndividual.Performance < generationBest.Performance))
						supremeIndividual = generationBest;

					int stabilityIndividuals = Math.Max(1, Math.Min(parents.Count, realStabilitySize));
					double stabilityScore = 0.0;
					for (int i = 0; i < stabilityIndividuals; i++)
						stabilityScore += parents[i].Performance;

					if (supremeIndividual != null && minimumPerformance > 0 && supremeIndividual.Performance > minimumPerformance)
						break;

					children.Clear(); //Clearing up space for the next generation

					if (siblings.Count >= combinations || generation == maxGenerations)
						break;

					//Repopulating the next generation.

					//If Population is stable, leave just best performing individuals
					if (stabilityScore == prevStabilityScore)
					{
						if (realResetSize <= 0)
							parents.Clear();
						else
							if (parents.Count > 0)
								parents = parents.GetRange(0, Math.Min(parents.Count, realResetSize));
						continue;
					}

					//Assign a weight for each parent
					RankPopulation(parents);

					//Reproduce extra individuals mutated from the best
					ShakeBestPerformer(children, siblings, combinations, generationBest, numberOfStrategyParameters);

					//Reproduce children using Crossover, Mutation and totally random genomes
					while (children.Count < populationSize && siblings.Count < combinations)
					{
						if (UserAbort)
							throw new AbortException();
						if (random.NextDouble() <= crossoverRate)
						{
							int[] chromosomeIndexesForSon;
							int[] chromosomeIndexesForDaughter;
							//Produce two children from two parents
							//Parents are selected using Roulette Selection Method
							//Note: There are many other selection algorithms (Google them out...)
							//TODO: Implements some additional selection methods
							//Crossover is performed using single break point
							//TODO: implement 2 or 3 break points Crossover algorithms
							Crossover(numberOfStrategyParameters, parents[RouletteSelection(parents)], parents[RouletteSelection(parents)], out chromosomeIndexesForSon, out chromosomeIndexesForDaughter);
							for (var idx = 0; idx < numberOfStrategyParameters; idx++)
							{
								if (StrategyParameters[idx].Count <= 1)
									continue;

								//After children are produced from crossover, small part should be mutated
								if (random.NextDouble() <= mutationRate)
									chromosomeIndexesForSon[idx] = Mutate(mutationStrength, StrategyParameters[idx].Count, chromosomeIndexesForSon[idx]);
								if (random.NextDouble() <= mutationRate)
									chromosomeIndexesForDaughter[idx] = Mutate(mutationStrength, StrategyParameters[idx].Count, chromosomeIndexesForDaughter[idx]);
							}
							//Make sure that children are unique, so we wont waste any time for evaluation of their performance
							ValidateIndividual(children, siblings, new Individual(chromosomeIndexesForSon));
							ValidateIndividual(children, siblings, new Individual(chromosomeIndexesForDaughter));
						}
						else
						{
							//Create random children
							//Make sure that children are unique, so we wont waste any time for evaluation of their performance
							ValidateIndividual(children, siblings, CreateRandomIndividual());
							ValidateIndividual(children, siblings, CreateRandomIndividual());
						}
					}
					prevStabilityScore = stabilityScore;
				}
			}
			catch
			{
			}
			finally
			{
				WaitForIterationsCompleted(true);
				if (options != null)
				{
					options.Dispose();
					options = null;
				}
			}
		}

		private static void RankPopulation(IList<Individual> individuals)
		{
			if (individuals == null || individuals.Count == 0)
				return;
			double minPerformance	= individuals[individuals.Count - 1].Performance;
			double maxPerformance	= individuals[0].Performance;
			double totalWeight		= 0;
			double denom = maxPerformance == minPerformance ? 1 : maxPerformance - minPerformance;
			foreach (Individual parent in individuals)
			{
				parent.Weight = (parent.Performance - minPerformance) / denom;
				totalWeight += parent.Weight;
			}
			if (totalWeight == 0)
				totalWeight = 1;
			individuals[individuals.Count - 1].Weight = 0;
			for (int i = individuals.Count - 2; i >= 0; i--)
				individuals[i].Weight = individuals[i + 1].Weight + individuals[i].Weight / totalWeight;
		}

		private int RouletteSelection(IList<Individual> parents)
		{
			var randomWeight	= random.NextDouble();
			var idx				= -1;
			var first			= 0;
			var last			= parents.Count - 1;
			var mid				= (last - first) / 2;

			while (idx == -1 && first <= last)
			{
				if (randomWeight < parents[mid].Weight)
					first = mid;
				else
					last = mid;
				mid = (first + last) / 2;
				if (last - first == 1)
					idx = last;
			}
			return idx;
		}

		private void ShakeBestPerformer(ICollection<Individual> children, Dictionary<Individual, bool> siblings, long combinations, Individual generationBest, int numberOfStrategyParameters)
		{
			if (generationBest == null)
				return;
			for (int i = 0; i < numberOfStrategyParameters; i++)
			{
				int numberOfPossibleParameterValues = StrategyParameters[i].Count;
				for (int x = 0; x < 5; x++)
				{
					if (children.Count >= populationSize || siblings.Count >= combinations)
						continue;
					var parameters = new int[numberOfStrategyParameters];
					for (int j = 0; j < numberOfStrategyParameters; j++)
						parameters[j] = generationBest.GetChromosomeIndex(j);
					parameters[i] = Mutate(mutationStrength, numberOfPossibleParameterValues, parameters[i]);
					ValidateIndividual(children, siblings, new Individual(parameters));
				}
			}
		}

		private void ValidateIndividual(ICollection<Individual> children, IDictionary<Individual, bool> siblings, Individual child)
		{
			if (siblings.ContainsKey(child) || children.Count >= populationSize)
				return;
			siblings.Add(child, true);
			children.Add(child);
		}

		#region Properties

		/// <summary>
		/// The maximum number of generations.
		/// </summary>
		[Description("The maximum number of generations.")]
		[Category("Optimize")]
		[Gui.Design.DisplayName("GO: # of Generations")]
		public int MaxGenerations
		{
			get { return maxGenerations; }
			set { maxGenerations = Math.Max(1, value); }
		}

		/// <summary>
		/// The number of individuals in each generation.
		/// </summary>
		[Description("The number of individuals in each generation.")]
		[Category("Optimize")]
		[Gui.Design.DisplayName("GO: Generation Size")]
		public int PopulationSize
		{
			get { return populationSize; }
			set { populationSize = Math.Max(1, value); }
		}

		/// <summary>
		/// The minimum performance. Optimizer will stop if min performance is reached. Ignored if 0
		/// </summary>
		[Description("The minimum performance. Optimizer will stop if min performance is reached. Ignored if 0")]
		[Category("Optimize")]
		[Gui.Design.DisplayName("GO: Minimum Performance")]
		public double MinimumPerformance
		{
			get { return minimumPerformance; }
			set { minimumPerformance = Math.Max(0, value); }
		}

		/// <summary>
		/// The probability that a parameter will be mutated during procreation.
		/// </summary>
		[Description("The probability that a parameter will be mutated during procreation.")]
		[Category("Optimize")]
		[Gui.Design.DisplayName("GO: Mutation Rate (%)")]
		public double MutationRate
		{
			get { return mutationRate * 100.0; }
			set { mutationRate = Math.Max(0, Math.Min(1, value * 0.01)); }
		}

		/// <summary>
		/// The amount that a parameter can mutate as a percentage of the parameter's possible range of values.
		/// </summary>
		[Description("The amount that a parameter can mutate, as a percentage of the parameter's possible range of values.")]
		[Category("Optimize")]
		[Gui.Design.DisplayName("GO: Mutation Strength (%)")]
		public double MutationStrength
		{
			get { return mutationStrength * 100.0; }
			set { mutationStrength = Math.Max(0, Math.Min(1, value * 0.01)); }
		}

		/// <summary>
		/// Percentage of the all time best performing non changing individuals. Once reached, Optimizer will generate random generation. See also Reset Size
		/// </summary>
		[Description("Percentage of the all time best performing non changing individuals. Once reached, Optimizer will generate random generation. See also Reset Size")]
		[Category("Optimize")]
		[Gui.Design.DisplayName("GO: Stability Size (%)")]
		public double StabilitySize
		{
			get { return stabilitySize * 100.0; }
			set { stabilitySize = Math.Max(0, Math.Min(1, value * 0.01)); }
		}

		/// <summary>
		/// The percentage of population which will be procreated from parents. The rest of population will be random generated
		/// </summary>
		[Description("The percentage of population which will be procreated from parents. The rest of population will be random generated")]
		[Category("Optimize")]
		[Gui.Design.DisplayName("GO: Crossover Rate (%)")]
		public double CrossoverRate
		{
			get { return crossoverRate * 100.0; }
			set { crossoverRate = Math.Max(0, Math.Min(1, value * 0.01)); }
		}

		/// <summary>
		/// Percentage best-performing individuals to keep when reseeding the population after stability is reached
		/// </summary>
		[Description("Percentage best-performing individuals to keep when reseeding the population after stability is reached.")]
		[Category("Optimize")]
		[Gui.Design.DisplayName("GO: Reset Size (%)")]
		public double ResetSize
		{
			get { return resetSize * 100.0; }
			set { resetSize = Math.Max(0, Math.Min(1, value * 0.01)); }
		}

		/// <summary>
		/// Gets the number of iterations. Return 0 if not determinable.
		/// </summary>
		public override int Iterations
		{
			get { return maxGenerations * populationSize; }
		}

		/// <summary>
		/// Gets a value indicating if NinjaTrader should toggle internal logic for multi thread support on the optimizer.
		/// </summary>
		public override bool MultiThreadSupport
		{
			get { return Strategy != null && Strategy.MultiThreadSupport && Globals.ProcessorsEnabled > 1 && StrategyParameters != null && !StrategyParameters.HasBoolOrEnum; }
		}

		public override bool RandomWalk
		{
			get { return true; }
		}

		public Chromosomes StrategyParameters
		{
			get { return strategyParameters; }
			set { strategyParameters = value; }
		}

		#endregion
	}

	#region Misc

	public class Chromosomes : IEnumerable<Chromosome>
	{
		private				List<Chromosome>	chromosomes = new List<Chromosome>();
		private				bool				hasBoolOrEnum;
		private readonly	ParameterCollection parameters;
		private readonly	Type				strategyType;

		public Chromosomes()
		{
		}

		public Chromosomes(StrategyBase strategy, bool flagInitValues)
		{
			parameters = new ParameterCollection();
			if (strategy.OptimizeDataSeries)
				parameters.Add((Parameter)strategy.BarsPeriodParameter.Clone());
			foreach (Parameter tmp in strategy.Parameters)
				parameters.Add((Parameter)tmp.Clone());

			strategyType = strategy.GetType();
			for (int idx = 0; idx < parameters.Count; idx++)
			{
				Parameter parameter	= parameters[idx];
				Type parameterType	= parameter.ParameterType;
				if (parameterType.IsEnum)
				{
					var enumChromosome	= new EnumChromosome(strategyType, parameter, idx);
					hasBoolOrEnum		= true;
					chromosomes.Add(enumChromosome);
					if (flagInitValues)
					{
						object value = enumChromosome.Get(strategy);
						if (value != null)
							enumChromosome.Add(value);
					}
				}
				else
					if (parameterType == typeof(bool))
					{
						var booleanChromosome	= new BooleanChromosome(strategyType, parameter, idx);
						hasBoolOrEnum			= true;
						chromosomes.Add(booleanChromosome);
						if (flagInitValues)
							booleanChromosome.Add(booleanChromosome.Get(strategy));
					}
					else
						if (parameterType.IsPrimitive)
							chromosomes.Add(new PrimitiveChromosome(parameters, idx));
			}
		}

		public List<Chromosome> ChromosomesList
		{
			get { return chromosomes; }
			set { chromosomes = value; }
		}

		public Chromosome this[int idx]
		{
			get { return chromosomes[idx]; }
		}

		public object Clone()
		{
			var ret = new Chromosomes();
			foreach (Chromosome chromosome in chromosomes)
				ret.ChromosomesList.Add((Chromosome)chromosome.Clone());
			ret.HasBoolOrEnum = HasBoolOrEnum;
			return ret;
		}

		public int Count
		{
			get { return chromosomes.Count; }
		}

		public long CountCombinations
		{
			get
			{
				decimal ret;
				try
				{
					ret = chromosomes.Aggregate<Chromosome, decimal>(1, (current, pd) => current * pd.Count);
				}
				catch (OverflowException)
				{
					ret = (decimal)long.MaxValue + 1;
				}
				if (ret > long.MaxValue)
				{
					MessageBox.Show(@"Optimization range decreased. The parameter settings exceed the maximum number of test combinations. For a complete optimization covering the full range please increase the increment steps on your parameters.", @"Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					ret = long.MaxValue;
				}
				return (long)ret;
			}
		}

		public Chromosome Find(Chromosome ch)
		{
			return chromosomes.FirstOrDefault(chromosome => chromosome.Parameter.Name == ch.Parameter.Name && chromosome.Parameter.ParameterType.Name == ch.Parameter.ParameterType.Name);
		}

		#region IEnumerable<Chromosome> Members

		public IEnumerator<Chromosome> GetEnumerator()
		{
			return chromosomes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return chromosomes.GetEnumerator();
		}

		#endregion

		public bool HasBoolOrEnum
		{
			get { return hasBoolOrEnum; }
			set { hasBoolOrEnum = value; }
		}

		public void InitializeValuesFrom(Chromosomes source)
		{
			foreach (var chromosome in this)
			{
				var tmpChromosome = source.Find(chromosome);
				if (tmpChromosome == null)
					continue;
				var nonPrimitiveOld = tmpChromosome	as NonPrimitiveChromosome;
				var nonPrimitiveNew = chromosome	as NonPrimitiveChromosome;
				if (nonPrimitiveOld == null || nonPrimitiveNew == null)
					continue;
				nonPrimitiveNew.InitializeFrom(nonPrimitiveOld);
			}
		}

		public void SetStrategyParameters(Individual individual, StrategyBase strategy, ParameterCollection pparameters)
		{
			for (var idx = 0; idx < chromosomes.Count; idx++)
			{
				var chromosome = chromosomes[idx];
				chromosome.Set(strategy, pparameters, individual.GetChromosomeIndex(idx));
			}
		}

		public string ToString(Individual individual)
		{
			var sb = new StringBuilder();
			sb.AppendLine("Parameters:");
			for (int idx = 0; idx < chromosomes.Count; idx++)
			{
				Chromosome pd = chromosomes[idx];
				sb.Append(pd.ToString());
				sb.Append("\t\t=  ");
				sb.AppendLine(pd.ToString(individual.GetChromosomeIndex(idx)));
			}

			return sb.ToString();
		}
	}

	public abstract class Chromosome : IEquatable<Chromosome>
	{
		private readonly int		idx;
		private readonly Parameter	parameter;

		protected Chromosome(Parameter parameter, int idx)
		{
			this.parameter	= parameter;
			this.idx		= idx;
		}

		public Parameter Parameter
		{
			get { return parameter; }
		}

		protected int Index
		{
			get { return idx; }
		}

		public abstract int Count { get; }

		#region IEquatable<Chromosome> Members

		public bool Equals(Chromosome chromosome)
		{
			return chromosome != null && Parameter.Name == chromosome.Parameter.Name && Parameter.ParameterType.Name == chromosome.Parameter.ParameterType.Name;
		}

		#endregion

		public abstract object Clone();

		public abstract void Set(StrategyBase strategy, ParameterCollection parameterCollection, int idx);

		public override string ToString()
		{
			return parameter.Name;
		}

		public abstract string ToString(int idx);
	}

	internal class PrimitiveChromosome : Chromosome
	{
		private readonly ParameterCollection chromosomes;

		public PrimitiveChromosome(ParameterCollection chromosomeCollection, int idx)
			: base(chromosomeCollection[idx], idx)
		{
			chromosomes = chromosomeCollection;
		}

		public ParameterCollection Chromosomes
		{
			get { return chromosomes; }
		}

		public override int Count
		{
			get { return 1 + (int)Math.Floor((Math.Max(Parameter.Max, Parameter.Min) - Parameter.Min + double.Epsilon) / Parameter.Increment); }
		}

		public override object Clone()
		{
			return new PrimitiveChromosome(chromosomes, Index);
		}

		public override void Set(StrategyBase strategy, ParameterCollection parameterCollection, int idx)
		{
			if (idx < 0 || idx >= Count)
				throw new ArgumentOutOfRangeException("idx");
			parameterCollection[Index].Value = Parameter.Min + Parameter.Increment * idx;
		}

		public override string ToString(int idx)
		{
			var value = Parameter.Min + Parameter.Increment * idx;
			return Parameter.ParameterType == typeof(double) || Parameter.ParameterType == typeof(float) ? value.ToString("N2") : value.ToString();
		}
	}

	internal abstract class NonPrimitiveChromosome : Chromosome
	{
		protected NonPrimitiveChromosome(Parameter param, int idx) : base(param, idx){}

		public abstract IEnumerable<ChromosomeInfo> GetListedValues();

		public abstract void InitializeFrom(NonPrimitiveChromosome source);

		#region Nested type: ChromosomeInfo

		public class ChromosomeInfo
		{
			public readonly bool			Active;
			public readonly Action<bool>	Checker;
			public readonly string			Name;

			public ChromosomeInfo(string name, bool active, Action<bool> checker)
			{
				Name	= name;
				Active	= active;
				Checker	= checker;
			}
		}

		#endregion
	}

	internal abstract class NonPrimitiveChromosome<T> : NonPrimitiveChromosome
	{
		private				List<T>			chromosomesList = new List<T>();
		private readonly	PropertyInfo	propertyInfo;
		protected readonly	Type			StrategyType;

		public void Add(T value)
		{
			if (!chromosomesList.Contains(value))
				chromosomesList.Add(value);
		}

		public List<T> ChromosomesList
		{
			get { return chromosomesList; }
			set { chromosomesList = value; }
		}

		public bool Contains(T value)
		{
			return chromosomesList.Contains(value);
		}

		public override int Count
		{
			get { return chromosomesList.Count; }
		}

		public T Get(StrategyBase strategy)
		{
			return (T)propertyInfo.GetValue(strategy, null);
		}

		public T GetValue(int idx)
		{
			if (idx < 0 || idx >= Count)
				throw new ArgumentOutOfRangeException("idx");

			return chromosomesList[idx];
		}

		public override void InitializeFrom(NonPrimitiveChromosome source)
		{
			var tmp = source as NonPrimitiveChromosome<T>;
			if (tmp == null)
				return;
			chromosomesList = new List<T>(tmp.chromosomesList);
		}

		protected NonPrimitiveChromosome(Type strType, Parameter param, int idx): base(param, idx)
		{
			StrategyType = strType;
			propertyInfo = strType.GetProperty(Parameter.Name);
		}

		public void Remove(T value)
		{
			chromosomesList.Remove(value);
		}

		public override void Set(StrategyBase strategy, ParameterCollection parameterCollection, int idx)
		{
			if (idx < 0 || idx >= Count)
				throw new ArgumentOutOfRangeException("idx");
			parameterCollection[Index].Value = chromosomesList[idx];
			propertyInfo.SetValue(strategy, chromosomesList[idx], null);
		}

		public override string ToString(int idx)
		{
			return GetValue(idx).ToString();
		}
	}

	internal class BooleanChromosome : NonPrimitiveChromosome<bool>
	{
		public BooleanChromosome(Type typeStrategy, Parameter param, int iParam): base(typeStrategy, param, iParam){}

		public override object Clone()
		{
			var ret = new BooleanChromosome(StrategyType, Parameter, Index);
			ret.InitializeFrom(this);
			return ret;
		}

		public override IEnumerable<ChromosomeInfo> GetListedValues()
		{
			yield return new ChromosomeInfo("True", Contains(true), delegate(bool isChecked)
																		{
																			if (isChecked)
																				Add(true);
																			else
																				Remove(true);
																		});
			yield return new ChromosomeInfo("False", Contains(false), delegate(bool isChecked)
																		  {
																			  if (isChecked)
																				  Add(false);
																			  else
																				  Remove(false);
																		  });
		}

	}

	internal class EnumChromosome : NonPrimitiveChromosome<object>
	{
		public override object Clone()
		{
			var ret = new EnumChromosome(StrategyType, Parameter, Index);
			ret.InitializeFrom(this);
			return ret;
		}

		public EnumChromosome(Type typeStrategy, Parameter param, int idx) : base(typeStrategy, param, idx){}

		public override IEnumerable<ChromosomeInfo> GetListedValues()
		{
			var type = Parameter.ParameterType;

			return from object enumValue in Enum.GetValues(type)
				   let value = enumValue
				   select new ChromosomeInfo(Enum.GetName(type, enumValue), Contains(enumValue), delegate(bool isChecked)
																									 {
																										 if (isChecked)
																											 Add(value);
																										 else
																											 Remove(value);
																									 });
		}

		public override void InitializeFrom(NonPrimitiveChromosome source)
		{
			var sourceAsEnum = source as EnumChromosome;
			if (sourceAsEnum == null)
				return;
			for (int idx = 0; idx < sourceAsEnum.Count; idx++)
				Add(Enum.Parse(Parameter.ParameterType, Enum.GetName(source.Parameter.ParameterType, sourceAsEnum.GetValue(idx))));
		}
	}

	public class Individual : IEquatable<Individual>, IComparable<Individual>, IComparable
	{
		private readonly	int[]	chromosomeIndexes;
		private readonly	int		hashcode;
		public				double	Performance;
		public				double	Weight;

		public Individual(int[] chromosomeIndexes)
		{
			this.chromosomeIndexes = chromosomeIndexes;
			foreach (int chromosomeIndex in chromosomeIndexes)
				hashcode = (hashcode << 1) ^ chromosomeIndex ^ ((hashcode & 0x80020) >> 4);
		}

		#region IComparable Members

		public int CompareTo(object o)
		{
			return ((Individual)o).Performance.CompareTo(Performance);
		}

		#endregion

		#region IComparable<Individual> Members

		public int CompareTo(Individual individual)
		{
			return individual.Performance.CompareTo(Performance);
		}

		#endregion

		#region IEquatable<Individual> Members

		public bool Equals(Individual individual)
		{
			return individual != null
				   && (chromosomeIndexes.Length == individual.chromosomeIndexes.Length
					   && !chromosomeIndexes.Where((t, i) => t != individual.chromosomeIndexes[i]).Any());
		}

		#endregion

		public override int GetHashCode()
		{
			return hashcode;
		}

		public int GetChromosomeIndex(int idx)
		{
			return chromosomeIndexes[idx];
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			foreach (int chromosomeIndex in chromosomeIndexes)
				sb.Append(chromosomeIndex + "|");
			sb.Append(chromosomeIndexes.Count());
			return sb.ToString();
		}
	}

	public class AbortException : Exception {}

	public class GoForm : Form
	{
		private Button		buttonCancel;
		private Button		buttonOk;
		private Chromosomes chromosomes;
		private GoTreeView	treeView;

		public GoForm(Chromosomes ch)
		{
			InitializeComponent(ch);
		}

		private void OnAfterCheck(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Level == 0) return;
			foreach (Chromosome chromosome in chromosomes)
			{
				var nonPrimitiveChromosome = chromosome as NonPrimitiveChromosome;
				if (nonPrimitiveChromosome == null) continue;
				if (e.Node.Parent.Text == nonPrimitiveChromosome.Parameter.Name)
					foreach (NonPrimitiveChromosome.ChromosomeInfo chromosomeInfo in nonPrimitiveChromosome.GetListedValues())
						if (chromosomeInfo.Name == e.Node.Text)
							chromosomeInfo.Checker(e.Node.Checked);
			}
		}

		private void OnBeforeCheck(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Node.Level == 0)
			{
				foreach (TreeNode node in e.Node.Nodes)
					node.Checked = true;
				foreach (var chromosome in chromosomes)
				{
					var nonPrimitiveChromosome = chromosome as NonPrimitiveChromosome;
					if (nonPrimitiveChromosome == null) continue;
					if (e.Node.Text == nonPrimitiveChromosome.Parameter.Name)
						foreach (NonPrimitiveChromosome.ChromosomeInfo chromosomeInfo in nonPrimitiveChromosome.GetListedValues())
							chromosomeInfo.Checker(true);
				}
				e.Cancel = true;
			}
			else
			{
				var allUnchecked = true;
				foreach (TreeNode node in e.Node.Parent.Nodes)
					if (node.Checked && node != e.Node)
						allUnchecked = false;
				e.Cancel = allUnchecked;
			}
		}

		private void LoadBoolAndEnumParameters(Chromosomes chr)
		{
			if (chromosomes != null) chr.InitializeValuesFrom(chromosomes);
			chromosomes = chr;
			treeView.Nodes.Clear();
			foreach (Chromosome chromosome in chr)
			{
				var nonPrimitiveChromosome = chromosome as NonPrimitiveChromosome;
				if (nonPrimitiveChromosome == null) continue;
				var nameNode = new TreeNode { Text = nonPrimitiveChromosome.Parameter.Name, Checked = true };
				treeView.Nodes.Add(nameNode);
				foreach (NonPrimitiveChromosome.ChromosomeInfo chromosomeInfo in nonPrimitiveChromosome.GetListedValues())
				{
					var valueNode = new TreeNode { Text = chromosomeInfo.Name, Checked = chromosomeInfo.Active };
					nameNode.Nodes.Add(valueNode);
				}
				nameNode.Expand();
			}
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent(Chromosomes ch)
		{
			this.SuspendLayout();
			this.treeView = new GoTreeView();
			// 
			// treeView
			// 
			this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
			this.UpdateStyles();
			LoadBoolAndEnumParameters(ch);
			this.treeView.Anchor = (((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
									   | System.Windows.Forms.AnchorStyles.Left)
									  | System.Windows.Forms.AnchorStyles.Right)));
			this.treeView.CheckBoxes = true;
			this.treeView.Location = new System.Drawing.Point(12, 12);
			this.treeView.Name = "treeView";
			this.treeView.Size = new System.Drawing.Size(264, 313);
			this.treeView.TabIndex = 2;
			this.treeView.BeforeCheck += this.OnBeforeCheck;
			this.treeView.AfterCheck += this.OnAfterCheck;
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = (((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(127, 333);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(75, 23);
			this.buttonOk.TabIndex = 0;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = (((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Abort;
			this.buttonCancel.Location = new System.Drawing.Point(208, 333);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(69, 23);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// GoForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(288, 366);
			this.Controls.Add(this.treeView);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.MinimumSize = new System.Drawing.Size(180, 241);
			this.Name = "GoForm";
			this.ShowIcon = false;
			this.Text = "Genetic Optimizer Options";
			this.ResumeLayout(false);
		}

		#endregion

		#region Nested type: GoTreeView

		public class GoTreeView : TreeView
		{
			protected override void WndProc(ref Message m)
			{
				// Filter double-click messages
				if (m.Msg == 0x203) return;
				base.WndProc(ref m);
			}
		}

		#endregion
	}

	#endregion
}