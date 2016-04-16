using UnityEngine;
using System.Collections;

public class EnemyPatrolState : StateMachineBehaviour 
{
	EnemyAI enemyAI;

	Vector3 velocity;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		enemyAI = animator.gameObject.GetComponent<EnemyAI> ();

		velocity = Vector3.zero;
		velocity.x = enemyAI.speed;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		if (enemyAI.controller.collisions.left && !enemyAI.controller.collisions.leftPrev)
			enemyAI.enemyInfo.JustHitWall = true;
		if (enemyAI.controller.collisions.right && !enemyAI.controller.collisions.rightPrev)
			enemyAI.enemyInfo.JustHitWall = true;

		if (!enemyAI.controller.collisions.belowLeft && enemyAI.controller.collisions.belowLeftPrev)
			enemyAI.enemyInfo.IsOnEdgeOfPlatform = true;
		if (!enemyAI.controller.collisions.belowRight && enemyAI.controller.collisions.belowRightPrev)
			enemyAI.enemyInfo.IsOnEdgeOfPlatform = true;
		
		enemyAI.controller.Move ((velocity + enemyAI.velocity) * Time.deltaTime);

		if(enemyAI.enemyInfo.IsOnEdgeOfPlatform)
			Debug.Log (enemyAI.enemyInfo.IsOnEdgeOfPlatform);

		if (enemyAI.enemyInfo.IsOnEdgeOfPlatform || enemyAI.enemyInfo.JustHitWall) 
		{
			Debug.Log ("here");
			velocity.x *= -1f;
		}

		enemyAI.enemyInfo.Reset ();
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
