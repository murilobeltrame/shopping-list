using Microsoft.EntityFrameworkCore;

namespace ShoppingList.Infrastructure.Db;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options) { }
