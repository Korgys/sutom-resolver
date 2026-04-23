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
        var patternLength = pattern.Length;
        var cacheKey = useCache ? BuildCacheKey(pattern, words) : string.Empty;

        if (useCache && TryGetCachedHeuristicValues(cacheKey, out var cachedValues))
        {
            return cachedValues;
        }

        var heuristicsValues = CreateHeuristicValues(words, patternLength);

        if (useCache)
        {
            CacheHeuristicValues(cacheKey, heuristicsValues);
        }

        return heuristicsValues;
    }

    private static List<Dictionary<char, int>> CreateHeuristicValues(List<string> words, int patternLength)
    {
        var heuristicsValues = new List<Dictionary<char, int>>(patternLength);

        for (int i = 0; i < patternLength; i++)
        {
            heuristicsValues.Add([]);
        }

        foreach (var word in words)
        {
            for (int i = 0; i < patternLength; i++)
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

        return heuristicsValues;
    }

    private static bool TryGetCachedHeuristicValues(string cacheKey, out List<Dictionary<char, int>> cachedValues)
    {
        lock (HeuristicValuesCacheLock)
        {
            if (!HeuristicValuesCache.TryGetValue(cacheKey, out var cachedNode))
            {
                cachedValues = [];
                return false;
            }

            HeuristicValuesCacheLru.Remove(cachedNode);
            HeuristicValuesCacheLru.AddFirst(cachedNode);
            cachedValues = cachedNode.Value.Values;
            return true;
        }
    }

    private static void CacheHeuristicValues(string cacheKey, List<Dictionary<char, int>> heuristicsValues)
    {
        lock (HeuristicValuesCacheLock)
        {
            if (HeuristicValuesCache.TryGetValue(cacheKey, out var existingNode))
            {
                HeuristicValuesCacheLru.Remove(existingNode);
                HeuristicValuesCache.Remove(cacheKey);
            }

            var cacheEntry = new HeuristicValuesCacheEntry(cacheKey, heuristicsValues);
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
