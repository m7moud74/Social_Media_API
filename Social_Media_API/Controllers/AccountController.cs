using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Social_Media_API.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Social_Media_API.Service;
using Social_Media_API.Dto;
namespace Social_Media_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
       private readonly UserManager<User> _userManager;
        private readonly IConfiguration configuration;
        private readonly JwtTokenService jwtToken;

        public AccountController(UserManager<User> userManager,IConfiguration configuration,JwtTokenService jwtToken)
        {
            this._userManager = userManager;
            this.configuration = configuration;
            this.jwtToken = jwtToken;

        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO )
        {
            try
            {
                if (registerDTO == null || !ModelState.IsValid)
                    return BadRequest(new { Message = "Invalid registration data.", Errors = ModelState });
                if (registerDTO.Password != registerDTO.ConfirmPassword)
                {
                    return BadRequest(new { message = "Password and Confirm Password do not match." });

                }
                var existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Email is already in use." });
                }
                var user = new User
                {
                    UserName = registerDTO.UserName,
                    Email = registerDTO.Email,
                 
                };
                if (registerDTO.ProfilePicture != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(registerDTO.ProfilePicture.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await registerDTO.ProfilePicture.CopyToAsync(stream);
                    }

                    user.ProfilePictureUrl = "/Images/" + fileName; 
                }
                var result = await _userManager.CreateAsync(user, registerDTO.Password);
                if (result.Succeeded)
                { 
                    
                    var token = await jwtToken.GenerateJwtToken(user);
                    return Ok(new
                    {
                        message = "User registered successfully.",
                        token = token
                    });
                }
                else
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new { message = "User registration failed.", errors });
                }

            }

            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration.", error = ex.Message });
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromQuery] LogInDTO loginDTO)
        {
            try
            {
                if (loginDTO == null || !ModelState.IsValid)
                    return BadRequest(new { Message = "Invalid login data.", Errors = ModelState });
                var user = await _userManager.FindByEmailAsync(loginDTO.Email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }
                var token = await jwtToken.GenerateJwtToken(user, loginDTO.RememberMe);
                return Ok(new
                {
                    message = "Login successful.",
                    token = token
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login.", error = ex.Message });
            }
        }

    }
}
