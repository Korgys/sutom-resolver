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
        HashSet<char> misplacedLetters, 
        HashSet<char> absentLetters,
        List<string> impossiblePatterns)
    {
        for (int i = 0; i < word.Length; i++)
        {
            // Les lettres correspondent
            if (pattern[i] == word[i])
                continue;

            // Vérification des lettres absentes
            if (absentLetters != null &&
                absentLetters.Any(l => l == word[i] && !pattern.Contains(l)))
            {
                return false;
            }

            // Lettres inconnues ou "joker" dans le pattern
            if (pattern[i] == '?' || pattern[i] == '_')
                continue;

            // Lettres mal placées et patterns impossibles
            if (pattern[i] == '+')
            {
                // Vérification de la cohérence avec misplacedLetters
                if (misplacedLetters != null && misplacedLetters.All(word.Contains))
                {
                    // Vérification de la sécurité d'accès à ImpossiblePatterns
                    if (impossiblePatterns != null &&
                        impossiblePatterns.All(ip => ip.Length > i && ip[i] != word[i]))
                    {
                        continue;
                    }
                }
            }

            // Si aucune des conditions n'est remplie, le mot ne correspond pas
            return false;
        }

        return true;
    }

    internal static HashSet<char> UpdateAbsentLetters(HashSet<char> absentLetters, string word, string pattern, HashSet<char> misplacedLetters)
    {
        for (int i = 0; i < pattern.Length; i++)
        {
            if (pattern[i] == '_' && !absentLetters.Contains(word[i]) && !misplacedLetters.Contains(word[i]))
            {
                absentLetters.Add(word[i]);
            }
        }
        return absentLetters;
    }
}
