using NonlinearEquationSolution.Core.Entities;

namespace NonlinearEquationSolution.Core.Interfaces
{
    public interface IEquationSolver
    {
        string MethodName { get; }
        SolverResult Solve(IEquation equation, ProblemDefinition problem, double epsilon);
    }
}
