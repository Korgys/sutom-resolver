namespace SutomResolver.Tests;

[TestClass]
public class SimulatorTests
{
    [TestMethod]
    public void EmulateGames_ShouldHaveGoodPercentSuccessRate_WithSolverV1()
    {
        // Arrange
        const int numberOfGames = 100;
        const float expectedSuccessRate = 80;
        var simulator = new Simulator<solver.v1.Solver>()
        {
            NumberOfGames = numberOfGames
        };

        // Act
        simulator.EmulateGames(displayLogs: false);
        float actualSuccessRate = (simulator.Wins / simulator.NumberOfGames) * 100;

        // Assert
        Console.WriteLine(actualSuccessRate);
        Assert.IsTrue(actualSuccessRate >= expectedSuccessRate, 
            $"Expected success rate to be at least {expectedSuccessRate}%, but was {actualSuccessRate}%");
    }

    [TestMethod]
    public void EmulateGames_ShouldHaveGoodPercentSuccessRate_WithSolverV2()
    {
        // Arrange
        const int numberOfGames = 100;
        const float expectedSuccessRate = 92;
        var simulator = new Simulator<solver.v2.Solver>()
        {
            NumberOfGames = numberOfGames
        };

        // Act
        simulator.EmulateGames(displayLogs: false);
        float actualSuccessRate = (simulator.Wins / simulator.NumberOfGames) * 100;

        // Assert
        Console.WriteLine(actualSuccessRate);
        Assert.IsTrue(actualSuccessRate >= expectedSuccessRate,
            $"Expected success rate to be at least {expectedSuccessRate}%, but was {actualSuccessRate}%");
    }

    [TestMethod]
    public void EmulateGames_ShouldHaveGoodPercentSuccessRate_WithSolverV3()
    {
        // Arrange
        const int numberOfGames = 100;
        const float expectedSuccessRate = 95;
        var simulator = new Simulator<solver.v3.Solver>()
        {
            NumberOfGames = numberOfGames
        };

        // Act
        simulator.EmulateGames(displayLogs: false);
        float actualSuccessRate = (simulator.Wins / simulator.NumberOfGames) * 100;

        // Assert
        Console.WriteLine(actualSuccessRate);
        Assert.IsTrue(actualSuccessRate >= expectedSuccessRate,
            $"Expected success rate to be at least {expectedSuccessRate}%, but was {actualSuccessRate}%");
    }

    [TestMethod]
    public void SolverV2_ShouldImproveOrMatchV1_OnAverageWins()
    {
        const int numberOfGames = 100;
        var simulatorV1 = new Simulator<solver.v1.Solver>()
        {
            NumberOfGames = numberOfGames
        };
        var simulatorV2 = new Simulator<solver.v2.Solver>()
        {
            NumberOfGames = numberOfGames
        };

        simulatorV1.EmulateGames(displayLogs: false);
        simulatorV2.EmulateGames(displayLogs: false);

        Assert.IsTrue(simulatorV2.Wins >= simulatorV1.Wins, 
            $"Expected Solver V2 to have at least as many wins as Solver V1, but got {simulatorV2.Wins} vs {simulatorV1.Wins}");
    }

    [TestMethod]
    public void SolverV3_ShouldImproveOrMatchV2_OnAverageWins()
    {
        const int numberOfGames = 100;
        var simulatorV2 = new Simulator<solver.v2.Solver>()
        {
            NumberOfGames = numberOfGames
        };
        var simulatorV3 = new Simulator<solver.v3.Solver>()
        {
            NumberOfGames = numberOfGames
        };

        simulatorV2.EmulateGames(displayLogs: false);
        simulatorV3.EmulateGames(displayLogs: false);

        Assert.IsTrue(simulatorV3.Wins >= simulatorV2.Wins,
            $"Expected Solver V3 to have at least as many wins as Solver V2, but got {simulatorV3.Wins} vs {simulatorV2.Wins}");
    }
}
