using System;
using System.Linq;

namespace Snake
{
    class SnakeGame
    {
        private int mWidth;
        private int mHeight;
        private int mX, mY;
        private int sleep;
        private int speedIncrease;
        private Queue<(int, int)> snake;
        private (int, int) origCursor;
        private Area[,] gameArea;
        private bool endGame = false;
        private Direction currentDirection;

        public SnakeGame(int width, int height)
        {
            mWidth = width;
            mHeight = height;
            mX = mWidth / 2;
            mY = height / 2;
            sleep = 300;
            speedIncrease = 10;
            gameArea = new Area[width, height];
            addFrameToGameArea(width, height);
            snake = new Queue<(int, int)>();
            origCursor = Console.GetCursorPosition();
            currentDirection = Direction.North;
            positionFood();

        }

        public void DrawFrame()
        {
            Console.Clear();
            var border = "+" + new string('-', mWidth - 2) + "+";
            Console.WriteLine(border);
            for (int i = 0; i < mHeight - 2; i++)
            {
                Console.WriteLine("|" + new string(' ', mWidth - 2) + "|");
            }
            Console.WriteLine(border);
        }

        private void addFrameToGameArea(int width, int height)
        {
            for (var i = 0; i < width; i++)
            {
                gameArea[i, 0] = Area.Border;
                gameArea[i, height - 1] = Area.Border;
            }
            for (var j = 1; j < height - 1; j++)
            {
                gameArea[0, j] = Area.Border;
                gameArea[width - 1, j] = Area.Border;
            }
        }

        private void positionFood()
        {
            var openPositions = new List<(int, int)>();
            for (int row = 0; row < mHeight; row++)
            {
                for (int col = 0; col < mWidth; col++)
                {
                    if (gameArea[col, row] == Area.Open)
                    {
                        openPositions.Add((col, row));
                    }
                }
            }
            var idx = new Random().Next(openPositions.Count);
            (int x, int y) = openPositions[idx];
            gameArea[x, y] = Area.Food;
            DrawFood(x, y);
            sleep -= speedIncrease;
        }
        private (int, int) getNextPos(Direction dir)
        {
            if (snake.Count() < 1)
                throw new Exception();
            var (dx, dy) = dir switch
            {
                Direction.North => (0, -1),
                Direction.East => (1, 0),
                Direction.South => (0, 1),
                Direction.West => (-1, 0),
                _ => throw new ArgumentOutOfRangeException(nameof(dir), $"Unknown direction: {dir}"),
            };
            return (mX + dx, mY + dy);
        }

        private Area nextAreaType(int x, int y)
        {
            return gameArea[x, y];
        }

        public void addFront(int x, int y)
        {
            snake.Enqueue(new(x, y));
            mX = x;
            mY = y;
            gameArea[x, y] = Area.Snake;
            DrawSnake(x, y);
        }

        public void removeTail()
        {
            var (x, y) = snake.Dequeue();
            gameArea[x, y] = Area.Open;
            DrawOpen(x, y);
        }
        public void MoveSnake(Direction dir)
        {
            var (x, y) = getNextPos(dir);
            switch (nextAreaType(x, y))
            {
                case Area.Open:
                    addFront(x, y);
                    removeTail();
                    break;
                case Area.Food:
                    addFront(x, y);
                    positionFood();
                    break;
                default:
                    endGame = true;
                    break;
            }
        }

        public void StartGame()
        {
            Console.CursorVisible = false;
            positionFood();
            snake.Enqueue(new(mX, mY));
            while (!endGame)
            {
                if (Console.KeyAvailable)
                {
                    SetDirection();
                }
                MoveSnake(currentDirection);
                System.Threading.Thread.Sleep(sleep);
            }
        }

        private void SetDirection()
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow: currentDirection = Direction.North; break;
                case ConsoleKey.RightArrow: currentDirection = Direction.East; break;
                case ConsoleKey.DownArrow: currentDirection = Direction.South; break;
                case ConsoleKey.LeftArrow: currentDirection = Direction.West; break;
                case ConsoleKey.Escape: endGame = true; break;
            }
        }
        public void DrawPoint(int x, int y, char c)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(c);
        }

        public void DrawSnake(int x, int y)
        {
            DrawPoint(x, y, '*');
        }

        public void DrawOpen(int x, int y)
        {
            DrawPoint(x, y, ' ');
        }

        public void DrawFood(int x, int y)
        {
            DrawPoint(x, y, '@');
        }

        public void EndGame()
        {
            Console.SetCursorPosition(origCursor.Item1, origCursor.Item2);
            Console.WriteLine("Game Over!");
            Console.CursorVisible = true;
        }

        public void Run()
        {
            DrawFrame();
            StartGame();
            EndGame();
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var game = new SnakeGame(50, 20);
            game.Run();

        }
    }

    enum Direction
    {
        North, South, East, West
    }

    enum Area
    {
        Open, Snake, Border, Food
    }
}
