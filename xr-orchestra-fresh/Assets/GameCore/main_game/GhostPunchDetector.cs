using UnityEngine;

public class GhostPunchDetector : MonoBehaviour
{
    [SerializeField] private float punchVelocityThreshold = 2f;
    
    private InstrumentManager instrumentManager;

    public void SetInstrumentManager(InstrumentManager manager)
    {
        instrumentManager = manager;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[GhostPunchDetector] Trigger entered by: {other.gameObject.name}");
        
        if (instrumentManager == null)
        {
            Debug.LogWarning("[GhostPunchDetector] No InstrumentManager assigned!");
            return;
        }

        HandVelocityTracker velocityTracker = other.GetComponent<HandVelocityTracker>();
        if (velocityTracker == null)
        {
            Debug.Log($"[GhostPunchDetector] No HandVelocityTracker on {other.gameObject.name}");
            return;
        }

        float velocityMagnitude = velocityTracker.Velocity.magnitude;
        Debug.Log($"[GhostPunchDetector] Hand velocity: {velocityMagnitude:F2} m/s (threshold: {punchVelocityThreshold})");

        if (velocityMagnitude >= punchVelocityThreshold)
        {
            Debug.Log("[GhostPunchDetector] PUNCH DETECTED! Calling InstrumentManager.OnGhostPunched()");
            instrumentManager.OnGhostPunched();
        }
    }
}

