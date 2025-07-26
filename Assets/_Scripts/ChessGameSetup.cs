using UnityEngine;

namespace Chess3D
{
    /// <summary>
    /// Quick setup script to create a chess game scene with support for both primitives and 3D assets
    /// Perfect for rapid prototyping and professional polish following the Carmack-Letterkenny philosophy
    /// </summary>
    public class ChessGameSetup : MonoBehaviour
    {
        [Header("Quick Setup Options")]
        [Tooltip("Automatically create all required GameObjects on Start")]
        public bool autoSetupOnStart = true;
        
        [Tooltip("Create basic materials if they don't exist")]
        public bool createBasicMaterials = true;
        
        [Tooltip("Automatically find and assign 3D chess assets on startup")]
        public bool autoAssignAssetsOnStart = true;

        [Header("3D Asset Integration (Chess Pieces & Board - Asset Store)")]
        [Tooltip("Assign these if you have the Chess Pieces & Board asset from Unity Asset Store")]
        public GameObject pawnPrefab;
        public GameObject rookPrefab;
        public GameObject knightPrefab;
        public GameObject bishopPrefab;
        public GameObject queenPrefab;
        public GameObject kingPrefab;
        public GameObject boardPrefab;
        
        [Header("Material Overrides")]
        public Material whitePieceMaterial;
        public Material blackPieceMaterial;
        public Material whiteTileMaterial;
        public Material blackTileMaterial;

        void Start()
        {
            if (autoAssignAssetsOnStart)
            {
                AutoAssignChessAssets();
                
                // If there's an existing Board in the scene, update its prefabs after auto-assignment
                Board existingBoard = FindFirstObjectByType<Board>();
                if (existingBoard != null)
                {
                    UpdateExistingBoardPrefabs(existingBoard);
                    existingBoard.SetupBoard(); // Trigger setup after assignment
                }
            }
            
            if (autoSetupOnStart)
            {
                SetupChessScene();
            }
        }

        /// <summary>
        /// Update an existing Board's prefab references with auto-assigned assets
        /// </summary>
        private void UpdateExistingBoardPrefabs(Board board)
        {
            if (pawnPrefab != null) board.pawnPrefab = pawnPrefab;
            if (rookPrefab != null) board.rookPrefab = rookPrefab;
            if (knightPrefab != null) board.knightPrefab = knightPrefab;
            if (bishopPrefab != null) board.bishopPrefab = bishopPrefab;
            if (queenPrefab != null) board.queenPrefab = queenPrefab;
            if (kingPrefab != null) board.kingPrefab = kingPrefab;
            
            Debug.Log("📋 Updated existing Board with auto-assigned prefabs");
        }

        /// <summary>
        /// Automatically find and assign chess asset prefabs at runtime
        /// This eliminates the need for manual assignment in the inspector
        /// </summary>
        private void AutoAssignChessAssets()
        {
            int assignedCount = 0;
            
            Debug.Log("🔍 Auto-assigning chess assets...");
            
            // First try manual assignment from known Chess Set paths
            assignedCount += TryManualChessSetAssignment();
            
            if (assignedCount == 0)
            {
                // Try color-specific prefabs
                assignedCount += TryAssignColorSpecificAssets();
                
                // Fall back to generic assets if needed
                if (assignedCount == 0)
                {
                    assignedCount += TryAssignGenericAssets();
                }
            }
            
            if (assignedCount > 0)
            {
                Debug.Log($"✅ Successfully auto-assigned {assignedCount} chess piece prefabs!");
            }
            else
            {
                Debug.LogError("❌ No 3D chess assets found!\n" +
                             "💡 Solutions:\n" +
                             "1. Manually drag Chess Set pieces from 'Assets/Chess Set/fbx/Pieces/' into the ChessGameSetup inspector\n" +
                             "2. Move your Chess Set assets to 'Assets/Resources/Chess Set/'\n" +
                             "3. Use the Editor auto-assignment tools (Tools > Chess3D > Auto Assign Assets)");
            }
        }

        /// <summary>
        /// Try to manually assign Chess Set pieces using direct asset loading
        /// </summary>
        private int TryManualChessSetAssignment()
        {
            // This will only work in Editor mode, but provides clear feedback
            #if UNITY_EDITOR
            return TryEditorAssetAssignment();
            #else
            return 0;
            #endif
        }

