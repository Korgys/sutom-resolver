using System.Globalization;
using System.Text;

namespace SutomResolver;

public static class SutomHelper
{
    private static readonly string _filePath = "../../../data/fr.txt";
    private static readonly Dictionary<int, List<string>> _loadWordsCache = new();
    private static List<string> _allWords;
    public static List<string> AllWords
    {
        get
        {
            if (_allWords == null)
            {
                _allWords = File.ReadAllLines(_filePath)
                    .Select(NormalizeString)
                    .Distinct()
                    .ToList();
            }
            return _allWords;
        }
    }

    public static string GetResultFromGuess(string guess, string targetWord)
    {
        int length = guess.Length;
        char[] result = new char[length];

        // Compte la fréquence des lettres du mot cible
        var counts = new Dictionary<char, int>();
        foreach (char c in targetWord)
        {
            counts[c] = counts.TryGetValue(c, out int val) ? val + 1 : 1;
        }

        // Première passe : bonnes positions
        for (int i = 0; i < length; i++)
        {
            if (guess[i] == targetWord[i])
            {
                result[i] = guess[i];   // lettre bien placée
                counts[guess[i]]--;     // on consomme la lettre
            }
            else
            {
                result[i] = '_';        // par défaut : inconnue
            }
        }

        // Deuxième passe : lettres mal placées
        for (int i = 0; i < length; i++)
        {
            if (result[i] == '_')
            {
                char c = guess[i];
                if (counts.TryGetValue(c, out int remaining) && remaining > 0)
                {
                    result[i] = '+';   // lettre présente ailleurs
                    counts[c]--;       // on consomme une occurrence
                }
            }
        }

        return new string(result);
    }


    public static List<string> LoadWordsFromFile(int size)
    {
        try
        {
            // Utilise un cache par taille de mot si disponible
            if (_loadWordsCache.TryGetValue(size, out var cachedWords))
            {
                return cachedWords;
            }

            var words = AllWords
                .Where(word => word.Length == size)
                .ToList();

            // Ajoute au cache les mots chargés de cette taille
            _loadWordsCache[size] = words;

            return words;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erreur lors de la lecture du fichier : {e.Message}");
            return new();
        }
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
