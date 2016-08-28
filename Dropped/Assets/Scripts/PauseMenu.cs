using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : Singleton<PauseMenu> 
{
	GameObject[] menuObjects;

	private Vector3 playerStoredVelocity; //This is to resume the player velocity after pausing.
	private Animator[] animatorObjs; //This is used for pausing all animators in the scene.
	private ParticleSystem[] particleObjs; //For pausing particle systems.

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
	}

	public void ReturnToMainMenu()
	{
		SceneManager.LoadScene ("MainMenu", LoadSceneMode.Single);
	}

	public void RestartLevel()
	{
		GameManager.instance.RestartLevel ();
		UnpauseGame ();
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

		GameManager.instance.isPaused = false;
	}
}
