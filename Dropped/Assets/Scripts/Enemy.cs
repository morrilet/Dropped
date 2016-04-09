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

	public enum EnemyAIMode
	{
		walkLeftRight,
		walkLeftRightOnPlatform
	}
	EnemyAIMode enemyAIMode = new EnemyAIMode();

	public override void Start()
	{
		base.Start ();

		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();

		controller = GetComponent<Controller2D> ();
		enemyAIMode = EnemyAIMode.walkLeftRightOnPlatform;
		velocity = Vector3.zero;
		velocity.x = speed;

		attackRate = 1.5f;
		attackTimer = attackRate;
		attackDamage = 15f;
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

		if (controller.coll.IsTouching (player.controller.coll)) 
		{
			if (attackTimer >= attackRate) 
			{
				attackTimer = 0;
				player.health -= attackDamage;
				Camera.main.GetComponent<CameraFollowTrap> ().ScreenShake (.075f, .075f);
			}
		}
		attackTimer += Time.deltaTime;

		controller.Move (velocity * Time.deltaTime);

		enemyInfo.Reset ();
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Bullet") 
		{
			health -= other.gameObject.GetComponent<Bullet> ().damage;
			other.gameObject.GetComponent<Bullet>().ReduceDamage ();
			if (health <= 0)
				Die (other.gameObject.GetComponent<Bullet> ());
		}
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
