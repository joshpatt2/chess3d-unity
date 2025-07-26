using UnityEngine;

namespace Chess3D.Tests
{
    /// <summary>
    /// Factory to create test piece instances without requiring Unity GameObjects
    /// </summary>
    public static class TestPieceFactory
    {
        /// <summary>
        /// Create a test pawn
        /// </summary>
        public static Pawn CreatePawn(PlayerColor color, bool hasMoved = false)
        {
            var pawn = CreatePieceGameObject<Pawn>();
            pawn.color = color;
            pawn.hasMoved = hasMoved;
            return pawn;
        }
        
        /// <summary>
        /// Create a test rook
        /// </summary>
        public static Rook CreateRook(PlayerColor color, bool hasMoved = false)
        {
            var rook = CreatePieceGameObject<Rook>();
            rook.color = color;
            rook.hasMoved = hasMoved;
            return rook;
        }
        
        /// <summary>
        /// Create a test knight
        /// </summary>
        public static Knight CreateKnight(PlayerColor color, bool hasMoved = false)
        {
            var knight = CreatePieceGameObject<Knight>();
            knight.color = color;
            knight.hasMoved = hasMoved;
            return knight;
        }
        
        /// <summary>
        /// Create a test bishop
        /// </summary>
        public static Bishop CreateBishop(PlayerColor color, bool hasMoved = false)
        {
            var bishop = CreatePieceGameObject<Bishop>();
            bishop.color = color;
            bishop.hasMoved = hasMoved;
            return bishop;
        }
        
        /// <summary>
        /// Create a test queen
        /// </summary>
        public static Queen CreateQueen(PlayerColor color, bool hasMoved = false)
        {
            var queen = CreatePieceGameObject<Queen>();
            queen.color = color;
            queen.hasMoved = hasMoved;
            return queen;
        }
        
        /// <summary>
        /// Create a test king
        /// </summary>
        public static King CreateKing(PlayerColor color, bool hasMoved = false)
        {
            var king = CreatePieceGameObject<King>();
            king.color = color;
            king.hasMoved = hasMoved;
            return king;
        }
        
        /// <summary>
        /// Create a GameObject with the specified piece component for testing
        /// </summary>
        private static T CreatePieceGameObject<T>() where T : Piece
        {
            var gameObject = new GameObject(typeof(T).Name);
            return gameObject.AddComponent<T>();
        }
    }
}