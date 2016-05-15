using UnityEngine;
using System.Collections;

public class EnemyAI : Entity 
{
	public GameObject corpsePrefab;
	Color baseColor;

	Player player;

	//[HideInInspector]
	//public Animator stateMachine;
	[HideInInspector]
	public EnemyInfo enemyInfo;

	[HideInInspector]
	public Controller2D controller;

	[HideInInspector]
	public Vector3 velocity;

	public float gravity;
	public float patrolSpeed;
	public float chaseSpeed;

	//Jumping stuff
	public float jumpHeight;
	public float timeToJumpApex;
	[HideInInspector]
	public bool canJump;
	bool jumpingCurrent;
	bool jumpingPrevious;
	float jumpVelocity;

	//AI stuff
	public LayerMask sightLayerMask; //The layers that the enemy can see.
	public LayerMask hearingLayerMask; //The layers the enemy can hear.
	float chaseTime; //How long after losing sight of the player will the enemy continue to chase.
	float chaseTimer; //Counts up to chaseTime.
	bool playerDetected; //Whether or not we can see/hear the player.

	public enum States
	{
		Patrol,
		ChasePlayer,
		GrabPlayer,
		JumpOverCorpse
	}
	public States currentState;

	void Start()
	{
		player = GameObject.Find ("Player").GetComponent<Player> ();

		//stateMachine = GetComponent<Animator> ();
		controller = GetComponent<Controller2D> ();

		baseColor = GetComponent<SpriteRenderer> ().color;

		currentState = States.Patrol;

		jumpingCurrent = false;
		jumpingPrevious = false;
		CalculateJumpPhysics ();

		chaseTime = 3f;
		playerDetected = false;
	}

	void Update()
	{
		base.Update (); //Update for entity.

		if (controller.collisions.left && !controller.collisions.leftPrev)
			enemyInfo.JustHitWall = true;
		if (controller.collisions.right && !controller.collisions.rightPrev)
			enemyInfo.JustHitWall = true;

		if (!controller.collisions.belowLeft && controller.collisions.belowLeftPrev)
			enemyInfo.IsOnEdgeOfPlatform = true;
		if (!controller.collisions.belowRight && controller.collisions.belowRightPrev)
			enemyInfo.IsOnEdgeOfPlatform = true;

		if (!controller.collisions.below)
			velocity.y += gravity * Time.deltaTime;
		else if (controller.collisions.below) 
		{
			if(velocity.y < 0)
				velocity.y = -.001f;
			jumpingCurrent = false;
		}

		//Debug.Log ("Wall: " + enemyInfo.JustHitWall);
		//Debug.Log ("Platform: " + enemyInfo.IsOnEdgeOfPlatform);

		if (Input.GetKeyDown (KeyCode.L) && !jumpingCurrent)
			Jump (ref velocity);

		switch (currentState) 
		{
		case States.Patrol:
			Patrol ();
			break;
		case States.ChasePlayer:
			ChasePlayer ();
			break;
		}
		ChooseState ();

		if (GetCanHearPlayer () || GetCanSeePlayer ()) 
		{
			playerDetected = true;
		} 
		else
		{
			playerDetected = false;
		}
			
		jumpingPrevious = jumpingCurrent;
		enemyInfo.Reset (); //Temporarily removed so that patrol state would work... This won't work as a permanent solution.
	}

	//This method pushes us away from other enemies so that we don't overlap with them.
	void KeepDistanceFromEnemies()
	{
		GameObject[] enemies = GameManager.instance.level.GetComponent<Level> ().enemies.ToArray ();
		for (int i = 0; i < enemies.Length; i++) 
		{
			if (enemies [i] != null) 
			{
				if (this != enemies [i]) 
				{
					if (controller.coll.IsTouching (enemies [i].GetComponent<EnemyAI> ().controller.coll))
					{
						if (transform.position.x <= enemies [i].transform.position.x) 
						{
							velocity.x -= .1f;
						}
						else if (transform.position.x > enemies [i].transform.position.x) 
						{
							velocity.x += .1f;
						}
					}
				}
			}
		}
	}

	#region Sensing
	//Gets the vision of the enemy, ie: does the enemy see the player?
	bool GetCanSeePlayer()
	{
		bool canSeePlayer = false;

		float raycastDistance = 4f;
		int raycastCount = 5; //How many raycasts to send out.
		float castAngle = 45f; //Angle of the arc to send out.

		RaycastHit2D[] raycastHits = new RaycastHit2D[raycastCount];
		for (int i = 0; i < raycastHits.Length; i++) 
		{
			//Distance between each angle.
			float angleStepAmount = (castAngle * 2f) / ((float)raycastCount - 1);
			//Angle to use for this cast.
			float angle = castAngle - (i * angleStepAmount);
			//The end point of the cast, used for finding the direction.
			Vector3 endPoint = new Vector3 (Mathf.Cos (angle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x), Mathf.Sin (angle * Mathf.Deg2Rad), 0f) * raycastDistance + transform.position;

			raycastHits [i] = Physics2D.Raycast ((Vector2)transform.position, (Vector2)(endPoint - transform.position), raycastDistance, sightLayerMask);

			if (raycastHits[i].transform != null) 
			{
				if (raycastHits [i].transform.tag == "Player") 
				{
					canSeePlayer = true;
					Debug.DrawLine (transform.position, endPoint, Color.blue);
				}
				else 
				{
					Debug.DrawLine (transform.position, endPoint, Color.grey);
				}
			} 
			else
			{
				Debug.DrawLine (transform.position, endPoint, Color.grey);
			}
		}

		return canSeePlayer;
	}

