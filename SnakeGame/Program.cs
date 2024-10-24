using System;
using System.Diagnostics;
using System.IO;

enum Direction {Up, Down, Left, Right}

internal class Program
{
    public static bool gameOver = false;
    public static Food food;
    public static Player player;
    private static void Main(string[] args)
    {
        Stopwatch sw = new Stopwatch();

        int turns = 0;
        bool endOfGame = false;

        Direction playerDirection = Direction.Right;

        player = new Player(12, 12);
        Map map = new Map(12);
        food = new Food(new Vector2(new Random().Next(0, map.size), new Random().Next(0, map.size)), map.size);

        Console.CursorVisible = false;
        Console.Title = "Snake Game";

        map.PrintMap(player);
        sw.Start();

        while (!gameOver)
        {
            ConsoleKeyInfo key = new ConsoleKeyInfo();
            if(sw.ElapsedMilliseconds > 100)
            {
                Console.Clear();
                if (playerDirection == Direction.Up) player.y--;
                else if (playerDirection == Direction.Down) player.y++;
                else if (playerDirection == Direction.Right) player.x++;
                else if (playerDirection == Direction.Left) player.x--;
                map.PrintMap(player);

                player.positionsMoved.Add(new Vector2(player.x, player.y));

                for (int i = 0; i < player.Length; i++)
                {
                    if (i == 0) player.positionsPlayer[0] = new Vector2(player.x, player.y);
                    else if (i == 1) player.positionsPlayer[1] = player.positionsPlayer[0];
                    else player.positionsPlayer[i] = player.positionsMoved[player.positionsMoved.Count - i];
                    if (player.positionsPlayer[i] == new Vector2(player.x, player.y))
                    {
                        gameOver = true;
                    }
                }

                turns++;
                sw.Restart();
            }

            if (Console.KeyAvailable)
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.RightArrow && playerDirection != Direction.Left) { playerDirection = Direction.Right; }
                if (key.Key == ConsoleKey.LeftArrow && playerDirection != Direction.Right) { playerDirection = Direction.Left; }
                if (key.Key == ConsoleKey.UpArrow && playerDirection != Direction.Down) { playerDirection = Direction.Up; }
                if (key.Key == ConsoleKey.DownArrow && playerDirection != Direction.Up) { playerDirection = Direction.Down; } 
            }
        }
        Console.ForegroundColor = ConsoleColor.White;
        string path = @"C:\\Users\\User\\source\\repos\\SnakeGame\Points.txt" ?? "Points.txt";
        int record = Convert.ToInt32(File.ReadAllLines(path)[0]);
        if(player.Length > record)
        {
            Console.WriteLine("¡Enhorabuena!, has superado tu récord.");
            File.WriteAllText(path, Convert.ToString(player.Length));
        }
        Console.WriteLine($"Puntos: {player.Length}.");
        //Console.ForegroundColor = ConsoleColor.Black;
        while (true) { }
    }

    static void ChangeColor(ConsoleColor c)
    {
        Console.ForegroundColor = c;
        Console.BackgroundColor = c;
    }

    class Map
    {
        public int size;
        public Map(int size)
        {
            this.size = size;
        }

        public void PrintMap(Player pl)
        {
            if (pl.y > this.size - 1) pl.y = 0;
            if (pl.x > this.size - 1) pl.x = 0;
            if (pl.y < 0) pl.y = this.size - 1;
            if (pl.x < 0) pl.x = this.size - 1;

            for (int i = 0; i < size;  i++)
            {
                for (int j = 0; j < size; j++)
                {
                    ChangeColor(ConsoleColor.Gray);

                    if (pl.y == i && pl.x == j) ChangeColor(ConsoleColor.DarkGreen);
                    else if (food.position.y == i && food.position.x == j) ChangeColor(ConsoleColor.DarkRed);

                    if (pl.x == food.position.x && pl.y == food.position.y) { player.SizePlus(); food.RandomizerPosition(player); }

                    for (int k = 0; k < pl.positionsPlayer.Count; k++)
                    {
                        if (pl.positionsPlayer[k].x == pl.x && pl.positionsPlayer[k].y == pl.y)
                        {
                            gameOver = true;
                        }
                        else if (pl.positionsPlayer[k].x == j && pl.positionsPlayer[k].y == i)
                        {
                            ChangeColor(ConsoleColor.Green);
                        }
                    }
                    Console.Write("  ");
                }
                Console.WriteLine();
            }
            ChangeColor(ConsoleColor.Black);
        }
    }

    public class Food
    {
        public Vector2 position;
        int max;
        public Food(Vector2 position, int max)
        {
            this.position = position;
            this.max = max;
        }

        public void RandomizerPosition(Player player)
        {
            position = new Vector2(new Random().Next(0, max), new Random().Next(0, max));
            for(int i = 0; i < player.Length; i++)
            {
                if (player.positionsPlayer[i] == position || position == new Vector2(player.x, player.y))
                {
                    position = new Vector2(new Random().Next(0, max), new Random().Next(0, max));
                    i = 0;
                }
            }
        }
    }

    public class Player
    {
        public int x { get; set; }
        public int y { get; set; }
        public List<Vector2> positionsPlayer = new List<Vector2>();
        public List<Vector2> positionsMoved = new List<Vector2>();
        public int Length { get; set; } 
        public Player(int x, int y)
        {
            this.x = x;
            this.y = y;
            positionsMoved.Add(new Vector2(x, y));
            SizePlus();
            SizePlus();
            SizePlus();
        }

        public void SizePlus()
        {
            if (positionsPlayer.Count == 0) { positionsPlayer.Add(new Vector2(x, y)); }
            else if(positionsPlayer.Count == 1) { positionsPlayer.Add(positionsPlayer[0]); }
            else { positionsPlayer.Add(new Vector2(positionsMoved[positionsMoved.Count - 1].x, positionsMoved[positionsMoved.Count - 1].y)); }
            Length++;
        }
    }

    public class Vector2
    {
        public int x { get; }
        public int y { get; }
        public Vector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}