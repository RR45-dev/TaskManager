namespace taskManager.Data;

using Microsoft.EntityFrameworkCore;
using taskManager.Models;

public class TaskDbContext : DbContext

/*The TaskDbContext class will act as a bridge between your C# application and the database, mapping your C# models to database tables.*/

{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }
    public DbSet<TaskItems> TaskTable { get; set; }
}