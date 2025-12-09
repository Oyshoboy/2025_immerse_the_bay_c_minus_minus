using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class ProModeController : MonoBehaviour
{
    [Header("Main Settings")]
    public AudioSource mainOutput; 

    [System.Serializable]
    public class InstrumentPart
    {
        public string id;
        public PokeInteractable interactable;
        public AudioClip clip;
    }

    public List<InstrumentPart> parts = new List<InstrumentPart>();

    // CHANGED TO ONENABLE (Runs every time script is enabled)
    void OnEnable()
    {
        foreach (var part in parts)
        {
            if (part.interactable != null)
            {
                part.interactable.WhenStateChanged += (args) => HandleStateChange(args, part);
                Debug.Log($"[ProController] Enabled: {part.id}");
            }
        }
    }

    // CHANGED TO ONDISABLE (Runs every time script is disabled)
    void OnDisable()
    {
        foreach (var part in parts)
        {
            if (part.interactable != null)
            {
                part.interactable.WhenStateChanged -= (args) => HandleStateChange(args, part);
            }
        }
    }

    void HandleStateChange(InteractableStateChangeArgs args, InstrumentPart part)
    {
        if (args.NewState == InteractableState.Select)
        {
            if (mainOutput != null && part.clip != null)
            {
                mainOutput.PlayOneShot(part.clip);
            }
        }
    }
}