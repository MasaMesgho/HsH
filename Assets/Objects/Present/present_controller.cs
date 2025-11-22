using UnityEngine;

public class present_controller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)

    {
        Debug.Log($"Object layer {collision.gameObject.layer}");

        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log(collision.gameObject.name);
            Destroy(collision.gameObject);
        }

        Destroy(this.gameObject);

    }
}
