using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Chess3D.Tests
{
    /// <summary>
    /// Tests for pawn movement validation from starting positions
    /// </summary>
    public class PawnMovementTests
    {
        /// <summary>
        /// Test that white pawn can move 1 square forward from starting position
        /// </summary>
        [Test]
        public void WhitePawnCanMove1SquareForwardFromStart()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var whitePawn = TestPieceFactory.CreatePawn(PlayerColor.White, hasMoved: false);
            var startPos = new Vector2Int(4, 1); // e2
            TestBoardHelper.PlacePiece(boardState, whitePawn, startPos.x, startPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(whitePawn, startPos, boardState);
            
            // Assert
            var oneForward = new Vector2Int(4, 2); // e3
            Assert.Contains(oneForward, moves, "White pawn should be able to move 1 square forward from starting position");
        }
        
        /// <summary>
        /// Test that white pawn can move 2 squares forward from starting position (first move only)
        /// </summary>
        [Test]
        public void WhitePawnCanMove2SquaresForwardFromStart()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var whitePawn = TestPieceFactory.CreatePawn(PlayerColor.White, hasMoved: false);
            var startPos = new Vector2Int(4, 1); // e2
            TestBoardHelper.PlacePiece(boardState, whitePawn, startPos.x, startPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(whitePawn, startPos, boardState);
            
            // Assert
            var twoForward = new Vector2Int(4, 3); // e4
            Assert.Contains(twoForward, moves, "White pawn should be able to move 2 squares forward from starting position");
        }
        
        /// <summary>
        /// Test that black pawn can move 1 square forward from starting position
        /// </summary>
        [Test]
        public void BlackPawnCanMove1SquareForwardFromStart()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var blackPawn = TestPieceFactory.CreatePawn(PlayerColor.Black, hasMoved: false);
            var startPos = new Vector2Int(4, 6); // e7
            TestBoardHelper.PlacePiece(boardState, blackPawn, startPos.x, startPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(blackPawn, startPos, boardState);
            
            // Assert
            var oneForward = new Vector2Int(4, 5); // e6
            Assert.Contains(oneForward, moves, "Black pawn should be able to move 1 square forward from starting position");
        }
        
        /// <summary>
        /// Test that black pawn can move 2 squares forward from starting position (first move only)
        /// </summary>
        [Test]
        public void BlackPawnCanMove2SquaresForwardFromStart()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var blackPawn = TestPieceFactory.CreatePawn(PlayerColor.Black, hasMoved: false);
            var startPos = new Vector2Int(4, 6); // e7
            TestBoardHelper.PlacePiece(boardState, blackPawn, startPos.x, startPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(blackPawn, startPos, boardState);
            
            // Assert
            var twoForward = new Vector2Int(4, 4); // e5
            Assert.Contains(twoForward, moves, "Black pawn should be able to move 2 squares forward from starting position");
        }
        
        /// <summary>
        /// Test that pawn cannot move 2 squares if it has already moved
        /// </summary>
        [Test]
        public void PawnCannotMove2SquaresAfterFirstMove()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var whitePawn = TestPieceFactory.CreatePawn(PlayerColor.White, hasMoved: true);
            var currentPos = new Vector2Int(4, 2); // e3 (already moved)
            TestBoardHelper.PlacePiece(boardState, whitePawn, currentPos.x, currentPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(whitePawn, currentPos, boardState);
            
            // Assert
            var twoForward = new Vector2Int(4, 4); // e5
            Assert.False(moves.Contains(twoForward), "Pawn should not be able to move 2 squares after it has already moved");
            
            var oneForward = new Vector2Int(4, 3); // e4
            Assert.Contains(oneForward, moves, "Pawn should still be able to move 1 square forward");
        }
        
        /// <summary>
        /// Test that pawns cannot move backward
        /// </summary>
        [Test]
        public void PawnsCannotMoveBackward()
        {
            // Arrange - White pawn
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var whitePawn = TestPieceFactory.CreatePawn(PlayerColor.White);
            var whitePos = new Vector2Int(4, 3); // e4
            TestBoardHelper.PlacePiece(boardState, whitePawn, whitePos.x, whitePos.y);
            
            // Act
            var whiteMoves = TestBoardHelper.GetPieceMovesFromPosition(whitePawn, whitePos, boardState);
            
            // Assert - White pawn should not move backward (toward rank 0)
            var whiteBackward = new Vector2Int(4, 2); // e3
            Assert.False(whiteMoves.Contains(whiteBackward), "White pawn should not be able to move backward");
            
            // Arrange - Black pawn
            var blackPawn = TestPieceFactory.CreatePawn(PlayerColor.Black);
            var blackPos = new Vector2Int(4, 4); // e5
            TestBoardHelper.PlacePiece(boardState, blackPawn, blackPos.x, blackPos.y);
            
            // Act
            var blackMoves = TestBoardHelper.GetPieceMovesFromPosition(blackPawn, blackPos, boardState);
            
            // Assert - Black pawn should not move backward (toward rank 7)
            var blackBackward = new Vector2Int(4, 5); // e6
            Assert.False(blackMoves.Contains(blackBackward), "Black pawn should not be able to move backward");
        }
        
        /// <summary>
        /// Test that pawns can capture diagonally when enemy piece is present
        /// </summary>
        [Test]
        public void PawnsCanCaptureDiagonallyWhenEnemyPresent()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var whitePawn = TestPieceFactory.CreatePawn(PlayerColor.White);
            var blackPawn1 = TestPieceFactory.CreatePawn(PlayerColor.Black);
            var blackPawn2 = TestPieceFactory.CreatePawn(PlayerColor.Black);
            
            var whitePawnPos = new Vector2Int(4, 4); // e5
            var blackPawn1Pos = new Vector2Int(3, 5); // d6 (diagonal left)
            var blackPawn2Pos = new Vector2Int(5, 5); // f6 (diagonal right)
            
            TestBoardHelper.PlacePiece(boardState, whitePawn, whitePawnPos.x, whitePawnPos.y);
            TestBoardHelper.PlacePiece(boardState, blackPawn1, blackPawn1Pos.x, blackPawn1Pos.y);
            TestBoardHelper.PlacePiece(boardState, blackPawn2, blackPawn2Pos.x, blackPawn2Pos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(whitePawn, whitePawnPos, boardState);
            
            // Assert
            Assert.Contains(blackPawn1Pos, moves, "White pawn should be able to capture diagonally left");
            Assert.Contains(blackPawn2Pos, moves, "White pawn should be able to capture diagonally right");
        }
        
        /// <summary>
        /// Test that pawns cannot move diagonally when no enemy piece to capture
        /// </summary>
        [Test]
        public void PawnsCannotMoveDiagonallyWhenNoEnemyToCapture()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var whitePawn = TestPieceFactory.CreatePawn(PlayerColor.White);
            var whitePawnPos = new Vector2Int(4, 4); // e5
            TestBoardHelper.PlacePiece(boardState, whitePawn, whitePawnPos.x, whitePawnPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(whitePawn, whitePawnPos, boardState);
            
            // Assert
            var diagonalLeft = new Vector2Int(3, 5); // d6
            var diagonalRight = new Vector2Int(5, 5); // f6
            
            Assert.False(moves.Contains(diagonalLeft), "Pawn should not be able to move diagonally when no enemy piece to capture");
            Assert.False(moves.Contains(diagonalRight), "Pawn should not be able to move diagonally when no enemy piece to capture");
        }
        
        /// <summary>
        /// Test that pawn is blocked when piece is directly in front
        /// </summary>
        [Test]
        public void PawnIsBlockedWhenPieceInFront()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var whitePawn = TestPieceFactory.CreatePawn(PlayerColor.White, hasMoved: false);
            var blockingPiece = TestPieceFactory.CreatePawn(PlayerColor.Black);
            
            var whitePawnPos = new Vector2Int(4, 1); // e2 (starting position)
            var blockingPos = new Vector2Int(4, 2); // e3 (directly in front)
            
            TestBoardHelper.PlacePiece(boardState, whitePawn, whitePawnPos.x, whitePawnPos.y);
            TestBoardHelper.PlacePiece(boardState, blockingPiece, blockingPos.x, blockingPos.y);
            
            // Act
            var moves = TestBoardHelper.GetPieceMovesFromPosition(whitePawn, whitePawnPos, boardState);
            
            // Assert
            Assert.IsEmpty(moves, "Pawn should have no moves when blocked by piece directly in front");
        }
        
        /// <summary>
        /// Test comprehensive pawn movement from starting position on full board
        /// </summary>
        [Test]
        public void PawnMovementFromStartingPositionOnFullBoard()
        {
            // Arrange - Standard starting position
            var boardState = TestBoardHelper.CreateStandardStartingPosition();
            
            // Test white pawns (should only be able to move 1 or 2 squares forward)
            for (int file = 0; file < 8; file++)
            {
                var whitePawn = boardState[file, 1] as Pawn;
                Assert.IsNotNull(whitePawn, $"White pawn should exist at {TestBoardHelper.CoordsToChessNotation(new Vector2Int(file, 1))}");
                
                var whitePawnPos = new Vector2Int(file, 1);
                var whiteMoves = TestBoardHelper.GetPieceMovesFromPosition(whitePawn, whitePawnPos, boardState);
                
                // Should have exactly 2 moves: 1 forward and 2 forward
                Assert.AreEqual(2, whiteMoves.Count, $"White pawn at {TestBoardHelper.CoordsToChessNotation(whitePawnPos)} should have exactly 2 moves");
                Assert.Contains(new Vector2Int(file, 2), whiteMoves, "White pawn should be able to move 1 square forward");
                Assert.Contains(new Vector2Int(file, 3), whiteMoves, "White pawn should be able to move 2 squares forward");
            }
            
            // Test black pawns (should only be able to move 1 or 2 squares forward)
            for (int file = 0; file < 8; file++)
            {
                var blackPawn = boardState[file, 6] as Pawn;
                Assert.IsNotNull(blackPawn, $"Black pawn should exist at {TestBoardHelper.CoordsToChessNotation(new Vector2Int(file, 6))}");
                
                var blackPawnPos = new Vector2Int(file, 6);
                var blackMoves = TestBoardHelper.GetPieceMovesFromPosition(blackPawn, blackPawnPos, boardState);
                
                // Should have exactly 2 moves: 1 forward and 2 forward
                Assert.AreEqual(2, blackMoves.Count, $"Black pawn at {TestBoardHelper.CoordsToChessNotation(blackPawnPos)} should have exactly 2 moves");
                Assert.Contains(new Vector2Int(file, 5), blackMoves, "Black pawn should be able to move 1 square forward");
                Assert.Contains(new Vector2Int(file, 4), blackMoves, "Black pawn should be able to move 2 squares forward");
            }
        }
    }
}