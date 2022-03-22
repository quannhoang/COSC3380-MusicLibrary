using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicLibrary.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [Key]
        [Required]
        [Display(Name = "User Name")]
        [RegularExpression(@"[0-9a-zA-Z\s]*$", ErrorMessage = "User name must have alphanumeric letter only, and has 3-30 characters")]
        [StringLength(30, MinimumLength = 3)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"(?=^.{8,20}$)(?=.*\d)(?=.*[a-zA-Z])(?=.*[!@#$%^&*()_+}{:;'?/>.<,])(?!.*\s).*$", ErrorMessage = "Passwords must be 8-20 characters long with at least one numeric, one alphabet and one special character")]
        [StringLength(30,MinimumLength = 8)]
        public string Passwords { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^([\w -\.] +)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9] { 1,3}\.)| (([\w -] +\.)+))([a - zA - Z]{ 2,4}|[0 - 9]{ 1,3})(\]?)$", ErrorMessage = "Invalid Email address")]
        [StringLength(30, MinimumLength = 5)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public Boolean IsAdmin { get; set; } = false;

        [Required]
        public Boolean IsArtist { get; set; } = false;

        public IEnumerable<Song>? Songs { get; set; }


    }
}
