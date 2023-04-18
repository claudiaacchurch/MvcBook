using System.ComponentModel.DataAnnotations;
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
        public virtual Author? Author { get; set; }
        public virtual Genre? Genre { get; set; }
        public virtual int AuthorId { get; set; }
        public virtual int GenreId { get; set; }
    }
}
