using System;
namespace Battleships.Engine;

public enum Player
{
    Player1,
    Player2
}

public delegate (int, int) CoordinateInput(int gridWidth, int gridHeight);
public delegate void MapDisplay(Grid own, Grid opponent);

public class BattleshipsGame
{
    private Player currentPlayer;
    private CoordinateInput GetPlayer1Input;
    private CoordinateInput GetPlayer2Input;
    private MapDisplay displayMaps;

    // Grid storing the state of player 1's ships (including the hits and misses of player 2)
    private Grid grid1;
    // Grid storing the state of player 2's ships (including the hits and misses of player 1)
    private Grid grid2;
    public BattleshipsGame(CoordinateInput p1Input, CoordinateInput p2Input, MapDisplay displayMaps)
    {
        GetPlayer1Input = p1Input;
        GetPlayer2Input = p2Input;
        this.displayMaps = displayMaps;

        grid1 = new Grid(10, 10);
        grid2 = new Grid(10, 10);
        PlaceShipsOnGrid(grid1);
        PlaceShipsOnGrid(grid2);

        currentPlayer = Player.Player1;
    }

    /// <summary>
    /// Places two destroyers and one battleship on the grid.
    /// </summary>
    /// <param name="grid">The Grid to place ships on.</param>
    private void PlaceShipsOnGrid(Grid grid)
    {
        // Place two destroyers and one battleship on the grid.
        PlaceShipClass(Ship.Destroyer, 2, grid);
        PlaceShipClass(Ship.Battleship, 1, grid);

        // Place n ships of the given class on the grid.
        void PlaceShipClass(Ship ship, int n, Grid g)
        {
            var random = new Random();

            int shipsPlaced = 0;

            // Keep placing ships at random until locations are found that don't overlap.
            while (shipsPlaced < n)
            {
                var x = random.Next(g.Width);
                var y = random.Next(g.Height);
                var orientation = random.Next(2) == 0 ? Orientation.Horizontal : Orientation.Vertical;
                if (g.TryPlaceShip(ship, x, y, orientation))
                {
                    // Ship was placed successfully.
                    shipsPlaced++;
                }
            }
        }
    }

    /// <summary>
    /// Runs the game loop and shows the map for player 1.
    /// </summary>
    public void Run()
    {
        // Keep taking turns until one player has sunk all of the other player's ships.
        while (grid1.HasFloatingShips && grid2.HasFloatingShips)
        {
            if (currentPlayer == Player.Player1)
            {
                displayMaps(grid1, grid2);
                TakeTurn(grid2, GetPlayer1Input);
            }
            else
            {
                TakeTurn(grid1, GetPlayer2Input);
            }

            // Advance to the next player.
            currentPlayer = currentPlayer == Player.Player1 ? Player.Player2 : Player.Player1;
        }

        // Display the final map.
        displayMaps(grid1, grid2);

        // Input the target coordinates and take a shot on the opponent's map.
        void TakeTurn(Grid opponentGrid, CoordinateInput getInput)
        {
            var (x, y) = getInput(opponentGrid.Width, opponentGrid.Height);
            opponentGrid.TakeShot(x, y);
        }
    }
}