using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour
{
	public Vector3 playerSpawnPosition; //This is where we spawn the player when they die. At start it's set to player position.

	public float xMin, xMax;
	public float yMin, yMax;

	public List<GameObject> enemies;
	public List<GameObject> enemiesPrev;

	void Start()
	{
		InitializeLists ();
		Debug.Log (enemies.Count);
		GameManager.instance.level = this.gameObject;
		if (GameManager.instance.player != null) 
		{
			GameManager.instance.StorePlayerInfo ();
			GameManager.instance.playerStartingGun = GameManager.instance.player.GetComponent<Player>().currentGun;
		}

		Debug.Log ("Start");
	}

	void Update()
	{
		enemiesPrev = enemies;
		enemies = enemies.Where (gameObject => gameObject != null).ToList();	
		enemies = enemies.Where (gameObject => gameObject.activeSelf == true).ToList ();
	}

	private void InitializeLists()
	{
		for (int i = 0; i < transform.FindChild("Enemies").transform.childCount; i++)
		{
			enemies.Add (transform.FindChild ("Enemies").GetChild (i).gameObject);
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;

		//Horiz lines.
		Gizmos.DrawLine (new Vector3 (xMin, yMax), new Vector3 (xMax, yMax));
		Gizmos.DrawLine (new Vector3 (xMin, yMin), new Vector3 (xMax, yMin));

		//Vert lines.
		Gizmos.DrawLine (new Vector3 (xMin, yMin), new Vector3 (xMin, yMax));
		Gizmos.DrawLine (new Vector3 (xMax, yMin), new Vector3 (xMax, yMax));

		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube (playerSpawnPosition, new Vector3 (.5f, 1.4f, 0f));	
	}
}
