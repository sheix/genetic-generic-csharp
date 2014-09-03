using System;
using System.Linq;
using Engine;
using SFML.Graphics;
using SFML.Window;

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
        private static Engine<Wave> _engine;
        private static ResultWave _myWave;

        public static void Main()
        {
            var random = new Random();
            _engine = new Engine<Wave>(new RandomWaveFactory());
            _myWave = new ResultWave();
            _myWave.AddWave(new SquareWave(127, 10, 10, 0));
            _myWave.AddWave(new TriangleWave(63, 25, 0, 0, 0));
            _engine.SetFitnessFunction(a => a.Generate(1024).AbsoluteDifference(_myWave.Generate(1024)));
            _engine.PopulationSize = 400;
            _engine.Populate();
            _engine.SurvivorsPercent = 10;
            _engine.AddCrossover((w1, w2) =>
                                    {
                                        var result = new ResultWave();
                                        foreach (var wave in w1.Waves())
                                        {
                                            if (random.Next(2)==0)
                                                result.AddWave(wave);
                                        }
                                        foreach (var wave in w2.Waves())
                                        {
                                            if (random.Next(2) == 0)
                                                result.AddWave(wave);
                                        }
                                        return result;
                                    }
                );

            //engine.SetTerminator();
            var window = new RenderWindow(VideoMode.DesktopMode, "Test");
            window.Closed += OnClosed;
            window.KeyPressed += OnKeyPressed;

            while (window.IsOpen())
            {

                window.DispatchEvents();
                window.Clear();

                RenderWaves(window);

                window.Display();
            }


            
        }

        private static void RenderWaves(RenderWindow window)
        {
            var bestWaves = _engine.GetBest(5);

            for (int i = 0; i < 5; i++)
            {
                DrawWave(window, bestWaves.ToArray()[i], i);
            }


            DrawWave(window, _myWave);
        }

        private static void DrawWave(RenderWindow window, Wave bestWave, int i = 5)
        {
            int x = 0;
            Vertex oldVertex = new Vertex(new Vector2f(x,i*256));
            
            foreach (var pitch in bestWave.Generate(1024))
            {
                
                var line = new Vertex[2];
                var newVertex = new Vertex(new Vector2f(x, i*256 + pitch));
                line[0] = oldVertex;
                line[1] = newVertex;
                x++;
                window.Draw(line,PrimitiveType.Lines);
                oldVertex = newVertex;
            }
            
        }

        private static void OnClosed(object sender, EventArgs e)
        {
            var window = (Window)sender;
            window.Close();

        }

        private static void OnKeyPressed(object sender, KeyEventArgs e)
        {
            var window = (Window)sender;
            Console.WriteLine((e.Code).ToString());
            if (e.Code == Keyboard.Key.Escape)
                window.Close();
            if (e.Code == Keyboard.Key.Right)
                _engine.RunIteration();
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
            var wave = new ResultWave();
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
