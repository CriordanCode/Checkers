Exception? exception = null;

Encoding encoding = Console.OutputEncoding;

bool gameCloseRequested = false;
bool shopToggle = true;

try
{
	Console.OutputEncoding = Encoding.UTF8;
	//New game running showintro screen option
	Game game = ShowIntroScreenAndGetOption();
	Console.Clear();
	// //Run tha game loop on the previously created game
	// RunGameLoop(game);
	// //Render the game state in the console
	// RenderGameState(game, promptPressKey: true);
	// Console.ReadKey(true);

	while (!gameCloseRequested)
	{
		RunGameLoop(game);
		RenderGameState(game, promptPressKey: true);
		if(Console.ReadKey(true).Key == ConsoleKey.Escape)
		{
			gameCloseRequested = true;
		}
		game = GameCleanUp(game);
	}

}
catch (Exception e)
{
	exception = e;
	throw;
}
finally
{
	Console.OutputEncoding = encoding;
	Console.CursorVisible = true;
	Console.Clear();
	Console.WriteLine(exception?.ToString() ?? "Checkers was closed.");
}

Game ShowIntroScreenAndGetOption()
{
	//Write the intro to console
	Console.Clear();
	Console.WriteLine();
	Console.WriteLine("  Checkers");
	Console.WriteLine();
	Console.WriteLine("  Checkers is played on an 8x8 board between two sides commonly known as black");
	Console.WriteLine("  and white. The objective is simple - capture all your opponent's pieces. An");
	Console.WriteLine("  alternative way to win is to trap your opponent so that they have no valid");
	Console.WriteLine("  moves left.");
	Console.WriteLine();
	Console.WriteLine("  Black starts first and players take it in turns to move their pieces forward");
	Console.WriteLine("  across the board diagonally. Should a piece reach the other side of the board");
	Console.WriteLine("  the piece becomes a king and can then move diagonally backwards as well as");
	Console.WriteLine("  forwards.");
	Console.WriteLine();
	Console.WriteLine("  Pieces are captured by jumping over them diagonally. More than one enemy piece");
	Console.WriteLine("  can be captured in the same turn by the same piece. If you can capture a piece");
	Console.WriteLine("  you must capture a piece.");
	Console.WriteLine();
	Console.WriteLine("  Moves are selected with the arrow keys. Use the [enter] button to select the");
	Console.WriteLine("  from and to squares. Invalid moves are ignored.");
	Console.WriteLine();
	Console.WriteLine("  Press a number key to choose number of human players:");
	Console.WriteLine("    [0] Black (computer) vs White (computer)");
	Console.WriteLine("    [1] Black (human) vs White (computer)");
	Console.Write("    [2] Black (human) vs White (human)");

	//Check what the player inputs and set the amount of players for the game of checkers.
	int? humanPlayerCount = null;
	while (humanPlayerCount is null)
	{
		Console.CursorVisible = false;
		//Basic switch case for the cursor, inside the while loop will continue until valid selection is made
		switch (Console.ReadKey(true).Key)
		{
			case ConsoleKey.D0 or ConsoleKey.NumPad0: humanPlayerCount = 0; break;
			case ConsoleKey.D1 or ConsoleKey.NumPad1: humanPlayerCount = 1; break;
			case ConsoleKey.D2 or ConsoleKey.NumPad2: humanPlayerCount = 2; break;
		}
	}
	return new Game(humanPlayerCount.Value);
}

