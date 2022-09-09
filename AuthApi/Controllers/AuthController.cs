using AuthApi.Data;
using AuthApi.Data.Models;
using AuthApi.Data.Models.User;
using AuthApi.Data.Models.User.UserViews;
using AuthApi.Services.Interfaces;
using AuthApi.Services.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace AuthApi.Controllers;

public interface IResponseStringBuilder<T>
{
    string Invoke(T key);
}

public class AuthBadRequestStringBuilder : IResponseStringBuilder<UserRepositoryResponseType>
{
    private readonly IConfiguration _configuration;
    public AuthBadRequestStringBuilder(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Invoke(UserRepositoryResponseType key) =>
        _configuration[$"UserRepositoryResponseText:{Enum.GetName(typeof(UserRepositoryResponseType), key)}"];
}


[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IUserHandler _repository;
    private readonly IUserAuth _auth;
    private readonly IConfiguration _configuration;
    private readonly AuthBadRequestStringBuilder _brStringBuilder;

    public AuthController(IUserHandler repository, IUserAuth auth, IConfiguration configuration) 
    {
        _repository = repository;
        _auth = auth;
        _configuration = configuration;

        _brStringBuilder = new AuthBadRequestStringBuilder(configuration);
    }


    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser(CreateUser newUser)
    {
        // Create and save a new user
        var createionResponse = await _repository.Create(newUser);
        
        // If unsuccess return bad request
        if (createionResponse.Result != UserRepositoryResponseType.Success)
            return BadRequest(_brStringBuilder.Invoke(createionResponse.Result).ToString());

        // get user.id
        var createdUserId = createionResponse.Value!;
        // create jwt
        var createdJwt = await _auth.GetJwt(createdUserId);

        return Ok(new UserIdentity
        {
            Id = createdUserId,
            Token = createdJwt!
        });

    }


    [HttpPost("Authenticate")]
    public async Task<IActionResult> AuthUser(GetUser user)
    {
        // try to find the user
        var foundUser = await _repository.FirstOrDefault(u => u.Email == user.NicknameOrEmail || u.NickName == user.NicknameOrEmail);

        // if it is not found, report a bad request
        if (foundUser == null)
            return BadRequest(_brStringBuilder.Invoke(UserRepositoryResponseType.UserNotFound));

        // check the password is correct for a found user
        var passwordCheck = await _repository.IsCorrectPassword(foundUser.Id, user.Password);
        // if not, report a bad request
        if (passwordCheck.Result != UserRepositoryResponseType.Success)
            return BadRequest(_brStringBuilder.Invoke(passwordCheck.Result));

        // create jwt
        var jwt = await _auth.GetJwt(foundUser.Id);

        return Ok(new UserIdentity
        {
            Id = foundUser.Id,
            Token = jwt!
        });
    }

}
