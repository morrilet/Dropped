using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour 
{
	Resolution[] supportedResolutions;
	bool isFullscreen;

	public Canvas settingsMenu;
	GameObject[] settingsObjects;

	public Canvas audioMenu;
	GameObject[] audioMenuObjects;

	public Canvas mainMenu;
	GameObject[] mainMenuObjects;

	Slider globalVolumeSlider;
	Text globalVolumePercentageText;
	Slider effectVolumeSlider;
	Text effectVolumePercentageText;
	Slider musicVolumeSlider;
	Text musicVolumePercentageText;

	Dropdown resolutionDropdown;
	int resolutionDropdownValuePrev;
	Button screenModeButton;

	Dropdown qualityDropdown;
	int qualityDropdownValuePrev;

	List<Dropdown.OptionData> resolutions;
	List<Dropdown.OptionData> qualities;

	AsyncOperation loader; //For loading the level to start in the background.

	public enum Menu
	{
		Main, 
		Settings, 
		Audio
	}
	public Menu currentMenu;

	void Start()
	{
		currentMenu = Menu.Main;
		isFullscreen = Screen.fullScreen;
		//AudioManager.instance.PlayMusic ("bg01_v02 mixed");
		AkSoundEngine.PostEvent("Music_Loop", Camera.main.gameObject);

		loader = SceneManager.LoadSceneAsync ("OpeningCutscene", LoadSceneMode.Single);
		loader.allowSceneActivation = false;

		supportedResolutions = Screen.resolutions;

		mainMenuObjects = new GameObject[mainMenu.transform.childCount];
		for (int i = 0; i < mainMenu.transform.childCount; i++) 
		{
			mainMenuObjects[i] = mainMenu.transform.GetChild (i).gameObject;
		}

		settingsObjects = new GameObject[settingsMenu.transform.childCount];
		for (int i = 0; i < settingsMenu.transform.childCount; i++) 
		{
			settingsObjects[i] = settingsMenu.transform.GetChild (i).gameObject;
			if (settingsObjects [i].name == "ScreenModeButton")
				screenModeButton = settingsObjects [i].GetComponent<Button> ();
			if (settingsObjects [i].name == "ResolutionDropdown")
				resolutionDropdown = settingsObjects [i].GetComponent <Dropdown> ();
			if (settingsObjects [i].name == "QualityDropdown")
				qualityDropdown = settingsObjects [i].GetComponent<Dropdown> ();
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		////////// TODO: Find a way to set slider value for global as well as set other slider values without globals influence //////////
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		audioMenuObjects = new GameObject[audioMenu.transform.childCount];
		for (int i = 0; i < audioMenu.transform.childCount; i++) 
		{
			audioMenuObjects [i] = audioMenu.transform.GetChild (i).gameObject;
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
				//musicVolumeSlider.value = AudioManager.instance.musicVolumeModifier;
				break;
			case "MusicVolumePercentage":
				musicVolumePercentageText = audioMenuObjects [i].gameObject.GetComponent<Text> ();
				break;
			case "EffectVolumeSlider":
				effectVolumeSlider = audioMenuObjects [i].gameObject.GetComponent<Slider> ();
				//effectVolumeSlider.value = AudioManager.instance.effectsVolumeModifier;
				break;
			case "EffectVolumePercentage":
				effectVolumePercentageText = audioMenuObjects [i].gameObject.GetComponent<Text> ();
				break;
			}
		}

		resolutionDropdown.ClearOptions ();
		resolutions = new List<Dropdown.OptionData> ();
		for (int i = 0; i < Screen.resolutions.Length; i++)
		{
			resolutions.Add (new Dropdown.OptionData (Screen.resolutions [i].ToString ()));
		}
		resolutionDropdown.AddOptions (resolutions);
		foreach (Dropdown.OptionData option in resolutionDropdown.options) 
		{
			if (Screen.currentResolution.ToString ().Equals (option.text))
			{
				resolutionDropdown.value = resolutionDropdown.options.IndexOf (option);
				resolutionDropdownValuePrev = resolutionDropdown.value;
				break;
			}
		}
		//SetResolution ();

		qualityDropdown.ClearOptions ();
		qualities = new List<Dropdown.OptionData> ();
		for (int i = 0; i < QualitySettings.names.Length; i++) 
		{
			qualities.Add (new Dropdown.OptionData (QualitySettings.names [i].ToString ()));
		}
		qualityDropdown.AddOptions (qualities);
		foreach (Dropdown.OptionData option in qualityDropdown.options) 
		{
			if (QualitySettings.names [QualitySettings.GetQualityLevel ()] == option.text) 
			{
				qualityDropdown.value = qualityDropdown.options.IndexOf (option);
				qualityDropdownValuePrev = qualityDropdown.value;
				break;
			}
		}

		if(Screen.fullScreen)
			screenModeButton.transform.GetChild (0).GetComponent<Text> ().text = "FULLSCREEN";
		else
			screenModeButton.transform.GetChild (0).GetComponent<Text> ().text = "WINDOWED";

		FaderController.instance.FadeIn (.75f);
		SwitchToMenu (currentMenu);
		//StartCoroutine(FadeToMenu(currentMenu));
	}

	void Update()
	{
		if (currentMenu == Menu.Audio) 
		{
			globalVolumePercentageText.text = (int)(globalVolumeSlider.normalizedValue * 100f) + "%";
			musicVolumePercentageText.text = (int)(musicVolumeSlider.normalizedValue * 100f) + "%";
			effectVolumePercentageText.text = (int)(effectVolumeSlider.normalizedValue * 100f) + "%";

			//Used 75f for effects because I found the effects (minus the gunshot) to be a little quiet. 50f is default.
			float effectsVolume = (effectVolumeSlider.normalizedValue * 75f) * globalVolumeSlider.normalizedValue;
			float musicVolume = (musicVolumeSlider.normalizedValue * 50f) * globalVolumeSlider.normalizedValue;

			AkSoundEngine.SetRTPCValue ("Zombie_Volume", effectsVolume);
			AkSoundEngine.SetRTPCValue("Gun_Volume", effectsVolume);
			AkSoundEngine.SetRTPCValue ("Music_Volume", musicVolume);

			//AudioManager.instance.globalVolumeModifier = globalVolumeSlider.normalizedValue;
			//AudioManager.instance.musicVolumeModifier = musicVolumeSlider.normalizedValue;
			//AudioManager.instance.effectsVolumeModifier = effectVolumeSlider.normalizedValue;

			//AudioManager.instance.SetEffectVolume ();
			//AudioManager.instance.SetMusicVolume ();

			int lastValue = -1; //This is used for randomizing the effects played when the effect slider is clicked.
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

		if (resolutionDropdown.value != resolutionDropdownValuePrev)
			SetResolution ();	
		resolutionDropdownValuePrev = resolutionDropdown.value;

		if (qualityDropdown.value != qualityDropdownValuePrev)
			SetQuality ();
		qualityDropdownValuePrev = qualityDropdown.value;
	}

	public void SwitchToMainMenu()
	{
		//SwitchToMenu (Menu.Main);
		StartCoroutine(FadeToMenu(Menu.Main));
	}

	public void SwitchToSettingsMenu()
	{
		//SwitchToMenu (Menu.Settings);
		StartCoroutine(FadeToMenu(Menu.Settings));
	}

	public void SwitchToAudioMenu()
	{
		//SwitchToMenu (Menu.Audio);
		StartCoroutine(FadeToMenu(Menu.Audio));
	}

	private IEnumerator FadeToMenu(Menu targetMenu)
	{
		FaderController.instance.FadeOut (.4f);
		yield return new WaitForSeconds (.4f);
		SwitchToMenu (targetMenu);
		FaderController.instance.FadeIn (.4f);
	}

	void SwitchToMenu(Menu targetMenu)
	{
		currentMenu = targetMenu;

		for (int i = 0; i < mainMenuObjects.Length; i++) 
		{
			mainMenuObjects [i].SetActive (false);
		}
		for (int i = 0; i < settingsObjects.Length; i++) 
		{
			settingsObjects [i].SetActive (false);
		}
		for (int i = 0; i < audioMenuObjects.Length; i++)
		{
			audioMenuObjects [i].SetActive (false);
		}

		switch (targetMenu) 
		{
		case Menu.Main:
			for (int i = 0; i < mainMenuObjects.Length; i++) 
			{
				mainMenuObjects [i].SetActive (true);
			}
			break;
		case Menu.Settings:
			for (int i = 0; i < settingsObjects.Length; i++) 
			{
				settingsObjects [i].SetActive (true);
			}
			break;
		case Menu.Audio:
			for (int i = 0; i < audioMenuObjects.Length; i++) 
			{
				audioMenuObjects [i].SetActive (true);
			}
			break;
		}
	}

	public void Play()
	{
		//SceneManager.LoadScene ("OpeningCutscene", LoadSceneMode.Single);
		StartCoroutine(FadeToPlay());
	}

	private IEnumerator FadeToPlay()
	{
		FaderController.instance.FadeOut (1.25f);
		yield return new WaitForSeconds (1.25f);
		loader.allowSceneActivation = true;
	}

	/*
	public void OpenSettingsMenu()
	{
		for (int i = 0; i < mainMenuObjects.Length; i++) 
		{
			mainMenuObjects[i].gameObject.SetActive (false);
		}
		settingsMenu.gameObject.SetActive (true);
	}

	public void CloseSettingsMenu()
	{
		for (int i = 0; i < mainMenuObjects.Length; i++) 
		{
			mainMenuObjects[i].gameObject.SetActive (true);
		}
		settingsMenu.gameObject.SetActive (false);
	}
	*/

	public void Exit()
	{
		Application.Quit ();
	}

	public void ChangeScreenMode()
	{
		isFullscreen = !isFullscreen;
		Screen.fullScreen = isFullscreen;

		if(isFullscreen)
			screenModeButton.transform.GetChild (0).GetComponent<Text> ().text = "FULLSCREEN";
		else
			screenModeButton.transform.GetChild (0).GetComponent<Text> ().text = "WINDOWED";
	}

	public void SetResolution()
	{
		Resolution res = Screen.resolutions [resolutionDropdown.value];
		Screen.SetResolution (res.width, res.height, isFullscreen);
	}

	public void SetQuality()
	{
		QualitySettings.SetQualityLevel (qualityDropdown.value, true);
	}
}
