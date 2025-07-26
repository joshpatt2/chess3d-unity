using System.Collections.Generic;
using UnityEngine;

namespace Chess3D
{
    public enum PlayerColor
    {
        White,
        Black
    }

    /// <summary>
    /// Abstract base class for all chess pieces
    /// Implements the common properties and behaviors shared by all pieces
    /// </summary>
    public abstract class Piece : MonoBehaviour
    {
        [Header("Piece Properties")]
        public PlayerColor color;
        public bool hasMoved = false;

        /// <summary>
        /// Abstract method that each piece type must implement
        /// Returns a list of all possible moves for this piece from its current position
        /// </summary>
        /// <param name="currentPos">Current position on the board (0-7, 0-7)</param>
        /// <param name="board">Reference to the game board</param>
        /// <returns>List of valid move coordinates</returns>
        public abstract List<Vector2Int> GetAvailableMoves(Vector2Int currentPos, Board board);

        /// <summary>
        /// Called when this piece is moved to mark it as having moved
        /// Important for castling and pawn initial double-move rules
        /// </summary>
        public virtual void OnMoved()
        {
            hasMoved = true;
        }

        /// <summary>
        /// Helper method to check if a position is within the board bounds
        /// </summary>
        /// <param name="position">Position to check</param>
        /// <returns>True if position is valid (0-7, 0-7)</returns>
        protected bool IsValidPosition(Vector2Int position)
        {
            return position.x >= 0 && position.x < 8 && position.y >= 0 && position.y < 8;
        }
    }
}
