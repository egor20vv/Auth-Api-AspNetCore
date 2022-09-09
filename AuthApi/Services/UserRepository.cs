using AuthApi.Data;
using AuthApi.Data.Models.User;
using AuthApi.Data.Models.User.UserViews;
using AuthApi.Services.Interfaces;
using AuthApi.Services.Responses;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace AuthApi.Services;

public class UserRepository : IRepository<Guid, User>
{
    private readonly MainDbContext _context;
    private readonly IMapper _mapper;

    public UserRepository(MainDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public IQueryable<User> GetAllQuery()
    {
        return _context.Users;
    }

    public async Task<bool> IsInstanceAsync(Guid id)
    {
        return await _context.Users.FindAsync(id) != null;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<User> CreateAsync(User newUserData)
    {
        var createdUesr = await _context.Users.AddAsync(newUserData);
        await _context.SaveChangesAsync();
        return createdUesr.Entity;
    }

    public async Task<int> UpdateAsync(User updatedUserData)
    {
        _context.Users.Update(updatedUserData);
        return await _context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(User deleteUserData)
    {
        _context.Users.Remove(deleteUserData);
        return await _context.SaveChangesAsync();
    }
}
