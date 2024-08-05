namespace App.Controllers;

using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

[Route("api/[controller]")]
[ApiController]
public class SearchController(IChatService chatService)
{
    [HttpPost]
    public async Task<string> SearchAsync(string query)
    {
        return await chatService.GetResponseAsync(query);
    }
    
}