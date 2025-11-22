using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GhostProjectile : MonoBehaviour
{
    [Tooltip("Units per second")]
    [SerializeField] private float speed = 2f;
    [Tooltip("Seconds until the projectile is destroyed automatically")]
    [SerializeField] private float lifeTime = 5f;
    [Tooltip("If true the projectile will home to the player's current position each frame; otherwise it moves to the player's position sampled at spawn time.")]
    [SerializeField] private bool homing = false;

    private Rigidbody rb;
    private Transform playerTransform;
    private Vector3 targetPosition;
    private bool expiredInvoked;

    // Event invoked when the projectile expires (either via timed expire or destroyed early)
    public event Action<GhostProjectile> OnExpired;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        expiredInvoked = false;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            targetPosition = playerTransform.position;
        }
        else
        {
            Debug.LogWarning("GhostProjectile: No GameObject found with tag 'Player'. Projectile will travel in its forward direction.");
            targetPosition = rb.position + transform.forward * 100f;
        }

        // Schedule expiration via Invoke so we can call OnExpired first
        CancelInvoke(nameof(Expire));
        Invoke(nameof(Expire), lifeTime);
    }

    void FixedUpdate()
    {
        // Determine direction (homing or fixed target)
        Vector3 currentTarget = homing && playerTransform != null ? playerTransform.position : targetPosition;
        Vector3 dir = currentTarget - rb.position;
        if (dir.sqrMagnitude <= 0.0001f)
            return;

        Vector3 moveDir = dir.normalized;
        Vector3 newPos = rb.position + moveDir * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);

        // Face movement direction while preserving roll
        Quaternion look = Quaternion.LookRotation(moveDir, Vector3.up);
        Vector3 e = look.eulerAngles;
        e.z = transform.eulerAngles.z;
        rb.MoveRotation(Quaternion.Euler(e));
    }

    // Called by Invoke when lifetime elapses
    private void Expire()
    {
        if (expiredInvoked) return;
        expiredInvoked = true;
        OnExpired?.Invoke(this);
        Destroy(gameObject);
    }

    // Ensure OnExpired is fired if object is destroyed by other means (collision, scene unload, etc.)
    void OnDestroy()
    {
        if (!expiredInvoked)
        {
            expiredInvoked = true;
            OnExpired?.Invoke(this);
        }
    }

    // Optional: allow external configuration
    public void Configure(float speed, float lifeTime, bool homing)
    {
        this.speed = speed;
        this.lifeTime = lifeTime;
        this.homing = homing;

        // refresh destroy timer
        CancelInvoke(nameof(Expire));
        Invoke(nameof(Expire), lifeTime);
    }
}