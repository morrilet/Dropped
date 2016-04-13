using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : Singleton<GameManager>
{
	public Player.PlayerAmmo playerStoredAmmo;
	public Player.CurrentGun playerStoredGun;
	public float playerStoredHealth;
	public GameObject player;
	public GameObject level;
	bool timeSlowed;
	
	float timeSlowedCounter;
	public bool isPaused;
	public bool isPausedPrev;

	public override void Awake()
	{
		isPersistant = true;

		base.Awake();
	}

	public void Start()
	{
		if (player == null)
			player = GameObject.FindGameObjectWithTag ("Player");
		if (level == null)
			level = GameObject.FindGameObjectWithTag ("Level");

		playerStoredAmmo.machineGunAmmo.maxAmmo = 50;
		playerStoredAmmo.machineGunAmmo.Refill ();
		playerStoredAmmo.shotgunAmmo.maxAmmo = 25;
		playerStoredAmmo.shotgunAmmo.Refill ();
		playerStoredAmmo.pistolAmmo.maxAmmo = 30;
		playerStoredAmmo.pistolAmmo.Refill ();
		playerStoredHealth = player.GetComponent<Player> ().maxHealth;
		playerStoredGun = Player.CurrentGun.None;

		timeSlowed = false;
		timeSlowedCounter = 0;

		isPaused = false;

		AudioManager.instance.PlayMusic ("Ethan Game");
	}

	void Update ()
	{
		HandleInput ();
		if (level.GetComponent<Level> ().enemies.Count == 0 && level.GetComponent<Level> ().preEnemies.Count != 0)
		{
			Debug.Log ("That's all folks.");
			StopCoroutine ("ApplySleep");
			Time.timeScale = 1;
			timeSlowed = true;
			timeSlowedCounter = 0;
			StartCoroutine (LerpTimeScale (.2f, .05f));
		}

		if (timeSlowedCounter >= 2 && timeSlowed)
			StartCoroutine (LerpTimeScale (1f, .4f));

		if(!isPaused)
			timeSlowedCounter += Time.deltaTime / Time.timeScale;
	}

	void HandleInput()
	{
		if (Input.GetKeyDown (KeyCode.Return))
		{
			RestartLevel ();
		}
		if (Input.GetKeyDown (KeyCode.Home))
		{
			StartCoroutine (LerpTimeScale (.2f, 2f));
		}

		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			if (!isPaused) 
			{
				PauseMenu.instance.PauseGame ();
				isPaused = true;
			} 
			else 
			{
				PauseMenu.instance.UnpauseGame ();
				isPaused = false;
			}
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

	public void Sleep(float framesOfSleep)
	{
		StartCoroutine ("ApplySleep", framesOfSleep);
	}

	public IEnumerator ApplySleep (float framesOfSleep) //For small pauses to help the game feel better
	{
		float storedTimeScale = Time.timeScale;

		for (int i = 0; i < framesOfSleep; i++)
		{
			if (i < framesOfSleep - 1)
				Time.timeScale = 0f;
			else
				Time.timeScale = storedTimeScale;
			yield return null;
		}
	}

	public IEnumerator LerpTimeScale(float value, float duration)
	{
		float startingTimeScale = Time.timeScale;

		for (float i = 0; i < duration; i += Time.deltaTime / Time.timeScale)
		{
			Time.timeScale = Mathf.Lerp (startingTimeScale, value, i / duration);
			yield return null;
		}
	}
}
