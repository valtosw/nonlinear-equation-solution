using NonlinearEquationSolution.Core.Entities;
using NonlinearEquationSolution.Core.Interfaces;

namespace NonlinearEquationSolution.Infrastructure.Solvers
{
    public class RelaxationSolver : IEquationSolver
    {
        private const int MaxIterations = 1000;
        public string MethodName => "Relaxation Method";

        public SolverResult Solve(IEquation equation, ProblemDefinition problem, double epsilon)
        {
            double tau = CalculateOptimalTau(equation, problem.A, problem.B);
            double xPrev = problem.RelaxationInitialGuess;

            int aprioriIterations = EstimateAprioriIterations(equation, problem, epsilon);

            int iterations = 0;

            while (iterations < MaxIterations)
            {
                iterations++;

                int sign = Math.Sign(equation.Derivative(xPrev));
                double xNext = xPrev - sign * tau * equation.Function(xPrev);

                if (Math.Abs(xNext - xPrev) < epsilon)
                {
                    return new SolverResult(
                        MethodName,
                        xNext,
                        iterations,
                        aprioriIterations,
                        epsilon,
                        $"tau = {tau:F6}"
                    );
                }

                xPrev = xNext;
            }

            return new SolverResult(
                MethodName,
                double.NaN,
                iterations,
                aprioriIterations,
                epsilon,
                "Maximum iterations reached without convergence"
            );
        }

        private static double CalculateOptimalTau(IEquation equation, double a, double b)
        {
            double m1 = Math.Abs(equation.Derivative(a));
            double M1 = Math.Abs(equation.Derivative(b));

            return 2.0 / (m1 + M1);
        }

        // TODO: Implement this method to estimate the number of iterations a priori
        private static int EstimateAprioriIterations(IEquation equation, ProblemDefinition problem, double epsilon)
        {
            throw new NotImplementedException();
        }
    }
}
