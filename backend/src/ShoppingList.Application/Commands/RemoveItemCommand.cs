namespace ShoppingList.Application.Commands;

public sealed record RemoveItemCommand(Guid ListId, Guid ItemId);