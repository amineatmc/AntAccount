using Microsoft.AspNetCore.Identity;

namespace AntalyaTaksiAccount.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int Age { get; set; }
        public string? OtherField { get; set; }
    }
}
