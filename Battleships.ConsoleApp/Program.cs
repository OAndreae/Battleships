// See https://aka.ms/new-console-template for more information

using Battleships.Engine;
using static System.Console;
using System;

public class Program
{
    public static void Main(string[] args)
    {
        var game = new BattleshipsGame(InputPlayerCoordinates, RandomCoordinate, DisplayMapsHorizontally);
        game.Run();
    }

    private static (int, int) InputPlayerCoordinates(int gridWidth, int gridHeight)
    {
        int y;
        int x;
        while (true)
        {
            WriteLine("Please input a coordinate (e.g. A0).");
            Write("> ");

            var input = ReadLine()?.Trim().ToUpper();
            if (input?.Length != 2)
                continue;

            y = input[0] - 'A';
            x = input[1] - '0'; ;

            if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
                continue;
            else
                return (x, y);
        }
    }

    private static (int, int) RandomCoordinate(int gridWidth, int gridHeight)
    {
        var random = new Random();
        return (random.Next(gridWidth), random.Next(gridHeight));
    }

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

    private static void DisplayMapsHorizontally(Grid own, Grid opponent)
    {
        const int gapLength = 16;
        const int indentSize = 3;
        const string ownMapHeader = "YOUR MAP";
        const string opponentMapHeader = "OPPONENT'S MAP";

        DisplayHeaders(own, opponent, ownMapHeader, opponentMapHeader);
        DisplayColumnNumbers(own, opponent);
        DisplayGridRows(own, opponent, hideOpponentShips: false);

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