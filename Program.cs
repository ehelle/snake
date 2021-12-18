using System;
using System.Linq;

namespace Snake
{
    class SnakeGame
    {
        private int _width;
        private int _height;
        private int _X, _Y;
        private int _sleep;
        private int _speedIncrease;
        private Queue<(int, int)> _snake;
        private (int, int) _origCursor;
        private Area[,] _gameArea;
        private bool _endGame = false;
        private Direction _currentDirection;

        public SnakeGame(int width, int height)
        {
            _width = width;
            _height = height;
            _X = _width / 2;
            _Y = height / 2;
            _sleep = 300;
            _speedIncrease = 10;
            _gameArea = new Area[width, height];
            AddFrameToGameArea(width, height);
            _snake = new Queue<(int, int)>();
            _origCursor = Console.GetCursorPosition();
            _currentDirection = Direction.North;
            PositionFood();

        }

        private void DrawFrame()
        {
            Console.Clear();
            var border = "+" + new string('-', _width - 2) + "+";
            Console.WriteLine(border);
            for (int i = 0; i < _height - 2; i++)
            {
                Console.WriteLine("|" + new string(' ', _width - 2) + "|");
            }
            Console.WriteLine(border);
        }

        private void AddFrameToGameArea(int width, int height)
        {
            for (var i = 0; i < width; i++)
            {
                _gameArea[i, 0] = Area.Border;
                _gameArea[i, height - 1] = Area.Border;
            }
            for (var j = 1; j < height - 1; j++)
            {
                _gameArea[0, j] = Area.Border;
                _gameArea[width - 1, j] = Area.Border;
            }
        }

        private void PositionFood()
        {
            var openPositions = new List<(int, int)>();
            for (int row = 0; row < _height; row++)
            {
                for (int col = 0; col < _width; col++)
                {
                    if (_gameArea[col, row] == Area.Open)
                    {
                        openPositions.Add((col, row));
                    }
                }
            }
            var idx = new Random().Next(openPositions.Count);
            (int x, int y) = openPositions[idx];
            _gameArea[x, y] = Area.Food;
            DrawFood(x, y);
            _sleep -= _speedIncrease;
        }
        private (int, int) GetNextPos(Direction dir)
        {
            if (_snake.Count() < 1)
                throw new Exception();
            var (dx, dy) = dir switch
            {
                Direction.North => (0, -1),
                Direction.East => (1, 0),
                Direction.South => (0, 1),
                Direction.West => (-1, 0),
                _ => throw new ArgumentOutOfRangeException(nameof(dir), $"Unknown direction: {dir}"),
            };
            return (_X + dx, _Y + dy);
        }

        private Area NextAreaType(int x, int y)
        {
            return _gameArea[x, y];
        }

        private void AddFront(int x, int y)
        {
            _snake.Enqueue(new(x, y));
            _X = x;
            _Y = y;
            _gameArea[x, y] = Area.Snake;
            DrawSnake(x, y);
        }

        private void RemoveTail()
        {
            var (x, y) = _snake.Dequeue();
            _gameArea[x, y] = Area.Open;
            DrawOpen(x, y);
        }
        private void MoveSnake(Direction dir)
        {
            var (x, y) = GetNextPos(dir);
            switch (NextAreaType(x, y))
            {
                case Area.Open:
                    AddFront(x, y);
                    RemoveTail();
                    break;
                case Area.Food:
                    AddFront(x, y);
                    PositionFood();
                    break;
                default:
                    _endGame = true;
                    break;
            }
        }

        private void StartGame()
        {
            Console.CursorVisible = false;
            PositionFood();
            _snake.Enqueue(new(_X, _Y));
            while (!_endGame)
            {
                if (Console.KeyAvailable)
                {
                    SetDirection();
                }
                MoveSnake(_currentDirection);
                System.Threading.Thread.Sleep(_sleep);
            }
        }

        private void SetDirection()
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow: _currentDirection = Direction.North; break;
                case ConsoleKey.RightArrow: _currentDirection = Direction.East; break;
                case ConsoleKey.DownArrow: _currentDirection = Direction.South; break;
                case ConsoleKey.LeftArrow: _currentDirection = Direction.West; break;
                case ConsoleKey.Escape: _endGame = true; break;
            }
        }
        private void DrawPoint(int x, int y, char c)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(c);
        }

        private void DrawSnake(int x, int y)
        {
            DrawPoint(x, y, '*');
        }

        private void DrawOpen(int x, int y)
        {
            DrawPoint(x, y, ' ');
        }

        private void DrawFood(int x, int y)
        {
            DrawPoint(x, y, '@');
        }

        private void EndGame()
        {
            Console.SetCursorPosition(_origCursor.Item1, _origCursor.Item2);
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
