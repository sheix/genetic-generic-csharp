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
        protected List<Action<T>> Mutations { get; set; }

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
            SelectBestMembers();
            Mutate();
            CrossOver();
            Iteration++;
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

        private bool Terminate()
        {
            if (_termination.Iterations.HasValue)
               if (Iteration == _termination.Iterations.Value)
                   return true;

            if (_termination.IterationsWithSameMaximum.HasValue)
            {   

                //_same
            }
            return false;
        }

        public int Iteration { get; private set; }

        private void CrossOver()
        {
            int newbornCount = PopulationSize - _population.Count;
            var newPopulation = new List<T>();
            for (int i = 0; i < newbornCount; i++)
            {
                var crossover = SelectCrossover();
                newPopulation.Add(crossover(_population[_random.Next(_population.Count)], _population[_random.Next(_population.Count)]));
            }
            _population.AddRange(newPopulation);
        }

        private Func<T,T,T> SelectCrossover()
        {
            return Crossovers[_random.Next(Crossovers.Count)];
        }

        private void Mutate()
        {
            var howManyMutations = PopulationSize/100*MutationRate;
            for (var i = 0; i < howManyMutations; i++)
            {
                var mutation = SelectMutation();
                mutation(_population[_random.Next(_population.Count)]);
            }
        }

        private Action<T> SelectMutation()
        {
            return Mutations[_random.Next(Mutations.Count)];
        }

        public int MutationRate { get; set; }

        private void SelectBestMembers()
        {
            // Thats optimization!
            var fitnessFunctions = _population.ToDictionary(item => item, item => FitnessFunction(item));
            _population.Sort((a, b) => fitnessFunctions[a].CompareTo(fitnessFunctions[b]));
            var newPopulation = _population.Take(PopulationSize*SurvivorsPercent/100);
            _population = newPopulation.ToList();
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
    }

    public interface IRandomSolutionFactory<out T>
    {
        T GetPossibleSolution();
    }
}
