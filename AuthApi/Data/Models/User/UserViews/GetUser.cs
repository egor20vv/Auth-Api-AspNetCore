using System.ComponentModel.DataAnnotations;

namespace AuthApi.Data.Models.User.UserViews;

public class GetUser
{
    [Required]
    public string NicknameOrEmail { get; set; }
    [Required]
    public string Password { get; set; }

    public bool IsEmail()
    {
        return new EmailAddressAttribute().IsValid(NicknameOrEmail);
    }
}
