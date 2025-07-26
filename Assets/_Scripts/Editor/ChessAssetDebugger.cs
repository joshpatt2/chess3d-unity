using UnityEngine;
using UnityEditor;

namespace Chess3D
{
    /// <summary>
    /// Debug tool to inspect chess asset assignment issues
    /// </summary>
    public class ChessAssetDebugger : EditorWindow
    {
        [MenuItem("Tools/Chess3D/Debug Asset Assignment")]
        public static void DebugAssetAssignment()
        {
            Debug.Log("🔧 CHESS ASSET DEBUGGER");
            Debug.Log("========================");

            // Find ChessGameSetup in scene
            ChessGameSetup setup = FindObjectOfType<ChessGameSetup>();
            if (setup == null)
            {
                Debug.LogError("❌ No ChessGameSetup found in scene!");
                return;
            }

            Debug.Log("✅ Found ChessGameSetup in scene");

            // Check current prefab assignments
            Debug.Log("\n📦 CURRENT PREFAB ASSIGNMENTS:");
            Debug.Log($"Pawn Prefab: {(setup.pawnPrefab != null ? setup.pawnPrefab.name : "NULL")}");
            Debug.Log($"Rook Prefab: {(setup.rookPrefab != null ? setup.rookPrefab.name : "NULL")}");
            Debug.Log($"Knight Prefab: {(setup.knightPrefab != null ? setup.knightPrefab.name : "NULL")}");
            Debug.Log($"Bishop Prefab: {(setup.bishopPrefab != null ? setup.bishopPrefab.name : "NULL")}");
            Debug.Log($"Queen Prefab: {(setup.queenPrefab != null ? setup.queenPrefab.name : "NULL")}");
            Debug.Log($"King Prefab: {(setup.kingPrefab != null ? setup.kingPrefab.name : "NULL")}");

            // List all chess-related assets in project
            Debug.Log("\n🔍 AVAILABLE CHESS ASSETS:");
            string[] chessAssets = AssetDatabase.FindAssets("chess t:GameObject");
            foreach (string guid in chessAssets)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                Debug.Log($"Found: {path} (Name: {asset.name})");
            }

            // Test specific paths
            Debug.Log("\n🎯 TESTING SPECIFIC PATHS:");
            TestSpecificPath("Assets/Chess Set/Prefabs/Chess Pawn White.prefab");
            TestSpecificPath("Assets/Chess Set/Prefabs/Chess Pawn Black.prefab");
            TestSpecificPath("Assets/Chess Set/fbx/Pieces/Chess Pawn.fbx");

            // Try manual assignment
            Debug.Log("\n🔧 MANUAL ASSIGNMENT TEST:");
            TryManualAssignment(setup);
        }

        private static void TestSpecificPath(string path)
        {
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (asset != null)
            {
                Debug.Log($"✅ SUCCESS: {path} → {asset.name}");
            }
            else
            {
                Debug.Log($"❌ FAILED: {path} not found");
            }
        }

        private static void TryManualAssignment(ChessGameSetup setup)
        {
            // Try to manually assign one piece to test the system
            GameObject pawnAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Chess Set/Prefabs/Chess Pawn White.prefab");
            
            if (pawnAsset != null)
            {
                setup.pawnPrefab = pawnAsset;
                EditorUtility.SetDirty(setup);
                Debug.Log("✅ Successfully assigned pawn prefab manually!");
                
                // Test if Board can use it
                Board board = FindObjectOfType<Board>();
                if (board != null)
                {
                    Debug.Log("✅ Board found - testing piece creation compatibility");
                    // We could test piece creation here, but that might be complex in edit mode
                }
            }
            else
            {
                Debug.LogError("❌ Could not find Chess Pawn White prefab at expected path");
                
                // List what's actually in the Chess Set folder
                Debug.Log("📁 Contents of Assets/Chess Set/:");
                string[] allAssets = AssetDatabase.FindAssets("", new[] { "Assets/Chess Set" });
                foreach (string guid in allAssets)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    Debug.Log($"  - {path}");
                }
            }
        }

        [MenuItem("Tools/Chess3D/Quick Test Assignment")]
        public static void QuickTestAssignment()
        {
            ChessGameSetup setup = FindObjectOfType<ChessGameSetup>();
            if (setup == null) return;

            // Try the most straightforward assignment
            setup.pawnPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Chess Set/Prefabs/Chess Pawn White.prefab");
            setup.rookPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Chess Set/Prefabs/Chess Rook White.prefab");
            setup.knightPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Chess Set/Prefabs/Chess Knight White.prefab");
            setup.bishopPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Chess Set/Prefabs/Chess Bishop White.prefab");
            setup.queenPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Chess Set/Prefabs/Chess Queen White.prefab");
            setup.kingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Chess Set/Prefabs/Chess King White.prefab");

            EditorUtility.SetDirty(setup);

            int successCount = 0;
            if (setup.pawnPrefab != null) successCount++;
            if (setup.rookPrefab != null) successCount++;
            if (setup.knightPrefab != null) successCount++;
            if (setup.bishopPrefab != null) successCount++;
            if (setup.queenPrefab != null) successCount++;
            if (setup.kingPrefab != null) successCount++;

            Debug.Log($"Quick assignment result: {successCount}/6 pieces assigned");
        }
    }
}
