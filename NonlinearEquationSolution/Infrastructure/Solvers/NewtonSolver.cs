using NonlinearEquationSolution.Core.Entities;
using NonlinearEquationSolution.Core.Interfaces;

namespace NonlinearEquationSolution.Infrastructure.Solvers
{
    public class NewtonSolver : AbstractSolver
    {
        public override string MethodName => "Newton's method";

        protected override double GetInitialGuess(ProblemDefinition problem) => problem.NewtonInitialGuess;

        // TODO: Fix convergence conditions check for x = -1
        protected override string PerformConvergenceCheck(IEquation equation, ProblemDefinition problem)
        {
            var messages = new List<string>();

            var (A, B) = (problem.A, problem.B);
            double x0 = GetInitialGuess(problem);

            if (equation.Function(x0) * equation.SecondDerivative(x0) <= 0)
                messages.Add("f(x0) * f''(x0) <= 0, convergence not guaranteed.");


            var (m1, _) = EquationAnalysis.GetMinMaxAbsFirstDerivative(equation, A, B);
            var (_, M2) = EquationAnalysis.GetMinMaxAbsSecondDerivative(equation, A, B);

            double initialError = B - A;

            double q = (M2 * initialError) / (2 * m1);

            if (q >= 1)
                messages.Add("q >= 1, convergence not guaranteed.");

            return messages.Count == 0 ? "Convergence conditions satisfied." : string.Join("; ", messages);
        }

        protected override int PerformAprioriEstimation(IEquation equation, ProblemDefinition problem, double epsilon)
        {
            var (A, B) = (problem.A, problem.B);
            var (m1, _) = EquationAnalysis.GetMinMaxAbsFirstDerivative(equation, A, B);
            var (_, M2) = EquationAnalysis.GetMinMaxAbsSecondDerivative(equation, A, B);
            var initialGuess = GetInitialGuess(problem);

            double initialError = Math.Max(Math.Abs(initialGuess - A), Math.Abs(initialGuess - B));
            double q = (M2 * initialError) / (2 * m1);

            double temp = Math.Log(initialError / epsilon) / Math.Log(1 / q);

            return (int)Math.Floor(Math.Log2(temp + 1)) + 1;
        }

        protected override (double? root, int iterations) RunIterationProcess(IEquation equation, ProblemDefinition problem, double epsilon)
        {
            double xPrev = GetInitialGuess(problem);

            int iterations = 0;

            while (iterations < MaxIterations)
            {
                iterations++;

                double xNext = xPrev - equation.Function(xPrev) / equation.Derivative(xPrev);

                if (Math.Abs(xNext - xPrev) < epsilon)
                {
                    return (xNext, iterations);
                }

                xPrev = xNext;
            }

            return (null, MaxIterations);
        }
    }
}
