using UnityEngine;
using System.Collections;

public class FadingForegroundWall : MonoBehaviour
{
	float duration;

	// Use this for initialization
	void Start ()
	{
		duration = .5f;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	IEnumerator Fade (float fadeToValue)
	{
		float startingAlpha = GetComponent<SpriteRenderer> ().color.a;

		Debug.Log ("Coroutine successfully called");
		for (float i = 0; i < duration; i += Time.deltaTime)
		{
			float durationPercentage = i / duration;
			Color baseColor = GetComponent<SpriteRenderer> ().color;
			baseColor.a = Mathf.Lerp (startingAlpha, fadeToValue, durationPercentage);
			GetComponent<SpriteRenderer> ().color = baseColor;

			yield return null;
		}
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.transform.tag == "Player") {
			Debug.Log ("Called coroutine");
			StartCoroutine (Fade (.08f));
		}
	}

	void OnCollisionExit2D(Collision2D coll)
	{
		if (coll.transform.tag == "Player")
			StartCoroutine (Fade (1));
	}
}
