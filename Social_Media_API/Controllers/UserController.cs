using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Social_Media_API.Dto;
using Social_Media_API.Dto.Account;
using Social_Media_API.Model;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Social_Media_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        public UserController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPut]
        
        public async Task<IActionResult> Update(UpdateUserDto userDto)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null)
                return Unauthorized();
            var existingUser = await userManager.FindByIdAsync(currentUserId);
            if (existingUser == null)
                return NotFound("User not found.");
            if (!string.IsNullOrWhiteSpace(userDto.Name))
                existingUser.UserName = userDto.Name;
            if (!string.IsNullOrWhiteSpace(userDto.Email))
                existingUser.Email = userDto.Email;
            if (userDto.ImageUrl != null && userDto.ImageUrl.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userDto.ImageUrl.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await userDto.ImageUrl.CopyToAsync(stream);
                }

                existingUser.ProfilePictureUrl = $"/images/{fileName}";
            }
            if (!string.IsNullOrEmpty(userDto.CurrPassword) && !string.IsNullOrEmpty(userDto.NewPassword))
            {
                var passwordResult = await userManager.ChangePasswordAsync(
                    existingUser,
                    userDto.CurrPassword,
                    userDto.NewPassword
                );

                if (!passwordResult.Succeeded)
                    return BadRequest(passwordResult.Errors);
            }
            var updateResult = await userManager.UpdateAsync(existingUser);
            if (!updateResult.Succeeded)
                return BadRequest(updateResult.Errors);


            return Ok(new
            {
                message = "User updated successfully.",
                existingUser.UserName,
                existingUser.Email,
                existingUser.ProfilePictureUrl
            });

        }
    }
}
