namespace Battleships.Engine;

/// <summary>
/// Represents a ship that can be placed on a Grid.
/// </summary>
public record Ship {
    /// <summary>
    /// The number of cells the ship occupies.
    /// </summary>
    public int Size { get; init;}

    /// <summary>
    /// A Ship of size 4.
    /// </summary>
    public static Ship Destroyer { get => new Ship { Size = 4 }; }

    /// <summary>
    /// A ship of size 5.
    /// </summary>
    public static Ship Battleship { get => new Ship { Size = 5 }; }
}