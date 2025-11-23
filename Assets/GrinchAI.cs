using UnityEngine;
using UnityEngine.SceneManagement;

public class GrinchAI : MonoBehaviour
{
    [Header("Behavior")]
    [SerializeField] private bool Boss = false;
    [SerializeField] private Transform player; // optional, will FindWithTag("Player") if null
    [SerializeField] private float stoppingDistance = 1.0f;

    [Header("Disable Options")]
    [Tooltip("GameObject to disable automatically when this becomes a boss. If null and 'Disable Self If No Target' is true, the Grinch GameObject will be disabled.")]
    [SerializeField] private GameObject objectToDisableOnBoss;

    [Tooltip("Degrees per second")]
    public float rotationSpeed = 360f;
    [Tooltip("Units per second")]
    public float movementSpeed = 3f;


    [Header("Movement (speed increases as health drops)")]
    [SerializeField] private float baseSpeed = 2f;   // speed at full health
    [SerializeField] private float maxSpeed = 6f;    // speed at 0 health

    [Header("Health / UI")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Transform healthBarFill; // child transform that will be scaled on X to represent health

    private float health;
    private Rigidbody rb;
    private float initialHealthBarScaleX = 1f;

    public GameObject playerCamera;

    void Start()
    {
        health = maxHealth;
        rb = GetComponent<Rigidbody>();

        if (player == null)
        {
            var pgo = GameObject.FindGameObjectWithTag("Player");
            if (pgo != null) player = pgo.transform;
        }

        if (healthBarFill != null)
            initialHealthBarScaleX = healthBarFill.localScale.x;
    }

    void FixedUpdate()
    {

        if (transform.position.y < 0)
        {
            this.gameObject.transform.position =  new Vector3 (500, 100, 500) ;
        }

        if (!Boss) return;
        if (player == null) return;

        movementSpeed = maxHealth - health;
        Vector3 toPlayer = player.transform.position - rb.position;
        if (toPlayer.sqrMagnitude < 0.0001f)
            return;

        // Compute rotation that looks at the player but preserve current Z (roll)
        Quaternion lookRot = Quaternion.LookRotation(toPlayer.normalized, Vector3.up);
        Vector3 targetEuler = lookRot.eulerAngles;
        targetEuler.z = transform.eulerAngles.z; // keep existing roll
        Quaternion targetRotation = Quaternion.Euler(targetEuler);

        // Smoothly rotate via Rigidbody for physics compatibility
        Quaternion newRot = Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(newRot);

        // Move toward the player's position using Rigidbody.MovePosition (physics-friendly)
        float step = movementSpeed * Time.fixedDeltaTime;
        Vector3 newPos = Vector3.MoveTowards(rb.position, player.transform.position, step);
        rb.MovePosition(newPos);
    }

    // Call to set the boss state (can be used by spawners or other scripts)
    public void SetBoss(bool isBoss)
    {
        Boss = isBoss;

        if (isBoss)
        {
            if (objectToDisableOnBoss != null)
            {
                objectToDisableOnBoss.SetActive(false);
            }
        }
    }

    // Damage the enemy; when Boss is false the Grinch is invulnerable.
    // While Boss is true, each hit reduces health by 2.
    public void TakeDamage(float amount)
    {
        // If not a boss yet, ignore damage entirely
        if (!Boss) return;

        // When Boss == true, each hit deals a flat 2 points of damage per spec.
        const float bossHitDamage = 2f;
        health = Mathf.Clamp(health - bossHitDamage, 0f, maxHealth);
        UpdateHealthBar();

        if (health <= 0f)
            Die();
    }

    // Handle physics collisions (non-trigger)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("bullet") )
        {
            TakeDamage(2f);
        }
    }

    // Handle trigger collisions
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("bullet"))
        {
            TakeDamage(2f);
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill == null) return;
        float frac = Mathf.Clamp01(health / maxHealth);
        Vector3 s = healthBarFill.localScale;
        s.x = initialHealthBarScaleX * frac;
        healthBarFill.localScale = s;
    }

    private void Die()
    {
        // simple death behavior; replace with your own (animation, pooling, etc.)
        playerCamera.GetComponent<SimpleSmoothMouseLook>().cursorUnlock();
        Destroy(gameObject);
        SceneManager.LoadScene("Menu");
    }

    // Optional: visual aid in editor
    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Boss ? Color.red : Color.yellow;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}