//Method to run the actual game
void RunGameLoop(Game game)
{
	//Continue the game until a winner is set
	while (game.Winner is null)
	{
		//Set the current player from the list of players in the game
		Player currentPlayer = game.Players.First(player => player.Color == game.Turn);
		//Code for if the turn is run by a human player
		if (currentPlayer.IsHuman)
		{
			while (game.Turn == currentPlayer.Color)
			{
				//Initialize selection tuple
				(int X, int Y)? selectionStart = null;
				//Initialize tuple from the aggressor
				(int X, int Y)? from = game.Board.Aggressor is not null ? (game.Board.Aggressor.X, game.Board.Aggressor.Y) : null;
				//Get a list of possible moves
				List<Move> moves = game.Board.GetPossibleMoves(game.Turn);
				//If there is only one move possible set it to be that move
				if (moves.Select(move => move.PieceToMove).Distinct().Count() is 1)
				{
					Move must = moves.First();
					from = (must.PieceToMove.X, must.PieceToMove.Y);
					selectionStart = must.To;
				}
				//Wait for the human to make a selection of where they are moving from
				while (from is null)
				{
					from = HumanMoveSelection(game);
					selectionStart = from;
				}
				//Now they make a selection of where they are moving to
				(int X, int Y)? to = HumanMoveSelection(game, selectionStart: selectionStart, from: from);
				//Create a new piece
				Piece? piece = null;
				//Set the piece equal to the piece that was at the location of from
				piece = game.Board[from.Value.X, from.Value.Y];
				//If there is no piece or the piece is not the right color seet both to null
				if (piece is null || piece.Color != game.Turn)
				{
					from = null;
					to = null;
				}
				//If from and to are not null, attempt to complete the move
				if (from is not null && to is not null)
				{
					//Create a new move and validate that this is a valid game move
					Move? move = game.Board.ValidateMove(game.Turn, from.Value, to.Value);
					//If the move is not null & there is a board agressor or moving the piece introduces a board agressor then preform the move
					if (move is not null &&
						(game.Board.Aggressor is null || move.PieceToMove == game.Board.Aggressor))
					{
						game.PerformMove(move);
					}
				}
			}
		}
		//If player is not human (ie the player is a computer do this)
		else
		{
			//Get a list of the possible moves from the current board state for the current players turn
			List<Move> moves = game.Board.GetPossibleMoves(game.Turn);
			//List of capture moves that are possible
			List<Move> captures = moves.Where(move => move.PieceToCapture is not null).ToList();
			//If there is a possible capture move, perform one of those at random
			if (captures.Count > 0)
			{
				game.PerformMove(captures[Random.Shared.Next(captures.Count)]);
			}
			//If there are no pieces that is promoted then look for the closest rival pieces on the 
			//board if so attempt to make that move otherwise make a random move
			else if(!game.Board.Pieces.Any(piece => piece.Color == game.Turn && !piece.Promoted))
			{
				var (a, b) = game.Board.GetClosestRivalPieces(game.Turn);
				Move? priorityMove = moves.FirstOrDefault(move => move.PieceToMove == a && Board.IsTowards(move, b));
				game.PerformMove(priorityMove ?? moves[Random.Shared.Next(moves.Count)]);
			}
			//If all else fails perform a random move off of the list of available moves
			else
			{
				game.PerformMove(moves[Random.Shared.Next(moves.Count)]);
			}
		}
		//After a move is completed rerender the gamestate
		RenderGameState(game, playerMoved: currentPlayer, promptPressKey: true);
		//Wait for a key press
		Console.ReadKey(true);
	}
}

Game GameCleanUp(Game game)
{
	return new Game(game);
}

