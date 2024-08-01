using System;
using System.Text;
using System.Timers;

public class GameField
{
    public int[,] field { get; private set; }
    public int width { get; private set; }
    public int height { get; private set; }

    private Tetromino _currentTetromino;
    private Timer _gameTimer;
    private StringBuilder _screenBuffer;

    public GameField(int width, int height)
    {
        this.width = width;
        this.height = height;

        field = new int[height, width];
        _currentTetromino = GenerateRandomTetromino();

        _gameTimer = new Timer(250);
        _gameTimer.Elapsed += GameTimer_Tick;
        _gameTimer.AutoReset = true;
        _gameTimer.Start();

        _screenBuffer = new StringBuilder();
    }

    private void GameTimer_Tick(object sender, ElapsedEventArgs e)
    {
        MoveTetrominoDown();
    }

    public void MoveTetrominoDown()
    {
        if (CanPlaceTetromino(_currentTetromino, _currentTetromino.X, _currentTetromino.Y + 1))
        {
            _currentTetromino.Y++;
            Draw();
        }
        else
        {
            PlaceTetromino(_currentTetromino);
            ClearLines();

            _currentTetromino = GenerateRandomTetromino();

            if (!CanPlaceTetromino(_currentTetromino, _currentTetromino.X, _currentTetromino.Y))
            {
                _gameTimer.Stop();

                Console.WriteLine("Game Over! Press any key to restart.");
                Console.ReadKey();

                StartNewGame();
            }
        }
    }

    public void Draw()
    {
        _screenBuffer.Clear();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (y >= _currentTetromino.Y && y < _currentTetromino.Y + _currentTetromino.shape.GetLength(0) &&
                    x >= _currentTetromino.X && x < _currentTetromino.X + _currentTetromino.shape.GetLength(1) &&
                    _currentTetromino.shape[y - _currentTetromino.Y, x - _currentTetromino.X] != 0)
                {
                    _screenBuffer.Append("#");
                }

                else
                {
                    _screenBuffer.Append(field[y, x] == 0 ? "." : "#");
                }
            }

            _screenBuffer.AppendLine();
        }

        Console.SetCursorPosition(0, 0);
        Console.Write(_screenBuffer.ToString());
    }

    public bool CanPlaceTetromino(Tetromino tetromino, int xOffset, int yOffset)
    {
        for (int y = 0; y < tetromino.shape.GetLength(0); y++)
        {
            for (int x = 0; x < tetromino.shape.GetLength(1); x++)
            {
                if (tetromino.shape[y, x] != 0)
                {
                    int newX = x + xOffset;
                    int newY = y + yOffset;

                    if (newX < 0 || newX >= width || newY < 0 || newY >= height || field[newY, newX] != 0)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public void PlaceTetromino(Tetromino tetromino)
    {
        for (int y = 0; y < tetromino.shape.GetLength(0); y++)
        {
            for (int x = 0; x < tetromino.shape.GetLength(1); x++)
            {
                if (tetromino.shape[y, x] != 0)
                {
                    field[tetromino.Y + y, tetromino.X + x] = tetromino.shape[y, x];
                }
            }
        }
    }

    public void ClearLines()
    {
        for (int y = height - 1; y >= 0; y--)
        {
            bool isLineFull = true;

            for (int x = 0; x < width; x++)
            {
                if (field[y, x] == 0)
                {
                    isLineFull = false;
                    break;
                }
            }

            if (isLineFull)
            {
                for (int yy = y; yy > 0; yy--)
                {
                    for (int xx = 0; xx < width; xx++)
                    {
                        field[yy, xx] = field[yy - 1, xx];
                    }
                }

                for (int xx = 0; xx < width; xx++)
                {
                    field[0, xx] = 0;
                }

                y++;
            }
        }
    }

    private Tetromino GenerateRandomTetromino()
    {
        int[][,] shapes = new int[][,]
        {
            new int[,] {{1, 1, 1, 1}}, // I
            new int[,] {{1, 1}, {1, 1}}, // O
            new int[,] {{0, 1, 0}, {1, 1, 1}}, // T
            new int[,] {{1, 0, 0}, {1, 1, 1}}, // L
            new int[,] {{0, 0, 1}, {1, 1, 1}}, // J
            new int[,] {{1, 1, 0}, {0, 1, 1}}, // S
            new int[,] {{0, 1, 1}, {1, 1, 0}} // Z
        };

        Random random = new Random();
        int index = random.Next(shapes.Length);
        return new Tetromino(shapes[index]);
    }

    public void StartNewGame()
    {
        field = new int[height, width];
        _currentTetromino = GenerateRandomTetromino();
        _gameTimer.Start();
    }

    public void HandleInput(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.A:
                if (CanPlaceTetromino(_currentTetromino, _currentTetromino.X - 1, _currentTetromino.Y))
                {
                    _currentTetromino.X--;
                }
                break;

            case ConsoleKey.D:
                if (CanPlaceTetromino(_currentTetromino, _currentTetromino.X + 1, _currentTetromino.Y))
                {
                    _currentTetromino.X++;
                }
                break;

            case ConsoleKey.W:

                Tetromino rotatedTetromino = new Tetromino(_currentTetromino); // Копируем текущую фигуру
                rotatedTetromino.Rotate();

                if (CanPlaceTetromino(rotatedTetromino, _currentTetromino.X, _currentTetromino.Y))
                {
                    _currentTetromino = rotatedTetromino;
                }

                break;

            case ConsoleKey.Spacebar:
                MoveTetrominoDown();
                break;
        }

        Draw();
    }
}
