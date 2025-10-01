namespace SutomResolver.solver.v2;

/// <summary>
/// Stratégie qui prend en compte les pattern impossibles pour filtrer.
/// </summary>
/// <remarks>
/// Games : 10000 | Win ratio : 95,83% | Turns per game : 3,4934
/// </remarks>
public class Solver : ISolver
{
    private HashSet<char> AbsentLetters { get; set; }
    private List<string> ImpossiblePatterns { get; set; } // Représente la liste des pattern impossibles pour le mot à trouver
    public List<string> RemainingWords { get; set; }

    public void Initialize(string pattern)
    {
        AbsentLetters = [];
        ImpossiblePatterns = [];
        RemainingWords = SutomHelper.LoadWordsFromFile(pattern.Length, pattern[0] != '_' ? pattern[0] : null);
    }

    public string GetNextGuess()
    {
        return RemainingWords
            .OrderByDescending(word => word.Distinct().Count())
            .FirstOrDefault()?
            .ToUpper();
    }

    public void ProcessResponse(string guess, string result)
    {
        var misplacedLetters = SolverHelper.GetMisplacedLetters(guess, result);
        SolverHelper.UpdateAbsentLetters(AbsentLetters, guess, result, misplacedLetters);
        ImpossiblePatterns.Add(SolverHelper.GetImpossiblePattern(guess, result));
        RemainingWords = RemainingWords
            .Where(word => SolverHelper.MatchesPattern(word, result, misplacedLetters, AbsentLetters, ImpossiblePatterns))
            .ToList();
    }
}
