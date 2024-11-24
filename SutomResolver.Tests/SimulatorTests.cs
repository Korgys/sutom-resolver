namespace SutomResolver.Tests;

[TestClass]
public class SimulatorTests
{
    [TestMethod]
    public void EmulateGames_ShouldHaveGoodPercentSuccessRate()
    {
        // Arrange
        const int numberOfGames = 100;
        const float expectedSuccessRate = 90;
        var simulator = new Simulator
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
}
