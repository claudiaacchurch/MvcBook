using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MvcBook.Data;

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
                    new Author() { Name = "Stephen King" }
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
                    new Genre() { Name = "Philosophy" },
                    new Genre() { Name = "Poetry" },
                    new Genre() { Name = "Romance" },
                    new Genre() { Name = "Sci-Fi" },

                    new Genre() { Name = "Art, Fashion & Photography" },
                    new Genre() { Name = "Biography" },
                    new Genre() { Name = "Business, Finance & Law" },
                    new Genre() { Name = "Computing & IT" },
                    new Genre() { Name = "Food & Drink" },
                    new Genre() { Name = "History" },
                    new Genre() { Name = "Languages" },
                    new Genre() { Name = "Health, Wellbeing & Self-Help" },
                    new Genre() { Name = "Politics, Society & Culture" },
                    new Genre() { Name = "Science, Technology & Medicine" },
                    new Genre() { Name = "Spirituality & Religion" },
                    new Genre() { Name = "Travel" }
               );

               context.SaveChanges();
            }
        }
    }
}
