using UnityEngine;
using UnityEngine.SceneManagement;

public class controll : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke(nameof(sceneChange), 13.5f); 
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void sceneChange()
    {
        SceneManager.LoadScene("Main");
    }
}
