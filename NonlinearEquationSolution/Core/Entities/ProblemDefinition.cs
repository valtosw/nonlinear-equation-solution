namespace NonlinearEquationSolution.Core.Entities
{
    public record ProblemDefinition(
        string Description,
        double A,
        double B,
        double RelaxationInitialGuess = double.NaN,
        double NewtonInitialGuess = double.NaN);
}
