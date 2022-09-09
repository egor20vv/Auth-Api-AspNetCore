using System.Text.Json.Serialization;

namespace AuthApi.Data.Models.User.UserViews;

public class UpdateUser
{
    public string? NickName { get; set; }
    public string? Name { get; set; } = null;
    
    //[JsonIgnore]
    //public string? Email { get; set; }
    //[JsonIgnore]
    //public bool? IsEmailVerified { get; set; }

    public User UpdateUserFromItself(User user)
    {
        user.NickName = NickName ?? user.NickName;
        user.Name = Name ?? user.Name;
        return user;
    }
}
