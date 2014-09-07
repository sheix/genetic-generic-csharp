using System;
using Engine;

namespace DSP
{
    internal class RandomWaveFactory : IRandomSolutionFactory<Wave>
    {
        private const int _maxHeight = 255;
        private const int _maxPause = 255;
        private const int _maxOffset = 255;
        private const int _maxWidth = 255;
        private const int _maxAttackEdge = 255;
        private const int _maxBackEdge = 255;
        private int maxWaves = 10;
        private static Random _random;

        public RandomWaveFactory()
        {
            _random = new Random();
        }
        public Wave GetPossibleSolution()
        {
            int waveCount = _random.Next(maxWaves) + 1;
            var wave = new ResultWave();
            for (int i = 0; i < waveCount; i++)
            {
                Wave newWave = null;
                switch (_random.Next(2))
                {
                    case 0:
                        newWave = GetNewSquareWave();
                        break;
                    case 1:

                        newWave = GetNewTriangleWave();
                        break;
                }
                wave.AddWave(newWave);
            }
            return wave;
        }

        public static Wave GetNewTriangleWave()
        {
            Wave newWave = new TriangleWave(
                (byte)_random.Next(_maxHeight),
                (byte)_random.Next(_maxAttackEdge),
                (byte)_random.Next(_maxBackEdge),
                (byte)_random.Next(_maxPause),
                (byte)_random.Next(_maxOffset));
            return newWave;
        }

        public static Wave GetNewSquareWave()
        {
            Wave newWave = new SquareWave(
                (byte)_random.Next(_maxHeight),
                (byte)_random.Next(_maxWidth),
                (byte)_random.Next(_maxPause),
                (byte)_random.Next(_maxOffset));
            return newWave;
        }
    }
}