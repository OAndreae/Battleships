namespace Battleships.Engine;

/// <summary>
/// Represents a ship that can be placed on a Grid.
/// </summary>
public record Ship {
    public int Size { get; init;}

    /// <summary>
    /// A Ship of size 4.
    /// </summary>
    public static Ship Destroyer = new Ship { Size = 4 };
    
    /// <summary>
    /// A ship of size 5.
    /// </summary>
    public static Ship Battleship = new Ship { Size = 5 };
}