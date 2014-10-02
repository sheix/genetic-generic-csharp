using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine
{
    public class Engine<T>
    {
        private readonly IRandomSolutionFactory<T> _factory;
        private readonly EngineParameters<T> _parameters;
        private List<T> _population;
        private readonly Random _random;
        private Termination _termination;
        private List<Func<T,T,T>> Crossovers { get; set; }
        private List<Action<T>> Mutations { get; set; }

        public Engine(IRandomSolutionFactory<T> factory, EngineParameters<T> parameters)
        {
            _random = new Random();
            _factory = factory;
            _parameters = parameters;
            Crossovers = new List<Func<T, T, T>>();
            Mutations = new List<Action<T>>();
            Iteration = 0;
            Populate();
        }

        public IEnumerable<T> GetBest(int n)
        {
            return _population.Take(n);
        }

        public void AddCrossover(Func<T,T,T> func)
        {
            Crossovers.Add(func);
        }

        public void AddMutation(Action<T> func)
        {
            Mutations.Add(func);
        }

        public void Populate()
        {
            _population = new List<T>();
            for (int i = 0; i < _parameters.PopulationSize; i++)
            {
                var t = _factory.GetPossibleSolution();
                _population.Add(t);
            }
        }


        public void RunIteration()
        {
            KillWorstMemebers();
            Mutate();
            CrossOver();
            SwitchPopulations();
            Iteration++;
        }

        private void SwitchPopulations()
        {
            _population = _newPopulation.ToList();
        }

        public void RunIterations(Termination termination)
        {
            if (termination == null) _termination = new Termination();
            _termination = termination;

            while (!Terminate())
            {
                RunIteration();
            }
        }

        private Queue<double> _lastFitnessFunctionValues;
        private List<T> _newPopulation;

        private bool Terminate()
        {
            
            if (_termination.Iterations.HasValue)
               if (Iteration == _termination.Iterations.Value)
                   return true;

            if (_termination.IterationsWithSameMaximum.HasValue && _termination.Epsilon != null )
            {   
                if (_lastFitnessFunctionValues == null)
                    _lastFitnessFunctionValues = new Queue<double>(_termination.IterationsWithSameMaximum.Value);
                if (_lastFitnessFunctionValues.Count == _termination.IterationsWithSameMaximum.Value)
                {
                    if (_lastFitnessFunctionValues.All(m => Math.Abs(m - _lastFitnessFunctionValues.Average()) < _termination.Epsilon.Value))
                            return true;

                    _lastFitnessFunctionValues.Dequeue();
                }

                _lastFitnessFunctionValues.Enqueue(_parameters.FitnessFunction(_population[0]));
                //_same
            }
            return false;
        }

        public int Iteration { get; private set; }

        private void CrossOver()
        {
            int newbornCount = _parameters.PopulationSize - _newPopulation.Count;
            var parentsForCrossover = SelectParentsForCrossover(newbornCount*2).ToArray();
            for (int i = 0; i < newbornCount; i++)
            {
                var crossover = SelectCrossover();
                _newPopulation.Add(crossover(parentsForCrossover[i*2], parentsForCrossover[i*2+1]));
            }
            
        }

        private IEnumerable<T> SelectParentsForCrossover(int count)
        {
            var result = new List<T>();
            for (int i = 0; i < count;i++ )
                result.Add( _population[_random.Next() % (int)(_population.Count / 100f * _parameters.BestParents)]);
            return result;
        }

        private Func<T,T,T> SelectCrossover()
        {
            return Crossovers[_random.Next(Crossovers.Count)];
        }

        private void Mutate()
        {
            var howManyMutations = _newPopulation.Count / 100f * _parameters.MutationRate;
            for (var i = 0; i < howManyMutations; i++)
            {
                var mutation = SelectMutation();
                mutation(_newPopulation[_random.Next(_newPopulation.Count)]);
            }
        }

        private Action<T> SelectMutation()
        {
            return Mutations[_random.Next(Mutations.Count)];
        }

        private void KillWorstMemebers()
        {
            var fitnessFunctions = _population.ToDictionary(item => item, item => _parameters.FitnessFunction(item));
            _population.Sort((a, b) => fitnessFunctions[a].CompareTo(fitnessFunctions[b]));
            _newPopulation = _population.Take(_parameters.PopulationSize*_parameters.Survivors/100).ToList();
        }
    }

    public class EngineParameters<T>
    {
        public int MutationRate;
        public int PopulationSize;
        public int BestParents;
        public int Survivors;
        public Func<T, double> FitnessFunction;
    }

    public class Termination
    {
        public int? Iterations;
        public int? IterationsWithSameMaximum;
        public double? Epsilon;
    }

    public interface IRandomSolutionFactory<out T>
    {
        T GetPossibleSolution();
    }
}
