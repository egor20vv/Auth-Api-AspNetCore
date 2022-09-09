using AuthApi.Data.Models.User.UserViews;
using AuthApi.Services;
using AuthApi.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _baseRep;
        private readonly IUserHandler _repository;
        private readonly IUserAuth _auth;
        private readonly IMapper _mapper;

        public UserController(UserRepository baseRep, IUserHandler repository, IUserAuth auth, IMapper mapper)
        {
            _baseRep = baseRep;
            _repository = repository;
            _auth = auth;
            _mapper = mapper;
        }


        [HttpGet()]
        public async Task<ActionResult<UserPublic>> GetInfo()
        {
            var id = await _auth.GetGuidFromPrincipal(User);

            var user = await _repository.GetByGuid(id);
            
            return Ok(_mapper.Map<UserPublic>(user));
        }

        [HttpGet("All")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<UserPublic>>> GetAllUsers(int amount = 5) 
        {
            return Ok(await _baseRep.GetAllQuery().Take(amount).Select(user => _mapper.Map<UserPublic>(user)).ToListAsync());
        }
        
    }
}
