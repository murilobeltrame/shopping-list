namespace ShoppingList.Application.Commands;

public sealed record CreateListCommand(string Owner, DateTime? Date);