using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneHandler : MonoBehaviour 
{
	public string levelToLoad;

	public Canvas skipCanvas;
	public Image skipImage;
	public Text skipText;
	bool skipping; //Whether or not we're currently skipping.
	const float SKIP_TIME = 2.0f; //How long it takes to skip the cutscene in seconds.
	float skipCount; //Timer to count up to skip time.

	MovieTexture cutscene;
	AsyncOperation loader;

	//The colors to fade the light to at the start and end of a cutscene.
	public Color fadeOutColor;
	public Color fadeInColor;

	bool finishedFade = false; //This is set to true when a fade is finished, then immediately set to false again to be reused. 

	void Start()
	{
		cutscene = (MovieTexture)GetComponent<Renderer>().material.mainTexture;
		cutscene.Play ();
		GetComponent<AudioSource> ().Play ();

		//StartCoroutine (FadeIn ());
		FaderController.instance.FadeIn(.75f);

		//Prepare the level in the background without switching to it.
		//Set loader.allowSceneActivation to true to start the level.
		loader = SceneManager.LoadSceneAsync (levelToLoad, LoadSceneMode.Single);
		loader.allowSceneActivation = false;
	}

	void Update()
	{
		if (!cutscene.isPlaying)
		{
			//Start the level.
			//loader.allowSceneActivation = true;
			StartCoroutine (FadeOut ());
		}

		//Maybe make escape pause it?
		if (Input.GetKey (KeyCode.Space) || Input.GetKey (KeyCode.Escape))
		{
			if (skipCount < SKIP_TIME) 
			{
				skipping = true;
				skipCount += Time.deltaTime;
			}
		} 
		else if (skipCount < SKIP_TIME)
		{
			skipping = false;
			skipCount = 0f;
		}

		if (skipCount >= SKIP_TIME) {
			//Start the level.
			//loader.allowSceneActivation = true;
			StartCoroutine (FadeOut ());
		} 

		if (FaderController.instance.JustFadedOut)
			loader.allowSceneActivation = true;

		skipCanvas.enabled = skipping;
		HandleSkippingObjects ();
	}

	void HandleSkippingObjects()
	{
		float imageFillAmount = 0f;
		float textOpacity = 0f;

		if (skipCount > 0) 
		{
			textOpacity = Mathf.PingPong (Time.time, 1f);
		} 
		else 
		{
			textOpacity = 0f;
		}

		Color textColorTemp = skipText.color;
		textColorTemp.a = textOpacity;
		skipText.color = textColorTemp;

		imageFillAmount = Mathf.Clamp01 (skipCount / SKIP_TIME);
		skipImage.fillAmount = imageFillAmount;
	}

	public IEnumerator FadeIn()
	{
		//yield return StartCoroutine (FadeToVolume(.4f, 1f));
		yield return StartCoroutine (FadeLightColorAndIntensity (.4f, fadeInColor, .4f));
	}

	public IEnumerator FadeOut()
	{
		FaderController.instance.FadeOut(.75f);
		yield return StartCoroutine (FadeToVolume(.75f, 0f));
		//yield return StartCoroutine (FadeLightColorAndIntensity (.75f, fadeOutColor, 4f));
		/*
		if (finishedFade == true) 
		{
			loader.allowSceneActivation = true;
			finishedFade = false;
		}
		*/
	}

	//Drunk for this one, take with a grain of salt.
	public IEnumerator FadeToVolume(float duration, float newVolume)
	{
		float startVolume = GetComponent<AudioSource> ().volume;
		for(float t = 0; t < duration; t += Time.deltaTime)
		{
			GetComponent<AudioSource> ().volume = Mathf.Lerp (startVolume, newVolume, t);
			yield return null;
		}
	}

	//Fades the light in the scene to a color in duration seconds.
	public IEnumerator FadeLightColorAndIntensity(float duration, Color color, float intensity)
	{
		GameObject light = GameObject.Find ("Directional light").gameObject;
		Color startColor = light.GetComponent<Light> ().color;
		float startIntensity = light.GetComponent<Light> ().intensity;
		for(float t = 0f; t <= duration; t += Time.deltaTime)
		{
			Debug.Log (t);
			light.GetComponent<Light> ().color = Color.Lerp (startColor, color, t / duration);
			light.GetComponent<Light> ().intensity = Mathf.Lerp (startIntensity, intensity, t / duration);
			yield return null;
		}
		finishedFade = true;
	}
}
