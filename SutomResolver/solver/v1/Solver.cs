namespace SutomResolver.solver.v1;

/******************************************************************
 * Games : 10000
 * Wins : 5408
 * Loses : 4592
 * Turns : 3,8616
 * RunTime: 00:02:54.28
 *****************************************************************/

public class Solver : ISolver
{
    private List<string> Words { get; set; }
    private HashSet<char> AbsentLetters { get; set; } = new HashSet<char>();

    public Solver(string pattern)
    {
        Words = SutomHelper.LoadWordsFromFile(pattern.Length, pattern[0] != '_' ? pattern[0] : null);
    }

    public string GetNextGuess()
    {
        return Words
            .OrderByDescending(word => word.Distinct().Count())
            .FirstOrDefault()?.ToUpper();
    }

    public void ProcessResponse(string guess, string result)
    {
        var misplacedLetters = GetMisplacedLetters(guess, result);
        RefreshAbsentLetters(guess, result, misplacedLetters);
        Words = Words
            .Where(word => MatchesPattern(word, result, misplacedLetters))
            .ToList();
    }

    private bool MatchesPattern(string word, string pattern, HashSet<char> misplacedLetters)
    {
        for (int i = 0; i < word.Length; i++)
        {
            if (AbsentLetters.Contains(word[i])) return false;
            if (pattern[i] == '?' || pattern[i] == '_' || pattern[i] == word[i]) continue;
            if (pattern[i] == '+' && misplacedLetters.All(word.Contains)) continue;
            return false;
        }
        return true;
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
}
