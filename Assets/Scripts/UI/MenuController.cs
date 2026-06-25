using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    
    public void LoadGame()
    {
        // TODO:
        Debug.Log("Load Game to be implemented");
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
