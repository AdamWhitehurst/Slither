using UnityEngine;
using System.Collections;
/// <summary>
/// Which direction the snake head is facing, directions and ints go clockwise:
/// Up = 0, Right = 1, Down = 2, Left = 3
/// </summary>
public enum SnakeDirection
{
	UP,
	RIGHT,
	DOWN,
	LEFT
}
/// <summary>
/// Handles the movement of the snake
/// </summary>
public class SnakeMovement : MonoBehaviour
{
	/// <summary> Duration of the pause between movement shifts </summary>
	public float shiftDelay;
	/// <summary> Reference to the grid object </summary>
	[SerializeField]
	private Grid grid;
	/// <summary> The direction the snake is going on this frame </summary>
	[SerializeField]
	private SnakeDirection currentDirection;
	/// <summary> The direction the snake will be going once DetermineNewPosition () is called </summary>
	[SerializeField]
	private SnakeDirection newDirection;
	/// <summary> Grid value of snake head for the respective axis </summary>
	[SerializeField]
	private int gridPositionX, gridPositionY;
	/// <summary> Controls whether the snake is allowed to move </summary>
	[SerializeField]
	private bool canMove;
	/// <summary> Moves snake in a cycling manner and prevents user control when enabled, supercedes user-input movement </summary>
	[SerializeField]
	private bool cycle;
	/// <summary> The required magnitude of Touch's delta position required for swipe input change the snake's direction </summary>
	[SerializeField]
	private float swipeThresholdX, swipeThresholdY;
	/// <summary> Value of Time.time when snake can next move </summary>
	private float delayTimer;
	void Start ()
	{
		delayTimer = shiftDelay + Time.time; // Reset the delay timer
		GameEvents.Death += EndMovement;
	}
	void FixedUpdate ()
	{
		if (canMove) // When the snake is allowed to move...
		{
			HandleSwipeInput ();
			if (Time.time >= delayTimer && cycle) // Only move when enough time has passed, cyclical movement has priority over input driven movement
			{
				CycleMovement (); // Initiate cyclical movement
				delayTimer = shiftDelay + Time.time; // Reset the delay timer
			}
			else if (Time.time >= delayTimer)  // Only perform user-input movement when enough time has passed and cyclical movement isn't enabled
			{
				DetermineNewPosition (); // Initiate user-input movement
				delayTimer = shiftDelay + Time.time; // Reset the delay timer
			}
		}
	}
	/// <summary>
	///  Moves Snake in cycle through the grid array when SnakeMovement.cycle is enabled
	/// </summary>
	void CycleMovement ()
	{
		if (gridPositionX + 1 < grid.gridSizeX) // Check that  the snake is not on the X edge of the grid
		{
			gridPositionX++; // Move the snake up in the X if not
			ShiftSnakeToNode (grid.grid [gridPositionX, gridPositionY]);
		}
		else if (gridPositionY + 1 < grid.gridSizeY) // Check that  the snake is not on the Y edge of the grid, only when the snake is on the X edge
		{
			gridPositionX = 0; // Reset the X position
			gridPositionY++; // Move the snake up in the Y
			ShiftSnakeToNode (grid.grid [0, gridPositionY]);
		}
		else // Reset the snake to (0,0)
		{
			ShiftSnakeToNode (grid.grid [0, 0]);
			gridPositionX = 0;
			gridPositionY = 0;
		}
	}
	/// <summary>
	/// Determines when a swipe has occurred and ensures the snake only moves at right angles
	/// </summary>
	void HandleSwipeInput ()
	{
		if (Input.touchCount > 0)
		{
			Touch touchInput = Input.GetTouch (0);
			if ((currentDirection == SnakeDirection.LEFT || currentDirection == SnakeDirection.RIGHT) && (Mathf.Abs (touchInput.deltaPosition.y) > swipeThresholdY))
			{
				if (touchInput.deltaPosition.y > swipeThresholdY)
					newDirection = SnakeDirection.UP;
				else
					newDirection = SnakeDirection.DOWN;
			}
			else if ((currentDirection == SnakeDirection.UP || currentDirection == SnakeDirection.DOWN) && Mathf.Abs (touchInput.deltaPosition.x) > swipeThresholdX)
			{
				if (touchInput.deltaPosition.x > swipeThresholdX)
					newDirection = SnakeDirection.RIGHT;
				else
					newDirection = SnakeDirection.LEFT;
			}
		}
	}
	/// <summary>
	///  Determines new grid position of the snake's when movement occurs and then calls for snake nodes to shift there
	/// </summary>
	void DetermineNewPosition ()
	{
		int newGridPositionX = gridPositionX; // Seperate variables are necessary incase snake new node is unmovable...
		int newGridPositionY = gridPositionY; // and grid position needs to remain the same
		switch (newDirection)
		{
			case (SnakeDirection.UP):
				newGridPositionY++;
				break;
			case (SnakeDirection.RIGHT):
				newGridPositionX++;
				break;
			case (SnakeDirection.DOWN):
				newGridPositionY--;
				break;
			case (SnakeDirection.LEFT):
				newGridPositionX--;
				break;
		}

		if (newGridPositionX >= 0 && newGridPositionX < grid.gridSizeX && newGridPositionY >= 0 && newGridPositionY < grid.gridSizeY) //Check that the new grid position is not out of bounds
		{
			ShiftSnakeToNode (grid.grid [newGridPositionX, newGridPositionY]);
			gridPositionX = newGridPositionX; // Set the current grid position to the...
			gridPositionY = newGridPositionY; // newly determined grid position values
			currentDirection = newDirection;
		}

		else // Kill the snake if it tries to move out of bounds
		{
			GameEvents.OnDeath ();
		}
	}
	/// <summary>
	/// Determines what node is being shifted onto and handles the game respectively:
	/// <para>If the node is free, the oldest snake node is replaced with this one</para>
	/// <para>If the node is already a snake node, calls gameover</para>
	/// <para>If the node is food, then the node is added to the snake node heap</para>
	/// </summary>
	/// <param name="newNode"></param>
	public void ShiftSnakeToNode (Node newNode)
	{
		switch (newNode.occupier)
		{
			case (OccupierType.NONE):
				{
					Node oldNode = grid.snakeNodeHeap.RemoveOldest ();
					oldNode.SetOccupier (OccupierType.NONE);
					grid.freeNodeHeap.Add (oldNode);
					grid.freeNodeHeap.Remove (newNode);
					newNode.SetOccupier (OccupierType.SNAKE);
					grid.snakeNodeHeap.Add (newNode);
					break;
				}
			case (OccupierType.SNAKE):
				{
					GameEvents.OnDeath ();
					break;
				}
			case (OccupierType.FOOD):
				{
					if (newNode == grid.foodNode)
					{
						GameEvents.OnFoodEaten ();
						grid.foodNode = null;
						grid.AddSnakeNode (newNode);
						grid.SpawnFood ();
					}
					break;
				}
		}
	}
	/// <summary>
	/// Stops the snake from moving (irreversible)
	/// </summary>
	void EndMovement ()
	{
		canMove = false;
	}
}
