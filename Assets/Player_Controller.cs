using System;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGLTF.Interactivity.Schema;

public class Player_Controller : MonoBehaviour
{

    private Rigidbody playerRigidbody;
    public float baseMoveForce = 5f;
    private float moveForce;
    public float Velocity;
    public float moveSpeedCap = 100f;
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
        Velocity = (float)(Math.Abs(playerRigidbody.linearVelocity.z) + Math.Abs(playerRigidbody.linearVelocity.x));
        float currentMoveForce = moveForce;
        if (Velocity < 30) { currentMoveForce *= 2; }

        if (transform.position.y < 0) 
        {
            Destroy(this.gameObject);
        }

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
        string tag = collision.gameObject.tag;
        if (tag == "Enemy" || tag == "Enemy Projectile" || tag == "Boss") Destroy(this.gameObject);

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
