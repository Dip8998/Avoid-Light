using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridController : MonoBehaviour
{
    [SerializeField] private Tilemap obstacleTilemap; 
    [SerializeField] private Tilemap walkableTilemap; 
    [SerializeField] private float nodeSize = 1f; 

    private Node[,] grid;
    private Vector2Int gridSize;

    void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        BoundsInt bounds = walkableTilemap.cellBounds;

        gridSize = new Vector2Int(bounds.size.x, bounds.size.y);
        grid = new Node[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3Int cellPosition = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
                Vector3 worldPosition = walkableTilemap.GetCellCenterWorld(cellPosition);

                bool walkable = !obstacleTilemap.HasTile(cellPosition);
                grid[x, y] = new Node(new Vector2Int(x, y), worldPosition, walkable);
            }
        }
    }

    public Node GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        Vector3Int cellPosition = walkableTilemap.WorldToCell(worldPosition);
        int x = cellPosition.x - walkableTilemap.cellBounds.xMin;
        int y = cellPosition.y - walkableTilemap.cellBounds.yMin;

        if (x >= 0 && x < gridSize.x && y >= 0 && y < gridSize.y)
        {
            return grid[x, y];
        }

        return null;
    }

    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = GetNodeFromWorldPoint(startPos);
        Node targetNode = GetNodeFromWorldPoint(targetPos);

        if (startNode == null || targetNode == null || !startNode.isWalkable || !targetNode.isWalkable)
            return null;

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor)) continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null; 
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.gridPosition.x + x;
                int checkY = node.gridPosition.y + y;

                if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
                {
                    Node neighbor = grid[checkX, checkY];

                    if (x != 0 && y != 0)
                    {
                        Node node1 = grid[node.gridPosition.x, checkY]; 
                        Node node2 = grid[checkX, node.gridPosition.y]; 

                        if (!node1.isWalkable || !node2.isWalkable)
                            continue; 
                    }

                    neighbors.Add(neighbor);
                }
            }
        }

        return neighbors;
    }


    private int GetDistance(Node a, Node b)
    {
        int distX = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
        int distY = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);
        return 14 * Mathf.Min(distX, distY) + 10 * Mathf.Abs(distX - distY);
    }
}

