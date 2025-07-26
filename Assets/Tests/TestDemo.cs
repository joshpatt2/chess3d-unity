using System.Collections.Generic;
using UnityEngine;

namespace Chess3D.Tests
{
    /// <summary>
    /// Quick test demonstration script showing how the unit tests work
    /// Run this in play mode to see test examples in action
    /// </summary>
    public class TestDemo : MonoBehaviour
    {
        [Header("Demo Settings")]
        public bool runDemoOnStart = true;
        public bool logDetailedResults = true;
        
        void Start()
        {
            if (runDemoOnStart)
            {
                RunTestDemo();
            }
        }
        
        /// <summary>
        /// Demonstrate key test scenarios
        /// </summary>
        [ContextMenu("Run Test Demo")]
        public void RunTestDemo()
        {
            Debug.Log("🎮 Starting Chess3D Test Demo...");
            
            DemoInitialGameState();
            DemoPawnMovement();
            DemoKnightOpeningMoves();
            DemoQueenPowerCombination();
            
            Debug.Log("🎯 Test Demo Complete!");
        }
        
        private void DemoInitialGameState()
        {
            Debug.Log("\n📋 DEMO: Initial Game State");
            
            var boardState = TestBoardHelper.CreateStandardStartingPosition();
            
            // Verify white pieces
            var whiteKing = boardState[4, 0];
            var whiteQueen = boardState[3, 0];
            var whitePawn = boardState[4, 1];
            
            Debug.Log($"✅ White King at e1: {whiteKing?.GetType().Name} ({whiteKing?.color})");
            Debug.Log($"✅ White Queen at d1: {whiteQueen?.GetType().Name} ({whiteQueen?.color})");
            Debug.Log($"✅ White Pawn at e2: {whitePawn?.GetType().Name} ({whitePawn?.color})");
            
            // Verify black pieces
            var blackKing = boardState[4, 7];
            var blackQueen = boardState[3, 7];
            var blackPawn = boardState[4, 6];
            
            Debug.Log($"✅ Black King at e8: {blackKing?.GetType().Name} ({blackKing?.color})");
            Debug.Log($"✅ Black Queen at d8: {blackQueen?.GetType().Name} ({blackQueen?.color})");
            Debug.Log($"✅ Black Pawn at e7: {blackPawn?.GetType().Name} ({blackPawn?.color})");
            
            // Verify empty center
            var centerSquare = boardState[4, 4];
            Debug.Log($"✅ Center e5 is empty: {centerSquare == null}");
            
            CleanupBoardState(boardState);
        }
        
        private void DemoPawnMovement()
        {
            Debug.Log("\n♟️ DEMO: Pawn Movement from Starting Position");
            
            var boardState = TestBoardHelper.CreateStandardStartingPosition();
            
            // Test white pawn at e2
            var whitePawn = boardState[4, 1] as Pawn;
            var whitePawnMoves = TestBoardHelper.GetPieceMovesFromPosition(whitePawn, new Vector2Int(4, 1), boardState);
            
            Debug.Log($"White pawn at e2 has {whitePawnMoves.Count} moves:");
            if (logDetailedResults)
            {
                foreach (var move in whitePawnMoves)
                {
                    Debug.Log($"  → {TestBoardHelper.CoordsToChessNotation(move)}");
                }
            }
            
            // Test black pawn at e7
            var blackPawn = boardState[4, 6] as Pawn;
            var blackPawnMoves = TestBoardHelper.GetPieceMovesFromPosition(blackPawn, new Vector2Int(4, 6), boardState);
            
            Debug.Log($"Black pawn at e7 has {blackPawnMoves.Count} moves:");
            if (logDetailedResults)
            {
                foreach (var move in blackPawnMoves)
                {
                    Debug.Log($"  → {TestBoardHelper.CoordsToChessNotation(move)}");
                }
            }
            
            CleanupBoardState(boardState);
        }
        
        private void DemoKnightOpeningMoves()
        {
            Debug.Log("\n♞ DEMO: Knight Opening Moves (As Specified in Requirements)");
            
            var boardState = TestBoardHelper.CreateStandardStartingPosition();
            
            // Test white queenside knight (b1) - should be able to go to c3 and a3
            var whiteQueensideKnight = boardState[1, 0] as Knight;
            var queensideMoves = TestBoardHelper.GetPieceMovesFromPosition(whiteQueensideKnight, new Vector2Int(1, 0), boardState);
            
            Debug.Log($"White knight at b1 has {queensideMoves.Count} moves:");
            var c3 = TestBoardHelper.ChessNotationToCoords("c3");
            var a3 = TestBoardHelper.ChessNotationToCoords("a3");
            
            Debug.Log($"  ✅ Can move Nb1-c3: {queensideMoves.Contains(c3)}");
            Debug.Log($"  ✅ Can move Nb1-a3: {queensideMoves.Contains(a3)}");
            
            // Test white kingside knight (g1) - should be able to go to f3 and h3
            var whiteKingsideKnight = boardState[6, 0] as Knight;
            var kingsideMoves = TestBoardHelper.GetPieceMovesFromPosition(whiteKingsideKnight, new Vector2Int(6, 0), boardState);
            
            Debug.Log($"White knight at g1 has {kingsideMoves.Count} moves:");
            var f3 = TestBoardHelper.ChessNotationToCoords("f3");
            var h3 = TestBoardHelper.ChessNotationToCoords("h3");
            
            Debug.Log($"  ✅ Can move Ng1-f3: {kingsideMoves.Contains(f3)}");
            Debug.Log($"  ✅ Can move Ng1-h3: {kingsideMoves.Contains(h3)}");
            
            CleanupBoardState(boardState);
        }
        
        private void DemoQueenPowerCombination()
        {
            Debug.Log("\n👑 DEMO: Queen Power (Rook + Bishop Combination)");
            
            // Test queen in center of empty board
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var queen = TestPieceFactory.CreateQueen(PlayerColor.White);
            var rook = TestPieceFactory.CreateRook(PlayerColor.White);
            var bishop = TestPieceFactory.CreateBishop(PlayerColor.White);
            
            var centerPos = new Vector2Int(4, 4); // e5
            
            var queenMoves = TestBoardHelper.GetPieceMovesFromPosition(queen, centerPos, boardState);
            var rookMoves = TestBoardHelper.GetPieceMovesFromPosition(rook, centerPos, boardState);
            var bishopMoves = TestBoardHelper.GetPieceMovesFromPosition(bishop, centerPos, boardState);
            
            Debug.Log($"From e5 on empty board:");
            Debug.Log($"  Rook has {rookMoves.Count} moves (horizontal + vertical)");
            Debug.Log($"  Bishop has {bishopMoves.Count} moves (diagonal)");
            Debug.Log($"  Queen has {queenMoves.Count} moves (should be {rookMoves.Count + bishopMoves.Count})");
            Debug.Log($"  ✅ Queen = Rook + Bishop: {queenMoves.Count == (rookMoves.Count + bishopMoves.Count)}");
            
            // Cleanup
            if (queen?.gameObject) DestroyImmediate(queen.gameObject);
            if (rook?.gameObject) DestroyImmediate(rook.gameObject);
            if (bishop?.gameObject) DestroyImmediate(bishop.gameObject);
        }
        
        private void CleanupBoardState(Piece[,] boardState)
        {
            // Clean up GameObjects to prevent memory leaks in demo
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    var piece = boardState[x, y];
                    if (piece?.gameObject) DestroyImmediate(piece.gameObject);
                }
            }
        }
    }
}