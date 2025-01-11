using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelWinController : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button nextLevelButton;


    private void Awake()
    {
        restartButton.onClick.AddListener(RestartLevel);
        nextLevelButton.onClick.AddListener(NextLevel);

    }
    private void RestartLevel()
    {
        Time.timeScale = 1.0f;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }
    private void NextLevel()
    {
        LevelManager.Instance.LoadNextScene();
        Time.timeScale = 1.0f;
    }
}