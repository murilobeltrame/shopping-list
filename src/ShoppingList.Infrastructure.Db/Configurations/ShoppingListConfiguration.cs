using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ShoppingList.Infrastructure.Db.Configurations;

public sealed class ShoppingListConfiguration : IEntityTypeConfiguration<global::ShoppingList.Domain.Entities.ShoppingList>
{
    public void Configure(EntityTypeBuilder<global::ShoppingList.Domain.Entities.ShoppingList> builder)
    {
        builder.ToTable("ShoppingLists");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Owner).IsRequired();
        builder.Property(x => x.Date).IsRequired(false);

        builder.Ignore(x => x.Finished);

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey("ShoppingListId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}