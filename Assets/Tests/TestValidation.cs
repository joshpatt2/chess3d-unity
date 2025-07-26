using UnityEngine;

namespace Chess3D.Tests
{
    /// <summary>
    /// Simple validation script to verify test compilation and basic functionality
    /// Can be attached to a GameObject to test the core test utilities
    /// </summary>
    public class TestValidation : MonoBehaviour
    {
        [Header("Test Validation")]
        public bool runValidationOnStart = true;
        
        void Start()
        {
            if (runValidationOnStart)
            {
                ValidateTestUtilities();
            }
        }
        
        /// <summary>
        /// Validate that our test utilities work correctly
        /// </summary>
        public void ValidateTestUtilities()
        {
            Debug.Log("🧪 Starting Test Validation...");
            
            try
            {
                // Test coordinate conversion
                ValidateCoordinateConversion();
                
                // Test piece factory
                ValidatePieceFactory();
                
                // Test board setup
                ValidateBoardSetup();
                
                // Test basic piece movement
                ValidateBasicPieceMovement();
                
                Debug.Log("✅ All test utilities validated successfully!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Test validation failed: {e.Message}\n{e.StackTrace}");
            }
        }
        
        private void ValidateCoordinateConversion()
        {
            Debug.Log("📍 Validating coordinate conversion...");
            
            // Test conversion
            var coords = TestBoardHelper.ChessNotationToCoords("e4");
            var notation = TestBoardHelper.CoordsToChessNotation(coords);
            
            if (coords.x == 4 && coords.y == 3 && notation == "e4")
            {
                Debug.Log("✅ Coordinate conversion working correctly");
            }
            else
            {
                throw new System.Exception($"Coordinate conversion failed: e4 -> {coords} -> {notation}");
            }
        }
        
        private void ValidatePieceFactory()
        {
            Debug.Log("🏭 Validating piece factory...");
            
            // Test creating different pieces
            var pawn = TestPieceFactory.CreatePawn(PlayerColor.White);
            var knight = TestPieceFactory.CreateKnight(PlayerColor.Black);
            var queen = TestPieceFactory.CreateQueen(PlayerColor.White);
            
            if (pawn != null && knight != null && queen != null)
            {
                if (pawn.color == PlayerColor.White && knight.color == PlayerColor.Black && queen.color == PlayerColor.White)
                {
                    Debug.Log("✅ Piece factory working correctly");
                }
                else
                {
                    throw new System.Exception("Piece colors not set correctly");
                }
            }
            else
            {
                throw new System.Exception("Failed to create pieces");
            }
            
            // Cleanup
            if (pawn?.gameObject) DestroyImmediate(pawn.gameObject);
            if (knight?.gameObject) DestroyImmediate(knight.gameObject);
            if (queen?.gameObject) DestroyImmediate(queen.gameObject);
        }
        
        private void ValidateBoardSetup()
        {
            Debug.Log("♟️ Validating board setup...");
            
            // Test creating board state
            var boardState = TestBoardHelper.CreateStandardStartingPosition();
            
            // Check white king is at e1
            var whiteKing = boardState[4, 0];
            if (whiteKing == null || !(whiteKing is King) || whiteKing.color != PlayerColor.White)
            {
                throw new System.Exception("White king not found at e1 in starting position");
            }
            
            // Check black queen is at d8
            var blackQueen = boardState[3, 7];
            if (blackQueen == null || !(blackQueen is Queen) || blackQueen.color != PlayerColor.Black)
            {
                throw new System.Exception("Black queen not found at d8 in starting position");
            }
            
            // Check empty squares
            if (boardState[4, 4] != null)
            {
                throw new System.Exception("Center square e5 should be empty in starting position");
            }
            
            Debug.Log("✅ Board setup working correctly");
            
            // Cleanup
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    var piece = boardState[x, y];
                    if (piece?.gameObject) DestroyImmediate(piece.gameObject);
                }
            }
        }
        
        private void ValidateBasicPieceMovement()
        {
            Debug.Log("♞ Validating basic piece movement...");
            
            // Test knight movement (doesn't depend on board state much)
            var boardState = TestBoardHelper.CreateEmptyBoard();
            var knight = TestPieceFactory.CreateKnight(PlayerColor.White);
            var knightPos = new Vector2Int(4, 4); // e5
            TestBoardHelper.PlacePiece(boardState, knight, knightPos.x, knightPos.y);
            
            var moves = TestBoardHelper.GetPieceMovesFromPosition(knight, knightPos, boardState);
            
            if (moves.Count == 8)
            {
                Debug.Log($"✅ Knight movement validation successful: {moves.Count} moves from center");
            }
            else
            {
                throw new System.Exception($"Knight should have 8 moves from center, got {moves.Count}");
            }
            
            // Cleanup
            if (knight?.gameObject) DestroyImmediate(knight.gameObject);
        }
        
        /// <summary>
        /// Public method to run validation from inspector
        /// </summary>
        [ContextMenu("Run Test Validation")]
        public void RunValidation()
        {
            ValidateTestUtilities();
        }
    }
}