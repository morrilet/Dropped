using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class Enemy : Entity 
{
	public float speed;
	public float gravity;

	Vector3 velocity;
	Controller2D controller;

	[HideInInspector]
	public EnemyInfo enemyInfo;

	public GameObject corpsePrefab;

	public enum EnemyAIMode
	{
		walkLeftRight,
		walkLeftRightOnPlatform
	}
	EnemyAIMode enemyAIMode = new EnemyAIMode();

	public override void Start()
	{
		base.Start ();

		controller = GetComponent<Controller2D> ();
		enemyAIMode = EnemyAIMode.walkLeftRightOnPlatform;
		velocity = Vector3.zero;
		velocity.x = speed;
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

		if (!isAlive)
			Die ();

		controller.Move (velocity * Time.deltaTime);

		enemyInfo.Reset ();
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Bullet") 
		{
			health -= other.gameObject.GetComponent<Bullet> ().damage;
			Destroy (other.gameObject);
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

	void Die()
	{
		GameObject corpse = Instantiate (corpsePrefab, transform.position, Quaternion.Euler (new Vector3 (0, 0, 90))) as GameObject;
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
