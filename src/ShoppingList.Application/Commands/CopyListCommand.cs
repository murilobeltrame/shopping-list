namespace ShoppingList.Application.Commands;

public sealed record CopyListCommand(Guid SourceListId, string NewOwner, DateTime? NewDate);