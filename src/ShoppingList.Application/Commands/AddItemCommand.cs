namespace ShoppingList.Application.Commands;

public sealed record AddItemCommand(Guid ListId, string Description, int? Quantity);