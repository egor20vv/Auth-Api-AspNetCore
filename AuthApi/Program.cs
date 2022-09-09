using AuthApi.Data;
using AuthApi.Data.Models.User;
using AuthApi.Services;
using AuthApi.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Db
builder.Services.AddDbContext<MainDbContext>(o =>
{
    o.UseSqlite($"Filename=Data{Path.DirectorySeparatorChar}MyDatabase.db");
});

// Mapper
builder.Services.AddSingleton(
        new MapperConfiguration(configuration => configuration.AddProfile(new UserProfile()))
    .CreateMapper());

// Services
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IUserAuth, UserAuth>();
builder.Services.AddScoped<IUserHandler, UserHandler>();


// Controllers
builder.Services.AddControllers();

// Debug
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
    };
});

// Authorization
builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
