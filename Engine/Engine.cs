using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine
{
    public class Engine<T> where T : new()
    {
        private List<T> _population;
        protected int SurvivorsPercent;
        private readonly Random _random;
        private List<Func<T,T,T>> Crossovers { get; set; }
        protected List<Action<T>> Mutations { get; set; }

        public Engine()
        {
            _random = new Random((int)(DateTime.Now.ToBinary() % int.MaxValue));
        }

        public void Populate()
        {
            _population = new List<T>();
            for (int i = 0; i < PopulationSize; i++)
            {
                var t = new T();
                _population.Add(t);
            }
        }

        protected int PopulationSize { get; set; }
        protected Func<T, int> FitnessFunction { get; set; }

        public void RunIteration()
        {
            SelectBestMembers();
            Mutate();
            CrossOver();
            //Terminate();
        }

        private void CrossOver()
        {
            int newbornCount = PopulationSize - _population.Count;
            var newPopulation = new List<T>();
            for (int i = 0; i < newbornCount; i++)
            {
                var crossover = SelectCrossover();
                newPopulation.Add(crossover(_population[_random.Next(PopulationSize)],_population[_random.Next(PopulationSize)]));
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

        protected int MutationRate { get; set; }

        private void SelectBestMembers()
        {
            _population.Sort((a,b) => FitnessFunction(a) - FitnessFunction(b));
            var newPopulation = _population.Take(PopulationSize*100/SurvivorsPercent);
            _population = newPopulation.ToList();
        }
    }
}
