using System.Security.Cryptography;
using System.Text;

namespace SutomResolver.solver.v3;

public class HeuristicsStatsHelper
{
    private const int HeuristicValuesCacheMaxEntries = 128;
    private static readonly object HeuristicValuesCacheLock = new();
    private static readonly Dictionary<string, LinkedListNode<HeuristicValuesCacheEntry>> HeuristicValuesCache = [];
    private static readonly LinkedList<HeuristicValuesCacheEntry> HeuristicValuesCacheLru = [];

    private sealed record HeuristicValuesCacheEntry(string Key, List<Dictionary<char, int>> Values);

    public static List<Dictionary<char, int>> GetHeuristicValues(List<string> words, string pattern, bool useCache = true)
    {
        var cacheKey = useCache ? BuildCacheKey(pattern, words) : string.Empty;

        if (useCache)
        {
            lock (HeuristicValuesCacheLock)
            {
                if (HeuristicValuesCache.TryGetValue(cacheKey, out var cachedNode))
                {
                    HeuristicValuesCacheLru.Remove(cachedNode);
                    HeuristicValuesCacheLru.AddFirst(cachedNode);
                    return cachedNode.Value.Values;
                }
            }
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
                if (!heuristicValues.TryGetValue(word[i], out var count))
                {
                    heuristicValues[word[i]] = 1;
                }
                else
                {
                    heuristicValues[word[i]] = count + 1;
                }
            }
        }

        if (useCache)
        {
            var cacheEntry = new HeuristicValuesCacheEntry(cacheKey, heuristicsValues);

            lock (HeuristicValuesCacheLock)
            {
                if (HeuristicValuesCache.TryGetValue(cacheKey, out var existingNode))
                {
                    HeuristicValuesCacheLru.Remove(existingNode);
                    HeuristicValuesCache.Remove(cacheKey);
                }

                var node = new LinkedListNode<HeuristicValuesCacheEntry>(cacheEntry);
                HeuristicValuesCacheLru.AddFirst(node);
                HeuristicValuesCache.Add(cacheKey, node);

                while (HeuristicValuesCache.Count > HeuristicValuesCacheMaxEntries)
                {
                    var lruNode = HeuristicValuesCacheLru.Last;
                    if (lruNode is null)
                    {
                        break;
                    }

                    HeuristicValuesCacheLru.RemoveLast();
                    HeuristicValuesCache.Remove(lruNode.Value.Key);
                }
            }
        }

        return heuristicsValues;
    }

    private static string BuildCacheKey(string pattern, List<string> words)
    {
        var wordsFingerprint = string.Join('\u001F', words);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(wordsFingerprint));
        return $"{pattern.Length}:{pattern}:{words.Count}:{Convert.ToHexString(hash)}";
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
