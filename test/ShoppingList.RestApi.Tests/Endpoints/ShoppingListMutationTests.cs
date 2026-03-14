using System.Net;
using System.Net.Http.Json;
using Ardalis.Specification;
using NSubstitute;
using Shouldly;

namespace ShoppingList.RestApi.Tests.Endpoints;

public class ShoppingListMutationTests(ShoppingListApiFactory factory) : IClassFixture<ShoppingListApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly IRepositoryBase<ShoppingList.Domain.Entities.ShoppingList> _repository = factory.Repository;

    [Fact]
    public async Task PurchaseItem_WithPersistedSystemGeneratedIds_Returns204()
    {
        ShoppingList.Domain.Entities.ShoppingList list = ShoppingList.Domain.Entities.ShoppingList.Create("alice");
        ShoppingList.Domain.Entities.ShoppingListItem item = list.AddItem("Milk", 1);
        _repository.GetByIdAsync(list.Id).Returns(list);
        _repository.UpdateAsync(Arg.Any<ShoppingList.Domain.Entities.ShoppingList>()).Returns(Task.FromResult(1));

        HttpResponseMessage response = await _client.PostAsync($"/shopping-lists/{list.Id}/items/{item.Id}/purchase", null);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        item.Purchased.ShouldBeTrue();
    }

    [Fact]
    public async Task RemoveItem_WithPersistedSystemGeneratedIds_Returns204()
    {
        ShoppingList.Domain.Entities.ShoppingList list = ShoppingList.Domain.Entities.ShoppingList.Create("alice");
        ShoppingList.Domain.Entities.ShoppingListItem item = list.AddItem("Bread", 1);
        _repository.GetByIdAsync(list.Id).Returns(list);
        _repository.UpdateAsync(Arg.Any<ShoppingList.Domain.Entities.ShoppingList>()).Returns(Task.FromResult(1));

        HttpResponseMessage response = await _client.DeleteAsync($"/shopping-lists/{list.Id}/items/{item.Id}");

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        item.Removed.ShouldBeTrue();
    }

    [Fact]
    public async Task UpdateQuantity_WithPersistedSystemGeneratedIds_Returns204()
    {
        ShoppingList.Domain.Entities.ShoppingList list = ShoppingList.Domain.Entities.ShoppingList.Create("alice");
        ShoppingList.Domain.Entities.ShoppingListItem item = list.AddItem("Eggs", 1);
        _repository.GetByIdAsync(list.Id).Returns(list);
        _repository.UpdateAsync(Arg.Any<ShoppingList.Domain.Entities.ShoppingList>()).Returns(Task.FromResult(1));

        HttpResponseMessage response = await _client.PutAsJsonAsync(
            $"/shopping-lists/{list.Id}/items/{item.Id}/quantity",
            new { Quantity = (int?)4 });

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        item.Quantity.ShouldBe(4);
    }
}
