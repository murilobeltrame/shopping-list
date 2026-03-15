using Shouldly;
using Xunit;

namespace ShoppingList.Domain.Tests.Entities;

public class ShoppingListItemTests
{
    /// <summary>
    /// Factory method creates item with valid description.
    /// </summary>
    [Fact]
    public void Create_WithValidDescription_CreatesItem()
    {
        // Arrange & Act
        var item = Domain.Entities.ShoppingListItem.Create("Milk");

        // Assert
        item.Description.ShouldBe("Milk");
        item.Purchased.ShouldBeFalse();
        item.Removed.ShouldBeFalse();
        item.Quantity.ShouldBeNull();
    }

    /// <summary>
    /// Factory rejects empty description.
    /// </summary>
    [Fact]
    public void Create_WithEmptyDescription_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => 
            Domain.Entities.ShoppingListItem.Create(""));
    }

    /// <summary>
    /// Factory rejects whitespace-only description.
    /// </summary>
    [Fact]
    public void Create_WithWhitespaceDescription_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => 
            Domain.Entities.ShoppingListItem.Create("   "));
    }

    /// <summary>
    /// Factory rejects description exceeding 255 characters.
    /// </summary>
    [Fact]
    public void Create_WithDescriptionExceeding255_ThrowsArgumentException()
    {
        // Arrange
        var longDescription = new string('a', 256);

        // Act & Assert
        Should.Throw<ArgumentException>(() => 
            Domain.Entities.ShoppingListItem.Create(longDescription));
    }

    /// <summary>
    /// Factory accepts valid positive quantity.
    /// </summary>
    [Fact]
    public void Create_WithValidQuantity_CreatesItem()
    {
        // Arrange & Act
        var item = Domain.Entities.ShoppingListItem.Create("Apples", 5);

        // Assert
        item.Quantity.ShouldBe(5);
    }

    /// <summary>
    /// Factory rejects zero quantity.
    /// </summary>
    [Fact]
    public void Create_WithZeroQuantity_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => 
            Domain.Entities.ShoppingListItem.Create("Milk", 0));
    }

    /// <summary>
    /// Factory rejects negative quantity.
    /// </summary>
    [Fact]
    public void Create_WithNegativeQuantity_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => 
            Domain.Entities.ShoppingListItem.Create("Milk", -1));
    }

    /// <summary>
    /// Factory accepts null quantity.
    /// </summary>
    [Fact]
    public void Create_WithNullQuantity_CreatesItemSuccessfully()
    {
        // Arrange & Act
        var item = Domain.Entities.ShoppingListItem.Create("Cereal", null);

        // Assert
        item.Quantity.ShouldBeNull();
    }

    /// <summary>
    /// MarkPurchased sets purchased flag to true.
    /// </summary>
    [Fact]
    public void MarkPurchased_SetsFlag_ToTrue()
    {
        // Arrange
        var item = Domain.Entities.ShoppingListItem.Create("Milk");

        // Act
        item.MarkPurchased();

        // Assert
        item.Purchased.ShouldBeTrue();
    }

    /// <summary>
    /// MarkRemoved sets removed flag to true.
    /// </summary>
    [Fact]
    public void MarkRemoved_SetsFlag_ToTrue()
    {
        // Arrange
        var item = Domain.Entities.ShoppingListItem.Create("Milk");

        // Act
        item.MarkRemoved();

        // Assert
        item.Removed.ShouldBeTrue();
    }

    /// <summary>
    /// MarkPurchased does not affect removed flag.
    /// </summary>
    [Fact]
    public void MarkPurchased_DoesNotAffect_RemovedFlag()
    {
        // Arrange
        var item = Domain.Entities.ShoppingListItem.Create("Milk");

        // Act
        item.MarkPurchased();

        // Assert
        item.Removed.ShouldBeFalse();
    }

    /// <summary>
    /// MarkRemoved does not affect purchased flag.
    /// </summary>
    [Fact]
    public void MarkRemoved_DoesNotAffect_PurchasedFlag()
    {
        // Arrange
        var item = Domain.Entities.ShoppingListItem.Create("Milk");

        // Act
        item.MarkRemoved();

        // Assert
        item.Purchased.ShouldBeFalse();
    }

    /// <summary>
    /// UpdateQuantity updates quantity to valid value.
    /// </summary>
    [Fact]
    public void UpdateQuantity_WithValidValue_Updates()
    {
        // Arrange
        var item = Domain.Entities.ShoppingListItem.Create("Milk", 2);

        // Act
        item.UpdateQuantity(3);

        // Assert
        item.Quantity.ShouldBe(3);
    }

    /// <summary>
    /// UpdateQuantity rejects zero.
    /// </summary>
    [Fact]
    public void UpdateQuantity_WithZero_ThrowsArgumentException()
    {
        // Arrange
        var item = Domain.Entities.ShoppingListItem.Create("Milk", 2);

        // Act & Assert
        Should.Throw<ArgumentException>(() => item.UpdateQuantity(0));
    }

    /// <summary>
    /// UpdateQuantity accepts null.
    /// </summary>
    [Fact]
    public void UpdateQuantity_WithNull_UpdatesToNull()
    {
        // Arrange
        var item = Domain.Entities.ShoppingListItem.Create("Milk", 2);

        // Act
        item.UpdateQuantity(null);

        // Assert
        item.Quantity.ShouldBeNull();
    }

    /// <summary>
    /// IsCompleted returns true when purchased.
    /// </summary>
    [Fact]
    public void IsCompleted_WhenPurchased_ReturnsTrue()
    {
        // Arrange
        var item = Domain.Entities.ShoppingListItem.Create("Milk");
        item.MarkPurchased();

        // Act & Assert
        item.IsCompleted().ShouldBeTrue();
    }

    /// <summary>
    /// IsCompleted returns true when removed.
    /// </summary>
    [Fact]
    public void IsCompleted_WhenRemoved_ReturnsTrue()
    {
        // Arrange
        var item = Domain.Entities.ShoppingListItem.Create("Milk");
        item.MarkRemoved();

        // Act & Assert
        item.IsCompleted().ShouldBeTrue();
    }

    /// <summary>
    /// IsCompleted returns false when neither purchased nor removed.
    /// </summary>
    [Fact]
    public void IsCompleted_WhenNeither_ReturnsFalse()
    {
        // Arrange
        var item = Domain.Entities.ShoppingListItem.Create("Milk");

        // Act & Assert
        item.IsCompleted().ShouldBeFalse();
    }

    [Fact]
    public void Create_GeneratesUniqueIds_ForTenThousandItems()
    {
        HashSet<Guid> ids = [];

        for (int i = 0; i < 10_000; i++)
        {
            Domain.Entities.ShoppingListItem item =
                Domain.Entities.ShoppingListItem.Create($"item-{i}", 1);
            ids.Add(item.Id);
        }

        ids.Count.ShouldBe(10_000);
    }
}
