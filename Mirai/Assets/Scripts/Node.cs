using UnityEngine;

public class Node
{
    public bool walkable;
    public bool interactable; 
    public Vector3 worldPosition;
    public int gridX, gridY;
    public int gCost, hCost;
    public Node parent;

    public int fCost { get { return gCost + hCost; } }

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        interactable = false; 
    }
}