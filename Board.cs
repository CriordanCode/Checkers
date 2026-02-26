using System.Drawing;

namespace Checkers;

public class Board
{
	//Get method for list of pieces within a board
	public List<Piece> Pieces { get; }

	//Get and Set methods for agressor pieces on the board
	public Piece? Aggressor { get; set; }

	public Piece? this[int x, int y] =>
		Pieces.FirstOrDefault(piece => piece.X == x && piece.Y == y);

	//List of traps on the board
	public List<Trap> Traps { get; }

	//Constructor for the board
	public Board()
	{
		Aggressor = null;
		Pieces = new List<Piece>
			{
				new() { NotationPosition ="A3", Color = Black},
				new() { NotationPosition ="A1", Color = Black},
				new() { NotationPosition ="B2", Color = Black},
				new() { NotationPosition ="C3", Color = Black},
				new() { NotationPosition ="C1", Color = Black},
				new() { NotationPosition ="D2", Color = Black},
				new() { NotationPosition ="E3", Color = Black},
				new() { NotationPosition ="E1", Color = Black},
				new() { NotationPosition ="F2", Color = Black},
				new() { NotationPosition ="G3", Color = Black},
				new() { NotationPosition ="G1", Color = Black},
				new() { NotationPosition ="H2", Color = Black},

				new() { NotationPosition ="A7", Color = White},
				new() { NotationPosition ="B8", Color = White},
				new() { NotationPosition ="B6", Color = White},
				new() { NotationPosition ="C7", Color = White},
				new() { NotationPosition ="D8", Color = White},
				new() { NotationPosition ="D6", Color = White},
				new() { NotationPosition ="E7", Color = White},
				new() { NotationPosition ="F8", Color = White},
				new() { NotationPosition ="F6", Color = White},
				new() { NotationPosition ="G7", Color = White},
				new() { NotationPosition ="H8", Color = White},
				new() { NotationPosition ="H6", Color = White}
			};
			Traps = new List<Trap>();
	}

	//String method to get position in the correct notation from 2 integers
	public static string ToPositionNotationString(int x, int y)
	{
		//Check if the position is valid before trying to conver it
		if (!IsValidPosition(x, y)) throw new ArgumentException("Not a valid position!");
		return $"{(char)('A' + x)}{y + 1}";
	}

	//Return the int pos from a string
	public static (int X, int Y) ParsePositionNotation(string notation)
	{
		//Check to make sure that notation isn't a null string
		if (notation is null) throw new ArgumentNullException(nameof(notation));
		//Convert notation to uppercase if it isn't already
		notation = notation.Trim().ToUpper();
		//Check that the notation string is a proper input
		if (notation.Length is not 2 ||
			notation[0] < 'A' || 'H' < notation[0] ||
			notation[1] < '1' || '8' < notation[1])
			throw new FormatException($@"{nameof(notation)} ""{notation}"" is not valid");
		//Convert notation char's to int's for position
		return (notation[0] - 'A', notation[1] - '1');
	}

	//Basick checker to make sure that they are a valid position and returns a bool if they are or not
	public static bool IsValidPosition(int x, int y) =>
		0 <= x && x < 8 &&
		0 <= y && y < 8;

	//Returns the pair of pieces that are teh closest to each other
	public (Piece A, Piece B) GetClosestRivalPieces(PieceColor priorityColor)
	{
		//Start by setting the min distance to the max value of a double
		double minDistanceSquared = double.MaxValue;
		//Create a tuple for the closest rivals
		(Piece A, Piece B) closestRivals = (null!, null!);
		foreach (Piece a in Pieces.Where(piece => piece.Color == priorityColor))
		{
			foreach (Piece b in Pieces.Where(piece => piece.Color != priorityColor))
			{
				//Get the distance from piece a and b's location
				(int X, int Y) vector = (a.X - b.X, a.Y - b.Y);
				//Add the squares together to get the total distance away from each other
				double distanceSquared = vector.X * vector.X + vector.Y * vector.Y;
				//Check the distance from the min distance
				if (distanceSquared < minDistanceSquared)
				{
					//When its smaller than the min distance set it to the min distance and the closest rivals,
					//since it starts as a max value it will at least set it to a new pair as long as their is a possible pair
					minDistanceSquared = distanceSquared;
					closestRivals = (a, b);
				}
			}
		}
		return closestRivals;
	}

	//Returns a list of possible moves for the specified color piece
	public List<Move> GetPossibleMoves(PieceColor color)
	{
		List<Move> moves = new();
		//Check that the person taking a move is not null
		if (Aggressor is not null)
		{
			//Check to make sure that the aggressor color is the same color specified, if not throw an exceptioni
			if (Aggressor.Color != color)
			{
				throw new Exception($"{nameof(Aggressor)} is not null && {nameof(Aggressor)}.{nameof(Aggressor.Color)} != {nameof(color)}");
			}
			moves.AddRange(GetPossibleMoves(Aggressor).Where(move => move.PieceToCapture is not null));
		}
		else
		{
			//Add a move for each piece that the piece matches the color
			foreach (Piece piece in Pieces.Where(piece => piece.Color == color))
			{
				moves.AddRange(GetPossibleMoves(piece));
			}
		}
		//Returns any move that satisfies the condition
		return moves.Any(move => move.PieceToCapture is not null)
			? moves.Where(move => move.PieceToCapture is not null).ToList()
			: moves;
	}

