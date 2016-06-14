using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	bool jumpingCurrent;
	bool jumpingPrevious;
	float jumpVelocity;

	//AI stuff
	public LayerMask sightLayerMask; //The layers that the enemy can see.
	public LayerMask hearingLayerMask; //The layers the enemy can hear.
	float chaseTime; //How long after losing sight of the player will the enemy continue to chase.
	float chaseTimer; //Counts up to chaseTime.
	bool playerDetected; //Whether or not we can see/hear the player.

	public int direction;
	int storedDirection; //This is the value used to return the enemy to its previous direction when it stops chasing. -1 = left, 1 = right.

	[HideInInspector]
	public bool isGrapplingPlayer; //Whether or not this enemy is grappling the player.
	float grappleStrength; //Strength of the grab every time the enemy grabs.
	float grappleModifier; //Modifies the grapple strength based on how many times we've attacked during one grapple.
	float maxGrappleRange; //Lets the player go if they're farther away than this value.

	float attackRate;
	float attackTimer;
	float attackDamage;

	Vector2 jumpTimeRange; //Range of values for jumpTime. x,y -- min,max;
	float jumpTime; //How long to wait after jumping before jumping again. Stops enemies from constantly bouncing on corpses.
	float jumpTimer;
	[HideInInspector]
	public bool jumpTriggered; //If the enemy is in a jump trigger that has had its conditions met.

	public enum States
	{
		Patrol,
		ChasePlayer,
		GrabPlayer,
		JumpOverCorpse
	}
	public States currentState;
	States previousState;

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

		attackRate = 1.5f;
		attackTimer = attackRate;
		attackDamage = 15f;

		grappleStrength = 5f;
		grappleModifier = 1f;
		maxGrappleRange = 1f;

		jumpTimeRange = new Vector2 (.5f, 2f);
		jumpTime = Random.Range (jumpTimeRange.x, jumpTimeRange.y);
		jumpTimer = jumpTime;

		velocity.x = patrolSpeed;
		storedDirection = (int)Mathf.Sign(velocity.x);
	}

	public override void Update()
	{
		base.Update (); //Update for entity.

		if (controller.collisions.left && !controller.collisions.leftPrev)
			enemyInfo.JustHitWall = true;
		if (controller.collisions.right && !controller.collisions.rightPrev)
			enemyInfo.JustHitWall = true;

		if (!controller.collisions.belowLeft && controller.collisions.belowLeftPrev) {
			enemyInfo.IsOnEdgeOfPlatform = true;
		}
		if (!controller.collisions.belowRight && controller.collisions.belowRightPrev) {
			enemyInfo.IsOnEdgeOfPlatform = true;
		}

		if (!controller.collisions.below) 
		{
			velocity.y += gravity * Time.deltaTime;
		}
		else
		{
			if(velocity.y < 0)
				velocity.y = -.001f;
			jumpingCurrent = false;
		}

		//Debug.Log ("Wall: " + enemyInfo.JustHitWall);
		//Debug.Log ("Platform: " + enemyInfo.IsOnEdgeOfPlatform);

		//if (Input.GetKeyDown (KeyCode.L) && !jumpingCurrent)
			//Jump (ref velocity);

		switch (currentState) 
		{
		case States.Patrol:
			Patrol ();
			break;
		case States.ChasePlayer:
			ChasePlayer ();
			break;
		case States.GrabPlayer:
			GrabPlayer ();
			break;
		case States.JumpOverCorpse:
			JumpOverCorpse ();
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

		if ((int)Mathf.Sign (velocity.x) != direction)
			velocity.x *= -1f;

		//Debug.Log (currentState + ", " + isGrapplingPlayer);

		if (currentState != States.GrabPlayer && previousState == States.GrabPlayer)
			isGrapplingPlayer = false;

		if(attackTimer <= attackRate)
			attackTimer += Time.deltaTime;

		if (jumpTimer <= jumpTime)
			jumpTimer += Time.deltaTime;

		if (health <= 0) 
		{
			Die (null);
		}

		//Debug.Log (currentState.ToString () + ", " + previousState.ToString ());

		//Reset the grapple modifier when we first grab the player.
		if (currentState == States.GrabPlayer && previousState != States.GrabPlayer) 
		{
			grappleModifier = 1f;
		}

		jumpingPrevious = jumpingCurrent;
		enemyInfo.Reset ();
		//if(currentState == previousState)
		previousState = currentState;
	}

	#region Flocking
	//This method pushes us away from other enemies so that we don't overlap with them.
	void KeepDistanceFromEnemies()
	{
		GameObject[] enemies = GetIsTouchingEnemies ();
		for (int i = 0; i < enemies.Length; i++) 
		{
			if (enemies [i] != null) 
			{
				if (this != enemies [i]) 
				{
					if (direction == enemies [i].GetComponent<EnemyAI> ().direction)
					{
						if (Mathf.Abs(transform.position.x - enemies [i].transform.position.x) >= 0.01f)
							velocity.x += .1f * Mathf.Sign (transform.position.x - enemies [i].transform.position.x);
						else 
						{
							//Debug.Log ("Here");
							velocity.x += .1f * Random.Range (-1f, 1f);
						}
						//Debug.Log (Mathf.Sign(transform.position.x - enemies [i].transform.position.x));
					}
				}
			}
		}
	}
	#endregion

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
			Vector3 endPoint = new Vector3 (Mathf.Cos (angle * Mathf.Deg2Rad) * direction, Mathf.Sin (angle * Mathf.Deg2Rad), 0f) * raycastDistance + transform.position;

			raycastHits [i] = Physics2D.Raycast ((Vector2)transform.position, (Vector2)(endPoint - transform.position), raycastDistance, sightLayerMask);

			if (raycastHits[i].transform != null) 
			{
				if (raycastHits [i].transform.tag == "Player") 
				{
					canSeePlayer = true;
					Debug.DrawLine (transform.position, raycastHits[i].point, Color.blue);
				}
				else 
				{
					Debug.DrawLine (transform.position, raycastHits[i].point, Color.grey);
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

	GameObject[] GetIsTouchingEnemies()
	{
		List<GameObject> touchingEnemies = new List<GameObject> ();

		GameObject[] enemies = GameManager.instance.level.GetComponent<Level> ().enemies.ToArray ();
		for (int i = 0; i < enemies.Length; i++) 
		{
			float colliderWidth = GetComponent<Collider2D> ().bounds.extents.x * 2;
			float colliderHeight = GetComponent<Collider2D> ().bounds.extents.y * 2;
			if (enemies [i] != null && enemies[i] != this.gameObject) 
			{
				if (this != enemies [i]) 
				{
					if (Mathf.Abs (transform.position.x - enemies [i].transform.position.x) < colliderWidth
						&& Mathf.Abs (transform.position.y - enemies [i].transform.position.y) < colliderHeight) 
					{
						//Debug.Log("TOUCHING");
						touchingEnemies.Add (enemies [i]);
					}
				}
			}
		}

		return touchingEnemies.ToArray ();
	}

	bool GetIsTouchingPlayer()
	{
		bool isTouchingPlayer = false;

		float minDistanceX = GetComponent<Collider2D> ().bounds.extents.x + player.GetComponent<Collider2D> ().bounds.extents.x;
		float minDistanceY = GetComponent<Collider2D> ().bounds.extents.y + player.GetComponent<Collider2D> ().bounds.extents.y;
		if (Mathf.Abs (transform.position.x - player.transform.position.x) < minDistanceX
		   && Mathf.Abs (transform.position.y - player.transform.position.y) < minDistanceY) 
		{
			isTouchingPlayer = true;
		}

		return isTouchingPlayer;
	}

	bool GetIsTouchingCorpse()
	{
		bool isTouchingCorpse = false;

		GameObject[] ragdolls = GameObject.FindGameObjectsWithTag ("Ragdoll");
		if (ragdolls.Length > 0) 
		{
			for (int i = 0; i < ragdolls.Length; i++) 
			{
				GameObject upperTorso = ragdolls [i].GetComponent<CorpseRagdoll> ().upperTorso;
				GameObject lowerTorso = ragdolls [i].GetComponent<CorpseRagdoll> ().lowerTorso;
				GameObject[] limbs = ragdolls [i].GetComponent<CorpseRagdoll> ().limbs.ToArray ();

				//This distance check is for optimization.
				if (Vector3.Distance (upperTorso.transform.position, transform.position) < 2f) 
				{
					//Because these can rotate, this all won't be SUPER accurate. Hopefully that won't be too bad.

					float upperTorsoMinDistanceX = GetComponent<Collider2D> ().bounds.extents.x + upperTorso.GetComponent<Collider2D> ().bounds.extents.x;
					float upperTorsoMinDistanceY = GetComponent<Collider2D> ().bounds.extents.y + upperTorso.GetComponent<Collider2D> ().bounds.extents.y;
					if (Mathf.Abs (transform.position.x - upperTorso.transform.position.x) < upperTorsoMinDistanceX
						&& Mathf.Abs (transform.position.y - upperTorso.transform.position.y) < upperTorsoMinDistanceY) 
					{
						isTouchingCorpse = true;
					}

					float lowerTorsoMinDistanceX = GetComponent<Collider2D> ().bounds.extents.x + lowerTorso.GetComponent<Collider2D> ().bounds.extents.x;
					float lowerTorsoMinDistanceY = GetComponent<Collider2D> ().bounds.extents.y + lowerTorso.GetComponent<Collider2D> ().bounds.extents.y;
					if (Mathf.Abs (transform.position.x - lowerTorso.transform.position.x) < lowerTorsoMinDistanceX
						&& Mathf.Abs (transform.position.y - lowerTorso.transform.position.y) < lowerTorsoMinDistanceY) 
					{
						isTouchingCorpse = true;
					}
						
					for (int j = 0; j < limbs.Length; j++) 
					{
						float minDistanceX = GetComponent<Collider2D> ().bounds.extents.x + limbs [j].GetComponent<Collider2D> ().bounds.extents.x;
						float minDistanceY = GetComponent<Collider2D> ().bounds.extents.y + limbs [j].GetComponent<Collider2D> ().bounds.extents.y;
						if (Mathf.Abs (transform.position.x - limbs [i].transform.position.x) < minDistanceX
						   && Mathf.Abs (transform.position.y - limbs [i].transform.position.y) < minDistanceY) 
						{
							isTouchingCorpse = true;
						}
					}
				}
			}
		}

		return isTouchingCorpse;
	}
	#endregion

	#region States
	//This method picks a state to use, contextually.
	void ChooseState()
	{
		if (playerDetected && !isGrapplingPlayer) 
		{
			chaseTimer = 0f;
			currentState = States.ChasePlayer;
		} 
		else if(chaseTimer >= chaseTime && !isGrapplingPlayer)
		{
			currentState = States.Patrol;
		}

		//If we can't see the player and we haven't run out of time to chase...
		if (!playerDetected && chaseTimer < chaseTime) 
		{
			chaseTimer += Time.deltaTime;
		}

		if (GetIsTouchingPlayer () && player.canBeGrabbed && Vector3.Distance(transform.position, player.transform.position) <= maxGrappleRange) 
		{
			currentState = States.GrabPlayer;

			if (player.ladder)
				player.ladder = null;
		}

		if (GetIsTouchingCorpse () || jumpTriggered) 
		{
			//Debug.Log ("Here");
			if (!jumpingCurrent && jumpTimer > jumpTime && controller.collisions.below) 
			{
				jumpTime = Random.Range (jumpTimeRange.x, jumpTimeRange.y);
				jumpTimer = 0f;
				Jump (ref velocity);
			}
			//currentState = States.JumpOverCorpse;
		}
	}

	void Patrol()
	{
		//If we just switched to patrol reset direction to stored direction.
		if (previousState != States.Patrol) 
		{
			if (direction != storedDirection)
				direction *= -1;
		} 
		else if (controller.collisions.left) //If we just switched to patrol and are touching an obstacle, turn around.
		{
			if (direction == -1)
				direction = 1;
		}
		else if (controller.collisions.right) //If we just switched to patrol and are touching an obstacle, turn around.
		{
			if (direction == 1)
				direction = -1;
		}

		if (Mathf.Abs (velocity.x) != patrolSpeed)
			velocity.x = patrolSpeed;

		if (enemyInfo.IsOnEdgeOfPlatform || enemyInfo.JustHitWall) 
		{
			if(!jumpingCurrent)
				direction *= -1;
		}

		KeepDistanceFromEnemies ();
		//velocity.x *= direction;
		controller.Move (velocity * Time.deltaTime);
	}

	void ChasePlayer()
	{
		storedDirection = direction;

		if (Mathf.Abs (velocity.x) != chaseSpeed)
			velocity.x = chaseSpeed * direction;

		direction = (int)Mathf.Sign (player.transform.position.x - transform.position.x);

		/*
		if (player.transform.position.x > transform.position.x) 
		{
			direction = 1;
		} 
		else if (player.transform.position.x < transform.position.x) 
		{
			direction = -1;
		}
		*/

		KeepDistanceFromEnemies ();
		//velocity.x *= direction;
		controller.Move (velocity * Time.deltaTime);
	}

	void GrabPlayer()
	{
		//if (previousState != States.GrabPlayer && currentState == States.GrabPlayer) 
		//{
			//Debug.Log ("Here 2");
			//grappleModifier = 1f;
		//}
		if (attackTimer >= attackRate && !GameManager.instance.isPaused) 
		{
			player.direction = Mathf.Sign (transform.position.x - player.transform.position.x); //Make the player face us.
			player.canMove = false; //Stop the player from moving.

			isGrapplingPlayer = true;
			if(!player.grapplingEnemies.Contains(this))
				player.grapplingEnemies.Add (this);

			player.grappleStrength += grappleStrength * grappleModifier;
			grappleModifier *= .65f; //Here is where we decide how strong the next successful attack will be.
			attackTimer = 0;
			player.health -= attackDamage;
			Camera.main.GetComponent<CameraFollowTrap> ().ScreenShake (.1f, .075f);
		}
			
		//Move, but only with the force of gravity.
		velocity.y += gravity * Time.deltaTime;
		velocity.x = 0f;
		KeepDistanceFromEnemies ();
		controller.Move (new Vector3 (velocity.x, velocity.y, 0f) * Time.deltaTime);
	}

	void JumpOverCorpse()
	{
		//Get what state we were in last time.
		//States storedState = previousState;

		//Try to jump.
		if(!jumpingCurrent)
			Jump (ref velocity);

		//Set state to storedState.
		//if (storedState != States.JumpOverCorpse)
			//currentState = storedState;
		//else
			//currentState = States.Patrol;
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

			chaseTimer = 0f;
			currentState = States.ChasePlayer;
		}
	}

	#region AdvancedMovement
	public void Jump(ref Vector3 velocity)
	{
		jumpingCurrent = true;

		velocity.y = jumpVelocity;
	}
		
	//Calculates gravity and jump velocity for a desired jumpHeight and timeToJumpApex.
	public void CalculateJumpPhysics()
	{
		//Based on REAL physics!! #FeaturePorn
		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs (gravity) * timeToJumpApex;

		//print ("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);
	}

	public void KnockBack(Vector3 vel, float duration)
	{
		//Check for possible collisions.
		int raycastCount = 4;
		for (int i = 0; i < raycastCount; i++) 
		{
			Vector2 startPoint = (Vector2)transform.position;
			startPoint.y += ((GetComponent<Collider2D> ().bounds.extents.y * 2f) / (float)raycastCount) * i;

			Vector2 endPoint = new Vector2(vel.x - transform.position.x, 0f) * Mathf.Sign(vel.x);
			endPoint.y += ((GetComponent<Collider2D> ().bounds.extents.y * 2f) / (float)raycastCount) * i;

			RaycastHit2D hit = Physics2D.Raycast (startPoint, endPoint, vel.magnitude, controller.collisionMask);
			if (hit) 
			{
				float dir = Mathf.Sign (vel.x);

				//Might need a check here to stop us from setting vel if it's bigger than the last vel we set.
				vel.x = (Mathf.Abs (hit.point.x - transform.position.x) * dir) + (controller.coll.bounds.extents.x * -dir);
				//Debug.Log (hit.transform.name + ", " + hit.point + ", " + vel);
			}
		}

		StartCoroutine (knockBack (vel, duration));
	}

	public IEnumerator knockBack(Vector3 vel, float duration)
	{
		Vector3 startPos = transform.position;
		//canMove = false;

		for (float t = 0; t < duration; t += Time.deltaTime)
		{
			transform.position = new Vector3((Mathf.Lerp(startPos.x, startPos.x + vel.x, t / duration)), startPos.y, startPos.z);
			yield return null;
		}
		//canMove = true;
	}
	#endregion

	void Die(Bullet bullet) //The bullet that killed us! DAMN YOU, BULLET!
	{
		GameObject corpse = Instantiate (corpsePrefab, transform.position + new Vector3(0f, .6f, 0f), Quaternion.Euler (new Vector3 (0, 0, -90))) as GameObject;
		corpse.GetComponent<CorpseRagdoll> ().direction = direction;
		//corpse.GetComponent<CorpseRagdoll> ().Flip ((int)Mathf.Sign (velocity.x));
		Camera.main.GetComponent<CameraFollowTrap> ().ScreenShake (.1f, .08f);

		Rigidbody2D[] corpseRigidbodies = corpse.GetComponentsInChildren<Rigidbody2D> ();
		//for (int i = 0; i < corpseRigidbodies.Length; i++) 
		//{
		//Debug.Log ("Enemies left = " + GameManager.instance.level.GetComponent<Level> ().enemies.Count);
		//Debug.Log ("Previous enemies left = " + GameManager.instance.level.GetComponent<Level> ().enemiesPrev.Count);
		//corpseRigidbodies[i].isKinematic = false;
		if (bullet != null) 
		{
			if (GameManager.instance.level.GetComponent<Level> ().enemies.Count > 1) 
			{
				//corpseRigidbodies[i].AddForceAtPosition (new Vector2 (bullet.corpseKnockback, 0f)
				//* GameObject.Find ("Player").GetComponent<Player> ().direction, (Vector2)bullet.transform.position, ForceMode2D.Impulse);
				corpse.GetComponent<CorpseRagdoll> ().AddForceAtPosition (new Vector2 (bullet.corpseKnockback, 0f) * player.direction, (Vector2)bullet.transform.position, ForceMode2D.Impulse);
			} 
			else 
			{
				//corpseRigidbodies[i].AddForceAtPosition (new Vector2 (bullet.corpseKnockback * 2, 0f)
				//* GameObject.Find ("Player").GetComponent<Player> ().direction, (Vector2)bullet.transform.position, ForceMode2D.Impulse);
				corpse.GetComponent<CorpseRagdoll> ().AddForceAtPosition (new Vector2 (bullet.corpseKnockback * 1.5f, 0f) * player.direction, (Vector2)bullet.transform.position, ForceMode2D.Impulse);
			}

			Physics2D.IgnoreCollision (controller.coll, bullet.GetComponent<Collider2D> ());
		}
		else 
		{
			corpse.GetComponent<CorpseRagdoll> ().AddForce (velocity, ForceMode2D.Impulse);
		}

		//GameManager.instance.FlashWhite (corpseRigidbodies[i].GetComponent<SpriteRenderer> (), 0.018f, baseColor);
		//}

		Destroy (gameObject);
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
