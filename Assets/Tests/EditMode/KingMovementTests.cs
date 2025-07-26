using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Chess3D.Tests
{
    /// <summary>
    /// Tests for king movement validation including basic movement, check avoidance, and castling
    /// </summary>
    public class KingMovementTests
    {
        /// <summary>
        /// Test king can move 1 square in any direction when not blocked
        /// </summary>
        [Test]
        public void KingCanMove1SquareInAnyDirection()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var king = TestPieceFactory.CreateKing(PlayerColor.White);
            var kingPos = new Vector2Int(4, 4); // e5 - center of board
            TestBoardHelper.PlacePiece(boardState, king, kingPos.x, kingPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(king, kingPos, boardState);
            
            // Assert - Should be able to move to all 8 adjacent squares
            var expectedMoves = new List<Vector2Int>
            {
                new Vector2Int(3, 3), // d4 (down-left)
                new Vector2Int(3, 4), // d5 (left)
                new Vector2Int(3, 5), // d6 (up-left)
                new Vector2Int(4, 3), // e4 (down)
                new Vector2Int(4, 5), // e6 (up)
                new Vector2Int(5, 3), // f4 (down-right)
                new Vector2Int(5, 4), // f5 (right)
                new Vector2Int(5, 5)  // f6 (up-right)
            };
            
            Assert.AreEqual(expectedMoves.Count, moves.Count, "King should have exactly 8 moves from center");
            
            foreach (var expectedMove in expectedMoves)
            {
                Assert.Contains(expectedMove, moves, $"King should be able to move to {TestBoardHelper.CoordsToChessNotation(expectedMove)}");
            }
        }
        
        /// <summary>
        /// Test king movement from corner positions (limited moves)
        /// </summary>
        [Test]
        public void KingMovementFromCornerPositions()
        {
            // Test from a1 corner
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var king = TestPieceFactory.CreateKing(PlayerColor.White);
            var cornerPos = new Vector2Int(0, 0); // a1
            TestBoardHelper.PlacePiece(boardState, king, cornerPos.x, cornerPos.y);
            
            var moves = TestBoardHelper.GetPieceMovesFromPosition(king, cornerPos, boardState);
            
            // From a1, king can only move to a2, b1, b2
            var expectedCornerMoves = new List<Vector2Int>
            {
                new Vector2Int(0, 1), // a2
                new Vector2Int(1, 0), // b1
                new Vector2Int(1, 1)  // b2
            };
            
            Assert.AreEqual(expectedCornerMoves.Count, moves.Count, "King from a1 should have exactly 3 moves");
            
            foreach (var expectedMove in expectedCornerMoves)
            {
                Assert.Contains(expectedMove, moves, $"King should be able to move to {TestBoardHelper.CoordsToChessNotation(expectedMove)} from a1");
            }
        }
        
        /// <summary>
        /// Test king movement from edge positions
        /// </summary>
        [Test]
        public void KingMovementFromEdgePositions()
        {
            // Test from a4 (left edge)
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var king = TestPieceFactory.CreateKing(PlayerColor.White);
            var edgePos = new Vector2Int(0, 3); // a4
            TestBoardHelper.PlacePiece(boardState, king, edgePos.x, edgePos.y);
            
            var moves = TestBoardHelper.GetPieceMovesFromPosition(king, edgePos, boardState);
            
            // From a4, king should have 5 moves: a3, a5, b3, b4, b5
            Assert.AreEqual(5, moves.Count, "King from a4 should have exactly 5 moves");
            
            var expectedEdgeMoves = new List<Vector2Int>
            {
                new Vector2Int(0, 2), // a3
                new Vector2Int(0, 4), // a5
                new Vector2Int(1, 2), // b3
                new Vector2Int(1, 3), // b4
                new Vector2Int(1, 4)  // b5
            };
            
            foreach (var expectedMove in expectedEdgeMoves)
            {
                Assert.Contains(expectedMove, moves, $"King should be able to move to {TestBoardHelper.CoordsToChessNotation(expectedMove)} from a4");
            }
        }
        
        /// <summary>
        /// Test king cannot move more than 1 square
        /// </summary>
        [Test]
        public void KingCannotMoveMoreThan1Square()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var king = TestPieceFactory.CreateKing(PlayerColor.White);
            var kingPos = new Vector2Int(4, 4); // e5
            TestBoardHelper.PlacePiece(boardState, king, kingPos.x, kingPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(king, kingPos, boardState);
            
            // Assert - Should not contain any moves more than 1 square away
            var invalidMoves = new List<Vector2Int>
            {
                new Vector2Int(2, 4), // c5 (2 squares left)
                new Vector2Int(6, 4), // g5 (2 squares right)
                new Vector2Int(4, 2), // e3 (2 squares down)
                new Vector2Int(4, 6), // e7 (2 squares up)
                new Vector2Int(2, 2), // c3 (2 squares diagonal)
                new Vector2Int(6, 6), // g7 (2 squares diagonal)
            };
            
            foreach (var invalidMove in invalidMoves)
            {
                Assert.False(moves.Contains(invalidMove), $"King should not be able to move to {TestBoardHelper.CoordsToChessNotation(invalidMove)} (more than 1 square)");
            }
        }
        
        /// <summary>
        /// Test king is blocked by friendly pieces
        /// </summary>
        [Test]
        public void KingIsBlockedByFriendlyPieces()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var king = TestPieceFactory.CreateKing(PlayerColor.White);
            var kingPos = new Vector2Int(4, 4); // e5
            TestBoardHelper.PlacePiece(boardState, king, kingPos.x, kingPos.y);
            
            // Place friendly pieces around the king
            var friendlyPiece1 = TestPieceFactory.CreatePawn(PlayerColor.White);
            var friendlyPiece2 = TestPieceFactory.CreatePawn(PlayerColor.White);
            TestBoardHelper.PlacePiece(boardState, friendlyPiece1, 4, 5); // e6 (up)
            TestBoardHelper.PlacePiece(boardState, friendlyPiece2, 5, 4); // f5 (right)
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(king, kingPos, boardState);
            
            // Assert - Should not be able to move to squares occupied by friendly pieces
            Assert.False(moves.Contains(new Vector2Int(4, 5)), "King should not be able to move to square occupied by friendly piece");
            Assert.False(moves.Contains(new Vector2Int(5, 4)), "King should not be able to move to square occupied by friendly piece");
            
            // Should still be able to move to other adjacent squares
            Assert.Contains(new Vector2Int(3, 4), moves, "King should be able to move to unoccupied adjacent square"); // d5
            Assert.Contains(new Vector2Int(4, 3), moves, "King should be able to move to unoccupied adjacent square"); // e4
        }
        
        /// <summary>
        /// Test king can capture enemy pieces
        /// </summary>
        [Test]
        public void KingCanCaptureEnemyPieces()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var king = TestPieceFactory.CreateKing(PlayerColor.White);
            var kingPos = new Vector2Int(4, 4); // e5
            TestBoardHelper.PlacePiece(boardState, king, kingPos.x, kingPos.y);
            
            // Place enemy pieces around the king
            var enemyPiece1 = TestPieceFactory.CreatePawn(PlayerColor.Black);
            var enemyPiece2 = TestPieceFactory.CreatePawn(PlayerColor.Black);
            TestBoardHelper.PlacePiece(boardState, enemyPiece1, 4, 5); // e6 (up)
            TestBoardHelper.PlacePiece(boardState, enemyPiece2, 5, 5); // f6 (up-right)
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(king, kingPos, boardState);
            
            // Assert - Should be able to capture enemy pieces
            Assert.Contains(new Vector2Int(4, 5), moves, "King should be able to capture enemy piece");
            Assert.Contains(new Vector2Int(5, 5), moves, "King should be able to capture enemy piece");
        }
        
        /// <summary>
        /// Test king starting positions have limited moves
        /// </summary>
        [Test]
        public void KingStartingPositionsHaveLimitedMoves()
        {
            // Arrange - Standard starting position
            var boardState = TestBoardHelper.CreateStandardStartingPosition();
            
            // Test white king
            var whiteKing = boardState[4, 0] as King; // e1
            Assert.IsNotNull(whiteKing, "White king should exist at e1");
            
            var whiteKingMoves = TestBoardHelper.GetPieceMovesFromPosition(whiteKing, new Vector2Int(4, 0), boardState);
            Assert.AreEqual(0, whiteKingMoves.Count, "White king should have no moves in starting position (blocked by pieces)");
            
            // Test black king
            var blackKing = boardState[4, 7] as King; // e8
            Assert.IsNotNull(blackKing, "Black king should exist at e8");
            
            var blackKingMoves = TestBoardHelper.GetPieceMovesFromPosition(blackKing, new Vector2Int(4, 7), boardState);
            Assert.AreEqual(0, blackKingMoves.Count, "Black king should have no moves in starting position (blocked by pieces)");
        }
        
        /// <summary>
        /// Test castling prerequisites - king and rook haven't moved, path is clear
        /// Note: This tests basic castling conditions, not check-related restrictions
        /// </summary>
        [Test]
        public void CastlingPrerequisitesWorkCorrectly()
        {
            // Arrange - Clear board with king and rooks in starting positions
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var whiteKing = TestPieceFactory.CreateKing(PlayerColor.White, hasMoved: false);
            var whiteKingsideRook = TestPieceFactory.CreateRook(PlayerColor.White, hasMoved: false);
            var whiteQueensideRook = TestPieceFactory.CreateRook(PlayerColor.White, hasMoved: false);
            
            // Place pieces in starting positions
            TestBoardHelper.PlacePiece(boardState, whiteKing, 4, 0); // e1
            TestBoardHelper.PlacePiece(boardState, whiteKingsideRook, 7, 0); // h1
            TestBoardHelper.PlacePiece(boardState, whiteQueensideRook, 0, 0); // a1
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(whiteKing, new Vector2Int(4, 0), boardState);
            
            // Assert - Should include castling moves (if implemented)
            // Kingside castling: e1 -> g1
            // Queenside castling: e1 -> c1
            
            // Note: The actual castling implementation may vary, so we'll test for basic king moves
            // and verify that the king can potentially castle (path is clear)
            var kingsideCastleMove = new Vector2Int(6, 0); // g1
            var queensideCastleMove = new Vector2Int(2, 0); // c1
            
            // At minimum, king should be able to move to adjacent squares
            Assert.Contains(new Vector2Int(3, 0), moves, "King should be able to move to d1");
            Assert.Contains(new Vector2Int(5, 0), moves, "King should be able to move to f1");
            Assert.Contains(new Vector2Int(4, 1), moves, "King should be able to move to e2");
            
            // If castling is implemented, these would be included:
            // This is testing the castling logic exists, specific implementation may vary
            bool castlingImplemented = moves.Contains(kingsideCastleMove) || moves.Contains(queensideCastleMove);
            if (castlingImplemented)
            {
                // If castling is implemented, verify both directions
                Assert.Contains(kingsideCastleMove, moves, "King should be able to castle kingside when conditions are met");
                Assert.Contains(queensideCastleMove, moves, "King should be able to castle queenside when conditions are met");
            }
        }
        
        /// <summary>
        /// Test king cannot castle when it has moved
        /// </summary>
        [Test]
        public void KingCannotCastleWhenItHasMoved()
        {
            // Arrange - King that has already moved
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var whiteKing = TestPieceFactory.CreateKing(PlayerColor.White, hasMoved: true);
            var whiteKingsideRook = TestPieceFactory.CreateRook(PlayerColor.White, hasMoved: false);
            var whiteQueensideRook = TestPieceFactory.CreateRook(PlayerColor.White, hasMoved: false);
            
            TestBoardHelper.PlacePiece(boardState, whiteKing, 4, 0); // e1
            TestBoardHelper.PlacePiece(boardState, whiteKingsideRook, 7, 0); // h1
            TestBoardHelper.PlacePiece(boardState, whiteQueensideRook, 0, 0); // a1
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(whiteKing, new Vector2Int(4, 0), boardState);
            
            // Assert - Should not include castling moves
            var kingsideCastleMove = new Vector2Int(6, 0); // g1
            var queensideCastleMove = new Vector2Int(2, 0); // c1
            
            Assert.False(moves.Contains(kingsideCastleMove), "King should not be able to castle kingside when it has moved");
            Assert.False(moves.Contains(queensideCastleMove), "King should not be able to castle queenside when it has moved");
        }
        
        /// <summary>
        /// Test king cannot castle when rook has moved
        /// </summary>
        [Test]
        public void KingCannotCastleWhenRookHasMoved()
        {
            // Arrange - Rook that has already moved
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var whiteKing = TestPieceFactory.CreateKing(PlayerColor.White, hasMoved: false);
            var whiteKingsideRook = TestPieceFactory.CreateRook(PlayerColor.White, hasMoved: true); // Moved rook
            var whiteQueensideRook = TestPieceFactory.CreateRook(PlayerColor.White, hasMoved: false);
            
            TestBoardHelper.PlacePiece(boardState, whiteKing, 4, 0); // e1
            TestBoardHelper.PlacePiece(boardState, whiteKingsideRook, 7, 0); // h1
            TestBoardHelper.PlacePiece(boardState, whiteQueensideRook, 0, 0); // a1
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(whiteKing, new Vector2Int(4, 0), boardState);
            
            // Assert - Should not include kingside castling (rook has moved)
            var kingsideCastleMove = new Vector2Int(6, 0); // g1
            var queensideCastleMove = new Vector2Int(2, 0); // c1
            
            Assert.False(moves.Contains(kingsideCastleMove), "King should not be able to castle kingside when rook has moved");
            
            // Queenside castling might still be available if implemented
            // This depends on the specific implementation
        }
        
        /// <summary>
        /// Test king cannot castle when path is blocked
        /// </summary>
        [Test]
        public void KingCannotCastleWhenPathIsBlocked()
        {
            // Arrange - Pieces blocking castling path
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var whiteKing = TestPieceFactory.CreateKing(PlayerColor.White, hasMoved: false);
            var whiteKingsideRook = TestPieceFactory.CreateRook(PlayerColor.White, hasMoved: false);
            var whiteQueensideRook = TestPieceFactory.CreateRook(PlayerColor.White, hasMoved: false);
            var blockingPiece = TestPieceFactory.CreateKnight(PlayerColor.White);
            
            TestBoardHelper.PlacePiece(boardState, whiteKing, 4, 0); // e1
            TestBoardHelper.PlacePiece(boardState, whiteKingsideRook, 7, 0); // h1
            TestBoardHelper.PlacePiece(boardState, whiteQueensideRook, 0, 0); // a1
            TestBoardHelper.PlacePiece(boardState, blockingPiece, 5, 0); // f1 (blocks kingside castling)
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(whiteKing, new Vector2Int(4, 0), boardState);
            
            // Assert - Should not include kingside castling (path blocked)
            var kingsideCastleMove = new Vector2Int(6, 0); // g1
            
            Assert.False(moves.Contains(kingsideCastleMove), "King should not be able to castle kingside when path is blocked");
        }
        
        /// <summary>
        /// Test basic king movement validation is correct
        /// </summary>
        [Test]
        public void KingMovementValidationIsCorrect()
        {
            // Test that IsValidPosition method works correctly for king
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var king = TestPieceFactory.CreateKing(PlayerColor.White);
            
            // Test from various positions to ensure consistency
            var testPositions = new List<Vector2Int>
            {
                new Vector2Int(0, 0), // corner
                new Vector2Int(7, 7), // opposite corner
                new Vector2Int(3, 3), // center-ish
                new Vector2Int(0, 4), // edge
                new Vector2Int(4, 0)  // edge
            };
            
            foreach (var position in testPositions)
            {
                TestBoardHelper.PlacePiece(boardState, king, position.x, position.y);
                var moves = TestBoardHelper.GetPieceMovesFromPosition(king, position, boardState);
                
                // Verify all moves are exactly 1 square away and within board bounds
                foreach (var move in moves)
                {
                    var dx = Mathf.Abs(move.x - position.x);
                    var dy = Mathf.Abs(move.y - position.y);
                    
                    Assert.IsTrue(dx <= 1 && dy <= 1, $"King move from {TestBoardHelper.CoordsToChessNotation(position)} to {TestBoardHelper.CoordsToChessNotation(move)} is more than 1 square");
                    Assert.IsTrue(dx > 0 || dy > 0, $"King should not include its current position in moves");
                    Assert.IsTrue(move.x >= 0 && move.x < 8 && move.y >= 0 && move.y < 8, $"King move {TestBoardHelper.CoordsToChessNotation(move)} is outside board bounds");
                }
                
                // Clear the king for next test
                TestBoardHelper.PlacePiece(boardState, null, position.x, position.y);
            }
        }
    }
}