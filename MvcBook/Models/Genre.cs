using System.ComponentModel.DataAnnotations;

namespace MvcBook.Models
{
    public class Genre
    {
        [Key]
        public virtual int? GenreId { get; set; }
        public virtual string? Name { get; set; }
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
