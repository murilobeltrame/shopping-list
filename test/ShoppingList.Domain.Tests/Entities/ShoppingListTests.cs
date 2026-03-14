using Shouldly;
using Xunit;

namespace ShoppingList.Domain.Tests.Entities;

public class ShoppingListTests
{
    /// <summary>
    /// Factory method creates list with valid owner.
    /// </summary>
    [Fact]
    public void Create_WithValidOwner_CreatesList()
    {
        // Arrange & Act
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user@example.com");

        // Assert
        list.Owner.ShouldBe("user@example.com");
        list.Finished.ShouldBeFalse();
        list.Items.Count.ShouldBe(0);
    }

    /// <summary>
    /// Factory rejects empty owner.
    /// </summary>
    [Fact]
    public void Create_WithEmptyOwner_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => global::ShoppingList.Domain.Entities.ShoppingList.Create(""));
    }

    /// <summary>
    /// Factory rejects null owner.
    /// </summary>
    [Fact]
    public void Create_WithNullOwner_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => global::ShoppingList.Domain.Entities.ShoppingList.Create(null!));
    }

    /// <summary>
    /// Factory rejects whitespace-only owner.
    /// </summary>
    [Fact]
    public void Create_WithWhitespaceOwner_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => global::ShoppingList.Domain.Entities.ShoppingList.Create("   "));
    }

    /// <summary>
    /// Factory creates list with optional date.
    /// </summary>
    [Fact]
    public void Create_WithDateAndOwner_CreatesListWithDate()
    {
        // Arrange
        var date = new DateTime(2026, 3, 1);

        // Act
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user@example.com", date);

        // Assert
        list.Date.ShouldBe(date);
    }

    /// <summary>
    /// New list initializes with empty items and finished = false.
    /// </summary>
    [Fact]
    public void Create_InitializesWithEmptyItems_AndFinishedFalse()
    {
        // Arrange & Act
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user@example.com");

        // Assert
        list.Items.Count.ShouldBe(0);
        list.Finished.ShouldBeFalse();
    }

    /// <summary>
    /// AddItem with valid description adds item to list and returns created item.
    /// </summary>
    [Fact]
    public void AddItem_WithValidDescription_AddsItemToList()
    {
        // Arrange
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user@example.com");

        // Act
        var item = list.AddItem("Milk");

        // Assert
        item.Description.ShouldBe("Milk");
        list.Items.Count.ShouldBe(1);
    }

    /// <summary>
    /// AddItem delegates validation to ShoppingListItem.Create (invalid description throws).
    /// </summary>
    [Fact]
    public void AddItem_DelegatesValidationToShoppingListItem()
    {
        // Arrange
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user@example.com");

        // Act & Assert
        Should.Throw<ArgumentException>(() => list.AddItem(""));
    }

    /// <summary>
    /// AddItem with invalid quantity throws.
    /// </summary>
    [Fact]
    public void AddItem_InvalidQuantity_ThrowsArgumentException()
    {
        // Arrange
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user@example.com");

        // Act & Assert
        Should.Throw<ArgumentException>(() => list.AddItem("Milk", 0));
    }

    /// <summary>
    /// Finished is false for empty list.
    /// </summary>
    [Fact]
    public void Finished_WithEmptyList_ReturnsFalse()
    {
        // Arrange & Act
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user@example.com");

        // Assert
        list.Finished.ShouldBeFalse();
    }

    /// <summary>
    /// Finished is true when all items are purchased.
    /// </summary>
    [Fact]
    public void Finished_WithOnePurchasedItem_ReturnsTrue()
    {
        // Arrange
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user@example.com");
        var item = list.AddItem("Milk");
        item.MarkPurchased();

        // Act & Assert
        list.Finished.ShouldBeTrue();
    }

    /// <summary>
    /// Finished is true when all items are removed.
    /// </summary>
    [Fact]
    public void Finished_WithOneRemovedItem_ReturnsTrue()
    {
        // Arrange
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user@example.com");
        var item = list.AddItem("Milk");
        item.MarkRemoved();

        // Act & Assert
        list.Finished.ShouldBeTrue();
    }

    /// <summary>
    /// Finished is false when items have mixed states.
    /// </summary>
    [Fact]
    public void Finished_WithMixedStates_ReturnsFalse()
    {
        // Arrange
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user@example.com");
        var item1 = list.AddItem("Milk");
        var item2 = list.AddItem("Bread");
        item1.MarkPurchased();
        // item2 remains unpurchased

        // Act & Assert
        list.Finished.ShouldBeFalse();
    }

    /// <summary>
    /// Finished is true when all items are completed (purchased or removed).
    /// </summary>
    [Fact]
    public void Finished_WithAllItemsCompleted_ReturnsTrue()
    {
        // Arrange
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user@example.com");
        var item1 = list.AddItem("Milk");
        var item2 = list.AddItem("Bread");
        item1.MarkPurchased();
        item2.MarkRemoved();

        // Act & Assert
        list.Finished.ShouldBeTrue();
    }

    /// <summary>
    /// RemoveItem with valid ID marks item as removed.
    /// </summary>
    [Fact]
    public void RemoveItem_WithValidItemId_MarksItemRemoved()
    {
        // Arrange
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user@example.com");
        var item = list.AddItem("Milk");

        // Act
        list.RemoveItem(item.Id);

        // Assert
        item.Removed.ShouldBeTrue();
    }

    /// <summary>
    /// RemoveItem with invalid ID throws.
    /// </summary>
    [Fact]
    public void RemoveItem_WithInvalidItemId_ThrowsInvalidOperationException()
    {
        // Arrange
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user@example.com");

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => list.RemoveItem(Guid.NewGuid()));
    }

    /// <summary>
    /// RemoveItem recalculates finished status correctly.
    /// </summary>
    [Fact]
    public void RemoveItem_RecalculatesFinished_ToTrueIfAllCompleted()
    {
        // Arrange
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user@example.com");
        var item = list.AddItem("Milk");

        // Act
        list.RemoveItem(item.Id);

        // Assert
        list.Finished.ShouldBeTrue();
    }

    /// <summary>
    /// MarkItemPurchased with valid ID marks item as purchased.
    /// </summary>
    [Fact]
    public void MarkItemPurchased_WithValidItemId_MarksPurchased()
    {
        // Arrange
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user@example.com");
        var item = list.AddItem("Milk");

        // Act
        list.MarkItemPurchased(item.Id);

        // Assert
        item.Purchased.ShouldBeTrue();
    }

    /// <summary>
    /// MarkItemPurchased with invalid ID throws.
    /// </summary>
    [Fact]
    public void MarkItemPurchased_WithInvalidItemId_ThrowsInvalidOperationException()
    {
        // Arrange
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user@example.com");

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => list.MarkItemPurchased(Guid.NewGuid()));
    }

    /// <summary>
    /// MarkItemPurchased recalculates finished status correctly.
    /// </summary>
    [Fact]
    public void MarkItemPurchased_RecalculatesFinished_ToTrueIfAllCompleted()
    {
        // Arrange
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user@example.com");
        var item = list.AddItem("Milk");

        // Act
        list.MarkItemPurchased(item.Id);

        // Assert
        list.Finished.ShouldBeTrue();
    }

    /// <summary>
    /// UpdateItemQuantity with valid ID and quantity updates the item.
    /// </summary>
    [Fact]
    public void UpdateItemQuantity_WithValidItemId_UpdatesQuantity()
    {
        // Arrange
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user@example.com");
        var item = list.AddItem("Milk", 2);

        // Act
        list.UpdateItemQuantity(item.Id, 3);

        // Assert
        item.Quantity.ShouldBe(3);
    }

    /// <summary>
    /// UpdateItemQuantity with invalid ID throws.
    /// </summary>
    [Fact]
    public void UpdateItemQuantity_WithInvalidItemId_ThrowsInvalidOperationException()
    {
        // Arrange
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user@example.com");

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => list.UpdateItemQuantity(Guid.NewGuid(), 5));
    }

    /// <summary>
    /// UpdateItemQuantity does not affect finished status.
    /// </summary>
    [Fact]
    public void UpdateItemQuantity_DoesNotAffectFinished_Status()
    {
        // Arrange
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user@example.com");
        var item = list.AddItem("Milk", 2);

        // Act
        list.UpdateItemQuantity(item.Id, 5);

        // Assert
        list.Finished.ShouldBeFalse();
    }

    /// <summary>
    /// Copy creates new list with copied items.
    /// </summary>
    [Fact]
    public void Copy_WithValidOwner_CreatesNewListWithCopiedItems()
    {
        // Arrange
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user1@example.com");
        var item = list.AddItem("Milk", 2);
        item.MarkPurchased();

        // Act
        var copiedList = list.Copy("user2@example.com");

        // Assert
        copiedList.Owner.ShouldBe("user2@example.com");
        copiedList.Items.Count.ShouldBe(1);
    }

    /// <summary>
    /// Copy resets item state to unpurchased/unremoved.
    /// </summary>
    [Fact]
    public void Copy_CopiedItems_ResetToNotPurchasedAndNotRemoved()
    {
        // Arrange
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user1@example.com");
        var item = list.AddItem("Milk", 2);
        item.MarkPurchased();
        item.MarkRemoved();

        // Act
        var copiedList = list.Copy("user2@example.com");
        var copiedItem = copiedList.Items.First();

        // Assert
        copiedItem.Purchased.ShouldBeFalse();
        copiedItem.Removed.ShouldBeFalse();
    }

    /// <summary>
    /// Copy leaves original list unchanged.
    /// </summary>
    [Fact]
    public void Copy_OriginalListRemains_Unchanged()
    {
        // Arrange
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user1@example.com");
        var item = list.AddItem("Milk");

        // Act
        var copiedList = list.Copy("user2@example.com");

        // Assert
        list.Owner.ShouldBe("user1@example.com");
        list.Items.Count.ShouldBe(1);
        item.Description.ShouldBe("Milk");
    }

    /// <summary>
    /// Copy with invalid owner throws.
    /// </summary>
    [Fact]
    public void Copy_WithInvalidNewOwner_PropagatesArgumentException()
    {
        // Arrange
        var list = global::ShoppingList.Domain.Entities.ShoppingList.Create("user@example.com");

        // Act & Assert
        Should.Throw<ArgumentException>(() => list.Copy(""));
    }

    [Fact]
    public void Create_GeneratesUniqueIds_ForTenThousandLists()
    {
        HashSet<Guid> ids = [];

        for (int i = 0; i < 10_000; i++)
        {
            global::ShoppingList.Domain.Entities.ShoppingList list =
                global::ShoppingList.Domain.Entities.ShoppingList.Create($"owner-{i}");
            ids.Add(list.Id);
        }

        ids.Count.ShouldBe(10_000);
    }
}
