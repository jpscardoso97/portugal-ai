namespace App.Services.Interfaces;

public interface IChatService
{
    Task<string> GetResponseAsync(string input);
}