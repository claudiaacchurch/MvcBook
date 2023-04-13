using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MvcBook.Data;
using MvcBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MvcBook.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MvcBookContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MvcBookContext>>()))
            {
     
                if (context.Authors.Any())
                {
                    return;
                }
                context.Authors.AddRange(
                    new Author() { Name = "J.K. Rowling" },
                    new Author() { Name = "Charles Dickens" },
                    new Author() { Name = "J.R.R. Tolkein" },
                    new Author() { Name = "Stephen King" },
                    new Author() { Name = "Harper Lee" },
                    new Author() { Name = "Paul Coelho" }
                );

                if (context.Genres.Any())
                {
                    return;
                }
                context.Genres.AddRange(
                    new Genre() { Name = "Action & Adventure" },
                    new Genre() { Name = "Children's Books" },
                    new Genre() { Name = "Classics" },
                    new Genre() { Name = "Crime & Thriller" },
                    new Genre() { Name = "Fantasy" },
                    new Genre() { Name = "Graphic Novels & Manga" },
                    new Genre() { Name = "Historical Fiction" },
                    new Genre() { Name = "Horror" },
                    new Genre() { Name = "Philosophical" }
               );

               context.SaveChanges();
            }
        }
    }
}
