using DevMed.Models;
using DevMed.Models.Authentication.SignUp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using User.Management.Service.Models;
using User.Management.Service.Services;

namespace DevMed.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailServices _emailService;

        public AuthenticationController(
          UserManager<IdentityUser> userManager,
          RoleManager<IdentityRole> roleManager,
          IConfiguration configuration,
          IEmailServices emailService
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
        }


        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser, string role)
        {
            if (registerUser == null || string.IsNullOrWhiteSpace(role))
            {
                return BadRequest("Invalid request data");
            }

            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);
            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response { Status = "Error", Message = "User already exists" });
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                return NotFound(new Response { Status = "Error", Message = "This role doesn't exist" });
            }

            var user = new IdentityUser
            {
                Email = registerUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUser.UserName
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);
            if (!result.Succeeded)
            {
                // Construct error message from IdentityResult
                var errorMessage = string.Join("\n", result.Errors.Select(e => e.Description));

                // Log the actual errors to diagnose the problem.
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = $"Failed to create user: {errorMessage}" });
            }

            await _userManager.AddToRoleAsync(user, role);

            return StatusCode(StatusCodes.Status200OK,
                new Response { Status = "Success", Message = "User Created Successfully" });
        }

        [HttpGet]
        public IActionResult TestEmail()
        {
            var message = new Message(new string[]
            { "amacusnemo9596@gmail.com" },
            "Test", "<h1>Welcome to new engineer!<h1>");

            _emailService.SendEmail(message);
            return StatusCode(StatusCodes.Status200OK,
                new Response { Status = "Message", Message = "Email Sent Successfully" });
        }

    }
}