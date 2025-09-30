namespace SutomResolver.solver.v3;

/// <summary>
/// Stratégie qui se base sur une analyse heuristique pour proposer 
/// d'abord les mots ayant les lettres qui ont plus de chances d'apparaitre.
/// Elimine les mots contenant des lettres absentes ou ceux ne correspondant pas au pattern de base (V1).
/// Prends également en compte les patterns impossibles pour filtrer (V2).
/// </summary>
/// <remarks>
/// Games : 10000 | Win ratio : 99,04% | Turns per game : 3,2625
/// </remarks>
public class Solver : ISolver
{
    public List<string> ImpossiblePatterns { get; set; }
    public HashSet<char> AbsentLetters { get; set; }
    public List<string> RemainingWords { get; set; }
    public List<Dictionary<char, int>> HeuristicValues { get; set; }

    public void Initialize(string pattern)
    {
        AbsentLetters = [];
        ImpossiblePatterns = [];
        RemainingWords = SutomHelper.LoadWordsFromFile(pattern.Length, pattern[0] != '_' ? pattern[0] : null);
        HeuristicValues = HeuristicsStatsHelper.GetHeuristicValues(RemainingWords, pattern);
    }

    public string GetNextGuess()
    {
        // Détermine le meilleur candidat à partir d'un score basé sur la variété des lettres utilisées et leur score de fréquence
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

