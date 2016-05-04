using UnityEngine;
using System.Collections;

public class EnemyAI : Entity 
{
	public GameObject corpsePrefab;
	Color baseColor;

	//[HideInInspector]
	//public Animator stateMachine;
	[HideInInspector]
	public EnemyInfo enemyInfo;

	[HideInInspector]
	public Controller2D controller;

	[HideInInspector]
	public Vector3 velocity;

	public float gravity;
	public float speed;

	//Jumping stuff
	public float jumpHeight;
	public float timeToJumpApex;
	[HideInInspector]
	public bool canJump;
	bool jumpingCurrent;
	bool jumpingPrevious;
	float jumpVelocity;

	public enum States
	{
		Patrol
	}
	public States currentState;

	void Start()
	{
		//stateMachine = GetComponent<Animator> ();
		controller = GetComponent<Controller2D> ();

		baseColor = GetComponent<SpriteRenderer> ().color;

		currentState = States.Patrol;

		jumpingCurrent = false;
		jumpingPrevious = false;
		CalculateJumpPhysics ();

		velocity.x = speed;
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
			velocity.y = 0;
			jumpingCurrent = false;
		}

		//Debug.Log ("Wall: " + enemyInfo.JustHitWall);
		Debug.Log ("Platform: " + enemyInfo.IsOnEdgeOfPlatform);

		if (Input.GetKeyDown (KeyCode.L) && !jumpingCurrent)
			Jump (ref velocity);

		switch (currentState) 
		{
		case States.Patrol:
			Patrol ();
			break;
		}

		jumpingPrevious = jumpingCurrent;
		enemyInfo.Reset (); //Temporarily removed so that patrol state would work... This won't work as a permanent solution.
	}

	void Patrol()
	{
		if (enemyInfo.IsOnEdgeOfPlatform || enemyInfo.JustHitWall) 
		{
			velocity.x *= -1f;
		}

		controller.Move (velocity * Time.deltaTime);
	}

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
			if (GameManager.instance.level.GetComponent<Level> ().enemies.Count > 1) {
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
