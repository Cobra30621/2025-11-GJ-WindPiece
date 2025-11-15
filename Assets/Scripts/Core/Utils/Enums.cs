namespace Core.Utils
{
    public enum GameState { Init, PlayerTurn, EnemyTurn, Animating, Paused, Win, Lose }
    public enum TileType { Empty, Hole, Obstacle }
    public enum PieceType { Wind, Enemy, Obstacle }
    public enum Direction { Up, Down, Left, Right, None }
    public enum WindDirection
    {
        None,
        North,
        South,
        East,
        West
    }
}