using UnityEngine;
using System.Collections;


public class GrinchMortar : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Projectile prefab must have a Rigidbody and the MortarProjectile script (recommended).")]
    [SerializeField] private GameObject projectilePrefab;
    [Tooltip("Spawn transform for projectiles. If null, this object's transform is used.")]
    [SerializeField] private Transform spawnPoint;

    [Header("Volley")]
    [Tooltip("Number of shots fired per volley")]
    [SerializeField] private int shotsPerVolley = 5;
    [Tooltip("Delay between individual shots in the volley (seconds)")]
    [SerializeField] private float timeBetweenShots = 0.15f;
    [Tooltip("Delay between volleys (seconds)")]
    [SerializeField] private float delayBetweenVolleys = 4f;
    [Tooltip("Optional small delay before each volley starts (seconds)")]
    [SerializeField] private float volleyStartDelay = 0.25f;

    [Header("Ballistics")]
    [Tooltip("Desired apex height above the mortar's spawn Y (meters)")]
    [SerializeField] private float apexHeight = 6f;
    [Tooltip("Maximum random horizontal deviation (meters) applied around the captured player position")]
    [SerializeField] private float deviationRadius = 1.0f;

    [Header("Timing")]
    [Tooltip("If true the mortar will automatically begin firing at Start")]
    [SerializeField] private bool autoStart = true;
    [Tooltip("Seconds to wait after this mortar is created before the first volley")]
    [SerializeField] private float initialVolleyDelay = 2f;

    [Header("Player detection")]
    [Tooltip("Tag used to find the player")]
    [SerializeField] private string playerTag = "Player";

    [Header("Projectile")]
    [Tooltip("Lifetime (seconds) assigned to spawned projectiles")]
    [SerializeField] private float projectileLifeTime = 8f;

    private Transform player;
    private Coroutine volleyLoopCoroutine;

    void Start()
    {
        player = GameObject.FindWithTag(playerTag)?.transform;
        if (player == null)
            Debug.LogWarning($"GrinchMortar: No GameObject found with tag '{playerTag}'. Mortar will still run but captured player position will be Vector3.zero.");

        if (autoStart && projectilePrefab != null)
            volleyLoopCoroutine = StartCoroutine(VolleyLoop());
    }

    void OnDisable()
    {
        if (volleyLoopCoroutine != null)
        {
            StopCoroutine(volleyLoopCoroutine);
            volleyLoopCoroutine = null;
        }
    }

    /// <summary>
    /// Manual trigger to fire a single volley immediately.
    /// </summary>
    public void TriggerVolley()
    {
        StartCoroutine(FireVolleyCoroutine());
    }

    /// <summary>
    /// Continuous loop that fires the initial volley after initial delay, then repeats with delayBetweenVolleys.
    /// </summary>
    private IEnumerator VolleyLoop()
    {
        yield return new WaitForSeconds(Mathf.Max(0f, initialVolleyDelay));

        while (true)
        {
            // Wait an optional small start delay (for telegraphing, animations, etc.)
            if (volleyStartDelay > 0f)
                yield return new WaitForSeconds(volleyStartDelay);

            // Fire the volley and wait until it's finished
            yield return StartCoroutine(FireVolleyCoroutine());

            // Wait between volleys
            yield return new WaitForSeconds(Mathf.Max(0f, delayBetweenVolleys));
        }
    }

    /// <summary>
    /// Fires one volley: captures player's current position, then spawns shotsPerVolley projectiles with small deviations.
    /// </summary>
    private IEnumerator FireVolleyCoroutine()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("GrinchMortar: projectilePrefab is not assigned.");
            yield break;
        }

        // Ensure we have the player reference (may be null at Start)
        if (player == null)
            player = GameObject.FindWithTag(playerTag)?.transform;

        Vector3 capturedPlayerPos = player ? player.position : Vector3.zero;
        Transform sp = spawnPoint ? spawnPoint : transform;
        Vector3 startPos = sp.position;

        int shots = Mathf.Max(1, shotsPerVolley);
        for (int i = 0; i < shots; i++)
        {
            // Slight horizontal deviation near captured player position
            Vector2 dev = Random.insideUnitCircle * deviationRadius;
            Vector3 target = new Vector3(capturedPlayerPos.x + dev.x, capturedPlayerPos.y, capturedPlayerPos.z + dev.y);

            // Compute ballistic velocity for a high arc aimed at target
            if (ComputeLaunchVelocity(startPos, target, apexHeight, out Vector3 launchVelocity))
            {
                SpawnProjectile(startPos, launchVelocity);
            }
            else
            {
                // fallback: give a modest lob toward the target
                Vector3 dir = (target - startPos).normalized;
                SpawnProjectile(startPos, dir * 6f + Vector3.up * 4f);
            }

            if (i < shots - 1)
                yield return new WaitForSeconds(Mathf.Max(0f, timeBetweenShots));
        }
    }

    /// <summary>
    /// Instantiates the projectile prefab and applies the initial velocity and recommended Rigidbody settings.
    /// If the prefab contains a MortarProjectile component the lifetime is passed via Initialize.
    /// </summary>
    private void SpawnProjectile(Vector3 spawnPos, Vector3 velocity)
    {
        GameObject go = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        Rigidbody rb = go.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("GrinchMortar: Instantiated projectile has no Rigidbody. Add one to the prefab for physics behaviour.");
            return;
        }

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = velocity;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Orient the projectile to point along its velocity for visuals
        if (velocity.sqrMagnitude > 0.0001f)
            go.transform.rotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);

        // If the projectile class supports initialization (for lifespan or other data), call it
        var mortarProj = go.GetComponent<MortarProjectile>();
        if (mortarProj != null)
            mortarProj.Initialize(velocity, projectileLifeTime);
        else
            Destroy(go, projectileLifeTime); // safety cleanup if no script handles lifetime
    }

    /// <summary>
    /// Compute initial velocity for a ballistic arc that reaches <paramref name="apexAboveSpawn"/> meters above the spawn's Y
    /// and lands at <paramref name="target"/> when fired from <paramref name="start"/>.
    /// Returns false if a valid solution can't be computed.
    /// </summary>
    private bool ComputeLaunchVelocity(Vector3 start, Vector3 target, float apexAboveSpawn, out Vector3 velocity)
    {
        velocity = Vector3.zero;

        float g = -Physics.gravity.y;
        if (g <= 0f)
            return false;

        Vector3 displacement = target - start;
        Vector3 displacementXZ = new Vector3(displacement.x, 0f, displacement.z);
        float dx = displacementXZ.magnitude;
        float dy = displacement.y;

        // Ensure apex is above both spawn and target a bit
        float apex = Mathf.Max(apexAboveSpawn, dy + 0.1f);
        apex = Mathf.Max(0.1f, apex);

        // Time to go from start Y up to apex (from v^2 = u^2 - 2gh, or t = sqrt(2*h/g) with initial v = g * t)
        float timeUp = Mathf.Sqrt(2f * apex / g);
        float vY = g * timeUp;

        // Time to fall from apex to target Y
        float heightFromApexToTarget = apex - dy;
        if (heightFromApexToTarget < 0f)
            heightFromApexToTarget = 0f;
        float timeDown = Mathf.Sqrt(2f * heightFromApexToTarget / g);

        float totalTime = timeUp + timeDown;
        if (totalTime <= 0.0001f)
            return false;

        float vXZ = dx / totalTime;
        Vector3 dirXZ = dx > 0.0001f ? displacementXZ.normalized : Vector3.zero;

        velocity = dirXZ * vXZ + Vector3.up * vY;
        return true;
    }

    void OnDrawGizmosSelected()
    {
        Transform sp = spawnPoint ? spawnPoint : transform;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(sp.position, 0.12f);

        if (player != null)
        {
            Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.5f);
            Gizmos.DrawWireSphere(player.position, deviationRadius);
        }
    }
}