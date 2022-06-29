using Xunit;
using BattleshipsEngine;

namespace BattleshipsEngineUnitTests;

public class GridUnitTests
{
    [Theory]
    [InlineData(9, 9, Orientation.Vertical)] // Vertical ship in bottom right corner.
    [InlineData(0, 9, Orientation.Vertical)] // Vertical ship in bottom left corner.
    [InlineData(9, 0, Orientation.Horizontal)] // Horizontal ship in top right corner.
    [InlineData(-1, 1, Orientation.Horizontal)] // Horizontal ship with negative coordinate.
    public void CanPlaceShip_BowOutOfBounds_ReturnsFalse(int x, int y, Orientation orientation)
    {
        // Arrange
        var grid = new Grid(10, 10);
        var ship = Ship.Destroyer;
        // Act
        var result = grid.CanPlaceShip(ship, x, y, orientation);
        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(0, 0, Orientation.Horizontal, 2, 0, Orientation.Horizontal)] // Two horizontal ships colliding
    [InlineData(0, 0, Orientation.Vertical, 0, 3, Orientation.Vertical)] // Two vertical ships colliding
    [InlineData(0, 0, Orientation.Vertical, 0, 1, Orientation.Horizontal)] // Vertical and horizontal ships colliding
    public void CanPlaceShip_OverlapsExistingShip_ReturnsFalse(int x1, int y1, Orientation orientation1, int x2, int y2, Orientation orientation2)
    {
        // Arrange
        var grid = new Grid(10, 10);
        var ship1 = Ship.Destroyer;
        grid.PlaceShip(ship1, x1, y1, orientation1);
        var ship2 = Ship.Destroyer;
        // Act
        var result = grid.CanPlaceShip(ship2, x2, y2, orientation2);
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void PlaceShip_VerticalDestroyerAtOrigin_FourShipCellsBelowOrigin()
    {
        // Arrange
        var grid = new Grid(10, 10);
        var ship = Ship.Destroyer;
        // Act
        grid.PlaceShip(ship, 0, 0, Orientation.Vertical);
        // Assert
        Assert.Equal(CellState.Ship, grid.GetCell(0, 0));
        Assert.Equal(CellState.Ship, grid.GetCell(0, 1));
        Assert.Equal(CellState.Ship, grid.GetCell(0, 2));
        Assert.Equal(CellState.Ship, grid.GetCell(0, 3));
    }

    
    [Fact]
    public void PlaceShip_HorizontalBattleshipAtOrigin_FiveShipCellsRightOfOrigin()
    {
        // Arrange
        var grid = new Grid(10, 10);
        var ship = Ship.Battleship;
        // Act
        grid.PlaceShip(ship, 0, 0, Orientation.Horizontal);
        // Assert
        Assert.Equal(CellState.Ship, grid.GetCell(0, 0));
        Assert.Equal(CellState.Ship, grid.GetCell(1, 0));
        Assert.Equal(CellState.Ship, grid.GetCell(2, 0));
        Assert.Equal(CellState.Ship, grid.GetCell(3, 0));
    }
}