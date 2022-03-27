using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicLibrary.DataAccess.Data;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace MusicLibrary.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly MusicLibraryContext _db;
        public RegisterModel(MusicLibraryContext context)
        {
            _db = context;
        }

        [BindProperty]
        public MusicLibrary.Models.User newUser { get; set; }

        //[BindProperty]
        public string errorMessage { get; set; }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            
            if (!ModelState.IsValid) // Check if all forms are filled correctly
            {
                return Page();
            }
            if (newUser.Passwords != newUser.PasswordsRetype)
            {
                errorMessage = "Passwords do not match";
                return Page();
            }
            if (UserExists(newUser.UserName))
            {
                errorMessage = "User name is already taken, please choose a different one";
                return Page();
            }
            byte[] salt = new byte[128 / 8]; //Salt for passwords hashing

            using (var rngCsp = new RNGCryptoServiceProvider()) // Generate random salt
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            // Hash passwords
            string HashedPass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                                            password: newUser.Passwords,
                                                            salt: salt,
                                                            prf: KeyDerivationPrf.HMACSHA256,
                                                            iterationCount: 100000,
                                                            numBytesRequested: 256 / 8));
            // Add new user to DB
            newUser.Passwords = HashedPass;
            newUser.Salt = Convert.ToBase64String(salt);
            _db.User.Add(newUser);
            await _db.SaveChangesAsync();
            return RedirectToPage("./Login");

        }

        public bool UserExists(string inputUserName)
        {
            return _db.User.Any(u => u.UserName == inputUserName);
        }
    }

}
