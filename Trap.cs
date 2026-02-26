namespace Checkers;



//Child class of piece called trap with additional functionality
//Child class allows it to be added to the list of pieces that the board
//renders on game updates
public class Trap : ShopPiece
{

    //Variable for cost, when shop is implemented players
    //will be able to purchase the trap for a price
    //public int Cost { get; }
    //Range variable allows for scalable traps/bombs that
    //can hit more pieces at once
    public int Range { get; }

    //To string method to get the position of a trap if needed
    public string TrapPosition
    {
        get => Board.ToPositionNotationString(X, Y);
		set => (X, Y) = Board.ParsePositionNotation(value);
    }

    
    //Constructor for the trap allowing for it to be coded to a team
    public Trap(int xPos, int yPos, PieceColor owner)
    {
        X = xPos;
        Y = yPos;
        Cost = 3;
        Color = owner;
        Range = 1;
        Name = "Trap";
        Symbol = 'X';
    }

    //***CURRENTLY UNUSED***
    //General constructor for a trap allowing it to belong to a third part
    //The netural faction that targets both players traps
    public Trap()
    {
        X = Random.Shared.Next(0,7);
        Y = Random.Shared.Next(0,7);
        Cost = 3;
        Color = Neutral;
        Range = 1;
        Name = "Trap";
        Symbol = 'X';
    }

    //***CURRENTLY UNUSED***
    //Random trap placement that allows for the placement of a trap randomly
    //While selecting a color for it
    public Trap(PieceColor owner)
    {
        X = Random.Shared.Next(0, 7);
        Y = Random.Shared.Next(0, 7);
        Cost = 3;
        Color = owner;
        Range = 1;
        Name = "Trap";
        Symbol = 'X';
    }
}


