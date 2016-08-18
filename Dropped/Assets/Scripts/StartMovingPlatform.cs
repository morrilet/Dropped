using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartMovingPlatform : MonoBehaviour 
{
	public GameObject triggerObject;

	public List<PlatformController> movingPlatforms;
	[HideInInspector]
	public List<float> platformStartingSpeeds;

	void Start ()
	{
		for (int i = 0; i < movingPlatforms.Count; i++) 
		{
			platformStartingSpeeds.Add (movingPlatforms [i].speed);
			movingPlatforms [i].speed = 0f;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject == triggerObject) 
		{
			for (int i = 0; i < movingPlatforms.Count; i++) 
			{
				movingPlatforms [i].speed = platformStartingSpeeds [i];
			}
		}
	}
}
