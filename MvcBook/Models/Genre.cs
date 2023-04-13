using System.ComponentModel.DataAnnotations;

namespace MvcBook.Models
{
    public class Genre
    {
        [Key]
        public virtual int? GenreId { get; set; }
        public virtual string? Name { get; set; }
        public virtual HashSet<Book> Books { get; set; } = new HashSet<Book>();
    }
}
