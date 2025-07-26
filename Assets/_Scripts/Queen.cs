using System.Collections.Generic;
using UnityEngine;

namespace Chess3D
{
    /// <summary>
    /// Implements queen movement rules - combination of rook and bishop (horizontal, vertical, and diagonal sliding)
    /// </summary>
    public class Queen : Piece
    {
        public override List<Vector2Int> GetAvailableMoves(Vector2Int currentPos, Board board)
        {
            List<Vector2Int> moves = new List<Vector2Int>();

            // Queen moves like both rook and bishop - all 8 directions
            Vector2Int[] directions = {
                // Rook directions (horizontal and vertical)
                new Vector2Int(0, 1),   // Up
                new Vector2Int(0, -1),  // Down
                new Vector2Int(1, 0),   // Right
                new Vector2Int(-1, 0),  // Left
                
                // Bishop directions (diagonal)
                new Vector2Int(1, 1),   // Up-right
                new Vector2Int(1, -1),  // Down-right
                new Vector2Int(-1, 1),  // Up-left
                new Vector2Int(-1, -1)  // Down-left
            };

            // Check each direction
            foreach (Vector2Int direction in directions)
            {
                AddMovesInDirection(currentPos, direction, board, moves);
            }

            return moves;
        }

        /// <summary>
        /// Add all valid moves in a specific direction until blocked
        /// </summary>
        private void AddMovesInDirection(Vector2Int startPos, Vector2Int direction, Board board, List<Vector2Int> moves)
        {
            Vector2Int currentPos = startPos + direction;

            while (IsValidPosition(currentPos))
            {
                if (board.IsEmpty(currentPos))
                {
                    // Empty square - can move here and continue
                    moves.Add(currentPos);
                }
                else if (board.IsEnemyPiece(currentPos, color))
                {
                    // Enemy piece - can capture but cannot continue
                    moves.Add(currentPos);
                    break;
                }
                else
                {
                    // Friendly piece - cannot move here or continue
                    break;
                }

                // Move to next square in this direction
                currentPos += direction;
            }
        }
    }
}
