using UnityEngine;

public class HandVelocityTracker : MonoBehaviour
{
    private Vector3 previousPosition;
    private Vector3 currentVelocity;
    
    public Vector3 Velocity => currentVelocity;

    private void Awake()
    {
        previousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        Vector3 currentPosition = transform.position;
        currentVelocity = (currentPosition - previousPosition) / Time.fixedDeltaTime;
        previousPosition = currentPosition;
    }
}

