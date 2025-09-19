using NonlinearEquationSolution.Core.Entities;
using NonlinearEquationSolution.Core.Interfaces;

namespace NonlinearEquationSolution.Infrastructure.Solvers
{
    public class NewtonSolver : IEquationSolver
    {
        public string MethodName => throw new NotImplementedException();

        public SolverResult Solve(IEquation equation, ProblemDefinition problem, double epsilon)
        {
            throw new NotImplementedException();
        }
    }
}
