using ChessChallenge.API;
using System;
using System.Collections.Generic;

public class MyBot : IChessBot
{
    private const int CheckmateScore = 100000;
    private const int DrawScore = 0;

    public Move Think(Board board, Timer timer)
    {
        return Search(board, 3);
    }

    public static Move Search(Board board, int depth)  // FIXME: fix this shit. Chess Bots is going to make me crazy
    {
        Move[] legalMoves = board.GetLegalMoves();
        Move bestMove = legalMoves[0];
        int bestEval = EvalSearch(board, depth - 1, int.MinValue, int.MaxValue);

        foreach (Move move in legalMoves)
        {
            int eval = EvalSearch(board, depth - 1, int.MinValue, int.MaxValue);
            Console.WriteLine($"{move} => {eval}");
            if (eval > bestEval)
            {
                bestEval = eval;
                bestMove = move;
            }
        }

        Console.WriteLine("-----");
        Console.WriteLine($"{bestMove} chosen with eval {bestEval}");
        Console.WriteLine("=====");
        return bestMove;
    }

    public static int EvalSearch(Board board, int depth, int alpha, int beta)
    {
        // Leaf node - evaluate position
        if (depth == 0)
            return Evaluate(board);

        Move[] moves = board.GetLegalMoves();
        
        // Terminal node check (checkmate/stalemate)
        if (moves.Length == 0)
        {
            if (board.IsInCheck())
                return board.IsWhiteToMove ? -CheckmateScore : CheckmateScore;
            return DrawScore;
        }

        int bestEval = board.IsWhiteToMove ? int.MinValue : int.MaxValue;

        foreach (Move move in moves)
        {
            board.MakeMove(move);
            int eval = EvalSearch(board, depth - 1, alpha, beta);
            board.UndoMove(move);

            if (board.IsWhiteToMove)
            {
                bestEval = Math.Max(bestEval, eval);
                alpha = Math.Max(alpha, eval);
            }
            else
            {
                bestEval = Math.Min(bestEval, eval);
                beta = Math.Min(beta, eval);
            }
            
            // Alpha-beta pruning
            if (beta <= alpha)
                break;
        }

        return bestEval;
    }

    public static int Evaluate(Board board)
    {
        // Simple material-count evaluation
        int whiteMaterial = 0;
        int blackMaterial = 0;

        // Calculate material (replace with actual piece counting)
        foreach (var pieceList in board.GetAllPieceLists())
        {
            var value = PieceToValue(pieceList.TypeOfPieceInList) * pieceList.Count;

            if (pieceList.IsWhitePieceList) whiteMaterial += value;
            else blackMaterial += value;
        }

        int evaluation = whiteMaterial - blackMaterial;
        return board.IsWhiteToMove ? evaluation : -evaluation;
    }

    public static int PieceToValue(PieceType piece)
    {
        return piece switch
        {
            PieceType.Pawn => 100,
            PieceType.Knight => 300,
            PieceType.Bishop => 305,
            PieceType.Rook => 500,
            PieceType.Queen => 900,
            _ => 0
        };
    }
}