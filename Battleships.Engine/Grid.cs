using System;

namespace Battleships.Engine;

/// <summary>
/// Represents a grid of cells in the game.
/// </summary>
public class Grid
{
    /// <summary>
    /// The width of the grid.
    /// </summary>
    public int Width { get; init; }

    /// <summary>
    /// The height of the grid.
    /// </summary>
    public int Height { get; init; }

    /// <summary>
    /// The grid is a 2D array of cells.
    /// </summary>
    private CellState[,] cells;

    /// <summary>
    /// The total number of hits on ships.
    /// </summary>
    private int totalHits = 0;

    /// <summary>
    /// The total number of ship cells originally placed in the grid.
    /// </summary>
    private int totalShipCells = 0;

    /// <summary>
    /// Returns true if there are any ships left to be hit.
    /// </summary>
    /// <value></value>
    public bool HasFloatingShips
    {
        get => totalShipCells > totalHits;
    }

    /// <summary>
    /// Creates a 10x10 grid and initialise all cells to Sea.
    /// </summary>
    public Grid() : this(10, 10)
    {
    }

    /// <summary>
    /// Initialise the grid with the given width and height. All cells are set initially to Sea.
    /// </summary>
    /// <param name="width">The width of the grid.</param>
    /// <param name="height">The height of the grid.</param>
    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
        cells = new CellState[this.Width, this.Height];

        // Initialise all cells to be empty.
        for (int i = 0; i < this.Width; i++)
        {
            for (int j = 0; j < this.Height; j++)
            {
                cells[i, j] = CellState.Sea;
            }
        }
    }

    /// <summary>
    /// Returns the state of the cell at the given coordinates.
    /// </summary>
    /// <param name="x">The x-coordinate (zero-based).</param>
    /// <param name="y">The y-coordinate (zero-based).</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Thrown when either <paramref name="x"/> or <paramref name="y"/> is outside the boundaries of the grid. 
    /// </exception>
    /// <returns></returns>
    public CellState GetCell(int x, int y)
    {
        // Check that the coordinates are valid.
        if (x < 0 || x >= Width)
            throw new ArgumentOutOfRangeException(nameof(x), $"x-coordinates must be between 0 and {Width - 1}.");
        else if (y < 0 || y >= Height)
            throw new ArgumentOutOfRangeException(nameof(y), $"y-coordinates must be between 0 and {Height - 1}.");

        return cells[x, y];
    }

    /// <summary>
    /// Checks if the given coordinates are within the boundaries of the grid.
    /// </summary>
    /// <param name="ship">The class of the ship to place.</param>
    /// <param name="x">The x-coordinate of the cell (zero-based).</param>
    /// <param name="y">The y-coordinate of the cell (zero-based).</param>
    /// <param name="orientation">The orientation of the ship. </param>
    /// <returns>True if the ship can be placed at the given coordinates and false otherwise.</returns>
    public bool CanPlaceShip(Ship ship, int x, int y, Orientation orientation)
    {
        if (x < 0 || y < 0)
            return false;

        if (orientation == Orientation.Vertical)
        {
            // Check if the ship will fit in the grid.
            if (y + ship.Size > Height)
                return false;

            // Check if the ship will overlap with another ship in the same column.
            for (int i = 0; i < ship.Size; i++)
            {
                if (cells[x, y + i] != CellState.Sea)
                    return false;
            }
        }
        else
        {
            // Check if the ship will fit in the grid.
            if (x + ship.Size > Width)
                return false;

            // Check if the ship will overlap with another ship in the same row.
            for (int i = 0; i < ship.Size; i++)
            {
                if (cells[x + i, y] != CellState.Sea)
                    return false;
            }
        }

        // The ship will fit in the grid and will not overlap with another ship.
        return true;
    }

    /// <summary>
    /// Attempts to place a ship on the grid, with its bow at the given
    /// coordinates. If the ship is out of bounds or overlaps another ship, then
    /// it is not placed.  
    /// The x and y coordinates are zero-based with (0, 0) denoting the top-left
    /// of the grid.
    /// </summary>
    /// <param name="ship">The class of ship to place.</param>
    /// <param name="size">The number of cells the ship occupies.</param>
    /// <param name="x">The x coordinate of the bow of the ship. The coordinates
    /// are zero-based with (0,0) denoting the top-left of the grid.</param>
    /// <param name="y">The y coordinate of the bow of the ship. The coordinates
    /// are zero-based with (0,0) denoting the top-left of the grid.</param>
    /// <param name="orientation">The orientation of the ship (vertical or horizontal).</param>
    /// <returns>
    /// True if the ship can be placed in the given coordinates. False
    /// otherwise.
    /// </returns>
    public bool TryPlaceShip(Ship ship, int x, int y, Orientation orientation)
    {
        // Check if the ship can be placed at the given coordinates.
        if (!CanPlaceShip(ship, x, y, orientation))
            return false;

        // Place the ship.
        for (int i = 0; i < ship.Size; i++)
        {
            if (orientation == Orientation.Vertical)
                cells[x, y + i] = CellState.Ship;
            else
                cells[x + i, y] = CellState.Ship;
        }

        // Increment the total number of ship cells.
        totalShipCells += ship.Size;
        return true;
    }

    /// <summary>
    /// Shoots at the given coordinates and updates the state of the cell.
    /// Throws ArgumentOutOfRangeException if the coordinates are out of bounds.
    /// </summary>
    /// <param name="targetX">The x-coordinate of the target cell.</param>
    /// <param name="targetY">The y-coordinate of the target cell</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Thrown when either <paramref name="targetX"/> or <paramref name="targetY"/> is outside the boundaries of the grid. 
    /// </exception>
    /// <returns>Returns true if and only if the target was originally a Ship. Otherwise returns false.</returns>
    public bool TakeShot(int targetX, int targetY)
    {
        // Check if the target is within the grid.
        if (targetX < 0 || targetX >= Width)
            throw new ArgumentOutOfRangeException(nameof(targetX), $"The x-coordinate must be between 0 and {Width - 1}.");
        else if (targetY < 0 || targetY >= Height)
            throw new ArgumentOutOfRangeException(nameof(targetY), $"The y-coordinate must be between 0 and {Height - 1}.");

        // Update the cell state.
        switch (cells[targetX, targetY])
        {
            case CellState.Sea:
                cells[targetX, targetY] = CellState.Miss;
                return false;
            case CellState.Ship:
                cells[targetX, targetY] = CellState.Hit;
                totalHits++;
                return true;
            case CellState.Hit:
                return false;
            case CellState.Miss:
                return false;
            default:
                return false;
        }
    }
}
