using System;
using UnityEngine;

public class SmoothGhostInitializer : MonoBehaviour
{
    [SerializeField] private SmoothFollowTracker smoothHeadTracker;
    [SerializeField] private SmoothFollowTracker smoothLeftHandTracker;
    [SerializeField] private SmoothFollowTracker smoothRightHandTracker;
    [SerializeField] private SmoothFollowTracker smoothBodyTracker;

    public void Initialize(Transform headTarget, Transform leftHandTarget, Transform rightHandTarget)
    {
        if (smoothHeadTracker != null && headTarget != null){
            smoothHeadTracker.SetTarget(headTarget);
            smoothBodyTracker.SetTarget(headTarget);
        }
        
        if (smoothLeftHandTracker != null && leftHandTarget != null)
            smoothLeftHandTracker.SetTarget(leftHandTarget);
        
        if (smoothRightHandTracker != null && rightHandTarget != null)
            smoothRightHandTracker.SetTarget(rightHandTarget);
    }

    public Transform GetTargetPosition()
    {
        return smoothHeadTracker.transform;
    }
}

