using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent (typeof (Controller2D))]
[RequireComponent (typeof (JumpAbility))] //The player will always have the ability to jump.
public class Player : Entity
{
	[HideInInspector]
	public PlayerInfo playerInfo;
	[HideInInspector]
	public PlayerAmmo playerAmmo;

	[HideInInspector]
	public float accelerationTimeAirborne = .2f;
	[HideInInspector]
	public float accelerationTimeGrounded = .1f;
	float moveSpeed = 6;
	float baseScaleX;

	Vector2 input;

	[HideInInspector]
	public bool canMove; //Determines whether or not the player is allowed to move.

	[HideInInspector]
	public float gravity;

	[HideInInspector]
	public Vector3 velocity;
	float velocityXSmoothing;

	[HideInInspector]
	public Controller2D controller;

	//Jumping
	[HideInInspector]
	public JumpAbility jumpAbility;

	[HideInInspector]
	public GameObject ladder; //The ladder that the player is on. Null if not on a ladder.
	float ladderExitTime;  //How long it takes to get off of the ladder via left and right input.
	float ladderExitTimer; //Timer to count up to ladderExitTime.

	[HideInInspector]
	public GameObject corpseCarried; //The corpse that is being carried. Null if no corpse is carried.
	[HideInInspector]
	public bool throwingCorpse = false; //Whether we should count up on the counter or not.
	float corpseThrowTime; //Max time to hold the throw button before it has no effect.
	float corpseThrowCount; //Counter for throw hold.

	[HideInInspector]
	public Vector3 corpseThrowDirection; //The direction to throw the corpse in.
	[HideInInspector]
	public float corpseThrowForce; //The strength with which to throw the corpse.

	[HideInInspector]
	public float direction;//Direction player is facing

	float horizontalAxisPrev;

	bool isStrafing;

	public enum CurrentGun
	{
		None,
		MachineGun,
		Shotgun,
		Pistol
	}
	public CurrentGun currentGun;

	[HideInInspector]
	public Gun activeGun; //The gun object currently in use.
	[HideInInspector]
	public GameObject machineGun;
	[HideInInspector]
	public GameObject shotGun;
	[HideInInspector]
	public GameObject pistol;

	[HideInInspector]
	public float grappleEscapeAttempt; //How much the player has slammed the button.
	[HideInInspector]
	public List<EnemyAI> grapplingEnemies;
	[HideInInspector]
	public float grappleStrength; //How much the player has to slam the button to escape the grapple.
	[HideInInspector]
	public bool canBeGrabbed; //Whether or not the player can be grabbed.
	private float grabSafeTime; //How long is the player safe from grabs after escaping a grab?
	private float grabSafeTimer; //Timer for grabSafeTime.

	Animator animator;

	public void Start()
	{
		machineGun = transform.FindChild ("Gun_Machinegun").gameObject;
		shotGun = transform.FindChild ("Gun_Shotgun").gameObject;
		pistol = transform.FindChild ("Gun_Pistol").gameObject;

		currentGun = GameManager.instance.playerStoredGun;
		activeGun = null;

		machineGun.GetComponent<Gun> ().ammoInClip = GameManager.instance.playerStoredAmmo.machineGunAmmo.ammountInClip;
		shotGun.GetComponent<Gun> ().ammoInClip = GameManager.instance.playerStoredAmmo.shotgunAmmo.ammountInClip;
		pistol.GetComponent<Gun> ().ammoInClip = GameManager.instance.playerStoredAmmo.pistolAmmo.ammountInClip;

		GameManager.instance.player = transform.gameObject; //On level load, this will allow the gamemanger to track the new player game object

		playerAmmo = GameManager.instance.playerStoredAmmo;

		if (GameManager.instance.playerStoredHealth != 0)
			health = GameManager.instance.playerStoredHealth;

		jumpAbility = GetComponent<JumpAbility> ();
		controller = GetComponent<Controller2D> ();
		baseScaleX = transform.localScale.x;
		direction = 1;

		corpseThrowTime = 1.5f;

		ladderExitTime = .25f;
		ladderExitTimer = 0;

		grapplingEnemies = new List<EnemyAI> ();
		canBeGrabbed = true;
		grabSafeTime = 1f;
		grabSafeTimer = 0f;

		animator = GetComponent<Animator> ();

		canMove = true;
	}

