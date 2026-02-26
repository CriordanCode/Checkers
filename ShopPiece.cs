namespace Checkers;





//Inheritance class to allow for pieces to fall under the purchaseable category but have different piece types still
public class ShopPiece : Piece
{
    public int Cost { get; set;}
    //Differentiate the shop pieces by name of what they are
    public String Name { get; set;}

    //Include their symbol that they appear on the board with
    public char Symbol { get; set;}


    public ShopPiece(int price, string nameIn)
    {
        Cost = price;
        Name = nameIn;

    }

    public ShopPiece()
    {
        Cost = 0;
        Name = "N/A";
        Symbol = '?';
    }

}