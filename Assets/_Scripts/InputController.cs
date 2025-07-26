using UnityEngine;

namespace Chess3D
{
    /// <summary>
    /// Handles player input using raycasting to detect clicks on board tiles and pieces
    /// Communicates with GameManager to process player actions
    /// </summary>
    public class InputController : MonoBehaviour
    {
        [Header("Input Settings")]
        public Camera gameCamera;
        public LayerMask interactableLayerMask = -1; // All layers by default

        void Start()
        {
            // Use main camera if none assigned
            if (gameCamera == null)
            {
                gameCamera = Camera.main;
            }
        }

        void Update()
        {
            HandleMouseInput();
        }

        /// <summary>
        /// Process mouse input and perform raycasting
        /// </summary>
        private void HandleMouseInput()
        {
            // Check for left mouse button click
            if (Input.GetMouseButtonDown(0))
            {
                ProcessMouseClick();
            }
        }

        /// <summary>
        /// Cast a ray from the camera through the mouse position to detect clicks
        /// </summary>
        private void ProcessMouseClick()
        {
            // Create ray from camera through mouse position
            Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);

            // Perform raycast
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactableLayerMask))
            {
                ProcessHit(hit);
            }
        }

        /// <summary>
        /// Process the raycast hit and determine what was clicked
        /// </summary>
        private void ProcessHit(RaycastHit hit)
        {
            GameObject hitObject = hit.collider.gameObject;
            Debug.Log($"🎯 Raycast hit: {hitObject.name} at position {hit.point}");

            // Safety check
            if (hitObject == null)
            {
                Debug.LogWarning("⚠️ Hit object is null!");
                return;
            }

            // Try to get board position from the hit
            Vector2Int? boardPosition = GetBoardPosition(hitObject, hit.point);

            if (boardPosition.HasValue)
            {
                Debug.Log($"Board position clicked: {boardPosition.Value}");
                
                // Check if GameManager exists before calling
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnTileClicked(boardPosition.Value);
                }
                else
                {
                    Debug.LogError("GameManager.Instance is null!");
                }
            }
            else
            {
                Debug.LogWarning($"⚠️ Could not determine board position for object: {hitObject.name}");
            }
        }

        /// <summary>
        /// Convert a world position or GameObject to board coordinates
        /// </summary>
        private Vector2Int? GetBoardPosition(GameObject hitObject, Vector3 worldPosition)
        {
            Vector2Int boardPos;

            // Method 1: Try to get position from object name (if it's a tile)
            if (hitObject.CompareTag("Untagged") && hitObject.name.StartsWith("Tile"))
            {
                boardPos = GetPositionFromTileName(hitObject.name);
                if (IsValidBoardPosition(boardPos))
                {
                    return boardPos;
                }
            }

            // Method 2: Try to get position from piece (if it's a piece)
            if (hitObject.CompareTag("Untagged") && (hitObject.name.Contains("Pawn") || hitObject.name.Contains("Rook") || 
                hitObject.name.Contains("Knight") || hitObject.name.Contains("Bishop") || 
                hitObject.name.Contains("Queen") || hitObject.name.Contains("King")))
            {
                boardPos = GetPositionFromWorldPosition(hitObject.transform.position);
                if (IsValidBoardPosition(boardPos))
                {
                    return boardPos;
                }
            }

            // Method 3: Calculate position from world coordinates
            boardPos = GetPositionFromWorldPosition(worldPosition);
            if (IsValidBoardPosition(boardPos))
            {
                return boardPos;
            }

            return null; // Invalid position
        }

        /// <summary>
        /// Extract board position from tile GameObject name (format: "Tile_x_y")
        /// </summary>
        private Vector2Int GetPositionFromTileName(string tileName)
        {
            try
            {
                string[] parts = tileName.Split('_');
                if (parts.Length >= 3)
                {
                    int x = int.Parse(parts[1]);
                    int y = int.Parse(parts[2]);
                    return new Vector2Int(x, y);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Failed to parse tile name '{tileName}': {e.Message}");
            }

            return new Vector2Int(-1, -1); // Invalid position
        }

        /// <summary>
        /// Convert world position to board coordinates
        /// Assumes board tiles are positioned at integer coordinates (0,0) to (7,7)
        /// </summary>
        private Vector2Int GetPositionFromWorldPosition(Vector3 worldPos)
        {
            // Round to nearest integer to get board coordinates
            int x = Mathf.RoundToInt(worldPos.x);
            int y = Mathf.RoundToInt(worldPos.z); // Note: using Z for board Y coordinate
            
            return new Vector2Int(x, y);
        }

        /// <summary>
        /// Check if a board position is valid (within 0-7 range)
        /// </summary>
        private bool IsValidBoardPosition(Vector2Int position)
        {
            return position.x >= 0 && position.x < 8 && position.y >= 0 && position.y < 8;
        }

        /// <summary>
        /// Optional: Visualize the raycast in the scene view for debugging
        /// </summary>
        private void OnDrawGizmos()
        {
            if (gameCamera != null && Application.isPlaying)
            {
                // Draw ray from camera through mouse position (when mouse is over game view)
                Vector3 mousePos = Input.mousePosition;
                Ray ray = gameCamera.ScreenPointToRay(mousePos);
                
                Gizmos.color = Color.red;
                Gizmos.DrawRay(ray.origin, ray.direction * 20f);
            }
        }
    }
}
