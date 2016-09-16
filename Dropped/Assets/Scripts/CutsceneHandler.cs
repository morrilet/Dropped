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

	void Start()
	{
		cutscene = (MovieTexture)GetComponent<Renderer>().material.mainTexture;
		cutscene.Play ();
		GetComponent<AudioSource> ().Play ();

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
			loader.allowSceneActivation = true;
		}

		//Maybe make escape pause it?
		if (Input.GetKey (KeyCode.Space) || Input.GetKey (KeyCode.Escape))
		{
			skipping = true;
			skipCount += Time.deltaTime;
		} 
		else 
		{
			skipping = false;
			skipCount = 0f;
		}

		if (skipCount >= SKIP_TIME) 
		{
			//Start the level.
			loader.allowSceneActivation = true;
		}

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
}
