using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcBook.Models;

namespace MvcBook.Data
{
    public class MvcBookContext : IdentityDbContext
    {
        public MvcBookContext(DbContextOptions<MvcBookContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; } = default!;
        public DbSet<Author> Authors { get; set; } = default!;
        public DbSet<Genre> Genres { get; set; } = default!;
        public DbSet<CartItem> ShoppingCartItems { get; set; } = default!;
        public DbSet<Order> Orders { get; set; } = default!;
        public DbSet<Comment> Comments { get; set; }

    }

}
