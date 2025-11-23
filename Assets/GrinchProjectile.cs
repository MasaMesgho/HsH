using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class MortarProjectile : MonoBehaviour
{
    [Tooltip("Seconds before the projectile is auto-destroyed if it doesn't hit anything.")]
    [SerializeField] private float lifeTime = 8f;
    [Tooltip("Delay before destroying after first impact (gives a frame for effects).")]
    [SerializeField] private float destroyOnImpactDelay = 0.05f;

    private Rigidbody rb;
    private bool initialized;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogWarning("MortarProjectile requires a Rigidbody.");
    }

    /// <summary>
    /// Sets initial velocity and lifetime. Call immediately after instantiation from the spawner.
    /// </summary>
    public void Initialize(Vector3 initialVelocity, float lifetime = 8f)
    {
        if (rb == null) rb = GetComponent<Rigidbody>();

        lifeTime = lifetime;
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = initialVelocity;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        initialized = true;

        // Orient to velocity for visuals
        if (initialVelocity.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(initialVelocity.normalized, Vector3.up);

        // Safety destroy
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Optionally ignore collisions with the mortar itself or certain layers here
         if (!collision.gameObject.CompareTag("Player")) return;

        // TODO: spawn impact VFX / sound here

        // Prevent further physics interactions visually, then destroy shortly after
        if (rb != null)
        {
           // rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }



       // Destroy(gameObject, destroyOnImpactDelay);
    }

    // If the projectile was not initialized by the spawner, make sure it still cleans up
    void Start()
    {
        if (!initialized)
            Destroy(gameObject, lifeTime);
    }
}