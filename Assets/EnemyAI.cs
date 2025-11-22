using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody rb;

    [Tooltip("Degrees per second")]
    public float rotationSpeed = 360f;
    [Tooltip("Units per second")]
    public float movementSpeed = 3f;

    void Start()
    {
        rb = rb ? rb : GetComponent<Rigidbody>();
        if (player == null)
            player = GameObject.FindWithTag("Player");
    }

    void FixedUpdate()
    {
        if (player == null || rb == null)
            return;

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
}