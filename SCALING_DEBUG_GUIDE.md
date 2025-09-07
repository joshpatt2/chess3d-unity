# 🐛 Chess Piece Scaling Debug Guide

## Issue: Chess Pieces Not Visible After Scaling Update

### 🔍 **Quick Diagnosis Steps**

#### **Step 1: Check Unity Inspector Values**
1. Select the **Board** object in your scene
2. Look at **"Piece Positioning & Scale"** section
3. **Expected values:**
   - Piece Scale: `0.7` (NOT 0!)
   - Piece Height Offset: `0.5`
   - Center Pieces On Tiles: `✓ True`

#### **Step 2: If Values Are Wrong (likely cause)**
**Problem:** Unity serialized old default values (probably 0 for scale)

**Quick Fix:**
1. In Unity Inspector, manually set **Piece Scale** to `0.7`
2. Right-click Board component → **"Validate Piece Settings"**
3. Right-click Board component → **"Update All Piece Positions and Scale"**

#### **Step 3: Check Console for Debug Output**
Look for these messages when starting the game:
- `🎯 Piece position calculated: (0,0) -> (0.5, 0.5, 0.5), Scale: 0.7`
- `🎮 Created piece: Pawn_White_0_1 at (0.5, 0.5, 1.5) with scale (0.7, 0.7, 0.7)`

**If you see scale (0, 0, 0):** That's the problem!

### 🛠️ **Fixes by Symptom**

#### **Symptom: Pieces completely invisible**
**Cause:** Scale is 0 or pieces positioned far away
**Fix:** 
```
1. Set Piece Scale to 0.7 in Inspector
2. Right-click Board → "Update All Piece Positions and Scale"
```

#### **Symptom: Pieces too small to see**
**Cause:** Scale is very small (like 0.1)
**Fix:**
```
1. Increase Piece Scale to 0.7-1.0
2. Right-click Board → "Update All Piece Positions and Scale"
```

#### **Symptom: Pieces in wrong positions**
**Cause:** Position calculation error
**Fix:**
```
1. Set Center Pieces On Tiles to True
2. Right-click Board → "Update All Piece Positions and Scale"
```

### 🔧 **Emergency Reset**

If nothing works, manually reset in Unity Inspector:
- **Piece Scale:** `0.7`
- **Piece Height Offset:** `0.5` 
- **Center Pieces On Tiles:** `True`

Then: **Right-click Board → "Update All Piece Positions and Scale"**

### 🎯 **Root Cause Analysis**

**Most Likely Issue:** Unity's serialization kept old default values instead of using the new defaults we added to the code.

**Why This Happens:** 
- Unity serializes public fields in scene files
- When we changed the default values in code, existing scenes kept the old values
- If the old default was `0f`, pieces become invisible

**Permanent Fix:** Always set proper values in Unity Inspector after code changes.

### 📋 **Verification Checklist**

After fixing:
- [ ] Can see chess pieces on the board
- [ ] Pieces are appropriately sized (not huge, not tiny)
- [ ] Pieces are centered on squares
- [ ] Console shows proper scale values (like 0.7, not 0)
- [ ] Moving pieces works correctly

### 🚨 **If Still Broken**

Check these in Unity Console:
1. Any error messages about missing prefabs?
2. Are the debug messages showing reasonable position/scale values?
3. Are pieces being created at all? (Look for creation messages)

**Last Resort:** Delete all existing pieces in the scene and restart the game to recreate them with proper values.
