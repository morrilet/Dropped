using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

	public Player.PlayerAmmo playerStoredAmmo;

	//Static Singleton property
	public static GameManager Instance { get; private set; }

	void Awake()
	{
		//Find and destroy and conflicting singleton instances
		if (Instance != null && Instance != this)
			Destroy (GameManager.Instance.gameObject);
		//Store singelton instance
		Instance = this;
		//GameManager prefab persists between scenes
		DontDestroyOnLoad (gameObject);
	}

	void Update ()
	{
		HandleInput ();
	}

	void HandleInput()
	{
		if (Input.GetKeyDown (KeyCode.R))
		{
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);

		}
			
	}

	public void ChangeLevel(string levelToChangeTo)
	{
//		playerStoredAmmo = player.GetComponent<Player> ().playerAmmo;
		SceneManager.LoadScene (levelToChangeTo, LoadSceneMode.Single);
	}
}
