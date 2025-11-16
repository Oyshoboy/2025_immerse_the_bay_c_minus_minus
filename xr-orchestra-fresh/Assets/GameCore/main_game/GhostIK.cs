using UnityEngine;

/// <summary>
/// Simple VR IK for Ghost character - hands and head only
/// </summary>
public class GhostIK : MonoBehaviour
{
    [Header("Required Components")]
    public Animator animator;

    [Header("VR Trackers")]
    public Transform headTracker;
    public Transform leftHandTracker;
    public Transform rightHandTracker;

    [Header("IK Settings")]
    [Range(0f, 1f)]
    public float handIKWeight = 1f;
    
    [Range(0f, 1f)]
    public float headLookWeight = 1f;

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        // === HAND IK ===
        if (leftHandTracker != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, handIKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, handIKWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTracker.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTracker.rotation);
        }

        if (rightHandTracker != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handIKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, handIKWeight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTracker.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTracker.rotation);
        }

        // === HEAD LOOK IK ===
        if (headTracker != null)
        {
            // Parameters: (weight, bodyWeight, headWeight, eyesWeight, clampWeight)
            animator.SetLookAtWeight(headLookWeight, 0.3f, 0.8f, 1f, 0.5f);
            Vector3 lookPosition = headTracker.position + headTracker.forward * 2f;
            animator.SetLookAtPosition(lookPosition);
        }
        else
        {
            animator.SetLookAtWeight(0f);
        }
    }
}
