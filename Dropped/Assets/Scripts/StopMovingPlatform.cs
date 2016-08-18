using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StopMovingPlatform : MonoBehaviour 
{
	public GameObject triggerObj;

	public List<PlatformController> movingPlatforms;

	void OnTriggerEnter2D (Collider2D other) 
	{
		if (other.gameObject == triggerObj) 
		{
			for (int i = 0; i < movingPlatforms.Count; i++) 
			{
				movingPlatforms [i].speed = 0f;
			}
		}
	}
}
