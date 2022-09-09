using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthApi.Data.Models.User;


[Index(nameof(NickName), nameof(Email), IsUnique = true)]
public class User
{
    [Key]
    [Required]
    public Guid Id { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "NickName is required")]
    [MinLength(4, ErrorMessage = "NickName is shorter then 4 symbols")]
    [MaxLength(35, ErrorMessage = "Nikname is longer then 35 symbols")]
    public string NickName { get; set; }

    [EmailAddress(ErrorMessage = "Email is not correct")]
    public string? Email { get; set; } = null;

    public bool IsEmailVerified { get; set; } = false;

    [MinLength(4, ErrorMessage = "Name is shorter then 4 symbols")]
    [MaxLength(35, ErrorMessage = "Name is longer then 35 symbols")]
    public string? Name { get; set; } = null;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
    public string Password { get; set; }
}
