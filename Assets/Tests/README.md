# Chess3D Unit Tests

This directory contains comprehensive unit tests for the Chess3D project's piece movement validation system.

## Overview

The test suite validates all chess piece movement logic from starting positions, ensuring:
- Correct initial game state setup
- Valid piece movement patterns
- Proper blocking and capture mechanics
- Edge cases and corner conditions

## Test Structure

### Assembly Definitions
- `Chess3D.Tests.EditMode.asmdef` - Edit mode tests (pure logic, no GameObjects)
- `Chess3D.Tests.PlayMode.asmdef` - Play mode tests (for integration testing)
- `Chess3D.asmdef` - Main game scripts assembly (referenced by tests)

### Test Categories

#### InitialGameStateTests.cs
- Board initializes with pieces in correct starting positions
- All piece components are properly attached and configured
- Piece colors are correctly assigned (White/Black)
- Coordinate conversion utilities work correctly

#### PawnMovementTests.cs
- White/black pawn can move 1 square forward from starting position
- White/black pawn can move 2 squares forward from starting position (first move only)
- Pawns cannot move backward
- Pawns can capture diagonally when enemy piece present
- Pawns cannot move diagonally when no enemy piece to capture

#### KnightMovementTests.cs
- **Specific opening moves**: Nb1-c3, Nb1-a3, Ng1-f3, Ng1-h3
- Black knights can move to valid L-shaped positions
- Knights can jump over other pieces
- Complete L-shaped movement pattern validation
- Corner and edge position movement

#### RookMovementTests.cs
- Rooks cannot move initially (blocked by pawns)
- Horizontal and vertical sliding movement validation
- Proper blocking by friendly vs enemy pieces
- Cannot move diagonally

#### BishopMovementTests.cs
- Bishops cannot move initially (blocked by pawns)
- Diagonal sliding movement in all four directions
- Cannot move horizontally or vertically
- Color consistency (light/dark square binding)

#### QueenMovementTests.cs
- Queen cannot move initially (blocked by pawns)
- Validates queen combines rook + bishop movement
- Maximum mobility from center positions

#### KingMovementTests.cs
- King can move 1 square in any direction
- Cannot move more than 1 square
- Castling prerequisites (king/rook haven't moved, path clear)
- Corner and edge movement limitations

### Test Utilities

#### TestBoardHelper.cs
- `CreateStandardStartingPosition()` - Sets up initial chess position
- `CreateEmptyBoard()` - Creates empty 8x8 board
- `GetPieceMovesFromPosition()` - Tests piece movement using reflection
- `ChessNotationToCoords()` / `CoordsToChessNotation()` - Coordinate conversion

#### TestPieceFactory.cs
- `CreatePawn()`, `CreateRook()`, `CreateKnight()`, etc. - Create test pieces
- Handles GameObject creation and component attachment
- Sets piece color and moved status

#### MockBoard.cs
- Lightweight board implementation for testing
- Compatible with piece movement logic
- No Unity GameObject dependencies

## Running Tests

### In Unity Editor
1. Open Window > General > Test Runner
2. Select "EditMode" tab
3. Run specific test classes or entire suite
4. View results and detailed logs

### Command Line (CI/CD)
```bash
# Run edit mode tests
Unity -batchmode -quit -projectPath . -runTests -testPlatform EditMode -testResults results.xml

# Run play mode tests  
Unity -batchmode -quit -projectPath . -runTests -testPlatform PlayMode -testResults results.xml
```

### Test Validation Script
Use `TestValidation.cs` to verify test utilities work correctly:
1. Add `TestValidation` component to any GameObject
2. Enable "Run Validation On Start" 
3. Play the scene to see validation results
4. Or use "Run Test Validation" context menu

## Requirements Met

✅ **Initial Game State Testing**
- Board initializes with pieces in correct starting positions
- All piece components are properly attached and configured  
- Piece colors are correctly assigned (White/Black)

✅ **Piece Movement Tests**
- Pawn: 1/2 squares forward, diagonal capture, no backward movement
- Knight: L-shaped moves including specific openings (Nb1-c3, Ng1-f3, etc.)
- Rook: Horizontal/vertical movement, initially blocked by pawns
- Bishop: Diagonal movement, initially blocked by pawns
- Queen: Combination of rook + bishop movement
- King: 1 square movement, check avoidance, castling prerequisites

✅ **Technical Implementation**
- Uses Unity Test Framework 1.5.1
- Compatible with Unity 6 project
- Edit Mode tests for pure logic validation
- Efficient execution for CI/CD integration
- Comprehensive edge case coverage

## Notes

- Tests focus on the `GetAvailableMoves()` method of each piece
- Uses reflection to access private Board state for testing
- Tests are designed to run without Unity scene setup
- All coordinate positions use 0-based indexing (a1 = 0,0)
- Tests validate movement logic, not visual rendering or input handling

## Future Enhancements

- Add performance benchmarks for movement calculation
- Implement Play Mode tests for full integration testing
- Add tests for special moves (en passant, promotion)
- Extend castling tests with check detection
- Add tests for stalemate and checkmate detection