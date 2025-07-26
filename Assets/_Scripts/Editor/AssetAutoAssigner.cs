using UnityEngine;
using UnityEditor;

namespace Chess3D
{
    /// <summary>
    /// Helper script to automatically find and assign Chess Pieces & Board assets after import
    /// This saves time manually dragging prefabs in the inspector
    /// </summary>
    public class AssetAutoAssigner : EditorWindow
    {
        [MenuItem("Tools/Chess3D/Auto-Assign Asset Store Pieces")]
        public static void AutoAssignAssets()
        {
            // Find the ChessGameSetup component in the scene
            ChessGameSetup setup = FindObjectOfType<ChessGameSetup>();
            if (setup == null)
            {
                Debug.LogWarning("No ChessGameSetup found in scene. Please create one first using Tools > Chess3D > Quick Setup Scene");
                return;
            }

            int assignedCount = 0;

            // Try to find and assign Chess Pieces & Board assets
            // These are common paths where the asset might be imported
            string[] possiblePaths = {
                "Assets/Chess Set/Prefabs/",
                "Assets/Chess Set/fbx/Pieces/",
                "Assets/ChessPieces&Board/Models/Pieces/",
                "Assets/Chess Pieces & Board/Models/Pieces/",
                "Assets/ChessSet/Models/",
                "Assets/Chess/Pieces/",
                "Assets/Models/Chess/"
            };

            foreach (string basePath in possiblePaths)
            {
                assignedCount += TryAssignFromPath(setup, basePath);
                if (assignedCount > 0) break; // Found assets, stop searching
            }

            if (assignedCount > 0)
            {
                Debug.Log($"✅ Successfully auto-assigned {assignedCount} chess piece prefabs to ChessGameSetup!");
                EditorUtility.SetDirty(setup);
            }
            else
            {
                Debug.LogWarning("❌ Could not find Chess Pieces & Board assets. Please:\n" +
                                "1. Import the 'Chess Pieces & Board' asset from Unity Asset Store\n" +
                                "2. Manually assign prefabs in the ChessGameSetup inspector\n" +
                                "3. Or check if assets are in a different folder structure");
            }
        }

        private static int TryAssignFromPath(ChessGameSetup setup, string basePath)
        {
            int count = 0;

            // Try to find each piece type with various naming conventions
            if (setup.pawnPrefab == null)
            {
                setup.pawnPrefab = FindPrefabAtPath(basePath, "Pawn");
                if (setup.pawnPrefab != null) count++;
            }

            if (setup.rookPrefab == null)
            {
                setup.rookPrefab = FindPrefabAtPath(basePath, "Rook");
                if (setup.rookPrefab != null) count++;
            }

            if (setup.knightPrefab == null)
            {
                setup.knightPrefab = FindPrefabAtPath(basePath, "Knight");
                if (setup.knightPrefab != null) count++;
            }

            if (setup.bishopPrefab == null)
            {
                setup.bishopPrefab = FindPrefabAtPath(basePath, "Bishop");
                if (setup.bishopPrefab != null) count++;
            }

            if (setup.queenPrefab == null)
            {
                setup.queenPrefab = FindPrefabAtPath(basePath, "Queen");
                if (setup.queenPrefab != null) count++;
            }

            if (setup.kingPrefab == null)
            {
                setup.kingPrefab = FindPrefabAtPath(basePath, "King");
                if (setup.kingPrefab != null) count++;
            }

            return count;
        }

        private static GameObject FindPrefabAtPath(string basePath, string pieceType)
        {
            // Try different file extensions and naming conventions
            string[] extensions = { ".fbx", ".prefab", ".obj" };
            string[] namingPatterns = {
                pieceType,                          // Pawn
                "Chess " + pieceType,               // Chess Pawn
                "Chess " + pieceType + " White",    // Chess Pawn White (prefer white for default)
                "Chess " + pieceType + " Black",    // Chess Pawn Black (fallback)
                pieceType + "Piece",                // PawnPiece
                pieceType + "_Prefab"               // Pawn_Prefab
            };

            foreach (string pattern in namingPatterns)
            {
                foreach (string ext in extensions)
                {
                    string fullPath = basePath + pattern + ext;
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
                    if (prefab != null)
                    {
                        Debug.Log($"Found: {fullPath}");
                        return prefab;
                    }
                }
            }

            return null;
        }

        [MenuItem("Tools/Chess3D/Clear All Asset Assignments")]
        public static void ClearAssetAssignments()
        {
            ChessGameSetup setup = FindObjectOfType<ChessGameSetup>();
            if (setup == null) return;

            setup.pawnPrefab = null;
            setup.rookPrefab = null;
            setup.knightPrefab = null;
            setup.bishopPrefab = null;
            setup.queenPrefab = null;
            setup.kingPrefab = null;
            setup.boardPrefab = null;

            EditorUtility.SetDirty(setup);
            Debug.Log("Cleared all asset assignments. Chess game will use primitives.");
        }
    }
}
