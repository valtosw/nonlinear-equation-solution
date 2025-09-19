namespace NonlinearEquationSolution.Core.Interfaces
{
    public interface IEquation
    {
        string Definition { get; }
        Func<double, double> Function { get; }
        Func<double, double> Derivative { get; }
        Func<double, double> SecondDerivative { get; }
    }
}
