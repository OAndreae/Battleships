/// <summary>
/// A ship that can be placed on a Grid.
/// </summary>
public class Ship {
    public int Size { get; init;}

    public static Ship Destroyer = new Ship { Size = 4 };
    public static Ship Battleship = new Ship { Size = 5 };
}