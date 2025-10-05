using Microsoft.Extensions.Configuration;
using NonlinearEquationSolution.Core.Entities;
using NonlinearEquationSolution.Infrastructure.Solvers;

namespace NonlinearEquationSolution.Application
{
    public static class Program
    {
        public static void Main()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var problems = configuration.GetSection("Problems").Get<List<ProblemDefinition>>() ?? [];

            var equation = new Equation();
            var relaxationSolver = new RelaxationSolver();
            var newtonSolver = new NewtonSolver();

            Console.WriteLine($"Solving the equation: {equation.Definition}");
            Console.WriteLine("------------------------------------------------------------\n");

            double epsilon = GetEpsilonFromUser();

            foreach (var problem in problems)
            {
                Console.WriteLine($"\n===== Analyzing {problem.Description} on interval [{problem.A}, {problem.B}] =====");

                var relaxationResult = relaxationSolver.Solve(equation, problem, epsilon);
                var newtonResult = newtonSolver.Solve(equation, problem, epsilon);

                ResultPrinter.PrintResult([relaxationResult, newtonResult]);
            }
        }

        private static double GetEpsilonFromUser()
        {
            Console.Write("Enter precision: ");

            if (Double.TryParse(Console.ReadLine(), out double epsilon) && epsilon > 0)
            {
                Console.WriteLine($"Using precision epsilon = {epsilon}");
                return epsilon;
            }

            Console.WriteLine("Invalid input. Using default precision epsilon = 1e-3");

            return 1e-3;
        }
    }
}