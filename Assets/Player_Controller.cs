using UnityEditor.UI;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{

    private Rigidbody playerRigidbody;
    private float moveForce = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        playerRigidbody.AddRelativeForce(Vector3.forward * moveForce);
    }
}
