using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameWinController : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenu;
    private void Awake()
    {
        restartButton.onClick.AddListener(RestartLevel);
        mainMenu.onClick.AddListener(MainMenu);
    }
    private void RestartLevel()
    {
        Time.timeScale = 1f;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }
    private void MainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }
}