	public override void Update()
	{
		base.Update ();

		//Getting playerInfo falling and landing states.
		if (controller.collisions.below && !controller.collisions.belowPrev)
			playerInfo.JustLanded = true;
		else if (controller.collisions.movingPlatform != null) //A hacky fix for not being able to jump from moving platforms due to not being considered on the ground.
			playerInfo.JustLanded = true;
		if (!controller.collisions.below && controller.collisions.belowPrev && !playerInfo.JustJumped)
			playerInfo.JustFell = true;

		if (velocity.y < 0 && !controller.collisions.below)
			playerInfo.IsFalling = true;
		else
			playerInfo.IsFalling = false;

		//Start counting down on jumpLeniency if we just fell.
		if (playerInfo.JustFell)
			jumpAbility.countdownLeniency = true;
		//If we just landed, stop counting down. This also resets it.
		else if (playerInfo.JustLanded || ladder != null)
		{
			jumpAbility.countdownLeniency = false;
			jumpAbility.ResetLeniency ();
		}

		if (GetComponent<Player> ().velocity.x != 0 && canMove)
		{
			if(!isStrafing && (int)velocity.x != 0)
				direction = Mathf.Sign (velocity.x);
		}
		//Just for now, so that at least SOMETHING happens.
		//In the future make a die method.
		if(!isAlive)
		{
			GameManager.instance.RestartLevel ();
			//Application.LoadLevel(Application.loadedLevel);
		}

		//Don't apply gravity if we're on the ground or on a ladder or the game is paused.
		if(controller.collisions.above || controller.collisions.below || ladder != null || GameManager.instance.isPaused)
		{
			velocity.y = 0;
		}

		if (throwingCorpse) 
		{
			corpseThrowCount += Time.deltaTime;
			//Debug.Log (corpseThrowCount / corpseThrowTime);
		}

		if (corpseThrowCount >= corpseThrowTime)
			corpseThrowCount = corpseThrowTime;

		//if (corpseCarried)
			//corpseCarried.GetComponent<Rigidbody2D> ().MovePosition ((Vector2)transform.position + new Vector2 (0f, .9f));
			//corpseCarried.GetComponent<CorpseRagdoll>().upperTorso.GetComponent<Rigidbody2D>().MovePosition(transform.position + new Vector3 (0, .9f, 0));

		if (canMove)
			HandleInput ();
		else
			input = Vector2.zero;

		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		if(ladder == null)
			velocity.y += gravity * Time.deltaTime;

		if (velocity.x != 0)
		{
			transform.localScale = new Vector3 (baseScaleX * direction, transform.localScale.y, transform.localScale.z);
		}

		if (ladder != null)
		{
			velocity.y = input.y * moveSpeed / 2f;
			velocity.x = 0;
			transform.position = new Vector3 (ladder.transform.position.x, transform.position.y, transform.position.z);

			if (transform.position.y - controller.coll.bounds.extents.y
				> ladder.transform.position.y + ladder.GetComponent<Collider2D> ().bounds.extents.y)  //If bottom is higher than ladder top... 
			{
				ladder = null;
			} 
			else if (transform.position.y + controller.coll.bounds.extents.y
				< ladder.transform.position.y - ladder.GetComponent<Collider2D> ().bounds.extents.y) //If top is lower than ladder bottom...
			{ 
				ladder = null;
			}
		}

		//GameManager.instance.playerStoredAmmo = playerAmmo;
		switch (currentGun)
		{
		case CurrentGun.None:
			machineGun.SetActive (false);
			shotGun.SetActive (false);
			pistol.SetActive (false);
			activeGun = null;
			break;
		case CurrentGun.MachineGun:
			machineGun.SetActive (true);
			shotGun.SetActive (false);
			pistol.SetActive (false);
			activeGun = machineGun.GetComponent<Gun> ();
			//activeGun.ammoInClip = GameManager.instance.playerStoredAmmo.machineGunAmmo.ammountInClip;
			playerAmmo.machineGunAmmo.ammountInClip = activeGun.ammoInClip; //GameManager.instance.playerStoredAmmo.machineGunAmmo.ammountInClip;
			break;
		case CurrentGun.Shotgun:
			machineGun.SetActive (false);
			shotGun.SetActive (true);
			pistol.SetActive (false);
			activeGun = shotGun.GetComponent<Gun> ();
			//activeGun.ammoInClip = GameManager.instance.playerStoredAmmo.shotgunAmmo.ammountInClip;
			playerAmmo.shotgunAmmo.ammountInClip = activeGun.ammoInClip; //GameManager.instance.playerStoredAmmo.shotgunAmmo.ammountInClip;
			break;
		case CurrentGun.Pistol:
			machineGun.SetActive (false);
			shotGun.SetActive (false);
			pistol.SetActive (true);
			activeGun = pistol.GetComponent<Gun> ();
			//activeGun.ammoInClip = GameManager.instance.playerStoredAmmo.pistolAmmo.ammountInClip;
			playerAmmo.pistolAmmo.ammountInClip = activeGun.ammoInClip; //GameManager.instance.playerStoredAmmo.pistolAmmo.ammountInClip;
			break;
		}

		corpseThrowForce = Mathf.Clamp((corpseThrowCount / corpseThrowTime), .2f, 1f);
		corpseThrowDirection = new Vector3 (10, 5, 0) + transform.right;
		corpseThrowDirection.x *= direction;

		if (currentGun != CurrentGun.None)
		{
			if (direction == 1)
				Camera.main.GetComponent<CameraFollowTrap> ().cameraFollowMode = CameraFollowTrap.CameraFollowMode.Right_Extended;
			if (direction == -1)
				Camera.main.GetComponent<CameraFollowTrap> ().cameraFollowMode = CameraFollowTrap.CameraFollowMode.Left_Extended;
		}
		else
			Camera.main.GetComponent<CameraFollowTrap> ().cameraFollowMode = CameraFollowTrap.CameraFollowMode.Center;

		if (ladder != null) 
		{
			if (ladderExitTimer >= ladderExitTime)
				ladderExitTimer = ladderExitTime;
			ladderExitTimer += Time.deltaTime;
		} 
		else 
		{
			ladderExitTimer = 0f;
		}

		if (grapplingEnemies.Count > 0 && !GameManager.instance.isPaused) 
		{
			if (corpseCarried != null)
				DropCorpse ();
			EscapeGrapple ();
			//Debug.Log (grappleEscapeAttempt + ", " + grappleStrength);
		}
		if (!canBeGrabbed) 
		{
			grabSafeTimer += Time.deltaTime;
		}
		if (grabSafeTimer >= grabSafeTime) 
		{
			canBeGrabbed = true;
			grabSafeTimer = 0f;
		}
		grapplingEnemies = grapplingEnemies.Where (Enemy => Enemy != null).ToList (); //Remove null elements from the list.

		controller.Move (velocity * Time.deltaTime);

		horizontalAxisPrev = Input.GetAxisRaw ("Horizontal");

		if (playerInfo.JustJumped) 
		{
			//Debug.Log ("JustJumped");
			animator.SetTrigger ("JustJumped");
		}
		if (playerInfo.JustLanded) 
		{
			//Debug.Log ("JustLanded");
			animator.SetTrigger ("JustLanded");
		}
		if (playerInfo.JustFell) 
		{
			//Debug.Log ("JustFell");
		}
		if (playerInfo.IsFalling && !playerInfo.IsFallingPrev)
		{
			animator.SetTrigger ("Falling");
			//Debug.Log ("Falling");
		}



		//Debug.Log (velocity);

		animator.SetFloat ("PlayerSpeed", Mathf.Abs (velocity.x));
		//Time.timeScale = .1f;
		playerInfo.Reset ();
	}

