namespace KaboomWebApi.Controllers;

[ApiController]
[Route("{lang}/api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly KaboomDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ILanguageService _languageService;

    public UsersController(KaboomDbContext context, IConfiguration configuration, ILanguageService languageService)
    {
        _context = context;
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<User>();
        _languageService = languageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromRoute] string lang)
    {
        _languageService.SetLanguage(lang);
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromRoute] string lang, [FromBody] User user)
    {
        _languageService.SetLanguage(lang);
        if (await _context.Users.AnyAsync(u => u.Email == user.Email))
        {
            return BadRequest("Email already exists");
        }
        var userToRegister = new User
        {
            Email = user.Email,
            Password = _passwordHasher.HashPassword(user, user.Password)
        };
        _context.Users.Add(userToRegister);
        await _context.SaveChangesAsync();
        return Ok("User registered successfully");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser([FromRoute] string lang, int id, [FromBody] User user)
    {
        _languageService.SetLanguage(lang);
        var userToUpdate = await _context.Users.FindAsync(id);
        if (userToUpdate == null)
        {
            return NotFound("User not found");
        }
        userToUpdate.Email = user.Email;
        userToUpdate.Password = _passwordHasher.HashPassword(user, user.Password);
        await _context.SaveChangesAsync();
        return Ok("User updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser([FromRoute] string lang, int id)
    {
        _languageService.SetLanguage(lang);
        var userToDelete = await _context.Users.FindAsync(id);
        if (userToDelete == null)
        {
            return NotFound("User not found");
        }
        _context.Users.Remove(userToDelete);
        await _context.SaveChangesAsync();
        return Ok("User deleted successfully");
    }
}
