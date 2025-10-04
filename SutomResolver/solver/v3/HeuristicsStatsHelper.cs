namespace SutomResolver.solver.v3;

public class HeuristicsStatsHelper
{
    private static Dictionary<string, List<Dictionary<char, int>>> HeuristicValuesCache { get; set; } = [];

    public static List<Dictionary<char, int>> GetHeuristicValues(List<string> words, string pattern, bool useCache = true)
    {
        if (useCache && HeuristicValuesCache.ContainsKey(pattern))
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

        if (useCache)
        {
            HeuristicValuesCache.Add(pattern, heuristicsValues);
        }

        return heuristicsValues;
    }

    public static int CalculateHeuristicScore(List<Dictionary<char, int>> heuristicValues, string word)
    {
        int score = 0;

        for (int i = 0; i < word.Length; i++)
        {
            if (heuristicValues[i].TryGetValue(word[i], out int charScore))
            {
                score += charScore;
            }
        }

        score *= word.Distinct().Count();

        return score;
    }
}
