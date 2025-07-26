using System.Collections.Generic;
using UnityEngine;

namespace Chess3D.Tests
{
    /// <summary>
    /// Mock board implementation for testing piece movement logic without Unity GameObjects
    /// Implements the same interface as Board but without MonoBehaviour dependencies
    /// </summary>
    public class MockBoard
    {
        private Piece[,] mockPieces = new Piece[8, 8];
        
        /// <summary>
        /// Create a mock board with specific piece setup
        /// </summary>
        public MockBoard()
        {
            // Initialize empty board
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    mockPieces[x, y] = null;
                }
            }
        }
        
        /// <summary>
        /// Place a piece at a specific position
        /// </summary>
        public void PlacePiece(Piece piece, int x, int y)
        {
            if (x >= 0 && x < 8 && y >= 0 && y < 8)
            {
                mockPieces[x, y] = piece;
            }
        }
        
        /// <summary>
        /// Get the piece at a specific position
        /// </summary>
        public Piece GetPieceAt(int x, int y)
        {
            if (x >= 0 && x < 8 && y >= 0 && y < 8)
            {
                return mockPieces[x, y];
            }
            return null;
        }
        
        /// <summary>
        /// Get the piece at a specific position using Vector2Int
        /// </summary>
        public Piece GetPieceAt(Vector2Int position)
        {
            return GetPieceAt(position.x, position.y);
        }
        
        /// <summary>
        /// Check if a position is empty
        /// </summary>
        public bool IsEmpty(Vector2Int position)
        {
            return GetPieceAt(position) == null;
        }
        
        /// <summary>
        /// Check if a position contains an enemy piece
        /// </summary>
        public bool IsEnemyPiece(Vector2Int position, PlayerColor playerColor)
        {
            Piece piece = GetPieceAt(position);
            if (piece != null)
            {
                return piece.color != playerColor;
            }
            return false;
        }
        
        /// <summary>
        /// Clear all pieces from the board
        /// </summary>
        public void ClearBoard()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    mockPieces[x, y] = null;
                }
            }
        }
        
        /// <summary>
        /// Setup standard starting position
        /// </summary>
        public void SetupStandardPosition()
        {
            ClearBoard();
            
            // White pieces (ranks 0-1)
            SetupBackRank(0, PlayerColor.White);
            SetupPawnRank(1, PlayerColor.White);
            
            // Black pieces (ranks 6-7)
            SetupPawnRank(6, PlayerColor.Black);
            SetupBackRank(7, PlayerColor.Black);
        }
        
        private void SetupBackRank(int rank, PlayerColor color)
        {
            PlacePiece(TestPieceFactory.CreateRook(color), 0, rank);
            PlacePiece(TestPieceFactory.CreateKnight(color), 1, rank);
            PlacePiece(TestPieceFactory.CreateBishop(color), 2, rank);
            PlacePiece(TestPieceFactory.CreateQueen(color), 3, rank);
            PlacePiece(TestPieceFactory.CreateKing(color), 4, rank);
            PlacePiece(TestPieceFactory.CreateBishop(color), 5, rank);
            PlacePiece(TestPieceFactory.CreateKnight(color), 6, rank);
            PlacePiece(TestPieceFactory.CreateRook(color), 7, rank);
        }
        
        private void SetupPawnRank(int rank, PlayerColor color)
        {
            for (int file = 0; file < 8; file++)
            {
                PlacePiece(TestPieceFactory.CreatePawn(color), file, rank);
            }
        }
    }
}