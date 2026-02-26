namespace Checkers;

//Separate class for move
//This contains the piece of which it is moving variable with get and set methods,
//the location vector To with get and set methods to define where the move is going
//And the piece to capture variable if with get and set methods if the move is capturing
//a piece.
//The constructor include also allows for all of the parameters to be set as well as
//allowing for pieceToCapture to be null if there isn't a piece being captured.
public class Move
{
	public Piece PieceToMove { get; set; }

	public (int X, int Y) To { get; set; }

	public Piece? PieceToCapture { get; set; }

	public Move(Piece pieceToMove, (int X, int Y) to, Piece? pieceToCapture = null)
	{
		PieceToMove = pieceToMove;
		To = to;
		PieceToCapture = pieceToCapture;
	}
}
