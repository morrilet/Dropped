﻿using UnityEngine;
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

	public enum EnemyAIMode
	{
		walkLeftRight
	}
	EnemyAIMode enemyAIMode = new EnemyAIMode();

	public override void Start()
	{
		base.Start ();

		controller = GetComponent<Controller2D> ();
		enemyAIMode = EnemyAIMode.walkLeftRight;
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

		velocity.y += gravity * Time.deltaTime;

		switch (enemyAIMode) 
		{
		case EnemyAIMode.walkLeftRight:
			WalkLeftRight ();
				break;
		}

		controller.Move (velocity * Time.deltaTime);

		enemyInfo.Reset ();
	}

	//Walks to the side until it hits a wall and then turns around and walks more.
	void WalkLeftRight()
	{
		
		if (enemyInfo.JustHitWall)
			velocity.x *= -1f;
	}

	public struct EnemyInfo
	{
		public bool JustHitWall;

		//Resets info.
		public void Reset()
		{
			JustHitWall = false;
		}
	}
}
