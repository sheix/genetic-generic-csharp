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
            //_myWave.AddWave(new SquareWave(127, 10, 10, 0));
            _myWave.AddWave(new TriangleWave(63, 25, 25, 0, 0));


            _engine.SetFitnessFunction(AbsoluteDifference);
            _engine.PopulationSize = 400;
            _engine.Populate();
            _engine.SurvivorsPercent = 50;
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
            _engine.AddMutation(m => {if (m is ResultWave)
            m.Waves().RemoveAt(random.Next(m.Waves().Count));
            });
            _engine.AddMutation(m=> {if (m is ResultWave) 
            m.Waves().Add(RandomWaveFactory.GetNewSquareWave());});
            _engine.AddMutation(m =>
            {
                if (m is ResultWave)
                    m.Waves().Add(RandomWaveFactory.GetNewTriangleWave());
            });
            //_engine.AddMutation(m => { if (m is ResultWave) TransformWave(m); });    

            //_engine.SetTerminator(_engine.SameFitnessFunctionFor(10));
            var window = new RenderWindow(VideoMode.DesktopMode, "Test");
            window.Closed += OnClosed;
            window.KeyPressed += OnKeyPressed;

            _engine.RunIterations(); 

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
            return a.Generate(256).AbsoluteDifference(_myWave.Generate(256));
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

            foreach (var pitch in bestWave.Generate(256))
            {
                
                var line = new Vertex[2];
                var newVertex = new Vertex(new Vector2f(x, i*128 + pitch/2));
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
