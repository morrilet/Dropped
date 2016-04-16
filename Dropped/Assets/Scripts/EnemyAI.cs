using UnityEngine;
using System.Collections;

public class EnemyAI : Entity 
{
	[HideInInspector]
	public Animator stateMachine;
	[HideInInspector]
	public EnemyInfo enemyInfo;

	[HideInInspector]
	public Controller2D controller;

	[HideInInspector]
	public Vector3 velocity;

	public float gravity;
	public float speed;

	void Start()
	{
		stateMachine = GetComponent<Animator> ();
		controller = GetComponent<Controller2D> ();
	}

	void Update()
	{
		base.Update ();

		/*
		if (controller.collisions.left && !controller.collisions.leftPrev)
			enemyInfo.JustHitWall = true;
		if (controller.collisions.right && !controller.collisions.rightPrev)
			enemyInfo.JustHitWall = true;

		if (!controller.collisions.belowLeft && controller.collisions.belowLeftPrev)
			enemyInfo.IsOnEdgeOfPlatform = true;
		if (!controller.collisions.belowRight && controller.collisions.belowRightPrev)
			enemyInfo.IsOnEdgeOfPlatform = true;
		*/

		if (!controller.collisions.below)
			velocity.y += gravity * Time.deltaTime;

		//enemyInfo.Reset ();
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
