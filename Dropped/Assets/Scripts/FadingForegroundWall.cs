using UnityEngine;
using System.Collections;

public class FadingForegroundWall : MonoBehaviour
{
	[HideInInspector]
	public bool triggered; //Whether the fadewall has been triggered or not.
	bool triggeredPrev;

	[HideInInspector]
	public bool touchingPlayer;

	float duration;
	bool isFaded;
	[HideInInspector]
	public bool isFading;

	// Use this for initialization
	void Start ()
	{
		duration = .5f;
		isFaded = false;
		isFading = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (triggered && !triggeredPrev) 
		{
			if (isFading)
				StopCoroutine ("Fade");
			StartCoroutine ("Fade", 0f);
		}
		else if (!triggered && triggeredPrev) 
		{
			if (isFading)
				StopCoroutine ("Fade");
			StartCoroutine ("Fade", 1f);
		}

		//Debug.Log (triggered);// + ", " + triggeredPrev);
		triggeredPrev = triggered;
	}

	public IEnumerator Fade (float fadeToValue)
	{
		float startingAlpha = GetComponent<SpriteRenderer> ().color.a;
		isFading = true;

		for (float i = 0; i < duration; i += Time.deltaTime)
		{
			float durationPercentage = i / duration;
			Color baseColor = GetComponent<SpriteRenderer> ().color;
			baseColor.a = Mathf.Lerp (startingAlpha, fadeToValue, durationPercentage);
			GetComponent<SpriteRenderer> ().color = baseColor;

			if (transform.childCount > 0) 
			{
				for (int j = 0; j < transform.childCount; j++) 
				{
					if(transform.GetChild(j).gameObject.tag == "Fadewall")
						transform.GetChild (j).GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
				}
			}

			yield return null;
		}

		isFading = false;
	}
		
	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.transform.tag == "Player") 
		{
			touchingPlayer = true;
			triggered = true;
		}
		/*
		if (coll.transform.tag == "Player")
		{
			isFading = true;
			StartCoroutine (Fade (0f));
		}
		*/
	}

	void OnCollisitionStay2D(Collision2D coll)
	{
		if (coll.transform.tag == "Player") 
		{
			touchingPlayer = true;
			triggered = true;
		}
	}

	void OnCollisionExit2D(Collision2D coll)
	{
		if (coll.transform.tag == "Player") 
		{
			touchingPlayer = false;
			triggered = false;
		}
		/*
		if (coll.transform.tag == "Player")
			StartCoroutine (Fade (1));
		*/
	}
}
