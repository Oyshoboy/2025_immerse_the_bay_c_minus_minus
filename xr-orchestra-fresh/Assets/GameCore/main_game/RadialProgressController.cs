using UnityEngine;
using UnityEngine.Events;

public class RadialProgressController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float cooldownDuration = 3f;
    [SerializeField] private bool startOnEnable = false;
    
    [Header("References")]
    [SerializeField] private Renderer targetRenderer;
    
    [Header("Events")]
    public UnityEvent onCooldownComplete;
    
    private MaterialPropertyBlock propertyBlock;
    private float currentProgress;
    private bool isCooldownActive;
    private float cooldownTimer;
    
    private static readonly int ProgressPropertyID = Shader.PropertyToID("_Progress");
    
    void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
        
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }
    }
    
    void OnEnable()
    {
        if (startOnEnable)
        {
            StartCooldown();
        }
    }
    
    void Update()
    {
        if (!isCooldownActive) return;
        
        cooldownTimer += Time.deltaTime;
        currentProgress = Mathf.Clamp01(cooldownTimer / cooldownDuration);
        
        UpdateShaderProgress();
        
        if (cooldownTimer >= cooldownDuration)
        {
            CompleteCooldown();
        }
    }
    
    public void StartCooldown()
    {
        isCooldownActive = true;
        cooldownTimer = 0f;
        currentProgress = 0f;
        UpdateShaderProgress();
    }
    
    public void ResetProgress()
    {
        isCooldownActive = false;
        cooldownTimer = 0f;
        currentProgress = 0f;
        UpdateShaderProgress();
    }
    
    public void SetProgress(float progress)
    {
        currentProgress = Mathf.Clamp01(progress);
        cooldownTimer = currentProgress * cooldownDuration;
        UpdateShaderProgress();
    }
    
    public void SetCooldownDuration(float duration)
    {
        cooldownDuration = Mathf.Max(0.1f, duration);
    }
    
    private void CompleteCooldown()
    {
        isCooldownActive = false;
        currentProgress = 1f;
        UpdateShaderProgress();
        onCooldownComplete?.Invoke();
    }
    
    private void UpdateShaderProgress()
    {
        if (targetRenderer == null) return;
        
        targetRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(ProgressPropertyID, currentProgress * 100f);
        targetRenderer.SetPropertyBlock(propertyBlock);
    }
    
    public bool IsCooldownActive => isCooldownActive;
    public float CurrentProgress => currentProgress;
    public float RemainingTime => Mathf.Max(0f, cooldownDuration - cooldownTimer);
}

