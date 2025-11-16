using UnityEngine;

public class SmoothGhostInitializer : MonoBehaviour
{
    [SerializeField] private SmoothFollowTracker smoothHeadTracker;
    [SerializeField] private SmoothFollowTracker smoothLeftHandTracker;
    [SerializeField] private SmoothFollowTracker smoothRightHandTracker;

    public void Initialize(Transform headTarget, Transform leftHandTarget, Transform rightHandTarget)
    {
        if (smoothHeadTracker != null && headTarget != null)
            smoothHeadTracker.SetTarget(headTarget);
        
        if (smoothLeftHandTracker != null && leftHandTarget != null)
            smoothLeftHandTracker.SetTarget(leftHandTarget);
        
        if (smoothRightHandTracker != null && rightHandTarget != null)
            smoothRightHandTracker.SetTarget(rightHandTarget);
    }
}

