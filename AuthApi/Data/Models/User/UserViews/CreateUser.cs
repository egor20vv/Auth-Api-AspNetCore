using System.ComponentModel.DataAnnotations;

namespace AuthApi.Data.Models.User.UserViews;

public class CreateUser
{
    [Required]
    public string NickName { get; set; }
    [EmailAddress]
    public string? Email { get; set; } = null;
    public string? Name { get; set; } = null;

    [Required]
    public string Password { get; set; }
}
