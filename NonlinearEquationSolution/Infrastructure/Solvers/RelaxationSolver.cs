using NonlinearEquationSolution.Core.Entities;
using NonlinearEquationSolution.Core.Interfaces;

namespace NonlinearEquationSolution.Infrastructure.Solvers
{
    public class RelaxationSolver : AbstractSolver
    {
        public override string MethodName => "Relaxation Method";

        protected override double GetInitialGuess(ProblemDefinition problem) => problem.RelaxationInitialGuess;

        protected override string PerformConvergenceCheck(IEquation equation, ProblemDefinition problem)
        {
            var (m1, M1) = EquationAnalysis.GetMinMaxAbsFirstDerivative(equation, problem.A, problem.B);
            return (0 < m1) && (m1 < M1) ? "Convergence conditions satisfied." : "0 > m1 or m1 > M1, convergence not guaranteed.";
        }

        protected override int PerformAprioriEstimation(IEquation equation, ProblemDefinition problem, double epsilon)
        {
            var (A, B) = (problem.A, problem.B);
            var (m1, M1) = EquationAnalysis.GetMinMaxAbsFirstDerivative(equation, A, B);

            if (m1 + M1 <= 0)
                return -1;

            var q0 = (M1 - m1) / (M1 + m1);

            if (q0 >= 1 || q0 <= 0)
                return -1;

            var initialError = Math.Max(Math.Abs(problem.RelaxationInitialGuess - A), Math.Abs(problem.RelaxationInitialGuess - B));
            var temp = Math.Log(initialError / epsilon) / Math.Log(1.0 / q0);

            return (int)Math.Floor(temp) + 1;
        }

        protected override (double? root, int iterations) RunIterationProcess(IEquation equation, ProblemDefinition problem, double epsilon)
        {
            var tau = CalculateOptimalTau(equation, problem.A, problem.B);
            var xPrev = GetInitialGuess(problem);
            var iterations = 0;

            while (iterations < MaxIterations)
            {
                iterations++;
                var sign = Math.Sign(equation.Derivative(xPrev));
                var xNext = xPrev - sign * tau * equation.Function(xPrev);

                if (Math.Abs(xNext - xPrev) < epsilon)
                    return (xNext, iterations);

                xPrev = xNext;
            }

            return (null, MaxIterations);
        }

        private static double CalculateOptimalTau(IEquation equation, double a, double b)
        {
            var (m1, M1) = EquationAnalysis.GetMinMaxAbsFirstDerivative(equation, a, b);
            return 2.0 / (m1 + M1);
        }
    }
}
