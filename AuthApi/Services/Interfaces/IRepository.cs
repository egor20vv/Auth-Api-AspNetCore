using AuthApi.Data.Models.User;
using AuthApi.Data.Models.User.UserViews;
using AuthApi.Services.Responses;
using System.ComponentModel.DataAnnotations;

namespace AuthApi.Services.Interfaces;

public interface IRepository<TKey, TValue>
{
    IQueryable<TValue> GetAllQuery();
    Task<bool> IsInstanceAsync(TKey id);
    Task<TValue?> GetByIdAsync(TKey id);
    Task<TValue> CreateAsync(TValue newUserData);
    Task<int> UpdateAsync(TValue updatedUserData);
    Task<int> DeleteAsync(TValue deleteUserData);
}
