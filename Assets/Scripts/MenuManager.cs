using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnPlay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void OnQuit()
    {
        Application.Quit();
    }
    public void OnRestart()
    {
        SceneManager.LoadScene(0);
        GameManager.instance.RestartSpawning();
        Time.timeScale = 1f;
    }
}
