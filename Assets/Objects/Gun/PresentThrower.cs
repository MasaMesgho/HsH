using UnityEngine;
using UnityEngine.InputSystem;

public class PresentThrower: MonoBehaviour

{
    [SerializeField] private GameObject proj;
    [SerializeField] private bool UseProjectileGravity = true;
    [SerializeField] private bool UseHitScan = true;
    [SerializeField] private float RangeProjectileSpeed = 100f;
    [SerializeField] private float HitscanRange = 100f;
    [SerializeField] private LayerMask hitscanLayers;
    [SerializeField] private Rigidbody playerRB;
    [SerializeField] private float inheritPlayerMomentum = 1f;

    public InputAction fire;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * HitscanRange);
    }
    void Start()
    {
        playerRB = this.GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))

        {
            if (UseHitScan)
            { FireHitScan(); }
            else
            { CreateProjectile(); }
        }

    }

    private void CreateProjectile()
    {
        GameObject bullet = Instantiate(proj, transform.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        // apply gravity setting
        rb.useGravity = UseProjectileGravity;

        // compute launch velocity: projectile speed in local -up direction plus inherited player velocity
        Vector3 launchVelocity = -transform.up * RangeProjectileSpeed;

        if (playerRB != null)
        {
            // inherit player's current momentum (scaled)
            launchVelocity += playerRB.linearVelocity * inheritPlayerMomentum;
        }

        // set the projectile's velocity directly so it immediately inherits player momentum
        rb.linearVelocity = launchVelocity;
    }

    private void FireHitScan()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, hitscanLayers))
        {
            Debug.Log(hit.collider.gameObject.name);
            Destroy(hit.collider.gameObject);
        }
    }
}