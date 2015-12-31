using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Print : MonoBehaviour
{
	public static Print instance;
	Text text;
	void Awake ()
	{
		if (instance == null)
			instance = this;
		else Destroy (this);
		text = GetComponent<Text> ();
	}
	public void PrintOut (string s)
	{
		text.text = s;
	}
}
