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
            string convergenceMessage = CheckConvergenceConditions(equation, problem);
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
                        convergenceMessage
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

        private static string CheckConvergenceConditions(IEquation equation, ProblemDefinition problem)
        {
            var (m1, M1) = GetMinMaxAbsForQuadratic(equation.Derivative, problem.A, problem.B);

            return (0 < m1) && (m1 < M1) ? "Convergence conditions satisfied." : "0 > m1 or m1 > M1, convergence not guaranteed.";
        }

        private static double CalculateOptimalTau(IEquation equation, double a, double b)
        {
            var (m1, M1) = GetMinMaxAbsForQuadratic(equation.Derivative, a, b);

            return 2.0 / (m1 + M1);
        }

        private static (double min, double max) GetMinMaxAbsForQuadratic(Func<double, double> quadFunc, double a, double b)
        {
            const double vertexX = -1.0;

            var poinstTocheck = new List<double> { a, b };

            if (a < vertexX && vertexX < b)
            {
                poinstTocheck.Add(vertexX);
            }

            var values = poinstTocheck.Select(p => Math.Abs(quadFunc(p)));

            return (values.Min(), values.Max());
        }

        private static int EstimateAprioriIterations(IEquation equation, ProblemDefinition problem, double epsilon)
        {
            var (m1, M1) = GetMinMaxAbsForQuadratic(equation.Derivative, problem.A, problem.B);

            if (m1 + M1 <= 0)
            {
                return -1;
            }

            double q0 = (M1 - m1) / (M1 + m1);

            if (q0 >= 1 || q0 <= 0)
            {
                return -1;
            }

            double initialError = Math.Max(Math.Abs(problem.RelaxationInitialGuess - problem.A), Math.Abs(problem.RelaxationInitialGuess - problem.B));

            double temp = Math.Log(initialError / epsilon) / Math.Log(1 / q0);

            return (int)Math.Floor(temp) + 1;
        }
    }
}
