namespace SutomResolver.solver;

public interface ISolver
{
    public string GetNextGuess();
    public void ProcessResponse(string guess, string result);
}
