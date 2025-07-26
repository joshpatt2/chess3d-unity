using System.Collections.Generic;
using UnityEngine;

namespace Chess3D.Tests
{
    /// <summary>
    /// Helper methods for testing piece movement logic
    /// </summary>
    public static class TestBoardHelper
    {
        /// <summary>
        /// Test piece movement using a minimal board setup
        /// Creates a temporary Board GameObject for testing
        /// </summary>
        public static List<Vector2Int> GetPieceMovesFromPosition(Piece piece, Vector2Int position, Piece[,] boardState)
        {
            // Create a minimal Board GameObject for testing
            var testBoardObject = new GameObject("TestBoard");
            var testBoard = testBoardObject.AddComponent<Board>();
            
            // Override the board's internal state for testing
            SetBoardState(testBoard, boardState);
            
            // Get moves from the piece
            var moves = piece.GetAvailableMoves(position, testBoard);
            
            // Cleanup
            Object.DestroyImmediate(testBoardObject);
            
            return moves;
        }
        
        /// <summary>
        /// Set up a board state for testing using reflection
        /// </summary>
        private static void SetBoardState(Board board, Piece[,] boardState)
        {
            // Use reflection to set the private pieces array
            var piecesField = typeof(Board).GetField("pieces", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (piecesField != null)
            {
                piecesField.SetValue(board, boardState);
            }
        }
        
        /// <summary>
        /// Create an empty 8x8 board state
        /// </summary>
        public static Piece[,] CreateEmptyBoard()
        {
            return new Piece[8, 8];
        }
        
        /// <summary>
        /// Create a board state with standard starting position
        /// </summary>
        public static Piece[,] CreateStandardStartingPosition()
        {
            var board = new Piece[8, 8];
            
            // White pieces (ranks 0-1)
            SetupBackRank(board, 0, PlayerColor.White);
            SetupPawnRank(board, 1, PlayerColor.White);
            
            // Black pieces (ranks 6-7)  
            SetupPawnRank(board, 6, PlayerColor.Black);
            SetupBackRank(board, 7, PlayerColor.Black);
            
            return board;
        }
        
        private static void SetupBackRank(Piece[,] board, int rank, PlayerColor color)
        {
            board[0, rank] = TestPieceFactory.CreateRook(color);
            board[1, rank] = TestPieceFactory.CreateKnight(color);
            board[2, rank] = TestPieceFactory.CreateBishop(color);
            board[3, rank] = TestPieceFactory.CreateQueen(color);
            board[4, rank] = TestPieceFactory.CreateKing(color);
            board[5, rank] = TestPieceFactory.CreateBishop(color);
            board[6, rank] = TestPieceFactory.CreateKnight(color);
            board[7, rank] = TestPieceFactory.CreateRook(color);
        }
        
        private static void SetupPawnRank(Piece[,] board, int rank, PlayerColor color)
        {
            for (int file = 0; file < 8; file++)
            {
                board[file, rank] = TestPieceFactory.CreatePawn(color);
            }
        }
        
        /// <summary>
        /// Place a piece on the board at the specified position
        /// </summary>
        public static void PlacePiece(Piece[,] board, Piece piece, int x, int y)
        {
            if (x >= 0 && x < 8 && y >= 0 && y < 8)
            {
                board[x, y] = piece;
            }
        }
        
        /// <summary>
        /// Helper to convert chess notation to coordinates
        /// e.g., "e4" -> (4, 3), "a1" -> (0, 0)
        /// </summary>
        public static Vector2Int ChessNotationToCoords(string notation)
        {
            if (notation.Length != 2) return Vector2Int.zero;
            
            int file = notation[0] - 'a'; // a=0, b=1, etc.
            int rank = notation[1] - '1'; // 1=0, 2=1, etc.
            
            return new Vector2Int(file, rank);
        }
        
        /// <summary>
        /// Helper to convert coordinates to chess notation
        /// e.g., (4, 3) -> "e4", (0, 0) -> "a1"
        /// </summary>
        public static string CoordsToChessNotation(Vector2Int coords)
        {
            char file = (char)('a' + coords.x);
            char rank = (char)('1' + coords.y);
            return $"{file}{rank}";
        }
    }
}