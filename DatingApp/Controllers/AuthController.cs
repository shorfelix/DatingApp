using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.Data;
using DatingApp.Dtos;
using DatingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthRepository repository, IConfiguration configuration)
        {
            _configuration = configuration;
            _repo = repository;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegistoreDto)
        {
            //validate request 

            userForRegistoreDto.UserName = userForRegistoreDto.UserName.ToLower();

            if (await _repo.UserExists(userForRegistoreDto.UserName))
                return BadRequest("Username is already exist");

            var userToCreate = new User
            {
                Username = userForRegistoreDto.UserName
            };
            var createUser = await _repo.Register(userToCreate, userForRegistoreDto.Password);

            return StatusCode(201);

        }
          [HttpPost("login")]
          public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
          {
              var userFromRepo = await _repo.Login(userForLoginDto.username.ToLower(), userForLoginDto.password);
              if (userFromRepo == null)
              {
                  return Unauthorized();
              }
              var claims = new[]
              {
            new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
            new Claim(ClaimTypes.Name,userFromRepo.Username)
          };

              var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
              var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
              var TokenDescripter= new SecurityTokenDescriptor
              {
                  Subject =new ClaimsIdentity(claims),
                  Expires = System.DateTime.Now.AddDays(1),
                  SigningCredentials= creds
              };
           var TokenHandler = new JwtSecurityTokenHandler();
           var token =TokenHandler.CreateToken(TokenDescripter);
           return Ok(new {
               token=TokenHandler.WriteToken(token)
           });
          }


    }
}