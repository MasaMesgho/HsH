using UnityEngine;

public class Drummer : MonoBehaviour
{

    public bool VelCutoff;
    public float VelCutoffIntensity;
    public GameObject player;
    public AudioSource basser;
    private Player_Controller pcontroller;
    public float velocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        pcontroller = player.GetComponent<Player_Controller>();

    }

    // Update is called once per frame
    void Update()
    {

        float velocity = pcontroller.GetVelocity();

        if ( velocity > 500)
        {
            VelCutoff = true;
        }
        else { VelCutoff = false; }

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
