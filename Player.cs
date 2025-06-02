class Player
{
    public int X, Y;
    public Player(int x, int y) { X = x; Y = y; }
    public void Move(int dx, int screenWidth)
    {
        X = Math.Clamp(X + dx, 0, screenWidth - 1);
    }
}