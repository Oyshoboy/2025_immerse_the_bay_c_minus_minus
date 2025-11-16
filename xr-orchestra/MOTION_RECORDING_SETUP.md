# Motion Recording Setup Instructions

## Quick Setup

### 1. Generate Ghost Prefabs
In Unity Editor:
- Go to menu: `Tools > Generate Ghost Prefabs`
- This creates 3 prefabs in `Assets/Prefabs/`:
  - HeadGhost.prefab
  - LeftHandGhost.prefab
  - RightHandGhost.prefab

### 2. Configure MotionRecorder Component
Add MotionRecorder to your Wrapper GameObject:

1. Select Wrapper in Hierarchy
2. Add Component > Motion Recorder
3. Assign references:
   - **Wrapper**: Drag the Wrapper GameObject itself
   - **Head**: Drag your Head transform
   - **Left Hand**: Drag your LeftHand transform
   - **Right Hand**: Drag your RightHand transform
4. Assign ghost prefabs:
   - **Head Ghost Prefab**: Assets/Prefabs/HeadGhost.prefab
   - **Left Hand Ghost Prefab**: Assets/Prefabs/LeftHandGhost.prefab
   - **Right Hand Ghost Prefab**: Assets/Prefabs/RightHandGhost.prefab
5. Settings (default values work):
   - Recording Duration: 10 seconds
   - Sample Rate: 60 fps
   - Record Key: R

### 3. Usage
- Press **R** key to start recording
- Move your head and hands for up to 10 seconds
- After 10 seconds, ghost objects spawn and loop your movements infinitely
- Press **R** again to record a new loop (replaces current ghosts)

## How It Works

**Recording Phase**:
- Captures local position/rotation of Head, LeftHand, RightHand at 60fps
- Stores ~600 samples per object over 10 seconds
- All positions are relative to Wrapper parent

**Playback Phase**:
- Spawns 3 ghost GameObjects as children of Wrapper
- Each ghost uses MotionPlayer component
- Loops recorded motion infinitely with smooth interpolation
- You continue moving normally while ghosts replay

## Customization

### Change Ghost Appearance
Edit the generated prefabs in `Assets/Prefabs/` or modify `GhostPrefabGenerator.cs` and regenerate.

### Change Recording Duration
Adjust "Recording Duration" on MotionRecorder component (default: 10s).

### Change Input Key
Modify "Record Key" on MotionRecorder component (default: R key).

### Use VR Controller Button
In `MotionRecorder.cs`, replace `HandleInput()` method:

```csharp
private void HandleInput()
{
    // Example for Oculus/Meta Quest
    if (OVRInput.GetDown(OVRInput.Button.One) && state == RecorderState.Idle)
    {
        StartRecording();
    }
}
```

## Memory Usage
- ~58 KB per 10-second recording (3 objects @ 60fps)
- Negligible CPU impact during playback

