namespace App.Plugins;

using System.ComponentModel;
using Microsoft.SemanticKernel;

public class RecommenderPlugin
{
    private static readonly IDictionary<string, string> LocationDict = new Dictionary<string, string>()
    {
        { "porto", "The Francesinha of the restaurant 'Tomar de Sal' in Porto is very good" },
        { "aveiro", "One of the best places to try Ovos Moles of the pastry maker 'Pastelaria Ramos'" }
    };
    
    [KernelFunction, 
     Description("Fetch suggestions for a location")]
    public static string GetRecommendations(
        [Description("The target location")] string location)
    {
        var key = location?.ToLower().Trim() ?? string.Empty;

        LocationDict.TryGetValue(key, out var recs);

        return recs;
    }
}