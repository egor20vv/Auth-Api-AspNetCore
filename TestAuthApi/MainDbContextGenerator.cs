using AuthApi.Data;
using AuthApi.Data.Models.User;
using AuthApi.Data.Models.User.UserViews;
using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAuthApi;

internal class MainDbContextGenerator
{
    private SqliteConnection _connection;
    private MainDbContext _context;
    private IMapper _mapper;

    public MainDbContextGenerator(IMapper mapper) 
    {
        var connectionString = "DataSource =:memory:";

        _connection = new SqliteConnection(connectionString);
        _connection.Open();

        var optionsBuilder = new DbContextOptionsBuilder<MainDbContext>()
            .UseSqlite(_connection);

        _context = new MainDbContext(optionsBuilder.Options);
        _context.Database.Migrate();
        var a = _context.Database.IsInMemory();
        var b =_context.Database.IsSqlite();
        var c = _context.Database.EnsureCreated();


        _mapper = mapper;
    }

    public MainDbContextGenerator AddUser(CreateUser userCreationData)
    {
        var user = _mapper.Map<User>(userCreationData);
        user.Id = Guid.NewGuid();

        _context.Users.Add(user);
        _context.SaveChanges();

        return this;
    }

    public MainDbContext Context => _context;

}
