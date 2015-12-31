using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RestartButton : MonoBehaviour
{
	void Start ()
	{
		GameEvents.Death += EnableButton;
		gameObject.SetActive (false);
	}
	public void EnableButton ()
	{
		GameEvents.Death -= EnableButton;
		gameObject.SetActive (true);
	}
	public void Restart ()
	{
		SceneManager.LoadScene (0);
	}
}
