using System.ComponentModel.DataAnnotations;
namespace MvcBook.Models
{
    public class Author
    {
        [Key]
        public virtual int? AuthorId { get; set; }
        public virtual string? Name { get; set; }
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
