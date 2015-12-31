using UnityEngine;
using System.Collections;

public class NodeObject : MonoBehaviour
{
	public float animationFrame;
	public float animationMax;
	public OccupierType nodeOccupier;
	void OnEnable ()
	{
		animationFrame = 0.0f;
	}
	void Update ()
	{
		if (animationFrame <= animationMax)
		{
			animationFrame += Time.deltaTime;
			switch (nodeOccupier)
			{
				case (OccupierType.SNAKE):
					gameObject.transform.localScale = new Vector3 (gameObject.transform.localScale.x + Time.deltaTime, gameObject.transform.localScale.y + Time.deltaTime, gameObject.transform.localScale.z + Time.deltaTime);
                    break;
				case (OccupierType.FOOD):
				default:
					break;
			}
		}
	}
}
