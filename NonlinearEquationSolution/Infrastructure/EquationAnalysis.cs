using NonlinearEquationSolution.Core.Interfaces;

namespace NonlinearEquationSolution.Infrastructure
{
    public static class EquationAnalysis
    {
        private const double FirstDerivativeVertexX = -1.0;

        public static (double m1, double M1) GetMinMaxAbsFirstDerivative(IEquation equation, double a, double b)
        {
            var pointsToCheck = new List<double> { a, b };

            if (a < FirstDerivativeVertexX && FirstDerivativeVertexX < b)
            {
                pointsToCheck.Add(FirstDerivativeVertexX);
            }

            var values = pointsToCheck.Select(p => Math.Abs(equation.Derivative(p)));

            return (values.Min(), values.Max());
        }

        public static (double m1, double M1) GetMinMaxAbsSecondDerivative(IEquation equation, double a, double b)
        {
            double valA = Math.Abs(equation.SecondDerivative(a));
            double valB = Math.Abs(equation.SecondDerivative(b));

            return (Math.Min(valA, valB), Math.Max(valA, valB));
        }
    }
}
