using AutoMapper;
using HotelListingAPI.Data;
using HotelListingAPI.Data.Dto;
using HotelListingAPI.Data.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {

        private readonly UserManager<ApiUser> userManager;
        //private readonly SignInManager<ApiUser> signInManager;
        private readonly ILogger<AccountController> logger;
        private readonly IMapper mapper;
        private readonly IAuthManager authManager;

        public AccountController(UserManager<ApiUser> userManager,
                                    ILogger<AccountController> logger,
                                    IMapper mapper,
                                    IAuthManager authManager)

        {
            this.userManager = userManager;
            this.logger = logger;
            this.mapper = mapper;
            this.authManager = authManager;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            logger.LogInformation($"Registration Attempt for {userDto.Email}");
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }
            try
            {
                var user = mapper.Map<ApiUser>(userDto);
                user.UserName = userDto.Email;
                var result = await userManager.CreateAsync(user, userDto.Password );

                if (!result.Succeeded)
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                await userManager.AddToRolesAsync(user, userDto.Roles);
                return StatusCode(StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in {nameof(Register)}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
                //another return type for an exception
                //return Problem($"Something went wrong in {nameof(Register)}", statusCode: 500);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto userDto)
        {
            
            logger.LogInformation($"Login Attempt for {userDto.Email}");
            //check if the values parsed are validated, if not correct, throw an error message
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            try
            {
                //check if the user is valid, if not valid, return Unauthorized
                if (!await authManager.ValidateUser(userDto)) //either ways would work perfectly
                {
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
                //if validated, create a token for the user
                return StatusCode(StatusCodes.Status200OK, 
                    new { Token = await authManager.CreateToken()});
            } 
            catch(Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(Login)}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Something went wrong in the {nameof(Login)}");
            }
        }

    }
}