//Method to render gamestate
void RenderGameState(Game game, Player? playerMoved = null, (int X, int Y)? selection = null, (int X, int Y)? from = null, bool promptPressKey = false)
{
	//Console.Clear();
	//Constant char's for the various pieces of the game
	const char BlackPiece = '○';
	const char BlackKing  = '☺';
	const char WhitePiece = '◙';
	const char WhiteKing  = '☻';
	const char Vacant     = '·';

	//New Pieces to be added (first being a trap, final char tbd if 'X' isn't clear enough)
	const char Trap = 'X';

	//List of strings for easier modification line by line
	List<String> printOut = new List<String>();

	printOut.Add("");
	printOut.Add("  Checkers");
	printOut.Add("");
	printOut.Add($"    ╔═══════════════════╗                  ");
	printOut.Add($"  8 ║  {B(0, 7)} {B(1, 7)} {B(2, 7)} {B(3, 7)} {B(4, 7)} {B(5, 7)} {B(6, 7)} {B(7, 7)}  ║ {BlackPiece} = Black        ");
	printOut.Add($"  7 ║  {B(0, 6)} {B(1, 6)} {B(2, 6)} {B(3, 6)} {B(4, 6)} {B(5, 6)} {B(6, 6)} {B(7, 6)}  ║ {BlackKing} = Black King   ");
	printOut.Add($"  6 ║  {B(0, 5)} {B(1, 5)} {B(2, 5)} {B(3, 5)} {B(4, 5)} {B(5, 5)} {B(6, 5)} {B(7, 5)}  ║ {WhitePiece} = White        ");
	printOut.Add($"  5 ║  {B(0, 4)} {B(1, 4)} {B(2, 4)} {B(3, 4)} {B(4, 4)} {B(5, 4)} {B(6, 4)} {B(7, 4)}  ║ {WhiteKing} = White King   ");
	printOut.Add($"  4 ║  {B(0, 3)} {B(1, 3)} {B(2, 3)} {B(3, 3)} {B(4, 3)} {B(5, 3)} {B(6, 3)} {B(7, 3)}  ║ {Trap} = Trap Placed  ");
	printOut.Add($"  3 ║  {B(0, 2)} {B(1, 2)} {B(2, 2)} {B(3, 2)} {B(4, 2)} {B(5, 2)} {B(6, 2)} {B(7, 2)}  ║ Piece Score:     ");
	printOut.Add($"  2 ║  {B(0, 1)} {B(1, 1)} {B(2, 1)} {B(3, 1)} {B(4, 1)} {B(5, 1)} {B(6, 1)} {B(7, 1)}  ║ {game.TakenScore(White),2} x {WhitePiece}           ");
	printOut.Add($"  1 ║  {B(0, 0)} {B(1, 0)} {B(2, 0)} {B(3, 0)} {B(4, 0)} {B(5, 0)} {B(6, 0)} {B(7, 0)}  ║ {game.TakenScore(Black),2} x {BlackPiece}           ");
	printOut.Add($"    ╚═══════════════════╝");
	printOut.Add($"       A B C D E F G H");
	printOut.Add("");

	if(shopToggle){
		game.GameShop.RenderShop(printOut);
	} else
	{
		game.GameShop.ClearShop(printOut);
	}
	Console.CursorVisible = false;
	Console.SetCursorPosition(0, 0);
	//StringBuilder for the gameboard
	StringBuilder sb = new();
	// sb.AppendLine();
	// sb.AppendLine("  Checkers");
	// sb.AppendLine();
	// sb.AppendLine($"    ╔═══════════════════╗");
	// sb.AppendLine($"  8 ║  {B(0, 7)} {B(1, 7)} {B(2, 7)} {B(3, 7)} {B(4, 7)} {B(5, 7)} {B(6, 7)} {B(7, 7)}  ║ {BlackPiece} = Black");
	// sb.AppendLine($"  7 ║  {B(0, 6)} {B(1, 6)} {B(2, 6)} {B(3, 6)} {B(4, 6)} {B(5, 6)} {B(6, 6)} {B(7, 6)}  ║ {BlackKing} = Black King");
	// sb.AppendLine($"  6 ║  {B(0, 5)} {B(1, 5)} {B(2, 5)} {B(3, 5)} {B(4, 5)} {B(5, 5)} {B(6, 5)} {B(7, 5)}  ║ {WhitePiece} = White");
	// sb.AppendLine($"  5 ║  {B(0, 4)} {B(1, 4)} {B(2, 4)} {B(3, 4)} {B(4, 4)} {B(5, 4)} {B(6, 4)} {B(7, 4)}  ║ {WhiteKing} = White King");
	// sb.AppendLine($"  4 ║  {B(0, 3)} {B(1, 3)} {B(2, 3)} {B(3, 3)} {B(4, 3)} {B(5, 3)} {B(6, 3)} {B(7, 3)}  ║ {Trap} = Trap Placed");
	// sb.AppendLine($"  3 ║  {B(0, 2)} {B(1, 2)} {B(2, 2)} {B(3, 2)} {B(4, 2)} {B(5, 2)} {B(6, 2)} {B(7, 2)}  ║ Taken:");
	// sb.AppendLine($"  2 ║  {B(0, 1)} {B(1, 1)} {B(2, 1)} {B(3, 1)} {B(4, 1)} {B(5, 1)} {B(6, 1)} {B(7, 1)}  ║ {game.TakenCount(White),2} x {WhitePiece}");
	// sb.AppendLine($"  1 ║  {B(0, 0)} {B(1, 0)} {B(2, 0)} {B(3, 0)} {B(4, 0)} {B(5, 0)} {B(6, 0)} {B(7, 0)}  ║ {game.TakenCount(Black),2} x {BlackPiece}");
	// sb.AppendLine($"    ╚═══════════════════╝");
	// sb.AppendLine($"       A B C D E F G H");
	// sb.AppendLine();

	foreach(string renderString in printOut)
	{
		sb.AppendLine(renderString);
	}

	//Score Section
	sb.AppendLine(game.renderScore());
	//Replace selection space with a new character to indicate that
	if (selection is not null)
	{
		sb.Replace(" $ ", $"[{ToChar(game.Board[selection.Value.X, selection.Value.Y])}]");
	}
	//Replace the character in the fromm space with another char
	if (from is not null)
	{
		char fromChar = ToChar(game.Board[from.Value.X, from.Value.Y]);
		sb.Replace(" @ ", $"<{fromChar}>");
		sb.Replace("@ ",  $"{fromChar}>");
		sb.Replace(" @",  $"<{fromChar}");
	}
	//Piece color for the winner, whoever just moved, and the current turn
	PieceColor? wc = game.Winner;
	PieceColor? mc = playerMoved?.Color;
	PieceColor? tc = game.Turn;
	// Note: these strings need to match in length
	// so they overwrite each other.
	string w = $"  *** {wc} wins ***";
	string m = $"  {mc} moved       ";
	string t = $"  {tc}'s turn      ";
	//If there is a winner, or a player has not moved cases for append lines
	sb.AppendLine(
		game.Winner is not null ? w :
		playerMoved is not null ? m :
		t);
	sb.AppendLine("  Press K to toggle the Shop");
	if(shopToggle){
		sb.AppendLine("  Press P to purchase an item from the Shop");
	}
	string p = "  Press any key to continue...                    ";
	string s = "                                                  ";
	//If they are waiting for a keypress prompt them otherwise do not
	sb.AppendLine(promptPressKey ? p : s);
	
	
	
	
	
	
	//Write the stringbuilder to console
	Console.Write(sb);

	//Char B to set based on the board state
	char B(int x, int y) =>
		(x, y) == selection ? '$' :
		(x, y) == from ? '@' :
		ToChar(game.Board[x, y]);

	//How to change a piece position to the proper character and what to replace it with
	static char ToChar(Piece? piece) =>
		piece is null ? Vacant :
		(piece.Color, piece.Promoted) switch
		{
			(Black, false) => BlackPiece,
			(Black, true)  => BlackKing,
			(White, false) => WhitePiece,
			(White, true)  => WhiteKing,
			//Included the first special piece a trap
			(Neutral, false) => Trap,
			_ => throw new NotImplementedException(),
		};
}

//Helper method for the player to make a move
(int X, int Y)? HumanMoveSelection(Game game, (int X, int y)? selectionStart = null, (int X, int Y)? from = null)
{
	(int X, int Y) selection = selectionStart ?? (3, 3);
	while (true)
	{
		RenderGameState(game, selection: selection, from: from);
		//Change the selection based on keys and wait for the player to hit enter to confirm their selection of movement
		switch (Console.ReadKey(true).Key)
		{
			case ConsoleKey.DownArrow:  selection.Y = Math.Max(0, selection.Y - 1); break;
			case ConsoleKey.UpArrow:    selection.Y = Math.Min(7, selection.Y + 1); break;
			case ConsoleKey.LeftArrow:  selection.X = Math.Max(0, selection.X - 1); break;
			case ConsoleKey.RightArrow: selection.X = Math.Min(7, selection.X + 1); break;
			case ConsoleKey.Enter:      return selection;
			case ConsoleKey.Escape:     return null;
			case ConsoleKey.K:			shopToggle = !shopToggle; break;
			case ConsoleKey.P:			game.GameShop.ShopSelection(game); break;
		}
	}
}
