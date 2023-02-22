#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
namespace JumpStarter.Models;

public class MyContext : DbContext 
{     
    public MyContext(DbContextOptions options) : base(options) { }   

    public DbSet<User> Users { get; set; } 
    public DbSet<Project> Projects{get;set;}
    public DbSet<UserAndProject> UsersAndProjects{ get; set; } 
}
