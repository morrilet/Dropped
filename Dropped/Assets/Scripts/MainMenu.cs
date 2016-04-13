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

	public Canvas mainMenu;
	GameObject[] mainMenuObjects;

	Dropdown resolutionDropdown;
	Button screenModeButton;

	List<Dropdown.OptionData> resolutions;

	void Start()
	{
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
				screenModeButton = settingsObjects [i].GetComponent<Button>();
			if (settingsObjects [i].name == "ResolutionDropdown")
				resolutionDropdown = settingsObjects [i].GetComponent <Dropdown> ();
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

		settingsMenu.gameObject.SetActive (false);
		isFullscreen = Screen.fullScreen;
	}

	public void Play()
	{
		SceneManager.LoadScene ("Level1.1", LoadSceneMode.Single);
	}

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
