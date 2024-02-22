using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{

    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = {0, 100, 300, 300, 500, 900, 10000};

    // PieceSquare values use the values that are described at
    // https://www.chessprogramming.org/Simplified_Evaluation_Function
    int[,] PieceSquareValues = new int[6, 64]{
    { // Pawn
         0,  0,  0,  0,  0,  0,  0,  0,
        50, 50, 50, 50, 50, 50, 50, 50,
        10, 10, 20, 30, 30, 20, 10, 10,
         5,  5, 10, 25, 25, 10,  5,  5,
         0,  0,  0, 20, 20,  0,  0,  0,
         5, -5,-10,  0,  0,-10, -5,  5,
         5, 10, 10,-20,-20, 10, 10,  5,
         0,  0,  0,  0,  0,  0,  0,  0
    },
    { // Knight
        -50,-40,-30,-30,-30,-30,-40,-50,
        -40,-20,  0,  0,  0,  0,-20,-40,
        -30,  0, 10, 15, 15, 10,  0,-30,
        -30,  5, 15, 20, 20, 15,  5,-30,
        -30,  0, 15, 20, 20, 15,  0,-30,
        -30,  5, 10, 15, 15, 10,  5,-30,
        -40,-20,  0,  5,  5,  0,-20,-40,
        -50,-40,-30,-30,-30,-30,-40,-50,
    },
    { // Bishop
        -20,-10,-10,-10,-10,-10,-10,-20,
        -10,  0,  0,  0,  0,  0,  0,-10,
        -10,  0,  5, 10, 10,  5,  0,-10,
        -10,  5,  5, 10, 10,  5,  5,-10,
        -10,  0, 10, 10, 10, 10,  0,-10,
        -10, 10, 10, 10, 10, 10, 10,-10,
        -10,  5,  0,  0,  0,  0,  5,-10,
        -20,-10,-10,-10,-10,-10,-10,-20,
    },
    { // Rook
         0,  0,  0,  0,  0,  0,  0,  0,
         5, 10, 10, 10, 10, 10, 10,  5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
         0,  0,  0,  5,  5,  0,  0,  0
    },
    { // Queen
        -20,-10,-10, -5, -5,-10,-10,-20,
        -10,  0,  0,  0,  0,  0,  0,-10,
        -10,  0,  5,  5,  5,  5,  0,-10,
         -5,  0,  5,  5,  5,  5,  0, -5,
          0,  0,  5,  5,  5,  5,  0, -5,
        -10,  5,  5,  5,  5,  5,  0,-10,
        -10,  0,  5,  0,  0,  0,  0,-10,
        -20,-10,-10, -5, -5,-10,-10,-20
    },
    { // King middle game
        -50,-40,-30,-20,-20,-30,-40,-50,
        -30,-20,-10,  0,  0,-10,-20,-30,
        -30,-10, 20, 30, 30, 20,-10,-30,
        -30,-10, 30, 40, 40, 30,-10,-30,
        -30,-10, 30, 40, 40, 30,-10,-30,
        -30,-10, 20, 30, 30, 20,-10,-30,
        -30,-30,  0,  0,  0,  0,-30,-30,
        -50,-30,-30,-30,-30,-30,-30,-50
    }};

    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();

        // Pick a random move to play if nothing better is found
        Random rng = new();
        Move bestMove = moves[rng.Next(moves.Length)];

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

            if (p.IsWhite)
                whitePoints += pieceValues[(int)p.PieceType] + PieceSquareValues[(int)p.PieceType - 1, i];
            else if (p.PieceType != 0)
                blackPoints += pieceValues[(int)p.PieceType] + PieceSquareValues[(int)p.PieceType - 1, 63 - i];
        }

        int score = (whitePoints - blackPoints) * who2Move;

        return score;
    }

}