using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicLibrary.Models
{
    public class Email
    {
        [Key]
        public int Id { get; set; }
        public string Recipients { get; set; } = string.Empty;
        public string? Cc_recipients { get; set; } = string.Empty;
        public string Email_Subject { get; set; } = string.Empty;
        public string Email_body { get; set; } = string.Empty;
        public DateTime QueueTime { get; set; }
        public DateTime? SentTime { get; set; }
    }
}