	//This handles all of the players input, separated from the update method for easy
	//enabling/disabling without interrupting the rest of the players mechanics.
	void HandleInput()
	{
		//Input vector. GetAxisRaw applies no smoothing, so keyboard input is either -1, 0, or 1. Always.
		input = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		//For debugging purposes!!!
		if (Input.GetKeyDown (KeyCode.Equals)) 
		{
			playerAmmo.RefillAll ();
		}

		//Jumping.
		if(Input.GetButtonDown("Jump"))
		{
			if(jumpAbility != null && jumpAbility.canJump)
			{
				AudioManager.instance.PlaySoundEffectVariation ("JumpSound", .85f, 1.15f);
				jumpAbility.Jump(ref velocity);
			}

			if (ladder != null)
				ladder = null;
		}

		if (Input.GetButtonDown("Action")) 
		{
			if (corpseCarried == null)
			{
				if(GetTouchingCorpse() != null)
				PickupCorpse (GetTouchingCorpse ());
				//GameObject[] corpses = GameObject.FindGameObjectsWithTag ("Corpse");
				//for (int i = 0; i < corpses.Length; i++) 
				//{
					//if (controller.coll.IsTouching (corpses [i].GetComponent<Collider2D> ())) 
					//{
						//PickupCorpse (corpses [i]);
						//break;
					//}
				//}
			} 
			else 
			{
				throwingCorpse = true;
			}
		}

		if (GetTouchingCorpse() != null && corpseCarried == null)
			GetTouchingCorpse ().transform.GetComponentInParent<CorpseRagdoll> ().SetOutline (true);

		if (Input.GetButtonUp ("Action") && throwingCorpse) 
		{
			ThrowCorpse (corpseThrowForce);
			throwingCorpse = false;
			corpseThrowCount = 0;
		}

		if (input.y != 0) 
		{
			foreach (GameObject ladderObj in GameObject.FindGameObjectsWithTag("Ladder")) 
			{
				if (controller.coll.IsTouching (ladderObj.GetComponent<Collider2D> ())) 
				{
					ladder = ladderObj;
				}
			}
		}

		if (input.x != 0 && ladder != null && ladderExitTimer >= ladderExitTime) 
		{
			ladderExitTimer = 0f;
			ladder = null;
		}

		if (activeGun != null) 
		{
			if (activeGun.isAuto) 
			{
				if (Input.GetButton("Fire1"))
					ShootGun ();
			} 
			else if(!activeGun.isAuto)
			{
				if (Input.GetButtonDown ("Fire1"))
					ShootGun ();
			}
		}

		if (activeGun != null)
		{
			if(Input.GetButtonDown("Reload") && !activeGun.isReloading && activeGun.ammoInClip != activeGun.clipSize)
			{
				switch (currentGun)
				{
				case CurrentGun.MachineGun:
					if (playerAmmo.machineGunAmmo.currentAmmo >= activeGun.bulletsSpentFromCurrentClip)
						ReloadActiveGun (-activeGun.bulletsSpentFromCurrentClip);
					else if (playerAmmo.machineGunAmmo.currentAmmo > 0)
						ReloadActiveGun (-playerAmmo.machineGunAmmo.currentAmmo);
					break;
				case CurrentGun.Shotgun:
					if (playerAmmo.shotgunAmmo.currentAmmo >= activeGun.bulletsSpentFromCurrentClip)
						ReloadActiveGun (-activeGun.bulletsSpentFromCurrentClip);
					else if (playerAmmo.shotgunAmmo.currentAmmo > 0)
						ReloadActiveGun (-playerAmmo.shotgunAmmo.currentAmmo);
					break;
				case CurrentGun.Pistol:
					if (playerAmmo.pistolAmmo.currentAmmo >= activeGun.bulletsSpentFromCurrentClip)
						ReloadActiveGun (-activeGun.bulletsSpentFromCurrentClip);
					else if (playerAmmo.pistolAmmo.currentAmmo > 0)
						ReloadActiveGun (-playerAmmo.pistolAmmo.currentAmmo);
					break;
				}
			}
		}

		if (Input.GetButton ("Strafe"))
			isStrafing = true;
		else
			isStrafing = false;
	}

