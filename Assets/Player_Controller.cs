using System;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityGLTF.Interactivity.Schema;

public class Player_Controller : MonoBehaviour
{

    private Rigidbody playerRigidbody;
    private float moveForce = 5f;
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
        playerRigidbody.AddRelativeForce(Vector3.forward * moveForce);

        Velocity = ((playerRigidbody.linearVelocity.x * playerRigidbody.linearVelocity.x) + (playerRigidbody.linearVelocity.y * playerRigidbody.linearVelocity.y) + (playerRigidbody.linearVelocity.z * playerRigidbody.linearVelocity.z) );
    }

    public float GetVelocity()
    { return Velocity; }
}