        #if UNITY_EDITOR
        /// <summary>
        /// Use Unity Editor APIs to assign assets directly (Editor only)
        /// </summary>
        private int TryEditorAssetAssignment()
        {
            int count = 0;
            string basePath = "Assets/Chess Set/fbx/Pieces/";
            
            var pieceMapping = new[]
            {
                new { field = "pawnPrefab", assetName = "Chess pawn.fbx" },
                new { field = "rookPrefab", assetName = "Chess rook.fbx" },
                new { field = "knightPrefab", assetName = "Chess knight.fbx" },
                new { field = "bishopPrefab", assetName = "Chess bishop.fbx" },
                new { field = "queenPrefab", assetName = "Chess queen.fbx" },
                new { field = "kingPrefab", assetName = "Chess king.fbx" }
            };

            foreach (var piece in pieceMapping)
            {
                if (GetPrefabField(piece.field) == null)
                {
                    string fullPath = basePath + piece.assetName;
                    GameObject asset = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
                    if (asset != null)
                    {
                        SetPrefabField(piece.field, asset);
                        count++;
                        Debug.Log($"📦 Editor assigned {piece.field}: {asset.name}");
                    }
                }
            }

            return count;
        }
        #endif

        /// <summary>
        /// Try to assign color-specific prefabs (e.g., "Chess Pawn White.prefab")
        /// </summary>
        private int TryAssignColorSpecificAssets()
        {
            int count = 0;
            string[] searchPaths = {
                "Assets/Resources/Chess Set/Prefabs/",
                "Assets/Resources/ChessSet/Prefabs/", 
                "Assets/Resources/Chess/Prefabs/",
                "Assets/Chess Set/Prefabs/",
                "Assets/ChessSet/Prefabs/",
                "Assets/Chess/Prefabs/"
            };

            foreach (string basePath in searchPaths)
            {
                count += AssignFromPath(basePath, "White"); // Prefer white pieces
                if (count > 0) break;
            }

            return count;
        }

        /// <summary>
        /// Try to assign generic prefabs (e.g., "Chess Pawn.fbx")
        /// </summary>
        private int TryAssignGenericAssets()
        {
            int count = 0;
            string[] searchPaths = {
                "Assets/Resources/Chess Set/fbx/Pieces/",
                "Assets/Resources/ChessSet/Models/",
                "Assets/Resources/Chess/Models/",
                "Assets/Chess Set/fbx/Pieces/",
                "Assets/ChessSet/Models/",
                "Assets/Chess/Models/"
            };

            foreach (string basePath in searchPaths)
            {
                count += AssignFromPath(basePath, "");
                if (count > 0) break;
            }

            return count;
        }

