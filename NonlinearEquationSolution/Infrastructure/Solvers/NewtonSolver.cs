using NonlinearEquationSolution.Core.Entities;
using NonlinearEquationSolution.Core.Interfaces;

namespace NonlinearEquationSolution.Infrastructure.Solvers
{
    public class NewtonSolver : IEquationSolver
    {
        private const int MaxIterations = 1000;
        public string MethodName => "Newton's method";

        public SolverResult Solve(IEquation equation, ProblemDefinition problem, double epsilon)
        {
            double intialGuess = problem.NewtonInitialGuess;
            string convergenceMessage = CheckConvergenceConditions(equation, problem);
            double xPrev = intialGuess;

            int aprioriIterations = EstimateAprioriIterations(equation, problem, epsilon);

            int iterations = 0;

            while (iterations < MaxIterations)
            {
                iterations++;

                //if (equation.Derivative(xPrev) < 1e-12)
                //{
                //    return new SolverResult(
                //        MethodName,
                //        double.NaN,
                //        iterations,
                //        aprioriIterations,  
                //        epsilon,
                //        "Derivative too small, method fails"
                //    );
                //}

                double xNext = xPrev - equation.Function(xPrev) / equation.Derivative(xPrev);

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
            var messages = new List<string>();

            double x0 = problem.NewtonInitialGuess;

            if (equation.Function(x0) * equation.SecondDerivative(x0) <= 0)
            {
                messages.Add("f(x0) * f''(x0) <= 0, convergence not guaranteed.");
            }

            (double A, double B) = (problem.A, problem.B);

            var (m1, _) = GetMinMaxAbsForQuadratic(equation.Derivative, A, B);
            var (_, M2) = GetMinMaxAbsForLinear(equation.SecondDerivative, A, B);

            double initialError = B - A;

            double q = (M2 * initialError) / (2 * m1);

            if (q >= 1)
            {
                messages.Add("q >= 1, convergence not guaranteed.");
            }

            return messages.Count == 0 ? "Convergence conditions satisfied." : string.Join("; ", messages);
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

        private static (double min, double max) GetMinMaxAbsForLinear(Func<double, double> linearFunc, double a, double b)
        {
            double valA = Math.Abs(linearFunc(a));
            double valB = Math.Abs(linearFunc(b));

            return (Math.Min(valA, valB), Math.Max(valA, valB));
        }

        private static int EstimateAprioriIterations(IEquation equation, ProblemDefinition problem, double epsilon)
        {
            double m1 = Math.Min(Math.Abs(equation.Derivative(problem.A)), Math.Abs(equation.Derivative(problem.B)));
            double M2 = Math.Max(Math.Abs(equation.SecondDerivative(problem.A)), Math.Abs(equation.SecondDerivative(problem.B)));

            double initialError = Math.Max(Math.Abs(problem.NewtonInitialGuess - problem.A), Math.Abs(problem.NewtonInitialGuess - problem.B));
            double q = (M2 * initialError) / (2 * m1);

            double temp = Math.Log(initialError / epsilon) / Math.Log(1 / q);

            return (int)Math.Floor(Math.Log2(temp + 1)) + 1;
        }
    }
}
