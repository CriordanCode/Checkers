namespace Checkers;







//Shop class to handle the shop features
public class Shop
{
    //List of pieces that are able to be purchased
    public List<ShopPiece> Inventory { get;}

    //Simple constructor
    public Shop()
    {
        Inventory = new List<ShopPiece>();
    }

    //Basic method to add items to the shops inventory
    public void addShopItem(ShopPiece item)
    {
        Inventory.Add(item);
    }

    //Method to render the shop (takes in a string list so that I can modify the current board output
    //so that it appears to the right of the board in the console and not just below it)
    public void RenderShop(List<String> display)
    {
        display[3]  += "   ╔══════════════════════════════════════╗";
        display[4]  += "   ║                 Shop                 ║";
        display[5]  += "   ║══════════════════════════════════════║";
        display[6]  += "   ";
        display[7]  += "   ";
        display[8]  += "   ";
        display[9]  += "   ";
        display[10] += "   ";
        int itemInShop = 0;
        //Render the items within the shop
        while(itemInShop < 3)
        {
            if(Inventory.Count > itemInShop){
                RenderContents(Inventory[itemInShop],display);
                itemInShop++;
            } else
            {
                RenderEmpty(display);
                itemInShop++;
            }
        }
        display[6]  += "║";
        display[7]  += "║";
        display[8]  += "║";
        display[9]  += "║";
        display[10] += "║";
        display[11] += "   ╚══════════════════════════════════════╝";
    }

    //Method to create the items in the shop so they are all a standard size regardless of the item 
    //(this requires names to be less than 12 characters however)
    public void RenderContents(ShopPiece currentItem, List<String> display)
    {
        display[6] +=  "║            ";
        display[7] += $"║{ItemToStringRender(currentItem)}";
        display[8] += $"║    [{currentItem.Symbol}]     ";
        display[9] += $"║  Cost: {currentItem.Cost}   ";
        display[10] += "║            ";
    }

    //If there is nothing in the shops space it will display an empty box
    public void RenderEmpty(List<String> display)
    {
        display[6]  += "║            ";
        display[7]  += "║            ";
        display[8]  += "║            ";
        display[9]  += "║            ";
        display[10] += "║            ";
    }

    //Custom method to render out the item names in the proper format
    public String ItemToStringRender(ShopPiece currentItem)
    {
        string shopRender = "";
        int maxLength = 12 - currentItem.Name.Length;
        int iter = 0;
        while(iter < (maxLength/2))
        {
            shopRender += " ";
            iter++;
        }
        shopRender += currentItem.Name;
        while(shopRender.Length < 12)
        {
            shopRender += " ";
        }
        return shopRender;
    }

    //Method to clear out the shop if the player decides to toggle it off
    public void ClearShop(List<String> display)
    {
        for(int iter = 3; iter < 12; iter++)
        {
            display[iter]  += "                                           ";    
        }
    }

    //Method to handle the selection within the shop, allows for the user to
    //move through all of the options and settle on a selection;
    public void ShopSelection(Game game)
    {

        //List of positions of the center of items, hardcoded based on the way the game is
        //rendered in the console
        List<int> shopPos = new List<int>();
        shopPos.Add(52);
        shopPos.Add(65);
        shopPos.Add(78);
        int currX = 0;
        Console.CursorVisible = true;
        Console.SetCursorPosition(shopPos[currX], 8);
        bool selectionMade = false;
        while(!selectionMade){
            GetInput:
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.RightArrow  : currX++; break;
                case ConsoleKey.LeftArrow   : currX--; break;
                case ConsoleKey.Enter       : selectionMade = true; break;
                case ConsoleKey.Escape      : return;
                default : goto GetInput;
            }
            switch (currX)
            {
                case > 2: currX = 0; break;
                case < 0: currX = 2; break;
                default: break;
            }
            Console.SetCursorPosition(shopPos[currX], 8);
        }
        //When the selection is made call the method to activate the item
        PurchaseItem(currX, game);
    }

    //Method to handle item purchases
    public void PurchaseItem(int itemNum, Game game)
    {
        //Check to make sure the spot isn't empty and would cause a null pointer error
        if(Inventory.Count < itemNum)
        {
            return;
        }
        else
        {
            //If the game is on blacks turn
            if(game.Turn == game.Players[0].Color)
            {
                //If they have taken enough white pieces to buy the item they can do so
                if(game.TakenScore(White) >= Inventory[itemNum].Cost)
                {
                    //Add to the total of purchases they have made so that it accurately tracks if they
                    //have purchased more than pieces they've taken
                    game.Players[1].ShopPurchases += Inventory[itemNum].Cost;
                    //If the item purchases is a trap, create a random trap on the white side of the board
                    //Then remomve the item from the inventory
                    if (Inventory[itemNum].Name.Equals("Trap"))
                    {
                        game.Board.CreateTrapBlack();
                        Inventory.RemoveAt(itemNum);
                    }
                }
            } else
            {
                //If enough black pieces have been taken off the board
                if(game.TakenScore(Black) >= Inventory[itemNum].Cost)
                {
                    game.Players[0].ShopPurchases += Inventory[itemNum].Cost;
                    //If it is a trap, then place a trap on the black side of the board
                    if (Inventory[itemNum].Name.Equals("Trap"))
                    {
                        game.Board.CreateTrapWhite();
                        Inventory.RemoveAt(itemNum);
                    }
                }
            }
        }
    }

}