        /// <summary>
        /// Assign prefabs from a specific path with optional color suffix
        /// </summary>
        private int AssignFromPath(string basePath, string colorSuffix)
        {
            int count = 0;
            
            var pieces = new[]
            {
                new { field = "pawnPrefab", names = new[] { "Pawn", "pawn" } },
                new { field = "rookPrefab", names = new[] { "Rook", "rook" } },
                new { field = "knightPrefab", names = new[] { "Knight", "knight" } },
                new { field = "bishopPrefab", names = new[] { "Bishop", "bishop" } },
                new { field = "queenPrefab", names = new[] { "Queen", "queen" } },
                new { field = "kingPrefab", names = new[] { "King", "king" } }
            };

            foreach (var piece in pieces)
            {
                if (GetPrefabField(piece.field) == null) // Only assign if not already set
                {
                    GameObject prefab = FindPrefabAsset(basePath, piece.names, colorSuffix);
                    if (prefab != null)
                    {
                        SetPrefabField(piece.field, prefab);
                        count++;
                        Debug.Log($"📦 Assigned {piece.field}: {prefab.name}");
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Find a prefab asset using various naming conventions
        /// Uses Resources.Load for runtime compatibility
        /// </summary>
        private GameObject FindPrefabAsset(string basePath, string[] pieceNames, string colorSuffix)
        {
            string[] extensions = { ".prefab", ".fbx" };
            string[] prefixes = { "Chess ", "" };

            foreach (string pieceName in pieceNames)
            {
                foreach (string prefix in prefixes)
                {
                    foreach (string ext in extensions)
                    {
                        string fileName = string.IsNullOrEmpty(colorSuffix) 
                            ? $"{prefix}{pieceName}{ext}"
                            : $"{prefix}{pieceName} {colorSuffix}{ext}";
                        
                        string fullPath = basePath + fileName;
                        
                        // Try Resources.Load for runtime loading
                        GameObject prefab = TryLoadFromResources(fullPath);
                        if (prefab != null)
                        {
                            Debug.Log($"✅ Loaded from Resources: {prefab.name}");
                            return prefab;
                        }
                        
                        // Check if asset exists but not in Resources folder
                        if (System.IO.File.Exists(fullPath))
                        {
                            Debug.LogWarning($"⚠️ Found asset at {fullPath} but it's not in a Resources folder. " +
                                           "Either move chess assets to Assets/Resources/ or assign prefabs manually in the inspector.");
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Try to load an asset from Resources folder with multiple path attempts
        /// </summary>
        private GameObject TryLoadFromResources(string fullPath)
        {
            // Try original path structure
            string resourcePath = GetResourcePath(fullPath);
            GameObject prefab = Resources.Load<GameObject>(resourcePath);
            if (prefab != null) return prefab;
            
            // Try common Resources subfolder structures
            string[] resourceSubfolders = {
                "Chess Set/",
                "ChessSet/", 
                "Chess/",
                "Pieces/",
                ""
            };
            
            string fileName = System.IO.Path.GetFileNameWithoutExtension(fullPath);
            
            foreach (string subfolder in resourceSubfolders)
            {
                string altPath = subfolder + fileName;
                prefab = Resources.Load<GameObject>(altPath);
                if (prefab != null)
                {
                    Debug.Log($"📁 Found in Resources subfolder: {altPath}");
                    return prefab;
                }
            }
            
            return null;
        }

        /// <summary>
        /// Convert full asset path to Resources path
        /// </summary>
        private string GetResourcePath(string fullPath)
        {
            // Convert "Assets/Chess Set/Prefabs/Chess Pawn White.prefab" 
            // to "Chess Set/Prefabs/Chess Pawn White"
            if (fullPath.StartsWith("Assets/"))
            {
                fullPath = fullPath.Substring(7); // Remove "Assets/"
            }
            
            int lastDot = fullPath.LastIndexOf('.');
            if (lastDot > 0)
            {
                fullPath = fullPath.Substring(0, lastDot); // Remove extension
            }
            
            return fullPath;
        }

        /// <summary>
        /// Get the current value of a prefab field
        /// </summary>
        private GameObject GetPrefabField(string fieldName)
        {
            switch (fieldName)
            {
                case "pawnPrefab": return pawnPrefab;
                case "rookPrefab": return rookPrefab;
                case "knightPrefab": return knightPrefab;
                case "bishopPrefab": return bishopPrefab;
                case "queenPrefab": return queenPrefab;
                case "kingPrefab": return kingPrefab;
                default: return null;
            }
        }

        /// <summary>
        /// Set the value of a prefab field
        /// </summary>
        private void SetPrefabField(string fieldName, GameObject prefab)
        {
            switch (fieldName)
            {
                case "pawnPrefab": pawnPrefab = prefab; break;
                case "rookPrefab": rookPrefab = prefab; break;
                case "knightPrefab": knightPrefab = prefab; break;
                case "bishopPrefab": bishopPrefab = prefab; break;
                case "queenPrefab": queenPrefab = prefab; break;
                case "kingPrefab": kingPrefab = prefab; break;
            }
        }

        /// <summary>
        /// Create a complete chess scene with primitives
        /// </summary>
        [ContextMenu("Setup Chess Scene")]
        public void SetupChessScene()
        {
            // Create parent object for organization
            GameObject chessGame = new GameObject("ChessGame");
            
            // Setup Camera Rig
            SetupCameraRig();
            
            // Setup Board
            SetupBoard(chessGame);
            
            // Setup GameManager
            SetupGameManager(chessGame);
            
            // Setup Input Controller
            SetupInputController();
            
            // Create basic lighting
            SetupLighting();
            
            // Check if 3D assets were successfully loaded
            bool using3DAssets = pawnPrefab != null && rookPrefab != null && knightPrefab != null && 
                                bishopPrefab != null && queenPrefab != null && kingPrefab != null;
            
            Debug.Log($"Chess scene setup complete! Using 3D assets: {(using3DAssets ? "YES" : "NO - Check prefab assignments!")}");
        }

        /// <summary>
        /// Create camera rig with proper positioning for chess
        /// </summary>
        private void SetupCameraRig()
        {
            // Create camera rig
            GameObject cameraRig = new GameObject("CameraRig");
            cameraRig.transform.position = new Vector3(3.5f, 0, 3.5f); // Center of board
            
            // Move main camera to be child of rig
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.transform.SetParent(cameraRig.transform);
                mainCam.transform.localPosition = new Vector3(0, 10, -7);
                mainCam.transform.localRotation = Quaternion.Euler(55, 0, 0);
                
                // Add camera controller
                cameraRig.AddComponent<CameraController>();
            }
        }

        /// <summary>
        /// Setup the board with primitive tiles and pieces
        /// </summary>
        private void SetupBoard(GameObject parent)
        {
            GameObject boardObject = new GameObject("Board");
            boardObject.transform.SetParent(parent.transform);
            
            Board boardScript = boardObject.AddComponent<Board>();
            
            // Assign 3D asset prefabs if available, otherwise Board will use primitives
            if (pawnPrefab != null) boardScript.pawnPrefab = pawnPrefab;
            if (rookPrefab != null) boardScript.rookPrefab = rookPrefab;
            if (knightPrefab != null) boardScript.knightPrefab = knightPrefab;
            if (bishopPrefab != null) boardScript.bishopPrefab = bishopPrefab;
            if (queenPrefab != null) boardScript.queenPrefab = queenPrefab;
            if (kingPrefab != null) boardScript.kingPrefab = kingPrefab;
            
            // Use material overrides if available, otherwise create basic materials
            if (whitePieceMaterial != null) boardScript.whitePieceMaterial = whitePieceMaterial;
            if (blackPieceMaterial != null) boardScript.blackPieceMaterial = blackPieceMaterial;
            if (whiteTileMaterial != null) boardScript.lightTileMaterial = whiteTileMaterial;
            if (blackTileMaterial != null) boardScript.darkTileMaterial = blackTileMaterial;
            
            // Create basic materials if needed and no overrides provided
            if (createBasicMaterials)
            {
                if (boardScript.lightTileMaterial == null) 
                {
                    boardScript.lightTileMaterial = CreateBasicMaterial("LightTile", new Color(0.9f, 0.9f, 0.85f)); // Cream
                    if (boardScript.lightTileMaterial == null)
                    {
                        Debug.LogError("❌ Failed to create light tile material! Using fallback.");
                        boardScript.lightTileMaterial = CreateFallbackMaterial("LightTile_Fallback", new Color(0.9f, 0.9f, 0.85f));
                    }
                }
                if (boardScript.darkTileMaterial == null) 
                {
                    boardScript.darkTileMaterial = CreateBasicMaterial("DarkTile", new Color(0.4f, 0.25f, 0.15f)); // Dark brown
                    if (boardScript.darkTileMaterial == null)
                    {
                        boardScript.darkTileMaterial = CreateFallbackMaterial("DarkTile_Fallback", new Color(0.4f, 0.25f, 0.15f));
                    }
                }
                if (boardScript.whitePieceMaterial == null) 
                {
                    boardScript.whitePieceMaterial = CreateBasicMaterial("WhitePieces", new Color(0.95f, 0.95f, 0.9f)); // Off-white
                    if (boardScript.whitePieceMaterial == null)
                    {
                        boardScript.whitePieceMaterial = CreateFallbackMaterial("WhitePieces_Fallback", new Color(0.95f, 0.95f, 0.9f));
                    }
                }
                if (boardScript.blackPieceMaterial == null) 
                {
                    boardScript.blackPieceMaterial = CreateBasicMaterial("BlackPieces", new Color(0.15f, 0.1f, 0.1f)); // Dark gray-brown
                    if (boardScript.blackPieceMaterial == null)
                    {
                        boardScript.blackPieceMaterial = CreateFallbackMaterial("BlackPieces_Fallback", new Color(0.15f, 0.1f, 0.1f));
                    }
                }
            }
            
            // Set tile prefab to primitive quad (3D board assets usually include their own tiles)
            GameObject tilePrefab = GameObject.CreatePrimitive(PrimitiveType.Quad);
            tilePrefab.transform.rotation = Quaternion.Euler(90, 0, 0); // Lay flat
            boardScript.tilePrefab = tilePrefab;
            
            Debug.Log($"Board setup complete. Using 3D assets: {(pawnPrefab != null ? "YES" : "NO (primitives)")}");
        }

        /// <summary>
        /// Setup the GameManager
        /// </summary>
        private void SetupGameManager(GameObject parent)
        {
            GameObject gameManagerObject = new GameObject("GameManager");
            gameManagerObject.transform.SetParent(parent.transform);
            
            GameManager gameManager = gameManagerObject.AddComponent<GameManager>();
            
            // Create move highlight prefab
            GameObject highlightPrefab = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            highlightPrefab.transform.localScale = new Vector3(0.9f, 0.1f, 0.9f);
            Material highlightMaterial = CreateBasicMaterial("MoveHighlight", new Color(0, 1, 0, 0.5f));
            highlightPrefab.GetComponent<Renderer>().material = highlightMaterial;
            gameManager.moveHighlightPrefab = highlightPrefab;
            
            // Create selected piece material
            gameManager.selectedPieceMaterial = CreateBasicMaterial("SelectedPiece", Color.yellow);
            
            // Connect references
            gameManager.board = FindFirstObjectByType<Board>();
        }

        /// <summary>
        /// Setup input controller
        /// </summary>
        private void SetupInputController()
        {
            GameObject inputObject = new GameObject("InputController");
            InputController inputController = inputObject.AddComponent<InputController>();
            
            // Connect to GameManager
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.inputController = inputController;
            }
        }

        /// <summary>
        /// Create comprehensive lighting setup for chess
        /// </summary>
        private void SetupLighting()
        {
            // Main directional light (sun)
            Light dirLight = FindFirstObjectByType<Light>();
            if (dirLight == null)
            {
                GameObject lightObject = new GameObject("Main Light (Directional)");
                dirLight = lightObject.AddComponent<Light>();
                dirLight.type = LightType.Directional;
                lightObject.transform.rotation = Quaternion.Euler(45, 45, 0);
            }
            
            // Configure main light for good chess visibility
            dirLight.intensity = 1.5f;
            dirLight.color = new Color(1f, 0.95f, 0.8f); // Warm white
            dirLight.shadows = LightShadows.Soft; // Enable soft shadows
            dirLight.shadowStrength = 0.8f;
            
            // Add fill light to reduce harsh shadows
            GameObject fillLightObj = new GameObject("Fill Light");
            Light fillLight = fillLightObj.AddComponent<Light>();
            fillLight.type = LightType.Directional;
            fillLight.intensity = 0.3f;
            fillLight.color = new Color(0.7f, 0.8f, 1f); // Cool fill
            fillLight.shadows = LightShadows.None;
            fillLightObj.transform.rotation = Quaternion.Euler(-30, -45, 0);
            
            // Set ambient lighting
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.5f, 0.7f, 1f);
            RenderSettings.ambientEquatorColor = new Color(0.4f, 0.4f, 0.6f);
            RenderSettings.ambientGroundColor = new Color(0.2f, 0.2f, 0.3f);
            RenderSettings.ambientIntensity = 0.3f;
            
            Debug.Log("✨ Lighting setup complete - Main light with shadows + fill light + ambient");
        }

        /// <summary>
        /// Create a basic material with specified color
        /// Uses URP-compatible shaders when available
        /// </summary>
        private Material CreateBasicMaterial(string name, Color color)
        {
            // Try to find the best available shader
            Shader shader = FindBestShader();
            
            if (shader == null)
            {
                Debug.LogError($"❌ No suitable shader found for material '{name}'! This will cause pink materials.");
                return null;
            }

            Material mat = new Material(shader);
            mat.name = name;
            
            // Set color using the most compatible method
            SetMaterialColor(mat, color);

            // Configure transparency if needed
            if (color.a < 1.0f)
            {
                ConfigureTransparentMaterial(mat);
            }

            Debug.Log($"✅ Created material '{name}' using shader: {shader.name}");
            return mat;
        }

        /// <summary>
        /// Find the best available shader for the current render pipeline
        /// </summary>
        private Shader FindBestShader()
        {
            // Check what render pipeline we're using
            var renderPipelineAsset = UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline;
            bool isURP = renderPipelineAsset != null && renderPipelineAsset.GetType().Name.Contains("Universal");
            bool isHDRP = renderPipelineAsset != null && renderPipelineAsset.GetType().Name.Contains("HighDefinition");

            string[] shaderPriority;
            
            if (isURP)
            {
                shaderPriority = new string[] {
                    "Universal Render Pipeline/Lit",
                    "Universal Render Pipeline/Simple Lit",
                    "Universal Render Pipeline/Unlit"
                };
                Debug.Log("🔍 Detected URP - trying URP shaders first");
            }
            else if (isHDRP)
            {
                shaderPriority = new string[] {
                    "HDRP/Lit",
                    "HDRP/Unlit"
                };
                Debug.Log("🔍 Detected HDRP - trying HDRP shaders first");
            }
            else
            {
                shaderPriority = new string[] {
                    "Standard",
                    "Legacy Shaders/Diffuse",
                    "Legacy Shaders/VertexLit"
                };
                Debug.Log("🔍 Detected Built-in RP - trying Standard shaders first");
            }

            // Try to find a working shader
            foreach (string shaderName in shaderPriority)
            {
                Shader shader = Shader.Find(shaderName);
                if (shader != null)
                {
                    Debug.Log($"✅ Found working shader: {shaderName}");
                    return shader;
                }
                else
                {
                    Debug.LogWarning($"⚠️ Shader not found: {shaderName}");
                }
            }

            // Absolute fallback - try any shader that exists
            string[] fallbackShaders = {
                "Sprites/Default",
                "UI/Default",
                "Hidden/Internal-Colored"
            };

            foreach (string shaderName in fallbackShaders)
            {
                Shader shader = Shader.Find(shaderName);
                if (shader != null)
                {
                    Debug.LogWarning($"⚠️ Using fallback shader: {shaderName}");
                    return shader;
                }
            }

            return null; // No shader found
        }

        /// <summary>
        /// Set material color using the appropriate property for the shader
        /// </summary>
        private void SetMaterialColor(Material mat, Color color)
        {
            // Try different color property names
            if (mat.HasProperty("_BaseColor"))
            {
                mat.SetColor("_BaseColor", color); // URP/HDRP
                Debug.Log($"Set color using _BaseColor property");
            }
            else if (mat.HasProperty("_Color"))
            {
                mat.SetColor("_Color", color); // Standard/Legacy
                Debug.Log($"Set color using _Color property");
            }
            else if (mat.HasProperty("_MainColor"))
            {
                mat.SetColor("_MainColor", color); // Some custom shaders
                Debug.Log($"Set color using _MainColor property");
            }
            else
            {
                // Try the generic color property
                try
                {
                    mat.color = color;
                    Debug.Log($"Set color using generic color property");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"❌ Failed to set color on material: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Configure material for transparency (works with multiple shader types)
        /// </summary>
        private void ConfigureTransparentMaterial(Material mat)
        {
            // URP transparency setup
            if (mat.HasProperty("_Surface"))
            {
                mat.SetFloat("_Surface", 1); // Transparent
                mat.SetFloat("_Blend", 0); // Alpha blend
            }
            // Standard shader transparency setup
            else if (mat.HasProperty("_Mode"))
            {
                mat.SetFloat("_Mode", 3); // Transparent mode
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            }
            
            mat.renderQueue = 3000;
        }

        /// <summary>
        /// Create a fallback material when shader detection fails
        /// Uses Unity's built-in default material as base
        /// </summary>
        private Material CreateFallbackMaterial(string name, Color color)
        {
            // Try to create a material from Unity's default material
            Material defaultMat = Resources.GetBuiltinResource<Material>("Default-Material.mat");
            if (defaultMat != null)
            {
                Material mat = new Material(defaultMat);
                mat.name = name;
                
                // Try to set color
                try
                {
                    mat.color = color;
                    Debug.Log($"⚠️ Created fallback material '{name}' from Default-Material");
                    return mat;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"❌ Failed to set color on fallback material: {e.Message}");
                }
            }

            // Last resort: create material with any available shader
            Shader[] allShaders = Resources.FindObjectsOfTypeAll<Shader>();
            foreach (Shader shader in allShaders)
            {
                if (shader.name.Contains("Standard") || shader.name.Contains("Diffuse") || shader.name.Contains("Lit"))
                {
                    try
                    {
                        Material mat = new Material(shader);
                        mat.name = name;
                        mat.color = color;
                        Debug.Log($"⚠️ Created emergency fallback material '{name}' with shader: {shader.name}");
                        return mat;
                    }
                    catch
                    {
                        continue; // Try next shader
                    }
                }
            }

            Debug.LogError($"❌ Complete failure to create material '{name}' - all methods failed!");
            return null;
        }
    }
}
