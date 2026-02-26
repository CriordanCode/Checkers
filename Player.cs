namespace Checkers;

//Seperate class for player
//Includes a bool for if they are human, the color of the piece
//and includes get methods for both of those values.
//Also includes a constructor using both of those values to set them.
//Without including a set method these values are not readily changeable
//without creating a new player entirely.
public class Player
{
	public bool IsHuman { get; }
	public PieceColor Color { get; }

	public int Score{ get; set;}

	public int ShopPurchases {get; set;}
	public Player(bool isHuman, PieceColor color)
	{
		IsHuman = isHuman;
		Color = color;
		Score = 0;
		ShopPurchases = 0;
	}
}
