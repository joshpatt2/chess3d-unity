using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Chess3D.Tests
{
    /// <summary>
    /// Tests for bishop movement validation including diagonal movement
    /// </summary>
    public class BishopMovementTests
    {
        /// <summary>
        /// Test that bishops cannot move initially (blocked by pawns)
        /// </summary>
        [Test]
        public void BishopsCannotMoveInitiallyBlockedByPawns()
        {
            // Arrange - Standard starting position
            var boardState = TestBoardHelper.CreateStandardStartingPosition();
            
            // Test white bishops
            var whiteQueensideBishop = boardState[2, 0] as Bishop; // c1
            Assert.IsNotNull(whiteQueensideBishop, "White queenside bishop should exist at c1");
            
            var whiteQueensideMoves = TestBoardHelper.GetPieceMovesFromPosition(whiteQueensideBishop, new Vector2Int(2, 0), boardState);
            Assert.AreEqual(0, whiteQueensideMoves.Count, "White queenside bishop should have no moves when blocked by pawns");
            
            var whiteKingsideBishop = boardState[5, 0] as Bishop; // f1
            Assert.IsNotNull(whiteKingsideBishop, "White kingside bishop should exist at f1");
            
            var whiteKingsideMoves = TestBoardHelper.GetPieceMovesFromPosition(whiteKingsideBishop, new Vector2Int(5, 0), boardState);
            Assert.AreEqual(0, whiteKingsideMoves.Count, "White kingside bishop should have no moves when blocked by pawns");
            
            // Test black bishops
            var blackQueensideBishop = boardState[2, 7] as Bishop; // c8
            Assert.IsNotNull(blackQueensideBishop, "Black queenside bishop should exist at c8");
            
            var blackQueensideMoves = TestBoardHelper.GetPieceMovesFromPosition(blackQueensideBishop, new Vector2Int(2, 7), boardState);
            Assert.AreEqual(0, blackQueensideMoves.Count, "Black queenside bishop should have no moves when blocked by pawns");
            
            var blackKingsideBishop = boardState[5, 7] as Bishop; // f8
            Assert.IsNotNull(blackKingsideBishop, "Black kingside bishop should exist at f8");
            
            var blackKingsideMoves = TestBoardHelper.GetPieceMovesFromPosition(blackKingsideBishop, new Vector2Int(5, 7), boardState);
            Assert.AreEqual(0, blackKingsideMoves.Count, "Black kingside bishop should have no moves when blocked by pawns");
        }
        
        /// <summary>
        /// Test bishop diagonal movement on empty board
        /// </summary>
        [Test]
        public void BishopCanMoveDiagonallyOnEmptyBoard()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var bishop = TestPieceFactory.CreateBishop(PlayerColor.White);
            var bishopPos = new Vector2Int(4, 4); // e5 - center of board
            TestBoardHelper.PlacePiece(boardState, bishop, bishopPos.x, bishopPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(bishop, bishopPos, boardState);
            
            // Assert - Should be able to move diagonally in all 4 directions
            var expectedDiagonalMoves = new List<Vector2Int>
            {
                // Up-right diagonal
                new Vector2Int(5, 5), // f6
                new Vector2Int(6, 6), // g7
                new Vector2Int(7, 7), // h8
                
                // Up-left diagonal
                new Vector2Int(3, 5), // d6
                new Vector2Int(2, 6), // c7
                new Vector2Int(1, 7), // b8
                
                // Down-right diagonal
                new Vector2Int(5, 3), // f4
                new Vector2Int(6, 2), // g3
                new Vector2Int(7, 1), // h2
                
                // Down-left diagonal
                new Vector2Int(3, 3), // d4
                new Vector2Int(2, 2), // c3
                new Vector2Int(1, 1), // b2
                new Vector2Int(0, 0)  // a1
            };
            
            foreach (var expectedMove in expectedDiagonalMoves)
            {
                Assert.Contains(expectedMove, moves, $"Bishop should be able to move diagonally to {TestBoardHelper.CoordsToChessNotation(expectedMove)}");
            }
            
            Assert.AreEqual(13, moves.Count, "Bishop should have 13 moves from center of empty board");
        }
        
        /// <summary>
        /// Test that bishop cannot move horizontally or vertically
        /// </summary>
        [Test]
        public void BishopCannotMoveHorizontallyOrVertically()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var bishop = TestPieceFactory.CreateBishop(PlayerColor.White);
            var bishopPos = new Vector2Int(4, 4); // e5
            TestBoardHelper.PlacePiece(boardState, bishop, bishopPos.x, bishopPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(bishop, bishopPos, boardState);
            
            // Assert - Should not contain any horizontal or vertical moves
            var nonDiagonalMoves = new List<Vector2Int>
            {
                // Horizontal moves
                new Vector2Int(0, 4), // a5
                new Vector2Int(1, 4), // b5
                new Vector2Int(2, 4), // c5
                new Vector2Int(3, 4), // d5
                new Vector2Int(5, 4), // f5
                new Vector2Int(6, 4), // g5
                new Vector2Int(7, 4), // h5
                
                // Vertical moves
                new Vector2Int(4, 0), // e1
                new Vector2Int(4, 1), // e2
                new Vector2Int(4, 2), // e3
                new Vector2Int(4, 3), // e4
                new Vector2Int(4, 5), // e6
                new Vector2Int(4, 6), // e7
                new Vector2Int(4, 7)  // e8
            };
            
            foreach (var nonDiagonalMove in nonDiagonalMoves)
            {
                Assert.False(moves.Contains(nonDiagonalMove), $"Bishop should not be able to move horizontally/vertically to {TestBoardHelper.CoordsToChessNotation(nonDiagonalMove)}");
            }
        }
        
        /// <summary>
        /// Test bishop is blocked by friendly pieces
        /// </summary>
        [Test]
        public void BishopIsBlockedByFriendlyPieces()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var bishop = TestPieceFactory.CreateBishop(PlayerColor.White);
            var bishopPos = new Vector2Int(4, 4); // e5
            TestBoardHelper.PlacePiece(boardState, bishop, bishopPos.x, bishopPos.y);
            
            // Place friendly pieces to block diagonal movement
            var friendlyPiece1 = TestPieceFactory.CreatePawn(PlayerColor.White);
            var friendlyPiece2 = TestPieceFactory.CreatePawn(PlayerColor.White);
            TestBoardHelper.PlacePiece(boardState, friendlyPiece1, 6, 6); // g7 (blocks up-right)
            TestBoardHelper.PlacePiece(boardState, friendlyPiece2, 2, 2); // c3 (blocks down-left)
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(bishop, bishopPos, boardState);
            
            // Assert - Should not be able to reach blocked squares or beyond
            Assert.False(moves.Contains(new Vector2Int(6, 6)), "Bishop should not be able to move to square occupied by friendly piece");
            Assert.False(moves.Contains(new Vector2Int(7, 7)), "Bishop should not be able to move beyond friendly piece");
            Assert.False(moves.Contains(new Vector2Int(2, 2)), "Bishop should not be able to move to square occupied by friendly piece");
            Assert.False(moves.Contains(new Vector2Int(1, 1)), "Bishop should not be able to move beyond friendly piece");
            Assert.False(moves.Contains(new Vector2Int(0, 0)), "Bishop should not be able to move beyond friendly piece");
            
            // Should still be able to move to squares before the blocking pieces
            Assert.Contains(new Vector2Int(5, 5), moves, "Bishop should be able to move to square before friendly piece"); // f6
            Assert.Contains(new Vector2Int(3, 3), moves, "Bishop should be able to move to square before friendly piece"); // d4
        }
        
        /// <summary>
        /// Test bishop can capture enemy pieces but cannot move beyond them
        /// </summary>
        [Test]
        public void BishopCanCaptureEnemyPiecesButCannotMoveBeyond()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var bishop = TestPieceFactory.CreateBishop(PlayerColor.White);
            var bishopPos = new Vector2Int(4, 4); // e5
            TestBoardHelper.PlacePiece(boardState, bishop, bishopPos.x, bishopPos.y);
            
            // Place enemy pieces
            var enemyPiece1 = TestPieceFactory.CreatePawn(PlayerColor.Black);
            var enemyPiece2 = TestPieceFactory.CreatePawn(PlayerColor.Black);
            TestBoardHelper.PlacePiece(boardState, enemyPiece1, 6, 6); // g7 (blocks up-right)
            TestBoardHelper.PlacePiece(boardState, enemyPiece2, 2, 2); // c3 (blocks down-left)
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(bishop, bishopPos, boardState);
            
            // Assert - Should be able to capture enemy pieces
            Assert.Contains(new Vector2Int(6, 6), moves, "Bishop should be able to capture enemy piece at g7");
            Assert.Contains(new Vector2Int(2, 2), moves, "Bishop should be able to capture enemy piece at c3");
            
            // Should not be able to move beyond captured pieces
            Assert.False(moves.Contains(new Vector2Int(7, 7)), "Bishop should not be able to move beyond captured enemy piece"); // h8
            Assert.False(moves.Contains(new Vector2Int(1, 1)), "Bishop should not be able to move beyond captured enemy piece"); // b2
            Assert.False(moves.Contains(new Vector2Int(0, 0)), "Bishop should not be able to move beyond captured enemy piece"); // a1
        }
        
        /// <summary>
        /// Test bishop movement from corner positions
        /// </summary>
        [Test]
        public void BishopMovementFromCornerPositions()
        {
            // Test from a1 corner
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var bishop = TestPieceFactory.CreateBishop(PlayerColor.White);
            var cornerPos = new Vector2Int(0, 0); // a1
            TestBoardHelper.PlacePiece(boardState, bishop, cornerPos.x, cornerPos.y);
            
            var moves = TestBoardHelper.GetPieceMovesFromPosition(bishop, cornerPos, boardState);
            
            // From a1, can only move up-right diagonal
            var expectedMoves = new List<Vector2Int>
            {
                new Vector2Int(1, 1), // b2
                new Vector2Int(2, 2), // c3
                new Vector2Int(3, 3), // d4
                new Vector2Int(4, 4), // e5
                new Vector2Int(5, 5), // f6
                new Vector2Int(6, 6), // g7
                new Vector2Int(7, 7)  // h8
            };
            
            Assert.AreEqual(expectedMoves.Count, moves.Count, "Bishop should have 7 moves from a1 corner");
            
            foreach (var expectedMove in expectedMoves)
            {
                Assert.Contains(expectedMove, moves, $"Bishop should be able to move to {TestBoardHelper.CoordsToChessNotation(expectedMove)} from a1");
            }
        }
        
        /// <summary>
        /// Test bishop movement from edge positions
        /// </summary>
        [Test]
        public void BishopMovementFromEdgePositions()
        {
            // Test from a4 (left edge)
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var bishop = TestPieceFactory.CreateBishop(PlayerColor.White);
            var edgePos = new Vector2Int(0, 3); // a4
            TestBoardHelper.PlacePiece(boardState, bishop, edgePos.x, edgePos.y);
            
            var moves = TestBoardHelper.GetPieceMovesFromPosition(bishop, edgePos, boardState);
            
            // Should be able to move along both diagonals available from edge
            // Up-right: b5, c6, d7, e8
            // Down-right: b3, c2, d1
            Assert.AreEqual(7, moves.Count, "Bishop should have 7 moves from a4 edge position");
            
            Assert.Contains(new Vector2Int(1, 4), moves, "Bishop should be able to move to b5");
            Assert.Contains(new Vector2Int(4, 7), moves, "Bishop should be able to move to e8");
            Assert.Contains(new Vector2Int(1, 2), moves, "Bishop should be able to move to b3");
            Assert.Contains(new Vector2Int(3, 0), moves, "Bishop should be able to move to d1");
        }
        
        /// <summary>
        /// Test bishop diagonal movement logic is present and working correctly
        /// This validates the core sliding piece logic for diagonal movement
        /// </summary>
        [Test]
        public void BishopDiagonalMovementLogicIsCorrect()
        {
            // Test from center position to verify all four diagonals work
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var bishop = TestPieceFactory.CreateBishop(PlayerColor.White);
            var centerPos = new Vector2Int(3, 3); // d4
            TestBoardHelper.PlacePiece(boardState, bishop, centerPos.x, centerPos.y);
            
            var moves = TestBoardHelper.GetPieceMovesFromPosition(bishop, centerPos, boardState);
            
            // Verify each diagonal direction
            // Up-right diagonal
            Assert.Contains(new Vector2Int(4, 4), moves, "Bishop should move up-right to e5");
            Assert.Contains(new Vector2Int(7, 7), moves, "Bishop should move up-right to h8");
            
            // Up-left diagonal
            Assert.Contains(new Vector2Int(2, 4), moves, "Bishop should move up-left to c5");
            Assert.Contains(new Vector2Int(0, 6), moves, "Bishop should move up-left to a7");
            
            // Down-right diagonal
            Assert.Contains(new Vector2Int(4, 2), moves, "Bishop should move down-right to e3");
            Assert.Contains(new Vector2Int(6, 0), moves, "Bishop should move down-right to g1");
            
            // Down-left diagonal
            Assert.Contains(new Vector2Int(2, 2), moves, "Bishop should move down-left to c3");
            Assert.Contains(new Vector2Int(0, 0), moves, "Bishop should move down-left to a1");
            
            Assert.AreEqual(13, moves.Count, "Bishop should have 13 moves from d4");
        }
        
        /// <summary>
        /// Test that bishop stays on same colored squares
        /// </summary>
        [Test]
        public void BishopStaysOnSameColoredSquares()
        {
            // Light-squared bishop test (starts on light square)
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var bishop = TestPieceFactory.CreateBishop(PlayerColor.White);
            var lightSquarePos = new Vector2Int(2, 0); // c1 (light square: 2+0=2, even)
            TestBoardHelper.PlacePiece(boardState, bishop, lightSquarePos.x, lightSquarePos.y);
            
            var moves = TestBoardHelper.GetPieceMovesFromPosition(bishop, lightSquarePos, boardState);
            
            // All moves should be on light squares (sum of coordinates is even)
            foreach (var move in moves)
            {
                var squareColor = (move.x + move.y) % 2;
                Assert.AreEqual(0, squareColor, $"Bishop on light square should only move to light squares, but {TestBoardHelper.CoordsToChessNotation(move)} is dark");
            }
            
            // Dark-squared bishop test (starts on dark square)
            boardState = TestBoardHelper.CreateEmptyBoard();
            var darkSquarePos = new Vector2Int(5, 0); // f1 (dark square: 5+0=5, odd)
            TestBoardHelper.PlacePiece(boardState, bishop, darkSquarePos.x, darkSquarePos.y);
            
            moves = TestBoardHelper.GetPieceMovesFromPosition(bishop, darkSquarePos, boardState);
            
            // All moves should be on dark squares (sum of coordinates is odd)
            foreach (var move in moves)
            {
                var squareColor = (move.x + move.y) % 2;
                Assert.AreEqual(1, squareColor, $"Bishop on dark square should only move to dark squares, but {TestBoardHelper.CoordsToChessNotation(move)} is light");
            }
        }
    }
}