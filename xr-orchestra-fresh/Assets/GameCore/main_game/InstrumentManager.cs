using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class InstrumentManager : MonoBehaviour
{
    [Header("Debug Display")]
    [SerializeField] private TMP_Text debugText;
    [SerializeField] private int maxDebugLines = 10;

    [Header("Haptic Settings")]
    [SerializeField] private float hapticIntensity = 0.5f;
    [SerializeField] private float hapticDuration = 0.1f;
    [SerializeField] private OVRInput.Controller defaultHapticController = OVRInput.Controller.RTouch;

    [Header("References")]
    [SerializeField] private GodmodeController godmodeController;

    private Queue<string> debugMessages = new Queue<string>();

    public void InteractionCapture(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        string timestamp = Time.time.ToString("F2");
        string formattedMessage = $"[{timestamp}s] {message}";
        
        AddDebugMessage(formattedMessage);

        OVRInput.Controller controller = message.ToLower().Contains("left")
            ? OVRInput.Controller.LTouch
            : message.ToLower().Contains("right")
                ? OVRInput.Controller.RTouch
                : defaultHapticController;

        TriggerHaptic(controller);
        godmodeController.TriggerMusicExternally();
    }

    public void InteractionCapture(Collider collider, OVRInput.Controller controller = OVRInput.Controller.None)
    {
        if (collider == null) return;

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
        if (target == null) return;

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
}

