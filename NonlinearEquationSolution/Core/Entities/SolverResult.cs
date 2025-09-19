namespace NonlinearEquationSolution.Core.Entities
{
    public record SolverResult(
        string MethodName,
        double Root,
        int AposterioriIterations,
        int AprioriIterations,
        double Epsilon,
        string Comments);
}
