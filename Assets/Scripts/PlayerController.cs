using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GridController gridController;

    private List<Node> path;       
    private int currentPathIndex; 
    private bool isMoving = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 targetPosition = new Vector3(mousePosition.x, mousePosition.y, 0);

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

    private void MoveAlongPath()
    {
        Node currentNode = path[currentPathIndex];

        Vector2 direction = (currentNode.worldPosition - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, currentNode.worldPosition, moveSpeed * Time.deltaTime);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (Vector2.Distance(transform.position, currentNode.worldPosition) < 0.1f)
        {
            currentPathIndex++; 

            if (currentPathIndex >= path.Count)
            {
                isMoving = false;
            }
        }
    }
}
