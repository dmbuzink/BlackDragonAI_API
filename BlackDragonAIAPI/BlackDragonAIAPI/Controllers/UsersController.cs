using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;
using BlackDragonAIAPI.Models.Validation;
using BlackDragonAIAPI.StorageHandlers;
using Jose;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BlackDragonAIAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserValidator _validator;
        private readonly IUserService _dbService;
        private readonly AuthConfig _authConfig;

        public UsersController(UserValidator validator, IUserService dbService, AuthConfig authConfig)
        {
            this._validator = validator;
            this._dbService = dbService;
            this._authConfig = authConfig;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthToken>> Login(User user)
        {
            var dbUser = await this._dbService.GetUserByName(user.Username.ToLower());
            if (dbUser == null || !BCrypt.Net.BCrypt.Verify(user.Password, dbUser.Password))
                return Unauthorized(new UnauthorizedError("Invalid username and/or password"));

            var token = JWT.Encode(new TokenContent(){Username = dbUser.Username, AuthorizationLevel = dbUser.AuthorizationLevel},
                ToByteArray(this._authConfig.Secret), JwsAlgorithm.HS256);
            return Ok(new AuthToken
            {
                Token = token,
                AuthorizationLevel = dbUser.AuthorizationLevel
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthToken>> Register(User user)
        {
            var validationResult = await this._validator.ValidateAsync(user);
            if (!validationResult.IsValid)
                return BadRequest(new BadRequestError(validationResult.ToString(" & ")));

            if (await this._dbService.GetUserByName(user.Username) != null)
                return BadRequest(new BadRequestError("Username already in use"));

            var dbUser = new User
            {
                Username = user.Username.ToLower(),
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password, 11)
            };
            await this._dbService.AddUser(dbUser);
            return await Login(user);
        }

        [HttpPut("auth/{username}")]
        public async Task<ActionResult> UpdateAuthLevel([FromRoute] string username, AuthorizationLevelInput authLevelInput)
        {
            if(!HttpContext.MeetsAuthorizationLevel(EAuthorizationLevel.ADMIN))
                return Unauthorized(new UnauthorizedError("Incorrect authorization level"));

            var dbUser = await this._dbService.GetUserByName(username.ToLower());
            if (dbUser.AuthorizationLevel == EAuthorizationLevel.GOD)
            {
                return Forbid();
            }
            await this._dbService.UpdateAuthLevel(username.ToLower(), authLevelInput.AuthorizationLevel);
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            if(!HttpContext.MeetsAuthorizationLevel(EAuthorizationLevel.ADMIN))
                return Unauthorized(new UnauthorizedError("Incorrect authorization level"));

            return Ok((await this._dbService.GetUsers(u => u.AuthorizationLevel <= EAuthorizationLevel.ADMIN)).Select(u => new User()
            {
                Username = u.Username,
                AuthorizationLevel = u.AuthorizationLevel
            }));
        }

        private static byte[] ToByteArray(string text) => 
            Encoding.ASCII.GetBytes(text);
    }
}
