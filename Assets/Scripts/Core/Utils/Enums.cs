namespace Core.Utils
{
    public enum GameState { Init, PlayerTurn, EnemyTurn, Animating, Paused, Win, Lose }
    public enum TileType { Empty, Hole}
    public enum PieceType { Wind, Enemy, Obstacle }
    public enum Direction { Up, Down, Left, Right, None }
}