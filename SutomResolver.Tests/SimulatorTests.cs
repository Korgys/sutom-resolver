namespace SutomResolver.Tests;

[TestClass]
public class SimulatorTests
{
    [TestMethod]
    public void EmulateGames_ShouldHaveGoodPercentSuccessRate()
    {
        // Arrange
        var simulator = new Simulator
        {
            NumberOfGames = 1000  // Utilisez un nombre plus élevé de jeux pour un test plus fiable statistiquement
        };

        // Act
        simulator.EmulateGames(displayLogs: false);
        float successRate = (simulator.Wins / simulator.NumberOfGames) * 100;

        // Assert
        Console.WriteLine(successRate);
        Assert.IsTrue(successRate >= 90, $"Expected success rate to be at least 70%, but was {successRate}%");
    }
}
