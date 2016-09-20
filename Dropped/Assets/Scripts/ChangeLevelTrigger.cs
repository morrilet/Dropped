using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ChangeLevelTrigger : MonoBehaviour 
{
	GameObject gameManager;
	public string levelToChangeTo;

	AsyncOperation loader;

	void Start()
	{
		loader = SceneManager.LoadSceneAsync (levelToChangeTo, LoadSceneMode.Single);
		loader.allowSceneActivation = false;
	}

	void Update()
	{
		if (FaderController.instance.JustFadedOut) 
		{
			loader.allowSceneActivation = true;
		}
		if (FaderController.instance.FadingOut) 
		{
			GameManager.instance.player.GetComponent<Player> ().canMove = false;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player") 
		{
			Debug.Log (other.gameObject.GetComponent<Player> ().canMove);
			FaderController.instance.FadeOut (1.25f);
			GameManager.instance.StorePlayerInfo ();

			//loader.allowSceneActivation = true;
		}
	}
}
