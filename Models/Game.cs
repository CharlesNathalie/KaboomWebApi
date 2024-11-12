namespace KaboomWebApi.Models;
public class Game
{
    public int GameID { get; set; }
    public int UserID { get; set; }
    public int GridSize { get; set; }
    public int MineCount { get; set; }
    public int Score { get; set; }
    public int TimeInSeconds { get; set; }
    public DateTime DateOfGame { get; set; }
}
