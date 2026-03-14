using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ShoppingList.Infrastructure.Db.Configurations;

public sealed class ShoppingListItemConfiguration : IEntityTypeConfiguration<global::ShoppingList.Domain.Entities.ShoppingListItem>
{
    public void Configure(EntityTypeBuilder<global::ShoppingList.Domain.Entities.ShoppingListItem> builder)
    {
        builder.ToTable("ShoppingListItems");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Quantity).IsRequired(false);
        builder.Property(x => x.Purchased).IsRequired();
        builder.Property(x => x.Removed).IsRequired();

        builder.Property<Guid>("ShoppingListId").IsRequired();
        builder.HasIndex("ShoppingListId");
    }
}