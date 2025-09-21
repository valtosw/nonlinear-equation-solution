using NonlinearEquationSolution.Core.Interfaces;

namespace NonlinearEquationSolution.Core.Entities
{
    public class Equation : IEquation
    {
        public string Definition => "x^3 + 3x^2 - x - 3 = 0";
        public Func<double, double> Function => x => Math.Pow(x, 3) + 3 * Math.Pow(x, 2) - x - 3;
        public Func<double, double> Derivative => x => 3 * Math.Pow(x, 2) + 6 * x - 1;
        public Func<double, double> SecondDerivative => x => 6 * x + 6;
    }
}
