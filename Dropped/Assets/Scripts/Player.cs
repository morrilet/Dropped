using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
[RequireComponent (typeof (JumpAbility))] //The player will always have the ability to jump.
public class Player : Entity
{
	[HideInInspector]
	public PlayerInfo playerInfo;

	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
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

	GameObject corpseCarried; //The corpse that is being carried.
	bool throwingCorpse = false; //Whether we should count up on the counter or not.
	float corpseThrowTime; //Max time to hold the throw button before it has no effect.
	float corpseThrowCount; //Counter for throw hold.

	[HideInInspector]
	public float direction;//Direction player is facing

	public override void Start()
	{
		base.Start ();

		jumpAbility = GetComponent<JumpAbility> ();
		controller = GetComponent<Controller2D> ();
		baseScaleX = transform.localScale.x;

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
		else if (playerInfo.JustLanded)
		{
			jumpAbility.countdownLeniency = false;
			jumpAbility.ResetLeniency ();
		}
			
		if (GetComponent<Player> ().velocity.x != 0)
			direction = Mathf.Sign (velocity.x);

		//Just for now, so that at least SOMETHING happens.
		//In the future make a die method.
		if(!isAlive)
		{
			Application.LoadLevel(Application.loadedLevel);
		}

		//Don't apply gravity if we're on the ground.
		if(controller.collisions.above || controller.collisions.below)
		{
			velocity.y = 0;
		}

		if (throwingCorpse) 
		{
			corpseThrowCount += Time.deltaTime;
			Debug.Log (corpseThrowCount / corpseThrowTime);
		}

		if (corpseThrowCount >= corpseThrowTime)
			corpseThrowCount = corpseThrowTime;

		if (corpseCarried)
			corpseCarried.transform.position = transform.position + new Vector3 (0, .75f, 0);

		if (canMove)
			HandleInput ();
		else
			input = Vector2.zero;

		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		velocity.y += gravity * Time.deltaTime;

		if (velocity.x != 0)
		{
			transform.localScale = new Vector3 (baseScaleX * Mathf.Sign (velocity.x), transform.localScale.y, transform.localScale.z);
		}

		controller.Move (velocity * Time.deltaTime);

		if (playerInfo.JustJumped)
			Debug.Log ("JustJumped");
		if (playerInfo.JustLanded)
			Debug.Log ("JustLanded");
		if(playerInfo.JustFell)
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
		if(jumpAbility != null)
		{
			if(Input.GetKeyDown(KeyCode.Space) && jumpAbility.canJump)
			{
				jumpAbility.Jump(ref velocity);
			}
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
			DropCorpse (corpseThrowCount / corpseThrowTime);
			throwingCorpse = false;
			corpseThrowCount = 0;
		}
	}

	void PickupCorpse(GameObject corpse)
	{
		corpseCarried = corpse;
		//corpseCarried.transform.SetParent (this.transform);
		corpseCarried.transform.position = transform.position + new Vector3(0, 1, 0);
		corpseCarried.GetComponent<Rigidbody2D> ().isKinematic = true;
		corpseCarried.transform.rotation = Quaternion.identity;
		Physics2D.IgnoreCollision (controller.coll, corpseCarried.GetComponent<Collider2D>());
	}

	void DropCorpse(float forceModifier)
	{
		//corpseCarried.transform.SetParent (null);
		corpseCarried.GetComponent<Rigidbody2D> ().isKinematic = false;

		Vector2 force = new Vector2 (10, 5) + (Vector2)transform.right;
		force *= forceModifier;
		corpseCarried.GetComponent<Rigidbody2D> ().AddForce (force, ForceMode2D.Impulse);

		Physics2D.IgnoreCollision (controller.coll, corpseCarried.GetComponent<Collider2D>(), false);
		corpseCarried = null;
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
}
