using UnityEngine;
using System.Collections;

public enum SnakeDirection // Tells you which direction the snake head is facing, ints go clockwise
{
	UP,
	RIGHT,
	DOWN,
	LEFT
}
public class Movement : MonoBehaviour
{
	float shiftDelay;
	public Grid grid;
	public bool cycle;
	public SnakeDirection currentDirection;

	private float currentTime;
	private int 
		gridPositionX,
		gridPositionY;
	private Node currentNode;
	void Start ()
	{
		currentNode = grid.grid[0, 0];
		currentNode.occupied = true;
	}

	void FixedUpdate ()
	{
		if (Time.time >= shiftDelay && cycle)
		{
			CycleMovement ();
		}
		else if (Time.time >= shiftDelay)
		{
			Move ();
		}
	}

	void CycleMovement () // Moves Snake in cycle through the grid array
	{
		currentNode.occupied = false;
		if (gridPositionX + 1 < grid.gridSizeX)
		{
			gridPositionX++;
			currentNode = grid.grid [gridPositionX, gridPositionY];
		}
		else if (gridPositionY + 1 < grid.gridSizeY)
		{
			gridPositionX = 0;
			gridPositionY++;
			currentNode = grid.grid [0, gridPositionY];
		}
		else
		{
			currentNode = grid.grid [0, 0];
			gridPositionX = 0;
			gridPositionY = 0;
		}
		currentNode.occupied = true;
		transform.position = currentNode.worldPosition;
		shiftDelay += 0.1f;
	}
	public void SetDirection (int direction) // Sets the direction of the snake head based on SnakeDirection ints (clockwise)
	{
		SnakeDirection newDirection = (SnakeDirection)direction;

		if (newDirection != currentDirection)
		{
			currentDirection = newDirection;
			Debug.Log (currentDirection);
		}
	}

	void Move () // Hop Nodes based on SnakeDirection
	{
	}
}
