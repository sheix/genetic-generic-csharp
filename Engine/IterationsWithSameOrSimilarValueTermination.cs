using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine
{
    public class IterationsWithSameOrSimilarValueTermination<T> : Termination<T>
    {
        IterationsWithSameOrSimilarValueTermination(int iterations, double epsilon = 0)
        {
            _iterations = iterations;
            _epsilon = epsilon;
            _lastFitnessFunctionValues = new Queue<double>(_iterations);
        }

        private double _epsilon;
        private int _iterations;
        private Queue<double> _lastFitnessFunctionValues;

        public override bool Terminate(Engine<T> engine)
        {
            if (_lastFitnessFunctionValues.Count == _iterations)
            {
                if (
                    _lastFitnessFunctionValues.All(
                        m => Math.Abs(m - _lastFitnessFunctionValues.Average()) <= _epsilon))
                    return true;

                _lastFitnessFunctionValues.Dequeue();
            }

            _lastFitnessFunctionValues.Enqueue(engine.FitnessFunction(engine.GetBest(1).FirstOrDefault()));
            
            return false;
        }
    }
}