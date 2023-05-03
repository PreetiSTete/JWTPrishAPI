using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace login_api.Model
{
    [Keyless]
    public class Authenticate
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}