using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Sets up and holds fields for the grid of nodes on which the snake moves and food appears
/// <para>Enforces othrographic main camera</para>
/// </summary>
public class Grid : MonoBehaviour
{
	/// <summary> The size of the grid in transform.position terms </summary>
	public Vector2 gridWorldSize;
	/// <summary> Each node's extent from the node center when the grid is generated </summary>
	public float nodeExtentX, nodeExtentY;
	/// <summary> Each node's total size when the grid is generated </summary>
	public float nodeSizeX, nodeSizeY;
	/// <summary> The two dimensional array of Nodes </summary>
	public Node [,] grid;
	/// <summary> User-defined amount of rows the grid has </summary>
	public int gridSizeX = 25;
	/// <summary> User-defined amount of columns the grid has </summary>
	public int gridSizeY = 15;
	/// <summary> Display each node when enabled </summary>
	public bool displayGridGizmos;
	/// <summary> The single node which is currently considered food </summary>
	public Node foodNode;
	/// <summary> The object instantiated for each node</summary>
	[SerializeField]
	GameObject nodeObjectPrefab;
	/// <summary> The heap which contains all the current snake nodes </summary>
	public Heap<Node> snakeNodeHeap;
	/// <summary> The heap containing all unoccupied nodes </summary>
	public Heap<Node> freeNodeHeap;
	/// <summary> Step size of the nodes interpolation during aniamtion </summary>
	[SerializeField]
	private float nodeAnimationStep;
	/// <summary> Determines the max size of a node in its animation </summary>
	[SerializeField]
	private float nodeAnimationMagnitude;
	/// <summary> The total number of nodes in the grid. Found by calculating the area </summary>
	public int MaxSize
	{
		get
		{
			return gridSizeX * gridSizeY;
		}
	}
	void Awake ()
	{
		Camera.main.orthographic = true; // Main camera must be orthographic
		gridWorldSize = new Vector2 ((2 * Camera.main.orthographicSize * Camera.main.aspect), (2 * Camera.main.orthographicSize)); // Calculate how big the grid is, should always fill the screen
		//Calculate X and Y Node extents based on how many nodes need to fit into the respective axis
		nodeExtentX = (gridWorldSize.x / (2 * gridSizeX));
		nodeExtentY = (gridWorldSize.y / (2 * gridSizeY));
		// Calculate Node X and Y sizes, always 2x the respective axis's extent
		nodeSizeX = nodeExtentX * 2;
		nodeSizeY = nodeExtentY * 2;
		freeNodeHeap = new Heap<Node> (MaxSize); // Construct the free node heap based on the max size of the grid
		snakeNodeHeap = new Heap<Node> (MaxSize); // Construct the snake node heap based on the max size of the grid (good luck filling it)
		InitializeGrid ();
	}

