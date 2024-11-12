namespace KaboomWebApi.Controllers;

[ApiController]
[Route("{lang}/api/[controller]")]
[Produces("application/json")]
public class GamesController : ControllerBase
{
    private readonly KaboomDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ILanguageService _languageService;

    public GamesController(KaboomDbContext context, IConfiguration configuration, ILanguageService languageService)
    {
        _context = context;
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<User>();
        _languageService = languageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetGames([FromQuery] string lang)
    {
        _languageService.SetLanguage(lang);
        var games = await _context.Games.ToListAsync();
        return Ok(games);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGame([FromQuery] string lang, [FromBody] Game game)
    {
        _languageService.SetLanguage(lang);
        if (await _context.Games.AnyAsync(g => g.GameID == game.GameID))
        {
            return BadRequest("Game already exists");
        }
        var gameToCreate = new Game
        {
            GameID = game.GameID,
            UserID = game.UserID,
            GridSize = game.GridSize,   
            MineCount = game.MineCount,
            Score = game.Score,
            TimeInSeconds = game.TimeInSeconds,
            DateOfGame = game.DateOfGame
        };
        _context.Games.Add(gameToCreate);
        await _context.SaveChangesAsync();
        return Ok("Game registered successfully");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGame([FromQuery] string lang, int id, [FromBody] Game game)
    {
        _languageService.SetLanguage(lang);
        var gameToUpdate = await _context.Games.FindAsync(id);
        if (gameToUpdate == null)
        {
            return NotFound("Game not found");
        }
        gameToUpdate.UserID = game.UserID;
        gameToUpdate.GridSize = game.GridSize;
        gameToUpdate.MineCount = game.MineCount;
        gameToUpdate.Score = game.Score;
        gameToUpdate.TimeInSeconds = game.TimeInSeconds;
        gameToUpdate.DateOfGame = game.DateOfGame;
        await _context.SaveChangesAsync();
        return Ok("Game updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame([FromQuery] string lang, int id)
    {
        _languageService.SetLanguage(lang);
        var gameToDelete = await _context.Games.FindAsync(id);
        if (gameToDelete == null)
        {
            return NotFound("Game not found");
        }
        _context.Games.Remove(gameToDelete);
        await _context.SaveChangesAsync();
        return Ok("Game deleted successfully");
    }
}
