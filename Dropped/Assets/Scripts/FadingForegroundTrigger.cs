using UnityEngine;
using System.Collections;

public class FadingForegroundTrigger : MonoBehaviour 
{
	public FadingForegroundWall[] fadewalls;

	[HideInInspector]
	public bool touchingPlayer;

	void OnTriggerEnter2D(Collider2D other)
	{
		touchingPlayer = true;

		if (other.transform.tag == "Player") 
		{
			for (int i = 0; i < fadewalls.Length; i++) 
			{
				if (!fadewalls[i].touchingPlayer)
					fadewalls[i].triggered = true;
			}
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		touchingPlayer = true;

		if (other.transform.tag == "Player") 
		{
			for (int i = 0; i < fadewalls.Length; i++) 
			{
				if (!fadewalls [i].touchingPlayer)
					fadewalls [i].triggered = true;
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		touchingPlayer = false;

		for (int i = 0; i < fadewalls.Length; i++) 
		{
			if (!fadewalls[i].touchingPlayer)
				fadewalls[i].triggered = false;
		}
	}
}
