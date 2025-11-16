# Core Export - Ready for Migration

This folder contains a clean copy of the Core assets without .meta files, ready to be imported into another Unity project.

## Contents

- **Scenes/** (2 files): main.unity, danlilaDev.unity
- **Materials/** (6 files): Black, Gray, Gray 1, HandLeft, HandRight, Head
- **Prefabs/** (5 files): HeadGhostTracker, LeftHandGhostTracker, RightHandGhostTracker, ShadowTracker, SmoothGhostWrapper
- **main_game/** (C# scripts & assembly definition)
  - GodmodeController.cs
  - SmoothFollowTracker.cs
  - SmoothGhostInitializer.cs
  - MainGame.asmdef
  - **MotionRecording/**
    - **Runtime/**: MotionPlayer.cs, MotionRecorder.cs, MotionRecording.cs, TransformSnapshot.cs
    - **Editor/**: GhostPrefabGenerator.cs

## How to Import

1. Open your target Unity project
2. Drag the entire `CoreExport` folder into your Assets directory
3. Unity will automatically generate new .meta files compatible with your project version
4. All scripts, prefabs, materials, and scenes will be imported with fresh GUIDs

## Features

- Motion recording system with 5 sequential ghosts
- Smooth ghost followers with configurable smoothness
- Godmode controller for testing
- Ghost prefab generation editor tool

Exported: November 2025
