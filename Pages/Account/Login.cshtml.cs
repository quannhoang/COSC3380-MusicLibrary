using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicLibrary.DataAccess.Data;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace MusicLibrary.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly MusicLibraryContext _db;
        public LoginModel(MusicLibraryContext context)
        {
            _db = context;
        }

        [BindProperty]
        public string LoginUserName { get; set; }

        [BindProperty]
        public string LoginPasswords { get; set; }

        public MusicLibrary.Models.User User { get; set; }



        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            
            if ((LoginUserName == null) || (LoginPasswords == null))
            {
                return Page();
            }
            
            if (UserExists(LoginUserName))
            {
                User = _db.User.FirstOrDefault(u => u.UserName == LoginUserName);

                // Hash passwords
                byte[] salt = new byte[128 / 8];
                string HashedPass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                                            password: LoginPasswords,
                                                            salt: Convert.FromBase64String(User.Salt),
                                                            prf: KeyDerivationPrf.HMACSHA256,
                                                            iterationCount: 100000,
                                                            numBytesRequested: 256 / 8));
                if (HashedPass == User.Passwords)
                {
                    return RedirectToPage("/Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Username not found or incorrect passwords");
                    return Page();
                }
                    
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Username not found or incorrect passwords");
                return Page();
            }
                
        }

        public bool UserExists(string inputUserName)
        {
            return _db.User.Any(u => u.UserName == inputUserName);
        }
    }
}
