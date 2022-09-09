using AuthApi.Data.Models.User;
using AuthApi.Data.Models.User.UserViews;
using AuthApi.Services.Responses;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AuthApi.Services.Interfaces;

public interface IUserAuth
{
    Task<string?> GetJwt(Guid id);
    Task<Guid> GetGuidFromPrincipal(ClaimsPrincipal user);
}
