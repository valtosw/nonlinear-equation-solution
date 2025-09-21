using NonlinearEquationSolution.Core.Entities;
using NonlinearEquationSolution.Core.Interfaces;

namespace NonlinearEquationSolution.Infrastructure.Solvers
{
    public abstract class AbstractSolver : IEquationSolver
    {
        public abstract string MethodName { get; }
        protected const int MaxIterations = 1000;

        public SolverResult Solve(IEquation equation, ProblemDefinition problem, double epsilon)
        {
            string convergenceMessage = PerformConvergenceCheck(equation, problem);
            int aprioriIterations = PerformAprioriEstimation(equation, problem, epsilon);

            var (root, iterations) = RunIterationProcess(equation, problem, epsilon);
            
            return new SolverResult(
                MethodName,
                root ?? double.NaN,
                iterations,
                aprioriIterations,
                epsilon,
                root.HasValue ? convergenceMessage : "Maximum iterations reached without convergence"
            );
        }

        protected abstract string PerformConvergenceCheck(IEquation equation, ProblemDefinition problem);
        protected abstract double GetInitialGuess(ProblemDefinition problem);
        protected abstract int PerformAprioriEstimation(IEquation equation, ProblemDefinition problem, double epsilon);
        protected abstract (double? root, int iterations) RunIterationProcess(IEquation equation, ProblemDefinition problem, double epsilon);
    }
}
