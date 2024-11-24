namespace SutomResolver.solver.v1;

/******************************************************************
* Games : 10000
* Wins : 5670
* Loses : 4330
* Turns : 3,9076
 *****************************************************************/

/// <summary>
/// Stratégie simple qui renvoie le premier mot de la liste répondant aux lettres trouvées et dévoilées.
/// </summary>
public class Solver : ISolver
{
    public List<string> RemainingWords { get; set; }
    private HashSet<char> AbsentLetters { get; set; } = new HashSet<char>();
    
    public void Initialize(string pattern)
    {
        RemainingWords = SutomHelper.LoadWordsFromFile(pattern.Length, pattern[0] != '_' ? pattern[0] : null);
        AbsentLetters = new HashSet<char>();
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
        RemainingWords = RemainingWords
            .Where(word => SolverHelper.MatchesPattern(word, result, misplacedLetters, AbsentLetters, null))
            .ToList();
    }
}
