using Microsoft.AspNetCore.Identity;

namespace ChatVerse.Shared.Models;

public class AppUser : IdentityUser
{
    public string? AvatarUrl { get; set; }
}
