using System.Collections.Generic;

public class MotionRecording
{
    public List<TransformSnapshot> headSnapshots;
    public List<TransformSnapshot> leftHandSnapshots;
    public List<TransformSnapshot> rightHandSnapshots;
    public float duration;

    public MotionRecording()
    {
        headSnapshots = new List<TransformSnapshot>();
        leftHandSnapshots = new List<TransformSnapshot>();
        rightHandSnapshots = new List<TransformSnapshot>();
        duration = 0f;
    }

    public void Clear()
    {
        headSnapshots.Clear();
        leftHandSnapshots.Clear();
        rightHandSnapshots.Clear();
        duration = 0f;
    }
}

