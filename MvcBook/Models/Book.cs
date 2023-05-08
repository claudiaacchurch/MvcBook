using System.ComponentModel.DataAnnotations;
using Microsoft.CodeAnalysis;

namespace MvcBook.Models
{
    public class Book
    {
        [Key]
        public virtual int id { get; set; }
        public virtual string Title { get; set; } = string.Empty;
        [Range(0, 10)]
        public virtual int Rating { get; set; }
        public virtual double Price { get; set; }
        public virtual string ImageUrl { get; set; } = string.Empty;
        public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
        public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
