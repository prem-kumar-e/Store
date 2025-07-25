using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Store.Models;
using Store.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Data;
using System.Xml.Linq;

namespace Store.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly InvoiceService  _invoiceService;

        public AuthController(SignInManager<AppUser> signInManager, InvoiceService invoiceService)
        {
            _signInManager = signInManager;
            _invoiceService = invoiceService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return Ok(); // Successful login
            }
            return Unauthorized("Invalid Credentials"); // Failed login
        }

        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(); // Successful logout
        }

        [HttpGet("me")]
        [AllowAnonymous]
        public IActionResult GetCurrentUser()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var name = User.Identity.Name;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                return Ok(new { name, email, role });
            }
            return Unauthorized("Unauthorized");
        }


        [HttpGet("Invoices")]
        [AllowAnonymous]
        public async Task<IActionResult> GetInvoices()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                List<Invoice> invoices = await _invoiceService.GetInvoicesAsync();

                return Ok( new {invoices});
            }
            return Unauthorized("Unauthorized");
        }
    }
}

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

