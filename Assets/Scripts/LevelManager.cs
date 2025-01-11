using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    [SerializeField] private string[] levels;
    [SerializeField] private PlayerController player;
    private static LevelManager instance;
    public static LevelManager Instance { get { return instance; } }

    private void Awake()
    {
        Time.timeScale = 1.0f;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            if (i == 0)
            {
                SetLevelStatus(levels[i], LevelStatus.Unlocked); 
            }
            else
            {
                SetLevelStatus(levels[i], LevelStatus.Locked); 
            }
        }
    }

    public void MarkCurrentLevelComplete()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SetLevelStatus(currentScene.name, LevelStatus.Completed);

        int currentSceneIndex = Array.FindIndex(levels, level => level == currentScene.name);
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < levels.Length)
        {
            LevelStatus nextLevelStatus = GetLevelStatus(levels[nextSceneIndex]);
            if (nextLevelStatus == LevelStatus.Locked)
            {
                SetLevelStatus(levels[nextSceneIndex], LevelStatus.Unlocked);
            }
        }
    }

    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
    public LevelStatus GetLevelStatus(string level)
    {
        LevelStatus levelStatus = (LevelStatus)PlayerPrefs.GetInt(level, 0);
        return levelStatus;
    }

    public void SetLevelStatus(string level, LevelStatus status)
    {
        PlayerPrefs.SetInt(level, (int)status);
        PlayerPrefs.Save();
    }
}