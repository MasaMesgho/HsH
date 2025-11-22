using System;
using Unity.VisualScripting;

using UnityEngine;
using UnityGLTF.Interactivity.Schema;

public class Player_Controller : MonoBehaviour
{

    private Rigidbody playerRigidbody;
    public float baseMoveForce = 5f;
    private float moveForce;
    public float Velocity;
    public float moveSpeedCap = 50f;
    public GameObject presentFrame;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        Velocity = 0;
        moveForce = baseMoveForce;
    }

    // Update is called once per frame
    void Update()
    {
        Velocity = (float)Math.Sqrt((playerRigidbody.linearVelocity.x * playerRigidbody.linearVelocity.x) + (playerRigidbody.linearVelocity.y * playerRigidbody.linearVelocity.y) + (playerRigidbody.linearVelocity.z * playerRigidbody.linearVelocity.z));
        float currentMoveForce = moveForce;
        if (Velocity < 12) { currentMoveForce *= 2; }

        if (Velocity < moveSpeedCap)
        {
            playerRigidbody.AddRelativeForce(Vector3.forward * (Input.GetAxis("Vertical") * currentMoveForce));
            playerRigidbody.AddRelativeForce(Vector3.right * (Input.GetAxis("Horizontal") * currentMoveForce));
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        moveForce = baseMoveForce;
        playerRigidbody.linearDamping = 0.5f;
    }
    void OnCollisionExit()
    {
        moveForce = baseMoveForce / 4;
        playerRigidbody.linearDamping = 0f;
    }

    public float GetVelocity()
    { 
        return Velocity; 
    }
}
