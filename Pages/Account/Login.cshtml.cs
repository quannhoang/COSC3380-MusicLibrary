using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicLibrary.DataAccess.Data;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

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
        public string LoginUserName { get; set; } = String.Empty;

        [BindProperty]
        public string LoginPasswords { get; set; } = String.Empty;

        public MusicLibrary.Models.User UserFromDB { get; set; }


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
                UserFromDB = _db.User.First(u => u.UserName == LoginUserName);

                // Hash passwords
                byte[] salt = new byte[128 / 8];
                string HashedPass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                                            password: LoginPasswords,
                                                            salt: Convert.FromBase64String(UserFromDB.Salt),
                                                            prf: KeyDerivationPrf.HMACSHA256,
                                                            iterationCount: 100000,
                                                            numBytesRequested: 256 / 8));
                if (HashedPass == UserFromDB.Passwords) // If UserExists and Passwords match, begin authentication
                {
                    var claims = new List<Claim> {
                        new Claim(ClaimTypes.Name, UserFromDB.UserName),
                        new Claim(ClaimTypes.Email, UserFromDB.Email)
                    };
                    var identity = new ClaimsIdentity(claims, "MusicLibraryCookie");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync("MusicLibraryCookie", claimsPrincipal);
                    return RedirectToPage("/Index"); // Authentication finished
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
