using Microsoft.AspNetCore.Identity;

namespace KaboomWebApi.Controllers
{
    [ApiController]
    [Route("{lang}/api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly KaboomDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILanguageService _languageService;

        public AuthController(
            KaboomDbContext context,
            IConfiguration configuration,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILanguageService languageService)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _languageService = languageService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromQuery] string lang, [FromBody] User user)
        {
            _languageService.SetLanguage(lang);

            var identityUser = new IdentityUser { UserName = user.Email };
            var result = await _userManager.CreateAsync(identityUser, user.Password);

            if (!result.Succeeded)
            {
                return BadRequest(GetErrorsText(result));
            }

            User userToCreate = new User
            {
                UserID = 0,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Initial = user.Initial,
            };

            try
            {
                _context.Users.Add(userToCreate);
            }
            catch (Exception ex)
            {
                return BadRequest(AuthControllerRes.UserCouldNotBeCreated);
            }

            return Ok(AuthControllerRes.UserRegisteredSuccessfully);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromQuery] string lang, [FromBody] User user)
        {
            _languageService.SetLanguage(lang);

            var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return Unauthorized(AuthControllerRes.EmailOrPasswordInvalid);
            }

            var identityUser = await _userManager.FindByEmailAsync(user.Email);
            if (identityUser == null)
            {
                return BadRequest(AuthControllerRes.UserNotFound);
            }
            var token = GenerateJwtToken(identityUser);
            if (token.StartsWith("Err: "))
            {
                return BadRequest(token);
            }
            return Ok(new { token });
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            string? userName = user.UserName;
            if (string.IsNullOrEmpty(userName))
            {
                return "Err: " + AuthControllerRes.UserNameNotConfiguredProperly;
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                return "Err: " + AuthControllerRes.JWTKeyNotConfiguredProperly;
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    
        private string GetErrorsText(IdentityResult result)
        {
            if (result.Errors.Any(e => e.Code == "DuplicateUserName"))
            {
                return AuthControllerRes.EmailAlreadyExists;
            }
            if (result.Errors.Any(e => e.Code == "PasswordTooShort"))
            {
                return AuthControllerRes.PasswordTooShort;
            }
            if (result.Errors.Any(e => e.Code == "PasswordRequiresNonAlphanumeric"))
            {
                return AuthControllerRes.PasswordRequiresNonAlphanumeric;
            }
            if (result.Errors.Any(e => e.Code == "PasswordRequiresDigit"))
            {
                return AuthControllerRes.PasswordRequiresDigit;
            }
            if (result.Errors.Any(e => e.Code == "PasswordRequiresLower"))
            {
                return AuthControllerRes.PasswordRequiresLower;
            }
            if (result.Errors.Any(e => e.Code == "PasswordRequiresUpper"))
            {
                return AuthControllerRes.PasswordRequiresUpper;
            }
            if (result.Errors.Any(e => e.Code == "PasswordRequiresUniqueChars"))
            {
                return AuthControllerRes.PasswordRequiresUniqueChars;
            }
            if (result.Errors.Any(e => e.Code == "InvalidUserName"))
            {
                return AuthControllerRes.InvalidUserName;
            }
            if (result.Errors.Any(e => e.Code == "InvalidEmail"))
            {
                return AuthControllerRes.InvalidEmail;
            }
            if (result.Errors.Any(e => e.Code == "DuplicateEmail"))
            {
                return AuthControllerRes.EmailAlreadyExists;
            }
            if (result.Errors.Any(e => e.Code == "UserAlreadyHasPassword"))
            {
                return AuthControllerRes.UserAlreadyHasPassword;
            }
            if (result.Errors.Any(e => e.Code == "UserLockoutNotEnabled"))
            {
                return AuthControllerRes.UserLockoutNotEnabled;
            }
            if (result.Errors.Any(e => e.Code == "UserAlreadyInRole"))
            {
                return AuthControllerRes.UserAlreadyInRole;
            }
            if (result.Errors.Any(e => e.Code == "UserNotInRole"))
            {
                return AuthControllerRes.UserNotInRole;
            }
            if (result.Errors.Any(e => e.Code == "PasswordMismatch"))
            {
                return AuthControllerRes.PasswordMismatch;
            }
            if (result.Errors.Any(e => e.Code == "LoginAlreadyAssociated"))
            {
                return AuthControllerRes.LoginAlreadyAssociated;
            }
            if (result.Errors.Any(e => e.Code == "InvalidToken"))
            {
                return AuthControllerRes.InvalidToken;
            }
            if (result.Errors.Any(e => e.Code == "RecoveryCodeRedemptionFailed"))
            {
                return AuthControllerRes.RecoveryCodeRedemptionFailed;
            }
            if (result.Errors.Any(e => e.Code == "ConcurrencyFailure"))
            {
                return AuthControllerRes.ConcurrencyFailure;
            }
            if (result.Errors.Any(e => e.Code == "DefaultError"))
            {
                return AuthControllerRes.DefaultError;
            }

            return AuthControllerRes.UnknownError;
        }

    }
}