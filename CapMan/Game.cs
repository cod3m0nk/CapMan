namespace CapMan;

public class Game : IGame
{
    public GameState State { get; set; } = GameState.Playing;
    public double RespawnTime { get; } = 2.0;
    public double PlayTime { get; private set; } = 0.0;
    public double RespawnCountDown { get; private set; } = 0;
    public int Lives { get; set; } = 3;
    public PlayerActor Player { get; private set; } = new();
    public EnemyActor[] Enemies { get; init; }
    public Board Board { get; } = new Board(Board.StandardBoard);
    public int Score { get; private set; }

    public Game(IEnumerable<EnemyActor> enemies)
    {
        Enemies = [.. enemies];
    }

    public void Update(double delta)
    {
        Action<double> action = State switch
        {
            GameState.Playing => Play,
            GameState.Respawning => Respawning,
            GameState.Paused => DoNothing,
            GameState.GameOver => DoNothing,
            _ => throw new Exception($"Encountered unknown GameState: {State}")
        };
        action.Invoke(delta);
        PlayTime += delta;
    }

    private void DoNothing(double _) { }

    private void Respawning(double delta)
    {
        RespawnCountDown -= delta;
        if (RespawnCountDown <= 0)
        {
            Player = new();
            //TODO: Reset enemies
            State = GameState.Playing;
            PlayTime = 0;
        }
    }

    private void Play(double delta)
    {
        Player.Update(this, delta);
        CheckEatDots();
        foreach (EnemyActor enemy in Enemies)
        {
            enemy.Update(this, delta);
            if (enemy.BoundingBox().IntersectsWith(Player.BoundingBox()))
            {
                PlayerKilled();
            }
        }
    }

    public void PlayerKilled()
    {
        RespawnCountDown = RespawnTime;
        Lives--;
        State = GameState.Respawning;
        if (Lives <= 0)
        {
            State = GameState.GameOver;
        }
    }

    private void CheckEatDots()
    {
        if (Board.IsDot(Player.Tile))
        {
            Board.RemoveElement(Player.Tile);
            Score += 10;
        }

        if (Board.IsPowerPill(Player.Tile))
        {
            Board.RemoveElement(Player.Tile);
            Score += 50;
        }
    }
}