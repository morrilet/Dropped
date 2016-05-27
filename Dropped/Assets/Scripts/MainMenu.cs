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
	Button screenModeButton;

	List<Dropdown.OptionData> resolutions;

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
		AudioManager.instance.PlayMusic ("BG1_V02");

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
		}

		audioMenuObjects = new GameObject[audioMenu.transform.childCount];
		for (int i = 0; i < audioMenu.transform.childCount; i++) 
		{
			audioMenuObjects [i] = audioMenu.transform.GetChild (i).gameObject;
			switch (audioMenuObjects [i].gameObject.name) 
			{
			case "GlobalVolumeSlider":
				globalVolumeSlider = audioMenuObjects [i].gameObject.GetComponent<Slider> ();
				break;
			case "GlobalVolumePercentage":
				globalVolumePercentageText = audioMenuObjects [i].gameObject.GetComponent<Text> ();
				break;
			case "MusicVolumeSlider":
				musicVolumeSlider = audioMenuObjects [i].gameObject.GetComponent<Slider> ();
				break;
			case "MusicVolumePercentage":
				musicVolumePercentageText = audioMenuObjects [i].gameObject.GetComponent<Text> ();
				break;
			case "EffectVolumeSlider":
				effectVolumeSlider = audioMenuObjects [i].gameObject.GetComponent<Slider> ();
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
		SetResolution ();

		if(Screen.fullScreen)
			screenModeButton.transform.GetChild (0).GetComponent<Text> ().text = "FULLSCREEN";
		else
			screenModeButton.transform.GetChild (0).GetComponent<Text> ().text = "WINDOWED";

		SwitchToMenu (currentMenu);
		isFullscreen = Screen.fullScreen;
	}

	void Update()
	{
		if (currentMenu == Menu.Audio) 
		{
			globalVolumePercentageText.text = (int)(globalVolumeSlider.normalizedValue * 100f) + "%";
			musicVolumePercentageText.text = (int)(musicVolumeSlider.normalizedValue * 100f) + "%";
			effectVolumePercentageText.text = (int)(effectVolumeSlider.normalizedValue * 100f) + "%";

			AudioManager.instance.globalVolumeModifier = globalVolumeSlider.normalizedValue;
			AudioManager.instance.musicVolumeModifier = musicVolumeSlider.normalizedValue;
			AudioManager.instance.effectsVolumeModifier = effectVolumeSlider.normalizedValue;

			AudioManager.instance.SetEffectVolume ();
			AudioManager.instance.SetMusicVolume ();

			if (effectVolumeSlider.GetComponent<SliderPointerUpEvent> ().pointerUp)
				AudioManager.instance.PlaySoundEffect ("Ethan_Gunshot");
		}
	}

	public void SwitchToMainMenu()
	{
		SwitchToMenu (Menu.Main);
	}

	public void SwitchToSettingsMenu()
	{
		SwitchToMenu (Menu.Settings);
	}

	public void SwitchToAudioMenu()
	{
		SwitchToMenu (Menu.Audio);
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
		SceneManager.LoadScene ("Level1.1", LoadSceneMode.Single);
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
}