	void EscapeGrapple()
	{
		GUI_Script.instance.escapeObjectsEnabled = true;

		if (Input.GetAxisRaw("Horizontal") != 0 && horizontalAxisPrev == 0) 
		{
			grappleEscapeAttempt += 1f;
			Camera.main.GetComponent<CameraFollowTrap> ().ScreenShake (.1f, .05f);
		}

		if (grappleEscapeAttempt >= grappleStrength) 
		{
			GUI_Script.instance.escapeObjectsEnabled = false;

			canMove = true;

			foreach (EnemyAI enemy in grapplingEnemies) 
			{
				enemy.isGrapplingPlayer = false;
				enemy.currentState = EnemyAI.States.ChasePlayer;

				Vector3 knockBackVelocity = new Vector3 (1f, 0f, 0f);
				knockBackVelocity.x *= (enemy.transform.position.x - transform.position.x) / Mathf.Abs (enemy.transform.position.x - transform.position.x);
				enemy.KnockBack (knockBackVelocity, .15f);
			}
			grapplingEnemies.Clear ();

			grappleStrength = 0;
			grappleEscapeAttempt = 0;

			canBeGrabbed = false;
			grabSafeTimer = 0f;
		}

		if (grappleEscapeAttempt <= 0f)
			grappleEscapeAttempt = 0f;
		grappleEscapeAttempt -= Time.deltaTime;
	}

