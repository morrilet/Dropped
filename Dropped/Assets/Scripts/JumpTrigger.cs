using UnityEngine;
using System.Collections;

public class JumpTrigger : MonoBehaviour 
{
	public bool useDuringPatrol; //Whether or not we should jump during patrol or just chase.
	public int direction; //1 = right, -1 = left, 0 = both.

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Enemy") 
		{
			EnemyAI enemy = other.gameObject.GetComponent<EnemyAI> ();
			if (direction == enemy.direction || direction == 0) 
			{
				if (enemy.currentState == EnemyAI.States.ChasePlayer)
					enemy.jumpTriggered = true;
				else if (enemy.currentState == EnemyAI.States.Patrol && useDuringPatrol)
					enemy.jumpTriggered = true;
				else
					enemy.jumpTriggered = false;
			}
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.tag == "Enemy") 
		{
			EnemyAI enemy = other.gameObject.GetComponent<EnemyAI> ();
			if (direction == enemy.direction || direction == 0) 
			{
				if (enemy.currentState == EnemyAI.States.ChasePlayer) 
					enemy.jumpTriggered = true;
				else if (enemy.currentState == EnemyAI.States.Patrol && useDuringPatrol) 
					enemy.jumpTriggered = true;
				else
					enemy.jumpTriggered = false;
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Enemy") 
		{
			other.gameObject.GetComponent<EnemyAI> ().jumpTriggered = false;
		}
	}
}
