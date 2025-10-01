namespace SutomResolver.solver.v1;

/// <summary>
/// Stratégie simple qui renvoie le premier mot de la liste répondant aux lettres trouvées et dévoilées.
/// Elimine les mots contenant des lettres absentes ou ceux ne correspondant pas au pattern.
/// </summary>
/// <remarks>
/// Games : 10000 | Win ratio : 78,12% | Turns per game : 4,3807
/// </remarks>
public class Solver : ISolver
{
    private HashSet<char> AbsentLetters { get; set; }
    public List<string> RemainingWords { get; set; }

    public void Initialize(string pattern)
    {
        AbsentLetters = [];
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
        RemainingWords = RemainingWords
            .Where(word => SolverHelper.MatchesPattern(word, result, misplacedLetters, AbsentLetters, null))
            .ToList();
    }
}
