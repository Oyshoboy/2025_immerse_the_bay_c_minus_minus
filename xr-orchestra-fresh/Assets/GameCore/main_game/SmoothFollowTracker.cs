using UnityEngine;

public class SmoothFollowTracker : MonoBehaviour
{
    public enum AxisLock
    {
        None,
        X,
        Y,
        Z
    }

    [SerializeField] private Transform target;
    
    [SerializeField] private float positionSmoothness = 8f;
    [SerializeField] private float rotationSmoothness = 6f;
    [SerializeField] private AxisLock lockedRotationAxis = AxisLock.None;

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

        if (lockedRotationAxis == AxisLock.None)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, rotationBlend);
            return;
        }

        Vector3 currentEuler = transform.localEulerAngles;
        float lockedAngle = lockedRotationAxis switch
        {
            AxisLock.X => currentEuler.x,
            AxisLock.Y => currentEuler.y,
            AxisLock.Z => currentEuler.z,
            _ => 0f
        };

        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, rotationBlend);

        Vector3 newEuler = transform.localEulerAngles;
        switch (lockedRotationAxis)
        {
            case AxisLock.X:
                newEuler.x = lockedAngle;
                break;
            case AxisLock.Y:
                newEuler.y = lockedAngle;
                break;
            case AxisLock.Z:
                newEuler.z = lockedAngle;
                break;
        }
        transform.localEulerAngles = newEuler;
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

