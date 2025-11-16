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

    private RecorderState state = RecorderState.Idle;
    private MotionRecording currentRecording;
    private float recordingTimer;
    private float sampleTimer;
    private float sampleInterval;

    private GameObject headGhost;
    private GameObject leftHandGhost;
    private GameObject rightHandGhost;

    void Start()
    {
        currentRecording = new MotionRecording();
        sampleInterval = 1f / sampleRate;
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
        if (headGhostPrefab != null && currentRecording.headSnapshots.Count > 0)
        {
            headGhost = Instantiate(headGhostPrefab, wrapper);
            var player = headGhost.AddComponent<MotionPlayer>();
            player.Initialize(currentRecording.headSnapshots, currentRecording.duration);
        }

        if (leftHandGhostPrefab != null && currentRecording.leftHandSnapshots.Count > 0)
        {
            leftHandGhost = Instantiate(leftHandGhostPrefab, wrapper);
            var player = leftHandGhost.AddComponent<MotionPlayer>();
            player.Initialize(currentRecording.leftHandSnapshots, currentRecording.duration);
        }

        if (rightHandGhostPrefab != null && currentRecording.rightHandSnapshots.Count > 0)
        {
            rightHandGhost = Instantiate(rightHandGhostPrefab, wrapper);
            var player = rightHandGhost.AddComponent<MotionPlayer>();
            player.Initialize(currentRecording.rightHandSnapshots, currentRecording.duration);
        }
    }

    private void DestroyGhosts()
    {
        if (headGhost != null) Destroy(headGhost);
        if (leftHandGhost != null) Destroy(leftHandGhost);
        if (rightHandGhost != null) Destroy(rightHandGhost);
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

