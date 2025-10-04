namespace SutomResolver.solver.v2;

/// <summary>
/// Stratégie qui prend en compte les pattern impossibles pour filtrer.
/// </summary>
/// <remarks>
/// Games : 10000 | Win ratio : 98,52% | Turns per game : 3,71
/// </remarks>
public class Solver : ISolver
{
    private HashSet<char> AbsentLetters { get; set; }
    private HashSet<string> ImpossiblePatterns { get; set; } // Représente la liste des pattern impossibles pour le mot à trouver
    public List<string> CandidatesWords { get; set; }

    public void Initialize(string pattern)
    {
        AbsentLetters = [];
        ImpossiblePatterns = [];
        CandidatesWords = SutomHelper.LoadWordsFromFile(pattern.Length);
    }

    public string GetNextGuess()
    {
        return CandidatesWords
            .OrderByDescending(word => word.Distinct().Count())
            .FirstOrDefault()?
            .ToUpper();
    }

    public void ProcessResponse(string guess, string result)
    {
        var misplacedLetters = SolverHelper.GetMisplacedLetters(guess, result);
        SolverHelper.UpdateAbsentLetters(AbsentLetters, guess, result, misplacedLetters);
        ImpossiblePatterns.Add(SolverHelper.GetImpossiblePattern(guess, result));
        CandidatesWords = CandidatesWords
            .Where(word => SolverHelper.MatchesPattern(word, result, misplacedLetters, AbsentLetters, ImpossiblePatterns))
            .ToList();
    }
}
