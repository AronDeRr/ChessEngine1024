using ChessChallenge.API;

public class MyBot : IChessBot
{

    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = {0, 100, 300, 300, 500, 900, 10000};

    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        Move bestMove = moves[0];
        int bestScore = -10000;
        int who2Move = board.IsWhiteToMove? 1: -1;

        foreach (Move move in moves)
        {
            board.MakeMove(move);
            int score = Evaluate(board, who2Move);
            board.UndoMove(move);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }
        return bestMove;
    }

    private int Evaluate(Board board, int who2Move)
    {
        if (board.IsInCheckmate())
        {
            return int.MaxValue;
        }
        else if (board.IsDraw())
        {
            return 0;
        }

        int whitePoints = 0;
        int blackPoints = 0;
        for (int i = 0; i <= 63; i++)
        {
            Piece p = board.GetPiece(new Square(i));

            if (p.IsWhite) whitePoints += pieceValues[(int)p.PieceType];
            else blackPoints += pieceValues[(int)p.PieceType];
        }

        int score = (whitePoints - blackPoints) * who2Move;

        return score;
    }

}