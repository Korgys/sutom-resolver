namespace SutomResolver.solver;

internal static class SolverHelper
{
    internal static HashSet<char> GetMisplacedLetters(string guess, string result)
    {
        var misplacedLetters = new HashSet<char>();
        for (int i = 0; i < result.Length; i++)
        {
            if (result[i] == '+')
            {
                misplacedLetters.Add(guess[i]);
            }
        }
        return misplacedLetters;
    }

    internal static string GetImpossiblePattern(string guess, string result)
    {
        var impossiblePattern = "";
        for (int i = 0; i < result.Length; i++)
        {
            if (result[i] == '+')
            {
                impossiblePattern += guess[i];
            }
            else
            {
                impossiblePattern += "_";
            }
        }
        return impossiblePattern;
    }

    internal static bool MatchesPattern(
        string word,
        string pattern,
        IEnumerable<(string Guess, string Result)>? guessResultPairs)
    {
        if (pattern.Length != word.Length)
        {
            return false;
        }

        var constraints = SutomConstraints.CreateEmpty(word.Length);

        if (guessResultPairs is not null)
        {
            foreach (var (guess, result) in guessResultPairs)
            {
                constraints = constraints.Merge(SutomConstraints.FromGuessAndResult(guess, result));
            }
        }

        if (!constraints.Matches(word))
        {
            return false;
        }

        // Compatibilité avec le pattern historique: toute lettre explicite est fixe.
        for (int i = 0; i < pattern.Length; i++)
        {
            var patternChar = pattern[i];
            if (patternChar != '_' && patternChar != '?' && patternChar != '+' && word[i] != patternChar)
            {
                return false;
            }
        }

        return true;
    }


    internal static void UpdateAbsentLetters(HashSet<char> absentLetters, string word, string pattern, HashSet<char> misplacedLetters)
    {
        for (int i = 0; i < pattern.Length; i++)
        {
            if (pattern[i] == '_')
            {
                char c = word[i];
                if (!misplacedLetters.Contains(c))
                    absentLetters.Add(c); // Si doublon, il sera ignoré par le HashSet
            }
        }
    }

    /// <summary>
    /// Méthode utilitaire pour compter le nombre de lettres distinctes dans un mot.
    /// Plus performant que word.Distinct().Count() car évite la création d'une collection intermédiaire.
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public static int CountDistinctLetters(string word)
    {
        var set = new HashSet<char>();
        foreach (var letter in word) set.Add(letter);
        return set.Count;
    }
}
