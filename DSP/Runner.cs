using System;
using System.Linq;
using Engine;

namespace DSP
{
    public static class Util
    {
        public static int AbsoluteDifference(this byte[] bytaArray1, byte[] bytaArray2)
        {
            return bytaArray1.Select((t, i) => Math.Abs(t - bytaArray2[i])).Sum();
        }
    }
    public class Runner
    {

        public static void Main()
        {
            var engine = new Engine<Wave>(new RandomWaveFactory());
            var MyWave = new ResultWave();
            MyWave.AddWave(new SquareWave(127, 10, 10, 0));
            MyWave.AddWave(new TriangleWave(63, 20, 0, 0, 0));
            engine.SetFitnessFunction((a) => a.Generate(1024).AbsoluteDifference(MyWave.Generate(1024)));
            engine.PopulationSize = 400;
            engine.Populate();

            engine.RunIteration();
        }
    }

    internal class RandomWaveFactory : IRandomSolutionFactory<Wave>
    {
        private int maxWaves = 10;
        private Random _random;

        public RandomWaveFactory()
        {
            _random = new Random();
        }
        public Wave GetPossibleSolution()
        {
            int waveCount = _random.Next(maxWaves) + 1;
            ResultWave wave = new ResultWave();
            for (int i = 0; i < waveCount; i++)
            {
                Wave newWave = null;
                const int maxHeight = 255;
                const int maxPause = 255;
                const int maxOffset = 255;
                const int maxWidth = 255;
                const int maxAttackEdge = 255;
                const int maxBackEdge = 255;
                switch (_random.Next(2))
                {
                    case 0:
                        newWave = new SquareWave(
                            (byte)_random.Next(maxHeight),
                            (byte)_random.Next(maxWidth),
                            (byte)_random.Next(maxPause),
                            (byte)_random.Next(maxOffset));
                        break;
                    case 1:

                        newWave = new TriangleWave(
                            (byte)_random.Next(maxHeight),
                            (byte)_random.Next(maxAttackEdge),
                            (byte)_random.Next(maxBackEdge),
                            (byte)_random.Next(maxPause),
                            (byte)_random.Next(maxOffset));
                        break;
                }
                wave.AddWave(newWave);
            }
            return wave;
        }
    }

}
