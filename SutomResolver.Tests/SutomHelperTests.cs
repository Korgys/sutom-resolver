namespace SutomResolver.Tests;

[TestClass]
public class SutomHelperTests
{
    [TestMethod]
    public void V4SolverHeuristicValues_ShouldDiffer_ForSamePatternAndDifferentWordLists()
    {
        // Arrange
        var pattern = "abcde";
        var solver = new SutomResolver.solver.v4.Solver();

        // Act
        var firstHeuristics = SutomResolver.solver.v3.HeuristicsStatsHelper.GetHeuristicValues(
            new List<string> { "abcde", "axcye" },
            pattern);

        var secondHeuristics = SutomResolver.solver.v3.HeuristicsStatsHelper.GetHeuristicValues(
            new List<string> { "bbcde", "bbcee" },
            pattern);

        // Assert
        Assert.AreNotEqual(
            firstHeuristics[0].TryGetValue('a', out var firstA) ? firstA : 0,
            secondHeuristics[0].TryGetValue('a', out var secondA) ? secondA : 0);
        Assert.AreNotEqual(
            firstHeuristics[0].TryGetValue('b', out var firstB) ? firstB : 0,
            secondHeuristics[0].TryGetValue('b', out var secondB) ? secondB : 0);
    }

    [TestMethod]
    public void GetHeuristicValues_ShouldNotReuseCacheAcrossDifferentWordLists_ForSamePattern()
    {
        // Arrange
        var pattern = "abcde";
        var firstWords = new List<string> { "abcde", "axcye" };
        var secondWords = new List<string> { "bbcde", "bbcee" };

        // Act
        var firstHeuristics = SutomResolver.solver.v3.HeuristicsStatsHelper.GetHeuristicValues(firstWords, pattern);
        var secondHeuristics = SutomResolver.solver.v3.HeuristicsStatsHelper.GetHeuristicValues(secondWords, pattern);

        // Assert
        Assert.AreNotEqual(firstHeuristics[0].TryGetValue('a', out var firstA) ? firstA : 0,
            secondHeuristics[0].TryGetValue('a', out var secondA) ? secondA : 0);
        Assert.AreNotEqual(firstHeuristics[0].TryGetValue('b', out var firstB) ? firstB : 0,
            secondHeuristics[0].TryGetValue('b', out var secondB) ? secondB : 0);
    }

    [TestMethod]
    public void LoadWordsFromFile_ShouldReturnDifferentSets_ForSameLengthDifferentFirstLetters()
    {
        // Arrange
        const int size = 5;

        // Act
        var wordsStartingWithA = SutomHelper.LoadWordsFromFile(size, 'a');
        var wordsStartingWithB = SutomHelper.LoadWordsFromFile(size, 'b');

        // Assert
        Assert.IsTrue(wordsStartingWithA.Count > 0);
        Assert.IsTrue(wordsStartingWithB.Count > 0);
        Assert.IsTrue(wordsStartingWithA.All(word => word.StartsWith('A')));
        Assert.IsTrue(wordsStartingWithB.All(word => word.StartsWith('B')));
        CollectionAssert.AreNotEquivalent(wordsStartingWithA, wordsStartingWithB);
    }

    [TestMethod]
    public void LoadWordsFromFile_ShouldKeepNullFirstLetterCacheDistinct_FromExplicitLetter()
    {
        // Arrange
        const int size = 5;

        // Act
        var wordsWithNoFirstLetterFilter = SutomHelper.LoadWordsFromFile(size, null);
        var wordsStartingWithA = SutomHelper.LoadWordsFromFile(size, 'a');

        // Assert
        Assert.IsTrue(wordsWithNoFirstLetterFilter.Count > wordsStartingWithA.Count);
        Assert.IsTrue(wordsWithNoFirstLetterFilter.Any(word => word.StartsWith('A')));
        Assert.IsTrue(wordsWithNoFirstLetterFilter.Any(word => !word.StartsWith('A')));
    }
}