	//Returns whether we can hear the player or not using a cast in all directions around us.
	bool GetCanHearPlayer()
	{
		bool canHearPlayer = false;
		float hearingRadius = 1.5f;
		int raycastCount = 8;

		RaycastHit2D[] raycastHits = new RaycastHit2D[raycastCount];
		for (int i = 0; i < raycastHits.Length; i++) 
		{
			float angleStepAmount = 360f / (raycastCount - 1);
			float angle = angleStepAmount * i;

			Vector3 endPoint = new Vector3 (Mathf.Cos (angle * Mathf.Deg2Rad), Mathf.Sin (angle * Mathf.Deg2Rad), 0f) * hearingRadius + transform.position;

			raycastHits [i] = Physics2D.Raycast ((Vector2)transform.position, (Vector2)(endPoint - transform.position), hearingRadius, hearingLayerMask);

			if (raycastHits [i].transform != null) 
			{
				if (raycastHits [i].transform.tag == "Player") 
				{
					canHearPlayer = true;
					Debug.DrawLine (transform.position, endPoint, Color.blue);
				} 
				else 
				{
					Debug.DrawLine (transform.position, endPoint, Color.grey);
				}
			}
			else
			{
				Debug.DrawLine (transform.position, endPoint, Color.grey);
			}
		}

		return canHearPlayer;
	}
	#endregion

	//This method picks a state to use, contextually.
	void ChooseState()
	{
		if (playerDetected) 
		{
			chaseTimer = 0f;
			currentState = States.ChasePlayer;
		} 
		else if(chaseTimer >= chaseTime)
		{
			currentState = States.Patrol;
		}

		//If we can't see the player and we haven't run out of time to chase...
		if (!playerDetected && chaseTimer < chaseTime) 
		{
			chaseTimer += Time.deltaTime;
		}
	}

	#region States
	void Patrol()
	{
		velocity.x = patrolSpeed;
		KeepDistanceFromEnemies ();

		if (enemyInfo.IsOnEdgeOfPlatform || enemyInfo.JustHitWall) 
		{
			velocity.x *= -1f;
		}

		controller.Move (velocity * Time.deltaTime);
	}

	void ChasePlayer()
	{
		velocity.x = chaseSpeed;
		KeepDistanceFromEnemies ();

		if (player.transform.position.x > transform.position.x) 
		{
			if (Mathf.Sign (velocity.x) == -1)
				velocity.x *= -1;
		} 
		else if (player.transform.position.x < transform.position.x) 
		{
			if (Mathf.Sign (velocity.x) == 1)
				velocity.x *= -1;
		}

		controller.Move (velocity * Time.deltaTime);
	}

	void GrabPlayer()
	{
	}

	void JumpOverCorpse()
	{
	}
	#endregion

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Bullet") 
		{
			health -= other.gameObject.GetComponent<Bullet> ().damage;
			other.gameObject.GetComponent<Bullet>().ReduceDamage ();
			GameManager.instance.Sleep (other.gameObject.GetComponent<Bullet>().sleepFramesOnHit);
			if (health <= 0)
				Die (other.gameObject.GetComponent<Bullet> ());

			GameManager.instance.FlashWhite (this.GetComponent<SpriteRenderer>(), 0.018f, baseColor);
		}
	}

	public void Jump(ref Vector3 velocity)
	{
		jumpingCurrent = true;

		velocity.y += jumpVelocity;
	}

	void Die(Bullet bullet) //The bullet that killed us! DAMN YOU, BULLET!
	{
		GameObject corpse = Instantiate (corpsePrefab, transform.position, Quaternion.Euler (new Vector3 (0, 0, 90))) as GameObject;

		Camera.main.GetComponent<CameraFollowTrap> ().ScreenShake (.1f, .08f);

		for (int i = 0; i < corpse.transform.childCount; i++) 
		{
			//Debug.Log ("Enemies left = " + GameManager.instance.level.GetComponent<Level> ().enemies.Count);
			//Debug.Log ("Previous enemies left = " + GameManager.instance.level.GetComponent<Level> ().enemiesPrev.Count);
			corpse.transform.GetChild (i).GetComponent<Rigidbody2D> ().isKinematic = false;
			if (GameManager.instance.level.GetComponent<Level> ().enemies.Count > 1) 
			{
				corpse.transform.GetChild (i).GetComponent<Rigidbody2D> ().AddForceAtPosition (new Vector2 (bullet.corpseKnockback, 0f)
					* GameObject.Find ("Player").GetComponent<Player> ().direction, (Vector2)bullet.transform.position, ForceMode2D.Impulse);
			}
			else
			{
				corpse.transform.GetChild (i).GetComponent<Rigidbody2D> ().AddForceAtPosition (new Vector2 (bullet.corpseKnockback * 2, 0f)
					* GameObject.Find ("Player").GetComponent<Player> ().direction, (Vector2)bullet.transform.position, ForceMode2D.Impulse);
			}

			GameManager.instance.FlashWhite (corpse.transform.GetChild (i).GetComponent<SpriteRenderer> (), 0.018f, baseColor);
		}

		Physics2D.IgnoreCollision (controller.coll, bullet.GetComponent<Collider2D> ());
		Destroy (gameObject);
	}

	//Calculates gravity and jump velocity for a desired jumpHeight and timeToJumpApex.
	public void CalculateJumpPhysics()
	{
		//Based on REAL physics!! #FeaturePorn
		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs (gravity) * timeToJumpApex;

		//print ("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);
	}

	public struct EnemyInfo
	{
		public bool JustHitWall;
		public bool IsOnEdgeOfPlatform;

		//Resets info.
		public void Reset()
		{
			JustHitWall = false;
			IsOnEdgeOfPlatform = false;
		}
	}
}
