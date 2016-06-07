using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour 
{
	GameObject spawnPoint;

	void Start()
	{
		spawnPoint = transform.GetChild (0).gameObject;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
		{
			Debug.Log ("Here");
			GameManager.instance.level.GetComponent<Level> ().playerSpawnPosition = spawnPoint.transform.position;
			GameManager.instance.StorePlayerInfo ();

			Destroy (this.GetComponent<Checkpoint> ());
		}
	}
}
