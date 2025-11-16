using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class InstrumentManager : MonoBehaviour
{
    public enum InstrumentState { Idle, Recording, Playing }

    [Header("Debug Display")]
    [SerializeField] private TMP_Text debugText;
    [SerializeField] private int maxDebugLines = 10;

    [Header("Haptic Settings")]
    [SerializeField] private float hapticIntensity = 0.5f;
    [SerializeField] private float hapticDuration = 0.1f;
    [SerializeField] private OVRInput.Controller defaultHapticController = OVRInput.Controller.RTouch;

    [Header("References")]
    [SerializeField] private GodmodeController godmodeController;
    [SerializeField] private MotionRecorder motionRecorder;
    [SerializeField] private Material radialProgressMaterial;
    [SerializeField] private GameObject radialProgressObject;
    [SerializeField] private GameObject dummyFXObject;

    [Header("Debug State")]
    [SerializeField] private InstrumentState debugInstrumentState;
    [SerializeField] private bool debugHasActiveGhost;
    [SerializeField] private float debugRecordingProgress;

    private static bool isAnyInstrumentRecording = false;

    private Queue<string> debugMessages = new Queue<string>();
    private InstrumentState instrumentState = InstrumentState.Idle;
    private RadialProgressController radialProgressController;
    private GameObject activeGhost;
    private string lastStatusText = "";
    private GameObject pendingDummyFX;
    private Vector3 pendingForceVelocity;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip punchSound;

    private void Awake()
    {
        if (radialProgressObject != null)
        {
            radialProgressController = radialProgressObject.GetComponent<RadialProgressController>();
        }
    }

    public void InteractionCapture(string message)
    {
        TryStartRecording();

        godmodeController.TriggerMusicExternally();

        if (string.IsNullOrEmpty(message)) return;
        OVRInput.Controller controller = message.ToLower().Contains("left")
            ? OVRInput.Controller.LTouch
            : message.ToLower().Contains("right")
                ? OVRInput.Controller.RTouch
                : defaultHapticController;

        TriggerHaptic(controller);
    }

    public void InteractionCapture(Collider collider, OVRInput.Controller controller = OVRInput.Controller.None)
    {
        if (instrumentState == InstrumentState.Playing) return;
        if (collider == null) return;

        TryStartRecording();

        string timestamp = Time.time.ToString("F2");
        string objectName = collider.gameObject.name;
        Vector3 position = collider.transform.position;
        
        string message = $"[{timestamp}s] {objectName} at {position}";
        AddDebugMessage(message);

        if (controller != OVRInput.Controller.None)
        {
            TriggerHaptic(controller);
        }
    }

    public void InteractionCapture(GameObject target, OVRInput.Controller controller = OVRInput.Controller.None)
    {
        if (instrumentState == InstrumentState.Playing) return;
        if (target == null) return;

        TryStartRecording();

        string timestamp = Time.time.ToString("F2");
        string objectName = target.name;
        Vector3 position = target.transform.position;
        
        string message = $"[{timestamp}s] {objectName} at {position}";
        AddDebugMessage(message);

        if (controller != OVRInput.Controller.None)
        {
            TriggerHaptic(controller);
        }
    }

    private void TryStartRecording()
    {
        if (isAnyInstrumentRecording) return;
        
        if (instrumentState == InstrumentState.Idle && motionRecorder != null)
        {
            isAnyInstrumentRecording = true;
            instrumentState = InstrumentState.Recording;
            motionRecorder.StartRecordingExternally(this);
            
            if (radialProgressController != null)
            {
                radialProgressController.SetProgress(0f);
            }
        }
    }

    private void TriggerHaptic(OVRInput.Controller controller)
    {
        OVRInput.SetControllerVibration(1f, hapticIntensity, controller);
        Invoke(nameof(StopHaptics), hapticDuration);
    }

    private void StopHaptics()
    {
        OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.LTouch);
        OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.RTouch);
    }

    private void AddDebugMessage(string message)
    {
        debugMessages.Enqueue(message);

        while (debugMessages.Count > maxDebugLines)
        {
            debugMessages.Dequeue();
        }

        UpdateDebugText();
    }

    private void UpdateDebugText()
    {
        if (debugText == null) return;

        debugText.text = string.Join("\n", debugMessages);
    }

    private void Update()
    {
        if (instrumentState == InstrumentState.Recording)
        {
            UpdateRecordingProgress();
        }
        else if (instrumentState == InstrumentState.Idle)
        {
            UpdateIdleStatus();
        }
        UpdateDebugInfo();
    }

    private void FixedUpdate()
    {
        if (pendingDummyFX != null)
        {
            PlayPunchSound();
            Rigidbody rb = pendingDummyFX.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = pendingDummyFX.AddComponent<Rigidbody>();
            }
            
            rb.AddForce(pendingForceVelocity * rb.mass, ForceMode.Impulse);
            pendingDummyFX = null;
            pendingForceVelocity = Vector3.zero;
        }
    }

    private void UpdateIdleStatus()
    {
        if (debugText == null) return;

        string targetStatus = isAnyInstrumentRecording ? "busy" : "ready";
        
        if (lastStatusText != targetStatus)
        {
            debugText.text = targetStatus;
            lastStatusText = targetStatus;
        }
    }

    private void UpdateDebugInfo()
    {
        debugInstrumentState = instrumentState;
        debugHasActiveGhost = activeGhost != null;
        
        if (instrumentState == InstrumentState.Recording && motionRecorder != null)
        {
            float currentTime = motionRecorder.GetRecordingTimer();
            float totalDuration = motionRecorder.GetRecordingDuration();
            debugRecordingProgress = totalDuration > 0 ? (currentTime / totalDuration) : 0f;
        }
        else
        {
            debugRecordingProgress = 0f;
        }
    }

    private void UpdateRecordingProgress()
    {
        if (motionRecorder == null) return;

        if (motionRecorder.GetState() == MotionRecorder.RecorderState.Idle)
        {
            OnRecordingComplete();
            return;
        }

        float currentTime = motionRecorder.GetRecordingTimer();
        float totalDuration = motionRecorder.GetRecordingDuration();
        float progress = currentTime / totalDuration;

        if (radialProgressController != null)
        {
            radialProgressController.SetProgress(progress);
        }

        float remainingTime = totalDuration - currentTime;
        int countdown = Mathf.CeilToInt(remainingTime);
        
        if (debugText != null)
        {
            debugText.text = $"Recording {countdown}...";
            lastStatusText = debugText.text;
        }
    }

    private void OnRecordingComplete()
    {
        isAnyInstrumentRecording = false;
        instrumentState = InstrumentState.Playing;
        
        if (motionRecorder != null)
        {
            activeGhost = motionRecorder.GetLastSpawnedGhost();
        }
        
        if (radialProgressObject != null)
        {
            radialProgressObject.SetActive(false);
        }
        
        if (debugText != null)
        {
            debugText.text = "Playing...";
            lastStatusText = debugText.text;
        }
    }

    public void OnGhostPunched(Vector3 punchVelocity)
    {
        if (activeGhost == null) return;
        
        Vector3 ghostPosition = activeGhost.transform.position;
        
        if (dummyFXObject != null)
        {
            pendingDummyFX = Instantiate(dummyFXObject, ghostPosition, Quaternion.identity);
            pendingForceVelocity = punchVelocity;
            Destroy(pendingDummyFX, 5f);
        }
        
        if (motionRecorder != null)
        {
            motionRecorder.RemoveGhost(activeGhost);
        }
        
        activeGhost = null;
        instrumentState = InstrumentState.Idle;
        
        if (radialProgressObject != null)
        {
            radialProgressObject.SetActive(true);
        }
        
        lastStatusText = "";
    }

    private void PlayPunchSound()
    {
       float pitch = UnityEngine.Random.Range(0.9f, 1.1f);
       audioSource.pitch = pitch;
       audioSource.PlayOneShot(punchSound);
    }
}

