namespace Battleships.Engine;

/// <summary>
/// Represents the state of a cell on the grid.
/// </summary>
public enum CellState
{
    Sea,
    Ship,
    Hit,
    Miss
}