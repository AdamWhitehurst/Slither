using UnityEngine;
using System.Collections;
/// <summary>
/// Holds events and triggers pertaining to the current game instance
/// </summary>
public class GameEvents
{
	/// <summary> Base delegate for game events </summary>
	public delegate void GameEvent ();
	/// <summary> Announces that food has been eaten </summary>
	public static event GameEvent FoodEaten;
	/// <summary> Annouces that the snake has died </summary>
	public static event GameEvent Death;
	/// <summary> True when the game is over </summary>
	public static bool gameOver;

	/// <summary>
	/// Called when food is eaten
	/// </summary>
	public static void OnFoodEaten ()
	{
		if (FoodEaten != null)
		{
			FoodEaten (); // Announce that the food has been eaten
		}
	}
	/// <summary>
	/// Called when the snake dies
	/// </summary>
	public static void OnDeath ()
	{
		if (Death != null)
		{
			Death (); // Announce death event...
			gameOver = true; // And set the game over
		}
	}
}
