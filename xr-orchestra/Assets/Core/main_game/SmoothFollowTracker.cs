using UnityEngine;

public class SmoothFollowTracker : MonoBehaviour
{
    [SerializeField] private Transform target;
    
    [SerializeField] private float positionSmoothness = 8f;
    [SerializeField] private float rotationSmoothness = 6f;

    private void LateUpdate()
    {
        if (target == null) return;

        SmoothFollowPosition();
        SmoothFollowRotation();
    }

    private void SmoothFollowPosition()
    {
        float positionBlend = 1f - Mathf.Exp(-positionSmoothness * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, target.position, positionBlend);
    }

    private void SmoothFollowRotation()
    {
        float rotationBlend = 1f - Mathf.Exp(-rotationSmoothness * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, rotationBlend);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public Transform GetTarget()
    {
        return target;
    }
}

