using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    [SerializeField] private GameObject gameWinScreen; 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            gameWinScreen.gameObject.SetActive(true);
            LevelManager.Instance.MarkCurrentLevelComplete();
            Time.timeScale = 0f;
        }
    }
}
