using System.Globalization;
using System.Text;

namespace SutomResolver;

public static class SutomHelper
{
    private static readonly string _filePath = ResolveWordListPath();
    private static readonly Dictionary<(int Size, char? FirstLetter), List<string>> _loadWordsCache = new();
    private static List<string> _allWords;
    public static List<string> AllWords
    {
        get
        {
            _allWords ??= File.ReadAllLines(_filePath)
                    .Select(NormalizeString)
                    .Distinct()
                    .ToList();
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

    public static List<string> LoadWordsFromFile(int size, char? firstLetter = null)
    {
        try
        {
            var normalizedFirstLetter = NormalizeFirstLetter(firstLetter);
            var cacheKey = (Size: size, FirstLetter: normalizedFirstLetter);

            if (_loadWordsCache.TryGetValue(cacheKey, out var cachedWords))
            {
                return cachedWords;
            }

            var words = AllWords
                .Where(word => word.Length == size && (normalizedFirstLetter == null || normalizedFirstLetter == word[0]))
                .ToList();

            _loadWordsCache[cacheKey] = words;

            return words;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erreur lors de la lecture du fichier : {e.Message}");
            return new();
        }
    }

    private static char? NormalizeFirstLetter(char? firstLetter)
    {
        if (firstLetter == null)
        {
            return null;
        }

        var normalized = NormalizeString(firstLetter.Value.ToString());
        return normalized.Length > 0 ? normalized[0] : null;
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

    private static string ResolveWordListPath()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var directCandidate = Path.Combine(directory.FullName, "data", "fr.txt");
            if (File.Exists(directCandidate))
            {
                return directCandidate;
            }

            var solutionRoot = Path.Combine(directory.FullName, "SutomResolver.sln");
            if (File.Exists(solutionRoot))
            {
                var projectCandidates = new[]
                {
                    Path.Combine(directory.FullName, "SutomResolver", "data", "fr.txt"),
                    Path.Combine(directory.FullName, "SutomResolver.Tests", "data", "fr.txt"),
                    Path.Combine(directory.FullName, "SutomResolver.Cli", "data", "fr.txt"),
                    Path.Combine(directory.FullName, "SutomResolver.Core", "data", "fr.txt")
                };

                foreach (var candidate in projectCandidates)
                {
                    if (File.Exists(candidate))
                    {
                        return candidate;
                    }
                }
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException("Unable to locate data/fr.txt from the application base directory.", "fr.txt");
    }
}
