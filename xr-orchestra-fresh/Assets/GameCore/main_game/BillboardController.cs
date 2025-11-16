using UnityEngine;

public class BillboardController : MonoBehaviour
{
    public enum UpdateMode { EveryFrame, OnEnable }
    
    [SerializeField] private Transform transformToFollow;
    [SerializeField] private bool faceCamera = true;
    [SerializeField] private UpdateMode updateMode = UpdateMode.EveryFrame;
    
    private Camera mainCamera;
    
    void Start()
    {
        if (transformToFollow == null && faceCamera)
        {
            mainCamera = Camera.main;
        }
    }
    
    void OnEnable()
    {
        UpdateBillboard();
    }
    
    void LateUpdate()
    {
        if (updateMode == UpdateMode.EveryFrame)
        {
            UpdateBillboard();
        }
    }
    
    private void UpdateBillboard()
    {
        Transform targetTransform = transformToFollow;
        
        if (targetTransform == null && faceCamera)
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            
            if (mainCamera != null)
            {
                targetTransform = mainCamera.transform;
            }
        }
        
        if (targetTransform != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - targetTransform.position);
        }
    }
}

