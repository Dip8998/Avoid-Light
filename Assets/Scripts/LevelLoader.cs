using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class LevelLoader : MonoBehaviour
{
    [SerializeField] private string levelName;
    [SerializeField] private TextMeshProUGUI levelText;
    private Button levelButton;

    private void Awake()
    {
        levelButton = GetComponent<Button>();
        levelButton.onClick.AddListener(LoadLevel);
    }

    private void Start()
    {
        UpdateButtonState();
    }
    public void LoadLevel()
    {
        LevelStatus levelStatus = LevelManager.Instance.GetLevelStatus(levelName);
        switch (levelStatus)
        {
            case LevelStatus.Locked:
                Debug.Log("Level is locked");
                break;
            case LevelStatus.Unlocked:
            case LevelStatus.Completed:
                SceneManager.LoadScene(levelName);
                break;
        }
    }

    public void UpdateButtonState()
    {
        LevelStatus levelStatus = LevelManager.Instance.GetLevelStatus(levelName);
        switch (levelStatus)
        {
            case LevelStatus.Locked:
                levelText.color = Color.grey;
                levelButton.interactable = false;
                break;
            case LevelStatus.Unlocked:
                levelButton.interactable = true;
                break;
            case LevelStatus.Completed:
                levelText.color = Color.black;
                levelButton.interactable = true;
                break;
        }
    }
}