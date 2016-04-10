using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class Enemy : Entity 
{
	Player player;

	public float speed;
	public float gravity;

	Vector3 velocity;
	Controller2D controller;

	[HideInInspector]
	public EnemyInfo enemyInfo;

	public GameObject corpsePrefab;

	float attackRate;
	float attackTimer;
	float attackDamage;

	bool isGrapplingPlayer; //Whether or not this enemy is grappling the player.
	float grappleStrength; //How much the player has to slam the button to escape the grapple.
	float grappleEscapeAttempt; //How much the player has slammed the button.

	bool canMove;

	public enum EnemyAIMode
	{
		walkLeftRight,
		walkLeftRightOnPlatform
	}
	EnemyAIMode enemyAIMode = new EnemyAIMode();

	void Start()
	{
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();

		controller = GetComponent<Controller2D> ();
		enemyAIMode = EnemyAIMode.walkLeftRightOnPlatform;
		velocity = Vector3.zero;
		velocity.x = speed;

		attackRate = 1.5f;
		attackTimer = attackRate;
		attackDamage = 15f;

		grappleStrength = 5f;
		grappleEscapeAttempt = 0;
		isGrapplingPlayer = false;

		canMove = true;
	}

	public override void Update()
	{
		base.Update ();

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

		switch (enemyAIMode) 
		{
		case EnemyAIMode.walkLeftRight:
			WalkLeftRight ();
			break;
		case EnemyAIMode.walkLeftRightOnPlatform:
			WalkLeftRightOnPlatform ();
			break;
		}

		#region Attacking&Grappling
		if(isGrapplingPlayer) //If we've got the player grappled...
		{
			canMove = false; //Don't move any more.
			player.direction = Mathf.Sign(transform.position.x - player.transform.position.x); //Make the player face the right way.
			player.isGrappled = true;
			player.canMove = false; //The player can't move either.

			if(grappleEscapeAttempt >= grappleStrength) //Player escaped the grapple!
			{
				isGrapplingPlayer = false;
				player.isGrappled = false;
				player.canMove = true;

				Vector3 knockBackVelocity = new Vector3(1f, 0f, 0f);
				knockBackVelocity.x *= (transform.position.x - player.transform.position.x) / Mathf.Abs(player.transform.position.x - transform.position.x);
				StartCoroutine(knockBack(knockBackVelocity, .15f));
			}
		}
			
		if(!isGrapplingPlayer || grappleEscapeAttempt < 0)
			grappleEscapeAttempt = 0;

		if(player.isGrappled)
		{
			if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
			grappleEscapeAttempt++;
		}

		if (controller.coll.IsTouching (player.controller.coll) || isGrapplingPlayer) 
		{
			if (attackTimer >= attackRate) 
			{
				if (!player.isGrappled) 
				{
					isGrapplingPlayer = true;
				}

				attackTimer = 0;
				player.health -= attackDamage;
				Camera.main.GetComponent<CameraFollowTrap> ().ScreenShake (.075f, .075f);
			}
		}
		attackTimer += Time.deltaTime;
		grappleEscapeAttempt -= Time.deltaTime;
		#endregion

		if (canMove)
			controller.Move (velocity * Time.deltaTime);

		enemyInfo.Reset ();
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Bullet") 
		{
			health -= other.gameObject.GetComponent<Bullet> ().damage;
			other.gameObject.GetComponent<Bullet>().ReduceDamage ();
			GameManager.instance.Sleep (4);
			if (health <= 0)
				Die (other.gameObject.GetComponent<Bullet> ());
		}
	}

	IEnumerator knockBack(Vector3 vel, float duration)
	{
		Vector3 startPos = transform.position;
		canMove = false;
		for (float t = 0; t < duration; t += Time.deltaTime) 
		{
			transform.position = (Vector3.Lerp(startPos, startPos + vel, t / duration));
			yield return null;
		}
		canMove = true;
	}

	#region AIModes
	//Walks to the side until it hits a wall and then turns around and walks more.
	void WalkLeftRight()
	{
		if (enemyInfo.JustHitWall)
			velocity.x *= -1f;
	}

	void WalkLeftRightOnPlatform()
	{
		if (enemyInfo.IsOnEdgeOfPlatform || enemyInfo.JustHitWall)
			velocity.x *= -1f;
	}
	#endregion

	void Die(Bullet bullet) //The bullet that killed us! DAMN YOU, BULLET!
	{
		GameObject corpse = Instantiate (corpsePrefab, transform.position, Quaternion.Euler (new Vector3 (0, 0, 90))) as GameObject;

		Camera.main.GetComponent<CameraFollowTrap> ().ScreenShake (.1f, .075f);

		corpse.gameObject.GetComponent<Rigidbody2D> ().AddForceAtPosition (new Vector2(bullet.corpseKnockback, 0f) 
			* GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().direction, (Vector2)bullet.transform.position, ForceMode2D.Impulse);
		Physics2D.IgnoreCollision (controller.coll, bullet.GetComponent<Collider2D> ());
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
