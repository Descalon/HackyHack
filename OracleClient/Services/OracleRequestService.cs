using Hackvip.Domain;
using Hackvip.Models;
using System.Text.Json;

namespace Hackvip.Services;

public class OracleRequestService(HttpClient client)
{
    private readonly HttpClient _client = client;

    public async Task<BurgerModel> GetBurger(Guid key)
    {
        var response = await _client.GetAsync($"/{key}");
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<BurgerModel>(content) ?? throw new Exception("Serialization error");
    }
}
