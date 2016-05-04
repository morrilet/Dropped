using UnityEngine;
using System.Collections;

public class EnemyPatrolState : StateMachineBehaviour 
{
	EnemyAI enemyAI;
	EnemyPatrolInfo patrolInfo;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		enemyAI = animator.gameObject.GetComponent<EnemyAI> ();

		enemyAI.velocity.x = enemyAI.speed;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		if (enemyAI.controller.collisions.left && !enemyAI.controller.collisions.leftPrev)
			patrolInfo.JustHitWall = true;
		if (enemyAI.controller.collisions.right && !enemyAI.controller.collisions.rightPrev)
			patrolInfo.JustHitWall = true;

		if (!enemyAI.controller.collisions.belowLeft && enemyAI.controller.collisions.belowLeftPrev)
			patrolInfo.IsOnEdgeOfPlatform = true;
		if (!enemyAI.controller.collisions.belowRight && enemyAI.controller.collisions.belowRightPrev)
			patrolInfo.IsOnEdgeOfPlatform = true;

		//if(enemyAI.enemyInfo.IsOnEdgeOfPlatform)
			//Debug.Log (enemyAI.enemyInfo.IsOnEdgeOfPlatform);

		if (enemyAI.enemyInfo.IsOnEdgeOfPlatform || patrolInfo.JustHitWall) 
		{
			//Debug.Log ("here");
			enemyAI.velocity.x *= -1f;
		}
		Debug.Log (patrolInfo.JustHitWall);
		//Debug.Log (enemyAI.controller.collisions.belowLeft + ", " + enemyAI.controller.collisions.belowLeftPrev);

		//enemyAI.controller.Move (enemyAI.velocity * Time.deltaTime);

		patrolInfo.Reset ();
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	public struct EnemyPatrolInfo
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
