using System.Net;
using Shouldly;

namespace ShoppingList.RestApi.Tests;

public class DatabaseStartupTests(ShoppingListApiFactory factory) : IClassFixture<ShoppingListApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task ApiHost_StartsWithSqlServerStyleConnectionConfiguration()
    {
        HttpResponseMessage response = await _client.GetAsync("/this-route-does-not-exist");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
