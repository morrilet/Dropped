using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour 
{
	GameObject spawnPoint;

	//Here's how I'll do moving checkpoints: Trigger enter -- store player info and remove collider; Update -- change spawn point while keeping same offset from checkpoint transform position.; 
	public bool isMovingCheckpoint = false; //Whether or not the spawn point/checkpoint object should move.

	private bool triggered = false; //Whether or not this trigger has been activated.
	private Vector3 spawnPointOffset; //The difference between the checkpoint trigger transform and the spawn point transform.

	void Start()
	{
		spawnPoint = transform.GetChild (0).gameObject;

		spawnPointOffset.x = Mathf.Abs (transform.position.x - spawnPoint.transform.position.x);
		spawnPointOffset.y = Mathf.Abs (transform.position.y - spawnPoint.transform.position.y);
		spawnPointOffset.z = 0f;
	}

	void Update()
	{
		if (triggered && isMovingCheckpoint) 
		{
			Debug.Log ("Here -- Checkpoint Update");
			//Issue here is that this would overwrite other checkpoints after this one... Luckily, this will be the last for the level I think. Still a cheap fix...
			spawnPoint.transform.position = transform.position + spawnPointOffset;
			GameManager.instance.level.GetComponent<Level> ().playerSpawnPosition = spawnPoint.transform.position;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
		{
			triggered = true;

			if (!isMovingCheckpoint)
			{
				Debug.Log ("Here -- Stationary Checkpoint");
				GameManager.instance.level.GetComponent<Level> ().playerSpawnPosition = spawnPoint.transform.position;
				GameManager.instance.StorePlayerInfo ();

				Destroy (this.GetComponent<Checkpoint> ());
			} 
			else if (isMovingCheckpoint)
			{
				Debug.Log ("Here -- Moving Checkpoint");
				GameManager.instance.level.GetComponent<Level> ().playerSpawnPosition = spawnPoint.transform.position;
				GameManager.instance.StorePlayerInfo ();

				Destroy (this.GetComponent<Collider2D> ());
			}
		}
	}
}
