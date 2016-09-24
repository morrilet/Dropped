using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AudioManager : Singleton<AudioManager> //Does this need to be a singleton anymore? Really? Best to treat it like PauseMenu, I think.
{
	public float globalVolumeModifier;
	public float musicVolumeModifier;
	public float effectsVolumeModifier;

	bool audioMenuActive = false;

	Canvas audioMenuCanvas;
	GameObject[] audioMenuObjects;

	Slider globalVolumeSlider;
	Text globalVolumePercentageText;
	Slider effectVolumeSlider;
	Text effectVolumePercentageText;
	Slider musicVolumeSlider;
	Text musicVolumePercentageText;
	[HideInInspector]
	public Button backButton; //Because this will be in multiple scenes we need to specify what this does for main menu and for pause menu.
					  		  //This could also be done in the prefab, as main menu will be the only different one.

	//public AudioClip[] effects;
	//public AudioClip[] music;

	//public AudioSource musicSource;
	//public AudioSource effectSource;

	//private float effectStartPitch; //The default pitch of the effect source.

	public override void Awake()
	{
		//isPersistant = true;

		//effectStartPitch = effectSource.pitch;

		base.Awake ();
	}

	public void Start()
	{
		UpdateVolumeModifiers ();

		audioMenuCanvas = GetComponent<Canvas> ();

		InitializeAudioObjects ();

		backButton.onClick.RemoveAllListeners ();
		//Sets the back buttons behaviour as long as there is a pause menu in the scene.
		//The check ensures that this won't overwrite the button behaviour on main menu.
		if (GameObject.Find ("PauseMenu") != null) 
		{
			backButton.onClick.AddListener (() => { GameObject.Find ("PauseMenu").GetComponent<PauseMenu> ().DisableAudioMenu (); });
		}
		if (GameObject.Find("MainMenu") != null)
		{
			backButton.onClick.AddListener (() => { GameObject.Find ("MainMenu").GetComponent<MainMenu> ().SwitchToSettingsMenu (); });
		}
	}

	void InitializeAudioObjects()
	{
		audioMenuObjects = new GameObject[transform.childCount];
		for (int i = 0; i < transform.childCount; i++) 
		{
			audioMenuObjects [i] = transform.GetChild (i).gameObject;
			switch (audioMenuObjects [i].gameObject.name)
			{
			case "GlobalVolumeSlider":
				globalVolumeSlider = audioMenuObjects [i].gameObject.GetComponent<Slider> ();
				//globalVolumeSlider.value = 
				break;
			case "GlobalVolumePercentage":
				globalVolumePercentageText = audioMenuObjects [i].gameObject.GetComponent<Text> ();
				break;
			case "MusicVolumeSlider":
				musicVolumeSlider = audioMenuObjects [i].gameObject.GetComponent<Slider> ();
				musicVolumeSlider.value = musicVolumeModifier / 100f;
				break;
			case "MusicVolumePercentage":
				musicVolumePercentageText = audioMenuObjects [i].gameObject.GetComponent<Text> ();
				break;
			case "EffectVolumeSlider":
				effectVolumeSlider = audioMenuObjects [i].gameObject.GetComponent<Slider> ();
				effectVolumeSlider.value = effectsVolumeModifier / 100f;
				break;
			case "EffectVolumePercentage":
				effectVolumePercentageText = audioMenuObjects [i].gameObject.GetComponent<Text> ();
				break;
			case "BackButton":
				backButton = audioMenuObjects [i].gameObject.GetComponent<Button> ();
				break;
			}
		}
	}

	void UpdateVolumeModifiers()
	{
		globalVolumeModifier = 1f;

		int musicRTPCType = (int)RTPCValue_type.RTPCValue_GameObject;
		float musicRTPCValue;
		AkSoundEngine.GetRTPCValue ("Music_Volume", Camera.main.gameObject, out musicRTPCValue, ref musicRTPCType);
		float tempValue = musicRTPCValue;
		musicVolumeModifier = tempValue;

		int gunRTPCType = (int)RTPCValue_type.RTPCValue_GameObject;
		float gunRTPCValue;
		AkSoundEngine.GetRTPCValue ("Gun_Volume", Camera.main.gameObject, out gunRTPCValue, ref gunRTPCType);
		effectsVolumeModifier = gunRTPCValue;

		Debug.Log (globalVolumeModifier + ", " + musicVolumeModifier + ", " + effectsVolumeModifier);
	}

	void Update()
	{
		if (audioMenuActive) 
		{
			UpdateVolumeModifiers ();

			//Write percents under the bars.
			globalVolumePercentageText.text = (int)(globalVolumeSlider.normalizedValue * 100f) + "%";
			musicVolumePercentageText.text = (int)(musicVolumeSlider.normalizedValue * 100f) + "%";
			effectVolumePercentageText.text = (int)(effectVolumeSlider.normalizedValue * 100f) + "%";

			//The values we multiply by become the MAX VALUES for the RTPC, anywhere from 0-100.
			//Used 75f for effects because I found the effects (minus the gunshot) to be a little quiet. 50f is default.
			AkSoundEngine.SetRTPCValue ("Zombie_Volume", effectVolumeSlider.normalizedValue * 75f * globalVolumeModifier);
			AkSoundEngine.SetRTPCValue("Gun_Volume", effectVolumeSlider.normalizedValue * 75f * globalVolumeModifier);
			AkSoundEngine.SetRTPCValue ("Music_Volume", musicVolumeSlider.normalizedValue * 100f * globalVolumeModifier);

			//Randomly play an effect when effect slider is released.
			int lastValue = -1;
			if (effectVolumeSlider.GetComponent<SliderPointerUpEvent> ().pointerUp)
			{
				int randomValue = Random.Range(0, 6);
				while (randomValue == lastValue && lastValue != -1)
				{
					randomValue = Random.Range (0, 6);
				}
				switch (randomValue)
				{
				case 0:
					AkSoundEngine.PostEvent ("Ammo_Pickup", Camera.main.gameObject);
					break;
				case 1:
					AkSoundEngine.PostEvent ("Empty_Trigger", Camera.main.gameObject);
					break;
				case 2:
					AkSoundEngine.PostEvent ("Gunshot", Camera.main.gameObject);
					break;
				case 3:
					AkSoundEngine.PostEvent ("Impacts_General", Camera.main.gameObject);
					break;
				case 4:
					AkSoundEngine.PostEvent ("Impacts_Metal", Camera.main.gameObject);
					break;
				case 5:
					AkSoundEngine.PostEvent ("Impacts_Zombie", Camera.main.gameObject);
					break;
				}
				lastValue = randomValue;
			}
		}
	}

	/// <summary>
	/// Turns the audio menu on or off.
	/// </summary>
	/// <param name="active">If set to <c>true</c> active.</param>
	public void SetAudioMenuActive(bool active)
	{
		audioMenuActive = active;

		foreach (GameObject obj in audioMenuObjects) 
		{
			obj.SetActive (active);
		}
	}

	////////////////////////////////////////////////////////////
	//////////////////////// ~R.I.P.~ //////////////////////////
	////////////////////////////////////////////////////////////
	////        Here lies the original AudioManager.        //// 
	////  May he be forever free of the compilers scrutiny. ////
    ////////////////////////////////////////////////////////////
	/* 
	public void PlayMusic(string songName)
	{
		for (int i = 0; i < music.Length; i++) 
		{
			if (music [i].name == songName) 
			{
				musicSource.clip = music [i];
				musicSource.loop = true;
				musicSource.Play ();
			}
		}
	}
		
	public void PlaySoundEffect(string clipName)
	{
		for (int i = 0; i < effects.Length; i++)
		{
			if (effects [i].name == clipName) 
			{
				effectSource.pitch = effectStartPitch;
				effectSource.PlayOneShot (effects [i]);
			}
		}
	}

	public void PlaySoundEffectVariation(string clipName, float minPitch, float maxPitch)
	{
		float pitch = Random.Range (minPitch, maxPitch);
		for (int i = 0; i < effects.Length; i++) 
		{
			if (effects [i].name == clipName) 
			{
				effectSource.pitch = pitch;
				effectSource.PlayOneShot (effects [i]);
			}
		}
	}

	public void SetEffectVolume()
	{
		effectSource.volume = Mathf.Clamp01 (1f * globalVolumeModifier * effectsVolumeModifier);
	}

	public void SetMusicVolume()
	{
		musicSource.volume = Mathf.Clamp01 (1f * globalVolumeModifier * musicVolumeModifier);
	}
	*/
}
