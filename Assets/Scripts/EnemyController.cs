using System.Collections.Generic;
using UnityEngine;
public class EnemyController : MonoBehaviour
{
    [SerializeField] private GridController gridController;
    [SerializeField] private Transform player;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float lightDetectionRadius = 5f;
    [SerializeField] private float lightAngle = 45f;
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private LineRenderer viewLineRenderer;
    [SerializeField] private float shootingInterval = 1f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float bulletSpeed = 10f;

    private List<Node> path;
    private int currentPatrolIndex = 0;
    private int currentTargetIndex;
    private float lastShotTime;
    private bool isShooting = false;
    private float playerLostTime;

    void Start()
    {
        StartPatrolling();
        if (viewLineRenderer != null)
        {
            viewLineRenderer.positionCount = 0;
            viewLineRenderer.startWidth = 0.1f;
            viewLineRenderer.endWidth = 0.1f;
        }
    }

    void Update()
    {
        if (PlayerInFieldOfView())
        {
            StopPatrolling();
            isShooting = true;
            ShootPlayer();
            playerLostTime = Time.time;
        }
        else
        {
            if (isShooting && Time.time - playerLostTime >= 2f)
            {
                isShooting = false;
                StartPatrolling();
            }

            if (!isShooting)
            {
                Patrol();
            }
        }

        DrawFieldOfView();
    }

    private void StartPatrolling()
    {
        UpdatePatrolPath();
    }

    private void StopPatrolling()
    {
        path = null;
    }

    private void UpdatePatrolPath()
    {
        path = gridController.FindPath(transform.position, patrolPoints[currentPatrolIndex].position);
        currentTargetIndex = 0;
    }

    private void Patrol()
    {
        if (path != null && path.Count > 0 && currentTargetIndex < path.Count)
        {
            MoveAlongPath();
        }
        else
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            UpdatePatrolPath();
        }
    }

    private void MoveAlongPath()
    {
        if (path != null && path.Count > 0 && currentTargetIndex < path.Count)
        {
            Node targetNode = path[currentTargetIndex];
            Vector3 targetPosition = targetNode.worldPosition;
            RotateTowards(targetPosition);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, patrolSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentTargetIndex++;
            }
        }
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private bool PlayerInFieldOfView()
    {
        if (player == null)
        {
            return false;
        }
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= lightDetectionRadius)
        {
            float angleToPlayer = Vector3.Angle(transform.right, directionToPlayer);

            if (angleToPlayer <= lightAngle / 2)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, lightDetectionRadius, detectionLayer);

                if (hit.collider != null && hit.collider.transform == player)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void ShootPlayer()
    {
        if (player == null || Time.time - lastShotTime < shootingInterval)
        {
            return;
        }

        lastShotTime = Time.time;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);

        Vector3 directionToPlayer = (player.position - bulletSpawnPoint.position).normalized;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = directionToPlayer * bulletSpeed;
        }
    }

    private void DrawFieldOfView()
    {
        if (viewLineRenderer == null) return;

        int rayCount = 50;
        float stepAngle = lightAngle / (rayCount - 1);
        List<Vector3> points = new List<Vector3> { transform.position };

        for (int i = 0; i < rayCount; i++)
        {
            float angle = -lightAngle / 2 + stepAngle * i;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * transform.right;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, lightDetectionRadius, detectionLayer);
            Vector3 endPoint = hit.collider != null ? hit.point : transform.position + direction * lightDetectionRadius;
            points.Add(endPoint);
        }

        viewLineRenderer.positionCount = points.Count;
        viewLineRenderer.SetPositions(points.ToArray());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.Die();
        }
    }
}