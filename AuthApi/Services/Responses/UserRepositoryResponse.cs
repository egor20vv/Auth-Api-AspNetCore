namespace AuthApi.Services.Responses;

public enum UserRepositoryResponseType
{
    None,
    Success,
    BadRequest,
    UserNotFound,
    IncorrectPassword,
    IncorrectEmail,
    NicknameOccupied,
    EmailOccupied,
    Email_N_NicknameOccupied
}

public class UserRepositoryResponse
{
    public UserRepositoryResponseType Result { get; set; } = UserRepositoryResponseType.None;
    public Exception? Exception { get; set; } = null;
}

public class UserRepositoryResponse<T> : UserRepositoryResponse
{
    public T Value { get; set; }
}