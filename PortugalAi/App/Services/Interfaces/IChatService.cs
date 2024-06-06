namespace App.Services.Interfaces;

public interface IChatService
{
    Task<string> GetInitialResponse(string initialInput);
    
    Task<string> GetCompletion(string input);
}