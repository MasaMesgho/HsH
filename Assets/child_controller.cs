using UnityEngine;

public class child_controller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    GameObject controller;
    void Start()
    {
        if (controller == null) controller = GameObject.FindWithTag("Controller");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Ghost Collision Detected: "+collision.gameObject.tag);
        if (collision.gameObject.tag == "Present")
        {
            Destroy(this.gameObject);
            controller.GetComponent<main_Controller>().objAmount -= 1;
        }
    }

}
