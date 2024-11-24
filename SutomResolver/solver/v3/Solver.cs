namespace SutomResolver.solver.v3;

/******************************************************************
* Games : 10000
* Wins : 9889
* Loses : 111
* Turns : 3,2738
* RunTime: 00:04:18.42
 *****************************************************************/

/// <summary>
/// Stratégie qui se base sur une analyse heuristique pour proposer 
/// d'abord les mots ayant les lettres qui ont plus de chances d'apparaitre.
/// </summary>
public class Solver : ISolver
{
    public List<string> ImpossiblePatterns { get; set; } = new List<string>();
    public HashSet<char> AbsentLetters { get; set; } = new HashSet<char>();
    public List<string> RemainingWords { get; set; }
    public List<Dictionary<char, int>> HeuristicValues { get; set; }

    public void Initialize(string pattern)
    {
        AbsentLetters = new HashSet<char>();
        ImpossiblePatterns = new List<string>();
        RemainingWords = SutomHelper.LoadWordsFromFile(pattern.Length, pattern[0] != '_' ? pattern[0] : null);
        HeuristicValues = HeuristicsStatsHelper.GetHeuristicValues(RemainingWords, pattern);
    }

    public string GetNextGuess()
    {
        var bestMatches = RemainingWords
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
        var misplacedLetters = SolverHelper.GetMisplacedLetters(guess, result);
        SolverHelper.UpdateAbsentLetters(AbsentLetters, guess, result, misplacedLetters);        
        ImpossiblePatterns.Add(SolverHelper.GetImpossiblePattern(guess, result));
        RemainingWords = RemainingWords
            .Where(word => word != guess && SolverHelper.MatchesPattern(word, result, misplacedLetters, AbsentLetters, ImpossiblePatterns))
            .ToList();
    }
}

