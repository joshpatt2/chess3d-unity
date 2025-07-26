using System.Collections.Generic;
using UnityEngine;

namespace Chess3D
{
    /// <summary>
    /// Manages both the logical representation (2D array) and visual representation 
    /// (GameObjects) of the chess board
    /// </summary>
    public class Board : MonoBehaviour
    {
        [Header("Board Configuration")]
        public GameObject tilePrefab;
        public Material lightTileMaterial;
        public Material darkTileMaterial;
        
        [Header("Piece Prefabs")]
        public GameObject pawnPrefab;
        public GameObject rookPrefab;
        public GameObject knightPrefab;
        public GameObject bishopPrefab;
        public GameObject queenPrefab;
        public GameObject kingPrefab;
        
        [Header("Piece Materials")]
        public Material whitePieceMaterial;
        public Material blackPieceMaterial;
        
        [Header("Piece Positioning & Scale")]
        [Tooltip("Scale multiplier for chess pieces (0.1 to 2.0). Smaller values make pieces smaller.")]
        [Range(0.1f, 2.0f)]
        public float pieceScale = 0.7f;
        
        [Tooltip("Height offset above tiles for pieces")]
        public float pieceHeightOffset = 0.5f;
        
        [Tooltip("If true, pieces will be positioned at tile centers. If false, at tile corners.")]
        public bool centerPiecesOnTiles = true;

        // The logical board - 2D array representing piece positions
        private Piece[,] pieces = new Piece[8, 8];
        
        // Visual board - store tile GameObjects for highlighting
        private GameObject[,] tiles = new GameObject[8, 8];
        
        // Track if setup has been completed
        private bool isSetupComplete = false;

        void Start()
        {
            // Safety check for piece scale - prevent invisible pieces
            if (pieceScale <= 0.01f)
            {
                Debug.LogWarning($"⚠️ Piece scale is too small ({pieceScale}), resetting to default 0.7f");
                pieceScale = 0.7f;
            }
            
            // Check if we should wait for auto-assignment
            ChessGameSetup setupScript = FindFirstObjectByType<ChessGameSetup>();
            if (setupScript != null && setupScript.autoAssignAssetsOnStart)
            {
                // Wait a frame for auto-assignment to complete
                StartCoroutine(DelayedSetup());
            }
            else
            {
                SetupBoard();
            }
        }

        /// <summary>
        /// Wait for auto-assignment then setup the board
        /// </summary>
        private System.Collections.IEnumerator DelayedSetup()
        {
            yield return null; // Wait one frame
            SetupBoard();
        }

        /// <summary>
        /// Setup the complete board (tiles and pieces)
        /// </summary>
        public void SetupBoard()
        {
            if (isSetupComplete) return; // Prevent double setup
            
            CreateBoard();
            SetupPieces();
            isSetupComplete = true;
        }

        /// <summary>
        /// Creates the visual 8x8 chessboard with alternating light/dark tiles
        /// </summary>
        private void CreateBoard()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    // Create tile GameObject
                    GameObject tile = Instantiate(tilePrefab, transform);
                    tile.name = $"Tile_{x}_{y}";
                    
                    // Position the tile in world space
                    tile.transform.position = new Vector3(x, 0, y);
                    
                    // DEBUG: Log tile positions for first few tiles to verify coordinate system
                    if (x < 3 && y < 3)
                    {
                        Debug.Log($"🟫 Created tile: Tile_{x}_{y} at position {tile.transform.position}");
                    }
                    
                    // Store reference to the tile
                    tiles[x, y] = tile;
                    
                    // Apply material based on checkerboard pattern
                    Renderer tileRenderer = tile.GetComponent<Renderer>();
                    if ((x + y) % 2 == 0)
                    {
                        tileRenderer.sharedMaterial = lightTileMaterial;
                    }
                    else
                    {
                        tileRenderer.sharedMaterial = darkTileMaterial;
                    }
                    
