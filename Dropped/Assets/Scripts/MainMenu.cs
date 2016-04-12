using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
	public void Play()
	{
		SceneManager.LoadScene ("Level1.1", LoadSceneMode.Single);
	}

	public void Settings()
	{
	}

	public void Exit()
	{
		Application.Quit ();
	}
}
