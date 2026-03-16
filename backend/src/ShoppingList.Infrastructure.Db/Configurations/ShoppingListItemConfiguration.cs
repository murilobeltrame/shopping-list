using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ShoppingList.Infrastructure.Db.Configurations;

public sealed class ShoppingListItemConfiguration : IEntityTypeConfiguration<Domain.Entities.ShoppingListItem>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.ShoppingListItem> builder)
    {
        builder.ToTable("ShoppingListItems");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnType("uniqueidentifier");

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnType("nvarchar(255)");

        builder.Property(x => x.Quantity).IsRequired(false).HasColumnType("int");
        builder.Property(x => x.Purchased).IsRequired().HasColumnType("bit");
        builder.Property(x => x.Removed).IsRequired().HasColumnType("bit");

        builder.Property<Guid>("ShoppingListId").IsRequired().HasColumnType("uniqueidentifier");
        builder.HasIndex("ShoppingListId");
    }
}