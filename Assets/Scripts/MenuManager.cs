using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    [SerializeField] GameObject pauseMenuPanel;
    [SerializeField] GameObject inGamePanel;

    public static bool GameIsPaused;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
        inGamePanel.SetActive(true);
        GameIsPaused = false;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        inGamePanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
        GameIsPaused = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
