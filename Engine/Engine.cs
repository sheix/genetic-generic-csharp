using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine
{
    public class Engine<T>
    {
        private readonly EngineParameters<T> _parameters;
        private readonly Random _random;
        private List<T> _newPopulation;
        private List<T> _population;

        public Engine(EngineParameters<T> parameters)
        {
            _random = new Random();
            _parameters = parameters;
            Crossovers = new List<Func<T, T, T>>();
            Mutations = new List<Action<T>>();
            Iteration = 0;
            Populate();
        }

        private List<Func<T, T, T>> Crossovers { get; set; }
        private List<Action<T>> Mutations { get; set; }
        public int Iteration { get; private set; }

        public IEnumerable<T> GetBest(int n)
        {
            return _population.Take(n);
        }

        public void AddCrossover(Func<T, T, T> func)
        {
            Crossovers.Add(func);
        }

        public void AddMutation(Action<T> func)
        {
            Mutations.Add(func);
        }

        private void Populate()
        {
            _population = new List<T>();
            for (var i = 0; i < _parameters.PopulationSize; i++)
            {
                T t = _parameters.RandomSolutionFactory.GetPossibleSolution();
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

        public void RunIterations(Termination<T>[] terminations)
        {
            while (!Terminate(terminations))
            {
                RunIteration();
            }
        }

        private bool Terminate(IEnumerable<Termination<T>> terminations)
        {
            if (terminations.Any(termination => termination.Terminate(this)))
            {
                return true;
            }
            return false;
        }

        private void CrossOver()
        {
            var newbornCount = _parameters.PopulationSize - _newPopulation.Count;
            var parentsForCrossover = SelectParentsForCrossover(newbornCount*2).ToArray();
            for (var i = 0; i < newbornCount; i++)
            {
                var crossover = SelectCrossover();
                _newPopulation.Add(crossover(parentsForCrossover[i*2], parentsForCrossover[i*2 + 1]));
            }
        }

        private IEnumerable<T> SelectParentsForCrossover(int count)
        {
            var result = new List<T>();
            for (var i = 0; i < count; i++)
                result.Add(_population[_random.Next()%(int) (_population.Count/100f*_parameters.BestParents)]);
            return result;
        }

        private Func<T, T, T> SelectCrossover()
        {
            return Crossovers[_random.Next(Crossovers.Count)];
        }

        private void Mutate()
        {
            var howManyMutations = _newPopulation.Count/100f*_parameters.MutationRate;
            for (int i = 0; i < howManyMutations; i++)
            {
                Action<T> mutation = SelectMutation();
                mutation(_newPopulation[_random.Next(_newPopulation.Count)]);
            }
        }

        private Action<T> SelectMutation()
        {
            return Mutations[_random.Next(Mutations.Count)];
        }

        private void KillWorstMemebers()
        {
            Dictionary<T, double> fitnessFunctions = _population.ToDictionary(item => item,
                                                                              item => _parameters.FitnessFunction(item));
            _population.Sort((a, b) => fitnessFunctions[a].CompareTo(fitnessFunctions[b]));
            _newPopulation = _population.Take(_parameters.PopulationSize*_parameters.Survivors/100).ToList();
        }

        public double FitnessFunction(T obj)
        {
            return _parameters.FitnessFunction(obj);
        }
    }
}