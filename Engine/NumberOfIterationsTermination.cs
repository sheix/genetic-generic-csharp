namespace Engine
{
    public class NumberOfIterationsTermination<T> : Termination<T>
    {
        public NumberOfIterationsTermination(int iterations)
        {
            _iterations = iterations;
        }
        private readonly int _iterations;
        
        public override bool Terminate(Engine<T> engine)
        {
            if (engine.Iteration == _iterations)
                return true;
            return false;
        }
    }
}