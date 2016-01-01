using UnityEngine;
using System.Collections;

public class NodeObject : MonoBehaviour
{
	/// <summary> How much to modify the object per frame </summary>
	private static float animationStep;
	/// <summary> The original scale of the node's GameObject (Varies with screen size). </summary>
	private static Vector3 originalNodeScale;
	/// <summary> Max size a node can get </summary>
	private static Vector3 maxNodeScale;
	/// <summary> Current progress of the animation </summary>
	
	public static void Initialize (Vector3 _originalNodeScale, float _animationStep, float _animationMagnitude)
	{
		originalNodeScale = _originalNodeScale;
		animationStep = _animationStep;
		maxNodeScale = new Vector3 (originalNodeScale.x * _animationMagnitude, originalNodeScale.y * _animationMagnitude, originalNodeScale.z);
	}
	void OnDisable ()
	{
		StopAllCoroutines (); // Cancel whichever routine this node is currently running
		gameObject.transform.localScale = originalNodeScale; // Reset the scale in case the node isn't a snake node next time
	}
	public void SetOccupier (OccupierType newOccupier)
	{
		switch (newOccupier)
		{
			case (OccupierType.FOOD):
				StartCoroutine (FoodRoutine ());
				gameObject.name = "Food Node";
				break;
			case (OccupierType.SNAKE):
				gameObject.transform.rotation = Quaternion.identity; //Ensure the new node isn't twisted
				gameObject.transform.localScale = maxNodeScale; //Maximize the node object
				StartCoroutine (SnakeRoutine ());
				gameObject.name = "Snake Node";
				break;
			case (OccupierType.NONE):
			default:
				gameObject.name = "Empty Node";
				break;
		}
	}
	/// <summary>
	/// Gradually shrinks snake nodes back to their "normal" size.
	/// </summary>
	/// <returns></returns>
	public IEnumerator SnakeRoutine ()
	{
		while (true)
		{
			gameObject.transform.localScale = Vector3.Lerp (gameObject.transform.localScale, originalNodeScale, animationStep * Time.deltaTime);
			yield return null;
		}
	}
	public IEnumerator FoodRoutine ()
	{
		while (true)
		{
			//TODO: Implement food spin animation
			yield return null;
		}
	}
}
