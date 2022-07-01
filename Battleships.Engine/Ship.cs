namespace Battleships.Engine;

/// <summary>
/// Represents a ship that can be placed on a Grid.
/// </summary>
public record Ship {
    public int Size { get; init;}

    public static Ship Destroyer = new Ship { Size = 4 };
    public static Ship Battleship = new Ship { Size = 5 };
}