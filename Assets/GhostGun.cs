using System.Collections;
using UnityEngine;

public class GhostGun : MonoBehaviour
{
    [Tooltip("Projectile prefab must have the GhostProjectile script and a Rigidbody.")]
    [SerializeField] private GameObject projectilePrefab;
    [Tooltip("Where the projectile will spawn. If null, this object's transform is used.")]
    [SerializeField] private Transform spawnPoint;
    [Tooltip("Projectile speed in units/sec")]
    [SerializeField] private float projectileSpeed = 2f;
    [Tooltip("Projectile lifetime in seconds")]
    [SerializeField] private float projectileLifeTime = 6f;
    [Tooltip("If true projectiles will home to the player's current position each frame.")]
    [SerializeField] private bool projectileHoming = false;
    [Tooltip("Optional fire key for quick testing")]
    [SerializeField] private KeyCode fireKey = KeyCode.Mouse0;

    [Header("Auto-fire")]
    [Tooltip("If true the gun will automatically fire a new projectile after the last one expires plus the delay.")]
    [SerializeField] private bool autoFireEnabled = true;
    [Tooltip("Seconds to wait after the last projectile expires before automatically firing.")]
    [SerializeField] private float delayAfterLastExpired = 2f;

    [Header("Initial spawn")]
    [Tooltip("If true the gun will automatically fire one projectile shortly after it spawns.")]
    [SerializeField] private bool initialSpawnEnabled = true;
    [Tooltip("Seconds to wait after this gun is created before firing the initial projectile.")]
    [SerializeField] private float initialSpawnDelay = 2f;

    private int activeProjectiles = 0;
    private Coroutine autoFireCoroutine;
    private Coroutine initialSpawnCoroutine;

    void Start()
    {
        // Schedule an initial projectile a few seconds after the gun spawns
        if (initialSpawnEnabled && projectilePrefab != null)
        {
            initialSpawnCoroutine = StartCoroutine(InitialSpawnAfterDelay());
        }
    }

    void Update()
    {
        // Quick test / manual fire
        if (projectilePrefab != null && Input.GetKeyDown(fireKey))
            Fire();
    }

    public void Fire()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("GhostGun: No projectilePrefab assigned.");
            return;
        }

        // If an auto-fire wait is pending, cancel it because the user manually fired
        if (autoFireCoroutine != null)
        {
            StopCoroutine(autoFireCoroutine);
            autoFireCoroutine = null;
        }

        // If an initial spawn wait is pending, cancel it to avoid duplicate shots
        if (initialSpawnCoroutine != null)
        {
            StopCoroutine(initialSpawnCoroutine);
            initialSpawnCoroutine = null;
        }

        Transform sp = spawnPoint ? spawnPoint : transform;
        GameObject go = Instantiate(projectilePrefab, sp.position, sp.rotation);
        GhostProjectile proj = go.GetComponent<GhostProjectile>();
        if (proj != null)
        {
            proj.Configure(projectileSpeed, projectileLifeTime, projectileHoming);
            // Track active projectile count and subscribe to its expiration
            activeProjectiles++;
            // Subscribe if the projectile exposes an expiration event
            proj.OnExpired += OnProjectileExpired;
        }
        else
        {
            Debug.LogWarning("GhostGun: Instantiated prefab does not contain GhostProjectile component.");
        }
    }

    private void OnProjectileExpired(GhostProjectile projectile)
    {
        // Unsubscribe and decrement active count
        projectile.OnExpired -= OnProjectileExpired;
        activeProjectiles = Mathf.Max(0, activeProjectiles - 1);

        // If no more active projectiles and auto-fire enabled, schedule the next shot
        if (autoFireEnabled && activeProjectiles == 0)
        {
            // Cancel previous coroutine if any, then start a fresh one
            if (autoFireCoroutine != null)
                StopCoroutine(autoFireCoroutine);
            autoFireCoroutine = StartCoroutine(AutoFireAfterDelay());
        }
    }

    private IEnumerator AutoFireAfterDelay()
    {
        // Defensive: ensure prefab exists
        if (projectilePrefab == null)
        {
            autoFireCoroutine = null;
            yield break;
        }

        yield return new WaitForSeconds(delayAfterLastExpired);

        // Clear coroutine handle before firing to allow Fire() to cancel it if needed
        autoFireCoroutine = null;
        Fire();
    }

    private IEnumerator InitialSpawnAfterDelay()
    {
        if (projectilePrefab == null)
        {
            initialSpawnCoroutine = null;
            yield break;
        }

        yield return new WaitForSeconds(Mathf.Max(0f, initialSpawnDelay));

        // Clear handle then fire
        initialSpawnCoroutine = null;
        Fire();
    }
}