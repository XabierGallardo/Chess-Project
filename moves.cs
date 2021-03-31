public static readonly int[] DirectionOffsets = { 8, -8, -1, 1, 7, -7, 9, -9 };
public static readonly int[][] NumSquaresToEdge;

// When the program starts up it will calculate the number of squares to the edge of the board
//Starting from each different square and going in every direction
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



//In the sliding piece function, let's loop over 8 different directions
//for each direction, we'll also have a loop for the number of squares that exist in that direction, up to the edge of the board
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