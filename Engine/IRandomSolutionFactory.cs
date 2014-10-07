namespace Engine
{
    public interface IRandomSolutionFactory<out T>
    {
        T GetPossibleSolution();
    }
}