	//Returns a list of possible moves from a specific piece
	public List<Move> GetPossibleMoves(Piece piece)
	{
		List<Move> moves = new();
		//Check all diagonal movements from the piece
		ValidateDiagonalMove(-1, -1);
		ValidateDiagonalMove(-1,  1);
		ValidateDiagonalMove( 1, -1);
		ValidateDiagonalMove( 1,  1);
		//Return moves when its not null
		return moves.Any(move => move.PieceToCapture is not null)
			? moves.Where(move => move.PieceToCapture is not null).ToList()
			: moves;

		//Check that diagonal move is proper
		void ValidateDiagonalMove(int dx, int dy)
		{
			//If the piece is not promoted and black and the delta y is negative close;
			if (!piece.Promoted && piece.Color is Black && dy is -1) return;
			//If the piece is not promoted and white and the delta y is positive close;
			if (!piece.Promoted && piece.Color is White && dy is 1) return;
			//Set target as the piece + the difference for x and y values;
			(int X, int Y) target = (piece.X + dx, piece.Y + dy);
			//If that targtet is not a valid position return;
			if (!IsValidPosition(target.X, target.Y)) return;
			PieceColor? targetColor = this[target.X, target.Y]?.Color;
			//If target color is null check that valid position is false otherwise add a new move (null color means no piece in that position)
			if (targetColor is null)
			{
				if (!IsValidPosition(target.X, target.Y)) return;
				Move newMove = new(piece, target);
				moves.Add(newMove);
			}
			//If the target color is not the same color this indicates a potential jump
			//Check if thats a valid position 2 spaces away and if it is null create a new
			//attack move and add that to the list of moves
			else if (targetColor != piece.Color)
			{
				(int X, int Y) jump = (piece.X + 2 * dx, piece.Y + 2 * dy);
				if (!IsValidPosition(jump.X, jump.Y)) return;
				PieceColor? jumpColor = this[jump.X, jump.Y]?.Color;
				if (jumpColor is not null) return;
				Move attack = new(piece, jump, this[target.X, target.Y]);
				moves.Add(attack);
			}
		}
	}

	/// <summary>Returns a <see cref="Move"/> if <paramref name="from"/>-&gt;<paramref name="to"/> is valid or null if not.</summary>
	public Move? ValidateMove(PieceColor color, (int X, int Y) from, (int X, int Y) to)
	{
		Piece? piece = this[from.X, from.Y];
		if (piece is null)
		{
			return null;
		}
		foreach (Move move in GetPossibleMoves(color))
		{
			if ((move.PieceToMove.X, move.PieceToMove.Y) == from && move.To == to)
			{
				return move;
			}
		}
		return null;
	}

	//Check to see if the move is towards the piece by comparing the distance of the piece to move and the move
	public static bool IsTowards(Move move, Piece piece)
	{
		(int Dx, int Dy) a = (move.PieceToMove.X - piece.X, move.PieceToMove.Y - piece.Y);
		int a_distanceSquared = a.Dx * a.Dx + a.Dy * a.Dy;
		(int Dx, int Dy) b = (move.To.X - piece.X, move.To.Y - piece.Y);
		int b_distanceSquared = b.Dx * b.Dx + b.Dy * b.Dy;
		return b_distanceSquared < a_distanceSquared;
	}

	//Brief add trap for later use, in the instance that you want to purchase a trap for placement
	public void AddTrap(int xTrap, int yTrap)
	{
		Traps.Add(new Trap(xTrap, yTrap, Neutral));
	}

	//Boolean meethod to check if a spot is empty (ex. looking for an available trap placement)
	public bool IsEmptySpot(int X, int Y)
	{

		//Look at each piece in the list and if it is equal 
		//to the parameter location provided return false
		foreach(Piece piece in this.Pieces)
		{
			if(piece.X == X && piece.Y == Y)
			{
				return false;
			}
		}
		return true;
	}



	
	public void CreateTrapRand()
	{
		bool trapValid = false;
		while (!trapValid)
		{
			//New location for trap (checks if its an empty spot);
			Trap current = new Trap(Random.Shared.Next(0,7), Random.Shared.Next(0,7), Neutral);
			trapValid = IsEmptySpot(current.X, current.Y);
			if(trapValid){
				//Add it to the list of traps and pieces to be rendered
				Traps.Add(current);
				Pieces.Add(current);
			}
		}
		
	}

	public void CreateTrapBlack()
	{
		bool trapValid = false;
		while (!trapValid)
		{
			Trap current = new Trap(Random.Shared.Next(0,7), Random.Shared.Next(4,7), Neutral);
			trapValid = IsEmptySpot(current.X, current.Y);
			if (trapValid)
			{
				Traps.Add(current);
				Pieces.Add(current);
			}
		}
	}

	public void CreateTrapWhite()
	{
		bool trapValid = false;
		while (!trapValid)
		{
			Trap current = new Trap(Random.Shared.Next(0,7), Random.Shared.Next(0,4), Neutral);
			trapValid = IsEmptySpot(current.X, current.Y);
			if (trapValid)
			{
				Traps.Add(current);
				Pieces.Add(current);
			}
		}
	}
}
