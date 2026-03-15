namespace ShoppingList.Application.Commands;

public sealed record UpdateItemQuantityCommand(Guid ListId, Guid ItemId, int? NewQuantity);