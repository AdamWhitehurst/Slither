using UnityEngine;
using UnityEngine.UI;
using System.Collections;
/// <summary>
/// Script that handles the scoring in-game
/// </summary>
public class Score : MonoBehaviour
{
	/// <summary> How much score eaten food will give </summary>
	public uint foodValue;
	/// <summary> The score of the current game </summary>
	public uint currentScore;
	/// <summary> Reference the the on-screen score text </summary>
	private Text scoreText;

	void Start ()
	{
		scoreText = GetComponent<Text> ();
		scoreText.text = ("Score: " + currentScore);
		GameEvents.FoodEaten += UpdateScore; // Adds method to food eaten event
		GameEvents.Death += SendScore; // Adds method to death event
		GameEvents.gameOver = false;
	}
	/// <summary>
	/// Calculates and updates the score only when the game isn't over
	/// </summary>
	void UpdateScore ()
	{
		if (!GameEvents.gameOver) // Update score as long as the game isn't over
		{
			currentScore += foodValue;
			scoreText.text = ("Score: " + currentScore);
		}
	}
	/// <summary>
	///  Sends the current score
	/// </summary>
	void SendScore ()
	{
		GameEvents.FoodEaten -= UpdateScore; // Removes score update method from foodeaten event
		GameEvents.Death -= SendScore; // Removes send score method from death event so that it only runs once
		// TODO: Send highscore somewhere (Online?)
	}
}
