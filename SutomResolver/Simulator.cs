using SutomResolver.solver.v3;
using System.Globalization;
using System.Text;

namespace SutomResolver;

public class Simulator
{
    public int NumberOfGames { get; set; } = 10;
    public float Turns { get; set; } = 0;
    public float Wins { get; set; } = 0;
    public float Loses { get; set; } = 0;

    public void EmulateGames(bool displayLogs = true)
    {
        Random random = new Random();
        int turns;

        for (int i = 1; i <= NumberOfGames; i++)
        {
            // Mot à deviner
            var targetWord = NormalizeString(SutomHelper.AllWords[random.Next(SutomHelper.AllWords.Count)].ToUpper());
            Console.WriteLine($"{i}) Mot à deviner : {targetWord}");

            // Exemple ____ pour LAIT
            var pattern = new string('_', targetWord.Length);
            var solver = new Solver(pattern);
            turns = 1;
            var maxTurns = 6;

            while (turns <= maxTurns)
            {
                var guess = solver.GetNextGuess();
                if (string.IsNullOrEmpty(guess))
                {
                    if (displayLogs)
                    {
                        Console.WriteLine("Le solveur n'a trouvé aucun mot correspondant.");
                    }
                    Loses++;
                    break;
                }

                if (displayLogs)
                {
                    Console.WriteLine($"Le solveur propose : {guess}");
                }

                if (guess == targetWord)
                {
                    if (displayLogs)
                    {
                        Console.WriteLine("Le solveur a trouvé le mot !");
                    }
                    Wins++;
                    break;
                }

                var result = SutomHelper.GetResultFromGuess(guess, targetWord);
                if (displayLogs)
                {
                    Console.WriteLine($"Réponse: {result}");
                }
                
                solver.ProcessResponse(guess, result);
                turns++;
            }

            if (turns > maxTurns)
            {
                Loses++;
            }
            Turns += turns;
        }

        Turns /= NumberOfGames;
    }

    public static void GuessWord()
    {
        Console.Write("Entrez le pattern du word à trouver (ex: L___) : ");
        var pattern = Console.ReadLine();
        var solver = new Solver(pattern);
        var displayRules = true;

        while (true)
        {
            var guess = solver.GetNextGuess();
            if (string.IsNullOrEmpty(guess))
            {
                Console.WriteLine("Le solveur n'a trouvé aucun mot correspondant.");
                break;
            }

            if (solver.Words.Count == 1)
            {
                Console.WriteLine($"Le solveur a trouvé : {guess}");
                break;
            }

            Console.WriteLine($"Le solveur propose : {guess}");

            if (displayRules)
            {
                Console.WriteLine($"Testez ce mot et entrez la retour avec : ");
                Console.WriteLine("- '?' ou '_' pour les lettres manquantes.");
                Console.WriteLine("- '+' pour les lettres mal placées.");
                Console.WriteLine("- la lettre si correcte.\n");
                displayRules = false;
            }

            Console.Write($"Entrez le pattern du word à trouver : ");
            var result = Console.ReadLine();

            if (guess == result)
            {
                Console.WriteLine("Le solveur a trouvé le mot !");
                break;
            }

            solver.ProcessResponse(guess, result);
        }
    }

    public void DisplayStatsResult()
    {
        Console.WriteLine($"Games : {NumberOfGames}");
        Console.WriteLine($"Wins : {Wins}");
        Console.WriteLine($"Loses : {Loses}");
        Console.WriteLine($"Turns : {Turns}");
    }

    private static string NormalizeString(string input)
    {
        var normalizedString = input.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToUpper();
    }
}
