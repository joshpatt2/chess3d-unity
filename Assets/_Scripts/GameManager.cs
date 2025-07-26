using System.Collections.Generic;
using UnityEngine;

namespace Chess3D
{
    /// <summary>
    /// Central singleton class that manages the overall game flow and state
    /// Handles turn management, game state, and coordinates between systems
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        public PlayerColor currentPlayer = PlayerColor.White;
        public bool isGameOver = false;

        [Header("References")]
        public Board board;
        public InputController inputController;

        // Game state tracking
        private Piece selectedPiece;
        private Vector2Int selectedPosition;
        private List<Vector2Int> availableMoves = new List<Vector2Int>();
        private List<GameObject> moveHighlights = new List<GameObject>();
        
        // Captured pieces tracking
        private List<Piece> capturedWhitePieces = new List<Piece>();
        private List<Piece> capturedBlackPieces = new List<Piece>();

        [Header("Highlight Prefabs")]
        public GameObject moveHighlightPrefab;
        public Material selectedPieceMaterial;

        void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("GameManager Instance created");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            // Initialize game
            Debug.Log($"Chess game started! {currentPlayer} to move.");
        }

        /// <summary>
        /// Called by InputController when a tile is clicked
        /// </summary>
        public void OnTileClicked(Vector2Int position)
        {
            if (isGameOver) return;

            Debug.Log($"🖱️ Tile clicked at position {position}");
            
            Piece clickedPiece = board.GetPieceAt(position);
            Debug.Log($"🔍 Piece at position: {(clickedPiece != null ? clickedPiece.GetType().Name + "_" + clickedPiece.color : "null")}");

            if (selectedPiece == null)
            {
                // No piece selected - try to select a piece
                if (clickedPiece != null && clickedPiece.color == currentPlayer)
                {
                    Debug.Log($"🎯 Attempting to select {clickedPiece.GetType().Name}");
                    SelectPiece(clickedPiece, position);
                }
            }
            else
            {
                // Piece already selected
                if (clickedPiece != null && clickedPiece.color == currentPlayer)
                {
                    // Clicking on another friendly piece - change selection
                    DeselectPiece();
                    SelectPiece(clickedPiece, position);
                }
                else if (availableMoves.Contains(position))
                {
                    // Valid move - execute it
                    ExecuteMove(selectedPosition, position);
                }
                else
                {
                    // Invalid move - deselect
                    DeselectPiece();
                }
            }
        }

        /// <summary>
        /// Select a piece and show its available moves
        /// </summary>
        private void SelectPiece(Piece piece, Vector2Int position)
        {
            Debug.Log($"🎮 SelectPiece called for {piece.GetType().Name} at {position}");
            
            selectedPiece = piece;
            selectedPosition = position;

            // Highlight the selected piece
            HighlightSelectedPiece(piece);

            Debug.Log($"🔄 Getting available moves for {piece.GetType().Name}...");
            
            // Get and display available moves
            try
            {
                availableMoves = piece.GetAvailableMoves(position, board);
                Debug.Log($"✅ Got {availableMoves.Count} available moves from piece");
                
                availableMoves = FilterLegalMoves(availableMoves, position);
                Debug.Log($"✅ Filtered to {availableMoves.Count} legal moves");
                
                ShowAvailableMoves(availableMoves);
                Debug.Log($"✅ Selected {piece.GetType().Name} at {position}. Available moves: {availableMoves.Count}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Exception in SelectPiece for {piece.GetType().Name}: {e.Message}\n{e.StackTrace}");
                DeselectPiece(); // Clean up on error
            }
        }

        /// <summary>
        /// Deselect the current piece and clear highlights
        /// </summary>
        private void DeselectPiece()
        {
            if (selectedPiece != null)
            {
                // Remove piece highlight
                RemoveSelectedPieceHighlight(selectedPiece);
                
                selectedPiece = null;
                selectedPosition = Vector2Int.zero;
                availableMoves.Clear();
                
                // Clear move highlights
                ClearMoveHighlights();
            }
        }

        /// <summary>
        /// Execute a move and handle all related game logic
        /// </summary>
        private void ExecuteMove(Vector2Int from, Vector2Int to)
        {
            // Check if this is a castling move
            if (selectedPiece is King && Mathf.Abs(to.x - from.x) == 2)
            {
                ExecuteCastlingMove(from, to);
            }
            else
            {
                // Regular move
                board.MovePiece(from, to);
            }

            // Clear selection and highlights
            DeselectPiece();

            // Check for game-ending conditions
            if (IsCheckmate(GetOpponentColor(currentPlayer)))
            {
                EndGame($"Checkmate! {currentPlayer} wins!");
            }
            else if (IsStalemate(GetOpponentColor(currentPlayer)))
            {
                EndGame("Stalemate! The game is a draw.");
            }
            else
            {
                // Switch turns
                NextPlayer();
                
                // Check if new player is in check
                if (IsInCheck(currentPlayer))
                {
                    Debug.Log($"{currentPlayer} is in check!");
                }
            }
        }

        /// <summary>
        /// Execute a castling move (moves both king and rook)
        /// </summary>
        private void ExecuteCastlingMove(Vector2Int kingFrom, Vector2Int kingTo)
        {
            Debug.Log($"Executing castling move: King from {kingFrom} to {kingTo}");

            // Determine if this is kingside or queenside castling
            bool isKingside = kingTo.x > kingFrom.x;
            
            // Calculate rook positions
            Vector2Int rookFrom = new Vector2Int(isKingside ? 7 : 0, kingFrom.y);
            Vector2Int rookTo = new Vector2Int(isKingside ? kingTo.x - 1 : kingTo.x + 1, kingFrom.y);

            // Move the king
            board.MovePiece(kingFrom, kingTo);
            
            // Move the rook
            board.MovePiece(rookFrom, rookTo);
            
            Debug.Log($"Castling complete: Rook moved from {rookFrom} to {rookTo}");
        }

        /// <summary>
        /// Switch to the next player's turn
        /// </summary>
        private void NextPlayer()
        {
            currentPlayer = (currentPlayer == PlayerColor.White) ? PlayerColor.Black : PlayerColor.White;
            Debug.Log($"{currentPlayer}'s turn to move.");
        }

        /// <summary>
        /// Get the opposite color
        /// </summary>
        private PlayerColor GetOpponentColor(PlayerColor color)
        {
            return (color == PlayerColor.White) ? PlayerColor.Black : PlayerColor.White;
        }

        /// <summary>
        /// Filter pseudo-legal moves to only include truly legal moves (no self-check)
        /// </summary>
        private List<Vector2Int> FilterLegalMoves(List<Vector2Int> moves, Vector2Int piecePosition)
        {
            Debug.Log($"🔍 FilterLegalMoves: Starting with {moves.Count} moves for piece at {piecePosition}");
            List<Vector2Int> legalMoves = new List<Vector2Int>();

            foreach (Vector2Int move in moves)
            {
                Debug.Log($"🔍 Checking if move {piecePosition} -> {move} is legal...");
                try
                {
                    if (IsMoveLegal(piecePosition, move))
                    {
                        legalMoves.Add(move);
                        Debug.Log($"✅ Move {piecePosition} -> {move} is legal");
                    }
                    else
                    {
                        Debug.Log($"❌ Move {piecePosition} -> {move} is NOT legal");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"❌ Exception checking move {piecePosition} -> {move}: {e.Message}\n{e.StackTrace}");
                }
            }

            Debug.Log($"🔍 FilterLegalMoves: Returning {legalMoves.Count} legal moves");
            return legalMoves;
        }

        /// <summary>
        /// Check if a move is legal (doesn't result in own king being in check)
        /// Uses "virtual move" simulation that only affects logical board, not visual pieces
        /// </summary>
        private bool IsMoveLegal(Vector2Int from, Vector2Int to)
        {
            // Store original state
            Piece movingPiece = board.GetPieceAt(from);
            Piece capturedPiece = board.GetPieceAt(to);
            
            if (movingPiece == null)
            {
                return false; // No piece to move
            }

            // Simulate the move in the logical board ONLY (no visual changes)
            board.SimulateMove(from, to);

            // Check if own king is in check after the move
            bool isLegal = !IsInCheck(movingPiece.color);

            // Restore original logical state
            board.UndoSimulatedMove(from, to, capturedPiece);

            return isLegal;
        }

        /// <summary>
        /// Check if a player's king is currently in check
        /// </summary>
        public bool IsInCheck(PlayerColor playerColor)
        {
            // Find the king
            Vector2Int kingPosition = FindKing(playerColor);
            if (kingPosition == new Vector2Int(-1, -1))
            {
                Debug.LogError($"King not found for {playerColor}!");
                return false;
            }

            // Check if any enemy piece can attack the king
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Piece piece = board.GetPieceAt(x, y);
                    if (piece != null && piece.color != playerColor)
                    {
                        List<Vector2Int> enemyMoves = piece.GetAvailableMoves(new Vector2Int(x, y), board);
                        if (enemyMoves.Contains(kingPosition))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Find the king's position for a given color
        /// </summary>
        private Vector2Int FindKing(PlayerColor color)
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Piece piece = board.GetPieceAt(x, y);
                    if (piece != null && piece.color == color && piece is King)
                    {
                        return new Vector2Int(x, y);
                    }
                }
            }
            return new Vector2Int(-1, -1); // King not found
        }

        /// <summary>
        /// Check if a player is in checkmate
        /// </summary>
        private bool IsCheckmate(PlayerColor playerColor)
        {
            return IsInCheck(playerColor) && GetAllLegalMoves(playerColor).Count == 0;
        }

        /// <summary>
        /// Check if a player is in stalemate
        /// </summary>
        private bool IsStalemate(PlayerColor playerColor)
        {
            return !IsInCheck(playerColor) && GetAllLegalMoves(playerColor).Count == 0;
        }

        /// <summary>
        /// Get all legal moves for a player
        /// </summary>
        private List<Vector2Int> GetAllLegalMoves(PlayerColor playerColor)
        {
            List<Vector2Int> allMoves = new List<Vector2Int>();

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Piece piece = board.GetPieceAt(x, y);
                    if (piece != null && piece.color == playerColor)
                    {
                        Vector2Int position = new Vector2Int(x, y);
                        List<Vector2Int> pieceMoves = piece.GetAvailableMoves(position, board);
                        List<Vector2Int> legalMoves = FilterLegalMoves(pieceMoves, position);
                        allMoves.AddRange(legalMoves);
                    }
                }
            }

            return allMoves;
        }

        /// <summary>
        /// Called when a piece is captured
        /// </summary>
        public void OnPieceCaptured(Piece capturedPiece)
        {
            if (capturedPiece.color == PlayerColor.White)
            {
                capturedWhitePieces.Add(capturedPiece);
            }
            else
            {
                capturedBlackPieces.Add(capturedPiece);
            }

            Debug.Log($"{capturedPiece.color} {capturedPiece.GetType().Name} captured!");
        }

        /// <summary>
        /// End the game with a message
        /// </summary>
        private void EndGame(string message)
        {
            isGameOver = true;
            Debug.Log(message);
            // TODO: Show game over UI
        }

        /// <summary>
        /// Highlight the selected piece
        /// </summary>
        private void HighlightSelectedPiece(Piece piece)
        {
            Renderer renderer = piece.GetComponent<Renderer>();
            if (renderer != null && selectedPieceMaterial != null)
            {
                renderer.material = selectedPieceMaterial;
            }
        }

        /// <summary>
        /// Remove highlight from the selected piece
        /// </summary>
        private void RemoveSelectedPieceHighlight(Piece piece)
        {
            Renderer renderer = piece.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Restore original material based on piece color
                Material originalMaterial = (piece.color == PlayerColor.White) ? 
                    board.whitePieceMaterial : board.blackPieceMaterial;
                renderer.material = originalMaterial;
            }
        }

        /// <summary>
        /// Show visual indicators for available moves
        /// </summary>
        private void ShowAvailableMoves(List<Vector2Int> moves)
        {
            foreach (Vector2Int move in moves)
            {
                if (moveHighlightPrefab != null)
                {
                    GameObject highlight = Instantiate(moveHighlightPrefab);
                    highlight.transform.position = new Vector3(move.x, 0.1f, move.y);
                    moveHighlights.Add(highlight);
                }
            }
        }

        /// <summary>
        /// Clear all move highlight indicators
        /// </summary>
        private void ClearMoveHighlights()
        {
            foreach (GameObject highlight in moveHighlights)
            {
                if (highlight != null)
                {
                    Destroy(highlight);
                }
            }
            moveHighlights.Clear();
        }
    }
}
