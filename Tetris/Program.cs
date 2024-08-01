using System;

class Program
{
    static void Main(string[] args)
    {
        int width = 10;
        int height = 20;
        GameField gameField = new GameField(width, height);

        Console.WriteLine("Press any key to start the game...");
        Console.ReadKey();

        while (true)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                gameField.HandleInput(keyInfo);
            }
        }
    }
}
