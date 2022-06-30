using System;
using System.Text;

namespace Battleships.Engine;
public class Grid
{

    // The width of the grid.
    public int Width { get; init; }
    // The height of the grid.
    public int Height { get; init; }

    // The grid is a 2D array of cells.
    private CellState[,] cells;

    // The total number of hits on ships.
    private int totalHits = 0;
    // The total number of ship cells originally placed in the grid.
    private int totalShipCells = 0;

    // Returns true if there are any ships left to be hit.
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

    public CellState GetCell(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            throw new ArgumentOutOfRangeException("Coordinates out of bounds");

        return cells[x, y];
    }

    /// <summary>
    /// Checks if the given cell is within the grid boundaries and whether the cell overlaps with another ship.
    /// </summary>
    /// <param name="size"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="isVertical"></param>
    /// <returns></returns>
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
    /// Attempt to place a ship on the grid, with its bow at the given
    /// coordinates. If the ship is out of bounds or overlaps another ship, then
    /// it is not placed.  
    /// The x and y coordinates are zero-based with (0, 0) denoting the top-left
    /// of the grid.
    /// </summary>
    /// <param name="size">The number of cells the ship occupies.</param>
    /// <param name="x">The x coordinate of the bow of the ship. The coordinates
    /// are zero-based with (0,0) denoting the top-left of the grid.</param>
    /// <param name="y">The y coordinate of the bow of the ship. The coordinates
    /// are zero-based with (0,0) denoting the top-left of the grid.</param>
    /// <param name="isVertical">Whether the ship is vertical or horizontal.
    /// </param>
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
    /// <returns>True if and only if the target was originally a Ship. Otherwise returns false.</returns>
    public bool TakeShot(int targetX, int targetY)
    {
        if (targetX < 0 || targetX >= Width || targetY < 0 || targetY >= Height)
            throw new ArgumentOutOfRangeException($"The coordinate {(targetX, targetY)} is out of bounds: [0,0] to [{Width - 1}, {Height - 1}]");

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
