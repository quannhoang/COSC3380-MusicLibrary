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
        [RegularExpression(@"(?=^.{8,20}$)(?=.*\d)(?=.*[a-zA-Z])(?=.*[!@#$%^&*()_+}{:;'?/>.<,])(?!.*\s).*$", ErrorMessage = "Passwords must be 8-50 characters long with at least one numeric, one alphabet and one special character")]
        [StringLength(50,MinimumLength = 8)]
        public string Passwords { get; set; } = string.Empty;

        [NotMapped]
        [Required]
        [Display(Name = "Retype Passwords")]
        [RegularExpression(@"(?=^.{8,20}$)(?=.*\d)(?=.*[a-zA-Z])(?=.*[!@#$%^&*()_+}{:;'?/>.<,])(?!.*\s).*$", ErrorMessage = "Passwords must be 8-50 characters long with at least one numeric, one alphabet and one special character")]
        [StringLength(50, MinimumLength = 8)]
        public string PasswordsRetype { get; set; } = string.Empty;

        public string Salt { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Invalid Email address")]
        [StringLength(30, MinimumLength = 5)]
        public string Email { get; set; } = string.Empty;

        public Boolean IsAdmin { get; set; } = false;

        public Boolean IsArtist { get; set; } = false;

        public Boolean IsSuspended { get; set; } = false;

        [DataType(DataType.Date)]
        [Required]
        public DateTime CreateDate { get; set; } = DateTime.Today;

        public IEnumerable<Song>? Songs { get; set; }


    }
}
