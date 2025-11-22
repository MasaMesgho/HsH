using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] private LayerMask hitscanLayers;
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
        Debug.Log(hitscanLayers.value);

        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log(collision.gameObject.name);
            Destroy(collision.gameObject); }

        Destroy(this.gameObject);

                }
}
