namespace ShoppingList.Domain.Entities;

/// <summary>
/// Represents a shopping list aggregate root.
/// Manages a collection of shopping list items with lifecycle and state management.
/// </summary>
public class ShoppingList
{
    private readonly List<ShoppingListItem> _items = [];

    /// <summary>
    /// Unique identifier for this shopping list.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// List owner (user ID or email identifier, required).
    /// </summary>
    public string Owner { get; private set; } = null!;

    /// <summary>
    /// Optional date associated with the list.
    /// </summary>
    public DateTime? Date { get; private set; }

    /// <summary>
    /// Read-only collection of items in this list.
    /// </summary>
    public IReadOnlyCollection<ShoppingListItem> Items => _items.AsReadOnly();

    /// <summary>
    /// Computed property: list is finished if all items are purchased/removed (or list is empty = false).
    /// </summary>
    public bool Finished => _items.Count != 0 && _items.All(i => i.IsCompleted());

    /// <summary>
    /// Private constructor for factory and EF Core hydration.
    /// </summary>
    private ShoppingList(Guid id, string owner, DateTime? date)
    {
        Id = id;
        Owner = owner;
        Date = date;
    }

    /// <summary>
    /// Factory method to create a new ShoppingList with validation.
    /// </summary>
    /// <param name="owner">List owner (required, non-empty string)</param>
    /// <param name="date">Optional date</param>
    /// <returns>New ShoppingList instance with empty items collection</returns>
    /// <throws>ArgumentException if owner is null, empty, or whitespace</throws>
    public static ShoppingList Create(string owner, DateTime? date = null) => string.IsNullOrWhiteSpace(owner)
            ? throw new ArgumentException("Owner cannot be null or empty.")
            : new ShoppingList(Guid.NewGuid(), owner, date);

    /// <summary>
    /// Add item to the list with validation delegated to ShoppingListItem.Create.
    /// </summary>
    /// <param name="description">Item description (1-255 chars, required)</param>
    /// <param name="quantity">Item quantity (positive or null)</param>
    /// <returns>Created ShoppingListItem</returns>
    /// <throws>ArgumentException if validation fails (via ShoppingListItem.Create)</throws>
    public ShoppingListItem AddItem(string description, int? quantity = null)
    {
        var item = ShoppingListItem.Create(description, quantity);
        _items.Add(item);
        return item;
    }

    /// <summary>
    /// Remove item from tracking by setting removed flag.
    /// Recalculates finished status.
    /// </summary>
    /// <param name="itemId">ID of item to remove</param>
    /// <throws>InvalidOperationException if item not found</throws>
    public void RemoveItem(Guid itemId)
    {
        var item = FindItem(itemId);
        item.MarkRemoved();
    }

    /// <summary>
    /// Mark item as purchased by setting purchased flag.
    /// Recalculates finished status.
    /// </summary>
    /// <param name="itemId">ID of item to mark purchased</param>
    /// <throws>InvalidOperationException if item not found</throws>
    public void MarkItemPurchased(Guid itemId)
    {
        var item = FindItem(itemId);
        item.MarkPurchased();
    }

    /// <summary>
    /// Update item quantity with validation.
    /// Does not affect finished status (quantity not part of completion logic).
    /// </summary>
    /// <param name="itemId">ID of item to update</param>
    /// <param name="newQuantity">New quantity (positive or null)</param>
    /// <throws>InvalidOperationException if item not found</throws>
    /// <throws>ArgumentException if quantity invalid (via item.UpdateQuantity)</throws>
    public void UpdateItemQuantity(Guid itemId, int? newQuantity)
    {
        var item = FindItem(itemId);
        item.UpdateQuantity(newQuantity);
    }

    /// <summary>
    /// Create a copy of this list with a new owner.
    /// Copied items have their purchased/removed flags reset to false.
    /// Original list remains unchanged.
    /// </summary>
    /// <param name="newOwner">Owner for copied list (required, non-empty)</param>
    /// <param name="newDate">Optional date for copied list</param>
    /// <returns>New independent ShoppingList</returns>
    /// <throws>ArgumentException if newOwner invalid (via Create factory)</throws>
    public ShoppingList Copy(string newOwner, DateTime? newDate = null)
    {
        var copiedList = Create(newOwner, newDate);

        foreach (var item in _items)
        {
            // Copy item with same description/quantity but reset purchase/remove flags
            var copiedItem = copiedList.AddItem(item.Description, item.Quantity);
            // Note: copiedItem starts with purchased=false, removed=false (default from Create)
        }

        return copiedList;
    }

    /// <summary>
    /// Find item by ID in current collection.
    /// </summary>
    /// <throws>InvalidOperationException if item not found</throws>
    private ShoppingListItem FindItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        return item is null ? throw new InvalidOperationException($"Item with ID {itemId} not found in list.") : item;
    }
}
