using UnityEngine;
using UnityEditor;

namespace Chess3D
{
    /// <summary>
    /// Editor script to quickly set up the chess scene for testing
    /// Run this from Tools menu to instantly create a playable chess game
    /// </summary>
    public class QuickChessSetup : EditorWindow
    {
        [MenuItem("Tools/Chess3D/Quick Setup Scene")]
        public static void SetupChessScene()
        {
            // Clear the scene
            var existingObjects = FindObjectsOfType<GameObject>();
            foreach (var obj in existingObjects)
            {
                if (obj.name != "Main Camera" && obj.name != "Directional Light")
                {
                    DestroyImmediate(obj);
                }
            }

            // Create the setup manager
            GameObject setupManager = new GameObject("ChessSetupManager");
            ChessGameSetup setup = setupManager.AddComponent<ChessGameSetup>();
            setup.autoSetupOnStart = true;
            setup.createBasicMaterials = true;

            // Save the scene
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene(), 
                "Assets/Scenes/ChessGame.unity"
            );

            Debug.Log("Chess scene setup complete! Press Play to start the game!");
        }

        [MenuItem("Tools/Chess3D/Build Standalone")]
        public static void BuildStandalone()
        {
            // Set build settings
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new[] { "Assets/Scenes/ChessGame.unity" };
            buildPlayerOptions.locationPathName = "Builds/Chess3D_Mac.app";
            buildPlayerOptions.target = BuildTarget.StandaloneOSX;
            buildPlayerOptions.options = BuildOptions.None;

            // Create builds directory if it doesn't exist
            System.IO.Directory.CreateDirectory("Builds");

            // Build the game
            BuildPipeline.BuildPlayer(buildPlayerOptions);
            
            Debug.Log("Build complete! Check the Builds/ folder.");
        }
    }
}