	void Start ()
	{
		InitializeSnake ();
		SpawnFood (); // Spawn the first food bit
	}
	/// <summary> 
	/// Sets up grid array and the nodes in the array, should only be run once
	/// </summary>
	void InitializeGrid ()
	{
		grid = new Node [gridSizeX, gridSizeY]; // Create the 2d array of nodes using the user-defined amount of nodes
		GameObject nodeObjectTemplate = Instantiate (nodeObjectPrefab) as GameObject; // Create a game object representation for a node...
		NodeObject.Initialize (new Vector3 (nodeExtentX, nodeExtentY, 0), nodeAnimationStep, nodeAnimationMagnitude); // And setup up the static fields, based on the calculated extents
		Vector2 worldBottomLeft = new Vector2 (transform.position.x - (gridWorldSize.x / 2), transform.position.y - (gridWorldSize.y / 2)); // Calculate the bottom-most point of the grid
		Camera.main.transform.position = new Vector3 (transform.position.x, transform.position.y, -1); // Position the main camera in the center of the grid

		for (int x = 0; x < gridSizeX; x++) // Iterate along the x axis...
		{
			for (int y = 0; y < gridSizeY; y++) // Then the y axis...
			{
				// Each node position is calculated with respect to the bottom left point of the grid, shifted by its x and y position
				Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeSizeX + nodeExtentX) + Vector2.up * (y * nodeSizeY + nodeExtentY);
				Node newNode = new Node (worldPoint, Instantiate (nodeObjectTemplate), x, y); // Construct the node...
				grid [x, y] = newNode; // Add it to its respective place in the grid array...
				freeNodeHeap.Add (newNode); // And then to the free node heap
			}
		}
		Destroy (nodeObjectTemplate); // Destroy the node object template because we're done using it
	}
	/// <summary> 
	/// Adds four snake nodes to the bottom left of the grid 
	/// </summary>
	void InitializeSnake ()
	{
		for (int i = 5; i > 0; i--)
		{
			AddSnakeNode (grid [i - 1,0]);
		}
	}
	/// <summary>
	/// Set the specified node to a snake node and put it in the snake node heap
	/// </summary>
	/// <param name="atNode"></param>
	public void AddSnakeNode (Node atNode)
	{
		if (atNode.occupier != OccupierType.SNAKE) // Make sure the specified node is not already a snake node
		{
			if (atNode.occupier == OccupierType.NONE) // If the node wasn't a food node...
			{
				freeNodeHeap.Remove (atNode); // Remove it from the free node heap...
			}
			snakeNodeHeap.Add (atNode); // And add it to the snake node heap
			atNode.SetOccupier (OccupierType.SNAKE); // Make sure it knows its a snake node
		}
		else
			Debug.LogError ("Attempted to add snake item to snake node!");
	}
	/// <summary>
	/// Finds the closest node to the given world position
	/// </summary>
	/// <param name="worldPosition"></param>
	/// <returns></returns>
	public Node NodeFromWorldPoint (Vector2 worldPosition)
	{
		float percentX = Mathf.InverseLerp (0, gridWorldSize.x, (worldPosition.x + nodeExtentX));
		float percentY = Mathf.InverseLerp (0, gridWorldSize.y, (worldPosition.y + nodeExtentY));

		percentX = Mathf.Clamp01 (percentX);
		percentY = Mathf.Clamp01 (percentY);

		int x = Mathf.RoundToInt ((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt ((gridSizeY - 1) * percentY);
		return grid [x, y];
	}
	/// <summary>
	/// Makes each node visible when displayGridGizmos is enabled
	/// </summary>
	void OnDrawGizmos ()
	{
		Gizmos.DrawWireCube (transform.position, new Vector3 (gridWorldSize.x, gridWorldSize.y, 1.0f));
		Vector3 cubeGizmo = new Vector3 (nodeSizeX - .01f, nodeSizeY - .01f, .99f);
		if (grid != null && displayGridGizmos)
		{
			foreach (Node n in grid)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawCube (n.worldPosition, cubeGizmo);
			}
		}
	}
	/// <summary>
	/// Pulls a node from the free node heap and makes it the food Node
	/// </summary>
	public void SpawnFood ()
	{
		if (foodNode != null)
		{
			Debug.LogError ("Attempted to spawn food when one already exists!");
		}
		else
		{
			foodNode = freeNodeHeap.RemoveRandom ();
			foodNode.SetOccupier (OccupierType.FOOD); 
		}
	}
}

/// <summary>
/// Points of the grid. Contains data about its world position, grid position, and what is occupying it
/// </summary>
public class Node : IHeapObject <Node>
{
	/// <summary> Game object representing this node </summary>
	GameObject nodeGameObject;
	NodeObject nodeObject;
	/// <summary> The current type of occupier on this node. Use SetOccupier () to change</summary>
	public OccupierType occupier
	{
		get;
		private set;
	}
	/// <summary>
	/// Changes the current occupier of this node, and adjusts its node object accordingly
	/// </summary>
	/// <param name="newOccupier"></param>
	public void SetOccupier (OccupierType newOccupier)
	{
		switch (newOccupier)
		{
			case (OccupierType.FOOD):
				nodeGameObject.SetActive (true);
				nodeObject.SetOccupier (OccupierType.FOOD);
				occupier = OccupierType.FOOD;
				break;
			case (OccupierType.SNAKE):
				nodeGameObject.SetActive (true);
				nodeObject.SetOccupier (OccupierType.SNAKE);
				occupier = OccupierType.SNAKE;
				break;
			case (OccupierType.NONE):
				occupier = OccupierType.NONE;
				nodeObject.SetOccupier (OccupierType.NONE);
				nodeGameObject.SetActive (false);
				break;
			default:
				Debug.LogError ("Invalid Occupation Type set at " + _worldPosition);
				break;
		}
	}
	private Vector2 _worldPosition;
	/// <summary> This node's X value on the grid </summary>
	public int gridX
	{
		get;
		private set;
	}
	/// <summary> This node's Y value on the grid </summary>
	public int gridY
	{
		get;
		private set;
	}
	/// <summary> The position of this Node in transform.position terms (Read Only)</summary>
	public Vector2 worldPosition
	{
		get
		{
			return _worldPosition;
		}
	}
	/// <summary> This node's index in the heap it last occupied. Does not imply it is is currently part of a heap </summary>
	public int heapIndex
	{
		get;
		set;
	}
	/// <summary> Constructs a new Node </summary>
	public Node (Vector2 _wp, GameObject _go, int _gridX, int _gridY)
	{
		this._worldPosition = _wp;
		this.nodeGameObject = _go;
		this.nodeGameObject.transform.position = (Vector3)_worldPosition;
		this.nodeObject = nodeGameObject.GetComponent<NodeObject> ();
		this.SetOccupier (OccupierType.NONE);
		this.gridX = _gridX;
		this.gridY = _gridY;
	}
}
/// <summary>
/// The various enumerations of occupation a node can have
/// </summary>
public enum OccupierType
{
	NONE,
	SNAKE,
	FOOD
}
