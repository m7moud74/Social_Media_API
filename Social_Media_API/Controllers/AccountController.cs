using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Social_Media_API.Model;
using Social_Media_API.Dto.Account_DTO;
using Social_Media_API.Service;
using Swashbuckle.AspNetCore.Annotations;

namespace Social_Media_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration configuration;
        private readonly JwtTokenService jwtToken;
        private readonly IWebHostEnvironment _environment;

        public AccountController(UserManager<User> userManager, IConfiguration configuration, JwtTokenService jwtToken, IWebHostEnvironment environment)
        {
            this._userManager = userManager;
            this.configuration = configuration;
            this.jwtToken = jwtToken;
            _environment = environment;
        }

        [HttpPost("Register")]
        //[SwaggerOperation(
        //    Summary = "Register a new user",
        //    Description = "Creates a new account for the user with optional profile picture upload."
        //)]
        //[SwaggerResponse(200, "User registered successfully")]
        //[SwaggerResponse(400, "Invalid registration data or email already in use")]
        //[SwaggerResponse(500, "An error occurred during registration")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            try
            {
                if (registerDTO == null || !ModelState.IsValid)
                    return BadRequest(new { Message = "Invalid registration data.", Errors = ModelState });

                if (registerDTO.Password != registerDTO.ConfirmPassword)
                    return BadRequest(new { message = "Password and Confirm Password do not match." });

                var existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);
                if (existingUser != null)
                    return BadRequest(new { message = "Email is already in use." });

                var user = new User
                {
                    UserName = registerDTO.UserName,
                    Email = registerDTO.Email
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
                    return Ok(new { message = "User registered successfully.", token });
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
        //[SwaggerOperation(
        //    Summary = "User login",
        //    Description = "Authenticates a user and generates a JWT token."
        //)]
        //[SwaggerResponse(200, "Login successful")]
        //[SwaggerResponse(400, "Invalid login data")]
        //[SwaggerResponse(401, "Invalid email or password")]
        //[SwaggerResponse(500, "An error occurred during login")]
        public async Task<IActionResult> Login([FromQuery] LogInDTO loginDTO)
        {
            try
            {
                if (loginDTO == null || !ModelState.IsValid)
                    return BadRequest(new { Message = "Invalid login data.", Errors = ModelState });

                var user = await _userManager.FindByEmailAsync(loginDTO.Email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
                    return Unauthorized(new { message = "Invalid email or password." });

                var token = await jwtToken.GenerateJwtToken(user, loginDTO.RememberMe);
                return Ok(new { message = "Login successful.", token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login.", error = ex.Message });
            }
        }

        [HttpDelete("delete-account")]
        [Authorize]
        //[SwaggerOperation(
        //    Summary = "Delete account",
        //    Description = "Deletes the logged-in user's account after confirming password."
        //)]
        //[SwaggerResponse(200, "Account deleted successfully")]
        //[SwaggerResponse(400, "Invalid password or deletion failed")]
        //[SwaggerResponse(401, "Unauthorized - user not found in token")]
        //[SwaggerResponse(404, "User not found")]
        //[SwaggerResponse(500, "An error occurred during account deletion")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountDTO deleteAccountDTO)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "User not found in token" });

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                if (!await _userManager.CheckPasswordAsync(user, deleteAccountDTO.Password))
                    return BadRequest(new { message = "Invalid password" });

                if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                {
                    await DeleteProfilePicture(user.ProfilePictureUrl);
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new { message = "Account deletion failed", errors = result.Errors.Select(e => e.Description) });
                }

                return Ok(new { message = "Account deleted successfully" });
            }
            catch
            {
                return StatusCode(500, new { message = "Error deleting account" });
            }
        }

        private async Task DeleteProfilePicture(string profilePictureUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(profilePictureUrl)) return;

                var fileName = Path.GetFileName(profilePictureUrl);
                var filePath = Path.Combine(_environment.WebRootPath, "Images", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch
            {
            }
        }
    }
}
