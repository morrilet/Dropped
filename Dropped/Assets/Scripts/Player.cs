using UnityEngine;
using System.Collections;

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

	GameObject ladder; //The ladder that the player is on. Null if not on a ladder.

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

	public enum CurrentGun
	{
		None,
		MachineGun,
		Shotgun,
		Pistol
	}
	public CurrentGun currentGun;

	Gun activeGun; //The gun object currently in use.
	GameObject machineGun;
	GameObject shotGun;
	GameObject pistol;

	public override void Start()
	{
		base.Start ();

		machineGun = transform.FindChild ("Gun_Machinegun").gameObject;
		shotGun = transform.FindChild ("Gun_Shotgun").gameObject;
		pistol = transform.FindChild ("Gun_Pistol").gameObject;

		currentGun = CurrentGun.None;
		activeGun = null;

		playerAmmo.machineGunAmmo.maxAmmo = 50;
		playerAmmo.machineGunAmmo.Refill ();
		playerAmmo.shotgunAmmo.maxAmmo = 25;
		playerAmmo.shotgunAmmo.Refill ();
		playerAmmo.pistolAmmo.maxAmmo = 30;
		playerAmmo.pistolAmmo.Refill ();

		jumpAbility = GetComponent<JumpAbility> ();
		controller = GetComponent<Controller2D> ();
		baseScaleX = transform.localScale.x;
		direction = 1;

		corpseThrowTime = 1.5f;

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

		//Start counting down on jumpLeniency if we just fell.
		if (playerInfo.JustFell)
			jumpAbility.countdownLeniency = true;
		//If we just landed, stop counting down. This also resets it.
		else if (playerInfo.JustLanded || ladder != null)
		{
			jumpAbility.countdownLeniency = false;
			jumpAbility.ResetLeniency ();
		}

		if (GetComponent<Player> ().velocity.x != 0)
			direction = Mathf.Sign (velocity.x);

		if (Input.GetKey (KeyCode.Comma))
			health--;
		if (Input.GetKey (KeyCode.Period))
			health++;

		//Just for now, so that at least SOMETHING happens.
		//In the future make a die method.
		if(!isAlive)
		{
			Application.LoadLevel(Application.loadedLevel);
		}

		//Don't apply gravity if we're on the ground or on a ladder.
		if(controller.collisions.above || controller.collisions.below || ladder != null)
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

		if (corpseCarried)
			corpseCarried.transform.position = transform.position + new Vector3 (0, .9f, 0);

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
			transform.localScale = new Vector3 (baseScaleX * Mathf.Sign (velocity.x), transform.localScale.y, transform.localScale.z);
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
			activeGun = machineGun.GetComponent<Gun>();
			break;
		case CurrentGun.Shotgun:
			machineGun.SetActive (false);
			shotGun.SetActive (true);
			pistol.SetActive (false);
			activeGun = shotGun.GetComponent<Gun>();
			break;
		case CurrentGun.Pistol:
			machineGun.SetActive (false);
			shotGun.SetActive (false);
			pistol.SetActive (true);
			activeGun = pistol.GetComponent<Gun>();
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

		controller.Move (velocity * Time.deltaTime);

		if (playerInfo.JustJumped)
			Debug.Log ("JustJumped");
		if (playerInfo.JustLanded)
			Debug.Log ("JustLanded");
		if (playerInfo.JustFell)
			Debug.Log ("JustFell");
		playerInfo.Reset ();
	}

	//This handles all of the players input, separated from the update method for easy
	//enabling/disabling without interrupting the rest of the players mechanics.
	void HandleInput()
	{
		//Input vector. GetAxisRaw applies no smoothing, so keyboard input is either -1, 0, or 1. Always.
		input = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		//Jumping.
		if(Input.GetKeyDown(KeyCode.Space))
		{
			if(jumpAbility != null && jumpAbility.canJump)
			{
				jumpAbility.Jump(ref velocity);
			}

			if (ladder != null)
				ladder = null;
		}

		if (Input.GetKeyDown (KeyCode.C)) 
		{
			if (corpseCarried == null)
			{
				GameObject[] corpses = GameObject.FindGameObjectsWithTag ("Corpse");
				for (int i = 0; i < corpses.Length; i++) 
				{
					if (controller.coll.IsTouching (corpses [i].GetComponent<Collider2D> ())) 
					{
						PickupCorpse (corpses [i]);
						break;
					}
				}
			} 
			else 
			{
				throwingCorpse = true;
			}
		}

		if (Input.GetKeyUp (KeyCode.C) && throwingCorpse) 
		{
			DropCorpse (corpseThrowForce);
			throwingCorpse = false;
			corpseThrowCount = 0;
		}

		if (input.y > 0) 
		{
			foreach (GameObject ladderObj in GameObject.FindGameObjectsWithTag("Ladder")) 
			{
				if (controller.coll.IsTouching (ladderObj.GetComponent<Collider2D> ())) 
				{
					ladder = ladderObj;
				}
			}
		}

		if (activeGun != null) 
		{
			if (activeGun.isAuto) 
			{
				if (Input.GetKey (KeyCode.X))
					ShootGun ();
			} 
			else if(!activeGun.isAuto)
			{
				if (Input.GetKeyDown (KeyCode.X))
					ShootGun ();
			}
		}
	}

	void ShootGun()
	{
		switch (currentGun) 
		{
		case CurrentGun.None:
			break;
		case CurrentGun.MachineGun:
			if (playerAmmo.machineGunAmmo.currentAmmo > 0) 
			{
				if(activeGun.Shoot ())
					playerAmmo.machineGunAmmo.ModifyAmmo (-1);
			}
			break;
		case CurrentGun.Shotgun:
			if (playerAmmo.shotgunAmmo.currentAmmo > 0) 
			{
				if(activeGun.Shoot ())
					playerAmmo.shotgunAmmo.ModifyAmmo (-1);
			}
			break;
		case CurrentGun.Pistol:
			if (playerAmmo.pistolAmmo.currentAmmo > 0) {
				if (activeGun.Shoot ())
					playerAmmo.pistolAmmo.ModifyAmmo (-1);
			}
			break;
		}
	}

	void PickupCorpse(GameObject corpse)
	{
		corpseCarried = corpse;
		corpseCarried.transform.position = transform.position + new Vector3(0, 1, 0);
		corpseCarried.GetComponent<Rigidbody2D> ().isKinematic = true;
		corpseCarried.transform.rotation = Quaternion.identity;
		corpseCarried.layer = LayerMask.NameToLayer("Default");
	}

	void DropCorpse(float forceModifier)
	{
		corpseCarried.GetComponent<Rigidbody2D> ().isKinematic = false;

		Vector2 force = corpseThrowDirection * forceModifier;
		corpseCarried.GetComponent<Rigidbody2D> ().AddForce (force, ForceMode2D.Impulse);

		corpseCarried.layer = LayerMask.NameToLayer("Obstacle");
		corpseCarried = null;
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
		public bool JustLanded; //Landing from a jump OR a fall.

		//Resets info.
		public void Reset()
		{
			JustJumped = false;
			JustFell = false;
			JustLanded = false;
		}
	}
	#endregion
}
