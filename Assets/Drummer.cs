using UnityEngine;

public class Drummer : MonoBehaviour
{

    public bool VelCutoff;
    public float VelCutoffIntensity;
    public GameObject player;
    public AudioSource basser;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        VelCutoffIntensity = 0.01f;
        

    }

    // Update is called once per frame
    void Update()
    {
        if (VelCutoff)
        {
            if (basser.volume <= 100)
            {
                basser.volume += VelCutoffIntensity;
            }
        }
        else
        {
            if (basser.volume >= 0)
            { basser.volume -= VelCutoffIntensity; }
        }
    }
}
