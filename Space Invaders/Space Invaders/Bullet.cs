class Bullet
{
    public int X, Y;
    public bool IsFromPlayer;
    public Bullet(int x, int y, bool isFromPlayer)
    {
        X = x; Y = y; IsFromPlayer = isFromPlayer;
    }
}