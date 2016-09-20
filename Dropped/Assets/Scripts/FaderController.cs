using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FaderController : Singleton<FaderController>
{
	Image fadeImage;

	private bool fadingOut = false;
	private bool fadingOutPrev = false;
	private bool fadingIn = false;
	private bool fadingInPrev = false;

	private bool justFadedOut = false;
	private bool justFadedIn = false;

	public bool FadingOut 	  {get{ return fadingOut; } }
	public bool FadingOutPrev {get{ return fadingOutPrev; } }
	public bool FadingIn 	  {get{ return fadingIn; } }
	public bool FadingInPrev  {get{ return fadingInPrev; } }

	public bool JustFadedOut {get{ return justFadedOut; } }
	public bool JustFadedIn  {get{ return justFadedIn; } }

	void Start()
	{
		fadeImage = instance.transform.FindChild ("FadeOverlay").GetComponent<Image> ();
	}

	void Update()
	{
		if (justFadedOut)
			justFadedOut = false;
		if (justFadedIn)
			justFadedIn = false;

		if (!fadingOut && fadingOutPrev)
			justFadedOut = true;
		if (!fadingIn && fadingInPrev)
			justFadedIn = true;

		if (fadingOut || fadingIn) 
		{
			fadeImage.GetComponent<CanvasGroup> ().blocksRaycasts = true;
			fadeImage.GetComponent<CanvasGroup> ().interactable = true;
		} 
		else 
		{
			fadeImage.GetComponent<CanvasGroup> ().blocksRaycasts = false;
			fadeImage.GetComponent<CanvasGroup> ().interactable = false;
		}

		fadingOutPrev = fadingOut;
		fadingInPrev = fadingIn;
	}

	public void FadeOut(float duration)
	{
		StartCoroutine (FadeOutCoroutine (duration));
	}

	public void FadeIn(float duration)
	{
		StartCoroutine (FadeInCoroutine (duration));
	}

	private IEnumerator FadeOutCoroutine(float duration)
	{
		fadingOut = true;

		Color startColor = fadeImage.color;
		startColor.a = 0f;
		fadeImage.color = startColor;

		Color endColor = startColor;
		endColor.a = 1f;

		for (float t = 0f; t <= duration; t += Time.deltaTime) 
		{
			Color tempColor = Color.Lerp (startColor, endColor, t / duration);
			fadeImage.color = tempColor;
			yield return null;
		}

		//If it hasn't faded completely by the end...
		if (fadeImage.color.a < 1f) 
		{
			Color c = fadeImage.color;
			c.a = 1f;
			fadeImage.color = c;
		}
		fadingOut = false;
	}

	private IEnumerator FadeInCoroutine(float duration)
	{
		fadingIn = true;

		Color startColor = fadeImage.color;
		startColor.a = 1f;
		fadeImage.color = startColor;

		Color endColor = startColor;
		endColor.a = 0f;

		for (float t = 0f; t <= duration; t += Time.deltaTime) 
		{
			Color tempColor = Color.Lerp (startColor, endColor, t / duration);
			fadeImage.color = tempColor;
			yield return null;
		}

		//If it hasn't faded completely by the end...
		if (fadeImage.color.a > 0f) 
		{
			Color c = fadeImage.color;
			c.a = 0f;
			fadeImage.color = c;
		}
		fadingIn = false;
	}
}
