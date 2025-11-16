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

    private Queue<string> debugMessages = new Queue<string>();
    private InstrumentState instrumentState = InstrumentState.Idle;
    private RadialProgressController radialProgressController;

    private void Awake()
    {
        if (radialProgressObject != null)
        {
            radialProgressController = radialProgressObject.GetComponent<RadialProgressController>();
        }
    }

    public void InteractionCapture(string message)
    {
        // if (instrumentState == InstrumentState.Playing) return;
        // if (string.IsNullOrEmpty(message)) return;

        TryStartRecording();

        // string timestamp = Time.time.ToString("F2");
        // string formattedMessage = $"[{timestamp}s] {message}";
        
        // AddDebugMessage(formattedMessage);
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
        if (instrumentState == InstrumentState.Idle && motionRecorder != null)
        {
            instrumentState = InstrumentState.Recording;
            motionRecorder.StartRecordingExternally();
            
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
    }

    private void UpdateRecordingProgress()
    {
        if (motionRecorder == null) return;

        if (motionRecorder.GetState() == MotionRecorder.RecorderState.Playing)
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
        }
    }

    private void OnRecordingComplete()
    {
        instrumentState = InstrumentState.Playing;
        
        if (radialProgressObject != null)
        {
            radialProgressObject.SetActive(false);
        }
        
        if (debugText != null)
        {
            debugText.text = "Playing...";
        }
    }
}

