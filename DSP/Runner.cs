using System;
using System.Linq;
using System.Threading.Tasks;
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
        const int Length = 256;
        private static Engine<Wave> _engine;
        private static ResultWave _myWave;
        private static byte[] _constWave;
        public static void Main()
        {
            var random = new Random();
            _engine = new Engine<Wave>( new EngineParameters<Wave>
                                                                    {
                                                                        MutationRate = 50, 
                                                                        PopulationSize = 1000, 
                                                                        BestParents = 5, 
                                                                        Survivors = 20,
                                                                        FitnessFunction = AbsoluteDifference,
                                                                        RandomSolutionFactory = new RandomWaveFactory()
                                                                    });
            _myWave = new ResultWave();
            _myWave.AddWave(new SquareWave(127, 64, 40, 10));
            _myWave.AddWave(new TriangleWave(63, 25, 25, 0, 5));
            _constWave = new byte[Length];
            _constWave = _myWave.Generate(Length);


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
            _engine.AddMutation(m =>
            {
                if (m is ResultWave && m.Waves().Count > 0)
            m.Waves().RemoveAt(random.Next(m.Waves().Count));
            });
            _engine.AddMutation(m=> {if (m is ResultWave) 
            m.Waves().Add(RandomWaveFactory.GetNewSquareWave());});
            _engine.AddMutation(m =>
            {
                if (m is ResultWave)
                    m.Waves().Add(RandomWaveFactory.GetNewTriangleWave());
            });
            var window = new RenderWindow(VideoMode.DesktopMode, "Test");
            window.Closed += OnClosed;
            window.KeyPressed += OnKeyPressed;

            var task = new Task(() => _engine.RunIterations(new Termination<Wave>[] {new NumberOfIterationsTermination<Wave>(100)})); 
            task.Start();

            while (window.IsOpen())
            {
                window.DispatchEvents();
                window.Clear();
                RenderWaves(window);
                RenderGeneration(window);    
                window.Display();
            }
        }

        private static void RenderGeneration(RenderWindow window)
        {
            Drawable text = new Text
            {
                DisplayedString = _engine.Iteration.ToString(),
                CharacterSize = 10,
                Position = new Vector2f(1010, 10),
                Font = new Font("c:\\windows\\fonts\\arial.ttf")
            };
            window.Draw(text);
        }

        private static double AbsoluteDifference(Wave a)
        {
            return a.Generate(Length).AbsoluteDifference(_constWave);
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
            Vertex oldVertex = new Vertex(new Vector2f(x,i*128));
            Drawable text = new Text
                                {
                                    DisplayedString = AbsoluteDifference(bestWave).ToString(),
                                    CharacterSize = 10,
                                    Position = new Vector2f(100, i * 128),
                                    Font = new Font("c:\\windows\\fonts\\arial.ttf") 
                                };
            window.Draw(text);

            foreach (var pitch in bestWave.Generate(Length))
            {
                
                var line = new Vertex[2];
                var newVertex = new Vertex(new Vector2f(x, (i+1)*128 - pitch/2));
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
}
