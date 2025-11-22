using System;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityGLTF.Interactivity.Schema;

public class Player_Controller : MonoBehaviour
{

    private Rigidbody playerRigidbody;
    public float moveForce = 5f;
    public float Velocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        Velocity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Velocity = (float)Math.Sqrt((playerRigidbody.linearVelocity.x * playerRigidbody.linearVelocity.x) + (playerRigidbody.linearVelocity.y * playerRigidbody.linearVelocity.y) + (playerRigidbody.linearVelocity.z * playerRigidbody.linearVelocity.z));
        float currentMoveForce = moveForce;
        if (Velocity < 12) { currentMoveForce *= 2; }

        playerRigidbody.AddRelativeForce(Vector3.forward * (Input.GetAxis("Vertical") * currentMoveForce));
        playerRigidbody.AddRelativeForce(Vector3.right * (Input.GetAxis("Horizontal") * currentMoveForce));


    }

    void OnCollisionEnter(Collision collision)
    {
        moveForce = 5f;
        playerRigidbody.linearDamping = 0.5f;
    }
    void OnCollisionExit()
    {
        moveForce = 2.5f;
        playerRigidbody.linearDamping = 0.25f;
    }

    public float GetVelocity()
    { 
        return Velocity; 
    }
}
