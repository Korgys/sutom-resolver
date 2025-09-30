using SutomResolver;

public class Program
{
    public static async Task Main(string[] args)
    {
        var simulatorV1 = new Simulator<SutomResolver.solver.v1.Solver>();
        var simulatorV2 = new Simulator<SutomResolver.solver.v2.Solver>();
        var simulatorV3 = new Simulator<SutomResolver.solver.v3.Solver>();

        // Exécute les simulatisons en parallèle
        await Task.WhenAll(
            Task.Run(() => simulatorV1.EmulateGames(false)),
            Task.Run(() => simulatorV2.EmulateGames(false)),
            Task.Run(() => simulatorV3.EmulateGames(false))
        );

        // Affiche les résultats après l'exécution des simulations
        simulatorV1.DisplayStatsResult();
        simulatorV2.DisplayStatsResult();
        simulatorV3.DisplayStatsResult();
    }
}
