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

	Color baseColor;

	public GameObject corpsePrefab;

	float attackRate;
	float attackTimer;
	float attackDamage;

	[HideInInspector]
	public bool isGrapplingPlayer; //Whether or not this enemy is grappling the player.

	float grappleStrength; //Strength of the grab every time the enemy grabs.
	float grappleModifier; //Modifies the grapple strength based on how many times we've attacked during one grapple.

	[HideInInspector]
	public bool canMove;

	public enum EnemyAIMode
	{
		walkLeftRight,
		walkLeftRightOnPlatform
	}
	EnemyAIMode enemyAIMode = new EnemyAIMode();

	void Start()
	{
		player = GameObject.Find ("Player").GetComponent<Player> ();

		controller = GetComponent<Controller2D> ();
		enemyAIMode = EnemyAIMode.walkLeftRightOnPlatform;
		velocity = Vector3.zero;
		velocity.x = speed;

		baseColor = GetComponent<SpriteRenderer> ().color;

		attackRate = 1.5f;
		attackTimer = attackRate;
		attackDamage = 15f;

		grappleStrength = 5f;
		grappleModifier = 1f;
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
			player.canMove = false; //The player can't move either.
		}
		else
			grappleModifier = 1;

		if (controller.coll.IsTouching (player.controller.coll) || isGrapplingPlayer) 
		{
			if (attackTimer >= attackRate && !GameManager.instance.isPaused) 
			{
				if(player.canBeGrabbed)
				{
					isGrapplingPlayer = true;
					player.grapplingEnemies.Add(this);
					player.grappleStrength += grappleStrength * grappleModifier;
					grappleModifier  *= .75f;
				}

				attackTimer = 0;
				player.health -= attackDamage;
				Camera.main.GetComponent<CameraFollowTrap> ().ScreenShake (.1f, .075f);
			}
		}
		attackTimer += Time.deltaTime;
		#endregion

		if (canMove)
			controller.Move (velocity * Time.deltaTime);
		
		enemyInfo.Reset ();
	}

	/*
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
	*/

	//Getting hit by a bullet.
	public void GetHit(Bullet bullet)
	{
		health -= bullet.damage;
		bullet.ReduceDamage ();
		GameManager.instance.Sleep (bullet.sleepFramesOnHit);
		if (health <= 0)
			Die (bullet);

		Camera.main.GetComponent<CameraFollowTrap> ().ScreenShake (.1f, .075f);
		GameManager.instance.FlashWhite (this.GetComponent<SpriteRenderer>(), 0.018f, baseColor);
	}

	public void KnockBack(Vector3 vel, float duration)
	{
		StartCoroutine (knockBack (vel, duration));
	}

	public IEnumerator knockBack(Vector3 vel, float duration)
	{
		Vector3 startPos = transform.position;
		canMove = false;
		for (float t = 0; t < duration; t += Time.deltaTime) 
		{
			transform.position = new Vector3((Mathf.Lerp(startPos.x, startPos.x + vel.x, t / duration)), startPos.y, startPos.z);
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

	void JumpOverCorpse()
	{
	}
	#endregion

	void Die(Bullet bullet) //The bullet that killed us! DAMN YOU, BULLET!
	{
		GameObject corpse = Instantiate (corpsePrefab, transform.position + new Vector3(0f, .6f, 0f), Quaternion.Euler (new Vector3 (0, 0, -90))) as GameObject;
		corpse.GetComponent<CorpseRagdoll> ().Flip ((int)Mathf.Sign (velocity.x));
		Camera.main.GetComponent<CameraFollowTrap> ().ScreenShake (.1f, .08f);

		Rigidbody2D[] corpseRigidbodies = corpse.GetComponentsInChildren<Rigidbody2D> ();
		//for (int i = 0; i < corpseRigidbodies.Length; i++) 
		//{
			//Debug.Log ("Enemies left = " + GameManager.instance.level.GetComponent<Level> ().enemies.Count);
			//Debug.Log ("Previous enemies left = " + GameManager.instance.level.GetComponent<Level> ().enemiesPrev.Count);
			//corpseRigidbodies[i].isKinematic = false;
			if (GameManager.instance.level.GetComponent<Level> ().enemies.Count > 1) {
				//corpseRigidbodies[i].AddForceAtPosition (new Vector2 (bullet.corpseKnockback, 0f)
					//* GameObject.Find ("Player").GetComponent<Player> ().direction, (Vector2)bullet.transform.position, ForceMode2D.Impulse);
				corpse.GetComponent<CorpseRagdoll>().AddForceAtPosition(new Vector2 (bullet.corpseKnockback, 0f) * player.direction, (Vector2)bullet.transform.position, ForceMode2D.Impulse);
			}
			else
			{
				//corpseRigidbodies[i].AddForceAtPosition (new Vector2 (bullet.corpseKnockback * 2, 0f)
					//* GameObject.Find ("Player").GetComponent<Player> ().direction, (Vector2)bullet.transform.position, ForceMode2D.Impulse);
				corpse.GetComponent<CorpseRagdoll>().AddForceAtPosition(new Vector2 (bullet.corpseKnockback * 1.5f, 0f) * player.direction, (Vector2)bullet.transform.position, ForceMode2D.Impulse);
			}

			//GameManager.instance.FlashWhite (corpseRigidbodies[i].GetComponent<SpriteRenderer> (), 0.018f, baseColor);
		//}

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
