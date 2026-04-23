using System.Reflection;

namespace SutomResolver.Tests;

[TestClass]
public class SolverCoverageTests
{
    [TestMethod]
    public void V2Solver_GetNextGuess_ShouldPreferWordWithMostDistinctLetters()
    {
        var solver = new SutomResolver.solver.v2.Solver();
        solver.Initialize("_____");
        solver.CandidatesWords = new List<string> { "AAAAA", "AABBC", "ABCDE" };

        Assert.AreEqual("ABCDE", solver.GetNextGuess());
    }

    [TestMethod]
    public void V2Solver_ProcessResponse_ShouldFilterByMisplacedAndAbsentLetters()
    {
        var solver = new SutomResolver.solver.v2.Solver();
        solver.Initialize("_____");
        solver.CandidatesWords = new List<string>
        {
            "ABCDE",
            "EZZAZ",
            "AZZZE",
            "QZZZQ"
        };

        var result = SutomHelper.GetResultFromGuess("ABCDE", "EZZAZ");
        solver.ProcessResponse("ABCDE", result);

        CollectionAssert.Contains(solver.CandidatesWords, "EZZAZ");
        CollectionAssert.DoesNotContain(solver.CandidatesWords, "ABCDE");
        CollectionAssert.DoesNotContain(solver.CandidatesWords, "AZZZE");
        CollectionAssert.DoesNotContain(solver.CandidatesWords, "QZZZQ");
    }

    [TestMethod]
    public void V3Solver_GetNextGuess_ShouldUseHeuristicScores()
    {
        var solver = new SutomResolver.solver.v3.Solver();
        solver.Initialize("_____");
        solver.CandidatesWords = new List<string> { "ABCDE", "VWXYZ" };
        solver.HeuristicValues = new List<Dictionary<char, int>>
        {
            new() { ['A'] = 10, ['V'] = 1 },
            new() { ['B'] = 10, ['W'] = 1 },
            new() { ['C'] = 10, ['X'] = 1 },
            new() { ['D'] = 10, ['Y'] = 1 },
            new() { ['E'] = 10, ['Z'] = 1 }
        };

        Assert.AreEqual("ABCDE", solver.GetNextGuess());
    }

    [TestMethod]
    public void V3Solver_ProcessResponse_ShouldRemoveGuessAndKeepMatchingCandidate()
    {
        var solver = new SutomResolver.solver.v3.Solver();
        solver.Initialize("_____");
        solver.CandidatesWords = new List<string>
        {
            "ABCDE",
            "EZZAZ",
            "AZZZE"
        };

        var result = SutomHelper.GetResultFromGuess("ABCDE", "EZZAZ");
        solver.ProcessResponse("ABCDE", result);

        CollectionAssert.DoesNotContain(solver.CandidatesWords, "ABCDE");
        CollectionAssert.Contains(solver.CandidatesWords, "EZZAZ");
        CollectionAssert.DoesNotContain(solver.CandidatesWords, "AZZZE");
    }

    [TestMethod]
    public void V4Solver_ProcessResponse_ShouldEnableDiversifyingMode_WhenEnoughCandidatesRemain()
    {
        var solver = new SutomResolver.solver.v4.Solver();
        solver.Initialize("A____");
        solver.CandidatesWords = Enumerable.Range(0, 20)
            .Select(i =>
            {
                var suffix = (char)('F' + (i % 10));
                return $"A{suffix}{suffix}{suffix}{suffix}";
            })
            .ToList();

        SetPrivateField(solver, "_remainingTurns", 5);
        SetPrivateField(solver, "_lastResult", string.Empty);

        solver.ProcessResponse("ABCDE", "A____");

        Assert.IsTrue(GetPrivateField<bool>(solver, "_useDiversifyingWord"));
        Assert.AreEqual("A____", GetPrivateField<string>(solver, "_lastResult"));

        var nextGuess = solver.GetNextGuess();

        Assert.IsFalse(string.IsNullOrEmpty(nextGuess));
        Assert.AreEqual(5, nextGuess.Length);
    }

    [TestMethod]
    public void V4Solver_MergeResults_ShouldPreferExplicitLettersAndPlusMarkers()
    {
        var solver = new SutomResolver.solver.v4.Solver();
        var mergeResults = typeof(SutomResolver.solver.v4.Solver)
            .GetMethod("MergeResults", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.IsNotNull(mergeResults);

        var merged = (string)mergeResults.Invoke(solver, new object[] { "A+_B_", "_C+__" })!;

        Assert.AreEqual("AC+B_", merged);
    }

    [TestMethod]
    public void V4Solver_MergeResults_ShouldThrowWhenLengthsDiffer()
    {
        var solver = new SutomResolver.solver.v4.Solver();
        var mergeResults = typeof(SutomResolver.solver.v4.Solver)
            .GetMethod("MergeResults", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.IsNotNull(mergeResults);

        var exception = Assert.ThrowsException<TargetInvocationException>(() =>
            mergeResults.Invoke(solver, new object[] { "ABC", "ABCD" }));

        Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentException));
    }

    [TestMethod]
    public void V4Solver_MergeResults_ShouldReturnTheNonEmptyResultWhenOneSideIsEmpty()
    {
        var solver = new SutomResolver.solver.v4.Solver();
        var mergeResults = typeof(SutomResolver.solver.v4.Solver)
            .GetMethod("MergeResults", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.IsNotNull(mergeResults);

        var emptyLeft = (string)mergeResults.Invoke(solver, new object[] { string.Empty, "ABCDE" })!;
        var emptyRight = (string)mergeResults.Invoke(solver, new object[] { "ABCDE", string.Empty })!;

        Assert.AreEqual("ABCDE", emptyLeft);
        Assert.AreEqual("ABCDE", emptyRight);
    }

    [TestMethod]
    public void V4Solver_CountValuedLetters_ShouldSkipLastResultPenalty_WhenNoLettersAreRecorded()
    {
        var solver = new SutomResolver.solver.v4.Solver();
        solver.Initialize("A____");
        SetPrivateField(solver, "_lastResult", string.Empty);

        var countValuedLetters = typeof(SutomResolver.solver.v4.Solver)
            .GetMethod("CountValuedLetters", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.IsNotNull(countValuedLetters);

        var score = (double)countValuedLetters.Invoke(solver, new object[]
        {
            "ABCDE",
            new Dictionary<char, int>
            {
                ['A'] = 2,
                ['B'] = 3
            }
        })!;

        Assert.AreEqual(55d, score);
    }

    private static void SetPrivateField<T>(object instance, string fieldName, T value)
    {
        var field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        field.SetValue(instance, value);
    }

    private static T GetPrivateField<T>(object instance, string fieldName)
    {
        var field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        return (T)field.GetValue(instance)!;
    }
}
