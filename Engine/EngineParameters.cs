using System;

namespace Engine
{
    public class EngineParameters<T>
    {
        public int BestParents;
        public Func<T, double> FitnessFunction;
        public int MutationRate;
        public int PopulationSize;
        public IRandomSolutionFactory<T> RandomSolutionFactory;
        public int Survivors;
    }
}