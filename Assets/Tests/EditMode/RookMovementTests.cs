using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Chess3D.Tests
{
    /// <summary>
    /// Tests for rook movement validation including horizontal and vertical movement
    /// </summary>
    public class RookMovementTests
    {
        /// <summary>
        /// Test that rooks cannot move initially (blocked by pawns)
        /// </summary>
        [Test]
        public void RooksCannotMoveInitiallyBlockedByPawns()
        {
            // Arrange - Standard starting position
            var boardState = TestBoardHelper.CreateStandardStartingPosition();
            
            // Test white rooks
            var whiteQueensideRook = boardState[0, 0] as Rook; // a1
            Assert.IsNotNull(whiteQueensideRook, "White queenside rook should exist at a1");
            
            var whiteQueensideMoves = TestBoardHelper.GetPieceMovesFromPosition(whiteQueensideRook, new Vector2Int(0, 0), boardState);
            Assert.AreEqual(0, whiteQueensideMoves.Count, "White queenside rook should have no moves when blocked by pawns");
            
            var whiteKingsideRook = boardState[7, 0] as Rook; // h1
            Assert.IsNotNull(whiteKingsideRook, "White kingside rook should exist at h1");
            
            var whiteKingsideMoves = TestBoardHelper.GetPieceMovesFromPosition(whiteKingsideRook, new Vector2Int(7, 0), boardState);
            Assert.AreEqual(0, whiteKingsideMoves.Count, "White kingside rook should have no moves when blocked by pawns");
            
            // Test black rooks
            var blackQueensideRook = boardState[0, 7] as Rook; // a8
            Assert.IsNotNull(blackQueensideRook, "Black queenside rook should exist at a8");
            
            var blackQueensideMoves = TestBoardHelper.GetPieceMovesFromPosition(blackQueensideRook, new Vector2Int(0, 7), boardState);
            Assert.AreEqual(0, blackQueensideMoves.Count, "Black queenside rook should have no moves when blocked by pawns");
            
            var blackKingsideRook = boardState[7, 7] as Rook; // h8
            Assert.IsNotNull(blackKingsideRook, "Black kingside rook should exist at h8");
            
            var blackKingsideMoves = TestBoardHelper.GetPieceMovesFromPosition(blackKingsideRook, new Vector2Int(7, 7), boardState);
            Assert.AreEqual(0, blackKingsideMoves.Count, "Black kingside rook should have no moves when blocked by pawns");
        }
        
        /// <summary>
        /// Test rook horizontal movement on empty board
        /// </summary>
        [Test]
        public void RookCanMoveHorizontallyOnEmptyBoard()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var rook = TestPieceFactory.CreateRook(PlayerColor.White);
            var rookPos = new Vector2Int(4, 4); // e5 - center of board
            TestBoardHelper.PlacePiece(boardState, rook, rookPos.x, rookPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(rook, rookPos, boardState);
            
            // Assert - Should be able to move horizontally (along rank 5)
            var expectedHorizontalMoves = new List<Vector2Int>
            {
                // Left moves
                new Vector2Int(0, 4), // a5
                new Vector2Int(1, 4), // b5
                new Vector2Int(2, 4), // c5
                new Vector2Int(3, 4), // d5
                // Right moves
                new Vector2Int(5, 4), // f5
                new Vector2Int(6, 4), // g5
                new Vector2Int(7, 4)  // h5
            };
            
            foreach (var expectedMove in expectedHorizontalMoves)
            {
                Assert.Contains(expectedMove, moves, $"Rook should be able to move horizontally to {TestBoardHelper.CoordsToChessNotation(expectedMove)}");
            }
        }
        
        /// <summary>
        /// Test rook vertical movement on empty board
        /// </summary>
        [Test]
        public void RookCanMoveVerticallyOnEmptyBoard()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var rook = TestPieceFactory.CreateRook(PlayerColor.White);
            var rookPos = new Vector2Int(4, 4); // e5 - center of board
            TestBoardHelper.PlacePiece(boardState, rook, rookPos.x, rookPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(rook, rookPos, boardState);
            
            // Assert - Should be able to move vertically (along e-file)
            var expectedVerticalMoves = new List<Vector2Int>
            {
                // Down moves
                new Vector2Int(4, 0), // e1
                new Vector2Int(4, 1), // e2
                new Vector2Int(4, 2), // e3
                new Vector2Int(4, 3), // e4
                // Up moves
                new Vector2Int(4, 5), // e6
                new Vector2Int(4, 6), // e7
                new Vector2Int(4, 7)  // e8
            };
            
            foreach (var expectedMove in expectedVerticalMoves)
            {
                Assert.Contains(expectedMove, moves, $"Rook should be able to move vertically to {TestBoardHelper.CoordsToChessNotation(expectedMove)}");
            }
        }
        
        /// <summary>
        /// Test that rook has complete range of movement on empty board
        /// </summary>
        [Test]
        public void RookHasFullRangeOnEmptyBoard()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var rook = TestPieceFactory.CreateRook(PlayerColor.White);
            var rookPos = new Vector2Int(4, 4); // e5
            TestBoardHelper.PlacePiece(boardState, rook, rookPos.x, rookPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(rook, rookPos, boardState);
            
            // Assert - Should have 14 moves total (7 horizontal + 7 vertical)
            Assert.AreEqual(14, moves.Count, "Rook should have 14 moves from center of empty board");
        }
        
        /// <summary>
        /// Test rook is blocked by friendly pieces
        /// </summary>
        [Test]
        public void RookIsBlockedByFriendlyPieces()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var rook = TestPieceFactory.CreateRook(PlayerColor.White);
            var rookPos = new Vector2Int(4, 4); // e5
            TestBoardHelper.PlacePiece(boardState, rook, rookPos.x, rookPos.y);
            
            // Place friendly pieces to block movement
            var friendlyPiece1 = TestPieceFactory.CreatePawn(PlayerColor.White);
            var friendlyPiece2 = TestPieceFactory.CreatePawn(PlayerColor.White);
            TestBoardHelper.PlacePiece(boardState, friendlyPiece1, 4, 6); // e7 (blocks upward)
            TestBoardHelper.PlacePiece(boardState, friendlyPiece2, 6, 4); // g5 (blocks rightward)
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(rook, rookPos, boardState);
            
            // Assert - Should not be able to reach blocked squares or beyond
            Assert.False(moves.Contains(new Vector2Int(4, 6)), "Rook should not be able to move to square occupied by friendly piece");
            Assert.False(moves.Contains(new Vector2Int(4, 7)), "Rook should not be able to move beyond friendly piece");
            Assert.False(moves.Contains(new Vector2Int(6, 4)), "Rook should not be able to move to square occupied by friendly piece");
            Assert.False(moves.Contains(new Vector2Int(7, 4)), "Rook should not be able to move beyond friendly piece");
            
            // Should still be able to move to squares before the blocking pieces
            Assert.Contains(new Vector2Int(4, 5), moves, "Rook should be able to move to square before friendly piece"); // e6
            Assert.Contains(new Vector2Int(5, 4), moves, "Rook should be able to move to square before friendly piece"); // f5
        }
        
        /// <summary>
        /// Test rook can capture enemy pieces but cannot move beyond them
        /// </summary>
        [Test]
        public void RookCanCaptureEnemyPiecesButCannotMoveBeyond()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var rook = TestPieceFactory.CreateRook(PlayerColor.White);
            var rookPos = new Vector2Int(4, 4); // e5
            TestBoardHelper.PlacePiece(boardState, rook, rookPos.x, rookPos.y);
            
            // Place enemy pieces
            var enemyPiece1 = TestPieceFactory.CreatePawn(PlayerColor.Black);
            var enemyPiece2 = TestPieceFactory.CreatePawn(PlayerColor.Black);
            TestBoardHelper.PlacePiece(boardState, enemyPiece1, 4, 6); // e7 (blocks upward)
            TestBoardHelper.PlacePiece(boardState, enemyPiece2, 2, 4); // c5 (blocks leftward)
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(rook, rookPos, boardState);
            
            // Assert - Should be able to capture enemy pieces
            Assert.Contains(new Vector2Int(4, 6), moves, "Rook should be able to capture enemy piece at e7");
            Assert.Contains(new Vector2Int(2, 4), moves, "Rook should be able to capture enemy piece at c5");
            
            // Should not be able to move beyond captured pieces
            Assert.False(moves.Contains(new Vector2Int(4, 7)), "Rook should not be able to move beyond captured enemy piece"); // e8
            Assert.False(moves.Contains(new Vector2Int(1, 4)), "Rook should not be able to move beyond captured enemy piece"); // b5
        }
        
        /// <summary>
        /// Test rook movement from corner positions
        /// </summary>
        [Test]
        public void RookMovementFromCornerPositions()
        {
            // Test from a1 corner
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var rook = TestPieceFactory.CreateRook(PlayerColor.White);
            var cornerPos = new Vector2Int(0, 0); // a1
            TestBoardHelper.PlacePiece(boardState, rook, cornerPos.x, cornerPos.y);
            
            var moves = TestBoardHelper.GetPieceMovesFromPosition(rook, cornerPos, boardState);
            
            // Should have 14 moves (7 along rank 1, 7 along a-file)
            Assert.AreEqual(14, moves.Count, "Rook should have 14 moves from corner");
            
            // Check some specific moves
            Assert.Contains(new Vector2Int(7, 0), moves, "Rook should be able to move to h1"); // along rank
            Assert.Contains(new Vector2Int(0, 7), moves, "Rook should be able to move to a8"); // along file
        }
        
        /// <summary>
        /// Test rook cannot move diagonally
        /// </summary>
        [Test]
        public void RookCannotMoveDiagonally()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var rook = TestPieceFactory.CreateRook(PlayerColor.White);
            var rookPos = new Vector2Int(4, 4); // e5
            TestBoardHelper.PlacePiece(boardState, rook, rookPos.x, rookPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(rook, rookPos, boardState);
            
            // Assert - Should not contain any diagonal moves
            var diagonalMoves = new List<Vector2Int>
            {
                new Vector2Int(3, 3), // d4
                new Vector2Int(3, 5), // d6
                new Vector2Int(5, 3), // f4
                new Vector2Int(5, 5), // f6
                new Vector2Int(2, 2), // c3
                new Vector2Int(6, 6), // g7
            };
            
            foreach (var diagonalMove in diagonalMoves)
            {
                Assert.False(moves.Contains(diagonalMove), $"Rook should not be able to move diagonally to {TestBoardHelper.CoordsToChessNotation(diagonalMove)}");
            }
        }
        
        /// <summary>
        /// Test rook movement logic is present and working correctly
        /// This validates the core sliding piece logic for horizontal and vertical movement
        /// </summary>
        [Test]
        public void RookSlidingMovementLogicIsCorrect()
        {
            // Test horizontal sliding
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var rook = TestPieceFactory.CreateRook(PlayerColor.White);
            var rookPos = new Vector2Int(0, 4); // a5 (edge position)
            TestBoardHelper.PlacePiece(boardState, rook, rookPos.x, rookPos.y);
            
            var moves = TestBoardHelper.GetPieceMovesFromPosition(rook, rookPos, boardState);
            
            // Should be able to slide across entire rank
            for (int file = 1; file < 8; file++)
            {
                Assert.Contains(new Vector2Int(file, 4), moves, $"Rook should be able to slide to {TestBoardHelper.CoordsToChessNotation(new Vector2Int(file, 4))}");
            }
            
            // Should be able to slide along entire file
            for (int rank = 0; rank < 8; rank++)
            {
                if (rank != 4) // Skip current position
                {
                    Assert.Contains(new Vector2Int(0, rank), moves, $"Rook should be able to slide to {TestBoardHelper.CoordsToChessNotation(new Vector2Int(0, rank))}");
                }
            }
        }
    }
}