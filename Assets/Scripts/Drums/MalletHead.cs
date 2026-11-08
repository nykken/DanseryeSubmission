using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MalletHead : MonoBehaviour
{
    public OVRInput.Controller controller = OVRInput.Controller.LTouch;


    public float hapticDuration = 0.05f;
    [Range(0f, 1f)] public float hapticFrequency = 0.8f;

    private Vector3 headCenter;
    private float headRadius;
    private DrumSkin[] drums;
    private Vector3 previousPosition;
    private Coroutine hapticRoutine;

    void Start()
    {
        Bounds bounds = GetComponent<MeshFilter>().sharedMesh.bounds;
        Vector3 extents = Vector3.Scale(bounds.extents, transform.lossyScale);
        headCenter = bounds.center;
        headRadius = (Mathf.Abs(extents.x) + Mathf.Abs(extents.y) + Mathf.Abs(extents.z)) / 3f;

        drums = FindObjectsByType<DrumSkin>();
        previousPosition = transform.TransformPoint(headCenter);
    }

    void Update()
    {
        Vector3 position = transform.TransformPoint(headCenter);
        foreach (DrumSkin drum in drums) CheckDrum(drum, position);
        previousPosition = position;
    }

    // TODO better comments
    void CheckDrum(DrumSkin drum, Vector3 position)
    {
        float previousHeight = drum.HeightAbove(previousPosition) - headRadius;
        float currentHeight = drum.HeightAbove(position) - headRadius;

        
        if (previousHeight <= 0f || currentHeight > 0f) return;

        float t = previousHeight / (previousHeight - currentHeight);
        Vector3 crossPoint = Vector3.Lerp(previousPosition, position, t);
        if (Vector3.ProjectOnPlane(crossPoint - drum.Center, drum.Normal).magnitude > drum.Radius) return;

       
        float speed = (previousHeight - currentHeight) / Time.deltaTime;
        PlayHaptic(drum.Hit(speed));
    }

    void PlayHaptic(float strength)
    {
        OVRInput.SetControllerVibration(hapticFrequency, strength, controller);
        CancelInvoke(nameof(StopHaptic));
        Invoke(nameof(StopHaptic), hapticDuration);
    }

    void StopHaptic() => OVRInput.SetControllerVibration(0, 0, controller);

    void OnDisable() => StopHaptic();
}