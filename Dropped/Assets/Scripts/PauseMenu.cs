using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : Singleton<PauseMenu> 
{
	GameObject[] menuObjects;

	private Vector3 playerStoredVelocity; //This is to resume the player velocity after pausing.

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

		GameManager.instance.isPaused = false;
	}
}
