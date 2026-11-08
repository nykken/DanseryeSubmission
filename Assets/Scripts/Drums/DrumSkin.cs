using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MeshFilter))]
public class DrumSkin : MonoBehaviour
{
    public AudioClip clip;
    public float maxVolume = 4.0f;

    public float maxSpeed = 4.0f;
    public Lane lane;

    private AudioSource source;

    public Vector3 Center { get; private set; }
    public Vector3 Normal { get; private set; }
    public float Radius { get; private set; }
    public float HeightAbove(Vector3 point) => Vector3.Dot(point - Center, Normal);

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    void OnValidate()
    {
        if (lane == null)
            Debug.LogWarning($"{name}: no lane assigned", this);
    }

    void Start()
    {
        Measure();
    }

    void Measure()
    {
        Bounds bounds = GetComponent<MeshFilter>().sharedMesh.bounds;
        Vector3 size = Vector3.Scale(bounds.size, transform.lossyScale);

        Normal = transform.forward;
        Center = transform.TransformPoint(bounds.center + Vector3.forward * bounds.extents.z);
        Radius = (Mathf.Abs(size.x) + Mathf.Abs(size.y)) * 0.25f;

    }

    // Called by MalletHead
    public float Hit(float speed)
    {
        float strength = Mathf.Clamp01(speed / maxSpeed);

        source.PlayOneShot(clip, strength*maxVolume);

        if (lane != null) lane.RegisterHit();

        return strength;
    }

    void OnDrawGizmos()
    {
        Measure();
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Center, Radius);
    }

}
