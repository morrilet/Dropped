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
			player = GameObject.Find ("Player");
		if (level == null)
			level = GameObject.FindGameObjectWithTag ("Level");

		/*
		playerStoredAmmo.machineGunAmmo.maxAmmo = 60;
		playerStoredAmmo.machineGunAmmo.Refill ();
		playerStoredAmmo.shotgunAmmo.maxAmmo = 16;
		playerStoredAmmo.shotgunAmmo.Refill ();
		playerStoredAmmo.pistolAmmo.maxAmmo = 20;
		playerStoredAmmo.pistolAmmo.Refill ();
		playerStoredHealth = player.GetComponent<Player> ().maxHealth;
		playerStoredGun = Player.CurrentGun.None;
		*/
		SetUpPlayer ();

		timeSlowed = false;
		timeSlowedCounter = 0;

		isPaused = false;

		AudioManager.instance.PlayMusic ("bg01_v02 mixed");
	}

	void Update ()
	{
		//Debug.Log ("GM: " + playerStoredAmmo.pistolAmmo.ammountInClip);

		if (SceneManager.GetActiveScene().name == "MainMenu")
			Destroy (instance.gameObject);

		HandleInput ();
		if(level != null)
			if (level.GetComponent<Level> ().enemies.Count == 0 && level.GetComponent<Level> ().enemiesPrev.Count != 0)
			{
				StopCoroutine ("ApplySleep");
				Time.timeScale = 1;
				timeSlowed = true;
				timeSlowedCounter = 0;
				StartCoroutine (LerpTimeScale (.2f, .05f));
			}

		if (timeSlowedCounter >= 2 && timeSlowed)
		{
			timeSlowed = false;
			StartCoroutine (LerpTimeScale (1f, .4f));
		}

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
			StartCoroutine (LerpTimeScale (.2f, .05f));
			timeSlowed = true;
			timeSlowedCounter = 0;
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

	public void SetUpPlayer ()
	{
		playerStoredAmmo.machineGunAmmo.maxAmmo = 60;
		playerStoredAmmo.machineGunAmmo.Refill ();
		playerStoredAmmo.shotgunAmmo.maxAmmo = 16;
		playerStoredAmmo.shotgunAmmo.Refill ();
		playerStoredAmmo.pistolAmmo.maxAmmo = 20;
		playerStoredAmmo.pistolAmmo.Refill ();

		playerStoredAmmo.machineGunAmmo.maxAmountInClip = 15;
		playerStoredAmmo.machineGunAmmo.ammountInClip = playerStoredAmmo.machineGunAmmo.maxAmountInClip;
		playerStoredAmmo.shotgunAmmo.maxAmountInClip = 2;
		playerStoredAmmo.shotgunAmmo.ammountInClip = playerStoredAmmo.shotgunAmmo.maxAmountInClip;
		playerStoredAmmo.pistolAmmo.maxAmountInClip = 5;
		playerStoredAmmo.pistolAmmo.ammountInClip = playerStoredAmmo.pistolAmmo.maxAmountInClip;

		player.GetComponent<Player> ().Start ();

		/*
		playerStoredAmmo.machineGunAmmo.ammountInClip = player.GetComponent<Player> ().machineGun.GetComponent<Gun> ().clipSize;
		playerStoredAmmo.shotgunAmmo.ammountInClip = player.GetComponent<Player> ().shotGun.GetComponent<Gun> ().clipSize;
		playerStoredAmmo.pistolAmmo.ammountInClip = player.GetComponent<Player> ().pistol.GetComponent<Gun> ().clipSize;
		Debug.Log (player.GetComponent<Player> ().pistol.GetComponent<Gun> ().clipSize);
		*/

		playerStoredHealth = player.GetComponent<Player> ().maxHealth;
		playerStoredGun = Player.CurrentGun.None;
	}

	public void ChangeLevel(string levelToChangeTo)
	{
		//playerStoredAmmo = player.GetComponent<Player> ().playerAmmo;
		//Debug.Log (playerStoredAmmo.pistolAmmo.ammountInClip);
		//playerStoredAmmo.machineGunAmmo.ammountInClip = player.GetComponent<Player> ().playerAmmo.machineGunAmmo.ammountInClip;
		//playerStoredAmmo.shotgunAmmo.ammountInClip = player.GetComponent<Player> ().playerAmmo.shotgunAmmo.ammountInClip;
		//playerStoredAmmo.pistolAmmo.ammountInClip = player.GetComponent<Player> ().playerAmmo.pistolAmmo.ammountInClip;

		//playerStoredGun = player.GetComponent<Player> ().currentGun;
		//playerStoredHealth = player.GetComponent<Player> ().health;

		StorePlayerInfo ();

		SceneManager.LoadScene (levelToChangeTo, LoadSceneMode.Single);

		//player.GetComponent<Player> ().playerAmmo.machineGunAmmo.ammountInClip = playerStoredAmmo.machineGunAmmo.ammountInClip;
		//player.GetComponent<Player> ().playerAmmo.shotgunAmmo.ammountInClip = playerStoredAmmo.shotgunAmmo.ammountInClip;
		//player.GetComponent<Player> ().playerAmmo.pistolAmmo.ammountInClip = playerStoredAmmo.pistolAmmo.ammountInClip;
	}

	public void RestartLevel()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex, LoadSceneMode.Single);
	}

	public void StorePlayerInfo()
	{
		playerStoredHealth = player.GetComponent<Player> ().health;
		playerStoredAmmo = player.GetComponent<Player> ().playerAmmo;
		playerStoredGun = player.GetComponent<Player> ().currentGun;
	}

	public void Sleep(float framesOfSleep)
	{
		StartCoroutine ("ApplySleep", framesOfSleep);
	}

	public IEnumerator ApplySleep (float framesOfSleep) //For small pauses to help the game feel better
	{
		float storedTimeScale = 1f;
		if(timeSlowed)
			storedTimeScale = Time.timeScale;

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
		float startingTimeScale = 1;

		for (float i = 0; i < duration; i += Time.deltaTime / Time.timeScale)
		{
			Time.timeScale = Mathf.Lerp (startingTimeScale, value, i / duration);
			yield return null;
		}
	}

	public void FlashWhite(SpriteRenderer sprite, float duration, Color baseColor)
	{
		StartCoroutine(ApplyFlashWhite(sprite, duration, baseColor));
	}

	IEnumerator ApplyFlashWhite(SpriteRenderer sprite, float duration, Color baseColor)
	{
			for (float i = 0; i < duration; i += Time.deltaTime) 
			{	
				if(sprite != null)
				sprite.color = new Color (baseColor.r + .5f * .3f, baseColor.g + .5f * .59f, baseColor.b + .5f * .11f);
				yield return null;
			}

			if(sprite != null)
			sprite.color = baseColor;
	}
}
