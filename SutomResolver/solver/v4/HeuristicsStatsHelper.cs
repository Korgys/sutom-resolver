
namespace SutomResolver.solver.v4;

public class HeuristicsStatsHelper
{
    private static Dictionary<string, List<Dictionary<char, int>>> HeuristicValuesCache { get; set; } = [];

    public static List<Dictionary<char, int>> GetHeuristicValues(List<string> words, string pattern)
    {
        if (HeuristicValuesCache.ContainsKey(pattern))
        {
            return HeuristicValuesCache[pattern];
        }

        var heuristicsValues = new List<Dictionary<char, int>>();

        for (int i = 0; i < pattern.Length; i++)
        {
            var heuristicValues = new Dictionary<char, int>();
            heuristicsValues.Add(heuristicValues);
        }

        foreach (var word in words)
        {
            for (int i = 0; i < pattern.Length; i++)
            {
                var heuristicValues = heuristicsValues[i];
                if (!heuristicValues.ContainsKey(word[i])) heuristicValues[word[i]] = 1;
                else heuristicValues[word[i]]++;
            }
        }

        HeuristicValuesCache.Add(pattern, heuristicsValues);

        return heuristicsValues;
    }

    public static int CalculateHeuristicScore(List<Dictionary<char, int>> heuristicValues, string word)
    {
        int score = 0;
        var uniqueChars = new HashSet<char>();

        for (int i = 0; i < word.Length; i++)
        {
            if (heuristicValues[i].TryGetValue(word[i], out int charScore))
            {
                score += charScore;
            }
            uniqueChars.Add(word[i]);
        }

        score *= uniqueChars.Count;

        return score;
    }

    public static int CalculateDiversityScore(List<string> remainingWords, string word)
    {
        throw new NotImplementedException();
    }
}
