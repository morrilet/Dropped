using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : Singleton<PauseMenu> 
{
	GameObject[] menuObjects;

	private Vector3 playerStoredVelocity; //This is to resume the player velocity after pausing.
	private Animator[] animatorObjs; //This is used for pausing all animators in the scene.
	private ParticleSystem[] particleObjs; //For pausing particle systems.
	private Rope[] ropes; //For pausing ropes.
	private TrailRenderer[] trailObjs;
	private float[] trailObjsStoredTimes;

	public override void Awake()
	{
		isPersistant = false;
		base.Awake ();
	}

	void Start()
	{
		menuObjects = new GameObject[transform.childCount];
		for (int i = 0; i < transform.childCount; i++) 
		{
			menuObjects [i] = transform.GetChild (i).gameObject;
			menuObjects [i].gameObject.SetActive (false);
		}
			
		animatorObjs = GameObject.FindObjectsOfType<Animator> ();
		ropes = GameObject.FindObjectsOfType<Rope> ();

		AudioManager.instance.SetAudioMenuActive (false);
	}

	public void ReturnToMainMenu()
	{
		StartCoroutine (FadeToMainMenu ());
	}

	private IEnumerator FadeToMainMenu()
	{
		//Loading the main menu async may be overkill but I figure it buys an 
		//extra half second of load time that the user doesn't sit through.
		//AsyncOperation tempLoader = SceneManager.LoadSceneAsync ("MainMenu", LoadSceneMode.Single);
		//tempLoader.allowSceneActivation = false;
		FaderController.instance.FadeOut (.75f);
		yield return new WaitForSeconds (.75f);
		//tempLoader.allowSceneActivation = true;
		SceneManager.LoadScene ("MainMenu", LoadSceneMode.Single);
	}

	public void RestartLevel()
	{
		StartCoroutine (FadeToRestartLevel ());
	}

	private IEnumerator FadeToRestartLevel()
	{
		FaderController.instance.FadeOut (.4f);
		yield return new WaitForSeconds (.4f);
		GameManager.instance.RestartLevel ();
		UnpauseGame ();
	}

	public void EnableAudioMenu()
	{
		AudioManager.instance.SetAudioMenuActive (true);
		foreach (GameObject menuObj in menuObjects) 
		{
			if(menuObj.name != "Background")
				menuObj.SetActive (false);
		}
	}

	public void DisableAudioMenu()
	{
		AudioManager.instance.SetAudioMenuActive (false);
		foreach (GameObject menuObj in menuObjects) 
		{
			if(menuObj.name != "Background")
				menuObj.SetActive (true);
		}
	}

	public void PauseGame()
	{
		playerStoredVelocity = GameManager.instance.player.GetComponent<Player> ().velocity;
		GameManager.instance.player.GetComponent<Player> ().velocity = Vector3.zero;
		GameManager.instance.player.GetComponent<Player> ().canMove = false;

		GameObject[] ragdolls = GameObject.FindGameObjectsWithTag ("Ragdoll");
		for (int i = 0; i < ragdolls.Length; i++) 
		{
			ragdolls [i].GetComponent<CorpseRagdoll> ().PauseCorpsePhysics ();
		}

		for (int i = 0; i < menuObjects.Length; i++) 
		{
			menuObjects [i].gameObject.SetActive (true);
		}

		for (int i = 0; i < animatorObjs.Length; i++) 
		{
			if(animatorObjs[i] != null)
				animatorObjs [i].speed = 0f;
		}

		//Pause any active particle systems... Would declare in start but they're largely inactive.
		particleObjs = GameObject.FindObjectsOfType<ParticleSystem>();
		for (int i = 0; i < particleObjs.Length; i++) 
		{
			particleObjs [i].Pause ();
		}

		//Pause any bullets.
		trailObjs = GameObject.FindObjectsOfType<TrailRenderer>();
		for (int i = 0; i < trailObjs.Length; i++) 
		{
			trailObjsStoredTimes = new float[trailObjs.Length];
			trailObjsStoredTimes [i] = trailObjs [i].time;
			trailObjs [i].time = float.PositiveInfinity;
		}

		//Pause any ropes.
		for (int i = 0; i < ropes.Length; i++) 
		{
			ropes [i].Pause ();
		}

		GameManager.instance.isPaused = true;
	}

	public void UnpauseGame()
	{
		GameManager.instance.player.GetComponent<Player> ().canMove = true;
		GameManager.instance.player.GetComponent<Player> ().velocity = playerStoredVelocity;

		GameObject[] ragdolls = GameObject.FindGameObjectsWithTag ("Ragdoll");
		for (int i = 0; i < ragdolls.Length; i++) 
		{
			ragdolls [i].GetComponent<CorpseRagdoll> ().ResumeCorpsePhysics ();
		}

		for (int i = 0; i < menuObjects.Length; i++) 
		{
			menuObjects [i].gameObject.SetActive (false);
		}

		for (int i = 0; i < animatorObjs.Length; i++) 
		{
			if(animatorObjs[i] != null)
				animatorObjs [i].speed = 1f;
		}

		for (int i = 0; i < particleObjs.Length; i++) 
		{
			particleObjs [i].Play ();
		}

		//Pause any bullets.
		for (int i = 0; i < trailObjs.Length; i++) 
		{
			trailObjs [i].time = trailObjsStoredTimes [i];
		}

		//Resume any ropes.
		for (int i = 0; i < ropes.Length; i++) 
		{
			ropes [i].Resume ();
		}

		GameManager.instance.isPaused = false;
	}
}
