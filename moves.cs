public static readonly int[] DirectionOffsets = { 8, -8, -1, 1, 7, -7, 9, -9 };
public static readonly int[][] NumSquaresToEdge;

/* When the program starts up it will calculate the number of squares to the edge of the board
Starting from each different square and going in every direction */
static PrecomputedMoveData () {
	for( int file = 0; file < 8; file ++) {
		for( int rank = 0; rank < 8; rank ++) {

			int numNorth = 7 - rank;
			int numSouth = rank;
			int numWest = file;
			int numEast = 7 - file;

			int squareIndex = rank * 8 + file;

			numSquaresToEdge[squareIndex] = {
				numNorth,
				numSouth,
				numWest;
				numEast,
				Min (numNorth, numWest),
				Min (numSouth, numEast),
				Min (numNorth, numEast),
				Min (numSouth, numWest),
			};
		}
	}
}



//Defining a structure for holding a move that will record the target square of the piece we want to move
public struct Move {
	public readonly int StartSquare;
	public readonly int TargetSquare;

	List <Move> moves;

	public List<Move> GenerateMoves () {

		moves = new List<Move> ();

		//Looping over all 64 squares and see what piece is on each square
		for(int startSquare = 0; startSquare < 64l startSquare ++) {

			int piece = Board.Square[startSquare];

			//We're only interested in the piece if it's the right colour for whoever's turn it is to move
			if( Piece.IsColour (piece, Board.ColourToMove)) {
				if(Piece.isSlidingPiece(piece)) {
					GenerateSlidingMoves (startSquare, piece);
				}
			}
		}
		return moves;
	}
}



/* In the sliding piece function, let's loop over 8 different directions
for each direction, we'll also have a loop for the number of squares that exist in that direction, up to the edge of the board */
void GenerateSlidingMoves (int startSquare, int piece) {
	int startDirIndex = (Piece.IsType (piece, Piece.Bishop)) ? 4 : 0;
	int endDirIndex = (Piece.IsType (piece, Piece.Rook)) ? 4 : 8;

	for(int directionIndex = startDirIndex; directionIndex < enDirIndex; directionIndex ++) {
		for (int n = 0; NumSquaresToEdge[startSquare][directionIndex]; n++) {
			int targetSquare = startSquare + DirectionOffsets[directionIndex] * (n + 1);
			int pieceOnTargetSquare = Board.Square[targetSquare];

			//Blocked by friendly piece, so can't move any further in this direction
			if(Piece.IsColour (pieceOnTargetSquare, friendlyColour)) {
				break;
			}

			move.Add (new Move (startSquare, targetSquare));

			//Can't move any further in this direction after capturing opponent's piece
			if (Piece.IsColour (pieceOnTargetSquare, opponentColour)) {
				break;
			}
		}
	}
}



/*	Idea 1 - Filtering illegal moves

//Take each pseudo-legal move, play it on the board and then look at all the oponents responses
//If any of those responses is a capture of our king, our last move will be illegal
List<Move> GenerateLegalMoves () {
	List<Move> pseudoLegalMoves = GenerateMoves ();
	List<Move> legalMoves = new List<Move> ();

	foreach (Move moveToVerify in pseudoLegalMoves) {
		Board.MakeMove (moveToVerify);
		List<Move> opponentResponses = GenerateMoves ();

		if (opponentResponses.Any (response => response.TargetSquare == myKingSquare)) {
			//The opponent has captured my king, therefore, my last move must have been illegal
		} else {
			legalMoves.Add (moveToVerify);
		}

		Board.UnmakeMove (moveToVerify);
	}
	return legalMoves;
}
*/



/* Idea 2 - Detecting if the king is in check
Instead of not keeping track of all the squares that the opponent attacks, so we can easily detect if the king is in check 
Get all the legal moves and 1 by 1 makes them on the board
Then recursively calls itself so that for each move, it makes each of the opponents responses and so on
*/
int MoveGenerationTest (int depth) {

	//Count the number of positions that can be reached after a certain number of moves
	if(depth == 0) {
		return 1;
	}

	List<Move> moves = moveGenerator.GenerateMoves ();
	int numPositions = 0;

	foreach (Move move in moves) {
		Board.MakeMove (move);
		numPositions += MoveGenerationTest (depth -1);
		Board.UnmakeMove (move);
	}
	return numPositions;
}



