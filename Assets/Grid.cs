using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Sets up and houses variables for the grid on which the snake moves and food appears
/// </summary>
public class Grid : MonoBehaviour
{
	public Vector2 gridWorldSize = new Vector2 (10, 6);
	public float nodeRadius = 0.2f;
	public Node [,] grid;
	float nodeDiameter;
	public int gridSizeX;
	public int gridSizeY;
	public bool displayGridGizmos;

	void Awake ()
	{
		CreateGrid ();
	}

	public int MaxSize
	{
		get
		{
			return gridSizeX * gridSizeY;
		}
	}

	void CreateGrid () // Sets up grid array and the nodes in the array, should only be run once.
	{
		grid = new Node [gridSizeX, gridSizeY];
		Vector2 worldBottomLeft = (Vector2)transform.position - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
				grid [x, y] = new Node (worldPoint, x, y);
			}
		}
	}

	public Node NodeFromWorldPoint (Vector2 worldPosition) // Finds the closest node to the given world position
	{
		float percentX = Mathf.InverseLerp (0, gridWorldSize.x, worldPosition.x + nodeRadius);
		float percentY = Mathf.InverseLerp (0, gridWorldSize.y, worldPosition.y + nodeRadius);

		percentX = Mathf.Clamp01 (percentX);
		percentY = Mathf.Clamp01 (percentY);

		int x = Mathf.RoundToInt ((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt ((gridSizeY - 1) * percentY);
		return grid [x, y];
	}

	void OnDrawGizmos () // Makes the grid visible
	{
		Gizmos.DrawWireCube (transform.position, new Vector3 (gridWorldSize.x, gridWorldSize.y, 1.0f));

		if (grid != null && displayGridGizmos)
		{
			foreach (Node n in grid)
			{
				Gizmos.color = new Color (0, 255, 0);
				Gizmos.DrawCube (n.worldPosition, Vector3.one * ((nodeRadius * 2) - .05f));
			}
		}
	}
}

/// <summary>
/// Points of the grid. Contains data about its world position, grid position, and whether there is something occupying it
/// </summary>
public class Node
{
	public bool occupied;
	public int gridX
	{
		get;
		private set;
	}
	public int gridY
	{
		get;
		private set;
	}
	public Vector2 worldPosition
	{
		get;
		private set;
	}

	public Node (Vector2 _worldposition, int _gridX, int _gridY)
	{
		worldPosition = _worldposition;
		gridX = _gridX;
		gridY = _gridY;
	}
}
