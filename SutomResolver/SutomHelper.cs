using System.Globalization;
using System.Text;

namespace SutomResolver;

public static class SutomHelper
{
    private static readonly string _filePath = "../../../data/fr.txt";
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
        char[] result = new char[guess.Length];
        bool[] guessed = new bool[targetWord.Length]; // Pour marquer les lettres déjà traitées dans le mot cible

        // D'abord, identifier les lettres correctement placées
        for (int i = 0; i < guess.Length; i++)
        {
            if (guess[i] == targetWord[i])
            {
                result[i] = guess[i];
                guessed[i] = true;
            }
            else
            {
                result[i] = '_'; // Utiliser '_' comme placeholder pour les lettres non traitées
            }
        }

        // Ensuite, identifier les lettres mal placées
        for (int i = 0; i < guess.Length; i++)
        {
            if (result[i] == '_')
            {
                for (int j = 0; j < targetWord.Length; j++)
                {
                    if (guess[i] == targetWord[j] && !guessed[j])
                    {
                        result[i] = '+'; // Marquer la lettre comme mal placée
                        guessed[j] = true; // Marquer comme traitée dans le mot cible
                        break;
                    }
                }
            }
        }

        return new string(result);
    }

    public static List<string> LoadWordsFromFile(int size, char? firstLetter = null)
    {
        try
        {
            return AllWords
                .Where(word => word.Length == size && (firstLetter == null || word[0] == firstLetter))
                .ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erreur lors de la lecture du fichier : {e.Message}");
            return new List<string>();
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
