namespace Checkers;

//Class object for piece with get and set methods for two parameters
//X and Y variables for its location.
//Also includes a string notation position that allows for the piece
//to output its location based on the set notation in the board file
//Piece also includes two other variables, the piececolor of the piece
//And the bool for if the piece is promoted, which is mutable, while
//piece color is able to be gotten and set on initialization but not
//changed later.
public class Piece
{
	public int X { get; set; }

	public int Y { get; set; }

	public string NotationPosition
	{
		get => Board.ToPositionNotationString(X, Y);
		set => (X, Y) = Board.ParsePositionNotation(value);
	}

	public PieceColor Color { get; init; }

	public bool Promoted { get; set; }
}
