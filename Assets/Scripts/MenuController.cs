using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Button playButton;
    public Button quitButton;
    public GameObject levelScreen;

    public void Awake()
    {
        playButton.onClick.AddListener(PlayGame);
        quitButton.onClick.AddListener(Quit);

    }
    private void PlayGame()
    {
        levelScreen.SetActive(true);
    }
    private void Quit()
    {
        Application.Quit();
    }
}