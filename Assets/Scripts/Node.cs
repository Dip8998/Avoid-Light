using UnityEngine;

public class Node
{
    public Vector2Int gridPosition;
    public Vector3 worldPosition;
    public bool isWalkable;
    public int gCost;
    public int hCost;
    public Node parent;

    public int FCost => gCost + hCost;

    public Node(Vector2Int gridPosition, Vector3 worldPosition, bool isWalkable)
    {
        this.gridPosition = gridPosition;
        this.worldPosition = worldPosition;
        this.isWalkable = isWalkable;
    }
}