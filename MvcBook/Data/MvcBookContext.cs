using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using MvcBook.Models;

namespace MvcBook.Data
{
    public class MvcBookContext : IdentityDbContext
    {
        public MvcBookContext (DbContextOptions<MvcBookContext> options)
            : base(options)
        {
        }

        public DbSet<MvcBook.Models.Book> Books { get; set; } = default!;
        public DbSet<MvcBook.Models.Author> Authors { get; set; } = default!;
        public DbSet<MvcBook.Models.Genre> Genres { get; set; } = default!;
        public DbSet<MvcBook.Models.CartItem> ShoppingCartItems { get; set; } = default!;
        public DbSet<Order> Orders { get; set; } = default!;
        public DbSet<Comment> Comments { get; set; }

    }

}
