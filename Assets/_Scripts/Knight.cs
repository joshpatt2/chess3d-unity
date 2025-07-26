using System.Collections.Generic;
using UnityEngine;

namespace Chess3D
{
    /// <summary>
    /// Implements knight movement rules - L-shaped moves that can jump over pieces
    /// </summary>
    public class Knight : Piece
    {
        public override List<Vector2Int> GetAvailableMoves(Vector2Int currentPos, Board board)
        {
            List<Vector2Int> moves = new List<Vector2Int>();

            // Knight moves in L-shapes: 2 squares in one direction, 1 square perpendicular
            Vector2Int[] knightMoves = {
                new Vector2Int(2, 1),   // 2 right, 1 up
                new Vector2Int(2, -1),  // 2 right, 1 down
                new Vector2Int(-2, 1),  // 2 left, 1 up
                new Vector2Int(-2, -1), // 2 left, 1 down
                new Vector2Int(1, 2),   // 1 right, 2 up
                new Vector2Int(1, -2),  // 1 right, 2 down
                new Vector2Int(-1, 2),  // 1 left, 2 up
                new Vector2Int(-1, -2)  // 1 left, 2 down
            };

            // Check each possible knight move
            foreach (Vector2Int move in knightMoves)
            {
                Vector2Int targetPos = currentPos + move;

                // Check if the target position is valid
                if (IsValidPosition(targetPos))
                {
                    // Knight can move to empty squares or capture enemy pieces
                    if (board.IsEmpty(targetPos) || board.IsEnemyPiece(targetPos, color))
                    {
                        moves.Add(targetPos);
                    }
                }
            }

            return moves;
        }
    }
}
