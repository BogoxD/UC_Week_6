using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject settingsMenu;

    public void OnPlay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void OnSettings()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }
    public void OnBack()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }
    public void OnQuit()
    {
        Application.Quit();
        PlayerPrefs.DeleteKey("TopScore");
    }
    public void OnRestart()
    {
        SceneManager.LoadScene(0);
        GameManager.instance.RestartSpawning();
    }
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex, true);
    }
}
