using AuthApi.Data.Models.User;
using AuthApi.Data.Models.User.UserViews;
using AuthApi.Services.Interfaces;
using AuthApi.Services.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthApi.Services;

public class UserHandler : IUserHandler
{
    private readonly UserRepository _repository;
    private readonly IMapper _mapper;

    public UserHandler(UserRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<UserPublic?> GetByGuid(Guid userGuid)
    {
        return _mapper.Map<UserPublic>(await _repository.GetByIdAsync(userGuid));
    }

    public async Task<UserPublic?> FirstOrDefault(System.Linq.Expressions.Expression<Func<User, bool>> func)
    {
        var foundUser = await _repository.GetAllQuery().FirstOrDefaultAsync(func);
        return _mapper.Map<UserPublic?>(foundUser);
    }

    public async Task<UserRepositoryResponse<Guid>> Create(CreateUser newUserData)
    {
        {
            var userByNickMame = await _repository.GetAllQuery().FirstOrDefaultAsync(u => u.NickName == newUserData.NickName);
            var userByEmail = await _repository.GetAllQuery().FirstOrDefaultAsync(u => u.Email == newUserData.Email);

            if (userByNickMame != null)
                return new UserRepositoryResponse<Guid> { Result = UserRepositoryResponseType.NicknameOccupied };
            if (userByEmail != null)
                return new UserRepositoryResponse<Guid> { Result = UserRepositoryResponseType.EmailOccupied };
        }
        

        var user = _mapper.Map<User>(newUserData);
        user.Id = Guid.NewGuid();
        user.Password = new PasswordHasher<string>()
            .HashPassword(user.Id.ToString(), newUserData.Password);

        var createdUesr = await _repository.CreateAsync(user);

        return new UserRepositoryResponse<Guid>
        {
            Result = UserRepositoryResponseType.Success,
            Value = createdUesr.Id
        };
    }

    public async Task<UserRepositoryResponse> Update(Guid id, UpdateUser actualUserData)
    {
        var foundUser = await _repository.GetByIdAsync(id);
        if (foundUser == null)
            return new UserRepositoryResponse { Result = UserRepositoryResponseType.UserNotFound };

        if (await _repository.GetAllQuery().FirstOrDefaultAsync(u => u.NickName == actualUserData.NickName) != null)
            return new UserRepositoryResponse { Result = UserRepositoryResponseType.NicknameOccupied };

        await _repository.UpdateAsync(actualUserData.UpdateUserFromItself(foundUser));

        return new UserRepositoryResponse { Result = UserRepositoryResponseType.Success };
    }


    public async Task<UserRepositoryResponse> IsCorrectPassword(Guid id, string password)
    {
        var foundUser = await _repository.GetByIdAsync(id);
        if (foundUser == null)
            return new UserRepositoryResponse { Result = UserRepositoryResponseType.UserNotFound };

        var verificationResult = new PasswordHasher<string>()
            .VerifyHashedPassword(foundUser.Id.ToString(), foundUser.Password, password);

        if (verificationResult is PasswordVerificationResult.Success)
            return new UserRepositoryResponse { Result = UserRepositoryResponseType.Success };
        else
            return new UserRepositoryResponse { Result = UserRepositoryResponseType.IncorrectPassword };
    }

    public async Task<UserRepositoryResponse> UpdatePassword(Guid id, string oldPassword, string newPassword)
    {
        var foundUser = await _repository.GetByIdAsync(id);
        if (foundUser == null)
            return new UserRepositoryResponse { Result = UserRepositoryResponseType.UserNotFound };

        var passwordChecker = await IsCorrectPassword(id, oldPassword);
        if (passwordChecker.Result == UserRepositoryResponseType.IncorrectPassword)
            return passwordChecker;

        foundUser.Password = new PasswordHasher<string>()
            .HashPassword(id.ToString(), newPassword);
        await _repository.UpdateAsync(foundUser);

        return new UserRepositoryResponse { Result = UserRepositoryResponseType.Success };
    }
    
    public async Task<UserRepositoryResponse> UpdateEmail(Guid id, string newEmail)
    {
        var foundUser = await _repository.GetByIdAsync(id);
        if (foundUser == null)
            return new UserRepositoryResponse { Result = UserRepositoryResponseType.UserNotFound };

        if (!new EmailAddressAttribute().IsValid(newEmail))
            return new UserRepositoryResponse { Result = UserRepositoryResponseType.IncorrectEmail };

        if (await _repository.GetAllQuery().FirstOrDefaultAsync(u => u.Email == newEmail) != null)
            return new UserRepositoryResponse { Result = UserRepositoryResponseType.EmailOccupied }; 

        foundUser.IsEmailVerified = false;
        foundUser.Email = newEmail;
        await _repository.UpdateAsync(foundUser);

        return new UserRepositoryResponse { Result = UserRepositoryResponseType.Success };
    }
    
    public async Task<UserRepositoryResponse> SetEmailVerified(Guid id)
    {
        var foundUser = await _repository.GetByIdAsync(id);
        if (foundUser == null)
            return new UserRepositoryResponse { Result = UserRepositoryResponseType.UserNotFound };

        foundUser.IsEmailVerified = true;
        await _repository.UpdateAsync(foundUser);

        return new UserRepositoryResponse { Result = UserRepositoryResponseType.Success };
    }

    public async Task<UserRepositoryResponse> Delete(Guid id)
    {
        var foundUser = await _repository.GetByIdAsync(id);
        if (foundUser == null)
            return new UserRepositoryResponse { Result = UserRepositoryResponseType.UserNotFound };

        await _repository.DeleteAsync(foundUser);

        return new UserRepositoryResponse { Result = UserRepositoryResponseType.Success };
    }
}
