namespace SutomResolver.solver;

public sealed record LetterCountConstraint(int Minimum, int? Maximum)
{
    public bool Allows(int count)
    {
        if (count < Minimum)
        {
            return false;
        }

        return Maximum is null || count <= Maximum.Value;
    }

    public LetterCountConstraint Merge(LetterCountConstraint other)
    {
        var minimum = Math.Max(Minimum, other.Minimum);
        int? maximum = Maximum.HasValue && other.Maximum.HasValue
            ? (int?)Math.Min(Maximum.Value, other.Maximum.Value)
            : Maximum ?? other.Maximum;

        if (maximum is not null && maximum < minimum)
        {
            throw new InvalidOperationException("Inconsistent letter occurrence constraints.");
        }

        return new LetterCountConstraint(minimum, maximum);
    }
}

public sealed class SutomConstraints
{
    private readonly char?[] _fixedLettersByPosition;
    private readonly Dictionary<int, HashSet<char>> _forbiddenLettersByPosition;
    private readonly Dictionary<char, LetterCountConstraint> _letterCountConstraints;

    private SutomConstraints(int length)
    {
        Length = length;
        _fixedLettersByPosition = new char?[length];
        _forbiddenLettersByPosition = new Dictionary<int, HashSet<char>>();
        _letterCountConstraints = new Dictionary<char, LetterCountConstraint>();
    }

    public int Length { get; }

    public static SutomConstraints CreateEmpty(int length) => new(length);

    public static SutomConstraints FromGuessAndResult(string guess, string result)
    {
        if (guess.Length != result.Length)
        {
            throw new ArgumentException("Guess and result must have the same length.");
        }

        var constraints = new SutomConstraints(guess.Length);
        var guessedLetterCounts = new Dictionary<char, int>();
        var matchedLetterCounts = new Dictionary<char, int>();

        for (int i = 0; i < guess.Length; i++)
        {
            var guessedLetter = guess[i];
            var resultChar = result[i];

            guessedLetterCounts[guessedLetter] = guessedLetterCounts.TryGetValue(guessedLetter, out var guessedCount)
                ? guessedCount + 1
                : 1;

            if (IsFixedLetter(resultChar))
            {
                constraints._fixedLettersByPosition[i] = guessedLetter;
                matchedLetterCounts[guessedLetter] = matchedLetterCounts.TryGetValue(guessedLetter, out var matchedCount)
                    ? matchedCount + 1
                    : 1;
                continue;
            }

            // Any non-green symbol (yellow or gray) forbids this letter at this position.
            // This is important for repeated letters: a gray occurrence is not "absent",
            // it is only an extra copy at this slot.
            constraints.AddForbiddenLetter(i, guessedLetter);

            if (resultChar == '+')
            {
                matchedLetterCounts[guessedLetter] = matchedLetterCounts.TryGetValue(guessedLetter, out var matchedCount)
                    ? matchedCount + 1
                    : 1;
            }
        }

        foreach (var (letter, guessedCount) in guessedLetterCounts)
        {
            matchedLetterCounts.TryGetValue(letter, out var matchedCount);

            // If a letter appears too many times in the guess, its maximum is capped by the
            // number of validated occurrences (green + yellow). Otherwise, it remains open.
            var maximum = guessedCount > matchedCount ? (int?)matchedCount : null;
            constraints._letterCountConstraints[letter] = new LetterCountConstraint(matchedCount, maximum);
        }

        return constraints;
    }

    public SutomConstraints Merge(SutomConstraints other)
    {
        if (Length != other.Length)
        {
            throw new ArgumentException("Constraints must have the same length.");
        }

        var merged = new SutomConstraints(Length);

        for (int i = 0; i < Length; i++)
        {
            var leftFixed = _fixedLettersByPosition[i];
            var rightFixed = other._fixedLettersByPosition[i];

            if (leftFixed is not null && rightFixed is not null && leftFixed != rightFixed)
            {
                throw new InvalidOperationException("Inconsistent fixed-letter constraints.");
            }

            merged._fixedLettersByPosition[i] = leftFixed ?? rightFixed;

            var hasLeftForbidden = _forbiddenLettersByPosition.TryGetValue(i, out var leftForbidden);
            var hasRightForbidden = other._forbiddenLettersByPosition.TryGetValue(i, out var rightForbidden);

            if (hasLeftForbidden || hasRightForbidden)
            {
                merged._forbiddenLettersByPosition[i] = new HashSet<char>();

                if (hasLeftForbidden && leftForbidden is not null)
                {
                    merged._forbiddenLettersByPosition[i].UnionWith(leftForbidden);
                }

                if (hasRightForbidden && rightForbidden is not null)
                {
                    merged._forbiddenLettersByPosition[i].UnionWith(rightForbidden);
                }
            }
        }

        foreach (var (letter, constraint) in _letterCountConstraints)
        {
            merged._letterCountConstraints[letter] = constraint;
        }

        foreach (var (letter, constraint) in other._letterCountConstraints)
        {
            merged._letterCountConstraints[letter] = merged._letterCountConstraints.TryGetValue(letter, out var existing)
                ? existing.Merge(constraint)
                : constraint;
        }

        return merged;
    }

    public bool Matches(string word)
    {
        if (word.Length != Length)
        {
            return false;
        }

        for (int i = 0; i < Length; i++)
        {
            var fixedLetter = _fixedLettersByPosition[i];
            var currentLetter = word[i];

            if (fixedLetter is not null && currentLetter != fixedLetter)
            {
                return false;
            }

            if (_forbiddenLettersByPosition.TryGetValue(i, out var forbiddenLetters) && forbiddenLetters.Contains(currentLetter))
            {
                return false;
            }
        }

        if (_letterCountConstraints.Count == 0)
        {
            return true;
        }

        var counts = new Dictionary<char, int>();
        foreach (var letter in word)
        {
            counts[letter] = counts.TryGetValue(letter, out var count) ? count + 1 : 1;
        }

        foreach (var (letter, constraint) in _letterCountConstraints)
        {
            counts.TryGetValue(letter, out var count);

            if (!constraint.Allows(count))
            {
                return false;
            }
        }

        return true;
    }

    public char? GetFixedLetter(int position) => _fixedLettersByPosition[position];

    public bool IsLetterForbiddenAt(int position, char letter)
    {
        return _forbiddenLettersByPosition.TryGetValue(position, out var forbiddenLetters) && forbiddenLetters.Contains(letter);
    }

    public bool TryGetLetterConstraint(char letter, out LetterCountConstraint constraint)
    {
        return _letterCountConstraints.TryGetValue(letter, out constraint!);
    }

    private void AddForbiddenLetter(int position, char letter)
    {
        if (!_forbiddenLettersByPosition.TryGetValue(position, out var forbiddenLetters))
        {
            forbiddenLetters = new HashSet<char>();
            _forbiddenLettersByPosition[position] = forbiddenLetters;
        }

        forbiddenLetters.Add(letter);
    }

    private static bool IsFixedLetter(char resultChar)
    {
        return resultChar != '_' && resultChar != '+';
    }
}
