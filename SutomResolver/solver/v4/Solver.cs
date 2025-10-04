using SutomResolver.solver.v3;

namespace SutomResolver.solver.v4;

/// <summary>
/// Provides functionality to solve word-based puzzles by managing candidate words,  processing feedback, and generating
/// the next guess based on heuristic analysis.
/// </summary>
/// <remarks>Games : 10000 | Win ratio : 99,10% | Turns per game : 3,48</remarks>
public class Solver : ISolver
{
    private List<string> _allWordsWithSameLength { get; set; }
    private int _remainingTurns = 6;
    private bool _useDiversifyingWord = false;
    private string _lastResult = "";

    public HashSet<string> ImpossiblePatterns { get; set; }
    public HashSet<char> AbsentLetters { get; set; }
    public List<string> CandidatesWords { get; set; }
    public List<Dictionary<char, int>> HeuristicValues { get; set; }

    public void Initialize(string pattern)
    {
        AbsentLetters = [];
        ImpossiblePatterns = [];
        _allWordsWithSameLength = SutomHelper.LoadWordsFromFile(pattern.Length);
        _remainingTurns = 6;
        _lastResult = "";
        _useDiversifyingWord = false;
        CandidatesWords = new List<string> (_allWordsWithSameLength);
        HeuristicValues = HeuristicsStatsHelper.GetHeuristicValues(CandidatesWords, pattern);
    }

    public string GetNextGuess()
    {
        // Soit on cherche à maximiser la diversité des lettres testées (dans certains cas particuliers)
        if (_useDiversifyingWord)
        {
            // Détermine le candidat le plus "diversifiant" en fonction des lettres déjà identifiées comme présentes dans le mot
            var lettersValued = new Dictionary<char, int>();
            foreach (var word in CandidatesWords)
            {
                for (int i = 0; i < word.Length; i++)
                {
                    if (_lastResult[i] == '_')
                    {
                        lettersValued[word[i]] = lettersValued.TryGetValue(word[i], out int val) ? val + 1 : 2;
                    }
                }
            }

            var bestMatch = _allWordsWithSameLength
                .Select(word =>
                    new
                    {
                        Word = word,
                        Score = CountValuedLetters(word, lettersValued)
                    })
                .OrderByDescending(w => w.Score)
                .Select(w => w.Word)
                .FirstOrDefault()?.ToUpper();
            return bestMatch;
        }
        else // Soit on utilise l'heuristique statistique
        {
            // Détermine le meilleur candidat à partir d'un score basé sur la variété des lettres utilisées et leur score de fréquence
            var bestMatches = CandidatesWords
                .Select(word =>
                    new
                    {
                        Word = word,
                        Score = HeuristicsStatsHelper.CalculateHeuristicScore(HeuristicValues, word)
                    })
                .OrderByDescending(w => w.Score)
                .Select(w => w.Word)
                .FirstOrDefault()?.ToUpper();
            return bestMatches;
        }
    }

    public void ProcessResponse(string guess, string result)
    {
        _remainingTurns--;

        // Met à jour les lettres absentes, les lettres mal placées et les patterns impossibles
        var misplacedLetters = SolverHelper.GetMisplacedLetters(guess, result);
        SolverHelper.UpdateAbsentLetters(AbsentLetters, guess, result, misplacedLetters);        
        ImpossiblePatterns.Add(SolverHelper.GetImpossiblePattern(guess, result));

        string resultToUseForFilter = _useDiversifyingWord ? _lastResult : result;
        CandidatesWords = CandidatesWords
            .Where(word => word != guess && SolverHelper.MatchesPattern(word, resultToUseForFilter, misplacedLetters, AbsentLetters, ImpossiblePatterns))
            .ToList();
        HeuristicValues = HeuristicsStatsHelper.GetHeuristicValues(CandidatesWords, resultToUseForFilter, false);

        // Cas particulier : s'il reste beaucoup de candidats mais peu d'inconnues, on peut envisager un mot permettant de tester plusieurs lettres
        int underscoreCount = result.Count(l => l == '_');
        if (_remainingTurns >= 2 && _remainingTurns <= 4 
            && underscoreCount > 0 && underscoreCount <= 4
            && _remainingTurns < CandidatesWords.Count / underscoreCount)
        {
            _useDiversifyingWord = true;
            _lastResult = MergeResults(_lastResult, result);
        }
        else
        {
            _useDiversifyingWord = false;
        }
    }

    private double CountValuedLetters(string word, Dictionary<char, int> lettersValued)
    {
        double score = 1;
        foreach (var letter in word.Distinct())
        {
            if (lettersValued.TryGetValue(letter, out int value))
            {
                score += value;
                if (!AbsentLetters.Contains(letter))
                {
                    score += value;
                }
            }
        }

        score *= word.Distinct().Count();

        if (_lastResult.Any(char.IsLetter))
        {
            for (int i = 0; i < word.Length; i++)
            {
                if (char.IsLetter(_lastResult[i]) && _lastResult[i] == word[i])
                {
                    score /= 2;
                }
            }
        }

        return score;
    }

    private string MergeResults(string result1, string result2)
    {
        if (string.IsNullOrEmpty(result1)) return result2;
        if (string.IsNullOrEmpty(result2)) return result1;
        if (result1.Length != result2.Length) throw new ArgumentException("Les résultats doivent avoir la même longueur.");

        var merged = new char[result1.Length];
        for (int i = 0; i < result1.Length; i++)
        {
            if (result1[i] != '_' && result1[i] != '+')
            {
                merged[i] = result1[i];
            }
            else if (result2[i] != '_' && result2[i] != '+')
            {
                merged[i] = result2[i];
            }
            else if (result1[i] == '+' || result2[i] == '+')
            {
                merged[i] = '+'; // Lettre présente mais mal placée
            }
            else
            {
                merged[i] = '_'; // Lettre absente
            }
        }
        return new string(merged);
    }
}
