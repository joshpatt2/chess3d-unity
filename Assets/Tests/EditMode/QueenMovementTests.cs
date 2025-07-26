using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Chess3D.Tests
{
    /// <summary>
    /// Tests for queen movement validation (combination of rook + bishop movement)
    /// </summary>
    public class QueenMovementTests
    {
        /// <summary>
        /// Test that queen cannot move initially (blocked by pawns)
        /// </summary>
        [Test]
        public void QueenCannotMoveInitiallyBlockedByPawns()
        {
            // Arrange - Standard starting position
            var boardState = TestBoardHelper.CreateStandardStartingPosition();
            
            // Test white queen
            var whiteQueen = boardState[3, 0] as Queen; // d1
            Assert.IsNotNull(whiteQueen, "White queen should exist at d1");
            
            var whiteQueenMoves = TestBoardHelper.GetPieceMovesFromPosition(whiteQueen, new Vector2Int(3, 0), boardState);
            Assert.AreEqual(0, whiteQueenMoves.Count, "White queen should have no moves when blocked by pawns");
            
            // Test black queen
            var blackQueen = boardState[3, 7] as Queen; // d8
            Assert.IsNotNull(blackQueen, "Black queen should exist at d8");
            
            var blackQueenMoves = TestBoardHelper.GetPieceMovesFromPosition(blackQueen, new Vector2Int(3, 7), boardState);
            Assert.AreEqual(0, blackQueenMoves.Count, "Black queen should have no moves when blocked by pawns");
        }
        
        /// <summary>
        /// Test queen combines rook + bishop movement on empty board
        /// </summary>
        [Test]
        public void QueenCombinesRookAndBishopMovement()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var queen = TestPieceFactory.CreateQueen(PlayerColor.White);
            var queenPos = new Vector2Int(4, 4); // e5 - center of board
            TestBoardHelper.PlacePiece(boardState, queen, queenPos.x, queenPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(queen, queenPos, boardState);
            
            // Assert - Should have all rook moves (14) + all bishop moves (13) = 27 total
            Assert.AreEqual(27, moves.Count, "Queen should have 27 moves from center of empty board (14 rook + 13 bishop)");
            
            // Test horizontal moves (rook-like)
            for (int file = 0; file < 8; file++)
            {
                if (file != 4) // Skip current position
                {
                    Assert.Contains(new Vector2Int(file, 4), moves, $"Queen should be able to move horizontally to {TestBoardHelper.CoordsToChessNotation(new Vector2Int(file, 4))}");
                }
            }
            
            // Test vertical moves (rook-like)
            for (int rank = 0; rank < 8; rank++)
            {
                if (rank != 4) // Skip current position
                {
                    Assert.Contains(new Vector2Int(4, rank), moves, $"Queen should be able to move vertically to {TestBoardHelper.CoordsToChessNotation(new Vector2Int(4, rank))}");
                }
            }
            
            // Test diagonal moves (bishop-like)
            var diagonalMoves = new List<Vector2Int>
            {
                // Up-right diagonal
                new Vector2Int(5, 5), new Vector2Int(6, 6), new Vector2Int(7, 7),
                // Up-left diagonal
                new Vector2Int(3, 5), new Vector2Int(2, 6), new Vector2Int(1, 7),
                // Down-right diagonal
                new Vector2Int(5, 3), new Vector2Int(6, 2), new Vector2Int(7, 1),
                // Down-left diagonal
                new Vector2Int(3, 3), new Vector2Int(2, 2), new Vector2Int(1, 1), new Vector2Int(0, 0)
            };
            
            foreach (var diagonalMove in diagonalMoves)
            {
                Assert.Contains(diagonalMove, moves, $"Queen should be able to move diagonally to {TestBoardHelper.CoordsToChessNotation(diagonalMove)}");
            }
        }
        
        /// <summary>
        /// Test queen horizontal movement (rook-like behavior)
        /// </summary>
        [Test]
        public void QueenCanMoveHorizontallyLikeRook()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var queen = TestPieceFactory.CreateQueen(PlayerColor.White);
            var queenPos = new Vector2Int(0, 4); // a5 - edge position
            TestBoardHelper.PlacePiece(boardState, queen, queenPos.x, queenPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(queen, queenPos, boardState);
            
            // Assert - Should be able to move across entire rank
            for (int file = 1; file < 8; file++)
            {
                Assert.Contains(new Vector2Int(file, 4), moves, $"Queen should be able to move horizontally to {TestBoardHelper.CoordsToChessNotation(new Vector2Int(file, 4))}");
            }
        }
        
        /// <summary>
        /// Test queen vertical movement (rook-like behavior)
        /// </summary>
        [Test]
        public void QueenCanMoveVerticallyLikeRook()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var queen = TestPieceFactory.CreateQueen(PlayerColor.White);
            var queenPos = new Vector2Int(4, 0); // e1 - bottom edge
            TestBoardHelper.PlacePiece(boardState, queen, queenPos.x, queenPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(queen, queenPos, boardState);
            
            // Assert - Should be able to move up entire file
            for (int rank = 1; rank < 8; rank++)
            {
                Assert.Contains(new Vector2Int(4, rank), moves, $"Queen should be able to move vertically to {TestBoardHelper.CoordsToChessNotation(new Vector2Int(4, rank))}");
            }
        }
        
        /// <summary>
        /// Test queen diagonal movement (bishop-like behavior)
        /// </summary>
        [Test]
        public void QueenCanMoveDiagonallyLikeBishop()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var queen = TestPieceFactory.CreateQueen(PlayerColor.White);
            var queenPos = new Vector2Int(0, 0); // a1 - corner
            TestBoardHelper.PlacePiece(boardState, queen, queenPos.x, queenPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(queen, queenPos, boardState);
            
            // Assert - Should be able to move diagonally up-right
            var diagonalMoves = new List<Vector2Int>
            {
                new Vector2Int(1, 1), // b2
                new Vector2Int(2, 2), // c3
                new Vector2Int(3, 3), // d4
                new Vector2Int(4, 4), // e5
                new Vector2Int(5, 5), // f6
                new Vector2Int(6, 6), // g7
                new Vector2Int(7, 7)  // h8
            };
            
            foreach (var diagonalMove in diagonalMoves)
            {
                Assert.Contains(diagonalMove, moves, $"Queen should be able to move diagonally to {TestBoardHelper.CoordsToChessNotation(diagonalMove)}");
            }
        }
        
        /// <summary>
        /// Test queen is blocked by friendly pieces (in all directions)
        /// </summary>
        [Test]
        public void QueenIsBlockedByFriendlyPieces()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var queen = TestPieceFactory.CreateQueen(PlayerColor.White);
            var queenPos = new Vector2Int(4, 4); // e5
            TestBoardHelper.PlacePiece(boardState, queen, queenPos.x, queenPos.y);
            
            // Place friendly pieces to block movement in different directions
            var friendlyPiece1 = TestPieceFactory.CreatePawn(PlayerColor.White);
            var friendlyPiece2 = TestPieceFactory.CreatePawn(PlayerColor.White);
            var friendlyPiece3 = TestPieceFactory.CreatePawn(PlayerColor.White);
            var friendlyPiece4 = TestPieceFactory.CreatePawn(PlayerColor.White);
            
            TestBoardHelper.PlacePiece(boardState, friendlyPiece1, 4, 6); // e7 (blocks vertical up)
            TestBoardHelper.PlacePiece(boardState, friendlyPiece2, 6, 4); // g5 (blocks horizontal right)
            TestBoardHelper.PlacePiece(boardState, friendlyPiece3, 6, 6); // g7 (blocks diagonal up-right)
            TestBoardHelper.PlacePiece(boardState, friendlyPiece4, 2, 2); // c3 (blocks diagonal down-left)
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(queen, queenPos, boardState);
            
            // Assert - Should not be able to reach blocked squares or beyond
            Assert.False(moves.Contains(new Vector2Int(4, 6)), "Queen should not be able to move to square occupied by friendly piece");
            Assert.False(moves.Contains(new Vector2Int(4, 7)), "Queen should not be able to move beyond friendly piece vertically");
            Assert.False(moves.Contains(new Vector2Int(6, 4)), "Queen should not be able to move to square occupied by friendly piece");
            Assert.False(moves.Contains(new Vector2Int(7, 4)), "Queen should not be able to move beyond friendly piece horizontally");
            Assert.False(moves.Contains(new Vector2Int(6, 6)), "Queen should not be able to move to square occupied by friendly piece");
            Assert.False(moves.Contains(new Vector2Int(7, 7)), "Queen should not be able to move beyond friendly piece diagonally");
            Assert.False(moves.Contains(new Vector2Int(2, 2)), "Queen should not be able to move to square occupied by friendly piece");
            Assert.False(moves.Contains(new Vector2Int(1, 1)), "Queen should not be able to move beyond friendly piece diagonally");
            
            // Should still be able to move to squares before the blocking pieces
            Assert.Contains(new Vector2Int(4, 5), moves, "Queen should be able to move to square before friendly piece"); // e6
            Assert.Contains(new Vector2Int(5, 4), moves, "Queen should be able to move to square before friendly piece"); // f5
            Assert.Contains(new Vector2Int(5, 5), moves, "Queen should be able to move to square before friendly piece"); // f6
            Assert.Contains(new Vector2Int(3, 3), moves, "Queen should be able to move to square before friendly piece"); // d4
        }
        
        /// <summary>
        /// Test queen can capture enemy pieces but cannot move beyond them
        /// </summary>
        [Test]
        public void QueenCanCaptureEnemyPiecesButCannotMoveBeyond()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var queen = TestPieceFactory.CreateQueen(PlayerColor.White);
            var queenPos = new Vector2Int(4, 4); // e5
            TestBoardHelper.PlacePiece(boardState, queen, queenPos.x, queenPos.y);
            
            // Place enemy pieces in different directions
            var enemyPiece1 = TestPieceFactory.CreatePawn(PlayerColor.Black);
            var enemyPiece2 = TestPieceFactory.CreatePawn(PlayerColor.Black);
            var enemyPiece3 = TestPieceFactory.CreatePawn(PlayerColor.Black);
            
            TestBoardHelper.PlacePiece(boardState, enemyPiece1, 4, 6); // e7 (vertical)
            TestBoardHelper.PlacePiece(boardState, enemyPiece2, 6, 4); // g5 (horizontal)
            TestBoardHelper.PlacePiece(boardState, enemyPiece3, 6, 6); // g7 (diagonal)
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(queen, queenPos, boardState);
            
            // Assert - Should be able to capture enemy pieces
            Assert.Contains(new Vector2Int(4, 6), moves, "Queen should be able to capture enemy piece at e7");
            Assert.Contains(new Vector2Int(6, 4), moves, "Queen should be able to capture enemy piece at g5");
            Assert.Contains(new Vector2Int(6, 6), moves, "Queen should be able to capture enemy piece at g7");
            
            // Should not be able to move beyond captured pieces
            Assert.False(moves.Contains(new Vector2Int(4, 7)), "Queen should not be able to move beyond captured enemy piece"); // e8
            Assert.False(moves.Contains(new Vector2Int(7, 4)), "Queen should not be able to move beyond captured enemy piece"); // h5
            Assert.False(moves.Contains(new Vector2Int(7, 7)), "Queen should not be able to move beyond captured enemy piece"); // h8
        }
        
        /// <summary>
        /// Test queen movement from corner positions
        /// </summary>
        [Test]
        public void QueenMovementFromCornerPositions()
        {
            // Test from a1 corner
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var queen = TestPieceFactory.CreateQueen(PlayerColor.White);
            var cornerPos = new Vector2Int(0, 0); // a1
            TestBoardHelper.PlacePiece(boardState, queen, cornerPos.x, cornerPos.y);
            
            var moves = TestBoardHelper.GetPieceMovesFromPosition(queen, cornerPos, boardState);
            
            // Should have 21 moves (7 horizontal + 7 vertical + 7 diagonal)
            Assert.AreEqual(21, moves.Count, "Queen should have 21 moves from corner");
            
            // Check specific moves
            Assert.Contains(new Vector2Int(7, 0), moves, "Queen should be able to move to h1"); // horizontal
            Assert.Contains(new Vector2Int(0, 7), moves, "Queen should be able to move to a8"); // vertical
            Assert.Contains(new Vector2Int(7, 7), moves, "Queen should be able to move to h8"); // diagonal
        }
        
        /// <summary>
        /// Test queen has maximum mobility from center
        /// </summary>
        [Test]
        public void QueenHasMaximumMobilityFromCenter()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var queen = TestPieceFactory.CreateQueen(PlayerColor.White);
            var centerPos = new Vector2Int(3, 3); // d4
            TestBoardHelper.PlacePiece(boardState, queen, centerPos.x, centerPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(queen, centerPos, boardState);
            
            // Assert - Should have maximum moves from this position
            // Horizontal: 7, Vertical: 7, Diagonal: 13 = 27 total
            Assert.AreEqual(27, moves.Count, "Queen should have 27 moves from d4");
            
            // Verify queen can reach all corners from center-ish position
            Assert.Contains(new Vector2Int(0, 0), moves, "Queen should be able to reach a1");
            Assert.Contains(new Vector2Int(7, 0), moves, "Queen should be able to reach h1");
            Assert.Contains(new Vector2Int(0, 7), moves, "Queen should be able to reach a8");
            Assert.Contains(new Vector2Int(7, 7), moves, "Queen should be able to reach h8");
        }
        
        /// <summary>
        /// Test queen movement combines all three piece types correctly
        /// Validates that the queen's GetAvailableMoves includes rook + bishop logic
        /// </summary>
        [Test]
        public void QueenMovementValidatesRookPlusBishopLogic()
        {
            // Arrange - Create separate pieces to compare
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var queen = TestPieceFactory.CreateQueen(PlayerColor.White);
            var rook = TestPieceFactory.CreateRook(PlayerColor.White);
            var bishop = TestPieceFactory.CreateBishop(PlayerColor.White);
            
            var position = new Vector2Int(4, 4); // e5
            
            // Act - Get moves for each piece type
            var queenMoves = TestBoardHelper.GetPieceMovesFromPosition(queen, position, boardState);
            var rookMoves = TestBoardHelper.GetPieceMovesFromPosition(rook, position, boardState);
            var bishopMoves = TestBoardHelper.GetPieceMovesFromPosition(bishop, position, boardState);
            
            // Assert - Queen moves should include all rook and bishop moves
            foreach (var rookMove in rookMoves)
            {
                Assert.Contains(rookMove, queenMoves, $"Queen should include rook move to {TestBoardHelper.CoordsToChessNotation(rookMove)}");
            }
            
            foreach (var bishopMove in bishopMoves)
            {
                Assert.Contains(bishopMove, queenMoves, $"Queen should include bishop move to {TestBoardHelper.CoordsToChessNotation(bishopMove)}");
            }
            
            // Queen should have exactly the sum of rook and bishop moves
            Assert.AreEqual(rookMoves.Count + bishopMoves.Count, queenMoves.Count, "Queen should have exactly rook moves + bishop moves");
        }
        
        /// <summary>
        /// Test queen starting position has no moves (comprehensive check)
        /// </summary>
        [Test]
        public void QueenStartingPositionHasNoMoves()
        {
            // Arrange - Standard starting position
            var boardState = TestBoardHelper.CreateStandardStartingPosition();
            
            // Test both queens
            var whiteQueen = boardState[3, 0] as Queen; // d1
            var blackQueen = boardState[3, 7] as Queen; // d8
            
            // Act
            var whiteQueenMoves = TestBoardHelper.GetPieceMovesFromPosition(whiteQueen, new Vector2Int(3, 0), boardState);
            var blackQueenMoves = TestBoardHelper.GetPieceMovesFromPosition(blackQueen, new Vector2Int(3, 7), boardState);
            
            // Assert
            Assert.AreEqual(0, whiteQueenMoves.Count, "White queen should have 0 moves in starting position");
            Assert.AreEqual(0, blackQueenMoves.Count, "Black queen should have 0 moves in starting position");
            
            // Verify specific directions are blocked
            // For white queen at d1:
            // - Forward blocked by pawn at d2
            // - Diagonals blocked by bishop (c1) and king (e1)
            // - Left/right blocked by pieces
            
            var whiteQueenPos = new Vector2Int(3, 0);
            var blockedMoves = new List<Vector2Int>
            {
                new Vector2Int(3, 1), // d2 (pawn blocks)
                new Vector2Int(2, 1), // c2 (diagonal blocked)
                new Vector2Int(4, 1), // e2 (diagonal blocked)
                new Vector2Int(2, 0), // c1 (bishop blocks)
                new Vector2Int(4, 0), // e1 (king blocks)
            };
            
            foreach (var blockedMove in blockedMoves)
            {
                Assert.False(whiteQueenMoves.Contains(blockedMove), $"White queen should not be able to move to {TestBoardHelper.CoordsToChessNotation(blockedMove)} in starting position");
            }
        }
    }
}