using AuthApi.Data.Models.User;
using AuthApi.Data.Models.User.UserViews;
using AuthApi.Services;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAuthApi.Services;

public class UserRepositoryTests
{
    private readonly IMapper _mapper;

    public UserRepositoryTests()
    {
        _mapper = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile(new UserProfile());
        })
        .CreateMapper();
    }

    [Theory]
    [InlineData("SomePassword", "SomePassword")]
    [InlineData("SomePassword", "SomeWrongPassword")]
    public void HashTest(string createdPassword, string checkedPassword)
    {
        var context = new MainDbContextGenerator(_mapper).Context;

        var service = new UserRepository(context, _mapper);

        var newUserData = new CreateUser
        {
            NickName = "@user",
            Password = createdPassword
        };

        var createdUser = service.CreateUser(newUserData).GetAwaiter().GetResult();
        var isCorrect = service.IsCorrectPassword(createdUser.Id, checkedPassword).GetAwaiter().GetResult();

        Assert.NotEqual(createdPassword, context.Users.FirstOrDefault(u => u.Id == createdUser.Id)?.Password);
        Assert.True(!isCorrect ^ createdPassword == checkedPassword);


    }

}
