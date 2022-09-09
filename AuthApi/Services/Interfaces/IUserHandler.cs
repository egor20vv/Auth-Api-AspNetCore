using AuthApi.Data.Models.User;
using AuthApi.Data.Models.User.UserViews;
using AuthApi.Services.Responses;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace AuthApi.Services.Interfaces;

public interface IUserHandler
{
    Task<UserPublic?> GetByGuid(Guid userGuid);

    Task<UserPublic?> FirstOrDefault(Expression<Func<User, bool>> func);  

    Task<UserRepositoryResponse<Guid>> Create(CreateUser newUserData);

    Task<UserRepositoryResponse> Update(Guid id, UpdateUser actualUserData);
    Task<UserRepositoryResponse> UpdatePassword(Guid id, string oldPassword, string newPassword);
    Task<UserRepositoryResponse> IsCorrectPassword(Guid id, string password);
    Task<UserRepositoryResponse> UpdateEmail(Guid id, [EmailAddress] string newEmail);
    Task<UserRepositoryResponse> SetEmailVerified(Guid id);

    Task<UserRepositoryResponse> Delete(Guid id);
}
