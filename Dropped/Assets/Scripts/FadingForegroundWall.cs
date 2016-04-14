using UnityEngine;
using System.Collections;

public class FadingForegroundWall : MonoBehaviour
{
	float duration;
	bool isFaded;
	bool isFading;

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
	
	}

	IEnumerator Fade (float fadeToValue)
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
					transform.GetChild (j).GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
				}
			}

			yield return null;
		}
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.transform.tag == "Player") {
			Debug.Log ("Called coroutine");
			isFading = true;
			StartCoroutine (Fade (0f));
		}
	}

	void OnCollisionExit2D(Collision2D coll)
	{
		if (coll.transform.tag == "Player")
			StartCoroutine (Fade (1));
	}
}
