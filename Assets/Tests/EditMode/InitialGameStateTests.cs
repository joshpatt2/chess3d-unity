using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Chess3D.Tests
{
    /// <summary>
    /// Tests for initial game state setup and board configuration
    /// </summary>
    public class InitialGameStateTests
    {
        /// <summary>
        /// Test that the board initializes with pieces in correct starting positions
        /// </summary>
        [Test]
        public void BoardInitializesWithCorrectStartingPositions()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateStandardStartingPosition();
            
            // Act & Assert - White pieces (rank 0 - back rank)
            Assert.IsInstanceOf<Rook>(boardState[0, 0], "White rook should be at a1");
            Assert.IsInstanceOf<Knight>(boardState[1, 0], "White knight should be at b1");
            Assert.IsInstanceOf<Bishop>(boardState[2, 0], "White bishop should be at c1");
            Assert.IsInstanceOf<Queen>(boardState[3, 0], "White queen should be at d1");
            Assert.IsInstanceOf<King>(boardState[4, 0], "White king should be at e1");
            Assert.IsInstanceOf<Bishop>(boardState[5, 0], "White bishop should be at f1");
            Assert.IsInstanceOf<Knight>(boardState[6, 0], "White knight should be at g1");
            Assert.IsInstanceOf<Rook>(boardState[7, 0], "White rook should be at h1");
            
            // White pawns (rank 1)
            for (int file = 0; file < 8; file++)
            {
                Assert.IsInstanceOf<Pawn>(boardState[file, 1], $"White pawn should be at {TestBoardHelper.CoordsToChessNotation(new Vector2Int(file, 1))}");
            }
            
            // Empty squares (ranks 2-5)
            for (int rank = 2; rank <= 5; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    Assert.IsNull(boardState[file, rank], $"Square {TestBoardHelper.CoordsToChessNotation(new Vector2Int(file, rank))} should be empty");
                }
            }
            
            // Black pawns (rank 6)
            for (int file = 0; file < 8; file++)
            {
                Assert.IsInstanceOf<Pawn>(boardState[file, 6], $"Black pawn should be at {TestBoardHelper.CoordsToChessNotation(new Vector2Int(file, 6))}");
            }
            
            // Black pieces (rank 7 - back rank)
            Assert.IsInstanceOf<Rook>(boardState[0, 7], "Black rook should be at a8");
            Assert.IsInstanceOf<Knight>(boardState[1, 7], "Black knight should be at b8");
            Assert.IsInstanceOf<Bishop>(boardState[2, 7], "Black bishop should be at c8");
            Assert.IsInstanceOf<Queen>(boardState[3, 7], "Black queen should be at d8");
            Assert.IsInstanceOf<King>(boardState[4, 7], "Black king should be at e8");
            Assert.IsInstanceOf<Bishop>(boardState[5, 7], "Black bishop should be at f8");
            Assert.IsInstanceOf<Knight>(boardState[6, 7], "Black knight should be at g8");
            Assert.IsInstanceOf<Rook>(boardState[7, 7], "Black rook should be at h8");
        }
        
        /// <summary>
        /// Test that all piece components are properly attached and configured
        /// </summary>
        [Test]
        public void AllPieceComponentsAreProperlyConfigured()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateStandardStartingPosition();
            
            // Act & Assert - Check all pieces are not null and have proper components
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    var piece = boardState[file, rank];
                    
                    // Skip empty squares
                    if (rank >= 2 && rank <= 5) continue;
                    
                    Assert.IsNotNull(piece, $"Piece at {TestBoardHelper.CoordsToChessNotation(new Vector2Int(file, rank))} should not be null");
                    Assert.IsNotNull(piece.gameObject, "Piece should have a GameObject");
                    Assert.IsFalse(piece.hasMoved, "Pieces should start with hasMoved = false");
                }
            }
        }
        
        /// <summary>
        /// Test that piece colors are correctly assigned (White/Black)
        /// </summary>
        [Test]
        public void PieceColorsAreCorrectlyAssigned()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateStandardStartingPosition();
            
            // Act & Assert - White pieces (ranks 0-1)
            for (int rank = 0; rank <= 1; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    var piece = boardState[file, rank];
                    Assert.IsNotNull(piece, $"White piece at {TestBoardHelper.CoordsToChessNotation(new Vector2Int(file, rank))} should exist");
                    Assert.AreEqual(PlayerColor.White, piece.color, $"Piece at {TestBoardHelper.CoordsToChessNotation(new Vector2Int(file, rank))} should be white");
                }
            }
            
            // Black pieces (ranks 6-7)
            for (int rank = 6; rank <= 7; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    var piece = boardState[file, rank];
                    Assert.IsNotNull(piece, $"Black piece at {TestBoardHelper.CoordsToChessNotation(new Vector2Int(file, rank))} should exist");
                    Assert.AreEqual(PlayerColor.Black, piece.color, $"Piece at {TestBoardHelper.CoordsToChessNotation(new Vector2Int(file, rank))} should be black");
                }
            }
        }
        
        /// <summary>
        /// Test that all pieces have not moved initially
        /// </summary>
        [Test]
        public void AllPiecesStartWithHasMovedFalse()
        {
            // Arrange
            var boardState = TestBoardHelper.CreateStandardStartingPosition();
            
            // Act & Assert
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    var piece = boardState[file, rank];
                    
                    // Skip empty squares
                    if (piece == null) continue;
                    
                    Assert.IsFalse(piece.hasMoved, $"Piece at {TestBoardHelper.CoordsToChessNotation(new Vector2Int(file, rank))} should start with hasMoved = false");
                }
            }
        }
        
        /// <summary>
        /// Test coordinate conversion helpers work correctly
        /// </summary>
        [Test]
        public void CoordinateConversionWorksProperly()
        {
            // Test notation to coordinates
            Assert.AreEqual(new Vector2Int(0, 0), TestBoardHelper.ChessNotationToCoords("a1"));
            Assert.AreEqual(new Vector2Int(4, 3), TestBoardHelper.ChessNotationToCoords("e4"));
            Assert.AreEqual(new Vector2Int(7, 7), TestBoardHelper.ChessNotationToCoords("h8"));
            
            // Test coordinates to notation
            Assert.AreEqual("a1", TestBoardHelper.CoordsToChessNotation(new Vector2Int(0, 0)));
            Assert.AreEqual("e4", TestBoardHelper.CoordsToChessNotation(new Vector2Int(4, 3)));
            Assert.AreEqual("h8", TestBoardHelper.CoordsToChessNotation(new Vector2Int(7, 7)));
        }
    }
}