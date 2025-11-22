using UnityEngine;

public class Present_Thrower : MonoBehaviour
{

    public GameObject presentFrame;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            ThrowPresent();
        }
    }

    void ThrowPresent()
    {
        Debug.Log("Present Thrown");
        GameObject present = Instantiate(presentFrame, transform.position, Quaternion.identity);
        Rigidbody prb = present.GetComponent<Rigidbody>();

        prb.AddForce(gameObject.transform.forward * 500f, ForceMode.Force);


    }

}
