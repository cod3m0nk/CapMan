namespace CapMan;

public class EnemyActor(Position position, double speed, Direction direction) : Actor(position, speed, direction)
{
    public Position StartPosition { get; } = position;
    public EnemyState State { get; set; } = EnemyState.Searching;
    public IEnemyBehaviour Behaviour { get; set; } = new TargetTileBehaviour(new Tile(1, 1));
    public Tile? LastTarget { get; set; }
    public bool IgnoreDoors { get; set; } = false;
    public override void Update(IGame game, double deltaTime)
    {
        if (IgnoreDoors)
        {
            game = new DelegateBoardGame(game, game.Board.WithoutDoors());
        }
        NextDirection = Behaviour.GetNextDirection(game, deltaTime, this);
        base.Update(game, deltaTime);
    }

    public void Reset()
    {
        Position = StartPosition;
    }

    private class DelegateBoardGame(IGame baseGame, Board board) : IGame
    {
        public IGame DelegateGame { get; set; } = baseGame;
        public Board Board { get; } = board;

        public GameState State { get => DelegateGame.State; set => DelegateGame.State = value; }
        public double RespawnTime => DelegateGame.RespawnTime;
        public double RespawnCountDown => DelegateGame.RespawnCountDown;
        public int Lives { get => DelegateGame.Lives; set => DelegateGame.Lives = value; }
        public PlayerActor Player => DelegateGame.Player;
        public EnemyActor[] Enemies => DelegateGame.Enemies;
        public int Score => DelegateGame.Score;
        public double PlayTime => DelegateGame.PlayTime;
        public void Update(double delta) => DelegateGame.Update(delta);
    }
}