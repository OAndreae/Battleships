// See https://aka.ms/new-console-template for more information

using Battleships.Engine;
using static System.Console;
using System;

public class Program
{
    public static void Main(string[] args)
    {
        var game = new BattleshipsGame(InputPlayerCoordinates, RandomCoordinate, DisplayMapsHorizontally);
        OutputWelcomeMessage();
        game.Run();

        // Outputs a welcome message and instructions for the player.
        void OutputWelcomeMessage()
        {
            WriteLine();
            WriteLine("Welcome to Battleships.\n\n" +
            "At each turn, enter the coordinates of the square you wish to attack (e.g. A0 for the top left corner).\n" +
            "Squares are marked as follows:\n" +
            "  ≈ - Sea\n" +
            "  S - Ship");
            Console.ForegroundColor = ConsoleColor.Red;
            Write("  X");
            Console.ResetColor();
            WriteLine(" - Hit");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Write("  M");
            Console.ResetColor();
            WriteLine(" - Miss");

            WriteLine("\nPress CMD+C or CTRL+C to exit at any time.\n");
            WriteLine("Good luck.");
        }
    }

    /// <summary>
    /// Inputs a valid pair of (x,y) coordinates from the user.
    /// </summary>
    /// <param name="gridWidth">The width of the grid. The x coordinate can be at most <c>gridWidth - 1</c></param>
    /// <param name="gridHeight">The height of the grid. The y coordinate can be at most <c>gridHeight - 1</c><</param>
    /// <returns>A pair of valid (x,y) coordinates.</returns>
    private static (int, int) InputPlayerCoordinates(int gridWidth, int gridHeight)
    {
        int y;
        int x;

        // Keep prompting the user until they enter a valid coordinate.
        while (true)
        {
            WriteLine("Please input a coordinate (e.g. A0).");
            Write("> ");

            var input = ReadLine()?.Trim().ToUpper();
            if (input?.Length != 2)
                continue;

            // Convert the character input to integers.
            y = input[0] - 'A';
            x = input[1] - '0'; ;

            if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
                continue;
            else
                return (x, y);
        }
    }

    /// <summary>
    /// Generates a random pair of coordinates such that x ∈ [0, gridWidth) and y ∈ [0, gridHeight).
    /// </summary>
    /// <param name="gridWidth">The width of the grid. The x coordinate can be at most <c>gridWidth - 1</c></param>
    /// <param name="gridHeight">The height of the grid. The y coordinate can be at most <c>gridHeight - 1</c><</param>
    /// <returns>A pair of valid (x,y) coordinates.</returns>
    private static (int, int) RandomCoordinate(int gridWidth, int gridHeight)
    {
        var random = new Random();
        return (random.Next(gridWidth), random.Next(gridHeight));
    }

    /// <summary>
    /// Displays the two grids one above the other.
    /// </summary>
    /// <param name="own">The current player's grid (displayed on top).</param>
    /// <param name="opponent">The opponent's grid (displayed at the bottom).</param>
    private static void DisplayMapsVertically(Grid own, Grid opponent)
    {
        DisplayGrid(own);
        WriteLine();
        WriteLine();
        DisplayGrid(opponent, hideShips: true);

        void DisplayGrid(Grid g, bool hideShips = false)
        {
            // Output the column headers
            Write("   ");
            for (int i = 0; i < g.Width; i++)
            {
                Write($"{i} ");
            }
            Write("\n  ");
            WriteLine(new string('_', g.Width * 2));

            // Output each row of the grid.
            for (int y = 0; y < g.Width; y++)
            {
                // Write the row letter.
                Write($"{Convert.ToChar('A' + y)}| ");

                // Output the contents of each row.
                for (int x = 0; x < g.Height; x++)
                {
                    DisplayCell(g.GetCell(x, y), hideShips);
                }
                WriteLine();
            }
        }
    }

    /// <summary>
    /// Displays the two grids side by side.
    /// </summary>
    /// <param name="own">The current player's grid (displayed on the left)</param>
    /// <param name="opponent">The opponent's grid (displayed on the right with ship positions hidden).</param>
    private static void DisplayMapsHorizontally(Grid own, Grid opponent)
    {
        const int gapLength = 16;
        const int indentSize = 3;
        const string ownMapHeader = "YOUR MAP";
        const string opponentMapHeader = "OPPONENT'S MAP";

        WriteLine();
        DisplayHeaders(own, opponent, ownMapHeader, opponentMapHeader);
        DisplayColumnNumbers(own, opponent);
        DisplayGridRows(own, opponent, hideOpponentShips: false);
        WriteLine();

        void WriteSpaces(int spaces) => Write(new string(' ', spaces));

        // Output the column headers.
        void DisplayHeaders(Grid own, Grid opponent, string ownMapHeader, string opponentMapHeader)
        {
            WriteSpaces(indentSize + own.Width - ownMapHeader.Length / 2);
            Write(ownMapHeader);
            WriteSpaces(own.Width - ownMapHeader.Length / 2);

            WriteSpaces(gapLength + opponent.Width - opponentMapHeader.Length / 2);
            Write(opponentMapHeader);
            WriteLine();
        }

        // Output the column numbers.
        void DisplayColumnNumbers(Grid own, Grid opponent)
        {
            WriteSpaces(indentSize);
            for (int i = 0; i < own.Width; i++) { Write($"{i} "); }

            WriteSpaces(gapLength);
            for (int i = 0; i < opponent.Width; i++) { Write($"{i} "); }

            WriteLine();
            WriteSpaces(indentSize - 1);
            Write(new string('_', own.Width * 2));
            WriteSpaces(gapLength);
            Write(new string('_', opponent.Width * 2));
            WriteLine();
        }

        // Output each row of the each grid.
        void DisplayGridRows(Grid own, Grid opponent, bool hideOpponentShips)
        {
            for (int y = 0; y < own.Height; y++)
            {
                // Display row from own grid.
                Write($"{Convert.ToChar('A' + y)}| ");
                for (int x = 0; x < own.Width; x++)
                {
                    DisplayCell(own.GetCell(x, y), hideOpponentShips);
                }

                WriteSpaces(gapLength - indentSize);

                // Display row from the opponent's grid.
                Write($"{Convert.ToChar('A' + y)}| ");
                for (int x = 0; x < opponent.Width; x++)
                {
                    DisplayCell(opponent.GetCell(x, y), hideOpponentShips);
                }

                WriteLine();
            }
        }
    }

    /// <summary>
    /// Outputs the cell as a coloured glyph.
    /// </summary>
    /// <param name="cell">The cell to output.</param>
    /// <param name="hideShips">Set to true to display Ship cells as Sea.</param>
    private static void DisplayCell(CellState cell, bool hideShips)
    {
        Console.ForegroundColor = cell switch
        {
            CellState.Sea => ConsoleColor.White,
            CellState.Ship => ConsoleColor.White,
            CellState.Miss => ConsoleColor.Cyan,
            CellState.Hit => ConsoleColor.Red,
            _ => throw new InvalidOperationException($"Unexpected cell type: {cell}")
        };

        char glyph = cell switch
        {
            CellState.Sea => '≈',
            CellState.Ship => hideShips ? '≈' : 'S',
            CellState.Hit => 'H',
            CellState.Miss => 'M',
            _ => throw new Exception($"Unexpected cell type: {cell}")
        };

        Write($"{glyph} ");
        Console.ResetColor();
    }
}