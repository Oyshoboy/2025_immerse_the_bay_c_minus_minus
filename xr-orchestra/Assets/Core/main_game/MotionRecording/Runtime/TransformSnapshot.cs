using UnityEngine;

[System.Serializable]
public struct TransformSnapshot
{
    public Vector3 localPosition;
    public Quaternion localRotation;
    public float timestamp;

    public TransformSnapshot(Vector3 localPosition, Quaternion localRotation, float timestamp)
    {
        this.localPosition = localPosition;
        this.localRotation = localRotation;
        this.timestamp = timestamp;
    }
}

