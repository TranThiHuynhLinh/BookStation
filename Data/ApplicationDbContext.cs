using BookStation.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookStation.Data;
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<SubUserInfo> SubUserInfos { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<FavouriteBook> FavouriteBooks { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<Block> Blocks { get; set; }
}

