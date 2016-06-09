using UnityEngine;
using System.Collections;

public class FadingForegroundTrigger : MonoBehaviour 
{
	public FadingForegroundWall fadewall;

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.transform.tag == "Player") 
		{
			Debug.Log ("Here");
			if(!fadewall.touchingPlayer)
				fadewall.triggered = true;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.transform.tag == "Player") 
		{
			if(!fadewall.touchingPlayer)
				fadewall.triggered = false;
		}
	}
}
