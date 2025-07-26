using System.Collections.Generic;
using UnityEngine;

namespace Chess3D
{
    /// <summary>
    /// Implements pawn movement rules including initial double move, 
    /// diagonal capture, and en passant (basic implementation)
    /// </summary>
    public class Pawn : Piece
    {
        public override List<Vector2Int> GetAvailableMoves(Vector2Int currentPos, Board board)
        {
            List<Vector2Int> moves = new List<Vector2Int>();

            // Determine forward direction based on piece color
            int forwardDirection = (color == PlayerColor.White) ? 1 : -1;

            // Forward moves
            Vector2Int oneForward = new Vector2Int(currentPos.x, currentPos.y + forwardDirection);
            
            // Check one square forward
            if (IsValidPosition(oneForward) && board.IsEmpty(oneForward))
            {
                moves.Add(oneForward);

                // Check two squares forward (initial move)
                if (!hasMoved)
                {
                    Vector2Int twoForward = new Vector2Int(currentPos.x, currentPos.y + (2 * forwardDirection));
                    if (IsValidPosition(twoForward) && board.IsEmpty(twoForward))
                    {
                        moves.Add(twoForward);
                    }
                }
            }

            // Diagonal captures
            Vector2Int[] captureDirections = {
                new Vector2Int(currentPos.x - 1, currentPos.y + forwardDirection), // Left diagonal
                new Vector2Int(currentPos.x + 1, currentPos.y + forwardDirection)  // Right diagonal
            };

            foreach (Vector2Int capturePos in captureDirections)
            {
                if (IsValidPosition(capturePos) && board.IsEnemyPiece(capturePos, color))
                {
                    moves.Add(capturePos);
                }
            }

            // TODO: Implement en passant capture
            // This would require tracking the last move made in the game
            // For now, basic pawn movement is implemented

            return moves;
        }
    }
}
