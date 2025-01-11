using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private ScoreController scoreController;
    [SerializeField] private GridController gridController;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private GameObject gameOverPanel;

    private int currentHealth;
    private List<Node> path;
    private int currentPathIndex;
    private bool isMoving;

    void Start()
    {
        currentHealth = maxHealth;
        scoreController.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        scoreController.UpdateHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 targetPosition = GetMouseWorldPosition();
            path = gridController.FindPath(transform.position, targetPosition);

            if (path != null && path.Count > 0)
            {
                currentPathIndex = 0;
                isMoving = true;
            }
        }

        if (isMoving && path != null && currentPathIndex < path.Count)
        {
            MoveAlongPath();
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(mousePosition.x, mousePosition.y, 0);
    }

    private void MoveAlongPath()
    {
        Node currentNode = path[currentPathIndex];

        Vector2 direction = (currentNode.worldPosition - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, currentNode.worldPosition, moveSpeed * Time.deltaTime);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (Vector2.Distance(transform.position, currentNode.worldPosition) < 0.1f)
        {
            currentPathIndex++;
            if (currentPathIndex >= path.Count)
            {
                isMoving = false;
            }
        }
    }

    public void PickUpKey()
    {
        scoreController.IncreaseScore(1);
    }
}
