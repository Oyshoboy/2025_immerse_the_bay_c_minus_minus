using UnityEngine;

public class MotionRecorder : MonoBehaviour
{
    public enum RecorderState { Idle, Recording, Playing }

    [Header("Tracking Targets")]
    [SerializeField] private Transform wrapper;
    [SerializeField] private Transform head;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;

    [Header("Recording Settings")]
    [SerializeField] private float recordingDuration = 10f;
    [SerializeField] private float sampleRate = 60f;
    [SerializeField] private KeyCode recordKey = KeyCode.R;

    [Header("Ghost Prefabs")]
    [SerializeField] private GameObject headGhostPrefab;
    [SerializeField] private GameObject leftHandGhostPrefab;
    [SerializeField] private GameObject rightHandGhostPrefab;
    [SerializeField] private GameObject smoothGhostPrefab;

    private RecorderState state = RecorderState.Idle;
    private MotionRecording currentRecording;
    private float recordingTimer;
    private float sampleTimer;
    private float sampleInterval;

    private GameObject ghostRoot;
    private GameObject actualGhostParent;
    private GameObject smoothGhostInstance;

    void Start()
    {
        currentRecording = new MotionRecording();
        sampleInterval = 1f / sampleRate;
        Invoke(nameof(StartRecording), 5f);
    }

    void Update()
    {
        HandleInput();
        UpdateRecording();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(recordKey) && state == RecorderState.Idle)
        {
            StartRecording();
        }
    }

    private void StartRecording()
    {
        state = RecorderState.Recording;
        currentRecording.Clear();
        recordingTimer = 0f;
        sampleTimer = 0f;

        DestroyGhosts();
    }

    private void UpdateRecording()
    {
        if (state != RecorderState.Recording) return;

        recordingTimer += Time.deltaTime;
        sampleTimer += Time.deltaTime;

        if (sampleTimer >= sampleInterval)
        {
            CaptureSnapshot();
            sampleTimer -= sampleInterval;
        }

        if (recordingTimer >= recordingDuration)
        {
            StopRecording();
        }
    }

    private void CaptureSnapshot()
    {
        if (wrapper == null) return;

        float timestamp = recordingTimer;

        if (head != null)
        {
            Vector3 localPos = wrapper.InverseTransformPoint(head.position);
            Quaternion localRot = Quaternion.Inverse(wrapper.rotation) * head.rotation;
            currentRecording.headSnapshots.Add(new TransformSnapshot(localPos, localRot, timestamp));
        }

        if (leftHand != null)
        {
            Vector3 localPos = wrapper.InverseTransformPoint(leftHand.position);
            Quaternion localRot = Quaternion.Inverse(wrapper.rotation) * leftHand.rotation;
            currentRecording.leftHandSnapshots.Add(new TransformSnapshot(localPos, localRot, timestamp));
        }

        if (rightHand != null)
        {
            Vector3 localPos = wrapper.InverseTransformPoint(rightHand.position);
            Quaternion localRot = Quaternion.Inverse(wrapper.rotation) * rightHand.rotation;
            currentRecording.rightHandSnapshots.Add(new TransformSnapshot(localPos, localRot, timestamp));
        }
    }

    private void StopRecording()
    {
        state = RecorderState.Playing;
        currentRecording.duration = recordingTimer;
        SpawnGhosts();
    }

    private void SpawnGhosts()
    {
        ghostRoot = new GameObject("Ghost");
        ghostRoot.transform.SetParent(wrapper);
        ghostRoot.transform.localPosition = Vector3.zero;
        ghostRoot.transform.localRotation = Quaternion.identity;

        actualGhostParent = new GameObject("ActualGhost");
        actualGhostParent.transform.SetParent(ghostRoot.transform);
        actualGhostParent.transform.localPosition = Vector3.zero;
        actualGhostParent.transform.localRotation = Quaternion.identity;

        GameObject headGhost = null;
        GameObject leftHandGhost = null;
        GameObject rightHandGhost = null;

        if (headGhostPrefab != null && currentRecording.headSnapshots.Count > 0)
        {
            headGhost = Instantiate(headGhostPrefab, actualGhostParent.transform);
            var player = headGhost.AddComponent<MotionPlayer>();
            player.Initialize(currentRecording.headSnapshots, currentRecording.duration);
        }

        if (leftHandGhostPrefab != null && currentRecording.leftHandSnapshots.Count > 0)
        {
            leftHandGhost = Instantiate(leftHandGhostPrefab, actualGhostParent.transform);
            var player = leftHandGhost.AddComponent<MotionPlayer>();
            player.Initialize(currentRecording.leftHandSnapshots, currentRecording.duration);
        }

        if (rightHandGhostPrefab != null && currentRecording.rightHandSnapshots.Count > 0)
        {
            rightHandGhost = Instantiate(rightHandGhostPrefab, actualGhostParent.transform);
            var player = rightHandGhost.AddComponent<MotionPlayer>();
            player.Initialize(currentRecording.rightHandSnapshots, currentRecording.duration);
        }

        if (smoothGhostPrefab != null)
        {
            smoothGhostInstance = Instantiate(smoothGhostPrefab, ghostRoot.transform);
            smoothGhostInstance.name = "SmoothGhost";
            
            var initializer = smoothGhostInstance.GetComponent<SmoothGhostInitializer>();
            if (initializer != null)
            {
                initializer.Initialize(
                    headGhost != null ? headGhost.transform : null,
                    leftHandGhost != null ? leftHandGhost.transform : null,
                    rightHandGhost != null ? rightHandGhost.transform : null
                );
            }
        }
    }

    private void DestroyGhosts()
    {
        if (ghostRoot != null) Destroy(ghostRoot);
    }

    public RecorderState GetState()
    {
        return state;
    }

    public void ResetToIdle()
    {
        state = RecorderState.Idle;
        DestroyGhosts();
        currentRecording.Clear();
    }
}

