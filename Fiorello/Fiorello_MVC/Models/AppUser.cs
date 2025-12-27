using Microsoft.AspNetCore.Identity;

namespace Fiorello_MVC.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