//Evaluation function

const int pawnValue = 100;
const int knightValue = 300;
const int bishopValue = 300;
const int rookValue = 500;
const int queenValue = 900;

/* Subtract the one from the other to end up with a value that is 
zero if position is equal
positive if the sides who's turn it is to move is doing better
negative if the other side is doing better */

public static int Evaluate () {
	int whiteEval = CountMaterial (Board.WhiteIndex);
	int blackEval = CountMaterial (Board.BlackIndex);

	int evaluation = whiteEval - blackEval;
	return evaluation * perspective;
}

//Add up the value of each side's pieces
static int CountMaterial (int colourIndex) {
	int material = 0;
	material += Board.pawns[colourIndex].Count * pawnValue;
	material += Board.knights[colourIndex].Count * knightValue;
	material += Board.bishops[colourIndex].Count * bishopValue;
	material += Board.rooks[colourIndex].Count * rookValue;
	material += Board.queens[colourIndex].Count * queenValue;
	return material;
}


/* Idea 1 - Search function */
//Instead of counting the number of positions after many moves, it'll evaluate those end positions
/*
int Search (int depth) {

	if (depth == 0) {
		return Evaluate ();
	}

	List<Move> moves = moveGenerator.GenerateMoves ();
	//If there are no legal moves available (checkmate) we can return engative infinity or stalemate (equals 0)
	if (moves.Count == 0) {
		if (Board.PlayerInCheck ()) {
			return negativeInfinity;
		}
		return 0;
	}

	//Keeping track of the best evaluation
	int bestEvaluation = negativeInfinity;

	foreach(Move move in moves) {
		Board.MakeMove (move);

		//A position that's good for our opponent is bad for us and vice versa
		int evaluation = -Search (depth - 1);
		bestEvaluation = Max (evaluation, bestEvaluation);
		Board.UnmakeMove (move);
	}

	return bestEvaluation;
}
*/


/* Idea 2 - Optimization using alpha, beta, pruning https://en.wikipedia.org/wiki/Alpha%E2%80%93beta_pruning

This optimization gives the exact same result as a pure search, but faster
how much faster depends on the order of the moves,
because if by some misfortune they happen to be ordered from worst to best, we can't prune anything at all
esentially, the more good moves are searched early on, the more branches will be pruned and the faster it will be
*/
int Search (int depth, intt alpha, int beta) {

	if (depth == 0) {
		return Evaluate ();
	}

	List<Move> moves = moveGenerator.GenerateMoves ();
	//If there are no legal moves available (checkmate) we can return engative infinity or stalemate (equals 0)
	if (moves.Count == 0) {
		if (Board.PlayerInCheck ()) {
			return negativeInfinity;
		}
		return 0;
	}

	foreach(Move move in moves) {
		Board.MakeMove (move);

		//A position that's good for our opponent is bad for us and vice versa
		int evaluation = -Search (depth - 1, -beta, -alpha);
		Board.UnmakeMove (move);


		if (evaluation >= beta) {
			//Move was too good, opponent will avoid this position
			return beta; //*Snip*
		}
		alpha = Max (alpha, evaluation);
	}
	return alpha;
}

/* We don't know in advance which moves are good, but we can make some guesses */
public void OrderMoves (List<Move> moves) {
	foreach (Move move in moves) {
		int moveScoreGuess = 0;
		int movePieceType = Piece.PieceType (Board.Square[move.StartSquare]);
		int capturePieceType = Piece.PieceType (Board.Square[move.TargetSquare]);

		//Prioritise capturing opponent's most valuable pieces with our least valuable pieces
		if (capturePieceType != Piece.None) {
			moveScoreGuess = 10* GetPieceValue (capturePieceType) - GetPieceValue(movePieceType);
		}

		//Promoting a pawn is usually a great idea
		if(move.Flag == MoveFlags.Promotion) {
			moveScoreGuess += GetPieceValue (move.PromotionPieceType);
		}

		//Penalize moving our piece to a square attacked by an opponent pawn
		if (Board.OpponentPawnAttackMap. ContainsSquare (move.TargetSquare)) {
			moveScoreGuess -= GetPieceValue (movePieceType);
		}
	}
}