using System.ComponentModel.DataAnnotations;
using Microsoft.CodeAnalysis;

namespace MvcBook.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int Rating { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }
    }
}
