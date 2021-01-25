using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    [SerializeField] GameObject pauseMenuPanel;
    [SerializeField] GameObject inGamePanel;
    [SerializeField] Toggle musicToggle;
    [SerializeField] Toggle soundToggle;

    public static bool GameIsPaused;

    private void Start()
    {
        musicToggle.isOn = AudioManager.Instance.MusicEnabled;
        soundToggle.isOn = AudioManager.Instance.SoundEnabled;
    }

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

    public void StartNewGame()
    {
        GameIsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
        GamePlayManager.Instance.ResetGame();
    }

    public void SetMusic(bool isEnabled)
    {
        AudioManager.Instance.SetMusic(isEnabled);
    }

    public void SetSound(bool isEnabled)
    {
        AudioManager.Instance.SetSound(isEnabled);
    }
}
