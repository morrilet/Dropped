using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour 
{
	public float xMin, xMax;
	public float yMin, yMax;

	List<GameObject> grounds;
	List<GameObject> hotspots;
	List<GameObject> movingPlatforms;
	List<GameObject> obstacles;

	void Start()
	{
		InitializeLists ();
	}

	private void InitializeLists()
	{
		grounds = new List<GameObject> ();
		hotspots = new List<GameObject> ();
		movingPlatforms = new List<GameObject> ();
		obstacles = new List<GameObject> ();

		foreach(Transform child in GameObject.Find("Grounds").transform)
		{
			grounds.Add(child.gameObject);
		}
		foreach(Transform child in GameObject.Find("Hotspots").transform)
		{
			hotspots.Add(child.gameObject);
		}
		foreach(Transform child in GameObject.Find("Moving Platforms").transform)
		{
			movingPlatforms.Add(child.gameObject);
		}
		foreach(Transform child in GameObject.Find("Obstacles").transform)
		{
			obstacles.Add(child.gameObject);
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
	}
}
