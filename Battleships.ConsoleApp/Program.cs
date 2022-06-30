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
        DisplayGrid(opponent);

        void DisplayGrid(Grid g)
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
            for (int i = 0; i < g.Width; i++)
            {
                // Write the row letter.
                Write($"{Convert.ToChar('A' + i)}| ");

                // Output the contents of each row.
                for (int j = 0; j < g.Height; j++)
                {
                    Write($"{g.GetCell(i, j).ToChar()} ");
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

        void WriteSpaces(int spaces)
        {
            Write(new string(' ', spaces));
        }

        void DisplayHeaders(Grid own, Grid opponent, string ownMapHeader, string opponentMapHeader)
        {
            WriteSpaces(indentSize + own.Width - ownMapHeader.Length / 2);
            Write(ownMapHeader);
            WriteSpaces(own.Width - ownMapHeader.Length / 2);

            WriteSpaces(gapLength + opponent.Width - opponentMapHeader.Length / 2);
            Write(opponentMapHeader);
            WriteLine();
        }

        // Output the column headers
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

        void DisplayGridRows(Grid own, Grid opponent, bool hideOpponentShips)
        {
            for (int y = 0; y < own.Height; y++)
            {
                // Display row from own grid.
                Write($"{Convert.ToChar('A' + y)}| ");
                for (int x = 0; x < own.Width; x++)
                {
                    Write($"{own.GetCell(x, y).ToChar()} ");
                }

                WriteSpaces(gapLength - indentSize);

                // Display row from the opponent's grid.
                Write($"{Convert.ToChar('A' + y)}| ");
                for (int j = 0; j < opponent.Width; j++)
                {
                    Write($"{opponent.GetCell(j, y).ToChar(hideOpponentShips)} ");
                }

                WriteLine();
            }
        }
    }
}

public static class CellStateExtensions
{
    public static string ToChar(this CellState c, bool hideShips = false) => c switch
    {
        CellState.Sea => "≈",
        CellState.Ship => hideShips ? "≈" : "S",
        CellState.Hit => "H",
        CellState.Miss => "M",
        _ => throw new Exception("Unknown cell state")
    };
}