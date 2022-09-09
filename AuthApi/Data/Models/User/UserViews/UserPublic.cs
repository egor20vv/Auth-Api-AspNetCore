namespace AuthApi.Data.Models.User.UserViews;

public class UserPublic
{
    public Guid Id { get; set; }
    public string NickName { get; set; }
    public string? Email { get; set; } = null;
    public bool IsEmailVerified { get; set; } = false;
    public string? Name { get; set; } = null;
}
