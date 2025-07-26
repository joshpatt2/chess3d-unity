using System.Collections.Generic;
using UnityEngine;

namespace Chess3D
{
    /// <summary>
    /// Implements king movement rules - one square in any direction
    /// TODO: Add castling logic in a future iteration
    /// </summary>
    public class King : Piece
    {
        public override List<Vector2Int> GetAvailableMoves(Vector2Int currentPos, Board board)
        {
            List<Vector2Int> moves = new List<Vector2Int>();

            // King can move one square in any direction (8 possible moves)
            Vector2Int[] directions = {
                new Vector2Int(0, 1),   // Up
                new Vector2Int(0, -1),  // Down
                new Vector2Int(1, 0),   // Right
                new Vector2Int(-1, 0),  // Left
                new Vector2Int(1, 1),   // Up-right
                new Vector2Int(1, -1),  // Down-right
                new Vector2Int(-1, 1),  // Up-left
                new Vector2Int(-1, -1)  // Down-left
            };

            // Check each adjacent square
            foreach (Vector2Int direction in directions)
            {
                Vector2Int targetPos = currentPos + direction;

                // Check if the target position is valid
                if (IsValidPosition(targetPos))
                {
                    // King can move to empty squares or capture enemy pieces
                    if (board.IsEmpty(targetPos) || board.IsEnemyPiece(targetPos, color))
                    {
                        moves.Add(targetPos);
                    }
                }
            }

            // Add castling moves if conditions are met
            if (!hasMoved && !IsInCheck(currentPos, board))
            {
                // Check kingside castling (short castling)
                if (CanCastle(currentPos, board, true))
                {
                    Vector2Int castleMove = new Vector2Int(currentPos.x + 2, currentPos.y);
                    moves.Add(castleMove);
                    Debug.Log($"{color} King can castle kingside to {castleMove}");
                }
                
                // Check queenside castling (long castling)
                if (CanCastle(currentPos, board, false))
                {
                    Vector2Int castleMove = new Vector2Int(currentPos.x - 2, currentPos.y);
                    moves.Add(castleMove);
                    Debug.Log($"{color} King can castle queenside to {castleMove}");
                }
            }

            return moves;
        }

        /// <summary>
        /// Check if castling is possible
        /// </summary>
        /// <param name="currentPos">King's current position</param>
        /// <param name="board">Game board reference</param>
        /// <param name="kingSide">True for kingside (short) castling, false for queenside (long)</param>
        /// <returns>True if castling is legal</returns>
        private bool CanCastle(Vector2Int currentPos, Board board, bool kingSide)
        {
            // King must not have moved
            if (hasMoved)
                return false;

            // Determine rook position based on castling side
            int rookFile = kingSide ? 7 : 0; // Kingside rook at file 7, queenside at file 0
            Vector2Int rookPos = new Vector2Int(rookFile, currentPos.y);
            
            // Check if rook exists and hasn't moved
            Piece rook = board.GetPieceAt(rookPos);
            if (rook == null || rook.color != color || rook.hasMoved || !(rook is Rook))
                return false;

            // Check if path between king and rook is clear
            int direction = kingSide ? 1 : -1;
            int startFile = currentPos.x + direction;
            int endFile = kingSide ? rookFile - 1 : rookFile + 1;
            
            for (int file = startFile; kingSide ? file <= endFile : file >= endFile; file += direction)
            {
                Vector2Int checkPos = new Vector2Int(file, currentPos.y);
                if (!board.IsEmpty(checkPos))
                    return false;
            }

            // Check that king doesn't pass through or end in check
            // King moves two squares toward the rook
            Vector2Int kingPassThrough = new Vector2Int(currentPos.x + direction, currentPos.y);
            Vector2Int kingDestination = new Vector2Int(currentPos.x + (2 * direction), currentPos.y);
            
            // King cannot pass through check
            if (IsSquareUnderAttack(kingPassThrough, board, GetOpponentColor(color)))
                return false;
                
            // King cannot end in check
            if (IsSquareUnderAttack(kingDestination, board, GetOpponentColor(color)))
                return false;

            return true;
        }

        /// <summary>
        /// Check if the king is currently in check at a specific position
        /// </summary>
        private bool IsInCheck(Vector2Int kingPos, Board board)
        {
            return IsSquareUnderAttack(kingPos, board, GetOpponentColor(color));
        }

        /// <summary>
        /// Check if a square is under attack by the opponent
        /// </summary>
        private bool IsSquareUnderAttack(Vector2Int position, Board board, PlayerColor attackingColor)
        {
            // Check all pieces of the attacking color
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Piece piece = board.GetPieceAt(x, y);
                    if (piece != null && piece.color == attackingColor)
                    {
                        // Get the piece's attacking moves
                        List<Vector2Int> attackingMoves = piece.GetAvailableMoves(new Vector2Int(x, y), board);
                        
                        // Special case for pawns - they attack diagonally, not forward
                        if (piece is Pawn)
                        {
                            attackingMoves = GetPawnAttackSquares(new Vector2Int(x, y), piece.color);
                        }
                        
                        if (attackingMoves.Contains(position))
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Get pawn attack squares (diagonal captures only)
        /// </summary>
        private List<Vector2Int> GetPawnAttackSquares(Vector2Int pawnPos, PlayerColor pawnColor)
        {
            List<Vector2Int> attacks = new List<Vector2Int>();
            int forwardDirection = (pawnColor == PlayerColor.White) ? 1 : -1;
            
            // Left diagonal attack
            Vector2Int leftAttack = new Vector2Int(pawnPos.x - 1, pawnPos.y + forwardDirection);
            if (IsValidPosition(leftAttack))
                attacks.Add(leftAttack);
                
            // Right diagonal attack  
            Vector2Int rightAttack = new Vector2Int(pawnPos.x + 1, pawnPos.y + forwardDirection);
            if (IsValidPosition(rightAttack))
                attacks.Add(rightAttack);
                
            return attacks;
        }

        /// <summary>
        /// Get the opposite color
        /// </summary>
        private PlayerColor GetOpponentColor(PlayerColor color)
        {
            return (color == PlayerColor.White) ? PlayerColor.Black : PlayerColor.White;
        }
    }
}
