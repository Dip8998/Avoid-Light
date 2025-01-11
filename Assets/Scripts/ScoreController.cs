using System;
using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText; 
    [SerializeField] private TextMeshProUGUI healthText;

    private int score = 0;
    private int maxHealth;
    private int currentHealth;

    private void Awake()
    {
        ResetUI();
    }

    public void SetMaxHealth(int maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void UpdateHealth(int newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    public void IncreaseScore(int increment)
    {
        score += increment;
        UpdateScoreUI();
    }

    private void ResetUI()
    {
        UpdateScoreUI();
        UpdateHealthUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth + " / " + maxHealth;
        }
    }
}
