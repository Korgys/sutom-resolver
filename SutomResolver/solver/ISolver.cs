namespace SutomResolver.solver;

/// <summary>
/// Interface d'un solveur implémentant des méthodes simples pour fournir des candidats et analyser les réponses.
/// </summary>
public interface ISolver
{
    List<string> RemainingWords { get; }
    void Initialize(string pattern);
    string GetNextGuess();
    void ProcessResponse(string guess, string response);
}
