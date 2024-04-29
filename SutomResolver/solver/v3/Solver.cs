namespace SutomResolver.solver.v3;

/******************************************************************
 * Games : 10000
 * Wins : 9730
 * Loses : 270
 * Turns : 3,2546
 * RunTime: 00:04:18.42
 *****************************************************************/

public class Solver : ISolver
{
    public List<string> ImpossiblePatterns { get; set; } = new List<string>();
    public HashSet<char> AbsentLetters { get; set; } = new HashSet<char>();
    public List<string> Words { get; set; }
    public List<Dictionary<char, int>> HeuristicValues { get; set; }

    public Solver(string pattern)
    {
        Words = SutomHelper.LoadWordsFromFile(pattern.Length, pattern[0] != '_' ? pattern[0] : null);
        HeuristicValues = HeuristicsStatsHelper.GetHeuristicValues(Words, pattern);
    }

    public string GetNextGuess()
    {
        var bestMatches = Words
            .Select(word => 
                new 
                { 
                    Word = word, 
                    Score = HeuristicsStatsHelper.CalculateHeuristicScore(HeuristicValues, word) 
                })
            .OrderByDescending(w => w.Score);

        return bestMatches
            .Select(w => w.Word)
            .FirstOrDefault()?.ToUpper();
    }

    public void ProcessResponse(string guess, string result)
    {
        var misplacedLetters = GetMisplacedLetters(guess, result);
        RefreshAbsentLetters(guess, result, misplacedLetters);        
        ImpossiblePatterns.Add(GetImpossiblePattern(guess, result));
        Words = Words
            .Where(word => word != guess && MatchesPattern(word, result, misplacedLetters))
            .ToList();
    }

    private bool MatchesPattern(string word, string pattern, HashSet<char> misplacedLetters)
    {
        for (int i = 0; i < word.Length; i++)
        {
            if (pattern[i] == word[i]) continue;
            if (AbsentLetters.Any(l => l == word[i] && !pattern.Contains(l))) return false;
            if (pattern[i] == '?' || pattern[i] == '_') continue;
            if (pattern[i] == '+' && misplacedLetters.All(word.Contains) && ImpossiblePatterns.All(ip => ip[i] != word[i])) continue;
            return false;
        }

        return true;
    }

    private HashSet<char> RefreshAbsentLetters(string word, string pattern, HashSet<char> misplacedLetters)
    {
        for (int i = 0; i < pattern.Length; i++)
        {
            if (pattern[i] == '_' && !AbsentLetters.Contains(word[i]) && !misplacedLetters.Contains(word[i]))
            {
                AbsentLetters.Add(word[i]);
            }
        }
        return AbsentLetters;
    }

    private HashSet<char> GetMisplacedLetters(string guess, string result)
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

    private string GetImpossiblePattern(string guess, string result)
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
}

