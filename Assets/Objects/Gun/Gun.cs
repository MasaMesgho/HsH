using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour

{
    [SerializeField] private GameObject proj;
    [SerializeField] private bool UseProjectileGravity = true;
    [SerializeField] private bool UseHitScan = true;
    [SerializeField] private float RangeProjectileSpeed = 100f;
    [SerializeField] private float HitscanRange = 100f;
    [SerializeField] private LayerMask hitscanLayers;

    public InputAction fire;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * HitscanRange);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))

        { if (UseHitScan)
            { FireHitScan(); }
            else
            { CreateProjectile();  } 
        }

    }

    private void CreateProjectile()
    {
        GameObject bullet = Instantiate(proj, transform.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (UseProjectileGravity)
        { rb.useGravity = true; }
        else
            { rb.useGravity = false; }
        rb.AddForce( - gameObject.transform.up * RangeProjectileSpeed, ForceMode.Force);


    }

    private void FireHitScan()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, hitscanLayers))
        { Debug.Log(hit.collider.gameObject.name);
            Destroy(hit.collider.gameObject);
        }
    }
}
