using NonlinearEquationSolution.Core.Entities;

namespace NonlinearEquationSolution.Application
{
    public static class ResultPrinter
    {
        public static void PrintResult(IEnumerable<SolverResult> results)
        {
            Console.WriteLine("+-------------------+----------------------+------------+--------------------+-----------------------------------+");
            Console.WriteLine("| Method            | Found Root           | Iterations | A Priori Estimate  | Comments                          |");
            Console.WriteLine("+-------------------+----------------------+------------+--------------------+-----------------------------------+");

            foreach (var result in results)
            {
                string aprioriStr = result.AprioriIterations > 0 ? result.AprioriIterations.ToString() : "N/A";
                Console.WriteLine($"| {result.MethodName,-17} | {result.Root,20:F10} | {result.AposterioriIterations,10} | {aprioriStr,18} | {result.Comments,-26} |");
            }

            Console.WriteLine("+-------------------+----------------------+------------+--------------------+-----------------------------------+");
        }
    }
}
