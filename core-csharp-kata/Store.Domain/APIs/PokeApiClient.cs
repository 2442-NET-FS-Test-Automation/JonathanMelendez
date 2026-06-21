using System.Text.Json;
using Serilog;

namespace Store.Domain;

public class PokeApiClient
{
    private static readonly HttpClient client = new();

    public async Task<(int pokeId, string pokeType)?> FetchByNameAsync(string name)
    {
        string url = $"https://pokeapi.co/api/v2/pokemon/{name.ToLower()}/";

        try
        {
            string jsonResponse = await client.GetStringAsync(url);
            return Parse(jsonResponse);
        }
        catch (HttpRequestException ex)
        {
            Log.Warning($"Network fetch failed for {name}: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Log.Warning($"FetchByNameAsync failed: {ex.Message}");
            return null;
        }
    }

    public static (int pokeId, string pokeType)? Parse(string json)
    {
        Dictionary<string, JsonElement>? resp = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        
        if (resp is null    || !resp.TryGetValue("id", out JsonElement pokeIdElement)
                            || !resp.TryGetValue("types", out JsonElement pokeTypeElement))
        {
            return null;
        }

        
        int pokeId = pokeIdElement.GetInt32();
        string pokeType = pokeTypeElement[0]
                            .GetProperty("type").GetProperty("name")
                            .GetString() ?? "UntrakedType";

        return (pokeId, pokeType);
        
    }
}