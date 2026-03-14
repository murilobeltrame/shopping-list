namespace ShoppingList.Application.Commands;

public sealed record MarkItemPurchasedCommand(Guid ListId, Guid ItemId);