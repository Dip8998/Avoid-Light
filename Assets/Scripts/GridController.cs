using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridController : MonoBehaviour
{
    [SerializeField] private Tilemap obstacleTilemap;
    [SerializeField] private Tilemap walkableTilemap;
    [SerializeField] private Tile highlightTile;
    [SerializeField] private GameObject collectiblePrefab;
    [SerializeField] private int collectibleCount = 10;

    public Node[,] grid;
    public Vector2Int gridSize;
    private Vector3Int highlightedCell;
    private TileBase originalTile;

    private void Start()
    {
        CreateGrid();
        SpawnCollectibles();
        highlightedCell = new Vector3Int(-1, -1, -1);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int clickedCell = walkableTilemap.WorldToCell(mouseWorldPosition);

            if (walkableTilemap.HasTile(clickedCell) && !obstacleTilemap.HasTile(clickedCell))
            {
                HighlightTile(clickedCell);
            }
        }
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

    private void HighlightTile(Vector3Int cellPosition)
    {
        if (highlightedCell != new Vector3Int(-1, -1, -1))
        {
            walkableTilemap.SetTile(highlightedCell, originalTile);
        }

        originalTile = walkableTilemap.GetTile(cellPosition);
        highlightedCell = cellPosition;
        walkableTilemap.SetTile(cellPosition, highlightTile);
    }

    private void SpawnCollectibles()
    {
        HashSet<Vector3Int> usedPositions = new HashSet<Vector3Int>();

        int attempts = 0;
        while (usedPositions.Count < collectibleCount && attempts < 100)
        {
            attempts++;
            Vector3Int randomCellPosition = new Vector3Int(
                Random.Range(walkableTilemap.cellBounds.xMin, walkableTilemap.cellBounds.xMax),
                Random.Range(walkableTilemap.cellBounds.yMin, walkableTilemap.cellBounds.yMax),
                0);

            if (walkableTilemap.HasTile(randomCellPosition) &&
                !obstacleTilemap.HasTile(randomCellPosition) &&
                !usedPositions.Contains(randomCellPosition))
            {
                usedPositions.Add(randomCellPosition);
                Vector3 spawnPosition = walkableTilemap.GetCellCenterWorld(randomCellPosition);
                Instantiate(collectiblePrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    public Vector2Int GetGridSize()
    {
        return gridSize;
    }

    public Node GetNode(int x, int y)
    {
        if (x >= 0 && x < gridSize.x && y >= 0 && y < gridSize.y)
        {
            return grid[x, y];
        }
        return null;
    }

    public Node GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        Vector3Int cellPosition = walkableTilemap.WorldToCell(worldPosition);
        return GetNode(cellPosition.x - walkableTilemap.cellBounds.xMin, cellPosition.y - walkableTilemap.cellBounds.yMin);
    }

    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = GetNodeFromWorldPoint(startPos);
        Node targetNode = GetNodeFromWorldPoint(targetPos);

        if (startNode == null || targetNode == null || !startNode.isWalkable || !targetNode.isWalkable)
            return null;

        HashSet<Node> openSet = new HashSet<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = GetNodeWithLowestFCost(openSet);
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
                    openSet.Add(neighbor);
                }
            }
        }

        return null;
    }

    private Node GetNodeWithLowestFCost(HashSet<Node> openSet)
    {
        Node lowestCostNode = null;
        foreach (Node node in openSet)
        {
            if (lowestCostNode == null || node.FCost < lowestCostNode.FCost ||
                (node.FCost == lowestCostNode.FCost && node.hCost < lowestCostNode.hCost))
            {
                lowestCostNode = node;
            }
        }
        return lowestCostNode;
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

                int neighborX = node.gridPosition.x + x;
                int neighborY = node.gridPosition.y + y;

                if (neighborX >= 0 && neighborX < gridSize.x && neighborY >= 0 && neighborY < gridSize.y)
                {
                    Node neighborNode = grid[neighborX, neighborY];
                    if (!(x != 0 && y != 0) ||
                        (grid[node.gridPosition.x, neighborY].isWalkable && grid[neighborX, node.gridPosition.y].isWalkable))
                    {
                        neighbors.Add(neighborNode);
                    }
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
