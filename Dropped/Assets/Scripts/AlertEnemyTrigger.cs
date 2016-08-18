using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlertEnemyTrigger : MonoBehaviour 
{
	public Collider2D triggerObject; //The object we're checking for collisions with.
	public List<EnemyAI> enemiesToAlert; //The enemies we're going to alert.

	public bool freezeEnemiesBeforeAlerted; //Whether or not to freeze enemy movement before they're triggered.

	private List<float> enemiesStartingSpeed; //The starting speeds of the enemies. Used if we freeze them before alerting them.

	void Start()
	{
		enemiesStartingSpeed = new List<float> ();

		if (freezeEnemiesBeforeAlerted) 
		{
			foreach (EnemyAI enemy in enemiesToAlert) 
			{
				enemiesStartingSpeed.Add (enemy.patrolSpeed);
				enemy.patrolSpeed = 0f;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject == triggerObject.gameObject)
			AlertEnemies ();
	}

	private void AlertEnemies()
	{
		for (int i = 0; i < enemiesToAlert.Count; i++)
		{
			if (freezeEnemiesBeforeAlerted) 
			{
				enemiesToAlert [i].patrolSpeed = enemiesStartingSpeed [i];
			}
			enemiesToAlert [i].playerDetected = true;
		}
	}
}
