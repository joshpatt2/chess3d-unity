using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Chess3D.Tests
{
    /// <summary>
    /// Tests for knight movement validation including specific opening moves
    /// </summary>
    public class KnightMovementTests
    {
        /// <summary>
        /// Test that white knights can move to valid L-shaped positions from starting position
        /// Specific moves: Nb1-c3, Nb1-a3, Ng1-f3, Ng1-h3
        /// </summary>
        [Test]
        public void WhiteKnightsCanMakeValidOpeningMoves()
        {
            // Arrange - Standard starting position
            var boardState = TestBoardHelper.CreateStandardStartingPosition();
            
            // Test Queenside Knight (b1)
            var queensideKnight = boardState[1, 0] as Knight; // b1
            Assert.IsNotNull(queensideKnight, "White queenside knight should exist at b1");
            
            var queensidePos = new Vector2Int(1, 0); // b1
            var queensideMoves = TestBoardHelper.GetPieceMovesFromPosition(queensideKnight, queensidePos, boardState);
            
            // Should be able to move to c3 and a3
            var c3 = TestBoardHelper.ChessNotationToCoords("c3");
            var a3 = TestBoardHelper.ChessNotationToCoords("a3");
            
            Assert.Contains(c3, queensideMoves, "White queenside knight should be able to move from b1 to c3");
            Assert.Contains(a3, queensideMoves, "White queenside knight should be able to move from b1 to a3");
            
            // Test Kingside Knight (g1)
            var kingsideKnight = boardState[6, 0] as Knight; // g1
            Assert.IsNotNull(kingsideKnight, "White kingside knight should exist at g1");
            
            var kingsidePos = new Vector2Int(6, 0); // g1
            var kingsideMoves = TestBoardHelper.GetPieceMovesFromPosition(kingsideKnight, kingsidePos, boardState);
            
            // Should be able to move to f3 and h3
            var f3 = TestBoardHelper.ChessNotationToCoords("f3");
            var h3 = TestBoardHelper.ChessNotationToCoords("h3");
            
            Assert.Contains(f3, kingsideMoves, "White kingside knight should be able to move from g1 to f3");
            Assert.Contains(h3, kingsideMoves, "White kingside knight should be able to move from g1 to h3");
        }
        
        /// <summary>
        /// Test that black knights can move to valid L-shaped positions from starting position
        /// </summary>
        [Test]
        public void BlackKnightsCanMakeValidOpeningMoves()
        {
            // Arrange - Standard starting position
            var boardState = TestBoardHelper.CreateStandardStartingPosition();
            
            // Test Black Queenside Knight (b8)
            var queensideKnight = boardState[1, 7] as Knight; // b8
            Assert.IsNotNull(queensideKnight, "Black queenside knight should exist at b8");
            
            var queensidePos = new Vector2Int(1, 7); // b8
            var queensideMoves = TestBoardHelper.GetPieceMovesFromPosition(queensideKnight, queensidePos, boardState);
            
            // Should be able to move to c6 and a6
            var c6 = TestBoardHelper.ChessNotationToCoords("c6");
            var a6 = TestBoardHelper.ChessNotationToCoords("a6");
            
            Assert.Contains(c6, queensideMoves, "Black queenside knight should be able to move from b8 to c6");
            Assert.Contains(a6, queensideMoves, "Black queenside knight should be able to move from b8 to a6");
            
            // Test Black Kingside Knight (g8)
            var kingsideKnight = boardState[6, 7] as Knight; // g8
            Assert.IsNotNull(kingsideKnight, "Black kingside knight should exist at g8");
            
            var kingsidePos = new Vector2Int(6, 7); // g8
            var kingsideMoves = TestBoardHelper.GetPieceMovesFromPosition(kingsideKnight, kingsidePos, boardState);
            
            // Should be able to move to f6 and h6
            var f6 = TestBoardHelper.ChessNotationToCoords("f6");
            var h6 = TestBoardHelper.ChessNotationToCoords("h6");
            
            Assert.Contains(f6, kingsideMoves, "Black kingside knight should be able to move from g8 to f6");
            Assert.Contains(h6, kingsideMoves, "Black kingside knight should be able to move from g8 to h6");
        }
        
        /// <summary>
        /// Test that knights can jump over other pieces
        /// </summary>
        [Test]
        public void KnightsCanJumpOverOtherPieces()
        {
            // Arrange - Place knight surrounded by pieces
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var knight = TestPieceFactory.CreateKnight(PlayerColor.White);
            var knightPos = new Vector2Int(4, 4); // e5 - center of board
            TestBoardHelper.PlacePiece(boardState, knight, knightPos.x, knightPos.y);
            
            // Surround knight with pieces
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue; // Skip knight's position
                    
                    var blockingPos = new Vector2Int(knightPos.x + dx, knightPos.y + dy);
                    if (blockingPos.x >= 0 && blockingPos.x < 8 && blockingPos.y >= 0 && blockingPos.y < 8)
                    {
                        var blockingPiece = TestPieceFactory.CreatePawn(PlayerColor.White);
                        TestBoardHelper.PlacePiece(boardState, blockingPiece, blockingPos.x, blockingPos.y);
                    }
                }
            }
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(knight, knightPos, boardState);
            
            // Assert - Knight should still be able to make all 8 L-shaped moves despite being surrounded
            // From e5, knight should be able to move to: d3, f3, c4, g4, c6, g6, d7, f7
            var expectedMoves = new List<Vector2Int>
            {
                new Vector2Int(3, 2), // d3
                new Vector2Int(5, 2), // f3
                new Vector2Int(2, 3), // c4
                new Vector2Int(6, 3), // g4
                new Vector2Int(2, 5), // c6
                new Vector2Int(6, 5), // g6
                new Vector2Int(3, 6), // d7
                new Vector2Int(5, 6)  // f7
            };
            
            Assert.AreEqual(expectedMoves.Count, moves.Count, "Knight should have 8 possible moves from center");
            
            foreach (var expectedMove in expectedMoves)
            {
                Assert.Contains(expectedMove, moves, $"Knight should be able to move to {TestBoardHelper.CoordsToChessNotation(expectedMove)} despite being surrounded");
            }
        }
        
        /// <summary>
        /// Test knight's complete L-shaped movement pattern
        /// </summary>
        [Test]
        public void KnightMakesCorrectLShapedMoves()
        {
            // Arrange - Knight in center of empty board
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var knight = TestPieceFactory.CreateKnight(PlayerColor.White);
            var knightPos = new Vector2Int(4, 4); // e5
            TestBoardHelper.PlacePiece(boardState, knight, knightPos.x, knightPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(knight, knightPos, boardState);
            
            // Assert - All 8 L-shaped moves from e5
            var expectedMoves = new List<Vector2Int>
            {
                new Vector2Int(2, 3), // c4 (2 left, 1 down)
                new Vector2Int(2, 5), // c6 (2 left, 1 up)
                new Vector2Int(6, 3), // g4 (2 right, 1 down)
                new Vector2Int(6, 5), // g6 (2 right, 1 up)
                new Vector2Int(3, 2), // d3 (1 left, 2 down)
                new Vector2Int(3, 6), // d7 (1 left, 2 up)
                new Vector2Int(5, 2), // f3 (1 right, 2 down)
                new Vector2Int(5, 6)  // f7 (1 right, 2 up)
            };
            
            Assert.AreEqual(expectedMoves.Count, moves.Count, "Knight should have exactly 8 moves from center");
            
            foreach (var expectedMove in expectedMoves)
            {
                Assert.Contains(expectedMove, moves, $"Knight should be able to move to {TestBoardHelper.CoordsToChessNotation(expectedMove)}");
            }
        }
        
        /// <summary>
        /// Test knight movement from corner positions (limited moves)
        /// </summary>
        [Test]
        public void KnightMovementFromCornerPositions()
        {
            // Test from a1 corner
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var knight = TestPieceFactory.CreateKnight(PlayerColor.White);
            var cornerPos = new Vector2Int(0, 0); // a1
            TestBoardHelper.PlacePiece(boardState, knight, cornerPos.x, cornerPos.y);
            
            var moves = TestBoardHelper.GetPieceMovesFromPosition(knight, cornerPos, boardState);
            
            // From a1, knight can only move to b3 and c2
            var expectedCornerMoves = new List<Vector2Int>
            {
                new Vector2Int(1, 2), // b3
                new Vector2Int(2, 1)  // c2
            };
            
            Assert.AreEqual(expectedCornerMoves.Count, moves.Count, "Knight from a1 should have exactly 2 moves");
            
            foreach (var expectedMove in expectedCornerMoves)
            {
                Assert.Contains(expectedMove, moves, $"Knight should be able to move to {TestBoardHelper.CoordsToChessNotation(expectedMove)} from a1");
            }
        }
        
        /// <summary>
        /// Test knight can capture enemy pieces but not friendly pieces
        /// </summary>
        [Test]
        public void KnightCanCaptureEnemyButNotFriendlyPieces()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var knight = TestPieceFactory.CreateKnight(PlayerColor.White);
            var knightPos = new Vector2Int(4, 4); // e5
            TestBoardHelper.PlacePiece(boardState, knight, knightPos.x, knightPos.y);
            
            // Place enemy piece at f3
            var enemyPiece = TestPieceFactory.CreatePawn(PlayerColor.Black);
            var enemyPos = new Vector2Int(5, 2); // f3
            TestBoardHelper.PlacePiece(boardState, enemyPiece, enemyPos.x, enemyPos.y);
            
            // Place friendly piece at d3
            var friendlyPiece = TestPieceFactory.CreatePawn(PlayerColor.White);
            var friendlyPos = new Vector2Int(3, 2); // d3
            TestBoardHelper.PlacePiece(boardState, friendlyPiece, friendlyPos.x, friendlyPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(knight, knightPos, boardState);
            
            // Assert
            Assert.Contains(enemyPos, moves, "Knight should be able to capture enemy piece at f3");
            Assert.False(moves.Contains(friendlyPos), "Knight should not be able to capture friendly piece at d3");
        }
        
        /// <summary>
        /// Test knight movement count from starting positions matches expected
        /// </summary>
        [Test]
        public void KnightStartingPositionMoveCount()
        {
            // Arrange - Standard starting position
            var boardState = TestBoardHelper.CreateStandardStartingPosition();
            
            // Test white knights - each should have exactly 2 moves from starting position
            var whiteQueensideKnight = boardState[1, 0] as Knight; // b1
            var whiteQueensideMoves = TestBoardHelper.GetPieceMovesFromPosition(whiteQueensideKnight, new Vector2Int(1, 0), boardState);
            Assert.AreEqual(2, whiteQueensideMoves.Count, "White queenside knight should have exactly 2 moves from starting position");
            
            var whiteKingsideKnight = boardState[6, 0] as Knight; // g1
            var whiteKingsideMoves = TestBoardHelper.GetPieceMovesFromPosition(whiteKingsideKnight, new Vector2Int(6, 0), boardState);
            Assert.AreEqual(2, whiteKingsideMoves.Count, "White kingside knight should have exactly 2 moves from starting position");
            
            // Test black knights - each should have exactly 2 moves from starting position
            var blackQueensideKnight = boardState[1, 7] as Knight; // b8
            var blackQueensideMoves = TestBoardHelper.GetPieceMovesFromPosition(blackQueensideKnight, new Vector2Int(1, 7), boardState);
            Assert.AreEqual(2, blackQueensideMoves.Count, "Black queenside knight should have exactly 2 moves from starting position");
            
            var blackKingsideKnight = boardState[6, 7] as Knight; // g8
            var blackKingsideMoves = TestBoardHelper.GetPieceMovesFromPosition(blackKingsideKnight, new Vector2Int(6, 7), boardState);
            Assert.AreEqual(2, blackKingsideMoves.Count, "Black kingside knight should have exactly 2 moves from starting position");
        }
    }
}