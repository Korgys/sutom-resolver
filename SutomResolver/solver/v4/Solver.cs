namespace SutomResolver.solver.v4;

/******************************************************************
* 
 *****************************************************************/

/// <summary>
/// Stratégie basée sur une analyse heuristique en 2 temps : 
/// 1) Propose d'abord les mots avec une grande diversité de lettres.
/// 2) 
/// </summary>
public class Solver : ISolver
{
    public List<string> ImpossiblePatterns { get; set; } = new List<string>();
    public HashSet<char> AbsentLetters { get; set; } = new HashSet<char>();
    public List<string> InitialWords { get; set; }
    public List<string> RemainingWords { get; set; }
    public List<Dictionary<char, int>> HeuristicValues { get; set; }
    public bool TryOtherPatternToReduceList { get; set; } = false;
    public int TryOtherPatternToReduceListTries { get; set; } = 0;
    public string LastPattern { get; set; }
    public int Turns { get; set; } = 6;

    public void Initialize(string pattern)
    {
        AbsentLetters = new HashSet<char>();
        ImpossiblePatterns = new List<string>();
        InitialWords = SutomHelper.LoadWordsFromFile(pattern.Length, pattern[0] != '_' ? pattern[0] : null);
        RemainingWords = InitialWords;
        HeuristicValues = HeuristicsStatsHelper.GetHeuristicValues(InitialWords, pattern);
        TryOtherPatternToReduceList = false;
        Turns = 6;
        TryOtherPatternToReduceListTries = 0;
    }

    public string GetNextGuess()
    {
        if (RemainingWords.Count == 0) return string.Empty;
        if (RemainingWords.Count == 1) return RemainingWords.First();

        IOrderedEnumerable<WordInfo> bestMatches;

        if (TryOtherPatternToReduceList)
        {
            var lettersToInvestigate = GetDistinctLettersFromPattern(LastPattern, RemainingWords);

            bestMatches = InitialWords
                .Select(word =>
                    new WordInfo
                    {
                        Word = word,
                        Score = word.Count(c => lettersToInvestigate.Contains(c)) * 2
                    })
                .OrderByDescending(w => w.Score);
            TryOtherPatternToReduceList = false;
        }
        else
        {
            bestMatches = RemainingWords
                .Select(word =>
                    new WordInfo
                    {
                        Word = word,
                        Score = HeuristicsStatsHelper.CalculateHeuristicScore(HeuristicValues, word)
                    })
                .OrderByDescending(w => w.Score);
        }

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

        Turns--;
        int missingLetters = result.Count(c => c == '?' || c == '_' || c == '+');
        float ratioMissing = missingLetters / (float)result.Length;
        if (ratioMissing <= 0.4 
            && Turns > 1
            && RemainingWords.Count / missingLetters >= Turns
            && TryOtherPatternToReduceListTries <= 2)
        {
            TryOtherPatternToReduceListTries++;
            TryOtherPatternToReduceList = true;
            LastPattern = result;
        }
    }

    public static List<char> GetDistinctLettersFromPattern(string pattern, List<string> words)
    {
        // Initialisation d'un ensemble pour collecter les lettres distinctes
        HashSet<char> possibleLetters = new HashSet<char>();

        // Parcourir chaque mot dans les mots restants
        foreach (var word in words)
        {
            // S'assurer que le mot correspond en longueur au pattern
            if (word.Length != pattern.Length)
                continue;

            for (int i = 0; i < pattern.Length; i++)
            {
                // Si l'emplacement est un _ ou ?, ajoute la lettre correspondante
                if (pattern[i] == '_' || pattern[i] == '?')
                {
                    possibleLetters.Add(word[i]);
                }
            }

        }

        return possibleLetters.Distinct().ToList();
    }
}

