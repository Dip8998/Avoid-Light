using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        restartButton.onClick.AddListener(Restart);
        mainMenuButton.onClick.AddListener(MainMenu);
        quitButton.onClick.AddListener(Quit);
    }

    private void Restart()
    {
        Time.timeScale = 1f;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }
    private void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    private void Quit()
    {
        Application.Quit();
    }
}
