using UnityEngine;

namespace Chess3D
{
    /// <summary>
    /// RTS-style camera controller for chess game
    /// Provides panning (WASD) and zooming (mouse wheel) controls
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float panSpeed = 5f;
        public float zoomSpeed = 2f;
        public float minZoom = 2f;
        public float maxZoom = 15f;

        [Header("Boundaries")]
        public Vector2 panBoundary = new Vector2(10f, 10f);

        private Camera playerCamera;
        private Vector3 initialPosition;

        void Start()
        {
            // Get camera component (this script should be on camera rig, camera is child)
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
            {
                playerCamera = Camera.main;
            }

            // Store initial position
            initialPosition = transform.position;
        }

        void Update()
        {
            HandlePanning();
            HandleZooming();
        }

        /// <summary>
        /// Handle camera panning with WASD or arrow keys
        /// </summary>
        private void HandlePanning()
        {
            float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows
            float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down arrows

            if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
            {
                // Calculate movement direction
                Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
                
                // Apply movement
                Vector3 newPosition = transform.position + moveDirection * panSpeed * Time.deltaTime;
                
                // Clamp to boundaries
                newPosition.x = Mathf.Clamp(newPosition.x, -panBoundary.x, panBoundary.x);
                newPosition.z = Mathf.Clamp(newPosition.z, -panBoundary.y, panBoundary.y);
                
                transform.position = newPosition;
            }
        }

        /// <summary>
        /// Handle camera zooming with mouse scroll wheel
        /// </summary>
        private void HandleZooming()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (Mathf.Abs(scroll) > 0.01f)
            {
                // Move camera forward/backward along its local Z axis
                Vector3 currentPos = playerCamera.transform.localPosition;
                float newZ = currentPos.z + (scroll * zoomSpeed);
                
                // Clamp zoom
                newZ = Mathf.Clamp(newZ, -maxZoom, -minZoom);
                
                playerCamera.transform.localPosition = new Vector3(currentPos.x, currentPos.y, newZ);
            }
        }

        /// <summary>
        /// Reset camera to initial position
        /// </summary>
        public void ResetCamera()
        {
            transform.position = initialPosition;
            if (playerCamera != null)
            {
                playerCamera.transform.localPosition = Vector3.zero;
            }
        }

        /// <summary>
        /// Focus camera on the center of the board
        /// </summary>
        public void FocusOnBoard()
        {
            transform.position = new Vector3(3.5f, transform.position.y, 3.5f);
        }
    }
}
