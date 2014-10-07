namespace Engine
{
    public abstract class Termination<T>
    {
        public abstract bool Terminate(Engine<T> engine);
    }
}