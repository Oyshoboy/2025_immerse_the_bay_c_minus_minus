using UnityEngine;
using Oculus.Interaction;

public class HoldingPokeHelper : MonoBehaviour
{
    [SerializeField] private PokeInteractable pokeInteractable;
    [SerializeField] private InstrumentManager instrumentManager;
    [SerializeField] private bool isPressRegisterContinous;
    [SerializeField] private string interactionMessage = "";

    private float timeSinceLastCapture;
    private const float CaptureInterval = 0.15f;

    private void OnEnable()
    {
        if (pokeInteractable != null)
        {
            pokeInteractable.WhenStateChanged += HandleStateChanged;
        }
    }

    private void OnDisable()
    {
        if (pokeInteractable != null)
        {
            pokeInteractable.WhenStateChanged -= HandleStateChanged;
        }
    }

    private void HandleStateChanged(InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Select)
        {
            Debug.Log("Poke started");
            timeSinceLastCapture = 0f;
            
            if (isPressRegisterContinous && instrumentManager != null)
            {
                instrumentManager.InteractionCapture(interactionMessage);
            }
        }
    }

    private void Update()
    {
        if (pokeInteractable != null && pokeInteractable.State == InteractableState.Select)
        {
            if (isPressRegisterContinous && instrumentManager != null)
            {
                timeSinceLastCapture += Time.deltaTime;
                
                if (timeSinceLastCapture >= CaptureInterval)
                {
                    instrumentManager.InteractionCapture(interactionMessage);
                    timeSinceLastCapture = 0f;
                }
            }
        }
    }
}
