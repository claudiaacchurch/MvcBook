using System.ComponentModel.DataAnnotations;
namespace MvcBook.Models
{
    public class Author
    {
        [Key]
        public virtual int? AuthorId { get; set; }
        public virtual string? Name { get; set; }
        public virtual HashSet<Book> Books { get; set; } = new HashSet<Book>();
    }
}
