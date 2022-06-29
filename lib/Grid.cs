using System;

namespace BattleshipsEngine;
public class Grid
{

    // The width of the grid.
    private readonly int _width = 10;
    // The height of the grid.
    private readonly int _height = 10;

    // The grid is a 2D array of cells.
    private CellState[,] cells;

    // The total number of hits on ships.
    private int totalHits = 0;
    // The total number of ship cells originally placed in the grid.
    private int totalShipCells = 0;

       // Returns true if there are any ships left to be hit.
    public bool HasFloatingShips {
        get => totalShipCells > totalHits;
    }

    // Create a 10x10 grid and initialize all cells to Sea.
    public Grid() : this (10, 10)
    {
    }

    /// <summary>
    /// Initialise the grid with the given width and height. ALl cells are set to Sea.
    /// </summary>
    /// <param name="width">The width of the grid.</param>
    /// <param name="height">The height of the grid.</param>
    public Grid(int width, int height) {
        _width = width;
        _height = height;
        cells = new CellState[_width, _height];

        // Initialise all cells to be empty.
        for (int i = 0; i < _width; i++) {
            for (int j = 0; j < _height; j++) {
                cells[i, j] = CellState.Sea;
            }
        }
    }

    public CellState GetCell(int x, int y) {
        if(x < 0 || x >= _width || y < 0 || y >= _height)
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
    public bool CanPlaceShip(Ship ship, int x, int y, Orientation orientation) {
        if(x < 0 || y < 0)
            return false;
        
        if (orientation == Orientation.Vertical) {
            // Check if the ship will fit in the grid.
            if (y + ship.Size > _height)
                return false;

            // Check if the ship will overlap with another ship in the same column.
            for (int i = 0; i < ship.Size; i++) {
                if (cells[x, y + i] != CellState.Sea)
                    return false;
            }
        } else {
            // Check if the ship will fit in the grid.
            if (x + ship.Size > _width)
                return false;

            // Check if the ship will overlap with another ship in the same row.
            for (int i = 0; i < ship.Size; i++) {
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
    public bool PlaceShip(Ship ship, int x, int y, Orientation orientation) 
    {
        // Check if the ship can be placed at the given coordinates.
        if (!CanPlaceShip(ship, x, y, orientation))
            return false;
        
        // Place the ship.
        for (int i = 0; i < ship.Size; i++) {
            if (orientation == Orientation.Vertical)
                cells[x, y + i] = CellState.Ship;
            else
                cells[x + i, y] = CellState.Ship;
        }

        // Increment the total number of ship cells.
        totalShipCells += ship.Size;
        return true;
    }
}
