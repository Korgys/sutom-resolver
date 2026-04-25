namespace SutomResolver.Tests;

[TestClass]
public class SimulatorTests
{
    private static readonly IReadOnlyList<string> DeterministicCorpus =
    [
        "ABOIS",
        "ABOUT",
        "ABATS",
        "ABIME",
        "ACIER"
    ];

    [TestMethod]
    public void EmulateGames_ShouldBeReproducible_WithSeededRandomAndDeterministicCorpus()
    {
        const int seed = 12345;
        const int numberOfGames = 20;

        var first = new Simulator<solver.v1.Solver>(new Random(seed), DeterministicCorpus)
        {
            NumberOfGames = numberOfGames
        };
        var second = new Simulator<solver.v1.Solver>(new Random(seed), DeterministicCorpus)
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
    public void EmulateGames_ShouldProduceDifferentOutcomes_WithDifferentSeedsOnSameCorpus()
    {
        const int numberOfGames = 5;

        var seededA = new Simulator<solver.v1.Solver>(new Random(1), DeterministicCorpus)
        {
            NumberOfGames = numberOfGames
        };
        var seededB = new Simulator<solver.v1.Solver>(new Random(2), DeterministicCorpus)
        {
            NumberOfGames = numberOfGames
        };

        seededA.EmulateGames(displayLogs: false);
        seededB.EmulateGames(displayLogs: false);

        Assert.IsFalse(
            seededA.Wins == seededB.Wins &&
            seededA.Loses == seededB.Loses &&
            Math.Abs(seededA.Turns - seededB.Turns) < 0.0001f);
    }

    [TestMethod]
    public void Initialize_ProcessResponse_GetNextGuess_ShouldConvergeToRemainingCandidate()
    {
        var solver = new solver.v4.Solver();
        solver.Initialize("A____");
        solver.CandidatesWords = ["ABOIS", "ABOUT", "ABATS"];

        var response = SutomHelper.GetResultFromGuess("ABATS", "ABOIS");
        solver.ProcessResponse("ABATS", response);
        var guess = solver.GetNextGuess();

        CollectionAssert.AreEquivalent(new List<string> { "ABOIS" }, solver.CandidatesWords);
        Assert.AreEqual("ABOIS", guess);
    }

    [TestMethod]
    public void GetNextGuess_ShouldPreferCandidateWithHigherHeuristicScore()
    {
        var solver = new solver.v4.Solver();
        solver.Initialize("_____");
        solver.CandidatesWords = ["ABCDE", "VWXYZ"];
        solver.HeuristicValues =
        [
            new() { ['A'] = 10, ['V'] = 1 },
            new() { ['B'] = 10, ['W'] = 1 },
            new() { ['C'] = 10, ['X'] = 1 },
            new() { ['D'] = 10, ['Y'] = 1 },
            new() { ['E'] = 10, ['Z'] = 1 }
        ];

        var guess = solver.GetNextGuess();

        Assert.AreEqual("ABCDE", guess);
    }

    [TestMethod]
    public void ProcessResponse_ShouldFilterCandidates_WhenRepeatedLettersAreOverrepresented()
    {
        var solver = new solver.v4.Solver();
        solver.Initialize("_____");
        solver.CandidatesWords = ["BACDE", "BAADE", "AACDE", "ZZZZZ"];

        var result = SutomHelper.GetResultFromGuess("AAFGH", "BACDE");
        solver.ProcessResponse("AAFGH", result);

        CollectionAssert.AreEquivalent(new List<string> { "BACDE" }, solver.CandidatesWords);
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
}
