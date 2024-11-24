namespace SutomResolver.solver.v2;

/******************************************************************
* Games : 10000
* Wins : 9904
* Loses : 96
* Turns : 3,4242
* RunTime: 00:04:09.95
 *****************************************************************/

/// <summary>
/// Stratégie qui prend en compte les pattern impossibles pour filtrer.
/// </summary>
public class Solver : ISolver
{
    private HashSet<char> AbsentLetters { get; set; } = new HashSet<char>();
    private List<string> ImpossiblePatterns { get; set; } = new List<string>();
    public List<string> RemainingWords { get; set; }

    public void Initialize(string pattern)
    {
        AbsentLetters = new HashSet<char>();
        ImpossiblePatterns = new List<string>();
        RemainingWords = SutomHelper.LoadWordsFromFile(pattern.Length, pattern[0] != '_' ? pattern[0] : null);
    }

    public string GetNextGuess()
    {
        return RemainingWords
            .OrderByDescending(word => word.Distinct().Count())
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
