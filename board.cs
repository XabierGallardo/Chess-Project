// Creating an array of 64 numbers as de computer representation of the chess board
public static class Board {
    
    public static int[] Square;

    public static Board () {
        Square = new int[64];

        // Examples on how to display the pieces on the board
        Square[0] = Piece.White | Piece.Bishop;
        Square[63] = Piece.Black | Piece.Queen;
        Square[7] = Piece.Black | Piece.Knight;
    }
}

//  FEN Notation | Forsyth-Edwards Notation
//  7k/3N2qp/b5r1/2p1Q1N1/Pp4PK/7P/1P3p2/6r1 w--7 4

public const string startFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"; //Initial positions

// To translate FEN Strings to our format
public static void LoadPositionFromFen (string fen) {
    var pieceTypeFromSymbol = new Dictionary <char, int> () {
        ['k'] = Piece.King, ['p'] = Piece.Pawn, ['n'] = Piece.Knight,
        ['b'] = Piece.Bishop, ['r'] = Piece.Rook, ['q'] = Piece.Queen
    };

    string fenBoard = fen.Split (' ')[0];
    int file = 0, rank = 7;

    foreach (char symbol in fenBoard) {
        if (symbol == '/') {
            file = 0;
            rank--;
        } else {
            if (char.isDigit (symbol)) {
                file += (int) char.GetNumericValue (symbol)
            } else {
                int pieceColour = (char.isUpper (symbol)) ? Piece.White : Piece.Black;
                int pieceType = pieceTypeFromSymbol[char.ToLower (symbol)];
                squares[rank * 8 + file] = pieceType | pieceColour;
                file++
            }
        }
    }
}

// Creating chess board
void CreateGraphicalBoard () {

    // Loops on x & y axis, named files & ranks
    for (int file = 0; file < 8; file ++) {
        for(int rank = 0; rank < 8; rank ++) {

            // Check if a square should be black or white
            bool isLightSquare = (file + rank) % 2 != 0;

            var squareColour = (isLightSquare) ? lightCol : darkCol;
            var position = new Vector2 (-3.5f + file, -3.5f + rank);

            DrawSquare(squareColour, position);
        }
    }
}