using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OptionsButton : MonoBehaviour
{
	[SerializeField]
	private Canvas optionsCanvas;
	void Start ()
	{
		GameEvents.Death += EnableButton;
		gameObject.SetActive (false);
		optionsCanvas.gameObject.SetActive (false);
    }
	public void EnableButton ()
	{
		GameEvents.Death -= EnableButton;
		gameObject.SetActive (true);
	}
	public void ToggleOptionsCanvas ()
	{
		if (optionsCanvas.gameObject.activeInHierarchy)
			optionsCanvas.gameObject.SetActive (false);
		else
			optionsCanvas.gameObject.SetActive (true);
	}
}