	void ShootGun()
	{
		switch (currentGun) 
		{
		case CurrentGun.None:
			break;
		case CurrentGun.MachineGun:
			if (activeGun.ammoInClip > 0) 
			{
				activeGun.Shoot ();
			} 
			else if(activeGun.fireRateCount >= activeGun.fireRate)
			{
				AudioManager.instance.PlaySoundEffect ("GunEmptyClick");
				activeGun.fireRateCount = 0;
			}
			break;
		case CurrentGun.Shotgun:
			if (activeGun.ammoInClip > 0) 
			{
				activeGun.Shoot ();
			}
			else if(activeGun.fireRateCount >= activeGun.fireRate)
			{
				AudioManager.instance.PlaySoundEffect ("GunEmptyClick");
				activeGun.fireRateCount = 0;
			}
			break;
		case CurrentGun.Pistol:
			if (activeGun.ammoInClip > 0) 
			{
				activeGun.Shoot ();
			}
			else if(activeGun.fireRateCount >= activeGun.fireRate)
			{
				AudioManager.instance.PlaySoundEffect ("GunEmptyClick");
				activeGun.fireRateCount = 0;
			}
			break;
		}
	}

	void ReloadActiveGun(int ammoModifier)
	{
		activeGun.Reload (-ammoModifier);
		switch (currentGun)
		{
		case CurrentGun.MachineGun:
			playerAmmo.machineGunAmmo.ModifyAmmo (ammoModifier);
			break;
		case CurrentGun.Shotgun:
			playerAmmo.shotgunAmmo.ModifyAmmo (ammoModifier);
			break;
		case CurrentGun.Pistol:
			playerAmmo.pistolAmmo.ModifyAmmo (ammoModifier);
			break;
		}
	}

