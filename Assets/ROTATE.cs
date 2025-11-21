using UnityEditor;
using UnityEngine;

public class ROTATE : MonoBehaviour
{
    public Component drummer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(0f, 0.5f, 0f, Space.Self);
    }
}
