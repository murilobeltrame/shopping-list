namespace ShoppingList.Domain.Entities;

/// <summary>
/// Represents an individual item on a shopping list.
/// Items have required descriptions (1-255 chars), optional quantities (positive integers),
/// and state flags for tracking purchase/removal.
/// </summary>
public class ShoppingListItem
{
    /// <summary>
    /// Unique identifier for this item.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Item description (1-255 characters, required).
    /// </summary>
    public string Description { get; private set; } = null!;

    /// <summary>
    /// Item quantity (positive integer > 0, or null if not specified).
    /// </summary>
    public int? Quantity { get; private set; }

    /// <summary>
    /// Flag indicating item has been purchased.
    /// </summary>
    public bool Purchased { get; private set; }

    /// <summary>
    /// Flag indicating item has been removed from active tracking.
    /// </summary>
    public bool Removed { get; private set; }

    /// <summary>
    /// Private constructor for EF Core hydration and factory method use.
    /// </summary>
    private ShoppingListItem(Guid id, string description, int? quantity, bool purchased, bool removed)
    {
        Id = id;
        Description = description;
        Quantity = quantity;
        Purchased = purchased;
        Removed = removed;
    }

    /// <summary>
    /// Factory method to create a new ShoppingListItem with validation.
    /// </summary>
    /// <param name="description">Item description (required, 1-255 chars)</param>
    /// <param name="quantity">Quantity (positive integer > 0, or null)</param>
    /// <returns>New ShoppingListItem instance</returns>
    /// <throws>ArgumentException if validation fails</throws>
    public static ShoppingListItem Create(string description, int? quantity = null)
    {
        ValidateDescription(description);
        ValidateQuantity(quantity);

        return new ShoppingListItem(Guid.NewGuid(), description, quantity, purchased: false, removed: false);
    }

    /// <summary>
    /// Mark this item as purchased.
    /// </summary>
    public void MarkPurchased()
    {
        Purchased = true;
    }

    /// <summary>
    /// Mark this item as removed from tracking.
    /// </summary>
    public void MarkRemoved()
    {
        Removed = true;
    }

    /// <summary>
    /// Update item quantity with validation.
    /// </summary>
    /// <param name="newQuantity">New quantity (positive or null)</param>
    /// <throws>ArgumentException if newQuantity is invalid</throws>
    public void UpdateQuantity(int? newQuantity)
    {
        ValidateQuantity(newQuantity);
        Quantity = newQuantity;
    }

    /// <summary>
    /// Check if item is in a completed state (purchased or removed).
    /// </summary>
    /// <returns>true if item is completed, false otherwise</returns>
    public bool IsCompleted()
    {
        return Purchased || Removed;
    }

    /// <summary>
    /// Validates description: 1-255 chars, non-null, non-whitespace.
    /// </summary>
    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description) || description.Length > 255)
        {
            throw new ArgumentException("Description cannot be null, empty, or exceed 255 characters.");
        }
    }

    /// <summary>
    /// Validates quantity: must be positive (> 0) or null.
    /// </summary>
    private static void ValidateQuantity(int? quantity)
    {
        if (quantity.HasValue && quantity <= 0)
        {
            throw new ArgumentException("Quantity must be positive (> 0) or null.");
        }
    }
}
