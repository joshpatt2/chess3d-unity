using UnityEngine;
using UnityEditor;

namespace Chess3D
{
    /// <summary>
    /// Enhanced asset auto-assigner that handles assets with separate white/black prefabs
    /// Supports the Chess Set asset structure with color-specific pieces
    /// </summary>
    public class EnhancedAssetAssigner : EditorWindow
    {
        [MenuItem("Tools/Chess3D/Smart Auto-Assign Chess Assets")]
        public static void SmartAutoAssignAssets()
        {
            ChessGameSetup setup = FindObjectOfType<ChessGameSetup>();
            if (setup == null)
            {
                Debug.LogWarning("No ChessGameSetup found in scene. Please create one first using Tools > Chess3D > Quick Setup Scene");
                return;
            }

            int assignedCount = 0;

            // First, try to find separate white/black prefabs (higher quality)
            assignedCount += TryAssignColorSpecificPrefabs(setup);

            // If that didn't work, fall back to generic prefabs
            if (assignedCount == 0)
            {
                assignedCount += TryAssignGenericPrefabs(setup);
            }

            if (assignedCount > 0)
            {
                Debug.Log($"✅ Successfully assigned {assignedCount} chess piece prefabs!");
                Debug.Log("♟️ Using color-specific prefabs for better visual quality");
                EditorUtility.SetDirty(setup);
            }
            else
            {
                Debug.LogWarning("❌ Could not find chess assets. Make sure you've imported a chess asset pack.");
                ListAvailableAssets();
            }
        }

        private static int TryAssignColorSpecificPrefabs(ChessGameSetup setup)
        {
            int count = 0;
            
            // Search for assets with "White" in the name (we'll use white pieces as defaults)
            string[] searchPaths = {
                "Assets/Chess Set/Prefabs/",
                "Assets/Chess Set/fbx/Pieces/",
                "Assets/ChessSet/Prefabs/",
                "Assets/Chess/Prefabs/"
            };

            foreach (string basePath in searchPaths)
            {
                count += TryAssignFromColorPath(setup, basePath, "White");
                if (count > 0) break; // Found assets, stop searching
            }

            return count;
        }

        private static int TryAssignGenericPrefabs(ChessGameSetup setup)
        {
            // Fallback: Try basic asset paths without color suffix
            string[] basicPaths = {
                "Assets/Chess Set/fbx/Pieces/",
                "Assets/ChessSet/Models/",
                "Assets/Chess/Pieces/"
            };

            int count = 0;
            foreach (string basePath in basicPaths)
            {
                count += TryAssignBasicPrefabs(setup, basePath);
                if (count > 0) break;
            }
            
            return count;
        }

        private static int TryAssignBasicPrefabs(ChessGameSetup setup, string basePath)
        {
            int count = 0;
            
            // Try to assign basic FBX models without color specification
            var pieceTypes = new[]
            {
                new { field = "pawnPrefab", names = new[] { "Pawn", "pawn" } },
                new { field = "rookPrefab", names = new[] { "Rook", "rook", "Castle" } },
                new { field = "knightPrefab", names = new[] { "Knight", "knight", "Horse" } },
                new { field = "bishopPrefab", names = new[] { "Bishop", "bishop" } },
                new { field = "queenPrefab", names = new[] { "Queen", "queen" } },
                new { field = "kingPrefab", names = new[] { "King", "king" } }
            };

            foreach (var pieceType in pieceTypes)
            {
                var field = typeof(ChessGameSetup).GetField(pieceType.field);
                if (field?.GetValue(setup) == null)
                {
                    GameObject prefab = FindBasicPrefab(basePath, pieceType.names);
                    if (prefab != null)
                    {
                        field.SetValue(setup, prefab);
                        count++;
                        Debug.Log($"Assigned {pieceType.field}: {prefab.name}");
                    }
                }
            }

            return count;
        }

        private static GameObject FindBasicPrefab(string basePath, string[] pieceNames)
        {
            string[] extensions = { ".fbx", ".prefab" };
            string[] prefixes = { "Chess ", "" };

            foreach (string pieceName in pieceNames)
            {
                foreach (string prefix in prefixes)
                {
                    foreach (string ext in extensions)
                    {
                        string fullPath = $"{basePath}{prefix}{pieceName}{ext}";
                        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
                        if (prefab != null)
                        {
                            return prefab;
                        }
                    }
                }
            }

            return null;
        }

        private static int TryAssignFromColorPath(ChessGameSetup setup, string basePath, string colorSuffix)
        {
            int count = 0;

            // Piece type mappings
            var pieceTypes = new[]
            {
                new { field = "pawnPrefab", names = new[] { "Pawn", "pawn" } },
                new { field = "rookPrefab", names = new[] { "Rook", "rook", "Castle" } },
                new { field = "knightPrefab", names = new[] { "Knight", "knight", "Horse" } },
                new { field = "bishopPrefab", names = new[] { "Bishop", "bishop" } },
                new { field = "queenPrefab", names = new[] { "Queen", "queen" } },
                new { field = "kingPrefab", names = new[] { "King", "king" } }
            };

            foreach (var pieceType in pieceTypes)
            {
                var field = typeof(ChessGameSetup).GetField(pieceType.field);
                if (field?.GetValue(setup) == null) // Only assign if not already set
                {
                    GameObject prefab = FindColorSpecificPrefab(basePath, pieceType.names, colorSuffix);
                    if (prefab != null)
                    {
                        field.SetValue(setup, prefab);
                        count++;
                        Debug.Log($"Assigned {pieceType.field}: {prefab.name}");
                    }
                }
            }

            return count;
        }

        private static GameObject FindColorSpecificPrefab(string basePath, string[] pieceNames, string colorSuffix)
        {
            string[] extensions = { ".prefab", ".fbx" };
            string[] prefixes = { "Chess ", "" };

            foreach (string pieceName in pieceNames)
            {
                foreach (string prefix in prefixes)
                {
                    foreach (string ext in extensions)
                    {
                        // Try: "Chess Pawn White.prefab"
                        string fullPath = $"{basePath}{prefix}{pieceName} {colorSuffix}{ext}";
                        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
                        if (prefab != null)
                        {
                            return prefab;
                        }

                        // Try: "Chess_Pawn_White.prefab"
                        fullPath = $"{basePath}{prefix}{pieceName}_{colorSuffix}{ext}";
                        prefab = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
                        if (prefab != null)
                        {
                            return prefab;
                        }
                    }
                }
            }

            return null;
        }

        private static void ListAvailableAssets()
        {
            Debug.Log("🔍 Searching for available chess assets...");
            
            string[] searchTerms = { "chess", "pawn", "king", "queen", "rook", "bishop", "knight" };
            
            foreach (string term in searchTerms)
            {
                string[] guids = AssetDatabase.FindAssets(term + " t:GameObject");
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    Debug.Log($"Found asset: {path}");
                }
            }
        }
    }
}
