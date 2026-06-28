using SutomResolver.solver.v4;

namespace SutomResolver;

public static class Program
{
    public static void Main(string[] args)
    {
        var solver = new Solver();

        var pattern = ReadInput("Entrez le pattern du mot à trouver (ex: L___) : ");
        if (pattern is null)
        {
            Console.WriteLine("Pattern vide.");
            return;
        }

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

            var result = ReadInput("Entrez le pattern du mot à trouver : ");
            if (result is null)
            {
                Console.WriteLine("Réponse vide ignorée.");
                continue;
            }

            if (result.Length != guess.Length)
            {
                Console.WriteLine($"Réponse invalide ({guess.Length} caractères attendus).");
                continue;
            }

            if (guess == result)
            {
                Console.WriteLine("Le solveur a trouvé le mot !");
                break;
            }

            solver.ProcessResponse(guess, result);
        }
    }

    private static string? ReadInput(string prompt)
    {
        Console.Write(prompt);
        var input = Console.ReadLine()?.Trim().ToUpperInvariant();
        return string.IsNullOrWhiteSpace(input) ? null : input;
    }
}
