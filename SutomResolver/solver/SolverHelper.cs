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
        HashSet<char>? misplacedLetters,
        HashSet<char>? absentLetters,
        HashSet<string>? impossiblePatterns)
    {
        // Vérifie la longueur du mot
        int n = word.Length;
        if (pattern.Length != n) return false;

        // Le mot doit contenir TOUTES les lettres "mal placées"
        if (misplacedLetters is { Count: > 0 })
        {
            foreach (var c in misplacedLetters)
            {
                if (word.IndexOf(c) < 0) return false;
            }
        }

        // Lettres interdites par position (à partir des "impossiblePatterns")
        HashSet<char>[]? forbidden = null;
        if (impossiblePatterns is { Count: > 0 })
        {
            forbidden = new HashSet<char>[n];
            for (int i = 0; i < n; i++) forbidden[i] = new HashSet<char>();

            foreach (var ip in impossiblePatterns)
            {
                if (string.IsNullOrEmpty(ip)) continue;
                int m = Math.Min(ip.Length, n);
                for (int i = 0; i < m; i++)
                {
                    char f = ip[i];
                    // On n'ajoute que de vraies lettres interdites (pas '_', '?', '+')
                    if (f != '_' && f != '?' && f != '+') forbidden[i].Add(f);
                }
            }
        }

        // Vérif positionnelle
        for (int i = 0; i < n; i++)
        {
            char p = pattern[i];
            char c = word[i];

            // Si le pattern fixe une lettre explicite à cette position, elle doit matcher
            if (p != '_' && p != '?' && p != '+')
            {
                if (c != p) return false;
                if (forbidden is not null && forbidden[i].Contains(c)) return false;
                continue;
            }

            // Dans tous les cas, ne pas violer une interdiction explicite à cette position
            if (forbidden is not null && forbidden[i].Contains(c)) return false;

            if (p == '+')
            {
                // '+' = lettre présente dans le mot mais pas à cette position.
                // La présence globale a déjà été vérifiée au pré-check.
                // Le "pas à cette position" est géré par forbidden[] construit depuis les patterns impossibles.
                continue;
            }

            // '_' ou '?' : cette lettre ne doit pas faire partie des "absentes"
            if (absentLetters is not null && absentLetters.Contains(c)) return false;
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
