using System.Net;
using System.Net.Http.Json;
using Ardalis.Specification;
using NSubstitute;
using Shouldly;

namespace ShoppingList.RestApi.Tests.Endpoints;

public class ShoppingListCreationTests(ShoppingListApiFactory factory) : IClassFixture<ShoppingListApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly IRepositoryBase<ShoppingList.Domain.Entities.ShoppingList> _repository = factory.Repository;

    [Fact]
    public async Task PostShoppingLists_WithoutIdField_Returns201WithGeneratedId()
    {
        _repository.AddAsync(Arg.Any<ShoppingList.Domain.Entities.ShoppingList>())
            .Returns(callInfo => callInfo.Arg<ShoppingList.Domain.Entities.ShoppingList>());

        HttpResponseMessage response = await _client.PostAsJsonAsync("/shopping-lists", new { Owner = "alice", Date = (DateTime?)null });

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        IdResponse? body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body.ShouldNotBeNull();
        body.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task PostShoppingListItems_WithoutIdField_Returns201WithGeneratedItemId()
    {
        ShoppingList.Domain.Entities.ShoppingList list = ShoppingList.Domain.Entities.ShoppingList.Create("alice");
        _repository.GetByIdAsync(list.Id).Returns(list);
        _repository.UpdateAsync(Arg.Any<ShoppingList.Domain.Entities.ShoppingList>())
            .Returns(Task.FromResult(1));

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            $"/shopping-lists/{list.Id}/items",
            new { Description = "Milk", Quantity = (int?)2 });

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        IdResponse? body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body.ShouldNotBeNull();
        body.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task PostShoppingListsCopy_Returns201WithNewGeneratedId()
    {
        ShoppingList.Domain.Entities.ShoppingList sourceList = ShoppingList.Domain.Entities.ShoppingList.Create("alice");
        _repository.GetByIdAsync(sourceList.Id).Returns(sourceList);
        _repository.AddAsync(Arg.Any<ShoppingList.Domain.Entities.ShoppingList>())
            .Returns(callInfo => callInfo.Arg<ShoppingList.Domain.Entities.ShoppingList>());

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            $"/shopping-lists/{sourceList.Id}/copy",
            new { NewOwner = "bob", NewDate = (DateTime?)null });

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        IdResponse? body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body.ShouldNotBeNull();
        body.Id.ShouldNotBe(Guid.Empty);
        body.Id.ShouldNotBe(sourceList.Id);
    }

    [Fact]
    public async Task PostShoppingLists_GeneratesUniqueIdsOnEachRequest()
    {
        _repository.AddAsync(Arg.Any<ShoppingList.Domain.Entities.ShoppingList>())
            .Returns(callInfo => callInfo.Arg<ShoppingList.Domain.Entities.ShoppingList>());

        HttpResponseMessage response1 = await _client.PostAsJsonAsync("/shopping-lists", new { Owner = "alice", Date = (DateTime?)null });
        HttpResponseMessage response2 = await _client.PostAsJsonAsync("/shopping-lists", new { Owner = "bob", Date = (DateTime?)null });

        IdResponse? body1 = await response1.Content.ReadFromJsonAsync<IdResponse>();
        IdResponse? body2 = await response2.Content.ReadFromJsonAsync<IdResponse>();
        body1.ShouldNotBeNull();
        body2.ShouldNotBeNull();
        body1.Id.ShouldNotBe(body2.Id);
    }

    [Fact]
    public async Task PostShoppingLists_WithClientSuppliedId_IgnoresPayloadIdAndReturnsGeneratedId()
    {
        _repository.AddAsync(Arg.Any<ShoppingList.Domain.Entities.ShoppingList>())
            .Returns(callInfo => callInfo.Arg<ShoppingList.Domain.Entities.ShoppingList>());

        Guid clientSuppliedId = Guid.NewGuid();
        HttpResponseMessage response = await _client.PostAsJsonAsync(
            "/shopping-lists",
            new { Owner = "alice", Date = (DateTime?)null, Id = clientSuppliedId });

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        IdResponse? body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body.ShouldNotBeNull();
        body.Id.ShouldNotBe(Guid.Empty);
        body.Id.ShouldNotBe(clientSuppliedId);
    }

    [Fact]
    public async Task PostShoppingListItems_WithClientSuppliedId_IgnoresPayloadIdAndReturnsGeneratedId()
    {
        ShoppingList.Domain.Entities.ShoppingList list = ShoppingList.Domain.Entities.ShoppingList.Create("alice");
        _repository.GetByIdAsync(list.Id).Returns(list);
        _repository.UpdateAsync(Arg.Any<ShoppingList.Domain.Entities.ShoppingList>())
            .Returns(Task.FromResult(1));

        Guid clientSuppliedId = Guid.NewGuid();
        HttpResponseMessage response = await _client.PostAsJsonAsync(
            $"/shopping-lists/{list.Id}/items",
            new { Description = "Milk", Quantity = (int?)2, Id = clientSuppliedId });

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        IdResponse? body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body.ShouldNotBeNull();
        body.Id.ShouldNotBe(Guid.Empty);
        body.Id.ShouldNotBe(clientSuppliedId);
    }
}

internal sealed record IdResponse(Guid Id);