                    // Add a tag for easy identification (using Untagged since Tile tag doesn't exist by default)
                    tile.tag = "Untagged";
                }
            }
        }

        /// <summary>
        /// Sets up the initial chess piece positions according to standard rules
        /// </summary>
        private void SetupPieces()
        {
            // White pieces (ranks 1-2)
            SetupPieceRow(0, PlayerColor.White, false); // Back rank
            SetupPieceRow(1, PlayerColor.White, true);  // Pawn rank
            
            // Black pieces (ranks 7-8)
            SetupPieceRow(7, PlayerColor.Black, false); // Back rank
            SetupPieceRow(6, PlayerColor.Black, true);  // Pawn rank
        }

        /// <summary>
        /// Helper method to set up a complete rank of pieces
        /// </summary>
        private void SetupPieceRow(int rank, PlayerColor color, bool isPawnRank)
        {
            if (isPawnRank)
            {
                // Set up pawns
                for (int file = 0; file < 8; file++)
                {
                    CreatePieceAtPosition(file, rank, color, "Pawn");
                }
            }
            else
            {
                // Set up back rank pieces
                string[] backRankPieces = { "Rook", "Knight", "Bishop", "Queen", 
                                          "King", "Bishop", "Knight", "Rook" };
                
                for (int file = 0; file < 8; file++)
                {
                    CreatePieceAtPosition(file, rank, color, backRankPieces[file]);
                }
            }
        }

        /// <summary>
        /// Creates a piece GameObject at a specific position
        /// Requires prefabs to be assigned - no primitive fallback
        /// </summary>
        private void CreatePieceAtPosition(int x, int y, PlayerColor color, string pieceType)
        {
            GameObject prefab = GetPrefabForPieceType(pieceType);

            if (prefab == null)
            {
                Debug.LogError($"No prefab assigned for {pieceType}! Please assign all piece prefabs in the Board component or use auto-assignment.");
                return;
            }

            // Use the prefab
            GameObject pieceObject = Instantiate(prefab, transform);
            
            // Set name
            pieceObject.name = $"{pieceType}_{color}_{x}_{y}";
            
            // Position the piece at tile corner (working position) then debug centering
            Vector3 workingCornerPos = new Vector3(x, 0.5f, y);
            Vector3 attemptedCenterPos = new Vector3(x + 0.5f, 0.5f, y + 0.5f);
            Vector3 altCenterPos1 = new Vector3(x - 0.5f, 0.5f, y - 0.5f); // Try opposite
            Vector3 altCenterPos2 = new Vector3(x + 0.5f, 0.5f, y - 0.5f); // Try mixed
            Vector3 altCenterPos3 = new Vector3(x - 0.5f, 0.5f, y + 0.5f); // Try other mixed
            
            pieceObject.transform.position = workingCornerPos; // Use working position
            pieceObject.transform.localScale = Vector3.one * 24.5f; // 30% smaller (35.0 * 0.7 = 24.5)
            Debug.Log($"🎮 DEBUGGING: {pieceObject.name} at WORKING corner {workingCornerPos}");
            Debug.Log($"🔍 Attempted center was {attemptedCenterPos}, Alt1: {altCenterPos1}, Alt2: {altCenterPos2}, Alt3: {altCenterPos3}");
            pieceObject.transform.SetParent(transform);
            
            // Get the Piece component and set its properties
            Piece pieceComponent = pieceObject.GetComponent<Piece>();
            
            // If no Piece component exists (e.g., from asset store prefabs), add the appropriate one
            if (pieceComponent == null)
            {
                pieceComponent = AddPieceComponent(pieceObject, pieceType);
            }
            
            if (pieceComponent != null)
            {
                // Set color immediately after component creation
                pieceComponent.color = color;
                
                // Apply the correct material for player colors
                ApplyPieceMaterial(pieceObject, color);
                
                // Add to logical board after color is set
                pieces[x, y] = pieceComponent;
            }
            else
            {
                Debug.LogError($"Could not create piece component for {pieceType}");
            }
            
            // Ensure piece has a collider for raycasting
            EnsurePieceHasCollider(pieceObject);
            
            // Add tag for easy identification (using Untagged since Piece tag doesn't exist by default)
            pieceObject.tag = "Untagged";
        }

        /// <summary>
        /// Ensure a piece GameObject has a collider for raycasting
        /// </summary>
        private void EnsurePieceHasCollider(GameObject pieceObject)
        {
            Collider existingCollider = pieceObject.GetComponent<Collider>();
            if (existingCollider == null)
            {
                // Check if there are child objects with colliders (for complex models)
                Collider childCollider = pieceObject.GetComponentInChildren<Collider>();
                if (childCollider == null)
                {
                    // Add a box collider as fallback
                    BoxCollider boxCollider = pieceObject.AddComponent<BoxCollider>();
                    Debug.Log($"🔗 Added BoxCollider to piece: {pieceObject.name}");
                }
                else
                {
                    Debug.Log($"🔗 Using existing child collider on piece: {pieceObject.name}");
                }
            }
            else
            {
                Debug.Log($"🔗 Piece already has collider: {pieceObject.name}");
            }
        }

        /// <summary>
        /// Get the piece at a specific board position
        /// </summary>
        public Piece GetPieceAt(int x, int y)
        {
            if (x >= 0 && x < 8 && y >= 0 && y < 8)
            {
                return pieces[x, y];
            }
            return null;
        }

        /// <summary>
        /// Get the piece at a specific board position using Vector2Int
        /// </summary>
        public Piece GetPieceAt(Vector2Int position)
        {
            return GetPieceAt(position.x, position.y);
        }

        /// <summary>
        /// Get the world position for a piece at the given board coordinates
        /// Handles centering and height offset based on current settings
        /// </summary>
        private Vector3 GetPieceWorldPosition(int x, int y)
        {
            Vector3 tilePosition = new Vector3(x, 0, y);
            if (centerPiecesOnTiles)
            {
                // Center piece on tile (standard chess positioning)
                tilePosition += new Vector3(0.5f, pieceHeightOffset, 0.5f);
            }
            else
            {
                // Position at tile corner
                tilePosition += new Vector3(0, pieceHeightOffset, 0);
            }
            
            Debug.Log($"🎯 Piece position calculated: ({x},{y}) -> {tilePosition}, Scale: {pieceScale}");
            return tilePosition;
        }

        /// <summary>
        /// Get the world position for a piece at the given board coordinates (Vector2Int version)
        /// </summary>
        private Vector3 GetPieceWorldPosition(Vector2Int position)
        {
            return GetPieceWorldPosition(position.x, position.y);
        }

        /// <summary>
        /// Update all existing pieces to use current scale and positioning settings
        /// Useful when adjusting settings during runtime or after setup
        /// </summary>
        [ContextMenu("Update All Piece Positions and Scale")]
        public void UpdateAllPiecePositionsAndScale()
        {
            // Validate settings first
            ValidateSettings();
            
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Piece piece = GetPieceAt(x, y);
                    if (piece != null)
                    {
                        piece.transform.position = GetPieceWorldPosition(x, y);
                        piece.transform.localScale = Vector3.one * pieceScale;
                        Debug.Log($"🔄 Updated {piece.name}: pos={piece.transform.position}, scale={piece.transform.localScale}");
                    }
                }
            }
            Debug.Log($"Updated all piece positions and scales. Scale: {pieceScale}, Height: {pieceHeightOffset}, Centered: {centerPiecesOnTiles}");
        }

        /// <summary>
        /// Validate and fix piece settings to prevent invisible or broken pieces
        /// </summary>
        [ContextMenu("Validate Piece Settings")]
        public void ValidateSettings()
        {
            bool wasFixed = false;
            
            if (pieceScale <= 0.01f || pieceScale > 5.0f)
            {
                Debug.LogWarning($"⚠️ Invalid piece scale ({pieceScale}), resetting to 0.7f");
                pieceScale = 0.7f;
                wasFixed = true;
            }
            
            if (pieceHeightOffset < 0f || pieceHeightOffset > 10f)
            {
                Debug.LogWarning($"⚠️ Invalid piece height offset ({pieceHeightOffset}), resetting to 0.5f");
                pieceHeightOffset = 0.5f;
                wasFixed = true;
            }
            
            if (wasFixed)
            {
                Debug.Log($"✅ Settings validated. Scale: {pieceScale}, Height: {pieceHeightOffset}");
            }
        }

        /// <summary>
        /// Simulate a move in the logical board only (no visual changes)
        /// Used for legal move validation
        /// </summary>
        public void SimulateMove(Vector2Int from, Vector2Int to)
        {
            Piece piece = GetPieceAt(from);
            if (piece != null)
            {
                // Only update the logical board array - no visual changes
                pieces[from.x, from.y] = null;
                pieces[to.x, to.y] = piece;
            }
        }

        /// <summary>
        /// Undo a simulated move in the logical board only
        /// Used to restore state after legal move validation
        /// </summary>
        public void UndoSimulatedMove(Vector2Int from, Vector2Int to, Piece originalCapturedPiece)
        {
            Piece piece = GetPieceAt(to);
            if (piece != null)
            {
                // Restore logical board state - no visual changes
                pieces[to.x, to.y] = originalCapturedPiece;
                pieces[from.x, from.y] = piece;
            }
        }

        public void MovePiece(Vector2Int from, Vector2Int to)
        {
            Piece piece = GetPieceAt(from);
            if (piece != null)
            {
                // Clear the old position
                pieces[from.x, from.y] = null;
                
                // Handle capture if there's a piece at the destination
                Piece capturedPiece = GetPieceAt(to);
                if (capturedPiece != null)
                {
                    // Notify GameManager about capture
                    GameManager.Instance?.OnPieceCaptured(capturedPiece);
                    Destroy(capturedPiece.gameObject);
                }
                
                // Move piece to new position
                pieces[to.x, to.y] = piece;
                
                // Update visual position - revert to working corner position
                Vector3 workingCornerPos = new Vector3(to.x, 0.5f, to.y);
                piece.transform.position = workingCornerPos;
                Debug.Log($"🔄 Moved piece to WORKING corner position {workingCornerPos} (logical coords: {to.x},{to.y})");
                
                // Mark piece as moved
                piece.OnMoved();
            }
        }

        /// <summary>
        /// Check if a position is empty
        /// </summary>
        public bool IsEmpty(Vector2Int position)
        {
            return GetPieceAt(position) == null;
        }

        /// <summary>
        /// Check if a position contains an enemy piece
        /// </summary>
        public bool IsEnemyPiece(Vector2Int position, PlayerColor playerColor)
        {
            Piece piece = GetPieceAt(position);
            if (piece != null)
            {
                return piece.color != playerColor;
            }
            return false;
        }

        /// <summary>
        /// Get the tile GameObject at a specific position
        /// </summary>
        public GameObject GetTileAt(Vector2Int position)
        {
            if (position.x >= 0 && position.x < 8 && position.y >= 0 && position.y < 8)
            {
                return tiles[position.x, position.y];
            }
            return null;
        }

        /// <summary>
        /// Helper method to get piece name from prefab for primitive creation
        /// </summary>
        private string GetPieceNameFromPrefab(GameObject prefab)
        {
            if (prefab == pawnPrefab) return "Pawn";
            if (prefab == rookPrefab) return "Rook";
            if (prefab == knightPrefab) return "Knight";
            if (prefab == bishopPrefab) return "Bishop";
            if (prefab == queenPrefab) return "Queen";
            if (prefab == kingPrefab) return "King";
            return "UnknownPiece";
        }

        /// <summary>
        /// Helper method to determine piece type from back rank position
        /// </summary>
        private string GetPieceTypeFromPosition(int file, bool isPawnRank)
        {
            if (isPawnRank) return "Pawn";
            
            switch (file)
            {
                case 0:
                case 7: return "Rook";
                case 1:
                case 6: return "Knight";
                case 2:
                case 5: return "Bishop";
                case 3: return "Queen";
                case 4: return "King";
                default: return "UnknownPiece";
            }
        }

        /// <summary>
        /// Add the appropriate piece component to a primitive GameObject
        /// </summary>
        private Piece AddPieceComponent(GameObject pieceObject, string pieceType)
        {
            switch (pieceType)
            {
                case "Pawn":
                    return pieceObject.AddComponent<Pawn>();
                case "Rook":
                    return pieceObject.AddComponent<Rook>();
                case "Knight":
                    return pieceObject.AddComponent<Knight>();
                case "Bishop":
                    return pieceObject.AddComponent<Bishop>();
                case "Queen":
                    return pieceObject.AddComponent<Queen>();
                case "King":
                    return pieceObject.AddComponent<King>();
                default:
                    Debug.LogWarning($"Unknown piece type: {pieceType}");
                    return pieceObject.AddComponent<Pawn>(); // Default fallback
            }
        }

        /// <summary>
        /// Get the prefab for a specific piece type
        /// </summary>
        private GameObject GetPrefabForPieceType(string pieceType)
        {
            switch (pieceType)
            {
                case "Pawn": return pawnPrefab;
                case "Rook": return rookPrefab;
                case "Knight": return knightPrefab;
                case "Bishop": return bishopPrefab;
                case "Queen": return queenPrefab;
                case "King": return kingPrefab;
                default: return null;
            }
        }

        /// <summary>
        /// Extract piece type from prefab name (handles asset store naming conventions)
        /// </summary>
        private string GetPieceTypeFromName(string pieceName)
        {
            if (string.IsNullOrEmpty(pieceName)) return "Pawn"; // Default fallback

            string name = pieceName.ToLower();
            
            if (name.Contains("pawn")) return "Pawn";
            if (name.Contains("rook") || name.Contains("castle")) return "Rook";
            if (name.Contains("knight") || name.Contains("horse")) return "Knight";
            if (name.Contains("bishop")) return "Bishop";
            if (name.Contains("queen")) return "Queen";
            if (name.Contains("king")) return "King";
            
            // Fallback - try to extract from various naming patterns
            return "Pawn"; // Safe default
        }

        /// <summary>
        /// Apply the appropriate material to a chess piece based on its color
        /// Handles both single and multiple renderer objects (like complex 3D models)
        /// </summary>
        private void ApplyPieceMaterial(GameObject pieceObject, PlayerColor color)
        {
            if (whitePieceMaterial == null || blackPieceMaterial == null)
            {
                Debug.LogWarning($"Missing piece materials! Cannot apply color to {pieceObject.name}");
                return;
            }

            Material targetMaterial = (color == PlayerColor.White) ? whitePieceMaterial : blackPieceMaterial;
            
            // Get all renderers in the piece (including children for complex models)
            Renderer[] renderers = pieceObject.GetComponentsInChildren<Renderer>();
            
            if (renderers.Length == 0)
            {
                Debug.LogWarning($"No renderers found on piece {pieceObject.name}!");
                return;
            }

            int materialsApplied = 0;
            foreach (Renderer renderer in renderers)
            {
                if (renderer != null)
                {
                    // Check if this renderer should get the color material
                    if (ShouldApplyColorMaterial(renderer))
                    {
                        renderer.material = targetMaterial;
                        materialsApplied++;
                        Debug.Log($"✅ Applied {color} material to {renderer.gameObject.name}");
                    }
                    else
                    {
                        Debug.Log($"⏭️ Skipped material override for {renderer.gameObject.name} (preserved original)");
                    }
                }
            }

            if (materialsApplied == 0)
            {
                Debug.LogWarning($"⚠️ No materials were applied to {pieceObject.name}. All renderers were skipped.");
            }
            else
            {
                Debug.Log($"🎨 Applied {color} material to {materialsApplied} renderer(s) on {pieceObject.name}");
            }
        }

        /// <summary>
        /// Determine if a renderer should receive the player color material
        /// </summary>
        private bool ShouldApplyColorMaterial(Renderer renderer)
        {
            // Always apply to objects with default/generic materials
            Material currentMaterial = renderer.material;
            string materialName = currentMaterial.name.ToLower();
            
            // Apply to default materials
            if (materialName.Contains("default") || materialName.Contains("material"))
            {
                return true;
            }

            // Apply to materials that look like chess piece materials
            if (materialName.Contains("chess") || materialName.Contains("piece"))
            {
                return true;
            }

            // Apply to materials that seem generic (no specific names)
            if (materialName.Length < 10 && !materialName.Contains("special") && !materialName.Contains("effect"))
            {
                return true;
            }

            // For Chess Set assets, apply to main piece materials
            string objectName = renderer.gameObject.name.ToLower();
            if (objectName.Contains("piece") || objectName.Contains("chess") || 
                objectName.Contains("pawn") || objectName.Contains("rook") || 
                objectName.Contains("knight") || objectName.Contains("bishop") || 
                objectName.Contains("queen") || objectName.Contains("king"))
            {
                return true;
            }

            // Skip materials that might be special effects or details
            if (materialName.Contains("eye") || materialName.Contains("gem") || 
                materialName.Contains("metal") || materialName.Contains("gold") || 
                materialName.Contains("silver"))
            {
                return false;
            }

            // Default: apply the material (can be changed to false for more conservative approach)
            return true;
        }
    }
}
