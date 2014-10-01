using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine
{
    public class Engine<T>
    {
        private readonly IRandomSolutionFactory<T> _factory;
        private List<T> _population;
        public int SurvivorsPercent;
        private readonly Random _random;
        private Termination _termination;
        private List<Func<T,T,T>> Crossovers { get; set; }
        private List<Action<T>> Mutations { get; set; }

        public Engine(IRandomSolutionFactory<T> factory)
        {
            _random = new Random();
            _factory = factory;
            Crossovers = new List<Func<T, T, T>>();
            Mutations = new List<Action<T>>();
            Iteration = 1;
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
            for (int i = 0; i < PopulationSize; i++)
            {
                var t = _factory.GetPossibleSolution();
                _population.Add(t);
            }
        }

        public int PopulationSize { get; set; }
        protected Func<T, double> FitnessFunction { get; set; }

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

                _lastFitnessFunctionValues.Enqueue(FitnessFunction(_population[0]));
                //_same
            }
            return false;
        }

        public int Iteration { get; private set; }

        private void CrossOver()
        {
            int newbornCount = PopulationSize - _newPopulation.Count;
            var parentsForCrossover = SelectParentsForCrossover(newbornCount*2).ToArray();
            for (int i = 0; i < newbornCount; i++)
            {
                var crossover = SelectCrossover();
                _newPopulation.Add(crossover(parentsForCrossover[i*2], parentsForCrossover[i*2+1]));
            }
            
        }

        private IEnumerable<T> SelectParentsForCrossover(int i)
        {
            return _population.Take(i);
        }

        private Func<T,T,T> SelectCrossover()
        {
            return Crossovers[_random.Next(Crossovers.Count)];
        }

        private void Mutate()
        {
            var howManyMutations = _newPopulation.Count / 100f * MutationRate;
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

        public int MutationRate { get; set; }

        private void KillWorstMemebers()
        {
            // Thats optimization!
            var fitnessFunctions = _population.ToDictionary(item => item, item => FitnessFunction(item));
            _population.Sort((a, b) => fitnessFunctions[a].CompareTo(fitnessFunctions[b]));
            _newPopulation = _population.Take(PopulationSize*SurvivorsPercent/100).ToList();
        }

        public void SetFitnessFunction(Func<T, double > func)
        {
            FitnessFunction = func;
        }
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
