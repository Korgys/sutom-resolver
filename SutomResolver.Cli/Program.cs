using SutomResolver.solver.v4;

namespace SutomResolver;

public static class Program
{
    public static void Main(string[] args)
    {
        var solver = new Solver();

        Console.Write("Entrez le pattern du mot à trouver (ex: L___) : ");
        var pattern = Console.ReadLine();
        solver.Initialize(pattern);
        var displayRules = true;

        while (true)
        {
            var guess = solver.GetNextGuess();
            if (string.IsNullOrEmpty(guess))
            {
                Console.WriteLine("Le solveur n'a trouvé aucun mot correspondant.");
                break;
            }

            if (solver.CandidatesWords.Count == 1)
            {
                Console.WriteLine($"Le solveur a trouvé : {guess}");
                break;
            }

            Console.WriteLine($"Le solveur propose : {guess}");

            if (displayRules)
            {
                Console.WriteLine("Testez ce mot et entrez la réponse avec : ");
                Console.WriteLine("- '?' ou '_' pour les lettres manquantes.");
                Console.WriteLine("- '+' pour les lettres mal placées.");
                Console.WriteLine("- la lettre si correcte.\n");
                displayRules = false;
            }

            Console.Write("Entrez le pattern du mot à trouver : ");
            var result = Console.ReadLine();

            if (guess == result)
            {
                Console.WriteLine("Le solveur a trouvé le mot !");
                break;
            }

            solver.ProcessResponse(guess, result);
        }
    }
}