	void PickupCorpse(GameObject corpse)
	{
		corpseCarried = corpse.transform.GetComponentInParent<CorpseRagdoll> ().gameObject;
		corpseCarried.GetComponentInParent<CorpseRagdoll> ().SetOutline (false);
		corpseCarried.GetComponentInParent<CorpseRagdoll> ().isCarried = true;
		//corpseCarried.transform.position = transform.position + new Vector3(0, 1, 0);
		//corpseCarried.GetComponent<Rigidbody2D> ().isKinematic = true;
		//corpseCarried.transform.rotation = Quaternion.identity;
		//corpseCarried.layer = LayerMask.NameToLayer("Default");
	}

	void DropCorpse()
	{
		corpseCarried.GetComponent<CorpseRagdoll> ().isCarried = false;
		throwingCorpse = false;
		corpseCarried = null;
	}

	void ThrowCorpse(float forceModifier)
	{
		//corpseCarried.GetComponent<Rigidbody2D> ().isKinematic = false;

		//Vector2 force = corpseThrowDirection * forceModifier;
		//corpseCarried.GetComponent<Rigidbody2D> ().AddForce (force, ForceMode2D.Impulse);

		//corpseCarried.layer = LayerMask.NameToLayer("Obstacle");
		corpseCarried.GetComponent<CorpseRagdoll>().isCarried = false;

		Vector2 force = corpseThrowDirection * forceModifier;
		corpseCarried.GetComponent<CorpseRagdoll> ().AddForce (force, ForceMode2D.Impulse);
		//for (int i = 0; i < corpseCarried.transform.childCount; i++) 
		//{
			//corpseCarried.transform.GetChild (i).GetComponent<Rigidbody2D> ().isKinematic = false;
			//corpseCarried.GetComponent<CorpseRagdoll>().AddForce(force, ForceMode2D.Impulse);
		//}

		corpseCarried = null;
	}

	public GameObject GetTouchingCorpse() //Returns the corpse segment we're touching.
	{
		GameObject FirstCorpseTouching = null;
		GameObject[] corpses = GameObject.FindGameObjectsWithTag ("Corpse");
		for (int i = 0; i < corpses.Length; i++)
		{
			if (controller.coll.IsTouching (corpses [i].GetComponent<Collider2D> ()))
			{
				if (FirstCorpseTouching == null)
					FirstCorpseTouching = corpses [i].gameObject;
			} 
			else 
			{
				corpses [i].transform.GetComponentInParent<CorpseRagdoll> ().SetOutline (false);
			}
		}
		return FirstCorpseTouching;
	}

	#region Custom Data
	public struct PlayerAmmo
	{
		public AmmoType machineGunAmmo;
		public AmmoType shotgunAmmo;
		public AmmoType pistolAmmo;

		public void RefillAll()
		{
			machineGunAmmo.Refill ();
			shotgunAmmo.Refill ();
			pistolAmmo.Refill ();
		}
	}

	public struct AmmoType
	{
		public int ammountInClip;
		public int maxAmountInClip;
		public int maxAmmo;
		public int currentAmmo;

		public void Refill()
		{
			currentAmmo = maxAmmo;
		}

		public void ModifyAmmo(int change)
		{
			if (currentAmmo + change >= 0 && currentAmmo + change <= maxAmmo) 
			{
				currentAmmo += change;
			} 
			else if (currentAmmo + change > maxAmmo)
				currentAmmo = maxAmmo;
		}
	}

	public struct PlayerInfo
	{
		public bool JustJumped;
		public bool JustFell;
		public bool IsFalling;
		public bool IsFallingPrev;
		public bool JustLanded; //Landing from a jump OR a fall.

		//Resets info.
		public void Reset()
		{
			JustJumped = false;
			JustFell = false;
			IsFallingPrev = IsFalling;
			IsFalling = false;
			JustLanded = false;
		}
	}
	#endregion
}
