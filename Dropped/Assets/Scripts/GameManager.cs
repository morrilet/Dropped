using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : Singleton<GameManager>
{
	public Player.PlayerAmmo playerStoredAmmo;
	public Player.CurrentGun playerStoredGun;
	public float playerStoredHealth;
	public GameObject player;

	public override void Awake()
	{
		isPersistant = true;

		base.Awake();
	}

	public void Start()
	{
		playerStoredAmmo.machineGunAmmo.maxAmmo = 50;
		playerStoredAmmo.machineGunAmmo.Refill ();
		playerStoredAmmo.shotgunAmmo.maxAmmo = 25;
		playerStoredAmmo.shotgunAmmo.Refill ();
		playerStoredAmmo.pistolAmmo.maxAmmo = 30;
		playerStoredAmmo.pistolAmmo.Refill ();
		playerStoredHealth = player.GetComponent<Player> ().maxHealth;
		playerStoredGun = Player.CurrentGun.None;
	}

	void Update ()
	{
		HandleInput ();
	}

	void HandleInput()
	{
		if (Input.GetKeyDown (KeyCode.R))
		{
			RestartLevel ();
		}
			
	}

	public void ChangeLevel(string levelToChangeTo)
	{
		playerStoredAmmo = player.GetComponent<Player> ().playerAmmo;
		playerStoredGun = player.GetComponent<Player> ().currentGun;
		playerStoredHealth = player.GetComponent<Player> ().health;
		SceneManager.LoadScene (levelToChangeTo, LoadSceneMode.Single);
	}

	public void RestartLevel()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex, LoadSceneMode.Single);
	}
}
