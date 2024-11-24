namespace SutomResolver.solver;

public interface ISolver
{
    List<string> RemainingWords { get; }
    void Initialize(string pattern);
    string GetNextGuess();
    void ProcessResponse(string guess, string response);
}
