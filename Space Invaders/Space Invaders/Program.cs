using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

class Program
{
    static int width = 40;
    static int height = 30;
    static char[,] screen = new char[height, width];

    static Player player = new Player(width / 2, height - 1);
    static List<Enemy> enemies = new List<Enemy>();
    static List<Bullet> bullets = new List<Bullet>();
    static bool gameRunning = true;
    static int frameDelay = 50;
    static int enemyDirection = 1;

    static DateTime lastShotTime = DateTime.MinValue;
    static TimeSpan shotCooldown = TimeSpan.FromMilliseconds(400);

    static TimeSpan bulletMoveDelay = TimeSpan.FromMilliseconds(0);
    static DateTime lastBulletMoveTime = DateTime.Now;

    static int score = 0;

    static void Main()
    {
        Console.CursorVisible = false;

        try
        {
            Console.SetWindowSize(width, height + 2); // Extra space for score
            Console.SetBufferSize(width, height + 2);
        }
        catch
        {
            Console.WriteLine("Warning: Unable to resize console window. Resize it manually if needed.");
            Thread.Sleep(1000);
        }

        InitializeEnemies();

        while (gameRunning)
        {
            HandleInput();
            UpdateGame();
            DrawScreen();
            Thread.Sleep(frameDelay);
        }

        Console.Clear();
        Console.SetCursorPosition(0, 0);
        Console.WriteLine(score == enemies.Count ? "You Win!" : "Game Over!");
        Console.WriteLine("Final Score: " + score);
    }


    static void InitializeEnemies()
    {
        for (int y = 1; y < 4; y++)
        {
            for (int x = 5; x < width - 5; x += 4)
            {
                enemies.Add(new Enemy(x, y));
            }
        }
    }

    static void HandleInput()
    {
        while (Console.KeyAvailable)
        {
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.LeftArrow) player.Move(-1, width);
            if (key == ConsoleKey.RightArrow) player.Move(1, width);
            if (key == ConsoleKey.Spacebar)
            {
                if (DateTime.Now - lastShotTime >= shotCooldown)
                {
                    bullets.Add(new Bullet(player.X, player.Y - 1, true));
                    lastShotTime = DateTime.Now;
                }
            }
        }
    }

    static void UpdateGame()
    {
        // Move bullets based on timing
        if (DateTime.Now - lastBulletMoveTime >= bulletMoveDelay)
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Y--;

                // Check if out of bounds
                if (bullets[i].Y < 0)
                {
                    bullets.RemoveAt(i);
                    continue;
                }

                // Check collision with enemies
                foreach (var enemy in enemies)
                {
                    if (!enemy.IsAlive) continue;
                    if (enemy.X == bullets[i].X && enemy.Y == bullets[i].Y)
                    {
                        enemy.IsAlive = false;
                        bullets.RemoveAt(i);
                        score++;
                        break;
                    }
                }
            }

            lastBulletMoveTime = DateTime.Now;
        }


        // Move enemies
        bool hitEdge = false;
        foreach (var enemy in enemies.Where(e => e.IsAlive))
        {
            if (enemy.X + enemyDirection < 0 || enemy.X + enemyDirection >= width)
                hitEdge = true;
        }

        foreach (var enemy in enemies.Where(e => e.IsAlive))
        {
            enemy.X += enemyDirection;
        }

        if (hitEdge)
        {
            enemyDirection *= -1;
            foreach (var enemy in enemies.Where(e => e.IsAlive))
            {
                enemy.Y++;
                if (enemy.Y >= player.Y)
                {
                    gameRunning = false;
                }
            }
        }

        if (enemies.All(e => !e.IsAlive))
        {
            gameRunning = false;
        }
    }

    static void DrawScreen()
    {
        Console.SetCursorPosition(0, 0);

        // Clear screen buffer
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                screen[y, x] = ' ';

        // Draw player
        screen[player.Y, player.X] = 'A';

        // Draw enemies safely
        foreach (var enemy in enemies)
        {
            if (enemy.IsAlive &&
                enemy.X >= 0 && enemy.X < width &&
                enemy.Y >= 0 && enemy.Y < height)
            {
                screen[enemy.Y, enemy.X] = 'W';
            }
        }

        // Draw bullets
        foreach (var bullet in bullets)
        {
            if (bullet.Y >= 0 && bullet.Y < height)
                screen[bullet.Y, bullet.X] = '|';
        }

        // Render screen
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
                Console.Write(screen[y, x]);
            Console.WriteLine();
        }

        // Display score
        Console.WriteLine("Score: " + score);
    }
}
