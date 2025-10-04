namespace SutomResolver.solver.v1;

/// <summary>
/// Stratégie simple qui renvoie le premier mot de la liste répondant aux lettres trouvées et dévoilées.
/// Elimine les mots contenant des lettres absentes ou ceux ne correspondant pas au pattern.
/// </summary>
/// <remarks>
/// Games : 10000 | Win ratio : 88,34% | Turns per game : 4,42
/// </remarks>
public class Solver : ISolver
{
    public HashSet<char> AbsentLetters { get; set; }
    public List<string> CandidatesWords { get; set; }

    public void Initialize(string pattern)
    {
        AbsentLetters = [];
        CandidatesWords = SutomHelper.LoadWordsFromFile(pattern.Length);
    }

    public string GetNextGuess()
    {
        return CandidatesWords
            .MaxBy(SolverHelper.CountDistinctLetters) // Privilégie le mot avec le plus de lettres distinctes
            .ToUpperInvariant();
    }

    public void ProcessResponse(string guess, string result)
    {
        var misplacedLetters = SolverHelper.GetMisplacedLetters(guess, result);
        SolverHelper.UpdateAbsentLetters(AbsentLetters, guess, result, misplacedLetters);
        CandidatesWords = CandidatesWords
            .Where(word => SolverHelper.MatchesPattern(word, result, misplacedLetters, AbsentLetters, null))
            .ToList();
    }
}
