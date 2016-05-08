using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : Singleton<PauseMenu> 
{
	GameObject[] menuObjects;

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

	public void PauseGame()
	{
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
