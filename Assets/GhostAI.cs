
using UnityEngine;

public class GhostAI : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [Tooltip("Degrees per second (target orbital angular speed)")]
    [SerializeField] private float orbitSpeed = 90f;
    [Tooltip("Horizontal distance from the player (target)")]
    [SerializeField] private float orbitDistance = 5f;
    [Tooltip("Height above the player's position (target)")]
    [SerializeField] private float orbitHeight = 2f;
    [Tooltip("If true the ghost faces the player while orbiting")]
    [SerializeField] private bool lookAtPlayer = true;
    [Tooltip("Initial orbit angle in degrees. Leave at 0 to randomize")]
    [SerializeField] private float initialAngle = 0f;
    [Tooltip("If true the initial angle will be randomized")]
    [SerializeField] private bool randomizeStartAngle = true;

    [Header("Momentum / smoothing")]
    [Tooltip("How quickly angular speed approaches the target (degrees/sec^2)")]
    [SerializeField] private float angularAcceleration = 180f;
    [Tooltip("Smoothing time for distance (seconds)")]
    [SerializeField] private float distanceSmoothTime = 0.5f;
    [Tooltip("Smoothing time for height (seconds)")]
    [SerializeField] private float heightSmoothTime = 0.5f;
    [Tooltip("Smoothing time for position (seconds) — additional softening of movement")]
    [SerializeField] private float positionSmoothTime = 0.25f;

    // internal state for momentum
    private float angleDeg;
    private float currentAngularSpeed = 0f; // degrees/sec
    private float currentDistance;
    private float currentHeight;
    private float distanceVelocity;
    private float heightVelocity;
    private Vector3 positionVelocity;

    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("GhostAI: No GameObject found with tag 'Player'. Disabling GhostAI.");
            enabled = false;
            return;
        }

        angleDeg = randomizeStartAngle ? Random.Range(0f, 360f) : initialAngle;

        // initialize current values to make motion start from current transform (or immediate snap to targets)
        currentDistance = orbitDistance;
        currentHeight = orbitHeight;

        // Optionally start with zero angular speed so the ghost "ramps up" into orbit
        currentAngularSpeed = 0f;
    }

    void Update()
    {
        if (player == null)
            return;

        // Ramp angular speed toward target orbitSpeed using a simple acceleration model
        if (currentAngularSpeed < orbitSpeed)
            currentAngularSpeed = Mathf.Min(orbitSpeed, currentAngularSpeed + angularAcceleration * Time.deltaTime);
        else if (currentAngularSpeed > orbitSpeed)
            currentAngularSpeed = Mathf.Max(orbitSpeed, currentAngularSpeed - angularAcceleration * Time.deltaTime);

        // Advance the angle using the smoothed angular speed
        angleDeg += currentAngularSpeed * Time.deltaTime;
        if (angleDeg >= 360f) angleDeg -= 360f;

        // Smoothly approach the target distance and height (creates momentum effect)
        currentDistance = Mathf.SmoothDamp(currentDistance, orbitDistance, ref distanceVelocity, Mathf.Max(0.0001f, distanceSmoothTime));
        currentHeight = Mathf.SmoothDamp(currentHeight, orbitHeight, ref heightVelocity, Mathf.Max(0.0001f, heightSmoothTime));

        // Compute orbit offset on XZ plane using current (smoothed) distance
        float rad = angleDeg * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * currentDistance;

        // Target position at fixed (smoothed) height above the player's Y
        Vector3 targetPos = player.transform.position + offset;
        targetPos.y = player.transform.position.y + currentHeight;

        // Apply position smoothly (soften sudden jumps and give inertia)
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref positionVelocity, Mathf.Max(0.0001f, positionSmoothTime));

        // Optional: face the player (preserving roll)
        if (lookAtPlayer)
        {
            Vector3 lookTarget = player.transform.position + Vector3.up * currentHeight;
            Vector3 dir = lookTarget - transform.position;
            if (dir.sqrMagnitude > Mathf.Epsilon)
            {
                Quaternion rot = Quaternion.LookRotation(dir.normalized, Vector3.up);
                // Preserve roll (z) by copying current Z euler
                Vector3 e = rot.eulerAngles;
                e.z = transform.eulerAngles.z;
                transform.rotation = Quaternion.Euler(e);
            }
        }
    }

    // Visualize the intended orbit radius in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = player ? (player.transform.position + Vector3.up * orbitHeight) : (transform.position + Vector3.down * orbitHeight);
        const int segments = 64;
        Vector3 prev = center + new Vector3(Mathf.Cos(0f), 0f, Mathf.Sin(0f)) * orbitDistance;
        for (int i = 1; i <= segments; i++)
        {
            float a = (i / (float)segments) * Mathf.PI * 2f;
            Vector3 next = center + new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * orbitDistance;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}