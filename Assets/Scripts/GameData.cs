using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameData : MonoBehaviour
{
	/// <summary> Global singleton pattern reference to player's game data </summary>
	public static GameData instance;
	/// <summary> Reference to the serializable object that stores player's game data and preferences </summary>
	private DataFile dataFile;
	/// <summary> The player's current highscore. New values must be greater than the current value </summary>
	public int Highscore
	{
		get
		{
			return instance.dataFile.highscore;
		}

		set
		{
			// Ensure that the new value is greater than existing value
			if (value > instance.dataFile.highscore)
				instance.dataFile.highscore = value;
        }
	}
	void Awake ()
	{
		Application.targetFrameRate = 12; // Makes application try to run at the specified FPS
		if (instance == null) // Set up singleton instance of the Game Data Object if the instance doesn't exist
		{
			instance = this; // Set this Game Data Object as the instance
			Time.timeScale = 1; // Set normal time speed for the game
			DontDestroyOnLoad (gameObject); // Make sure the instance won't be destroyed when the scene is changed
			dataFile = new DataFile (); // Create a new Data File...
			instance.Load (); // In case one isn't saved
		}
		else if (instance != this) // If the instance already exists...
		{
			Debug.LogError ("Two Game Data objects exist!");
			Destroy (gameObject); // Destroy this one because there shouldn't be more than one
		}
	}
	/// <summary>
	/// Loads the game data from the save file if there is one
	/// </summary>
	public void Load ()
	{
		if (File.Exists (Application.persistentDataPath + "/Settings.slither")) // Check that there is a save file
		{
			BinaryFormatter bf = new BinaryFormatter ();  // Create the object that reads binary files
			FileStream file = File.Open (Application.persistentDataPath + "/Settings.slither", FileMode.Open); // Open the save file in the specified filepath
			dataFile = (DataFile)bf.Deserialize (file); // Read the file and typecast as the Data File Object
			file.Close (); // Close the filestream
		}
	}
	/// <summary>
	/// Saves the game data to file
	/// </summary>
	public void Save ()
	{
		BinaryFormatter bf = new BinaryFormatter (); // Create the object that writes binary files
		FileStream file = File.Create (Application.persistentDataPath + "/Settings.slither"); // Create save file in the appliaction path
		bf.Serialize (file, dataFile); // Writes file in binary
		file.Close (); // Close the filestream
	}
}

/// <summary>
/// Serializable class that holds all savable data for the game
/// </summary>
[Serializable]
class DataFile
{
	public int
		highscore;
}
