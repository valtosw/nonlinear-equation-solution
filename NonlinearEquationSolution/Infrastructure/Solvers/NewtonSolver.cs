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
            // string convergenceMessage = CheckConvergenceConditions(equation, intialGuess);
            string convergenceMessage = "Convergence conditions not checked"; // TODO: Implement this method
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

        private static string CheckConvergenceConditions(IEquation equation, double x0)
        {
            throw new NotImplementedException();
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
