namespace SutomResolver.Tests;

[TestClass]
public class SimulatorTests
{
    [TestMethod]
    public void EmulateGames_ShouldBeReproducible_WithSeededRandom()
    {
        const int seed = 12345;
        const int numberOfGames = 20;

        var first = new Simulator<solver.v1.Solver>(new Random(seed))
        {
            NumberOfGames = numberOfGames
        };
        var second = new Simulator<solver.v1.Solver>(new Random(seed))
        {
            NumberOfGames = numberOfGames
        };

        first.EmulateGames(displayLogs: false);
        second.EmulateGames(displayLogs: false);

        Assert.AreEqual(first.Wins, second.Wins);
        Assert.AreEqual(first.Loses, second.Loses);
        Assert.AreEqual(first.Turns, second.Turns);
    }

    [TestMethod]
    public void GetResultFromGuess_ShouldMarkExactAndMisplacedLetters()
    {
        var result = SutomHelper.GetResultFromGuess("ABATS", "ABOIS");

        Assert.AreEqual("AB__S", result);
    }

    [TestMethod]
    public void GetResultFromGuess_ShouldUseTwoPassMatchingForRepeatedLetters()
    {
        var result = SutomHelper.GetResultFromGuess("AABBB", "AACCC");

        Assert.AreEqual("AA___", result);
    }

    [TestMethod]
    public void ProcessResponse_ShouldFilterCandidatesUsingPublicBehavior()
    {
        var solver = new solver.v4.Solver();
        solver.Initialize("A____");

        var initialCount = solver.CandidatesWords.Count;
        var response = SutomHelper.GetResultFromGuess("ABATS", "ABOIS");

        solver.ProcessResponse("ABATS", response);

        Assert.IsTrue(solver.CandidatesWords.Count < initialCount);
        CollectionAssert.Contains(solver.CandidatesWords, "ABOIS");
        CollectionAssert.DoesNotContain(solver.CandidatesWords, "ABATS");
        CollectionAssert.DoesNotContain(solver.CandidatesWords, "ABOUT");
    }

    [TestMethod]
    public void GetNextGuess_ShouldReturnRemainingCandidate_WhenFilteringLeavesOneWord()
    {
        var solver = new solver.v4.Solver();
        solver.Initialize("A____");

        solver.ProcessResponse("ABATS", SutomHelper.GetResultFromGuess("ABATS", "ABOIS"));

        var nextGuess = solver.GetNextGuess();

        Assert.IsFalse(string.IsNullOrEmpty(nextGuess));
        CollectionAssert.Contains(solver.CandidatesWords, nextGuess);
    }

    [TestMethod]
    public void ProcessResponse_ShouldRespectFixedLettersAndAbsentLetters()
    {
        var solver = new solver.v4.Solver();
        solver.Initialize("A____");

        solver.ProcessResponse("ABATS", "AB__S");

        CollectionAssert.Contains(solver.CandidatesWords, "ABOIS");
        CollectionAssert.DoesNotContain(solver.CandidatesWords, "ABOUT");
        CollectionAssert.DoesNotContain(solver.CandidatesWords, "ABUSE");
    }
}
