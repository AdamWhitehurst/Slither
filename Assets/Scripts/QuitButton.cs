using UnityEngine;
using System.Collections;

public class QuitButton : MonoBehaviour
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
	public void Quit ()
	{
		Application.Quit ();
	}
}
