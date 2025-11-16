using System.Collections.Generic;
using UnityEngine;

public class MotionPlayer : MonoBehaviour
{
    private List<TransformSnapshot> snapshots;
    private float duration;
    private float playbackTime;
    private bool initialized;

    public void Initialize(List<TransformSnapshot> snapshots, float duration)
    {
        this.snapshots = snapshots;
        this.duration = duration;
        this.playbackTime = 0f;
        this.initialized = true;
    }

    void Update()
    {
        if (!initialized || snapshots == null || snapshots.Count == 0) return;

        playbackTime += Time.deltaTime;

        if (playbackTime >= duration)
        {
            playbackTime = playbackTime % duration;
        }

        ApplySnapshotAtTime(playbackTime);
    }

    private void ApplySnapshotAtTime(float time)
    {
        if (snapshots.Count == 1)
        {
            transform.localPosition = snapshots[0].localPosition;
            transform.localRotation = snapshots[0].localRotation;
            return;
        }

        int currentIndex = -1;
        int nextIndex = -1;

        for (int i = 0; i < snapshots.Count - 1; i++)
        {
            if (time >= snapshots[i].timestamp && time < snapshots[i + 1].timestamp)
            {
                currentIndex = i;
                nextIndex = i + 1;
                break;
            }
        }

        if (currentIndex == -1)
        {
            if (time >= snapshots[snapshots.Count - 1].timestamp)
            {
                currentIndex = snapshots.Count - 1;
                nextIndex = 0;
            }
            else
            {
                currentIndex = 0;
                nextIndex = 1;
            }
        }

        TransformSnapshot current = snapshots[currentIndex];
        TransformSnapshot next = snapshots[nextIndex];

        float currentTime = current.timestamp;
        float nextTime = next.timestamp;

        if (nextIndex == 0)
        {
            nextTime = duration;
        }

        float t = Mathf.InverseLerp(currentTime, nextTime, time);

        transform.localPosition = Vector3.Lerp(current.localPosition, next.localPosition, t);
        transform.localRotation = Quaternion.Slerp(current.localRotation, next.localRotation, t);
    }
}

