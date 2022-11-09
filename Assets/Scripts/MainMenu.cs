using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string battleSelectScene;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.StopMusic();
        AudioManager.instance.PlayMenuMusic();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene(battleSelectScene);

        // Button Press SFX
        AudioManager.instance.PlaySFX(0);

    }

    public void QuitGame()
    {
        Application.Quit();

        Debug.Log("Quitting Game");

        // Button Press SFX
        AudioManager.instance.PlaySFX(0);
    }
}
